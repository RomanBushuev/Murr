using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TestFullSolutions.Core
{
    [TestClass]
    public class TestCsvReaderTest
    {
        [TestMethod]
        public void Test()
        {
            var result = new CsvReaderTest().Download();
            Assert.IsNull(result.First().Bushuev);
            Assert.AreEqual(default(decimal), result.First().BushuevDecimal);
            Assert.IsNull(result.First().BushuevDecimalNull);
        }
    }

    public class CsvReaderTest
    {
        string root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        string file = @"\Files\Values.csv";

        public CsvReaderTest()
        {

        }

        public List<ReadingDate> Download()
        {
            List<string> strings = new List<string>();
            List<string> fields = new List<string>();
            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            csvConfiguration.BadDataFound = context =>
            {
                strings.Add(context.RawRecord);
                fields.Add(context.Field);
            };

            csvConfiguration.Delimiter = ";";
            csvConfiguration.MissingFieldFound = null;
            //ShouldQuote ShouldQuote = filed => true;
            //csvConfiguration.ShouldQuote = ShouldQuote;
            csvConfiguration.Mode = CsvMode.NoEscape;

            using (StreamReader streamReader = new StreamReader(root + file))
            using (var csv = new CsvReader(streamReader,csvConfiguration))
            {
                csv.Context.RegisterClassMap<ReadingDateMap>();
                //csv.Configuration.MissingFieldFound = null;
                var options = new TypeConverterOptions { Formats = new[] { "dd.MM.yyyy" } };
                var numberOptions = new TypeConverterOptions { NumberStyles = NumberStyles.Any };
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<decimal>(numberOptions);
                csv.Context.TypeConverterOptionsCache.AddOptions<decimal?>(numberOptions);

                var restul = csv.GetRecords<ReadingDate>().ToList();

                var headears = csv.HeaderRecord;
                var members = new ReadingDateMap().MemberMaps.Select(z => z.Data);

                return restul;                
            }
        }
    }

    public class ReadingDateMap : ClassMap<ReadingDate>
    {
        public ReadingDateMap()
        {
            Map(m => m.NumberEmpty).Name("numberEmpty").Optional();
            Map(m => m.Number).Name("number").Optional();
            Map(m => m.Text).Name("text");
            Map(m => m.Date).Name("date");
            Map(m => m.DateEmpty).Name("dateEmpty");
            Map(m => m.IsT).Name("isT");//.TypeConverter<MyBooleanConverter>();
            Map(m => m.Bushuev).Name("Bushuev").Optional();
            Map(m => m.BushuevDecimal).Name("BushuevDecimal").Optional();
            Map(m => m.BushuevDecimalNull).Name("BushuevDecimalNull").Optional();
        }
    }

    public class MyBooleanConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text == "0")
                return false;
            else
                return true;
        }
    }

    public class ReadingDate
    {
        public decimal? NumberEmpty { get; set; }

        public decimal Number { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public DateTime? DateEmpty { get; set; }
        public bool IsT { get; set; }

        public string Bushuev { get; set; }

        public decimal? BushuevDecimalNull { get; set; }

        public decimal BushuevDecimal { get; set; }
    }

}
