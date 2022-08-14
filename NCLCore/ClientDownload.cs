using ICSharpCode.SharpZipLib.Zip;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NCLCore;

public class ClientDownload
{
    private static readonly ILog loger = LogManager.GetLogger("ClientDownload");
    private readonly InfoManager infoManager;
    public int AllCount;
    private int cancellationsOccurrenceCount = 0;

    public int DownloadCount = 0;
    private string error = "";
    public string toDir;

    public ClientDownload(InfoManager infoManager)
    {
        this.infoManager = infoManager;
    }

    public void DownloadOfficialClient(string ver, string DownloadSoureURL, string rootdir)
    {
        var downloadManager = new DownloadManagerV2(infoManager);
        var downloadItems = new List<DownloadItem>();
        downloadItems.Add(new DownloadItem(DownloadSoureURL + "version/" + ver + "/client",
            rootdir + "\\versions\\" + ver + "\\" + ver + ".jar"));
        downloadItems.Add(new DownloadItem(DownloadSoureURL + "version/" + ver + "/json",
            rootdir + "\\versions\\" + ver + "\\" + ver + ".json"));
        downloadManager.Start(downloadItems, 2);
    }

    public void DownloadNchargeClient(NchargeClient nchargeClient, string DownloadSoureURL, string rootdir)
    {
        infoManager.Info(new Info("开始下载客户端:" + nchargeClient.name + "(" + nchargeClient.Cname + ")",
            InfoType.info));


        infoManager.Info(new Info("开始下载原版" + nchargeClient.version + "客户端", InfoType.info));

        var downloadManager = new DownloadManagerV2(infoManager);
        var downloadItems = new List<DownloadItem>();
        //downloadManager.setInfoManager(infoManager);
        downloadItems.Add(new DownloadItem(DownloadSoureURL + "version/" + nchargeClient.version + "/client",
            rootdir + "\\versions\\" + nchargeClient.version + "\\" + nchargeClient.version + ".jar"));
        downloadItems.Add(new DownloadItem(DownloadSoureURL + "version/" + nchargeClient.version + "/json",
            rootdir + "\\versions\\" + nchargeClient.version + "\\" + nchargeClient.version + ".json"));
        downloadManager.Start(downloadItems, 2);
        infoManager.Info(new Info("下载原版" + nchargeClient.version + "客户端完成", InfoType.info));
        infoManager.Info(new Info("开始下载Forge" + nchargeClient.forgeVersion, InfoType.info));

        ForgeInstaller.installForge(DownloadSoureURL, nchargeClient.name, rootdir, nchargeClient.forgeVersion,
            infoManager);
        infoManager.Info(new Info("安装Forge" + nchargeClient.forgeVersion + "完成", InfoType.info));
        infoManager.Info(new Info("开始下载" + nchargeClient.name + "覆盖包", InfoType.info));
        downloadItems.Clear();
        downloadItems.Add(new DownloadItem(
            "https://download.ncserver.top:8000/NCL/clients/" + nchargeClient.name + "/" +
            nchargeClient.NchargeVersion + ".zip", rootdir + "\\temp\\" + nchargeClient.name + ".zip"));
        downloadManager.Start(downloadItems, 1);
        new FastZip().ExtractZip(rootdir + "\\temp\\" + nchargeClient.name + ".zip",
            rootdir + "\\versions\\" + nchargeClient.name, "");

        var jObject1 = new JObject();
        jObject1.Add("ver", nchargeClient.NchargeVersion);
        var output = JsonConvert.SerializeObject(jObject1, Formatting.Indented);
        if (File.Exists(rootdir + "\\versions\\" + nchargeClient.name + "\\" + nchargeClient.name + ".ncharge"))
            File.Delete(rootdir + "\\versions\\" + nchargeClient.name + "\\" + nchargeClient.name + ".ncharge");
        File.WriteAllText(rootdir + "\\versions\\" + nchargeClient.name + "\\" + nchargeClient.name + ".ncharge",
            output);
        infoManager.Info(new Info("下载" + nchargeClient.name + "覆盖包完成", InfoType.info));

        var re1 = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse(
            "https://download.ncserver.top:8000/NCL/clients/" + nchargeClient.name + "/" +
            nchargeClient.NchargeVersion + ".json", new Dictionary<string, string>()));
        var jObject = JArray.Parse(re1);
        var modsdownload = new NchargeModsDownload(infoManager);
        // modsdownload.ClientDownload = this;
        modsdownload.toDir = rootdir + "\\versions\\" + nchargeClient.name + "\\mods\\";
        modsdownload.Start(250, jObject);
        loger.Info("1");
        loger.Info(modsdownload.getMODs().Count);
        infoManager.Info(new Info("解析MODS完成,共需下载" + modsdownload.getMODs().Count + "个文件", InfoType.info));
        if (modsdownload.getMODs().Count != 0)
        {
            AllCount = modsdownload.getMODs().Count;

            var directory = new DirectoryInfo(modsdownload.toDir);
            if (!directory.Exists) directory.Create();

            string uri = null;

          
            var downloads = modsdownload.getMODs();

            var mod = new DownloadManagerV2(infoManager);
            DownloadReslut re2= mod.Start(downloads, 50);
            infoManager.Info(new Info("有文件下载失败,再次尝试下载", InfoType.info));
            DownloadReslut downloadReslut = mod.Start(re2.downloadItems, 20,true);
            if(!(downloadReslut.downloadItems.Count==0))
            infoManager.Info(new Info("下载" + nchargeClient.name + "客户端完成\n但有"+downloadReslut.downloadItems.Count+
                "个文件下载失败,以下为具体消息:\n"+downloadReslut.error, InfoType.errorDia));

            infoManager.Info(new Info("下载" + nchargeClient.name + "客户端完成", InfoType.info));
        }
        else

        {
            infoManager.Info(new Info("下载" + nchargeClient.name + "客户端完成", InfoType.info));
        }
        modsdownload = null;
        downloadManager = null;
        System.GC.Collect();
    }
}