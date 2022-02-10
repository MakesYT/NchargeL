using Downloader;
using log4net;

namespace NCLCore
{
    internal class AssetsDownloadManager
    {
        private static readonly ILog log = LogManager.GetLogger("DownloadManager");
        List<string> Hashs = new List<string>();
        public string DownloadSoureURL;
        public string AssetsDir;
        public SDK sDK;
        int cancellationsOccurrenceCount = 0;
        string error;
        int All = 0;
        int Count = 0;
        public void Add(string hash)
        {
            Hashs.Add(hash);
        }
        int nowthreadnum = 0;
        public void Start(int thread)
        {
            All = Hashs.Count;
            log.Debug(Hashs.Count);
            while (Hashs.Count != 0 || nowthreadnum != 0)
                while (nowthreadnum < thread)
                {
                    if (Hashs.Count > 0)
                    {
                        string hash = Hashs.First();
                        // int name = Hashs.Count;
                        Hashs.Remove(hash);
                        Task.Factory.StartNew(() => DownloadTool(hash));

                        nowthreadnum++;


                    }
                    else if (nowthreadnum == 0) break;
                }
            if (cancellationsOccurrenceCount != 0)
                sDK.info = new Info("有" + cancellationsOccurrenceCount + "个资源文件下载失败,但仍将尝试启动\n错误信息" + error, "errorDia");
        }

        private void DownloadTool(string hash)
        {
            try
            {
                log.Debug(DownloadSoureURL + "assets/" + hash[0] + hash[1] + "/" + hash);
                IDownload download = DownloadBuilder.New()
            .WithUrl(DownloadSoureURL + "assets/" + hash[0] + hash[1] + "/" + hash)
            .WithDirectory(AssetsDir + "\\assets\\objects\\" + hash[0] + hash[1])
            //.WithConfiguration(new DownloadConfiguration() { Timeout = 5000 })
            .Build();
                download.DownloadFileCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        cancellationsOccurrenceCount++;
                        log.Error("下载出现错误:" + e.Error.Message);
                        error = error + "下载" + hash + "时出现错误\n下载地址:" + DownloadSoureURL + "assets/" + hash[0] + hash[1] + "/" + hash + "\n错误信息" + e.Error.Message + "\n";

                    }
                };
                // download.DownloadFileCompleted += OnDownloadFileCompleted;

                download.StartAsync().Wait();
                nowthreadnum--;
                Count++;
                if (Count % 16 == 0)
                    sDK.info = new Info("还有" + (All - Count).ToString() + "个资源文件未下载", "info");
            }
            catch (Exception ex)
            {

                // sDK.info = new Info("有" + cancellationsOccurrenceCount + "个资源文件下载失败,但仍将尝试启动\n错误信息" + ex.Message, "error");
            }






        }



    }
}
