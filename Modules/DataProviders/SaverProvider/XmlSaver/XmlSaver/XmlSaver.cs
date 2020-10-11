using KarmaCore.Interfaces;
using System;
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
    }
}
