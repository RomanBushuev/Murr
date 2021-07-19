using KarmaCore.BaseTypes;
using KarmaCore.BaseTypes.Logger;
using KarmaCore.BaseTypes.MurrEvents;
using KarmaCore.Enumerations;
using KarmaCore.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text;

namespace TestFullSolutions.Core
{
    [TestClass]
    public class TestXmlParse
    {
        string root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        [TestMethod]
        public void ParseMoexFile()
        {
            string moexDocument = @"\Files\MoexDownload.xml";            
            string fullpath = root + moexDocument;
            Assert.IsTrue(File.Exists(fullpath));
            var xmlDocument = XDocument.Load(fullpath);
            var result = ParseXmlStructure.GetDocument(xmlDocument);
            Assert.AreEqual(2, result.Datas.Count);
            Assert.AreEqual("TQBR", result.Datas.First().Rows.Rowss.OfType<MoexShareDataRow>().First().Boardid);
        }

        [TestMethod]
        public void ParseCbrExchangeFileToSave()
        {
            string moexDocument = @"\Files\CbrExchangeToSave.xml";
            string fullpath = root + moexDocument;
            Assert.IsTrue(File.Exists(fullpath));
            var xmlDocument = XDocument.Load(fullpath);
            var result = ParseXmlStructure.GetCurrencies(xmlDocument);
            Assert.AreEqual(new DateTime(2021, 07, 17), result.ValidDate);
            Assert.AreEqual(34, result.ValuteCursOnDates.Count);
            Assert.IsTrue(result.ValuteCursOnDates.Any(z => z.VchCode == "AUD"));
            Assert.IsTrue(result.ValuteCursOnDates.Any(z => z.VchCode == "JPY"));
        }

        [TestMethod]
        public void ParseToXml()
        {
            Person person = new Person()
            {
                Name = "Roman",
                Val = 20.9m
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Person));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            MemoryStream ms = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
            XmlWriter writer = XmlWriter.Create(ms, settings);
            xmlSerializer.Serialize(writer, person, ns);

            string str = Encoding.Unicode.GetString(ms.ToArray());
            Console.WriteLine(str);
        }
    }

    [Serializable]
    [XmlRoot("Person", Namespace ="")]
    public class Person
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Val")]
        public decimal Val { get; set; }
    }

}
