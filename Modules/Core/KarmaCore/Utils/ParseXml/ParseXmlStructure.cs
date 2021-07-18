using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KarmaCore.Utils
{
    public static partial class ParseXmlStructure
    {
        public static string ParseToXml<T>(T instance)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            MemoryStream ms = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
            XmlWriter writer = XmlWriter.Create(ms, settings);
            xmlSerializer.Serialize(writer, instance, ns);

            string str = Encoding.Unicode.GetString(ms.ToArray());
            return str;
        }

        public static XElement GetXElement(XDocument xDocument, string attriubte)
        {
            return xDocument.Element(attriubte);
        }

        public static XElement GetXElement(XElement xElement, string attribute)
        {
            return xElement.Element(attribute);
        }

        public static IEnumerable<XElement> GetXElements(XElement xElement, string attribute)
        {
            return xElement.Elements(attribute);
        }   
        
        public static XElement GetXElement(XElement xDocument, List<string> attriubtes)
        {
            XElement element = null;
            foreach (var attriubte in attriubtes)
            {
                element = GetXElement(xDocument, attriubte);
            }
            return element;
        }
    }
}
