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
using KarmaCore.Calculations;
using KarmaCore.Calculations.Saver;
using AutoMapper;
using Microsoft.Extensions.Configuration;

namespace TestFullSolutions.CbrServices
{
    [TestClass]
    public class TestDownloads
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

        [TestMethod]
        public void TestDownloadG2()
        {
            Calculation calculation = new DownloadG2();
            calculation.SetParamDescriptors(new ParamDescriptor()
            { 
                Ident = DownloadG2.RunDateTime,
                Value = new DateTime(2017, 11, 15),
                ParamType = ParamType.DateTime
            });

            calculation.Run();

            if(calculation as IXmlResult != null)
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

        [TestMethod]
        public void TestDownloadMoexShares()
        {
            Calculation calculation = new DownloadMoexInstruments();
            calculation.SetParamDescriptors(new ParamDescriptor()
            { 
                Ident = DownloadMoexInstruments.RunDateTime,
                Value = new DateTime(2021, 02, 09),
                ParamType = ParamType.DateTime
            });
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMoexInstruments.InstrumentType,
                Value = "shares",
                ParamType = ParamType.String
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

        /// <summary>
        /// Что-то в данных должно быть
        /// </summary>
        [TestMethod]
        public void TestDownloadMoexBonds()
        {
            Calculation calculation = new DownloadMoexInstruments();
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMoexInstruments.RunDateTime,
                Value = new DateTime(2021, 02, 09),
                ParamType = ParamType.DateTime
            });
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMoexInstruments.InstrumentType,
                Value = "bonds",
                ParamType = ParamType.String
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

        /// <summary>
        /// Что-то в данных должно быть
        /// </summary>
        [TestMethod]
        public void TestDownloadMoexBondsInHolidays()
        {
            Calculation calculation = new DownloadMoexInstruments();
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMoexInstruments.RunDateTime,
                Value = new DateTime(2021, 02, 09),
                ParamType = ParamType.DateTime
            });
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMoexInstruments.InstrumentType,
                Value = "bonds",
                ParamType = ParamType.String
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

        [TestMethod]
        public void TestDownloadMoexIndex()
        {
            Calculation calculation = new DownloadMoexInstruments();
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMoexInstruments.RunDateTime,
                Value = new DateTime(2021, 02, 09),
                ParamType = ParamType.DateTime
            });
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMoexInstruments.InstrumentType,
                Value = "index",
                ParamType = ParamType.String
            });

            calculation.Run();
        }

        [TestMethod]
        public void TestDownloadMosPrime()
        {
            Calculation calculation = new DownloadMosprime();
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMosprime.RunDateTime,
                Value = new DateTime(2020, 11, 10),
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

        [TestMethod]
        public void TestDownloadKeyRate()
        {
            Calculation calculation = new DownloadKeyRate();
            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = DownloadMosprime.RunDateTime,
                Value = new DateTime(2020, 11, 10),
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

        [TestMethod]
        public void TestSaverForeignExchange()
        {
            var karmaSaverConnection = GetStringConnectionKarmaSaver();
            var config = AutoMapperConfiguration.Configure();
            IMapper mapper = config.CreateMapper();            

            Calculation calculation = new SaveForeignExchange(new CbrRepository(), new MarkerRepository(karmaSaverConnection,
                    new FinInstrumentRepository(mapper),
                    new FinDataSourceRepository(mapper)));

            string root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            string moexDocument = @"\Files\CbrExchangeToSave.xml";
            string fullpath = root + moexDocument;

            calculation.SetParamDescriptors(new ParamDescriptor()
            {
                Ident = SaveForeignExchange.Document,
                Value = fullpath,
                ParamType = ParamType.String
            });

            calculation.Run();
        }

        private string GetStringConnectionKarmaSaver()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            if (!config.GetSection("DataProviders").Exists())
                throw new Exception("Отсутствует DataProviders");

            var npgConnection = config["DataProviders:KarmaSaver"];

            return npgConnection;
        }
    }
}
