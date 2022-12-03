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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="thread"></param>
    /// <param name="flag">如果为True强制使用WebClient直接请求文件,默认false自动选择</param>
    /// <returns></returns>
    public  DownloadReslut Start(List<DownloadItem> list, int thread,bool flag=false)
    {
        cancellationsOccurrenceCount = 0;
        downloadReslut = new DownloadReslut();
        Hashs = list;
        infoManager.Info(new Info("共需下载" + Hashs.Count, InfoType.info));
        DownloadCount = 0;
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
                Task.Factory.StartNew(() => ExecuteInCmd(hash,flag));
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
            log.Debug("有" + downloadReslut.downloadItems.Count + "个文件下载失败\n错误信息" + downloadReslut.error);
            return downloadReslut;
        }

        return new DownloadReslut(true);
    }
   

    private async Task ExecuteInCmd(DownloadItem hash,bool flag,int times=0)
    {
        times++;
        Downloading downloading = new(hash,flag);
        try
        {
            var finnsh = false;
       
            
                // process.StandardInput.AutoFlush = true;
                var directoryInfo =
                    new DirectoryInfo(hash.fullname.Substring(0, hash.fullname.LastIndexOf("\\") + 1));
                if (!directoryInfo.Exists) directoryInfo.Create();
                 
            //log.Info("\"" + Directory.GetCurrentDirectory() + "\\Resources\\wget.exe\" \"" + hash.uri + "\" -O \"" + hash.fullname + "\"" + "&exit");
            //log.Debug(hash.uri+" "+ hash.fullname);
           await downloading.StartAsync();
           
            if (downloading.GetDownloadReslut().allSuccess)
            {
                DownloadCount++;
                infoManager.Info(new Info(0, DownloadCount + "/" + AllCount));
                nowthreadnum--;
                downloadReslut.addReslut(downloading.GetDownloadReslut());
            }
            else
            {
                if (times < 5)
                {
                    await Task.Factory.StartNew(() => ExecuteInCmd(hash, flag, times));
                }
                else
                {
                    DownloadCount++;
                    infoManager.Info(new Info(0, DownloadCount + "/" + AllCount));
                    nowthreadnum--;

                    downloadReslut.addReslut(downloading.GetDownloadReslut());
                }
            }
            

        }catch(Exception e)
        {
            if (times < 5)
            {
               await Task.Factory.StartNew(() => ExecuteInCmd(hash, flag,times));
            }
            else
            {
                DownloadCount++;
                infoManager.Info(new Info(0, DownloadCount + "/" + AllCount));
                nowthreadnum--;

                downloadReslut.addReslut(downloading.GetDownloadReslut());
            }
           
        }finally
        {
            downloading = null;
        }
        
        
        //AllCount--;


        //ClientDownload.log = DownloadCount ;

    }
    }
