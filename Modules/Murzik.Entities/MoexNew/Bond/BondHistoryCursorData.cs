namespace Murzik.Entities.MoexNew.Bond
{
    public class BondHistoryCursorData
    {
        public BondHistoryBondDataRow[] Rows { get; set; }

        public string Id { get; set; } = "history.cursor";
    }
}