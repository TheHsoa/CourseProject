using System;

namespace CourseProject.Dictionary
{
    internal class CompositeDictionaryKey<TId, TName> : IEquatable<CompositeDictionaryKey<TId, TName>>
    {
        public CompositeKey<TId, TName> Key { get; }

        private string serializedId;
        private string serializedName;

        public string SerializedId => serializedId ?? (serializedId = Key.Id.SerializeObjectToXmlString());
        public string SerializedName => serializedName ?? (serializedName = Key.Name.SerializeObjectToXmlString());

        public CompositeDictionaryKey(CompositeKey<TId, TName> key)
        {
            Key = key;
        }

        public CompositeDictionaryKey(TId id, TName name)
        {
            Key = new CompositeKey<TId, TName>(id, name);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.GetType() != GetType())
                return false;
            return Equals((CompositeDictionaryKey<TId, TName>)other);
        }

        public bool Equals(CompositeDictionaryKey<TId, TName> other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return SerializedId.Equals(other.SerializedId) && SerializedName.Equals(other.SerializedName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (SerializedId.GetHashCode() * 397) ^ SerializedName.GetHashCode();
            }
        }
    }
}
