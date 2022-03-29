using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLCore
{
    
    public class DownloadManagerV2
    {
        private static readonly ILog log = LogManager.GetLogger("DownloadManagerV2");
        private List<DownloadItem> Hashs = new List<DownloadItem>();
        private List<DownloadItem> cancellationsOccurrenceLists = new List<DownloadItem>();
        private int DownloadCount = 0;
        private int AllCount = 0;
        private int cancellationsOccurrenceCount = 0;
        private string error = "";
        private int nowthreadnum = 0;
        private InfoManager infoManager=null;
        public void setInfoManager(InfoManager infoManager)
        {
            this.infoManager = infoManager;
        }
        public DownloadReslut Start(List<DownloadItem> list,int thread)
        {
            Hashs = list;
            log.Debug(Hashs.Count);
            AllCount = Hashs.Count;
            while (Hashs.Count != 0 || nowthreadnum != 0)
                while (nowthreadnum < thread)
                {
                    //log.Debug(nowthreadnum+"   "+ Hashs.Count);
                    Thread.Sleep(10);
                    if (Hashs.Count > 0 && nowthreadnum < thread)
                    {
                        nowthreadnum++;
                        DownloadItem hash = Hashs.First();
                        Hashs.Remove(hash);
                        Task.Factory.StartNew(() => ExecuteInCmd(hash ));
                    }
                    else if (nowthreadnum == 0) break;
                }
            //infoManager.Info( new Info("下载" + "客户端完成",InfoType.success));
            if (cancellationsOccurrenceCount != 0)
            {
                //infoManager.Info(new Info("有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error, InfoType.errorDia));
                //infoManager.log = "有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error;
                log.Debug("有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error);
                return new DownloadReslut(false,cancellationsOccurrenceLists);
            }
            else
                return new DownloadReslut(true);

        }
        private string ExecuteInCmd(DownloadItem hash)
        {
            bool finnsh = false;
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //process.StartInfo.
                // process.StartInfo.
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                // process.StandardInput.AutoFlush = true;
                process.StandardInput.WriteLine("\"" + Directory.GetCurrentDirectory() + "\\Resources\\wget.exe\"" + hash.uri + "\" -O \"" + hash.fullname + "\"" + "&exit");
                process.StandardInput.Close();
                string line;
                string allline = "";

                while ((line = process.StandardError.ReadLine()) != null)
                {
                    allline = allline + line + "\n";
                    // ClientDownload.log = line;
                    log.Debug(line);
                    if (line.Contains("saved"))
                    {
                        finnsh = true;
                    }
                }
                process.WaitForExit();
                process.Close();
                if (!finnsh)
                {
                    cancellationsOccurrenceCount++;
                    error = error + allline;
                    cancellationsOccurrenceLists.Add(hash);
                }
                nowthreadnum--;
                //AllCount--;
                DownloadCount++;
                if(infoManager !=null)
                infoManager.Info(new Info(DownloadCount.ToString(), InfoType.info));
                //ClientDownload.log = DownloadCount + "/" + (AllCount);
                return "";
            }
        }





    }
}
