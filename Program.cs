using System;

using CourseProject.Dictionary;
using CourseProject.UserTypes;

namespace CourseProject
{
    internal class Program
    {
        private static void Main()
        {
            var a = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA {UserName = "FirstUserName"}, new UserTypeB { Level = "First", FriendlyUserTypeA = new UserTypeA {UserName = "SecondFriendlyUserName" } });
            var b = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "SecondUserName" }, new UserTypeB { Level = "First", FriendlyUserTypeA = new UserTypeA { UserName = "FirstFriendlyUserName" } });
            var e = new CompositeKey<UserTypeA, UserTypeB>(new UserTypeA { UserName = "FirstUserName" }, new UserTypeB { Level = "Second", FriendlyUserTypeA = new UserTypeA { UserName = "FirstFriendlyUserName" } });

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
