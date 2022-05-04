namespace NCLCore;

public class DownloadReslut
{
    private bool allSuccess;
    private List<DownloadItem> downloadItems = new();

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