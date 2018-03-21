using System;

namespace CourseProject.UserTypes
{
    public class UserTypeA : IEquatable<UserTypeA>
    {
        public long UserId;
        public string UserName;
        
        public bool Equals(UserTypeA other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return UserId == other.UserId && string.Equals(UserName, other.UserName);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserTypeA) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (UserId.GetHashCode() * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
            }
        }

        public static bool operator ==(UserTypeA left, UserTypeA right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserTypeA left, UserTypeA right)
        {
            return !Equals(left, right);
        }
    }
}
