namespace Murzik.Entities.MoexNew.Share
{
    public class ShareHistoryData
    {
        public ShareDataRow[] Rows { get; set; }

        public string Id { get; set; } = "history";
    }
}