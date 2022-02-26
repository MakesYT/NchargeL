using Downloader;
using log4net;

namespace NCLCore
{
    internal class DownloadManager
    {
        private static readonly ILog log = LogManager.GetLogger("DownloadManager");
        List<DownloadItem> Hashs = new List<DownloadItem>();
        public int DownloadCount = 0;
        public SDK sDK;
        int cancellationsOccurrenceCount = 0;
        string error = "";
        int All = 0;
        int Count = 0;
        public void Add(DownloadItem di)
        {
            Hashs.Add(di);
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
                        DownloadItem hash = Hashs.First();
                        Hashs.Remove(hash);
                        Task.Factory.StartNew(() => DownloadTool(hash));
                        nowthreadnum++;
                        Thread.Sleep(10);

                    }
                    else if (nowthreadnum == 0) break;
                }
            if (cancellationsOccurrenceCount != 0)
                sDK.info = new Info("有" + cancellationsOccurrenceCount + "个资源文件下载失败,但仍将尝试启动\n错误信息" + error, "errorDia");
        }

        private void DownloadTool(DownloadItem hash)
        {

            if (hash.uri != null && hash.uri != "")
            {
                log.Debug(hash.uri);
                //log.Debug(Path.GetDirectoryName(hash.dir));
                IDownload download = DownloadBuilder.New()
                .WithUrl(hash.uri)
                .WithFileLocation(hash.dir)
                // .WithConfiguration(new DownloadConfiguration() { Timeout = 5000, BufferBlockSize = 10240, ChunkCount = 16,ParallelDownload = true })
                .Build();
                download.DownloadFileCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        cancellationsOccurrenceCount++;
                        log.Error("下载出现错误:" + e.Error.Message);
                        error = error + "下载" + hash.dir + "时出现错误\n下载地址:" + hash.uri + "\n错误信息" + e.Error.Message + "\n";

                    }
                };
                download.StartAsync().Wait();
                Count++;
                //  if (name % 100 == 0)
                sDK.pro = new Info(Count / All * 100, "下载Libs" + Count + "/" + All);
                //sDK.info = new Info("还有" + (All - Count).ToString() + "个库文件未下载", "info");
                //downloader.DownloadFileTaskAsync(url, dir).Wait();

            }
            else
            {
                cancellationsOccurrenceCount++;
                error = error + "下载" + hash.dir + "时出现错误\n下载地址:" + hash.uri + "\n错误信息:不存在下载地址" + "\n";
            }
            DownloadCount++;
            nowthreadnum--;


        }



    }
}
