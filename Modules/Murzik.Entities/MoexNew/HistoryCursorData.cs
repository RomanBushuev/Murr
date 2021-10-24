namespace Murzik.Entities.MoexNew
{
    public class HistoryCursorData
    {
        public HistoryBondDataRow[] Rows { get; set; }

        public string Id { get; set; } = "history.cursor";
    }
}