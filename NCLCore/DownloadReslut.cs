namespace NCLCore
{
    public class DownloadReslut
    {
        bool allSuccess = false;
        List<DownloadItem> downloadItems = new List<DownloadItem>();

        public DownloadReslut(bool allSuccess, List<DownloadItem> downloadItems)
        {
            this.allSuccess = allSuccess;
            this.downloadItems = downloadItems;
        }

        public DownloadReslut(bool allSuccess)
        {
            this.allSuccess = allSuccess;
        }
    }
}