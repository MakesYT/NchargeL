using Downloader;
using log4net;
using System.Diagnostics;

namespace NCLCore
{
    internal class NchargeModDownload
    {
        private static readonly ILog log = LogManager.GetLogger("NchargeModDownload");
        List<DownloadItem> Hashs = new List<DownloadItem>();
        public ClientDownload ClientDownload { get; set; }
        public int DownloadCount = 0;
        public int AllCount = 0;
        int cancellationsOccurrenceCount = 0;
        public string minecraftDir;
        public string toDir;
        string error = "";
        public void setList(List<DownloadItem> list)
        {
            Hashs= list;
        }
        int nowthreadnum = 0;
        public void Start(int thread)
        {

            log.Debug(Hashs.Count);
            AllCount = Hashs.Count;
            while (Hashs.Count != 0 || nowthreadnum != 0)
                while (nowthreadnum < thread)
                {
                    //log.Debug(nowthreadnum+"   "+ Hashs.Count);
                    Thread.Sleep(10);
                    if (Hashs.Count > 0&& nowthreadnum < thread)
                    {
                        nowthreadnum++;
                        DownloadItem hash = Hashs.First();
                        Hashs.Remove(hash);
                        Task.Factory.StartNew(() => ExecuteInCmd(minecraftDir + "\\wget.exe \""+hash.uri +"\" -O \""+hash.dir+"\"", toDir));
                        
                       

                    }
                    else if (nowthreadnum == 0) break;
                }
           ClientDownload.log = "下载"  + "客户端完成";
            if (cancellationsOccurrenceCount != 0)
            {
ClientDownload.log = "有" + cancellationsOccurrenceCount + "个文件下载失败\n错误信息" + error;
                log.Debug(ClientDownload.log);
            }
                
        }
        public string ExecuteInCmd(string cmdline, string dir)
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
                process.StandardInput.WriteLine("cd /d " + dir);
                process.StandardInput.WriteLine(cmdline + "&exit");
                process.StandardInput.Close();
                string line;
                string allline="";

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
                //获取cmd窗口的输出信息  
                // string output = process.StandardOutput.ReadToEnd();
                // process.StandardOutput.
                process.WaitForExit();
                process.Close();
                if (!finnsh)
                {
                    cancellationsOccurrenceCount++;
                    error = error + allline;
                }
                nowthreadnum--;
                //AllCount--;
                DownloadCount++;
                ClientDownload.log = DownloadCount+"/" + (AllCount) ;
                return "";
            }
        }





    }
}
