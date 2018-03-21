using System;
using System.Collections.Generic;

namespace CourseProject.Dictionary
{
    public sealed class CompositeKey<TId, TName> :  IEquatable<CompositeKey<TId, TName>>
    {
        public TId Id { get; }
        public TName Name { get; }

        public CompositeKey(TId id, TName name)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Id = id;
            Name = name;
        }

        public bool Equals(CompositeKey<TId, TName> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id.Equals(other.Id) && Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CompositeKey<TId, TName> key && Equals(key);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Id.GetHashCode() * 397 ^ Name.GetHashCode();
            }
        }
    }
}
