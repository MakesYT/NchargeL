using System.ComponentModel;
using System.Diagnostics;
using Downloader;
using log4net;

namespace NCLCore;

public class DownloadManagerV2
{
    private static readonly ILog log = LogManager.GetLogger("DownloadManagerV2");
    
    private readonly InfoManager infoManager;
    private DownloadReslut downloadReslut=new DownloadReslut();
    private readonly bool returnProcess;
    private int AllCount;
    private int cancellationsOccurrenceCount;
    private int DownloadCount;
    
    private List<DownloadItem> Hashs = new();

    private int nowthreadnum;

    public DownloadManagerV2(InfoManager infoManager)
    {
        this.infoManager = infoManager;
    }

    public DownloadManagerV2(InfoManager infoManager, bool a)
    {
        this.infoManager = infoManager;
        returnProcess = a;
    }
    //private InfoManager infoManager = null;
    
    public  DownloadReslut Start(List<DownloadItem> list, int thread)
    {
        Hashs = list;
        infoManager.Info(new Info("共需下载" + Hashs.Count, InfoType.info));
        
        log.Debug(Hashs.Count);
        AllCount = Hashs.Count;
        while (Hashs.Count != 0 || nowthreadnum != 0||DownloadCount<AllCount)
        while (nowthreadnum < thread)
        {
             
            //log.Debug(nowthreadnum+"   "+ Hashs.Count);
            Thread.Sleep(10);
            if (Hashs.Count > 0 && nowthreadnum < thread)
            {
                nowthreadnum++;
                var hash = Hashs.First();
                Hashs.Remove(hash);
                    //infoManager.Info(new Info(hash.uri, InfoType.info));
                Task.Factory.StartNew(() => ExecuteInCmd(hash));
            }
            else if (nowthreadnum == 0)
            {
                break;
            }
        }

        //infoManager.Info( new Info("下载" + "客户端完成",InfoType.success));
        if (downloadReslut.allSuccess ==false)
        {
            //infoManager.Info(new Info("有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error, InfoType.errorDia));
            //infoManager.log = "有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error;
            log.Debug("有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + downloadReslut.error);
            return downloadReslut;
        }

        return new DownloadReslut(true);
    }
   

    private async Task ExecuteInCmd(DownloadItem hash)
    {
        var finnsh = false;
       
            
            // process.StandardInput.AutoFlush = true;
            var directoryInfo =
                new DirectoryInfo(hash.fullname.Substring(0, hash.fullname.LastIndexOf("\\") + 1));
            if (!directoryInfo.Exists) directoryInfo.Create();
             Downloading downloading = new Downloading(hash);
        //log.Info("\"" + Directory.GetCurrentDirectory() + "\\Resources\\wget.exe\" \"" + hash.uri + "\" -O \"" + hash.fullname + "\"" + "&exit");
        //log.Debug(hash.uri+" "+ hash.fullname);
       await downloading.StartAsync();
        Console.WriteLine("11");

        DownloadCount++;
        infoManager.Info(new Info(0, DownloadCount + "/" + AllCount));
        nowthreadnum--;
        downloadReslut.addReslut(downloading.GetDownloadReslut());
        //AllCount--;


        //ClientDownload.log = DownloadCount ;

    }
    }
