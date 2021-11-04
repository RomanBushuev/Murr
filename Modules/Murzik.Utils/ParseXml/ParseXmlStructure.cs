using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Murzik.Utils.ParseXml
{
    public static class ParseXmlStructure
    {
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
