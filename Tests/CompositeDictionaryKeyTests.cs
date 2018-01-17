using CourseProject.Dictionary;
using CourseProject.UserTypes;

using NUnit.Framework;

namespace CourseProject.Tests
{
    [TestFixture]
    internal sealed class CompositeDictionaryKeyTests
    {
        [Test]
        public void EqualsObjectEqualTest()
        {
            var firstObject = new CompositeDictionaryKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserName = "12345" } });
            var secondObject = new CompositeDictionaryKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserName = "12345" } });
            
            Assert.That(firstObject.Equals(secondObject), Is.True);
        }

        [Test]
        public void DifferentObjectEqualTest()
        {
            var firstObject = new CompositeDictionaryKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserName = "12345" } });
            var secondObject = new CompositeDictionaryKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "12345" }, new UserTypeB { Level = "12345", FriendlyUserTypeA = new UserTypeA { UserName = "1234" } });

            Assert.That(firstObject.Equals(secondObject), Is.False);
        }

        [Test]
        public void EqualsObjectGetHashCodeTest()
        {
            var firstObject = new CompositeDictionaryKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserName = "12345" } });
            var secondObject = new CompositeDictionaryKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserName = "12345" } });

            Assert.That(firstObject.GetHashCode() == secondObject.GetHashCode());
        }

        [Test]
        public void DifferentObjectGetHashCodeTest()
        {
            var firstObject = new CompositeDictionaryKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "123456" }, new UserTypeB { Level = "123456", FriendlyUserTypeA = new UserTypeA { UserName = "12345" } });
            var secondObject = new CompositeDictionaryKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "12345" }, new UserTypeB { Level = "12345", FriendlyUserTypeA = new UserTypeA { UserName = "1234" } });

            Assert.That(firstObject.GetHashCode() != secondObject.GetHashCode());
        }
    }
}
