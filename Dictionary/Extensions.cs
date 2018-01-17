using System.IO;
using System.Xml.Serialization;

namespace CourseProject.Dictionary
{
    internal static class Extensions
    {
        public static string SerializeObjectToXmlString<T>(this T obj)
        {
            using (var textWriter = new StringWriter())
            {
                new XmlSerializer(typeof(T)).Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }
    }
}
