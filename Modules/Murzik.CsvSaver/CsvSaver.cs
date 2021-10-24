using CsvHelper;
using Murzik.Entities.Moex;
using Murzik.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Murzik.CsvSaver
{
    public class CsvSaver : ICsvSaver
    {
        public void Save(IReadOnlyCollection<MoexShareDataRow> moexShares, string connection)
        {
            using (var writer = new StreamWriter(connection))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(moexShares);
            }
        }

        public void Save(IReadOnlyCollection<MoexBondDataRow> moexBonds, string connection)
        {
            using (var writer = new StreamWriter(connection))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(moexBonds);
            }
        }
    }
}
