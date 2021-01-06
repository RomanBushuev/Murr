using KarmaCore.Entities;
using KarmaCore.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace XmlSaver
{
    public class XmlSaver : IXmlSaver
    {
        public bool Save()
        {
            if (!string.IsNullOrEmpty(Connection) && XmlResult != null)
            {
                try
                {
                    string path = Path.GetDirectoryName(Connection);
                    if(!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    if (File.Exists(Connection))
                    {
                        File.Delete(Connection);
                    }
                    
                    XmlResult.Document.Save(Connection);
                    return true;
                }
                catch(Exception ex)
                {
                    //надо будет сделать нотифайер
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public XmlSaver()
        {

        }

        public string Connection { get; set; } = null;
        public IXmlResult XmlResult { get; set; } = null;
        public bool IsReplaced { get; set; } = false;

        public static XmlSaver Deserialize(SaverJson saverJson)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(saverJson.JsonParameters);
            XmlSaver xmlSaver = new XmlSaver();
            if(dict.ContainsKey("Path"))
            {
                var connection = dict["Path"];
                xmlSaver.Connection = connection;
            }

            if(dict.ContainsKey("IsReplaced"))
            {
                var isReplaced = bool.Parse(dict["IsReplaced"]);
                xmlSaver.IsReplaced = isReplaced;
            }

            return xmlSaver;
        }
    }
}
