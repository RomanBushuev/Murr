using Murzik.Entities;
using System.Xml;

namespace Murzik.Interfaces
{
    public interface IXmlSaver
    {
        void Save(XmlDocument document);

        string Connection { get; set; }

        bool IsReplaced { get; set; }

        IXmlSaver Deserialize(SaverJson saverJson);
    }
}
