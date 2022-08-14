using System.Security.Cryptography;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;

namespace NCLCore;

internal class NchargeModsDownload
{
    private static readonly ILog log = LogManager.GetLogger("NchargeModsDownload");
    private readonly List<DownloadItem> allmods = new();
    private  int cancellationsOccurrenceCount = 0;
    private  string error = "";
    private readonly InfoManager infoManager;
    private readonly List<DownloadItem> listmods = new();
    private readonly List<JObject> modJsons = new();
    private readonly bool process;
    public int AllCount;
    private int nowthreadnum;
    public string toDir;
    private int DownloadCount;

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
        DownloadCount = 0;
        infoManager.Info(new Info("开始解析MODS列表", InfoType.info));
        foreach (JObject mod in jArray) modJsons.Add(mod);

        log.Debug(jArray.Count);
        AllCount = jArray.Count;
        while (modJsons.Count != 0 || nowthreadnum != 0 || DownloadCount < AllCount)
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
            infoManager.Info(new Info("有" + cancellationsOccurrenceCount + "个Mod获取失败\n错误信息" + error,
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
            var re1 = HttpRequestHelper.getHttpTool("https://cursemaven.com/test/" + hash["projectID"] + "/" + hash["fileID"]);
            if (re1 != null)
            {
                string re = re1.Result;
                if (re.Contains("Response: 200"))
                {
                    var uri = re.Substring(re.IndexOf("Found: ") + 7);
                    var dir = toDir + uri.Substring(uri.LastIndexOf("/") + 1);

                    if (!process) infoManager.Info(new Info("需要下载" + uri.Substring(uri.LastIndexOf("/") + 1), InfoType.info));


                    log.Debug("需要下载:" + uri.Substring(uri.LastIndexOf("/") + 1));
                    var downloadItem = new DownloadItem(uri, dir);
                    listmods.Add(downloadItem);
                    allmods.Add(downloadItem);
                    nowthreadnum--;
                    DownloadCount++;
                    log.Info(DownloadCount);
                }
                else
                {
                    nowthreadnum--;
                    if (hash.ContainsKey("times"))
                    {
                        if ((int)(hash["times"]) <= 5)
                        {
                            hash["times"] = ((int)(hash["times"])) + 1;
                            log.Debug("获取File失败重新获取," + hash["projectID"] + "/" + hash["fileID"]);
                            infoManager.Info(new Info("获取File失败重新获取," + hash["projectID"] + "/" + hash["fileID"],
                            InfoType.error));
                            nowthreadnum++;
                            Task.Factory.StartNew(() => DownloadTool(hash));
                        }
                        else
                        {
                            log.Debug("获取File失败超过重试次数," + hash["projectID"] + "/" + hash["fileID"]);
                            infoManager.Info(new Info("获取File失败超过重试次数," + hash["projectID"] + "/" + hash["fileID"],
                                InfoType.error));
                            DownloadCount++;
                            cancellationsOccurrenceCount++;
                            error = error + "\nmod下载链接获取失败" + hash["projectID"] + "/" + hash["fileID"];
                        }
                        
                    }
                        
                    else 
                    {
                        hash.Add("times", 1);
                        nowthreadnum++;
                        Task.Factory.StartNew(() => DownloadTool(hash));
                    }
                }

            }
            else
            {
                nowthreadnum--;
                if (hash.ContainsKey("times"))
                {
                    if ((int)(hash["times"]) <= 5)
                    {
                        hash["times"] = ((int)(hash["times"])) + 1;
                        log.Debug("获取File失败重新获取," + hash["projectID"] + "/" + hash["fileID"]);
                        infoManager.Info(new Info("获取File失败重新获取," + hash["projectID"] + "/" + hash["fileID"],
                        InfoType.error));
                        nowthreadnum++;
                        Task.Factory.StartNew(() => DownloadTool(hash));
                    }
                    else
                    {
                        log.Debug("获取File失败超过重试次数," + hash["projectID"] + "/" + hash["fileID"]);
                        infoManager.Info(new Info("获取File失败超过重试次数," + hash["projectID"] + "/" + hash["fileID"],
                            InfoType.error));
                        DownloadCount++;
                        cancellationsOccurrenceCount++;
                        error = error + "\nmod下载链接获取失败" + hash["projectID"] + "/" + hash["fileID"];
                    }

                }

                else
                {
                    hash.Add("times", 1);
                    nowthreadnum++;
                    Task.Factory.StartNew(() => DownloadTool(hash));
                }
            }
            
           
            //string sha1 = jObject["hashes"][0]["value"].ToString();
            
                // log.Debug(uri);
                
            
        }
        catch (Exception e)
        {
            nowthreadnum--;
            if (hash.ContainsKey("times"))
            {
                if ((int)(hash["times"]) <= 5)
                {
                    hash["times"] = ((int)(hash["times"])) + 1;
                    log.Debug("获取File失败重新获取," + hash["projectID"] + "/" + hash["fileID"]);
                    infoManager.Info(new Info("获取File失败重新获取," + hash["projectID"] + "/" + hash["fileID"],
                    InfoType.error));
                    nowthreadnum++;
                    Task.Factory.StartNew(() => DownloadTool(hash));
                }
                else
                {
                    log.Debug("获取File失败超过重试次数," + hash["projectID"] + "/" + hash["fileID"]);
                    infoManager.Info(new Info("获取File失败超过重试次数," + hash["projectID"] + "/" + hash["fileID"],
                        InfoType.error));
                    DownloadCount++;
                    cancellationsOccurrenceCount++;
                    error = error + "\nmod下载链接获取失败" + hash["projectID"] + "/" + hash["fileID"];
                }

            }

            else
            {
                hash.Add("times", 1);
                nowthreadnum++;
                Task.Factory.StartNew(() => DownloadTool(hash));
            }
        }
    }
}