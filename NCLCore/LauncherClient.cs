using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;

namespace NCLCore
{
    public class LauncherClient
    {
        private readonly ILog log = LogManager.GetLogger("LauncherClient");
        // public InfoManager infoManager;
        public LauncherClient() { }
        /// <summary>
        /// 正常启动 返回代码1
        /// 令牌无效 返回代码2
        /// 其他异常 返回代码-1
        /// </summary>
        /// <param name="clt"></param>
        /// <param name="name"></param>
        /// <param name="uuid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public int StartClient(InfoManager infoManager, Client clt, string DownloadSoureURL, string name, string uuid, string token, string java, int RAM)
        {
            //infostr = new Info("有" + 1 + "个资源文件下载失败,但仍将尝试启动\n错误信息" , "error");
            FileInfo launcher_profiles = new FileInfo(Directory.GetCurrentDirectory() + "\\Resources\\launcher_profiles.json");
            launcher_profiles.CopyTo(clt.rootdir + "\\launcher_profiles.json", true);
            FileInfo server_data = new FileInfo(Directory.GetCurrentDirectory() + "\\Resources\\servers.dat");
            server_data.CopyTo(clt.dir + "\\servers.dat", true);
            ClientTools clientTools = new ClientTools(infoManager);
            log.Info(clt.Name+" "+clt.assets);
            infoManager.Info(new Info(1, "正在检测令牌是否失效"));
            //pro = new Info(1, "正在检测令牌是否失效");
            log.Info("1");
            // info = new Info("正在检测令牌是否失效", "info");
            if (!CheckToken(token))
            {
                return 2;
            }
            infoManager.Info(new Info(10, "游戏令牌通过检测"));

            //info = new Info("游戏令牌通过检测", "success");
            infoManager.Info(new Info(15, "正在验证Assets"));

            // info = new Info("正在验证Assets", "info");

            clientTools.checkAssets(clt, DownloadSoureURL);

            infoManager.Info(new Info(80, "验证Assets完成,正在获取Libs"));

            // info = new Info("验证Assets完成", "success");


            //string log4jbug=null;
            // info = new Info("正在获取Libs", "info");

            string libstr = null;
            if (clt.Forge)
            {
                libstr = clientTools.GetLibsCommandstr(clt, new Client { Name = clt.McVer, rootdir = clt.rootdir }, DownloadSoureURL, java);
            }
            else
                libstr = clientTools.GetLibsCommandstr(clt, DownloadSoureURL);

            // info = new Info("Libs获取成功", "success");
            // pro = new Info(90, "Libs获取成功");
            FileInfo fileInfo = new FileInfo(clt.dir + "\\" + clt.Name + ".json");
            string mainClass;
            string args = null;

            using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName))
            {
                using (JsonTextReader reader = new JsonTextReader(jsonfile))
                {
                    JObject jObject = (JObject)JToken.ReadFrom(reader);
                    mainClass = jObject["mainClass"].ToString();
                    if (clt.Forge)
                        if (jObject.Property("arguments") != null)
                        {

                            foreach (string keys in jObject["arguments"]["jvm"].ToArray())
                            {
                                args = args + " " + keys;
                            }
                            foreach (string keys in jObject["arguments"]["game"].ToArray())
                            {
                                args = args + " " + keys;
                            }
                        }
                }
            }

