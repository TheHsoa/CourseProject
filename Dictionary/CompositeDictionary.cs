using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CourseProject.Dictionary
{
    public class CompositeDictionary<TId, TName, TValue> : IDictionary<CompositeKey<TId, TName>, TValue>
    {
        private readonly Dictionary<CompositeDictionaryKey<TId, TName>, TValue> fullDictionary = new Dictionary<CompositeDictionaryKey<TId, TName>, TValue>();
        private readonly Dictionary<string, List<TValue>> idDictionary = new Dictionary<string, List<TValue>>();
        private readonly Dictionary<string, List<TValue>> nameDictionary = new Dictionary<string, List<TValue>>();

        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public int Count => fullDictionary.Count;
        public bool IsReadOnly => false;
        public ICollection<CompositeKey<TId, TName>> Keys => fullDictionary.Keys.Select(key => key.Key).ToArray();
        public ICollection<TValue> Values => fullDictionary.Values;

        public TValue this[TId id, TName name]
        {
            get
            {
                using (rwLock.UseReadLock()) {
                    return fullDictionary[new CompositeDictionaryKey<TId, TName>(id, name)];
                }
            }
            set
            {
                using (rwLock.UseUpgratableReadLock())
                {
                    if (!ContainsKey(id, name))
                    {
                        Add(id, name, value);
                    }
                    else
                    {
                        SetValue(id, name, value);
                    }
                }
            }
        }

        TValue IDictionary<CompositeKey<TId, TName>, TValue>.this[CompositeKey<TId, TName> key]
        {
            get
            {
                return this[key.Id, key.Name];
            }
            set
            {
                this[key.Id, key.Name] = value;
            }
        }


        public void Add(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        private void AddToFullDictionary(CompositeDictionaryKey<TId, TName> key, TValue value)
        {
            fullDictionary.Add(key, value);
        }

        private void AddToIdDictionary(string id, TValue value)
        {
            if (idDictionary.ContainsKey(id))
            {
                idDictionary[id].Add(value);
            }
            else
            {
                idDictionary.Add(id, new List<TValue> { value });
            }
        }

        private void AddToNameDictionary(string name, TValue value)
        {
            if (nameDictionary.ContainsKey(name))
            {
                nameDictionary[name].Add(value);
            }
            else
            {
                nameDictionary.Add(name, new List<TValue> { value });
            }
        }
        public void Add(TId id, TName name, TValue value)
        {
            var key = new CompositeKey<TId, TName>(id, name);
            Add(key, value);
        }

        public void Add(CompositeKey<TId, TName> key, TValue value)
        {
            var dictionaryKey = new CompositeDictionaryKey<TId, TName>(key);

            using (rwLock.UseWriteLock())
            {
                AddToFullDictionary(dictionaryKey, value);
                AddToIdDictionary(dictionaryKey.SerializedId, value);
                AddToNameDictionary(dictionaryKey.SerializedName, value);
            }
        }

        private void SetValue(TId id, TName name, TValue value)
        {
            using (rwLock.UseWriteLock())
            {
                var key = new CompositeDictionaryKey<TId, TName>(id, name);
                var oldValue = fullDictionary[key];
                fullDictionary[key] = value;
                idDictionary[key.SerializedId].Remove(oldValue);
                idDictionary[key.SerializedId].Add(value);
                nameDictionary[key.SerializedName].Remove(oldValue);
                nameDictionary[key.SerializedName].Add(value);
            }
        }

        public IEnumerator<KeyValuePair<CompositeKey<TId, TName>, TValue>> GetEnumerator()
        {
            using (rwLock.UseReadLock())
            {
                return fullDictionary.Select(keyValue => new KeyValuePair<CompositeKey<TId, TName>, TValue>(keyValue.Key.Key, keyValue.Value)).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {

            return GetEnumerator();
        }

        public void Clear()
        {
            using (rwLock.UseWriteLock())
            {
                fullDictionary.Clear();
                idDictionary.Clear();
                nameDictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            using (rwLock.UseReadLock())
            {
                return fullDictionary.Contains(new KeyValuePair<CompositeDictionaryKey<TId, TName>, TValue>(new CompositeDictionaryKey<TId, TName>(item.Key), item.Value));
            }
        }

        public bool ContainsKey(CompositeKey<TId, TName> key)
        {
            using (rwLock.UseReadLock())
            {
                return fullDictionary.ContainsKey(new CompositeDictionaryKey<TId, TName>(key));
            }
        }

        public bool ContainsKey(TId id, TName name)
        {
            using (rwLock.UseReadLock())
            {
                return fullDictionary.ContainsKey(new CompositeDictionaryKey<TId, TName>(id, name));
            }
        }

        public void CopyTo(KeyValuePair<CompositeKey<TId, TName>, TValue>[] array, int arrayIndex)
        {
            using (rwLock.UseReadLock())
            {
                fullDictionary.Select(x => new KeyValuePair<CompositeKey<TId, TName>, TValue>(x.Key.Key, x.Value)).ToArray().CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(CompositeKey<TId, TName> key)
        {
            var fullDictionaryKey = new CompositeDictionaryKey<TId, TName>(key);

            using (rwLock.UseWriteLock())
            {
                if (!fullDictionary.ContainsKey(fullDictionaryKey) || !idDictionary.ContainsKey(fullDictionaryKey.SerializedId) || !nameDictionary.ContainsKey(fullDictionaryKey.SerializedName))
                {
                    return false;
                }

                var value = fullDictionary[fullDictionaryKey];

                if (idDictionary[fullDictionaryKey.SerializedId].Count == 1)
                {
                    idDictionary.Remove(fullDictionaryKey.SerializedId);
                }
                else
                {
                    idDictionary[fullDictionaryKey.SerializedId].Remove(value);
                }

                if (nameDictionary[fullDictionaryKey.SerializedName].Count == 1)
                {
                    nameDictionary.Remove(fullDictionaryKey.SerializedName);
                }
                else
                {
                    nameDictionary[fullDictionaryKey.SerializedName].Remove(value);
                }

                fullDictionary.Remove(fullDictionaryKey);
            }
            return true;
        }

        public bool Remove(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            return Contains(item) ? Remove(item.Key) : false;
            
        }

        public bool Remove(TId id, TName name)
        {
            return Remove(new CompositeKey<TId, TName>(id, name));
        }

        public bool TryGetValue(TId id, TName name, out TValue value)
        {
            return TryGetValue(new CompositeKey<TId, TName>(id, name), out value);
        }

        public bool TryGetValue(CompositeKey<TId, TName> key, out TValue value)
        {
            using (rwLock.UseReadLock())
            {
                return fullDictionary.TryGetValue(new CompositeDictionaryKey<TId, TName>(key), out value);
            }
        }

        public bool TryGetById(TId id, out List<TValue> values)
        {
            using (rwLock.UseReadLock())
            {
                return idDictionary.TryGetValue(id.SerializeObjectToXmlString(), out values);
            }
        }

        public List<TValue> GetById(TId id)
        {
            using (rwLock.UseReadLock())
            {
                return idDictionary[id.SerializeObjectToXmlString()];
            }
        }

        public bool TryGetByName(TName name, out List<TValue> values)
        {
            using (rwLock.UseReadLock())
            {
                return nameDictionary.TryGetValue(name.SerializeObjectToXmlString(), out values);
            }
        }

        public List<TValue> GetByName(TName name)
        {
            using (rwLock.UseReadLock())
            {
                return nameDictionary[name.SerializeObjectToXmlString()];
            }
        }
    }
}
