using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KarmaCore.Interfaces
{
    public interface IResult
    {
        
    }

    public interface IXmlResult : IResult
    {
        XmlDocument Document { get; set; }
    }

    public interface IMoexResult : IResult
    {

    }
}
