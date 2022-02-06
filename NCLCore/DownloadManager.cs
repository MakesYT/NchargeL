using Downloader;
using log4net;

namespace NCLCore
{
    internal class DownloadManager
    {
        private static readonly ILog log = LogManager.GetLogger("DownloadManager");
        List<DownloadItem> Hashs = new List<DownloadItem>();
        public SDK sDK;
        public void Add(DownloadItem di)
        {
            Hashs.Add(di);
        }
        int nowthreadnum = 0;
        public void Start(int thread)
        {
            
            log.Debug(Hashs.Count);
            while (Hashs.Count != 0|| nowthreadnum!=0)
                while (nowthreadnum < thread)
                {
                    if (Hashs.Count > 0)
                    {
                        DownloadItem hash = Hashs.First();
                        Hashs.Remove(hash);
                        Thread t5 = new Thread(() => DownloadTool(Hashs.Count+1,hash));
                        t5.IsBackground = true;
                        t5.Name = Hashs.Count.ToString();
                        t5.Start();
                        nowthreadnum++;
                        Thread.Sleep(10);
                        
                    }
                    else if (nowthreadnum == 0) break;
                }
        }

        private void DownloadTool(int name,DownloadItem hash)
        {
            
           
            log.Debug(hash.dir);
            log.Debug(Path.GetDirectoryName(hash.dir));
            DownloadBuilder.New()
            .WithUrl(hash.uri)
            .WithFileLocation(hash.dir)
            .WithConfiguration(new DownloadConfiguration() { Timeout = 5000,  ParallelDownload = true })
            .Build()
            .StartAsync().Wait();
            if(name%100==0)
            sDK.info = new Info(name.ToString(), "info");
            //downloader.DownloadFileTaskAsync(url, dir).Wait();
            nowthreadnum--;
            
            

        }



    }
}
