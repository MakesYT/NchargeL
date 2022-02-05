﻿using Downloader;
using log4net;

namespace NCLCore
{
    internal class DownloadManager
    {
        private static readonly ILog log = LogManager.GetLogger("DownloadManager");
        List<DownloadItem> Hashs = new List<DownloadItem>();
        public string DownloadSoureURL;
        public string AssetsDir;
        public SDK sDK;
        public void Add(DownloadItem di)
        {
            Hashs.Add(di);
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
            DownloadItem hash = Hashs.First();
            Hashs.Remove(hash);
           
            //log.Debug(url + " \n" + dir);

            DownloadBuilder.New()
            .WithUrl(hash.uri)
            .WithDirectory(hash.dir)
            .WithFileName(hash.name)
            .Build()
            .StartAsync().Wait();
            if(name%100==0)
            sDK.info = new Info(name.ToString(), "info");
            //downloader.DownloadFileTaskAsync(url, dir).Wait();
            nowthreadnum--;
            
            

        }



    }
}
