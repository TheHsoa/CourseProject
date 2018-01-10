using System;
using System.Runtime.InteropServices;

using CourseProject.Dictionary;

namespace CourseProject
{
    class Program
    {
        static void Main()
        {
            var a = new CompositeDictionaryKey<A, B>(new A {B = "123456"}, new B {K = "123456", A = new A {B = "12345"} });
            var b = new CompositeDictionaryKey<A, B>(new A { B = "12345" }, new B { K = "123456", A = new A { B = "12345" } });

            var c = new CompositeDictionary<A, B, string>();

            c.Add(a, "12345");
            c.Add(b, "123456");

            Console.WriteLine(a.GetHashCode());
            Console.WriteLine(b.GetHashCode());
            Console.WriteLine(a.Equals(b));
            Console.ReadKey();
        }
    }

    public class A
    {
        public string B;
    }

    public class B
    {
        public string K;
        public A A;
    }
}
