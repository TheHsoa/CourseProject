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

        public int Count => fullDictionary.Count;

        public bool IsReadOnly => false;

        public TValue this[TId id, TName name]
        {
            get => GetValue(new CompositeKey<TId, TName>(id, name));
            set => SetValue(new CompositeKey<TId, TName>(id, name), value);
        }

        public IEnumerator<KeyValuePair<CompositeKey<TId, TName>, TValue>> GetEnumerator()
        {
            var dictionaryKeyValuePairs = fullDictionary.ToList();

            return dictionaryKeyValuePairs.GetEnumerator();
        }

        public void Add(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TId id, TName name, TValue value)
        {
            Add(new CompositeKey<TId, TName>(id, name), value);
        }

        public void SetValue(CompositeKey<TId, TName> key, TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (rwLock.UseUpgratableReadLock())
            {
                if (!fullDictionary.ContainsKey(key))
                {
                    Add(key, value);
                }
                else
                {
                    ChangeValue(key, value);
                }
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

            using (rwLock.UseReadLock())
            {
                if (array.Length - arrayIndex < Count)
                {
                    throw new ArgumentException(message: "The number of elements in the source dictionary values is greater than the available space from index to the end of the destination array.");
                }

                foreach (var keyValue in fullDictionary)
                {
                    array[arrayIndex++] = new KeyValuePair<CompositeKey<TId, TName>, TValue>(keyValue.Key, keyValue.Value);
                }
            }
        }

        public bool Remove(KeyValuePair<CompositeKey<TId, TName>, TValue> item)
        {
            using (rwLock.UseUpgratableReadLock())
            {
                return fullDictionary.Contains(item) ? Remove(item.Key) : false;
            }
        }

        public bool Remove(TId id, TName name)
        {
            return Remove(new CompositeKey<TId, TName>(id, name));
        }

        public IReadOnlyCollection<TValue> GetById(TId id)
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

                var valuesById = idDictionary[id].ToArray();
                return valuesById;
            }
        }

        public IReadOnlyCollection<TValue> GetByName(TName name)
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

                var valuesByName = nameDictionary[name].ToArray();
                return valuesByName;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static void AddToKeyPartDictionary<TKeyPart>(IDictionary<TKeyPart, List<TValue>> dictionary, TKeyPart key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(value);
            }
            else
            {
                dictionary.Add(key,
                               new List<TValue>
                                   {
                                       value
                                   });
            }
        }

        private static void ChangeKeyPartDictionary<TKeyPart>(IDictionary<TKeyPart, List<TValue>> dictionary, TKeyPart key, TValue oldValue, TValue newValue)
        {
            dictionary[key].Remove(oldValue);
            dictionary[key].Add(newValue);
        }

        private static void RemoveFromKeyPartDictionary<TKeyPart>(IDictionary<TKeyPart, List<TValue>> dictionary, TKeyPart key, TValue value)
        {
            if (dictionary[key].Count == 1)
            {
                dictionary.Remove(key);
            }
            else
            {
                dictionary[key].Remove(value);
            }
        }

        private void Add(CompositeKey<TId, TName> key, TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Id == null)
            {
                throw new ArgumentNullException(nameof(key.Id));
            }

            if (key.Name == null)
            {
                throw new ArgumentNullException(nameof(key.Name));
            }

            using (rwLock.UseWriteLock())
            {
                if (fullDictionary.ContainsKey(key))
                {
                    throw new ArgumentException(message: "An item with the same key has already been added.");
                }

                AddToFullDictionary(key, value);
                AddToIdDictionary(key.Id, value);
                AddToNameDictionary(key.Name, value);
            }
        }

        private void AddToFullDictionary(CompositeKey<TId, TName> key, TValue value)
        {
            fullDictionary.Add(key, value);
        }

        private void AddToIdDictionary(TId id, TValue value)
        {
            AddToKeyPartDictionary(idDictionary, id, value);
        }

        private void AddToNameDictionary(TName name, TValue value)
        {
            AddToKeyPartDictionary(nameDictionary, name, value);
        }

        private void ChangeValue(CompositeKey<TId, TName> key, TValue value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Id == null)
            {
                throw new ArgumentNullException(nameof(key.Id));
            }

            if (key.Name == null)
            {
                throw new ArgumentNullException(nameof(key.Name));
            }

            using (rwLock.UseWriteLock())
            {
                var oldValue = fullDictionary[key];
                fullDictionary[key] = value;
                ChangeKeyPartDictionary(idDictionary, key.Id, oldValue, value);
                ChangeKeyPartDictionary(nameDictionary, key.Name, oldValue, value);
            }
        }

        private bool Remove(CompositeKey<TId, TName> key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Id == null)
            {
                throw new ArgumentNullException(nameof(key.Id));
            }

            if (key.Name == null)
            {
                throw new ArgumentNullException(nameof(key.Name));
            }

            using (rwLock.UseWriteLock())
            {
                if (!fullDictionary.ContainsKey(key) || !idDictionary.ContainsKey(key.Id) || !nameDictionary.ContainsKey(key.Name))
                {
                    return false;
                }

                var value = fullDictionary[key];

                RemoveFromKeyPartDictionary(idDictionary, key.Id, value);
                RemoveFromKeyPartDictionary(nameDictionary, key.Name, value);

                fullDictionary.Remove(key);
            }

            return true;
        }

        private TValue GetValue(CompositeKey<TId, TName> key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Id == null)
            {
                throw new ArgumentNullException(nameof(key.Id));
            }

            if (key.Name == null)
            {
                throw new ArgumentNullException(nameof(key.Name));
            }

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
    }
}