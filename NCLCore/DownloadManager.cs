using Downloader;
using log4net;

namespace NCLCore
{
    internal class DownloadManager
    {
        private static readonly ILog log = LogManager.GetLogger("DownloadManager");
        List<DownloadItem> Hashs = new List<DownloadItem>();
        public SDK sDK;
        int cancellationsOccurrenceCount = 0;
        string error="";
        public void Add(DownloadItem di)
        {
            Hashs.Add(di);
        }
        int nowthreadnum = 0;
        public void Start(int thread)
        {

            log.Debug(Hashs.Count);
            while (Hashs.Count != 0 || nowthreadnum != 0)
                while (nowthreadnum < thread)
                {
                    if (Hashs.Count > 0)
                    {
                        DownloadItem hash = Hashs.First();
                        Hashs.Remove(hash);
                        Task.Factory.StartNew(() => DownloadTool(Hashs.Count, hash));
                        nowthreadnum++;
                        Thread.Sleep(10);

                    }
                    else if (nowthreadnum == 0) break;
                }
            if (cancellationsOccurrenceCount != 0)
                sDK.info = new Info("有" + cancellationsOccurrenceCount + "个资源文件下载失败,但仍将尝试启动\n错误信息" + error, "errorDia");
        }

        private void DownloadTool(int name, DownloadItem hash)
        {

            if (hash.uri != null&& hash.uri != "")
            {
                log.Debug(hash.uri);
                //log.Debug(Path.GetDirectoryName(hash.dir));
                IDownload download = DownloadBuilder.New()
                .WithUrl(hash.uri)
                .WithFileLocation(hash.dir)
                .WithConfiguration(new DownloadConfiguration() { Timeout = 5000, BufferBlockSize = 4096, ChunkCount = 32})
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
              //  if (name % 100 == 0)
                    sDK.info = new Info(name.ToString(), "info");
                //downloader.DownloadFileTaskAsync(url, dir).Wait();

            }
            else
            {
                cancellationsOccurrenceCount++;
                error = error + "下载" + hash.dir + "时出现错误\n下载地址:" + hash.uri + "\n错误信息:不存在下载地址"  + "\n";
            }

            nowthreadnum--;


        }



    }
}
