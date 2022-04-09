using System.Net;
using System.Security.Cryptography;
using System.Text;
using log4net;
using Newtonsoft.Json.Linq;

namespace NCLCore
{
    internal class NchargeModsDownload
    {
        private static readonly ILog log = LogManager.GetLogger("NchargeModsDownload");
        public int AllCount = 0;
        int cancellationsOccurrenceCount = 0;
        public int DownloadCount = 0;
        string error = "";
        InfoManager infoManager;
        List<DownloadItem> listmods = new List<DownloadItem>();
        List<JObject> modJsons = new List<JObject>();

        int nowthreadnum = 0;
        public string toDir;

        public NchargeModsDownload(InfoManager infoManager)
        {
            this.infoManager = infoManager;
        }

        public ClientDownload ClientDownload { get; set; }

        public string getDir()
        {
            return toDir;
        }

        public void Start(int thread, JArray jArray)
        {
            infoManager.Info(new Info("开始解析MODS列表", InfoType.info));
            foreach (JObject mod in jArray)
            {
                modJsons.Add(mod);
            }

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
                        JObject hash = modJsons.First();
                        if (modJsons.Remove(hash))
                        {
                            Thread.Sleep(2);
                            Task.Factory.StartNew(() => DownloadTool(hash));
                            nowthreadnum++;
                        }
                    }
                    else if (nowthreadnum == 0) break;
                }
            }

            if (cancellationsOccurrenceCount != 0)
                infoManager.Info(new Info("有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error,
                    InfoType.errorDia));
        }

        string GetSHA1(string s)
        {
            try
            {
                FileStream file = new FileStream(s, FileMode.Open);
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] retval = sha1.ComputeHash(file);
                file.Close();

                StringBuilder sc = new StringBuilder();
                for (int i = 0; i < retval.Length; i++)
                {
                    sc.Append(retval[i].ToString("x2"));
                }

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
                HttpWebResponse hwr = HttpRequestHelper.CreatePostHttpResponse(
                    "https://addons-ecs.forgesvc.net/api/v2/addon/" + hash["projectID"] + "/file/" + hash["fileID"],
                    new Dictionary<String, String>());

                string re1 = HttpRequestHelper.GetResponseString(hwr);
                var jObject = JObject.Parse(re1);
                string uri = jObject["downloadUrl"].ToString();
                string dir = toDir + jObject["fileName"].ToString();
                //string sha1 = jObject["hashes"][0]["value"].ToString();
                if (uri != null && uri != "")
                {
                    // log.Debug(uri);
                    bool flag = false;
                    //log.Debug(Path.GetDirectoryName(hash.dir));
                    if (File.Exists(dir) && jObject["hashes"] != null && ((JArray) jObject["hashes"]).Count > 0)
                    {
                        if (GetSHA1(dir) == jObject["hashes"][0]["value"].ToString())
                        {
                            flag = true;
                            DownloadCount++;
                            infoManager.Info(new Info(
                                DownloadCount + "/" + AllCount + "文件" + dir.Substring(dir.LastIndexOf("\\") + 1) +
                                "无需下载,sha1校验通过", InfoType.info));

                            nowthreadnum--;
                        }
                    }

                    if (!flag)
                    {
                        infoManager.Info(new Info("需要下载" + jObject["fileName"].ToString(), InfoType.info));

                        log.Debug("需要下载:" + jObject["downloadUrl"].ToString());
                        DownloadItem downloadItem = new DownloadItem(uri, dir);
                        listmods.Add(downloadItem);
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
}