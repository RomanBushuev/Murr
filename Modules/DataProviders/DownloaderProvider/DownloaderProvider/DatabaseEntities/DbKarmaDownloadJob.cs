namespace DownloaderProvider.DatabaseEntities
{
    public class DbKarmaDownloadJob
    {
        public long TaskId { get; set; }
        public long TaskTemplateId { get; set; }
        public long TaskStatusId { get; set; }
        public long SaverTemplateId { get; set; }
    }
}
