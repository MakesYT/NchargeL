using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NCLCore;

public class ClientTools
{
    private static readonly ILog log = LogManager.GetLogger("ClientTools");
    private readonly InfoManager infoManager;

    public ClientTools(InfoManager infoManager)
    {
        this.infoManager = infoManager;
    }

    public void checkAssets(Client client, string DownloadSoureURL)
    {
        var assetsIndexInfo = new FileInfo(client.rootdir + "\\assets\\indexes\\" + client.assets + ".json");
        if (!assetsIndexInfo.Exists)
        {
            infoManager.Info(new Info(0, "Assets-Index文件不存在,正在下载"));

            var AssetsFileInfo =
                new FileInfo(client.rootdir + "\\versions\\" + client.McVer + "\\" + client.McVer + ".json");
            using (var jsonfile = File.OpenText(AssetsFileInfo.FullName))
            {
                using (var reader = new JsonTextReader(jsonfile))
                {
                    var jObject = (JObject) JToken.ReadFrom(reader);
                    var downloadManagerV2 = new DownloadManagerV2(infoManager, true);

                    //downloadManagerV2.setInfoManager(infoManager);
                    var downloadItems = new List<DownloadItem>();
                    downloadItems.Add(new DownloadItem(
                        jObject["assetIndex"]["url"].ToString()
                            .Replace("https://launchermeta.mojang.com/", DownloadSoureURL),
                        assetsIndexInfo.FullName));
                   var re1= downloadManagerV2.Start(downloadItems, 1);
                    if (!re1.allSuccess)
                    {
                        {
                            infoManager.Info(new Info("有" + re1.downloadItems.Count + "个文件下载失败\n错误信息" + re1.error, InfoType.errorDia));
                            log.Info(re1.error);
                        }
                    }
                }
            }
        }

        var downloadManager = new DownloadManagerV2(infoManager, true);
        //downloadManager.setInfoManager(infoManager);
        var assetsDownloadItems = new List<DownloadItem>();
        using (var jsonfile = File.OpenText(assetsIndexInfo.FullName))
        {
            using (var reader = new JsonTextReader(jsonfile))
            {
                var jObject = (JObject) JToken.ReadFrom(reader);
                string jstr;
                foreach (var o in ((JObject) jObject["objects"]).Properties())
                {
                    jstr = o.Name;

                    var hash = jObject["objects"][o.Name]["hash"].ToString();
                    var assetinfo =
                        new FileInfo(client.rootdir + "\\assets\\objects\\" + hash[0] + hash[1] + "\\" + hash);
                    if (!assetinfo.Exists)
                    {
                        log.Info("资源文件:" + hash + "不存在");
                        assetsDownloadItems.Add(new DownloadItem(
                            DownloadSoureURL + "assets/" + hash[0] + hash[1] + "/" + hash, assetinfo.FullName));
                    }
                }
            }
        }

       var re= downloadManager.Start(assetsDownloadItems, 100,true);
        if (!re.allSuccess)
        {
            {
 infoManager.Info(new Info("有" + re.downloadItems.Count + "个文件下载失败\n错误信息"+re.error, InfoType.errorDia));
                log.Info(re.error);
            }
        }
       
    }

    /// <summary>
    ///     return 0 java正确
    ///     return 1 java版本错误
    ///     return -1 java为32位
    ///     return -2 无java
    ///     return 其它 建议java版本
    ///     <param name="java"></param>
    ///     <param name="mcVer"></param>
    ///     <returns></returns>
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
            process.StandardInput.WriteLine("\"" + java + "\" -jar \"" + Directory.GetCurrentDirectory() +
                                            "\\Resources\\Javacheck.jar\"" + "&exit");
            process.StandardInput.Close();
            var line = "";
            var line2 = "";
            string linetemp;

            while (true)
                if ((linetemp = process.StandardOutput.ReadLine()) != null)
                {
                    line2 = line;
                    line = linetemp;
                }

                else
                {
                    break;
                }

            process.WaitForExit();
            process.Close();
            if (line2 == "false") return -1;

