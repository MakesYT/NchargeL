using System.Text;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NCLCore;

public class ClientUpdate
{
    private InfoManager infoManager;
    private static readonly ILog log = LogManager.GetLogger("ClientUpdate");
    public ClientUpdate(InfoManager infoManager)
    {
        this.infoManager = infoManager;
    }

    public void update(Client clt, NchargeClient nchargeClient, string java, string DownloadSoureURL)
    {
        var fileInfo =
            new FileInfo(clt.rootdir + "\\versions\\" + clt.Name + "\\" + clt.Name + ".json");
        using var jsonfile = File.OpenText(fileInfo.FullName);

        using var reader = new JsonTextReader(jsonfile);
        var jObject = (JObject) JToken.ReadFrom(reader);
        jsonfile.Dispose();
        var installed = false;
        foreach (JObject libsjosn in jObject["libraries"])
            if (libsjosn["name"].ToString().Contains("net.minecraftforge:forge"))
            {
                var tmplibsjosn = (JObject) libsjosn["downloads"];
                var lib = new Lib
                {
                    path = tmplibsjosn["artifact"]["path"].ToString(),
                    url = tmplibsjosn["artifact"]["url"].ToString(),
                    sha1 = tmplibsjosn["artifact"]["sha1"].ToString(),
                    name = libsjosn["name"].ToString()
                };
                if (lib.name == "net.minecraftforge:forge:" + nchargeClient.forgeVersion)
                {
                    infoManager.Info(new Info(0, "forge无需更新"));
                }
                else
                {
                    var ver = lib.name.Replace("net.minecraftforge:forge:", "");
                    infoManager.Info(new Info(0, "更新Forge从" + ver + "更新到" + nchargeClient.forgeVersion));

                    ForgeInstaller.installForge(DownloadSoureURL, nchargeClient.name, clt.rootdir,
                        nchargeClient.forgeVersion,
                        infoManager);

                    var clientTools = new ClientTools(infoManager);
                    lib.path = lib.path.Replace(ver, nchargeClient.forgeVersion);

                    clientTools.installForge(clt, DownloadSoureURL, lib, java);
                    infoManager.Info(new Info(0, "更新Forge完成"));
                }

                installed = true;
                break;
            }

        if (!installed)
        {
            infoManager.Info(new Info("未找到需要的Forge,这个bug不应该出现", InfoType.errorDia));
        }
        else //Mods对比
        {
            var info = new StringBuilder();
            JArray oldLists;
            JArray newLists;
            {
                //删除无法对比Mods
                infoManager.Info(new Info(0, "开始对比Mods"));
                var re1 = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse(
                    "http://download.ncserver.top:8000/NCL/clients/" + nchargeClient.name + "/" +
                    clt.NchargeVer + ".json", new Dictionary<string, string>()));
                var oldJArray = JArray.Parse(re1);
                var modsdownload = new NchargeModsDownload(infoManager, true);
                // modsdownload.ClientDownload = this;
                modsdownload.toDir = clt.rootdir + "\\versions\\" + nchargeClient.name + "\\mods\\";
                modsdownload.Start(250, oldJArray);
                oldLists = oldJArray;
                var existsmods = new StringBuilder();
                foreach (var mod in modsdownload.getAllmods()) existsmods.Append(mod.fullname);

                var oldDirectoryInfo =
                    new DirectoryInfo(clt.rootdir + "\\versions\\" + nchargeClient.name + "\\mods\\");
                var existsmodsStr = existsmods.ToString();
                info.AppendLine("删除的Mods:");

                foreach (var file in oldDirectoryInfo.GetFiles())
                    if (!existsmodsStr.Contains(file.FullName))
                    {
                        file.Delete();
                        infoManager.Info(new Info(0, "删除Mods:" + file.Name));
                        info.AppendLine("   " + file.Name);
                    }

                infoManager.Info(new Info(0, "删除无法对比Mods完成,获取新版本Mods中"));
            }
            {
                //获取新版本mods
                var re1 = HttpRequestHelper.GetResponseString(HttpRequestHelper.CreatePostHttpResponse(
                    "http://download.ncserver.top:8000/NCL/clients/" + nchargeClient.name + "/" +
                    nchargeClient.NchargeVersion + ".json", new Dictionary<string, string>()));
                var newJArray = JArray.Parse(re1);

                var modsdownload = new NchargeModsDownload(infoManager, true);
                // modsdownload.ClientDownload = this;
                modsdownload.toDir = clt.rootdir + "\\versions\\" + nchargeClient.name + "\\mods\\";
                modsdownload.Start(250, newJArray);
                newLists = newJArray;
            }
            {
                //确认多余mods,以及需要下载的mod
                JArray dels;
                JArray needs;
                var tempoldLists = oldLists;
                var tempnewLists = newLists;
                foreach (var mod in newLists) tempoldLists.Remove(mod);
                log.Debug(tempnewLists.ToString());
                dels = tempoldLists;
                var delsdownload = new NchargeModsDownload(infoManager, true);
                // modsdownload.ClientDownload = this;
                delsdownload.toDir = clt.rootdir + "\\versions\\" + nchargeClient.name + "\\mods\\";
                delsdownload.Start(250, dels);
                foreach (var mod in delsdownload.getAllmods())
                {
                    var file = new FileInfo(mod.fullname);
                    file.Delete();
                    info.AppendLine("   " + file.Name);
                }

                foreach (var mod in oldLists) tempnewLists.Remove(mod);
                needs = tempnewLists;
                info.AppendLine("需要下载的Mods:");
                
                var needsdownload = new NchargeModsDownload(infoManager, true);
                // modsdownload.ClientDownload = this;
                needsdownload.toDir = clt.rootdir + "\\versions\\" + nchargeClient.name + "\\mods\\";
                needsdownload.Start(250, dels);
                foreach (var mod in needsdownload.getAllmods())
                {
                    var file = new FileInfo(mod.fullname);

                    info.AppendLine("   " + file.Name);
                }

                var mod1 = new DownloadManagerV2(infoManager, true);
                mod1.Start(needsdownload.getAllmods(), 50);
                infoManager.Info(new Info(info.ToString(), InfoType.successDia));
            }
        }
    }
}