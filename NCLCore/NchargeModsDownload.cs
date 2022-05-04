using System.Security.Cryptography;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;

namespace NCLCore;

internal class NchargeModsDownload
{
    private static readonly ILog log = LogManager.GetLogger("NchargeModsDownload");
    private readonly List<DownloadItem> allmods = new();
    private readonly int cancellationsOccurrenceCount = 0;
    private readonly string error = "";
    private readonly InfoManager infoManager;
    private readonly List<DownloadItem> listmods = new();
    private readonly List<JObject> modJsons = new();
    private readonly bool process;
    public int AllCount;
    public int DownloadCount;
    private int nowthreadnum;
    public string toDir;

    public NchargeModsDownload(InfoManager infoManager)
    {
        this.infoManager = infoManager;
    }

    public NchargeModsDownload(InfoManager infoManager, bool process)
    {
        this.infoManager = infoManager;
        this.process = process;
    }

    public ClientDownload ClientDownload { get; set; }

    public string getDir()
    {
        return toDir;
    }

    public List<DownloadItem> getAllmods()
    {
        return allmods;
    }

    public void Start(int thread, JArray jArray)
    {
        infoManager.Info(new Info("开始解析MODS列表", InfoType.info));
        foreach (JObject mod in jArray) modJsons.Add(mod);

        log.Debug(jArray.Count);
        AllCount = jArray.Count;
        while (modJsons.Count != 0 || nowthreadnum != 0)
        {
            Thread.Sleep(500);
            while (nowthreadnum < thread)
            {
                Thread.Sleep(100);
                if (modJsons.Count > 0)
                {
                    var hash = modJsons.First();
                    if (modJsons.Remove(hash))
                    {
                        Thread.Sleep(2);
                        Task.Factory.StartNew(() => DownloadTool(hash));
                        nowthreadnum++;
                    }
                }
                else if (nowthreadnum == 0)
                {
                    break;
                }
            }
        }

        if (cancellationsOccurrenceCount != 0)
            infoManager.Info(new Info("有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error,
                InfoType.errorDia));
    }

    private string GetSHA1(string s)
    {
        try
        {
            var file = new FileStream(s, FileMode.Open);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            var retval = sha1.ComputeHash(file);
            file.Close();

            var sc = new StringBuilder();
            for (var i = 0; i < retval.Length; i++) sc.Append(retval[i].ToString("x2"));

            return sc.ToString();
        }
        catch (Exception ex)
        {
            throw new NCLException("c");
        }
    }

    public List<DownloadItem> getMODs()
    {
        return listmods;
    }

    private void DownloadTool(JObject hash)
    {
        try
        {
            var hwr = HttpRequestHelper.CreatePostHttpResponse(
                "https://addons-ecs.forgesvc.net/api/v2/addon/" + hash["projectID"] + "/file/" + hash["fileID"],
                new Dictionary<string, string>());

            var re1 = HttpRequestHelper.GetResponseString(hwr);
            var jObject = JObject.Parse(re1);
            var uri = jObject["downloadUrl"].ToString();
            var dir = toDir + jObject["fileName"];
            //string sha1 = jObject["hashes"][0]["value"].ToString();
            if (uri != null && uri != "")
            {
                // log.Debug(uri);
                var flag = false;
                //log.Debug(Path.GetDirectoryName(hash.dir));
                if (File.Exists(dir) && jObject["hashes"] != null && ((JArray) jObject["hashes"]).Count > 0)
                    if (GetSHA1(dir) == jObject["hashes"][0]["value"].ToString())
                    {
                        flag = true;
                        DownloadCount++;
                        if (process)
                            infoManager.Info(new Info(0,
                                DownloadCount + "/" + AllCount + "文件" + dir.Substring(dir.LastIndexOf("\\") + 1) +
                                "无需下载,sha1校验通过"));
                        else
                            infoManager.Info(new Info(
                                DownloadCount + "/" + AllCount + "文件" + dir.Substring(dir.LastIndexOf("\\") + 1) +
                                "无需下载,sha1校验通过", InfoType.info));
                        var downloadItem = new DownloadItem(uri, dir);
                        allmods.Add(downloadItem);
                        nowthreadnum--;
                    }

                if (!flag)
                {
                    if (!process) infoManager.Info(new Info("需要下载" + jObject["fileName"], InfoType.info));


                    log.Debug("需要下载:" + jObject["downloadUrl"]);
                    var downloadItem = new DownloadItem(uri, dir);
                    listmods.Add(downloadItem);
                    allmods.Add(downloadItem);
                    nowthreadnum--;
                }
            }
        }
        catch (Exception e)
        {
            log.Debug("获取File失败重新获取," + hash["projectID"] + "/file/" + hash["fileID"]);
            infoManager.Info(new Info("获取File失败重新获取," + hash["projectID"] + "/file/" + hash["fileID"],
                InfoType.error));

            Task.Factory.StartNew(() => DownloadTool(hash));
        }
    }
}