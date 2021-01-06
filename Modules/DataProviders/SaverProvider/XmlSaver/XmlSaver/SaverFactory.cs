using KarmaCore.Entities;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSaver
{
    public class SaverFactory
    {
        public static ISaver GetXmlSaver(SaverJson json, IResult result)
        {
            if(json.SaverType == (long)SaverTypes.Xml)
            {
                IXmlResult xmlResult = result as IXmlResult;
                var xmlSaver = XmlSaver.Deserialize(json);
                xmlSaver.XmlResult = xmlResult;

                return xmlSaver;
            }

            return null;
        }
    }
}
