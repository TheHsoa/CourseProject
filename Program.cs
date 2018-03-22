using System;

using CourseProject.Dictionary;
using CourseProject.UserTypes;

namespace CourseProject
{
    internal class Program
    {
        private static void Main()
        {
            var firstUser = new UserTypeA {UserId = 1, UserName = "FirstUserName"};
            var secondUser = new UserTypeA { UserId = 2, UserName = "SecondUserName" };
            var thirdUser = new UserTypeA { UserId = 3, UserName = "ThirdUserName" };

            var a = new CompositeKey<UserTypeA, UserTypeB>(firstUser, new UserTypeB { Level = "First", FriendlyUserTypeA = secondUser });
            var b = new CompositeKey<UserTypeA, UserTypeB>(secondUser, new UserTypeB { Level = "First", FriendlyUserTypeA = firstUser });
            var e = new CompositeKey<UserTypeA, UserTypeB>(firstUser, new UserTypeB { Level = "Second", FriendlyUserTypeA = thirdUser });

            var c = new CompositeDictionary<UserTypeA, UserTypeB, string>
                        {
                            { e.Id, e.Name, "One" }
                        };

            c.Add(a.Id, a.Name, "Two");
            c[b.Id, b.Name] = "Three";

            c[a.Id, a.Name] = "Four";

            foreach (var variable in c)
            {
                Console.WriteLine($"Id.UserName:{variable.Key.Id.UserName};{Environment.NewLine}"
                    + $"Name.FriendlyUserTypeA.UserName: {variable.Key.Name.FriendlyUserTypeA.UserName}, Name.Level: {variable.Key.Name.Level};{Environment.NewLine}"
                    + $"Value: {variable.Value}{Environment.NewLine}{Environment.NewLine}");
            }

            Console.ReadKey();
        }
    }
}
