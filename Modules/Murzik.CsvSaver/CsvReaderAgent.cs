using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Murzik.CsvProvider.Mapping;
using Murzik.Entities.MoexNew.Share;

namespace Murzik.CsvProvider
{
    public class CsvReaderAgent : ICsvReaderAgent
    {
        public async Task<IReadOnlyCollection<BondDataRow>> ReadBondDataRawAsync(string filename)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var configuration = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                Delimiter = ";",
                Mode = CsvMode.NoEscape,
                TrimOptions = TrimOptions.InsideQuotes | TrimOptions.Trim,
            };

            var options = new TypeConverterOptions { Formats = new[] { "dd.MM.yyyy" } };
            using var textReader = new StreamReader(filename, Encoding.GetEncoding("windows-1251"));
            using (var csv = new CsvReader(textReader, configuration))
            {
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.Context.RegisterClassMap<BondDataRowMap>();

                var data = await csv.GetRecordsAsync<BondDataRow>().ToListAsync();
                return data;
            }
        }

        public async Task<IReadOnlyCollection<BondDescription>> ReadBondDescriptionAsync(string filename)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var configuration = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                Delimiter = ";",
                Mode = CsvMode.NoEscape,
                TrimOptions = TrimOptions.InsideQuotes | TrimOptions.Trim,
            };

            var options = new TypeConverterOptions { Formats = new[] { "dd.MM.yyyy" } };
            using var textReader = new StreamReader(filename, Encoding.GetEncoding("windows-1251"));
            for (int i = 0; i < 2; ++i)
                textReader.ReadLine();
            using (var csv = new CsvReader(textReader, configuration))
            {
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.Context.RegisterClassMap<BondDescriptionMap>();

                var data = await csv.GetRecordsAsync<BondDescription>().ToListAsync();
                return data;
            }
        }

        public async Task<IReadOnlyCollection<ShareDescription>> ReadShareDescriptions(string filename)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var configuration = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                Delimiter = ";",
                Mode = CsvMode.NoEscape,
                TrimOptions = TrimOptions.InsideQuotes | TrimOptions.Trim,
            };

            var options = new TypeConverterOptions { Formats = new[] { "dd.MM.yyyy" } };
            var numberOptions = new TypeConverterOptions() { NumberStyles = NumberStyles.Any };
            using var textReader = new StreamReader(filename, Encoding.GetEncoding("windows-1251"));
            for (int i = 0; i < 2; ++i)
                textReader.ReadLine();
            using (var csv = new CsvReader(textReader, configuration))
            {
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
                csv.Context.TypeConverterOptionsCache.AddOptions<decimal?>(numberOptions);
                csv.Context.TypeConverterOptionsCache.AddOptions<decimal>(numberOptions);

                csv.Context.RegisterClassMap<ShareDescriptionMap>();

                var data = await csv.GetRecordsAsync<ShareDescription>().ToListAsync();
                return data;
            }
        }
    }
}
