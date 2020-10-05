using KarmaCore.BaseTypes.Logger;
using KarmaCore.BaseTypes.MurrEvents;
using KarmaCore.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
