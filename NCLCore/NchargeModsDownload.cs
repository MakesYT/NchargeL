using Downloader;
using log4net;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NCLCore
{
    internal class NchargeModsDownload
    {
        private static readonly ILog log = LogManager.GetLogger("NchargeModsDownload");
        List<JObject> modJsons = new List<JObject>();
        public ClientDownload ClientDownload { get; set; }
        public int DownloadCount = 0;
        public int AllCount = 0;
        int cancellationsOccurrenceCount = 0;
        public string toDir;
        string error = "";

        int nowthreadnum = 0;
        public void Start(int thread, JArray jArray)
        {
            ClientDownload.log = "开始解析MODS列表";

            foreach (JObject mod in jArray)
            {
                modJsons.Add(mod);

            }

            log.Debug(jArray.Count);
            AllCount = jArray.Count;
            while (modJsons.Count != 0 || nowthreadnum != 0)
                while (nowthreadnum < thread)
                {
                    if (modJsons.Count > 0)
                    {
                        JObject hash = modJsons.First();
                        modJsons.Remove(hash);
                        Thread.Sleep(10);
                        Task.Factory.StartNew(() => DownloadTool(hash));
                        nowthreadnum++;
                        

                    }
                    else if (nowthreadnum == 0) break;
                }
            if (cancellationsOccurrenceCount != 0)
                ClientDownload.log = "有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error;
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
        private void DownloadTool(JObject hash)
        {
            string re1 = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse("https://addons-ecs.forgesvc.net/api/v2/addon/" + hash["projectID"] + "/file/" + hash["fileID"], new Dictionary<String, String>()));
            var jObject = JObject.Parse(re1);
            string uri = jObject["downloadUrl"].ToString();
            string dir = toDir + jObject["fileName"].ToString();
            //string sha1 = jObject["hashes"][0]["value"].ToString();
            if (uri != null && uri != "")
            {
                log.Debug(uri);
                bool flag = false;
                //log.Debug(Path.GetDirectoryName(hash.dir));
                if (File.Exists(dir)&& jObject["hashes"]!=null && ((JArray)jObject["hashes"]).Count>0)
                {
                    if (GetSHA1(dir) == jObject["hashes"][0]["value"].ToString())
                    {
                        flag = true;
                        ClientDownload.log = DownloadCount+"/"+AllCount+"文件"+dir.Substring(dir.LastIndexOf("\\")+1)+"无需下载,sha1校验通过";
                    }
                }
                if (!flag)
                {
                    IDownload download = DownloadBuilder.New()
                    .WithUrl(uri)
                    .WithFileLocation(dir)
                   // .WithConfiguration(new DownloadConfiguration() { Timeout = 5000, BufferBlockSize = 10240, ChunkCount = 16,ParallelDownload = true })
                    .Build();
                    download.DownloadFileCompleted += (s, e) =>
                    {
                        if (e.Error != null)
                        {
                            cancellationsOccurrenceCount++;
                            log.Error("下载出现错误:" + e.Error.Message);
                            error = error + "下载" + dir + "时出现错误\n下载地址:" + uri + "\n错误信息" + e.Error.Message + "\n";

                        }
                    };
                    download.StartAsync().Wait();
                    ClientDownload.log = "当前已下载:" + DownloadCount + ",总计:" + AllCount;
                }

                DownloadCount++;
                //  if (name % 100 == 0)
                
                //downloader.DownloadFileTaskAsync(url, dir).Wait();

            }
            else
            {
                cancellationsOccurrenceCount++;
                error = error + "下载" + dir + "时出现错误\n下载地址:" + uri + "\n错误信息:不存在下载地址" + "\n";
            }

            nowthreadnum--;


        }



    }
}
