namespace Murzik.Entities.MoexNew.Bond
{
    public class BondHistoryData
    {
        public BondDataRow[] Rows { get; set; }

        public string Id { get; set; } = "history";
    }
}