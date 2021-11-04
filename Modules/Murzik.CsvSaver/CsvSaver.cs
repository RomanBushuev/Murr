using CsvHelper;
using Murzik.Entities.MoexNew.Bond;
using Murzik.Entities.MoexNew.Share;
using Murzik.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Murzik.CsvSaver
{
    public class CsvSaver : ICsvSaver
    {
        public void Save(IReadOnlyCollection<ShareDataRow> shares, string connection)
        {
            using (var writer = new StreamWriter(connection))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(shares);
            }
        }

        public void Save(IReadOnlyCollection<BondDataRow> bonds, string connection)
        {
            using (var writer = new StreamWriter(connection))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(bonds);
            }
        }
    }
}
