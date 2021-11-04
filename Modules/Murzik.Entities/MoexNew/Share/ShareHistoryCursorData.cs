namespace Murzik.Entities.MoexNew.Share
{
    public class ShareHistoryCursorData
    {
        public ShareHistoryBondDataRow[] Rows { get; set; }

        public string Id { get; set; } = "history.cursor";
    }
}