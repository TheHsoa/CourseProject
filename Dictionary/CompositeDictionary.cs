using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CourseProject.Dictionary
{
    public class CompositeDictionary<TId, TName, TValue> : ICollection<KeyValuePair<CompositeKey<TId, TName>, TValue>>
    {
        private readonly Dictionary<CompositeKey<TId, TName>, TValue> fullDictionary = new Dictionary<CompositeKey<TId, TName>, TValue>();
        private readonly Dictionary<TId, List<TValue>> idDictionary = new Dictionary<TId, List<TValue>>();
        private readonly Dictionary<TName, List<TValue>> nameDictionary = new Dictionary<TName, List<TValue>>();

        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

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

                var key = new CompositeKey<TId, TName>(id, name);

                using (rwLock.UseReadLock())
                {
                    if (!fullDictionary.ContainsKey(key))
                    {
                        throw new KeyNotFoundException();
                    }

                    var value = fullDictionary[key];

                    return value;
                }
            }
            set
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

                using (rwLock.UseUpgratableReadLock())
                {
                    if (!fullDictionary.ContainsKey(key))
                    {
                        Add(id, name, value);
                    }
                    else
                    {
                        SetValue(key, value);
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<CompositeKey<TId, TName>, TValue>> GetEnumerator()
        {
            var dictionaryKeyValuePairs = fullDictionary.ToList();

            return dictionaryKeyValuePairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            Add(item.Key.Id, item.Key.Name, item.Value);
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

            var compositeKey = new CompositeKey<TId, TName>(id, name);

            using (rwLock.UseWriteLock())
            {
                if (fullDictionary.ContainsKey(compositeKey))
                {
                    throw new ArgumentException("An item with the same key has already been added.");
                }

                AddToFullDictionary(compositeKey, value);
                AddToIdDictionary(id, value);
                AddToNameDictionary(name, value);
            }
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

        private void SetValue(CompositeKey<TId, TName> key, TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (rwLock.UseWriteLock())
            {
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
                fullDictionary.ToArray().CopyTo(array, arrayIndex);
            }
        }
        public bool Remove(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            using (rwLock.UseUpgratableReadLock())
            {
                return fullDictionary.Contains(item) ? Remove(item.Key.Id, item.Key.Name) : false;
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

            var key = new CompositeKey<TId, TName>(id, name);

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

        public int Count
        {
            get
            {
                using (rwLock.UseReadLock())
                {
                    return fullDictionary.Count;
                }
            }
        }

        public bool IsReadOnly => false;

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

                var valuesById = idDictionary[id];

                return valuesById;
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

                var valuesByName = nameDictionary[name];
                return valuesByName;
            }
        }
    }
}
