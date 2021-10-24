using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Murzik.Utils
{
    public static class XmlExtensions
    {
        public static string SerializeToXml<T>(this T entity)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            //settings.OmitXmlDeclaration = true;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            using (var sw = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    var xmlns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                    xsSubmit.Serialize(writer, entity, xmlns);
                    return sw.ToString();
                }
            }
        }

        public static T DeserializeFromXml<T>(this string str)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(str))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        public static decimal? ToNullDecimal(this string str)
        {
            return string.IsNullOrEmpty(str) ? null : decimal.Parse(str, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        }
    }
}
