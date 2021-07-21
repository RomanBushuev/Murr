using KarmaCore.BaseTypes;
using KarmaCore.BaseTypes.Logger;
using KarmaCore.BaseTypes.MurrEvents;
using KarmaCore.Enumerations;
using KarmaCore.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestFullSolutions.Core
{
    [TestClass]
    public class TestMurrLogger
    {
        public class TestLogger
        {
            private readonly List<string> _messages = new List<string>();

            public void OnMessageLogged(object source, MurrMessageEventArgs args)
            {
                _messages.Add(args.Message);
            }

            public List<string> Messages { get { return _messages; } }
        }

        [TestMethod]
        public void TestMurrLoggerNotify()
        {
            MurrLogger murrLogger = new MurrLogger();
            TestLogger testLogger = new TestLogger();
            murrLogger.ProcessCompleted += testLogger.OnMessageLogged;

            murrLogger.Notify("Test1", MurrMessageType.Information);
            murrLogger.Notify("Test2", MurrMessageType.Information);

            Assert.AreEqual("Test1", testLogger.Messages.First());
            Assert.AreEqual("Test2", testLogger.Messages.Last());
        }

        [TestMethod]
        public void TestGetAttribute()
        {
            ServiceStatuses status = ServiceStatuses.Running;
            var attribute = status.ToDbAttribute();
            Assert.AreEqual("RUNNING", attribute);
        }

        [TestMethod]
        public void TestGetAttribute2()
        {
            ServiceStatuses status = ServiceStatuses.Running;
            var attribute = status.ToDbAttribute();
            Assert.AreEqual(status, attribute.ToEnum<ServiceStatuses>());
        }

        [TestMethod]
        public void SerializeAndDeserialize()
        {
            List<ParamDescriptor> paramDescriptors = new List<ParamDescriptor>()
            {
              new ParamDescriptor()
              {
                  Ident = "dat",
                  Description = "dat",
                  ParamType = ParamType.DateTime,
                  Value = new DateTime(2021, 01, 05)
              },
              new ParamDescriptor()
              {
                  Ident = "num",
                  Description = "num",
                  ParamType = ParamType.Decimal,
                  Value = 123456.123456m
              },
               new ParamDescriptor()
              {
                  Ident = "ident",
                  Description = "ident",
                  ParamType = ParamType.String,
                  Value = "ident"
              }
            };

            string json = paramDescriptors.SerializeJson();

            paramDescriptors = new List<ParamDescriptor>()
            {
              new ParamDescriptor()
              {
                  Ident = "dat",
                  Description = "dat",
                  ParamType = ParamType.DateTime,
                  Value = new DateTime(2020, 01, 01)
              },
              new ParamDescriptor()
              {
                  Ident = "num",
                  Description = "num",
                  ParamType = ParamType.Decimal,
                  Value = 0.1m
              },
               new ParamDescriptor()
              {
                  Ident = "ident",
                  Description = "ident",
                  ParamType = ParamType.String,
                  Value = "val"
              }
            };

            var newValuews = ParamDescriptorExtensions.DeserializeJson(json, paramDescriptors);

            Assert.AreEqual(new DateTime(2021, 01, 05).ToString(), newValuews.FirstOrDefault(z => z.Ident == "dat").Value.ToString());
            Assert.AreEqual(123456.123456m, (decimal)newValuews.FirstOrDefault(z => z.Ident == "num").Value);
            Assert.AreEqual("ident", newValuews.FirstOrDefault(z => z.Ident == "ident").Value.ToString());
        }
    }
}
