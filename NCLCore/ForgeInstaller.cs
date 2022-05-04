using ICSharpCode.SharpZipLib.Zip;
using log4net;

namespace NCLCore;

public class ForgeInstaller
{
    private static readonly ILog log = LogManager.GetLogger("ForgeInstaller");

    public static void installForge(string DownloadS, string name, string rootdir, string installerVer,
        InfoManager infoManager)
    {
        var tempdir = rootdir + "\\temp";
        if (!Directory.Exists(tempdir)) Directory.CreateDirectory(tempdir);
        var downloadManager = new DownloadManagerV2(infoManager);
        var downloadItems = new List<DownloadItem>();
        downloadItems.Add(new DownloadItem(
            DownloadS + "maven/net/minecraftforge/forge/" + installerVer + "/forge-" + installerVer +
            "-installer.jar", tempdir + "\\" + installerVer + "-installer.jar"));
        downloadManager.Start(downloadItems, 1);
        var jardir = tempdir + "\\" + installerVer + "-installer.jar";
        infoManager.Info(new Info("Forge下载成功", InfoType.info));
        // log.Debug("111");
        var forge_bootstrapper =
            new FileInfo(Directory.GetCurrentDirectory() + "\\Resources\\forge-install-bootstrapper.jar");
        forge_bootstrapper.CopyTo(rootdir + "\\forge-install-bootstrapper.jar", true);
        //log.Debug(java + " -cp \"forge-install-bootstrapper.jar;" + jardir + "\" com.bangbang93.ForgeInstaller " + "\"" + clt.rootdir + "\"");
        // ExecuteInCmd(java + " -cp \"forge-install-bootstrapper.jar;" + jardir + "\" com.bangbang93.ForgeInstaller " + "\"" + clt.rootdir + "\"", clt.rootdir);
        new FastZip().ExtractZip(tempdir + "\\" + installerVer + "-installer.jar", tempdir,
            "forge-" + installerVer + ".jar");
        new FastZip().ExtractZip(tempdir + "\\" + installerVer + "-installer.jar", tempdir, "version.json");
        infoManager.Info(new Info("解压文件完成", InfoType.info));
        // log.Debug("111");
        //DirectoryInfo dir = new DirectoryInfo(rootdir + "\\libraries\\net\\minecraftforge\\forge\\" + installerVer);
        // if (!dir.Exists) dir.Create();
        // FileInfo fileInfo = new FileInfo(tempdir + "\\maven\\net\\minecraftforge\\forge\\" + installerVer + "\\forge-" + installerVer + ".jar");
        // fileInfo.CopyTo(rootdir + "\\libraries\\net\\minecraftforge\\forge\\" + installerVer + "\\forge-" + installerVer + ".jar", true);
        var verDir = new DirectoryInfo(rootdir + "\\versions\\" + name);
        verDir.Create();
        var verfileInfo = new FileInfo(tempdir + "\\version.json");
        verfileInfo.CopyTo(rootdir + "\\versions\\" + name + "\\" + name + ".json", true);
        var directoryInfo = new DirectoryInfo(tempdir);
        directoryInfo.Delete(true);
        infoManager.Info(new Info("缓存清理完成", InfoType.info));
    }
}