            string all = "\"" + java + "\"" + " -javaagent:\"" + Directory.GetCurrentDirectory() + "\\Resources\\authlib-injector.jar\"={api} " +
                // "-Dauthlibinjector.side=client -Dauthlibinjector.yggdrasil.prefetched=eyJtZXRhIjp7InNlcnZlck5hbWUiOiJOY2hhcmdlIE1DXHU1OTE2XHU3ZjZlXHU3NjdiXHU1ZjU1IiwiaW1wbGVtZW50YXRpb25OYW1lIjoiWWdnZHJhc2lsIEFQSSBmb3IgQmxlc3NpbmcgU2tpbiIsImltcGxlbWVudGF0aW9uVmVyc2lvbiI6IjUuMS41IiwibGlua3MiOnsiaG9tZXBhZ2UiOiJodHRwczpcL1wvd3d3Lm5jc2VydmVyLnRvcDo2NjYiLCJyZWdpc3RlciI6Imh0dHBzOlwvXC93d3cubmNzZXJ2ZXIudG9wOjY2NlwvYXV0aFwvcmVnaXN0ZXIifSwiZmVhdHVyZS5ub25fZW1haWxfbG9naW4iOnRydWV9LCJza2luRG9tYWlucyI6WyJ3d3cubmNzZXJ2ZXIudG9wIl0sInNpZ25hdHVyZVB1YmxpY2tleSI6Ii0tLS0tQkVHSU4gUFVCTElDIEtFWS0tLS0tXG5NSUlDSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQWc4QU1JSUNDZ0tDQWdFQXlmcjY0R09icXZkRENFcjhFT1JBXG5QaTg5VkxiUDVOV3JSaG9BbDcyZ2pLbTRvUmppblp2WFMrRzZnRCtaTGJvdnlMVmg3SktKSUc1QlI5SHJWTGNLXG5Jb3ArMFNuN0lQUUo4XC9YMkt1UkhqYnNiVEFTREtLV1RkTHNCcDMzcTR5SEIwMFJpNzFTbkxhK2tQdFZ4UE5kcFxuelE0Tnk0czU0c2JCejdOWmM0OHJXdWh4RG1rZTh5anIycWxXQ0FwS1ZHVngxYUJrNVYxb3loeFwveFFnVUtaUmRcbndYeDVhVmtkY2NDd045eWc2STlMY0hPa2d1Y1NCUHY4NTZSU2ZTTnZHbHVYV1N0VlFXTVhLNVVcL25YU2pYUTdHXG45bFdiNUJ4T1JqY3h0TDFIWXBnYm9RanVVNW9oTWUrdmRMRytmUmp4TE1mVDdLUlgzTzZRelkyOGdlT096d1l6XG5MSGwyV0xISEhlTXdiRDJtYng5MlZCY0tsZkwwUDR1eGVxeG9mYWplOURyWVVIY1VvN2ZGbUF2VHN0UU00VDNIXG5GXC85YTZ1emxHRHV1MHp4RjljWkJ6Z3JyXC92MDRROGZ4Tkh0TjlZRjl2MGZSazk0b0c4QVcrSU5CQmFnTWFTbkNcblQ4XC9XYUtaOUtSRStBMk5YWFhvZ1E1NWppOU12dFB6NlJNalBQNWtlR0hNZW8xbXNWN2VPTExXZGRaYStxWE5OXG5aZ0ZUcXlpc3pYRnhRTWZRVTRDREcyZEVsdUZ6MndTemsxY0xVN3pYemUwVk9ldVorQnJvVm5pWmZ1enpSTFBTXG5PbENTSUJYQys1dGVnd3lXWTVCaU1zSldhWmdveUhpVmppWHpFaVJ4aW9xelJGbkorc1FJSFpYWnI2UVpyVXBqXG5MalBvQUtBOWs5QkZ3d0Fhbkl5ajF6a0NBd0VBQVE9PVxuLS0tLS1FTkQgUFVCTElDIEtFWS0tLS0tXG4ifQ==" +
                " -XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Xmn256m -Xmx" + RAM + "m \"-Djava.library.path=" + clt.dir + "\\natives\"" + " -cp \"" + libstr + "\"" + " " + mainClass +
        " --username " + name + " --version " + clt.Name + " --gameDir \"" + clt.dir + "\" --assetsDir \"" + clt.rootdir + "\\assets\" --assetIndex " + clt.assets + " --versionType NCL" +
        " --uuid " + uuid + " --accessToken " + token + " --userType mojang --width 854 --height 480";
            // info.msg = all;
            all = all.Replace("//", "/");
            all = all.Replace("/", "\\");
            all = all.Replace("{api}", "\"https://www.ncserver.top:666/api/yggdrasil\"");
            if (clt.Forge)
            {
                if (args.Length > 0)
                    all = all + args;
                else
                    all = all + " --tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker";
            }
            //return 
            infoManager.Info(new Info(99.99, "游戏已启动等待窗口"));
            //info = new Info("正在启动游戏", "info");
            ExecuteInCmd(all, clt.rootdir + "\\versions\\" + clt.Name);
            infoManager.Info(new Info(100, "游戏已启动等待窗口"));


            return 1;

        }
        public bool CheckToken(string token)
        {
            Dictionary<String, String> pList = new Dictionary<String, String>();
            pList.Add("accessToken", token);
            try
            {
                HttpWebResponse re = HttpRequestHelper.CreatePostHttpResponse("https://www.ncserver.top:666/api/yggdrasil/authserver/validate", pList);
                if (re.StatusCode == HttpStatusCode.NoContent)
                {
                    re.Close();
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;


            }
            return false;
        }
        public string ExecuteInCmd(string cmdline, string dir)
        {

            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = false;
                // process.StartInfo.
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                // process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine("cd /d " + dir);
                process.StandardInput.WriteLine(cmdline + "&exit");
                process.StandardInput.Close();
                string line;
                while ((line = process.StandardOutput.ReadLine()) != null)
                {
                    log.Debug(line);
                }
                //获取cmd窗口的输出信息  
                // string output = process.StandardOutput.ReadToEnd();
                // process.StandardOutput.
                process.WaitForExit();
                process.Close();

                return "";
            }
        }
    }
}
