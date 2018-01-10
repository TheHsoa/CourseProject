using System;

namespace CourseProject.Dictionary
{
    public class CompositeDictionaryKey<TId, TName> : IEquatable<CompositeDictionaryKey<TId, TName>>
    {
        public TId Id { get; }
        public TName Name { get; }

        private string _serializedId;
        private string _serializedName;

        public string SerializedId => _serializedId ?? (_serializedId = Id.SerializeObjectToXmlString());
        public string SerializedName => _serializedName ?? (_serializedName = Name.SerializeObjectToXmlString());

        public CompositeDictionaryKey(TId id, TName name)
        {
            Name = name;
            Id = id;
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
