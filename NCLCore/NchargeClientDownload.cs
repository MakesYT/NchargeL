using Downloader;
using log4net;

namespace NCLCore
{
    internal class NchargeClientDownload
    {
        private static readonly ILog log = LogManager.GetLogger("NchargeClientDownload");
        List<DownloadItem> Hashs = new List<DownloadItem>();
        public ClientDownload ClientDownload { get; set; }
        public int DownloadCount = 0;
        public int AllCount = 0;
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
            AllCount=Hashs.Count;
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
                ClientDownload.log = "有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error;
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
                DownloadCount++;
                //  if (name % 100 == 0)
                ClientDownload.log = "当前已下载:" + DownloadCount + ",总计:" + AllCount;
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