            if (line2 == "true")
                switch (line)
                {
                    case "11":
                    {
                        if (client.McVer=="1.16.5") return 0;
                            return 1;
                    }
                    case "1.8":
                    {
                            if (client.McVer == "1.16.5") return 11;
                            return 0;
                    }
                   default: return 1;
                }

            return -2;
        }
    }
    public static string JavaVer(string java)
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
            process.StandardInput.WriteLine("\"" + java + "\" -jar \"" + Directory.GetCurrentDirectory() +
                                            "\\Resources\\Javacheck.jar\"" + "&exit");
            process.StandardInput.Close();
            var line = "";
            var line2 = "";
            string linetemp;

            while (true)
                if ((linetemp = process.StandardOutput.ReadLine()) != null)
                {
                    line2 = line;
                    line = linetemp;
                }

                else
                {
                    break;
                }

            process.WaitForExit();
            process.Close();
            if (line2 == "false") return "Java为32位请修改";

            if (line2 == "true") return "Java版本为"+line;
               
                }

            return "Java不存在";
        }
        
    

    public static ObservableCollection<Client> GetALLClient(string dir)
    {
        var clients = new ObservableCollection<Client>();
        try
        {
            var root = new DirectoryInfo(dir);

            //foreach (DirectoryInfo directory in root.GetDirectories())
            {
                // if (directory.Name == "versions")
                {
                    root = new DirectoryInfo(dir + "\\versions");
                    if (!root.Exists) root.Create();
                    foreach (var file in root.GetDirectories())
                    {
                        var client = new Client();

                        var jsonFile = new FileInfo(file.FullName + "\\" + file.Name + ".json");
                        if (jsonFile.Exists)
                        {
                            client.dir = file.FullName;
                            client.rootdir = dir;
                            client.Name = file.Name;
                            try
                            {
                                using (var jsonfile = File.OpenText(jsonFile.FullName))
                                {
                                    using (var reader = new JsonTextReader(jsonfile))
                                    {
                                        var jObject = (JObject) JToken.ReadFrom(reader);
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
                                            var pa = jsonFile.FullName;
                                            log.Debug(dir + "\\versions\\" + client.McVer + "\\" + client.McVer +
                                                      ".json");
                                            using (var jsonfile1 = File.OpenText(dir + "\\versions\\" +
                                                       client.McVer + "\\" + client.McVer + ".json"))
                                            {
                                                using (var reader1 = new JsonTextReader(jsonfile1))
                                                {
                                                    var jObject1 = (JObject) JToken.ReadFrom(reader1);
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
                            catch (Exception ex)
                            {
                                log.Error(ex);
                            }
                        }

                        var nchargeFile = new FileInfo(file.FullName + "\\" + file.Name + ".ncharge");
                        if (nchargeFile.Exists)
                        {
                            client.Ncharge = true;
                            try
                            {
                                using (var jsonfile = File.OpenText(nchargeFile.FullName))
                                {
                                    using (var reader = new JsonTextReader(jsonfile))
                                    {
                                        var jObject = (JObject) JToken.ReadFrom(reader);
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
                            catch (Exception ex)
                            {
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
            log.Error(ex);
        }

        return clients;
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

    public bool installForge(Client clt, string DownloadSoureURL, Lib forgelib, string java)
    {
        log.Debug("forge安装");
        forgelib.url = DownloadSoureURL + "maven/" + forgelib.path.Replace(".jar", "-installer.jar");
        var jardir =
            (clt.rootdir + "\\libraries\\" + forgelib.path.Replace("/", "\\")).Replace(".jar", "-installer.jar");
        var downloadManagerV2 = new DownloadManagerV2(infoManager);
        //downloadManagerV2.setInfoManager(infoManager);
        var downloadItems = new List<DownloadItem>();
        downloadItems.Add(new DownloadItem(forgelib.url, jardir));
        downloadManagerV2.Start(downloadItems, 1);
        //string forgeInstallerName = jardir.Substring(jardir.LastIndexOf("\\")+1);
        var forge_bootstrapper =
            new FileInfo(Directory.GetCurrentDirectory() + "\\Resources\\forge-install-bootstrapper.jar");
        forge_bootstrapper.CopyTo(clt.rootdir + "\\forge-install-bootstrapper.jar", true);
        log.Debug(java + " -cp \"forge-install-bootstrapper.jar;" + jardir + "\" com.bangbang93.ForgeInstaller " +
                  "\"" + clt.rootdir + "\"");
        return ExecuteInCmd(
            "\"" + java + "\"" + " -cp \"forge-install-bootstrapper.jar;" + jardir +
            "\" com.bangbang93.ForgeInstaller " + "\"" + clt.rootdir + "\"", clt.rootdir);
       
    }

    public Libs GetLibs(Client client, string DownloadSoureURL)
    {
        var downloadManager = new DownloadManagerV2(infoManager);
        //downloadManager.setInfoManager(infoManager);
        var items = new List<DownloadItem>();
        var libs = new Libs();
        var Normallibs = new List<Lib>();
        var Nativelibs = new List<Lib>();
        try
        {
            var fileInfo =
                new FileInfo(client.rootdir + "\\versions\\" + client.Name + "\\" + client.Name + ".json");
            using var jsonfile = File.OpenText(fileInfo.FullName);
            using var reader = new JsonTextReader(jsonfile);
            var jObject = (JObject) JToken.ReadFrom(reader);
            if (jObject["libraries"] != null)
                foreach (JObject libsjosn in jObject["libraries"])
                {
                    var need = false;
                    if ((JArray) libsjosn["rules"] != null)
                    {
                        foreach (JObject js in (JArray) libsjosn["rules"])
                            if (js["action"].ToString() == "allow")
                            {
                                if (js["os"] == null) need = true;
                                else if (js["os"]["name"].ToString() == "windows") need = true;
                            }
                    }
                    else
                    {
                        need = true;
                    }

                    if (need)
                    {
                        var tmplibsjosn = (JObject) libsjosn["downloads"];
                        if (libsjosn["natives"] != null)
                        {
                            var lib = new Lib
                            {
                                path = tmplibsjosn["classifiers"]["natives-windows"]["path"].ToString(),
                                url = tmplibsjosn["classifiers"]["natives-windows"]["url"].ToString(),
                                sha1 = tmplibsjosn["classifiers"]["natives-windows"]["sha1"].ToString(),
                                name = libsjosn["name"].ToString(),
                                native = true
                            };
                            var fileInfo1 =
                                new FileInfo(client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                            if (!fileInfo1.Exists)
                            {
                                infoManager.Info(new Info(lib.path + "Natives库文件不存在,正在重新获取", InfoType.info));
                                //info = new Info(lib.path + "Natives库文件不存在,正在重新获取", "info");
                                //log.Debug(lib.path + "Natives库文件不存在");
                                if (DownloadSoureURL != null)
                                    lib.url = lib.url.Replace("https://libraries.minecraft.net/",
                                        DownloadSoureURL + "maven/");

                                items.Add(new DownloadItem(lib.url,
                                    client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                //info = new Info(lib.path + "Natives库文件获取成功", "success");
                            }
                            else
                            {
                                if (GetSHA1(fileInfo1.FullName) == lib.sha1)
                                {
                                    log.Debug("Natives库文件存在" + client.rootdir + "\\libraries\\" +
                                              lib.path.Replace("/", "\\"));
                                }
                                else
                                {
                                    fileInfo1.Delete();
                                    if (DownloadSoureURL != null)
                                    {
                                        lib.url = lib.url.Replace("https://libraries.minecraft.net/",
                                            DownloadSoureURL + "maven/");
                                        lib.url = lib.url.Replace("https://maven.minecraftforge.net/",
                                            DownloadSoureURL + "maven/");
                                    }

                                    items.Add(new DownloadItem(lib.url,
                                        client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
                                    log.Debug("Natives库文件异常sha1校验不通过" + client.rootdir + "\\libraries\\" +
                                              lib.path.Replace("/", "\\"));
                                }
                            }
                            //log.Debug(rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));

                            // lib.verDir = rootdir + "\\libraries\\" + lib.path.Replace("/", "\\");
                            Nativelibs.Add(lib);
                        }
                        else if (tmplibsjosn["artifact"] != null)
                        {
                            var lib = new Lib
                            {
                                path = tmplibsjosn["artifact"]["path"].ToString(),
                                url = tmplibsjosn["artifact"]["url"].ToString(),
                                sha1 = tmplibsjosn["artifact"]["sha1"].ToString(),
                                name = libsjosn["name"].ToString()
                            };
                            //log.Debug(DownloadSoureURL);
                            if (DownloadSoureURL != null)
                            {
                                lib.url = lib.url.Replace("https://maven.minecraftforge.net/",
                                    DownloadSoureURL + "maven/");
                                lib.url = lib.url.Replace("https://libraries.minecraft.net/",
                                    DownloadSoureURL + "maven/");
                            }

                            var fileInfo1 =
                                new FileInfo(client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                            if (!fileInfo1.Exists)
                            {
                                infoManager.Info(new Info(lib.path + "库文件不存在,正在重新获取", InfoType.info));
                                //info = new Info(lib.path + "库文件不存在,正在重新获取", "info");
                                if (!lib.path.Contains("net/minecraftforge/forge/"))
                                {
                                    items.Add(new DownloadItem(lib.url,
                                        client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
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
                                    items.Add(new DownloadItem(lib.url,
                                        client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\")));
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
                            else
                            {
                                log.Debug("库文件存在" + client.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"));
                            }

                            Normallibs.Add(lib);
                        }
                    }
                }
        }
        catch (Exception ex)
        {
        }

        downloadManager.Start(items, 50);

        libs.Normallibs = Normallibs;
        libs.Nativelibs = Nativelibs;

        return libs;
    }

    public string GetLibsCommandstr(Client clt, string DownloadSoureURL)
    {
        string libstr = null;


        var libs = GetLibs(clt, DownloadSoureURL);

        foreach (var lib in libs.Normallibs) libstr = libstr + clt.rootdir + "\\libraries\\" + lib.path + ";";

        libstr = libstr + clt.dir + "\\" + clt.Name + ".jar";
        var nativedir = new DirectoryInfo(clt.rootdir + "\\versions\\" + clt.Name + "\\natives");
        if (!nativedir.Exists)
            nativedir.Create();
        log.Debug(nativedir.FullName);
        foreach (var lib in libs.Nativelibs)
            new FastZip().ExtractZip(clt.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"),
                clt.rootdir + "\\versions\\" + clt.McVer + "\\natives\\", "");

        return libstr;
    }

    public string GetLibsCommandstr(Client clt, Client ver, string DownloadSoureURL, string java)
    {
        string libstr = null;

        List<Lib> log4jbugs = new();
        List<Lib> allLibs = new();
        var libs2 = GetLibs(ver, DownloadSoureURL);
        // throw new NCLException("");
        foreach (var lib in libs2.Normallibs)
        {
            if (lib.path.Contains("org/apache/logging/log4j/"))
                log4jbugs.Add(lib);
            //  { log.Info("1"); }
            allLibs.Add(lib);
        }

        var nativedir = new DirectoryInfo(clt.rootdir + "\\versions\\" + clt.Name + "\\natives");
        if (!nativedir.Exists)
            nativedir.Create();
        log.Debug(nativedir.FullName);
        foreach (var lib in libs2.Nativelibs)
            new FastZip().ExtractZip(clt.rootdir + "\\libraries\\" + lib.path.Replace("/", "\\"),
                clt.rootdir + "\\versions\\" + clt.Name + "\\natives\\", "");

        var libs = GetLibs(clt, DownloadSoureURL);
        if (libs.forgeintall) installForge(clt, DownloadSoureURL, libs.forgelib, java);
        //libstr = libstr + "\"";
        foreach (var lib in libs.Normallibs)
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

        foreach (var lib in allLibs) libstr = libstr + clt.rootdir + "\\libraries\\" + lib.path + ";";

        libstr = libstr + clt.rootdir + "\\versions\\" + clt.McVer + "\\" + clt.McVer + ".jar";


        return libstr.Replace("/", "\\");
    }


    public bool ExecuteInCmd(string cmdline, string dir)
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
                infoManager.Info(new Info(0, line));
                log.Debug(line);
                if (line == "false") return false;
            }

            //获取cmd窗口的输出信息  
            // string output = process.StandardOutput.ReadToEnd();
            // process.StandardOutput.
            process.WaitForExit();
            process.Close();

            return true;
        }
    }
}