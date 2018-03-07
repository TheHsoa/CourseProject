using CourseProject.Dictionary;
using CourseProject.UserTypes;

using NUnit.Framework;

namespace CourseProject.Tests
{
    [TestFixture]
    internal sealed class CompositeKeyTests
    {
        [Test]
        public void EqualsObjectEqualTest()
        {
            var firstObject = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserId = 1, UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserId = 2, UserName = "12345" } });
            var secondObject = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserId = 1, UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserId = 2, UserName = "12345" } });
            
            Assert.That(firstObject.Equals(secondObject), Is.True);
        }

        [Test]
        public void DifferentObjectEqualTest()
        {
            var firstObject = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserId = 1, UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserId = 3, UserName = "12345" } });
            var secondObject = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserId = 2, UserName = "12345" }, new UserTypeB { Level = "12345", FriendlyUserTypeA = new UserTypeA { UserId = 4, UserName = "1234" } });

            Assert.That(firstObject.Equals(secondObject), Is.False);
        }

        [Test]
        public void EqualsObjectGetHashCodeTest()
        {
            var firstObject = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserId = 1, UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserId = 2, UserName = "12345" } });
            var secondObject = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserId = 1, UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserId = 2, UserName = "12345" } });

            Assert.That(firstObject.GetHashCode() == secondObject.GetHashCode());
        }

        [Test]
        public void DifferentObjectGetHashCodeTest()
        {
            var firstObject = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserId = 1, UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserId = 3, UserName = "12345" } });
            var secondObject = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserId = 2, UserName = "12345" }, new UserTypeB { Level = "12345", FriendlyUserTypeA = new UserTypeA { UserId = 4, UserName = "1234" } });

            Assert.That(firstObject.GetHashCode() != secondObject.GetHashCode());
        }
    }
}
