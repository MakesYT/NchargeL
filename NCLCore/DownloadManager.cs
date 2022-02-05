using Downloader;
using log4net;

namespace NCLCore
{
    internal class DownloadManager
    {
        private static readonly ILog log = LogManager.GetLogger("DownloadManager");
        List<string> Hashs = new List<string>();
        public string DownloadSoureURL;
        public string AssetsDir;
        public SDK sDK;
        public void Add(string hash)
        {
            Hashs.Add(hash);
        }
        int nowthreadnum = 0;
        public void Start(int thread)
        {
            
            log.Debug(Hashs.Count);
            while (Hashs.Count != 0)
                while (nowthreadnum < thread)
                {
                    if (Hashs.Count > 0)
                    {

                        Thread t5 = new Thread(() => DownloadTool(Hashs.Count));
                        t5.IsBackground = true;
                        t5.Name = Hashs.Count.ToString();
                        t5.Start();
                        nowthreadnum++;
                        Thread.Sleep(10);
                        
                    }
                    else if (nowthreadnum == 0) break;
                }
        }

        private void DownloadTool(int name)
        {
            string hash = Hashs.First();
            Hashs.Remove(hash);
            string url = DownloadSoureURL + "assets/" + hash[0] + hash[1] + "/" + hash;
            string dir = AssetsDir + "\\assets\\objects\\" + hash[0] + hash[1];
            //log.Debug(url + " \n" + dir);
            DownloadBuilder.New()
            .WithUrl(url)
            .WithDirectory(dir)
            .Build()
            .StartAsync().Wait();
            if(name%100==0)
            sDK.info = new Info(name.ToString(), "info");
            //downloader.DownloadFileTaskAsync(url, dir).Wait();
            nowthreadnum--;
            
            

        }



    }
}
