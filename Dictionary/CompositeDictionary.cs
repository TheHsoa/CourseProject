using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CourseProject.Dictionary
{
    public class CompositeDictionary<TId, TName, TValue> : IDictionary<CompositeDictionaryKey<TId, TName>, TValue>
    {
        private Dictionary<CompositeDictionaryKey<TId, TName>, TValue> fullDictionary = new Dictionary<CompositeDictionaryKey<TId, TName>, TValue>();
        private Dictionary<string, List<TValue>> idDictionary = new Dictionary<string, List<TValue>>();
        private Dictionary<string, List<TValue>> nameDictionary = new Dictionary<string, List<TValue>>();

        public void Add(KeyValuePair<CompositeDictionaryKey<TId, TName>, TValue> item)
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
            var key = new CompositeDictionaryKey<TId, TName>(id, name);
            Add(key, value);
        }

        public void Add(CompositeDictionaryKey<TId, TName> key, TValue value)
        {
            AddToFullDictionary(key, value);
            AddToIdDictionary(key.SerializedId, value);
            AddToNameDictionary(key.SerializedName, value);
        }

        public IEnumerator<KeyValuePair<CompositeDictionaryKey<TId, TName>, TValue>> GetEnumerator()
        {
            return fullDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            fullDictionary.Clear();
            idDictionary.Clear();
            nameDictionary.Clear();
        }

        public bool Contains(KeyValuePair<CompositeDictionaryKey<TId, TName>, TValue> item)
        {
            return fullDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<CompositeDictionaryKey<TId, TName>, TValue>[] array, int arrayIndex)
        {
            fullDictionary.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(CompositeDictionaryKey<TId, TName> key)
        {
            if (!fullDictionary.ContainsKey(key) || !idDictionary.ContainsKey(key.SerializedId) || !nameDictionary.ContainsKey(key.SerializedName))
            {
                return false;
            }

            if (idDictionary[key.SerializedId].Count == 1)
            {
                idDictionary.Remove(key.SerializedId);
            }
            else
            {
                idDictionary[key.SerializedId].Remove(fullDictionary[key]);
            }


            if (nameDictionary[key.SerializedId].Count == 1)
            {
                nameDictionary.Remove(key.SerializedName);
            }
            else
            {
                nameDictionary[key.SerializedName].Remove(fullDictionary[key]);
            }

            fullDictionary.Remove(key);
            return true;
        }

        public bool Remove(KeyValuePair<CompositeDictionaryKey<TId, TName>, TValue> item)
        {
            return Remove(item.Key);
        }

        public int Count => fullDictionary.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(CompositeDictionaryKey<TId, TName> key)
        {
            return fullDictionary.ContainsKey(key);
        }

        public bool ContainsKey(TId id, TName name)
        {
            return fullDictionary.ContainsKey(new CompositeDictionaryKey<TId, TName>(id, name));
        }

        public bool Remove(TId id, TName name)
        {
            return Remove(new CompositeDictionaryKey<TId, TName>(id, name));
        }

        public bool TryGetValue(TId id, TName name, out TValue value)
        {
            return TryGetValue(new CompositeDictionaryKey<TId, TName>(id, name), out value);
        }

        public bool TryGetValue(CompositeDictionaryKey<TId, TName> key, out TValue value)
        {
            return fullDictionary.TryGetValue(key, out value);
        }

        public TValue this[TId id, TName name]
        {
            get
            {
                return fullDictionary[new CompositeDictionaryKey<TId, TName>(id, name)];
            }
            set
            {
                Add(id, name, value);
            }
        }

        TValue IDictionary<CompositeDictionaryKey<TId, TName>, TValue>.this[CompositeDictionaryKey<TId, TName> key]
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

        public ICollection<CompositeDictionaryKey<TId, TName>> Keys => fullDictionary.Keys;

        public ICollection<TValue> Values => fullDictionary.Values;
    }
}
