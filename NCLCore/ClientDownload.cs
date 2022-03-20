using Downloader;
using ICSharpCode.SharpZipLib.Zip;

namespace NCLCore
{

    public class ClientDownload
    {
        public string? DownloadSoureURL;
        public string? minecraftDir;
        public string _log = "";
        public string log
        {
            get { return _log; }
            set
            {
                _log = value;
                this.OnWorkStateChanged(new EventArgs());
            }
        }
        public event EventHandler PropertyChanged;
        public void OnWorkStateChanged(EventArgs eventArgs)
        {
            if (this.PropertyChanged != null)//判断事件是否有处理函数
            {
                this.PropertyChanged(this, eventArgs);
            }

        }
        public static ClientDownload cd;
        public void init()
        {
            cd = this;
        }
        public void DownloadOfficialClient(string ver)
        {
            DownloadManager downloadManager = new DownloadManager();
            downloadManager.Add(new DownloadItem(DownloadSoureURL + "version/" + ver + "/client", minecraftDir + "\\versions\\" + ver + "\\" + ver + ".jar"));
            downloadManager.Add(new DownloadItem(DownloadSoureURL + "version/" + ver + "/json", minecraftDir + "\\versions\\" + ver + "\\" + ver + ".json"));
            downloadManager.Start(2);
        }
        public void DownloadNchargeClient(NchargeClient nchargeClient)
        {

            log = "开始下载客户端:" + nchargeClient.name + "(" + nchargeClient.Cname + ")";
            log = "开始下载原版" + nchargeClient.version + "客户端";
            NchargeClientDownload downloadManager = new NchargeClientDownload();

            downloadManager.ClientDownload = this;
            downloadManager.Add(new DownloadItem(DownloadSoureURL + "version/" + nchargeClient.version + "/client", minecraftDir + "\\versions\\" + nchargeClient.version + "\\" + nchargeClient.version + ".jar"));
            downloadManager.Add(new DownloadItem(DownloadSoureURL + "version/" + nchargeClient.version + "/json", minecraftDir + "\\versions\\" + nchargeClient.version + "\\" + nchargeClient.version + ".json"));
            downloadManager.Start(2);
            log = "下载原版" + nchargeClient.version + "客户端完成";
            //log = "开始下载Forge" + nchargeClient.forgeVersion + "";
            //ForgeInstaller.installForge(DownloadSoureURL, nchargeClient.name, minecraftDir, nchargeClient.forgeVersion, this);
           // log = "安装Forge" + nchargeClient.forgeVersion + "完成";
            log = "开始下载" + nchargeClient.name + "覆盖包";
            DownloadBuilder.New()
            .WithUrl("http://download.ncserver.top:8000/NCL/" + nchargeClient.name + ".zip")
            .WithFileLocation(minecraftDir + "\\temp\\" + nchargeClient.name + ".zip").Build().StartAsync().Wait();
            (new FastZip()).ExtractZip(minecraftDir + "\\temp\\" + nchargeClient.name + ".zip", minecraftDir + "\\versions\\" + nchargeClient.name, "");
            log = "下载" + nchargeClient.name + "覆盖包完成";
            NchargeModsDownload modsdownload = new NchargeModsDownload();
            modsdownload.ClientDownload = this;
            modsdownload.toDir = minecraftDir + "\\versions\\" + nchargeClient.name + "\\mods\\";
            modsdownload.Start(80, nchargeClient.mods);
            log = "下载" + nchargeClient.name + "客户端完成";
        }
    }
}
