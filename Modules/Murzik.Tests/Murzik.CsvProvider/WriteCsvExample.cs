using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Murzik.Tests.Murzik.CsvSaver
{
    public class WriteCsvExample
    {
        public class TestClass
        {
            public string Text { get; set; }

            public DateTime? Date { get; set; }

            public decimal? Number { get; set; }

            public long? Id { get; set; }

            public DateTime DateTime { get; set; }
        }

        public class TestClassMap : ClassMap<TestClass>
        {
            public TestClassMap()
            {
                Map(m => m.Id).Name("id");
                Map(m => m.Date).Name("date");//.TypeConverterOption.Format("dd.MM.yyyy");
                Map(m => m.Number).Name("number");
                Map(m => m.Text).Name("text");
                Map(m => m.DateTime).Name("date_time");//.TypeConverterOption.Format("dd.MM.yyyy");
            }
        }

        [Fact]
        public void WriteCsvTest()
        {
            var csvPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "test.csv");
            var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                Delimiter = ";"
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var writer = new StreamWriter(csvPath, false, Encoding.GetEncoding(1251));
            using var csv = new CsvWriter(writer, csvConfiguration);
            var options = new TypeConverterOptions
            {
                Formats = new[] { "dd.MM.yyyy" },
            };
            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

            var testClasses = new List<TestClass>()
            {
                new TestClass
                {
                    Id = 1,
                    Date = new DateTime(2022, 12, 24),
                    Number = 777.666m,
                    Text = "Роман текст",
                    DateTime = DateTime.Now
                },
                new TestClass
                {
                    Id = null,
                    Date = null,
                    Number = null,
                    Text = null,
                    DateTime = DateTime.Now
                }
            };
            csv.Context.RegisterClassMap<TestClassMap>();
            csv.WriteRecords(testClasses);
            writer.Flush();            
            csv.Dispose();
            writer.Close();
            writer.Dispose();
            File.Delete(csvPath);
        }
    }
}
