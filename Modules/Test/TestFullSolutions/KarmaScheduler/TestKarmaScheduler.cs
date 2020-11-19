﻿using KarmaCore.BaseTypes.Logger;
using KarmaCore.BaseTypes.MurrEvents;
using KarmaCore.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduleProvider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestFullSolutions.KarmaScheduler
{
    [TestClass]
    public class TestKarmaScheduler
    {
        [TestMethod]
        public void TestUtilsMakeNextDate()
        {
            //время не задано
            DateTime? date = null;
            DateTime currentDate = new DateTime(2020, 11 ,17, 12, 01, 00);
            var result = Utils.MakeNextDate(date, currentDate);
            Assert.IsTrue(result);
            
            //меньше на минуту
            date = new DateTime(2020, 11, 17, 12, 00, 00);
            result = Utils.MakeNextDate(date, currentDate);
            Assert.IsTrue(result);

            //одинаковые
            date = new DateTime(2020, 11, 17, 12, 01, 00);
            result = Utils.MakeNextDate(date, currentDate);
            Assert.IsTrue(result);

            //больше на секунду
            date = new DateTime(2020, 11, 17, 12, 01, 01);
            result = Utils.MakeNextDate(date, currentDate);
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void TestUtilsGetNextDateTime()
        {
            //время не задано
            DateTime? date = null;
            DateTime currentDate = new DateTime(2020, 11, 17, 12, 01, 00);
            string template = "* * * * *";
            var result = Utils.GetNextDateTime(date, currentDate, template);
            Assert.AreEqual(result, currentDate);

            //меньше на минуту
            date = new DateTime(2020, 11, 17, 12, 00, 00);
            result = Utils.GetNextDateTime(date, currentDate, template);
            Assert.AreEqual(result, currentDate.AddMinutes(1));

            //одинаковые
            result = Utils.GetNextDateTime(currentDate, currentDate, template);
            Assert.AreEqual(currentDate.AddMinutes(1), result);

            //больше на секунду
            date = new DateTime(2020, 11, 17, 12, 01, 01);
            result = Utils.GetNextDateTime(date, currentDate, template);
            Assert.AreEqual(date.Value, result);

            //30 секунд
            date = new DateTime(2020, 11, 17, 12, 00, 30);
            result = Utils.GetNextDateTime(date, currentDate, template);
            Assert.AreEqual(result, currentDate.AddMinutes(1));
        }
    }
}
