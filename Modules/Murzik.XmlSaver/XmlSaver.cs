using Murzik.Entities;
using Murzik.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Murzik.XmlSaver
{
    public class XmlSaver : IXmlSaver
    {
        public void Save(XmlDocument document)
        {
            string path = Path.GetDirectoryName(Connection);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (File.Exists(Connection))
            {
                File.Delete(Connection);
            }

            document.Save(Connection);
        }

        public string Connection { get; set; } = null;
        public bool IsReplaced { get; set; } = false;

        public IXmlSaver Deserialize(SaverJson saverJson)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(saverJson.JsonParameters);
            XmlSaver xmlSaver = new XmlSaver();
            if (dict.ContainsKey("Path"))
            {
                var connection = dict["Path"];
                xmlSaver.Connection = connection;
            }

            if (dict.ContainsKey("IsReplaced"))
            {
                var isReplaced = bool.Parse(dict["IsReplaced"]);
                xmlSaver.IsReplaced = isReplaced;
            }

            return xmlSaver;
        }
    }
}
