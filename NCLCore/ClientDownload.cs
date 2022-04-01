
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using Newtonsoft.Json.Linq;

namespace NCLCore
{

    public class ClientDownload
    {
        InfoManager infoManager;
        public ClientDownload(InfoManager infoManager) { this.infoManager = infoManager; }
        public void DownloadOfficialClient(string ver, string DownloadSoureURL, string rootdir)
        {
            DownloadManagerV2 downloadManager = new DownloadManagerV2(infoManager);
            List<DownloadItem> downloadItems = new List<DownloadItem>();
            downloadItems.Add(new DownloadItem(DownloadSoureURL + "version/" + ver + "/client", rootdir + "\\versions\\" + ver + "\\" + ver + ".jar"));
            downloadItems.Add(new DownloadItem(DownloadSoureURL + "version/" + ver + "/json", rootdir + "\\versions\\" + ver + "\\" + ver + ".json"));
            downloadManager.Start(downloadItems, 2);
        }
        public int DownloadCount = 0;
        public int AllCount = 0;
        int cancellationsOccurrenceCount = 0;
        public string toDir;
        string error = "";
        private static readonly ILog loger = LogManager.GetLogger("ClientDownload");
        public void DownloadNchargeClient(NchargeClient nchargeClient, string DownloadSoureURL, string rootdir)
        {
            infoManager.Info(new Info("开始下载客户端:" + nchargeClient.name + "(" + nchargeClient.Cname + ")", InfoType.info));


            infoManager.Info(new Info("开始下载原版" + nchargeClient.version + "客户端", InfoType.info));

            DownloadManagerV2 downloadManager = new DownloadManagerV2(infoManager);
            List<DownloadItem> downloadItems = new List<DownloadItem>();
            //downloadManager.setInfoManager(infoManager);
            downloadItems.Add(new DownloadItem(DownloadSoureURL + "version/" + nchargeClient.version + "/client", rootdir + "\\versions\\" + nchargeClient.version + "\\" + nchargeClient.version + ".jar"));
            downloadItems.Add(new DownloadItem(DownloadSoureURL + "version/" + nchargeClient.version + "/json", rootdir + "\\versions\\" + nchargeClient.version + "\\" + nchargeClient.version + ".json"));
            downloadManager.Start(downloadItems, 2);
            infoManager.Info(new Info("下载原版" + nchargeClient.version + "客户端完成", InfoType.info));
            infoManager.Info(new Info("开始下载Forge" + nchargeClient.forgeVersion, InfoType.info));

            ForgeInstaller.installForge(DownloadSoureURL, nchargeClient.name, rootdir, nchargeClient.forgeVersion, infoManager);
            infoManager.Info(new Info("安装Forge" + nchargeClient.forgeVersion + "完成", InfoType.info));
            infoManager.Info(new Info("开始下载" + nchargeClient.name + "覆盖包", InfoType.info));
            downloadItems.Clear();
            downloadItems.Add(new DownloadItem("http://download.ncserver.top:8000/NCL/" + nchargeClient.name + ".zip", rootdir + "\\temp\\" + nchargeClient.name + ".zip"));
            downloadManager.Start(downloadItems, 1);
            (new FastZip()).ExtractZip(rootdir + "\\temp\\" + nchargeClient.name + ".zip", rootdir + "\\versions\\" + nchargeClient.name, "");

            JObject jObject1 = new JObject();
            jObject1.Add("ver", nchargeClient.NchargeVersion);
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject1, Newtonsoft.Json.Formatting.Indented);
            if (File.Exists(rootdir + "\\versions\\" + nchargeClient.name + "\\" + nchargeClient.name + ".ncharge"))
                File.Delete(rootdir + "\\versions\\" + nchargeClient.name + "\\" + nchargeClient.name + ".ncharge");
            File.WriteAllText(rootdir + "\\versions\\" + nchargeClient.name + "\\" + nchargeClient.name + ".ncharge", output);
            infoManager.Info(new Info("下载" + nchargeClient.name + "覆盖包完成", InfoType.info));


            NchargeModsDownload modsdownload = new NchargeModsDownload(infoManager);
            // modsdownload.ClientDownload = this;
            modsdownload.toDir = rootdir + "\\versions\\" + nchargeClient.name + "\\mods\\";
            modsdownload.Start(250, nchargeClient.mods);

            infoManager.Info(new Info("解析MODS完成,共需下载" + modsdownload.getMODs().Count + "个文件", InfoType.info));
            if (modsdownload.getMODs().Count != 0)
            {

                AllCount = modsdownload.getMODs().Count;

                DirectoryInfo directory = new DirectoryInfo(modsdownload.toDir);
                if (!directory.Exists)
                {
                    directory.Create();
                }
                string uri = null;

                FileInfo forge_bootstrapper = new FileInfo(Directory.GetCurrentDirectory() + "\\Resources\\wget.exe");
                forge_bootstrapper.CopyTo(rootdir + "\\wget.exe", true);
                List<DownloadItem> downloads = modsdownload.getMODs();
                DownloadManagerV2 mod = new DownloadManagerV2(infoManager);
                mod.Start(downloads, 50);
                infoManager.Info(new Info("下载" + nchargeClient.name + "客户端完成", InfoType.info));

            }
            else

                infoManager.Info(new Info("下载" + nchargeClient.name + "客户端完成", InfoType.info));
        }


    }
}
