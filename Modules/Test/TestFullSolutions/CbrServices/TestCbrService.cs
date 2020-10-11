using KarmaCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using DownloaderProvider;
using KarmaCore.BaseTypes;
using KarmaCore.Enumerations;
using KarmaCore.Interfaces;
using System.Reflection;
using System.IO;
using DownloadTasks;

namespace TestFullSolutions.CbrServices
{
    [TestClass]
    public class TestCbrService
    {
        [TestMethod]
        public void TestDownloadRuonia()
        {
            Calculation calculation = new DownloadForeignExchange();
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadForeignExchange.RunDateTime,
                Value = new DateTime(2020, 10, 05),
                ParamType = ParamType.DateTime
            });

            calculation.Run();

            if (calculation as IXmlResult != null)
            {
                IXmlResult xmlResult = (IXmlResult)calculation;
                
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                string newPath = Path.GetDirectoryName(path);

                string guid = $"{Guid.NewGuid()}.xml";
                XmlSaver.XmlSaver xmlSaver = new XmlSaver.XmlSaver();
                xmlSaver.Connection = newPath + "\\TempFiles\\" + guid;
                xmlSaver.IsReplaced = true;
                xmlSaver.XmlResult = xmlResult;
                bool isSaved = xmlSaver.Save();

                Assert.IsTrue(isSaved);
            }
        }
    }
}
