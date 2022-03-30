namespace NCLCore
{
    public class DownloadItem
    {
        public string uri;

        public string fullname;

        public DownloadItem(string uri, string dir, string name)
        {
            this.uri = uri;
            this.fullname = dir + "\\" + name;

        }
        public DownloadItem(string uri, string fullname)
        {
            this.uri = uri;
            this.fullname = fullname;
        }
    }
}
