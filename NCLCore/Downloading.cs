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
        private string error = "";
        private static DownloadConfiguration downloadOpt = new DownloadConfiguration()
        {
            ChunkCount = 8, // file parts to download, default value is 1
            OnTheFlyDownload = true, // caching in-memory or not? default values is true
            ParallelDownload = true // download parts of file as parallel or not. Default value is false
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
            downloadItem = hash;
             download = DownloadBuilder.New()
               .WithUrl(hash.uri)
               .WithFileLocation(hash.fullname)
               .WithConfiguration(downloadOpt)
                .Build();
            download.DownloadFileCompleted += DownloadFileCompleted;
        }
        public async Task StartAsync()
        {
           await download.StartAsync();
           
           
        }
        public DownloadReslut GetDownloadReslut()
        {
            if (error == "") return new DownloadReslut(false, new List<DownloadItem> { downloadItem }, error);
            else return new DownloadReslut(true);
        }
    }
}
