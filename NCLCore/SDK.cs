using Downloader;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace NCLCore
{
    public class SDK
    {
        private Info _info = new("1", "info");
        public string? DownloadSoureURL;
        public Info info
        {
            get { return _info; }
            set
            {
                _info = value;
                this.OnWorkStateChanged(new EventArgs());
            }
        }public event EventHandler PropertyChanged;
        public void OnWorkStateChanged(EventArgs eventArgs)
        {
            if (this.PropertyChanged != null)//判断事件是否有处理函数
            {
                this.PropertyChanged(this, eventArgs);
            }

        }
        private Info _pro = new(1, "info");
        public Info pro
        {
            get { return _pro; }
            set
            {
                _pro = value;
                this.OnProWorkStateChanged(new EventArgs());
            }
        }
        public event EventHandler ProPropertyChanged;
        public void OnProWorkStateChanged(EventArgs eventArgs)
        {
            if (this.ProPropertyChanged != null)//判断事件是否有处理函数
            {
                this.ProPropertyChanged(this, eventArgs);
            }

        }

        private readonly ILog log = LogManager.GetLogger("SDK");
        public enum DownloadSource
        {
            Official,
            MCBBS,
            BMCLAPI,
            Custom
        }
        public string GetDownloadSoure(DownloadSource ds)
        {
            switch (ds)
            {
                case DownloadSource.Official:
                    return null;

                case DownloadSource.MCBBS:
                    return "https://download.mcbbs.net/";
                case DownloadSource.BMCLAPI:
                    return "https://bmclapi2.bangbang93.com/";
                case DownloadSource.Custom:
                    return "custom";
                default:
                    return null;
            }
        }
        public ObservableCollection<Client> GetALLClient(string dir)
        {
            ObservableCollection<Client> clients = new ObservableCollection<Client>();
            try
            {
                DirectoryInfo root = new DirectoryInfo(dir);

                //foreach (DirectoryInfo directory in root.GetDirectories())
                {
                    // if (directory.Name == "versions")
                    {
                        root = new DirectoryInfo(dir + "\\versions");
                        if (!root.Exists) root.Create();
                        foreach (DirectoryInfo file in root.GetDirectories())
                        {
                            Client client = new Client();
                            client.dir = file.FullName;
                            client.rootdir = dir;
                            foreach (FileInfo fileInfo in file.GetFiles())
                            {

                                if (fileInfo.Name == file.Name + ".json")
                                {
                                    client.Name = file.Name;
                                    try
                                    {
                                        using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName))
                                        {
                                            using (JsonTextReader reader = new JsonTextReader(jsonfile))
                                            {
                                                JObject jObject = (JObject)JToken.ReadFrom(reader);
                                                if (jObject["assets"] != null)
                                                {
                                                    client.assets = jObject["assets"].ToString();
                                                    client.McVer = jObject["id"].ToString();
                                                }
                                                else if (jObject["inheritsFrom"] != null)
                                                {
                                                    client.Forge = true;
                                                    client.McVer = jObject["inheritsFrom"].ToString();
                                                    string pa = fileInfo.FullName;
                                                    using (System.IO.StreamReader jsonfile1 = System.IO.File.OpenText(pa.Replace(file.Name, jObject["inheritsFrom"].ToString())))
                                                    {
                                                        using (JsonTextReader reader1 = new JsonTextReader(jsonfile1))
                                                        {
                                                            JObject jObject1 = (JObject)JToken.ReadFrom(reader1);
                                                            client.assets = jObject1["assets"].ToString();
                                                         //   log.Debug("asdwdwd" + client.assets);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    client.McVer = jObject["id"].ToString();
                                                }

                                            }
                                        }
                                    }
                                    catch (Exception ex) { }
                                }
                                if (fileInfo.Name == file.Name + ".ncharge")
                                {
                                    client.Ncharge = true;
                                    try
                                    {
                                        using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName))
                                        {
                                            using (JsonTextReader reader = new JsonTextReader(jsonfile))
                                            {
                                                JObject jObject = (JObject)JToken.ReadFrom(reader);
                                                try
                                                {
                                                    client.NchargeVer = jObject["ver"].ToString();
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex) { }
                                }

                            }
                            if (client.isNotNull())
                            {
                                client.Id = clients.Count + 1;
                                clients.Add(client);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                log.Error(ex);
                return clients;
            }

            return clients;
        }
        public void checkAssets(string rootdir, string dir, string assets, string clientdir)
        {


            FileInfo fileInfo = new FileInfo(dir);
            if (!fileInfo.Exists)
            {
                info = new Info("Assets-Index文件不存在,正在下载", "info");

                FileInfo AssetsFileInfo = new FileInfo(clientdir);
                using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(AssetsFileInfo.FullName))
                {
                    using (JsonTextReader reader = new JsonTextReader(jsonfile))
                    {
                        JObject jObject = (JObject)JToken.ReadFrom(reader);
                        //info = new Info(DownloadSoureURL, "info");
                     //   info = new Info(jObject["assetIndex"]["url"].ToString().Replace("https://launchermeta.mojang.com/", DownloadSoureURL), "info");
                        //downloader.DownloadFileTaskAsync(jObject["assetIndex"]["url"].ToString().Replace("https://launchermeta.mojang.com/", DownloadSoureURL), dir).Wait();
                        DownloadBuilder.New()
                        .WithUrl(jObject["assetIndex"]["url"].ToString().Replace("https://launchermeta.mojang.com/", DownloadSoureURL))
                        .WithFileLocation(dir).Build().StartAsync().Wait();
                    }
                }
            }
            AssetsDownloadManager downloadManager = new AssetsDownloadManager();
            using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName))
            {
                using (JsonTextReader reader = new JsonTextReader(jsonfile))
                {
                    JObject jObject = (JObject)JToken.ReadFrom(reader);
                    string jstr;
                    foreach (var o in ((JObject)jObject["objects"]).Properties())
                    {
                        jstr = o.Name;

                        var hash = jObject["objects"][o.Name]["hash"].ToString();
                        FileInfo assetinfo = new FileInfo(rootdir + "\\assets\\objects\\" + hash[0] + hash[1] + "\\" + hash);
                        if (!assetinfo.Exists)
                        {
                            log.Info("资源文件:" + hash + "不存在");
                            downloadManager.Add(hash);
                        }
                    }
                }
            }
            downloadManager.DownloadSoureURL = DownloadSoureURL;
            downloadManager.AssetsDir = rootdir;
            downloadManager.sDK = this;
            downloadManager.Start(150);
            // DownloadPackage pack = downloader.Package;
            //downloader.CancelAsync();
            //while (!downloader.IsCancelled) { }
            // downloader.DownloadFileTaskAsync(pack).Wait();

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
        public Libs GetLibs(string rootdir, string ver, string dir)
        {
            DownloadManager downloadManager = new DownloadManager();
            downloadManager.sDK = this;
            Libs libs = new Libs();
            List<Lib> Normallibs = new List<Lib>();
            List<Lib> Nativelibs = new List<Lib>();
            try
            {
                FileInfo fileInfo = new FileInfo(dir);
                using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName))
                {
                    using (JsonTextReader reader = new JsonTextReader(jsonfile))
                    {
                        JObject jObject = (JObject)JToken.ReadFrom(reader);

                        if (jObject["libraries"] != null)
                        {
                            foreach (JObject libsjosn in jObject["libraries"])
                            {
                                bool need = false;
                                if ((JArray)libsjosn["rules"] != null)
                                {
                                    foreach (JObject js in (JArray)libsjosn["rules"])
                                    {
                                        if (js["action"].ToString() == "allow")
                                        {
                                            if (js["os"] == null) need = true;
                                            else if (js["os"]["name"].ToString() == "windows") need = true;


                                        }
                                    }
                                }
                                else need = true;


                                if (need)
                                {
                                    JObject tmplibsjosn = (JObject)libsjosn["downloads"];
                                    if (libsjosn["natives"] != null)
                                    {
                                        Lib lib = new Lib()
                                        {
                                            path = tmplibsjosn["classifiers"]["natives-windows"]["path"].ToString(),
                                            url = tmplibsjosn["classifiers"]["natives-windows"]["url"].ToString(),
                                            sha1 = tmplibsjosn["classifiers"]["natives-windows"]["sha1"].ToString(),
                                            name = libsjosn["name"].ToString(),
                                            native = true
                                        };
                                        FileInfo fileInfo1 = new FileInfo(rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                        if (!fileInfo1.Exists)
                                        {
                                            info = new Info(lib.path + "Natives库文件不存在,正在重新获取", "info");
                                            //log.Debug(lib.path + "Natives库文件不存在");
                                            if (DownloadSoureURL != null)
                                            {
                                                lib.url = lib.url.Replace("https://libraries.minecraft.net/", DownloadSoureURL + "maven/");
                                            }
                                            downloadManager.Add(new DownloadItem(lib.url, rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                            //info = new Info(lib.path + "Natives库文件获取成功", "success");
                                        }
                                        else
                                        {

                                            if (GetSHA1(fileInfo1.FullName) == lib.sha1)
                                            {
                                                log.Debug("Natives库文件存在" + rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                            }
                                            else
                                            {
                                                fileInfo1.Delete();
                                                if (DownloadSoureURL != null)
                                                {
                                                    lib.url = lib.url.Replace("https://libraries.minecraft.net/", DownloadSoureURL + "maven/");
                                                    lib.url = lib.url.Replace("https://maven.minecraftforge.net/", DownloadSoureURL + "maven/");
                                                }
                                                downloadManager.Add(new DownloadItem(lib.url, rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                                log.Debug("Natives库文件异常sha1校验不通过" + rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                            }
                                        }
                                        //log.Debug(rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));

                                        // lib.verDir = rootdir + "\\libraries\\" + lib.path.Replace("/", "\\");
                                        Nativelibs.Add(lib);
                                    }
                                    else if (tmplibsjosn["artifact"] != null)
                                    {
                                        Lib lib = new Lib()
                                        {
                                            path = tmplibsjosn["artifact"]["path"].ToString(),
                                            url = tmplibsjosn["artifact"]["url"].ToString(),
                                            sha1 = tmplibsjosn["artifact"]["sha1"].ToString(),
                                            name = libsjosn["name"].ToString()

                                        };
                                        //log.Debug(DownloadSoureURL);
                                        if (DownloadSoureURL != null)
                                        {
                                            lib.url = lib.url.Replace("https://maven.minecraftforge.net/", DownloadSoureURL + "maven/");
                                            lib.url = lib.url.Replace("https://libraries.minecraft.net/", DownloadSoureURL + "maven/");
                                        }
                                        FileInfo fileInfo1 = new FileInfo(rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                        if (!fileInfo1.Exists)
                                        {
                                            info = new Info(lib.path + "库文件不存在,正在重新获取", "info");
                                            if (!lib.path.Contains("net/minecraftforge/forge/"))
                                            {

                                                downloadManager.Add(new DownloadItem(lib.url, rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));


                                                //info = new Info(lib.path + "库文件获取成功", "success");
                                            }
                                            else
                                            {
                                                info = new Info("Forge状态异常,正在重新安装", "warn");
                                                lib.url = DownloadSoureURL + "maven/" + lib.path.Replace(".jar", "-installer.jar");
                                                string jardir = (rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")).Replace(".jar", "-installer.jar");
                                                DownloadBuilder.New()
                                                .WithUrl(lib.url)
                                                .WithFileLocation(jardir).Build().StartAsync().Wait();
                                                (new FastZip()).ExtractZip(jardir, jardir[..jardir.LastIndexOf("\\")], jardir.Substring(jardir.LastIndexOf("\\") + 1).Replace("-installer.jar", ".jar"));
                                                FileInfo fileInfo2 = new FileInfo(jardir[..jardir.LastIndexOf("\\")] + "\\maven\\" + lib.path.Replace("/", "\\"));
                                                fileInfo2.CopyTo(jardir.Replace("-installer.jar", ".jar"));
                                            }

                                        }
                                        else if (GetSHA1(fileInfo1.FullName) != lib.sha1)
                                        {
                                            fileInfo1.Delete();
                                            if (!lib.path.Contains("net/minecraftforge/forge/"))
                                            {

                                                downloadManager.Add(new DownloadItem(lib.url, rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                                log.Debug("库文件异常sha1校验不通过" + fileInfo1.FullName);
                                            }
                                            else
                                            {
                                                info = new Info("Forge sha1校验未通过,正在重新安装", "warn");
                                                lib.url = DownloadSoureURL + "maven/" + lib.path.Replace(".jar", "-installer.jar");
                                                string jardir = (rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")).Replace(".jar", "-installer.jar");
                                                DownloadBuilder.New()
                                                .WithUrl(lib.url)
                                                .WithFileLocation(jardir).Build().StartAsync().Wait();
                                                (new FastZip()).ExtractZip(jardir, jardir[..jardir.LastIndexOf("\\")], jardir.Substring(jardir.LastIndexOf("\\") + 1).Replace("-installer.jar", ".jar"));
                                                FileInfo fileInfo2 = new FileInfo(jardir[..jardir.LastIndexOf("\\")] + "\\maven\\" + lib.path.Replace("/", "\\"));
                                                fileInfo2.CopyTo(jardir.Replace("-installer.jar", ".jar"), true);
                                            }
                                        }
                                        else log.Debug("库文件存在" + rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                        Normallibs.Add(lib);
                                    }
                                }

                            }
                        }
                        else
                        {

                        }

                    }
                }
            }
            catch (Exception ex) { }
            downloadManager.Start(50);
            libs.Normallibs = Normallibs;
            libs.Nativelibs = Nativelibs;
            foreach (Lib lib in Nativelibs)
            {
                DirectoryInfo nativedir = new DirectoryInfo(ver + "\\natives");
                if (!nativedir.Exists)
                    nativedir.Create();
                log.Debug(nativedir.FullName);
                (new FastZip()).ExtractZip(rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"), ver + "\\natives\\", "");
            }
            return libs;
        }
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
        public async Task<int> StartClient(Client clt, string name, string uuid, string token, string java, int RAM)
        {
            //infostr = new Info("有" + 1 + "个资源文件下载失败,但仍将尝试启动\n错误信息" , "error");
            FileInfo launcher_profiles = new FileInfo(Directory.GetCurrentDirectory() + "\\Resources\\launcher_profiles.json");
            launcher_profiles.CopyTo(clt.rootdir + "\\launcher_profiles.json", true);
            FileInfo server_data = new FileInfo(Directory.GetCurrentDirectory() + "\\Resources\\servers.dat");
            server_data.CopyTo(clt.dir + "\\servers.dat", true);
            log.Info("1");
            pro = new Info(1, "正在检测令牌是否失效");
            log.Info("1");
            // info = new Info("正在检测令牌是否失效", "info");
               if (!CheckToken(token))
            {
                 return 2;
            }
            pro = new Info(10, "游戏令牌通过检测");
            //info = new Info("游戏令牌通过检测", "success");
            pro = new Info(15, "正在验证Assets");
           // info = new Info("正在验证Assets", "info");

            checkAssets(clt.rootdir, clt.rootdir + "\\assets\\indexes\\" + clt.assets + ".json", clt.assets, clt.rootdir + "\\versions\\" + clt.McVer + "\\" + clt.McVer + ".json");
            pro = new Info(80, "验证Assets完成,正在获取Libs");


           // info = new Info("验证Assets完成", "success");

            string libstr = null;
           // info = new Info("正在获取Libs", "info");
            if (clt.Forge)
            {
                Libs libs2 = GetLibs(clt.rootdir, clt.rootdir + "\\versions\\" + clt.Name, clt.rootdir + "\\versions\\" + clt.McVer + "\\" + clt.McVer + ".json");
                // throw new NCLException("");
                foreach (Lib lib in libs2.Normallibs)
                {
                    libstr = libstr + clt.rootdir + "\\libraries\\" + lib.path + ";";
                }

                Libs libs = GetLibs(clt.rootdir, clt.dir, clt.dir + "\\" + clt.Name + ".json");
                //libstr = libstr + "\"";
                foreach (Lib lib in libs.Normallibs)
                {
                    libstr = libstr + clt.rootdir + "\\libraries\\" + lib.path + ";";
                }
                libstr = libstr + clt.rootdir + "\\versions\\" + clt.McVer + "\\" + clt.McVer + ".jar";
            }
            else
            {
                Libs libs = GetLibs(clt.rootdir, clt.dir, clt.dir + "\\" + clt.Name + ".json");

                foreach (Lib lib in libs.Normallibs)
                {
                    libstr = libstr + clt.rootdir + "\\libraries\\" + lib.path + ";";
                }
                libstr = libstr + clt.dir + "\\" + clt.Name + ".jar";
            }
            // info = new Info("Libs获取成功", "success");
           // pro = new Info(90, "Libs获取成功");
            FileInfo fileInfo = new FileInfo(clt.dir + "\\" + clt.Name + ".json");
            string mainClass;

            using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName))
            {
                using (JsonTextReader reader = new JsonTextReader(jsonfile))
                {
                    JObject jObject = (JObject)JToken.ReadFrom(reader);
                    mainClass = jObject["mainClass"].ToString();

                }
            }
            string all = "\"" + java + "\"" + " -javaagent:\"" + Directory.GetCurrentDirectory() + "\\Resources\\authlib-injector.jar\"={api} " +
                // "-Dauthlibinjector.side=client -Dauthlibinjector.yggdrasil.prefetched=eyJtZXRhIjp7InNlcnZlck5hbWUiOiJOY2hhcmdlIE1DXHU1OTE2XHU3ZjZlXHU3NjdiXHU1ZjU1IiwiaW1wbGVtZW50YXRpb25OYW1lIjoiWWdnZHJhc2lsIEFQSSBmb3IgQmxlc3NpbmcgU2tpbiIsImltcGxlbWVudGF0aW9uVmVyc2lvbiI6IjUuMS41IiwibGlua3MiOnsiaG9tZXBhZ2UiOiJodHRwczpcL1wvd3d3Lm5jc2VydmVyLnRvcDo2NjYiLCJyZWdpc3RlciI6Imh0dHBzOlwvXC93d3cubmNzZXJ2ZXIudG9wOjY2NlwvYXV0aFwvcmVnaXN0ZXIifSwiZmVhdHVyZS5ub25fZW1haWxfbG9naW4iOnRydWV9LCJza2luRG9tYWlucyI6WyJ3d3cubmNzZXJ2ZXIudG9wIl0sInNpZ25hdHVyZVB1YmxpY2tleSI6Ii0tLS0tQkVHSU4gUFVCTElDIEtFWS0tLS0tXG5NSUlDSWpBTkJna3Foa2lHOXcwQkFRRUZBQU9DQWc4QU1JSUNDZ0tDQWdFQXlmcjY0R09icXZkRENFcjhFT1JBXG5QaTg5VkxiUDVOV3JSaG9BbDcyZ2pLbTRvUmppblp2WFMrRzZnRCtaTGJvdnlMVmg3SktKSUc1QlI5SHJWTGNLXG5Jb3ArMFNuN0lQUUo4XC9YMkt1UkhqYnNiVEFTREtLV1RkTHNCcDMzcTR5SEIwMFJpNzFTbkxhK2tQdFZ4UE5kcFxuelE0Tnk0czU0c2JCejdOWmM0OHJXdWh4RG1rZTh5anIycWxXQ0FwS1ZHVngxYUJrNVYxb3loeFwveFFnVUtaUmRcbndYeDVhVmtkY2NDd045eWc2STlMY0hPa2d1Y1NCUHY4NTZSU2ZTTnZHbHVYV1N0VlFXTVhLNVVcL25YU2pYUTdHXG45bFdiNUJ4T1JqY3h0TDFIWXBnYm9RanVVNW9oTWUrdmRMRytmUmp4TE1mVDdLUlgzTzZRelkyOGdlT096d1l6XG5MSGwyV0xISEhlTXdiRDJtYng5MlZCY0tsZkwwUDR1eGVxeG9mYWplOURyWVVIY1VvN2ZGbUF2VHN0UU00VDNIXG5GXC85YTZ1emxHRHV1MHp4RjljWkJ6Z3JyXC92MDRROGZ4Tkh0TjlZRjl2MGZSazk0b0c4QVcrSU5CQmFnTWFTbkNcblQ4XC9XYUtaOUtSRStBMk5YWFhvZ1E1NWppOU12dFB6NlJNalBQNWtlR0hNZW8xbXNWN2VPTExXZGRaYStxWE5OXG5aZ0ZUcXlpc3pYRnhRTWZRVTRDREcyZEVsdUZ6MndTemsxY0xVN3pYemUwVk9ldVorQnJvVm5pWmZ1enpSTFBTXG5PbENTSUJYQys1dGVnd3lXWTVCaU1zSldhWmdveUhpVmppWHpFaVJ4aW9xelJGbkorc1FJSFpYWnI2UVpyVXBqXG5MalBvQUtBOWs5QkZ3d0Fhbkl5ajF6a0NBd0VBQVE9PVxuLS0tLS1FTkQgUFVCTElDIEtFWS0tLS0tXG4ifQ==" +
                " -XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmn256m -Xmx" + RAM + "m \"-Djava.library.path=" + clt.dir + "\\natives\"" + " -cp \"" + libstr + "\"" + " " + mainClass +
        " --username " + name + " --version " + clt.Name + " --gameDir \"" + clt.dir + "\" --assetsDir \"" + clt.rootdir + "\\assets\" --assetIndex " + clt.assets + " --versionType NCL" +
        " --uuid " + uuid + " --accessToken " + token + " --userType mojang --width 854 --height 480";
           // info.msg = all;
            all = all.Replace("\\", "/");
            all = all.Replace("//", "/");
            all = all.Replace("/", "\\");
            all = all.Replace("{api}", "\"https://www.ncserver.top:666/api/yggdrasil\"");
           // info.msg = all;
            // info.isWorking = false;
            //return 
            if (clt.Forge)
            {
                all = all + " --tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker";
            }
            //return 
            pro = new Info(99.99, "游戏已启动等待窗口");
            //info = new Info("正在启动游戏", "info");
            ExecuteInCmd(all, clt.rootdir + "\\versions\\" + clt.Name);
            pro = new Info(100, "游戏已启动等待窗口");
            return 1;

        }

        /// <summary>
        /// return 0 java正确
        /// return 1 java版本错误
        /// return -1 java为32位
        /// return 其它 建议java版本
        /// </summary>
        /// <param name="java"></param>
        /// <param name="mcVer"></param>
        /// <returns></returns>
        public int JavaCheck(string java, string mcVer)
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
                process.StandardInput.WriteLine("\"" + java + "\" -jar \"" + Directory.GetCurrentDirectory() + "\\Resources\\Javacheck.jar\"" + "&exit");
                process.StandardInput.Close();
                string line = "";
                string linetemp;
                while (true)
                {

                    if ((linetemp = process.StandardOutput.ReadLine()) != null)
                    {
                        line = linetemp;
                        log.Debug(line);
                    }
                    else break;
                }
                log.Debug(line);
                log.Debug(mcVer);
                if (line == "false") return -1;
                else
                    switch (mcVer)
                    {
                        case "1.16.5":
                            {
                                if (line == "11") return 0;
                                else return 11;

                            }
                        default:
                            {
                                if (line == "1.8") return 0;
                                else return 1;
                            }

                    }
                //获取cmd窗口的输出信息  
                // string output = process.StandardOutput.ReadToEnd();
                // process.StandardOutput.
                process.WaitForExit();
                process.Close();


            }

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
                log.Debug(ex.ToString());
                log.Debug(ex.Message);


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
