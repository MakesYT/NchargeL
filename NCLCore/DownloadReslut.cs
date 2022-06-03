namespace NCLCore;

public class DownloadReslut
{
    public bool allSuccess=false;
    public List<DownloadItem> downloadItems = new();
    public string error="";
    public DownloadReslut() 
    {

    }
    public DownloadReslut(bool allSuccess, List<DownloadItem> downloadItems,string error)
    {
        this.allSuccess = allSuccess;
        this.downloadItems = downloadItems;
        this.error = error;
    }
    public void addReslut(DownloadReslut download)
    {
        if (download.allSuccess == false)
        {
            allSuccess = false;
            downloadItems = downloadItems.Concat(download.downloadItems).ToList();
            error = error + download.error;
        }

    }
    public DownloadReslut(bool allSuccess)
    {
        this.allSuccess = allSuccess;
    }
}