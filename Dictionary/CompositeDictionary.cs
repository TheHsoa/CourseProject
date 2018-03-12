using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CourseProject.Dictionary
{
    public class CompositeDictionary<TId, TName, TValue> : IDictionary<CompositeKey<TId, TName>, TValue>
    {
        private readonly Dictionary<CompositeKey<TId, TName>, TValue> fullDictionary = new Dictionary<CompositeKey<TId, TName>, TValue>();
        private readonly Dictionary<TId, List<TValue>> idDictionary = new Dictionary<TId, List<TValue>>();
        private readonly Dictionary<TName, List<TValue>> nameDictionary = new Dictionary<TName, List<TValue>>();

        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public int Count => fullDictionary.Count;
        public bool IsReadOnly => false;
        public ICollection<CompositeKey<TId, TName>> Keys => fullDictionary.Keys;
        public ICollection<TValue> Values => fullDictionary.Values;

        public TValue this[TId id, TName name]
        {
            get
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                using (rwLock.UseReadLock())
                {
                    if (!ContainsKey(id, name))
                    {
                        throw new KeyNotFoundException();
                    }
                    return fullDictionary[new CompositeKey<TId, TName>(id, name)];
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
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return this[key.Id, key.Name];
            }
            set
            {
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                this[key.Id, key.Name] = value;
            }
        }


        public void Add(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        private void AddToFullDictionary(CompositeKey<TId, TName> key, TValue value)
        {
            fullDictionary.Add(key, value);
        }

        private void AddToIdDictionary(TId id, TValue value)
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

        private void AddToNameDictionary(TName name, TValue value)
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
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var key = new CompositeKey<TId, TName>(id, name);
            Add(key, value);
        }

        public void Add(CompositeKey<TId, TName> key, TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (rwLock.UseWriteLock())
            {
                if (fullDictionary.ContainsKey(key))
                {
                    throw new ArgumentException("An item with the same key has already been added");
                }

                AddToFullDictionary(key, value);
                AddToIdDictionary(key.Id, value);
                AddToNameDictionary(key.Name, value);
            }
        }

        private void SetValue(TId id, TName name, TValue value)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (rwLock.UseWriteLock())
            {
                var key = new CompositeKey<TId, TName>(id, name);

                if (!fullDictionary.ContainsKey(key))
                {
                    throw new KeyNotFoundException();
                }

                var oldValue = fullDictionary[key];
                fullDictionary[key] = value;
                idDictionary[key.Id].Remove(oldValue);
                idDictionary[key.Id].Add(value);
                nameDictionary[key.Name].Remove(oldValue);
                nameDictionary[key.Name].Add(value);
            }
        }

        public IEnumerator<KeyValuePair<CompositeKey<TId, TName>, TValue>> GetEnumerator()
        {
            var dictionaryKeyValuePairs = fullDictionary.Select(keyValue =>
                new KeyValuePair<CompositeKey<TId, TName>, TValue>(keyValue.Key, keyValue.Value)).ToList();

           return dictionaryKeyValuePairs.GetEnumerator();
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
                return fullDictionary.Contains(item);
            }
        }

        public bool ContainsKey(CompositeKey<TId, TName> key)
        {
            if (key is null)
            {
                throw  new ArgumentNullException(nameof(key));
            }

            using (rwLock.UseReadLock())
            {
                return fullDictionary.ContainsKey(key);
            }
        }

        public bool ContainsKey(TId id, TName name)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (rwLock.UseReadLock())
            {
                return fullDictionary.ContainsKey(new CompositeKey<TId, TName>(id, name));
            }
        }

        public void CopyTo(KeyValuePair<CompositeKey<TId, TName>, TValue>[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException(
                    "The number of elements in the source dictionary values is greater than the available space from index to the end of the destination array.");
            }

            using (rwLock.UseReadLock())
            {
                fullDictionary.Select(x => new KeyValuePair<CompositeKey<TId, TName>, TValue>(x.Key, x.Value)).ToArray().CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(CompositeKey<TId, TName> key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (rwLock.UseWriteLock())
            {
                if (!fullDictionary.ContainsKey(key) || !idDictionary.ContainsKey(key.Id) || !nameDictionary.ContainsKey(key.Name))
                {
                    return false;
                }

                var value = fullDictionary[key];

                if (idDictionary[key.Id].Count == 1)
                {
                    idDictionary.Remove(key.Id);
                }
                else
                {
                    idDictionary[key.Id].Remove(value);
                }

                if (nameDictionary[key.Name].Count == 1)
                {
                    nameDictionary.Remove(key.Name);
                }
                else
                {
                    nameDictionary[key.Name].Remove(value);
                }

                fullDictionary.Remove(key);
            }
            return true;
        }

        public bool Remove(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            using (rwLock.UseUpgratableReadLock())
            { 
                return Contains(item) ? Remove(item.Key) : false;
            }
        }

        public bool Remove(TId id, TName name)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return Remove(new CompositeKey<TId, TName>(id, name));
        }

        public bool TryGetValue(TId id, TName name, out TValue value)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return TryGetValue(new CompositeKey<TId, TName>(id, name), out value);
        }

        public bool TryGetValue(CompositeKey<TId, TName> key, out TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (rwLock.UseReadLock())
            {
                return fullDictionary.TryGetValue(key, out value);
            }
        }

        public bool TryGetById(TId id, out List<TValue> values)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            using (rwLock.UseReadLock())
            {
                return idDictionary.TryGetValue(id, out values);
            }
        }

        public List<TValue> GetById(TId id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            using (rwLock.UseReadLock())
            {
                if (!idDictionary.ContainsKey(id))
                {
                    throw new KeyNotFoundException();
                }

                return idDictionary[id];
            }
        }

        public bool TryGetByName(TName name, out List<TValue> values)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (rwLock.UseReadLock())
            {
                return nameDictionary.TryGetValue(name, out values);
            }
        }

        public List<TValue> GetByName(TName name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (rwLock.UseReadLock())
            {
                if (!nameDictionary.ContainsKey(name))
                {
                    throw new KeyNotFoundException();
                }
                return nameDictionary[name];
            }
        }
    }
}
