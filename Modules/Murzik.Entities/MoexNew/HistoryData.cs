namespace Murzik.Entities.MoexNew
{
    public class HistoryData
    {
        public BondDataRow[] Rows { get; set; }

        public string Id { get; set; } = "history";
    }
}