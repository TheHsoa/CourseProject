using System;

namespace CourseProject.UserTypes
{
    public class UserTypeB : IEquatable<UserTypeB>
    {
        public string Level;
        public UserTypeA FriendlyUserTypeA;

        public bool Equals(UserTypeB other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Level, other.Level) && Equals(FriendlyUserTypeA, other.FriendlyUserTypeA);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserTypeB) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Level != null ? Level.GetHashCode() : 0) * 397) ^ (FriendlyUserTypeA != null ? FriendlyUserTypeA.GetHashCode() : 0);
            }
        }
    }
}
