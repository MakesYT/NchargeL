namespace NCLCore;

public class DownloadItem
{
    public string fullname;
    public string uri;

    public DownloadItem(string uri, string dir, string name)
    {
        this.uri = uri;
        fullname = dir + "\\" + name;
    }

    public DownloadItem(string uri, string fullname)
    {
        this.uri = uri;
        this.fullname = fullname;
    }
}