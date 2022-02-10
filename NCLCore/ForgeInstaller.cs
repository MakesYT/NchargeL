using Downloader;
using ICSharpCode.SharpZipLib.Zip;
using log4net;

namespace NCLCore
{
    public class ForgeInstaller
    {
        private static readonly ILog log = LogManager.GetLogger("ForgeInstaller");
        public static void installForge(string DownloadS, string name, string rootdir, string installerVer, ClientDownload cd)
        {
            string tempdir = rootdir + "\\temp";
            DownloadBuilder.New()
            .WithUrl(DownloadS + "maven/net/minecraftforge/forge/" + installerVer + "/forge-" + installerVer + "-installer.jar")
            .WithFileLocation(tempdir + "\\" + installerVer + "-installer.jar").Build().StartAsync().Wait();
            cd.log = "Forge下载成功";
            // log.Debug("111");
            (new FastZip()).ExtractZip(tempdir + "\\" + installerVer + "-installer.jar", tempdir, "forge-" + installerVer + ".jar");
            (new FastZip()).ExtractZip(tempdir + "\\" + installerVer + "-installer.jar", tempdir, "version.json");
            cd.log = "解压文件完成";
            // log.Debug("111");
            DirectoryInfo dir = new DirectoryInfo(rootdir + "\\libraries\\net\\minecraftforge\\forge\\" + installerVer);
            if (!dir.Exists) dir.Create();
            FileInfo fileInfo = new FileInfo(tempdir + "\\maven\\net\\minecraftforge\\forge\\" + installerVer + "\\forge-" + installerVer + ".jar");
            fileInfo.CopyTo(rootdir + "\\libraries\\net\\minecraftforge\\forge\\" + installerVer + "\\forge-" + installerVer + ".jar", true);
            DirectoryInfo verDir = new DirectoryInfo(rootdir + "\\versions\\" + name);
            verDir.Create();
            FileInfo verfileInfo = new FileInfo(tempdir + "\\version.json");
            verfileInfo.CopyTo(rootdir + "\\versions\\" + name + "\\" + name + ".json", true);
            DirectoryInfo directoryInfo = new DirectoryInfo(tempdir);
            directoryInfo.Delete(true);
            cd.log = "缓存清理完成";
        }
    }
}
