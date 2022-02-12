using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.Extensions.Options;
using Murzik.Entities.MoexNew;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Coupon;
using Murzik.Entities.MoexNew.Share;
using Murzik.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Murzik.CsvProvider
{
    public class CsvSaver : ICsvSaver
    {
        private MoexSettings _moexSettings;

        public CsvSaver(IOptions<MoexSettings> moexSettings)
        {
            _moexSettings = moexSettings.Value;
        }

        public void Save(IReadOnlyCollection<ShareDataRow> shares, string connection)
        {
            var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                Delimiter = ";"
            };
            var options = new TypeConverterOptions
            {
                Formats = new[] { "dd.MM.yyyy" },
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var writer = new StreamWriter(connection, false, Encoding.GetEncoding(1251)))
            using (var csv = new CsvWriter(writer, csvConfiguration))
            {
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.WriteRecords(shares);
            }
        }

        public void Save(IReadOnlyCollection<BondDataRow> bonds, string connection)
        {
            var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                Delimiter = ";"
            };
            var options = new TypeConverterOptions
            {
                Formats = new[] { "dd.MM.yyyy" },
            };

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var writer = new StreamWriter(connection, false, Encoding.GetEncoding(1251)))
            using (var csv = new CsvWriter(writer, csvConfiguration))
            {
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.WriteRecords(bonds);
            }
        }

        public string Save(IReadOnlyCollection<Coupon> coupons, long taskId)
        {
            var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                Delimiter = ";"
            };
            var options = new TypeConverterOptions
            {
                Formats = new[] { "dd.MM.yyyy" },
            };

            string connection = Path.Combine(_moexSettings.FolderPath, $"{taskId}.csv");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var writer = new StreamWriter(connection, false, Encoding.GetEncoding(1251)))
            using (var csv = new CsvWriter(writer, csvConfiguration))
            {
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.WriteRecords(coupons);
            }
            return connection;
        }
    }
}
