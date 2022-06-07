using Downloader;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLCore
{
    public class Downloading
    {
        private static readonly ILog log = LogManager.GetLogger("Downloading");
        private string error = "1";
        private HttpClient webClient = new HttpClient();
        private bool candownload = true;
        private static DownloadConfiguration downloadOpt = new DownloadConfiguration()
        {
            ChunkCount = 8, // file parts to download, default value is 1
            BufferBlockSize = 10485760,//10MB
            OnTheFlyDownload = true, // caching in-memory or not? default values is true
            ParallelDownload = true, // download parts of file as parallel or not. Default value is false
            MaxTryAgainOnFailover = 10,
            Timeout=3000
            
        };
        private static DownloadConfiguration downloadOptSmell = new DownloadConfiguration()
        {
            ChunkCount = 1, // file parts to download, default value is 1
            OnTheFlyDownload = true, // caching in-memory or not? default values is true
            MaxTryAgainOnFailover = 10,
            Timeout = 3000
        };
        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            // ..infoManager.Info(new Info(e., InfoType.info));
            
            if (e.Error != null)
            {
                error = error + e.Error.Message;
                //cancellationsOccurrenceLists.Add(sender.);
            }
        }
        private IDownload download;
        private DownloadItem downloadItem;
        public Downloading(DownloadItem hash)
        {

            var result = webClient.Send(new HttpRequestMessage(HttpMethod.Head, hash.uri));
            if (result.IsSuccessStatusCode)
            {
                if(result.Content.Headers.ContentLength>= 10485760)
                    {
                        downloadItem = hash;
            
                        download = DownloadBuilder.New()
                        .WithUrl(hash.uri)
                        .WithFileLocation(hash.fullname)
                        .WithConfiguration(downloadOpt)
                        .Build();
                        download.DownloadFileCompleted += DownloadFileCompleted;
                    }
                    else
                    {
                        downloadItem = hash;

                        download = DownloadBuilder.New()
                            .WithUrl(hash.uri)
                            .WithFileLocation(hash.fullname)
                            .WithConfiguration(downloadOptSmell)
                            .Build();
                        download.DownloadFileCompleted += DownloadFileCompleted;
                    }
            }else
                {
                error = result.RequestMessage.Content.ToString();
                log.Error(error);
                candownload = false;
                }
            
            

          
        }
        public async Task StartAsync()
        {
            if (candownload)
           await download.StartAsync();
            download.Clear();
           
           
        }
        public DownloadReslut GetDownloadReslut()
        {
            if (error != "1")
            {
                log.Error("文件"+downloadItem.fullname+"下载失败");
                return new DownloadReslut(false, new List<DownloadItem> { downloadItem }, error);
            }
            else return new DownloadReslut(true);
        }
    }
}
