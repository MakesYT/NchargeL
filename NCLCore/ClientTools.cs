using ICSharpCode.SharpZipLib.Zip;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace NCLCore
{
    public class ClientTools
    {
        private static readonly ILog log = LogManager.GetLogger("ClientTools");
        InfoManager infoManager;
        public ClientTools(InfoManager infoManager) { this.infoManager = infoManager; }
        public void checkAssets(Client client, string DownloadSoureURL)
        {


            FileInfo assetsIndexInfo = new FileInfo(client.rootdir + "\\assets\\indexes\\" + client.assets + ".json");
            if (!assetsIndexInfo.Exists)
            {
                infoManager.Info(new Info(0, "Assets-Index文件不存在,正在下载"));

                FileInfo AssetsFileInfo = new FileInfo(client.rootdir + "\\versions\\" + client.McVer + "\\" + client.McVer + ".json");
                using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(AssetsFileInfo.FullName))
                {
                    using (JsonTextReader reader = new JsonTextReader(jsonfile))
                    {
                        JObject jObject = (JObject)JToken.ReadFrom(reader);
                        DownloadManagerV2 downloadManagerV2 = new DownloadManagerV2(infoManager, true);

                        //downloadManagerV2.setInfoManager(infoManager);
                        List<DownloadItem> downloadItems = new List<DownloadItem>();
                        downloadItems.Add(new DownloadItem(jObject["assetIndex"]["url"].ToString().Replace("https://launchermeta.mojang.com/", DownloadSoureURL), assetsIndexInfo.FullName));
                        downloadManagerV2.Start(downloadItems, 1);

                    }
                }
            }
            DownloadManagerV2 downloadManager = new DownloadManagerV2(infoManager, true);
            //downloadManager.setInfoManager(infoManager);
            List<DownloadItem> assetsDownloadItems = new List<DownloadItem>();
            using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(assetsIndexInfo.FullName))
            {
                using (JsonTextReader reader = new JsonTextReader(jsonfile))
                {
                    JObject jObject = (JObject)JToken.ReadFrom(reader);
                    string jstr;
                    foreach (var o in ((JObject)jObject["objects"]).Properties())
                    {
                        jstr = o.Name;

                        var hash = jObject["objects"][o.Name]["hash"].ToString();
                        FileInfo assetinfo = new FileInfo(client.rootdir + "\\assets\\objects\\" + hash[0] + hash[1] + "\\" + hash);
                        if (!assetinfo.Exists)
                        {
                            log.Info("资源文件:" + hash + "不存在");
                            assetsDownloadItems.Add(new DownloadItem(DownloadSoureURL + "assets/" + hash[0] + hash[1] + "/" + hash, assetinfo.FullName));

                        }
                    }
                }
            }
            downloadManager.Start(assetsDownloadItems, 300);
        }
        /// <summary>
        /// return 0 java正确
        /// return 1 java版本错误
        /// return -1 java为32位
        /// return -2 无java
        /// return 其它 建议java版本
        /// <param name="java"></param>
        /// <param name="mcVer"></param>
        /// <returns></returns>
        /// </summary>
        public static int JavaCheck(string java, Client client)
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
                string line2 = "";
                string linetemp;

                while (true)
                {
                    if ((linetemp = process.StandardOutput.ReadLine()) != null)
                    {
                        line2 = line;
                        line = linetemp;
                    }

                    else break;
                }
                process.WaitForExit();
                process.Close();
                if (line2 == "false") return -1;

                else if (line2 == "true")
                    switch (client.McVer)
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
                else return -2;

            }
        }
        public static ObservableCollection<Client> GetALLClient(string dir)
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

                            FileInfo jsonFile = new FileInfo(file.FullName + "\\" + file.Name + ".json");
                            if (jsonFile.Exists)
                            {
                                client.dir = file.FullName;
                                client.rootdir = dir;
                                client.Name = file.Name;
                                try
                                {
                                    using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(jsonFile.FullName))
                                    {
                                        using (JsonTextReader reader = new JsonTextReader(jsonfile))
                                        {
                                            JObject jObject = (JObject)JToken.ReadFrom(reader);
                                            if (jObject["assets"] != null)
                                            {
                                                client.assets = jObject["assets"].ToString();
                                                client.McVer = jObject["id"].ToString();
                                                log.Debug("assets" + client.assets);
                                            }
                                            else if (jObject["inheritsFrom"] != null)
                                            {
                                                client.Forge = true;
                                                client.McVer = jObject["inheritsFrom"].ToString();
                                                string pa = jsonFile.FullName;
                                                log.Debug(dir + "\\versions\\" + client.McVer + "\\" + client.McVer + ".json");
                                                using (System.IO.StreamReader jsonfile1 = System.IO.File.OpenText(dir + "\\versions\\" + client.McVer + "\\" + client.McVer + ".json"))
                                                {
                                                    using (JsonTextReader reader1 = new JsonTextReader(jsonfile1))
                                                    {

                                                        JObject jObject1 = (JObject)JToken.ReadFrom(reader1);
                                                        //log.Debug(jObject1.ToString());
                                                        client.assets = jObject1["assets"].ToString();
                                                        log.Debug("inheritsFrom" + client.assets);
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
                                catch (Exception ex) { log.Error(ex); }
                            }
                            FileInfo nchargeFile = new FileInfo(file.FullName + "\\" + file.Name + ".ncharge");
                            if (nchargeFile.Exists)
                            {
                                client.Ncharge = true;
                                try
                                {
                                    using (System.IO.StreamReader jsonfile = System.IO.File.OpenText(nchargeFile.FullName))
                                    {
                                        using (JsonTextReader reader = new JsonTextReader(jsonfile))
                                        {
                                            JObject jObject = (JObject)JToken.ReadFrom(reader);
                                            try
                                            {
                                                client.NchargeVer = jObject["ver"].ToString();
                                            }
                                            catch (Exception ex) { }
                                        }
                                    }
                                }
                                catch (Exception ex) { }
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
            catch (Exception ex) { log.Error(ex); }
            return clients;
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
        public void installForge(Client clt, string DownloadSoureURL, Lib forgelib, string java)
        {
            log.Debug("forge安装");
            forgelib.url = DownloadSoureURL + "maven/" + forgelib.path.Replace(".jar", "-installer.jar");
            string jardir = (clt.rootdir + "\\libraries\\" + forgelib.path.Replace("/", "\\")).Replace(".jar", "-installer.jar");
            DownloadManagerV2 downloadManagerV2 = new DownloadManagerV2(infoManager);
            //downloadManagerV2.setInfoManager(infoManager);
            List<DownloadItem> downloadItems = new List<DownloadItem>();
            downloadItems.Add(new DownloadItem(forgelib.url, jardir));
            downloadManagerV2.Start(downloadItems, 1);
            //string forgeInstallerName = jardir.Substring(jardir.LastIndexOf("\\")+1);
            FileInfo forge_bootstrapper = new FileInfo(Directory.GetCurrentDirectory() + "\\Resources\\forge-install-bootstrapper.jar");
            forge_bootstrapper.CopyTo(clt.rootdir + "\\forge-install-bootstrapper.jar", true);
            log.Debug(java + " -cp \"forge-install-bootstrapper.jar;" + jardir + "\" com.bangbang93.ForgeInstaller " + "\"" + clt.rootdir + "\"");
            ExecuteInCmd("\"" + java + "\"" + " -cp \"forge-install-bootstrapper.jar;" + jardir + "\" com.bangbang93.ForgeInstaller " + "\"" + clt.rootdir + "\"", clt.rootdir);
        }
        public Libs GetLibs(Client client, string DownloadSoureURL)
        {
            DownloadManagerV2 downloadManager = new DownloadManagerV2(infoManager);
            //downloadManager.setInfoManager(infoManager);
            List<DownloadItem> items = new List<DownloadItem>();
            Libs libs = new Libs();
            List<Lib> Normallibs = new List<Lib>();
            List<Lib> Nativelibs = new List<Lib>();
            try
            {
                FileInfo fileInfo = new FileInfo(client.rootdir + "\\versions\\" + client.Name + "\\" + client.Name + ".json");
                using System.IO.StreamReader jsonfile = System.IO.File.OpenText(fileInfo.FullName);
                using JsonTextReader reader = new JsonTextReader(jsonfile);
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
                                FileInfo fileInfo1 = new FileInfo(client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                if (!fileInfo1.Exists)
                                {
                                    infoManager.Info(new Info(lib.path + "Natives库文件不存在,正在重新获取", InfoType.info));
                                    //info = new Info(lib.path + "Natives库文件不存在,正在重新获取", "info");
                                    //log.Debug(lib.path + "Natives库文件不存在");
                                    if (DownloadSoureURL != null)
                                    {
                                        lib.url = lib.url.Replace("https://libraries.minecraft.net/", DownloadSoureURL + "maven/");
                                    }
                                    items.Add(new DownloadItem(lib.url, client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                    //info = new Info(lib.path + "Natives库文件获取成功", "success");
                                }
                                else
                                {

                                    if (GetSHA1(fileInfo1.FullName) == lib.sha1)
                                    {
                                        log.Debug("Natives库文件存在" + client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                    }
                                    else
                                    {
                                        fileInfo1.Delete();
                                        if (DownloadSoureURL != null)
                                        {
                                            lib.url = lib.url.Replace("https://libraries.minecraft.net/", DownloadSoureURL + "maven/");
                                            lib.url = lib.url.Replace("https://maven.minecraftforge.net/", DownloadSoureURL + "maven/");
                                        }
                                        items.Add(new DownloadItem(lib.url, client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                        log.Debug("Natives库文件异常sha1校验不通过" + client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
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
                                FileInfo fileInfo1 = new FileInfo(client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                if (!fileInfo1.Exists)
                                {
                                    infoManager.Info(new Info(lib.path + "库文件不存在,正在重新获取", InfoType.info));
                                    //info = new Info(lib.path + "库文件不存在,正在重新获取", "info");
                                    if (!lib.path.Contains("net/minecraftforge/forge/"))
                                    {
                                        items.Add(new DownloadItem(lib.url, client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                    }
                                    else
                                    {
                                        infoManager.Info(new Info("Forge状态异常,需要重新安装", InfoType.warn));
                                        //info = new Info("Forge状态异常,需要重新安装", "warn");
                                        libs.forgeintall = true;
                                        libs.forgelib = lib;
                                    }

                                }
                                else if (GetSHA1(fileInfo1.FullName) != lib.sha1)
                                {
                                    fileInfo1.Delete();
                                    if (!lib.path.Contains("net/minecraftforge/forge/"))
                                    {

                                        items.Add(new DownloadItem(lib.url, client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                        log.Debug("库文件异常sha1校验不通过" + fileInfo1.FullName);
                                    }
                                    else
                                    {
                                        infoManager.Info(new Info("Forge sha1校验未通过,正在重新安装", InfoType.warn));
                                        //info = new Info("Forge sha1校验未通过,正在重新安装", "warn");
                                        libs.forgeintall = true;
                                        libs.forgelib = lib;
                                    }
                                }
                                else log.Debug("库文件存在" + client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                                Normallibs.Add(lib);
                            }
                        }

                    }
                }
                else
                {

                }
            }
            catch (Exception ex) { }
            downloadManager.Start(items, 50);

            libs.Normallibs = Normallibs;
            libs.Nativelibs = Nativelibs;

            return libs;
        }

        public string GetLibsCommandstr(Client clt, string DownloadSoureURL)
        {
            string libstr = null;


            Libs libs = GetLibs(clt, DownloadSoureURL);

            foreach (Lib lib in libs.Normallibs)
            {
                libstr = libstr + clt.rootdir + "\\libraries\\" + lib.path + ";";
            }
            libstr = libstr + clt.dir + "\\" + clt.Name + ".jar";
            DirectoryInfo nativedir = new DirectoryInfo(clt.rootdir + "\\versions\\" + clt.Name + "\\natives");
            if (!nativedir.Exists)
                nativedir.Create();
            log.Debug(nativedir.FullName);
            foreach (Lib lib in libs.Nativelibs)
            {

                (new FastZip()).ExtractZip(clt.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"), clt.rootdir + "\\versions\\" + clt.McVer + "\\natives\\", "");
            }
            return libstr;
        }
        public string GetLibsCommandstr(Client clt, Client ver, string DownloadSoureURL, string java)
        {
            string libstr = null;

            List<Lib> log4jbugs = new();
            List<Lib> allLibs = new();
            Libs libs2 = GetLibs(ver, DownloadSoureURL);
            // throw new NCLException("");
            foreach (Lib lib in libs2.Normallibs)
            {
                if (lib.path.Contains("org/apache/logging/log4j/"))
                    log4jbugs.Add(lib);
                //  { log.Info("1"); }
                allLibs.Add(lib);
            }
            DirectoryInfo nativedir = new DirectoryInfo(clt.rootdir + "\\versions\\" + clt.Name + "\\natives");
            if (!nativedir.Exists)
                nativedir.Create();
            log.Debug(nativedir.FullName);
            foreach (Lib lib in libs2.Nativelibs)
            {

                (new FastZip()).ExtractZip(clt.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"), clt.rootdir + "\\versions\\" + clt.Name + "\\natives\\", "");
            }
            Libs libs = GetLibs(clt, DownloadSoureURL);
            if (libs.forgeintall) installForge(clt, DownloadSoureURL, libs.forgelib, java);
            //libstr = libstr + "\"";
            foreach (Lib lib in libs.Normallibs)
            {
                if (lib.path.Contains("org/apache/logging/log4j/"))
                    log4jbugs.Add(lib);
                allLibs.Add(lib);
            }
            log.Info(log4jbugs.Count);
            if (log4jbugs.Count > 2)
            {
                allLibs.Remove(log4jbugs[0]);
                allLibs.Remove(log4jbugs[1]);
            }
            foreach (Lib lib in allLibs)
            {
                libstr = libstr + clt.rootdir + "\\libraries\\" + lib.path + ";";
            }
            libstr = libstr + clt.rootdir + "\\versions\\" + clt.McVer + "\\" + clt.McVer + ".jar";


            return libstr.Replace("/", "\\");
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
