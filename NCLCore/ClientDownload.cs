using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLCore
{
    
    public class ClientDownload
    {
        public string? DownloadSoureURL;
        public string? minecraftDir;
        public void DownloadOfficialClient(string ver)
        {
            DownloadManager downloadManager = new DownloadManager();
            downloadManager.Add(new DownloadItem(DownloadSoureURL + "version/" + ver + "/client", minecraftDir + "\\versions\\" + ver,ver+".jar"));
            downloadManager.Add(new DownloadItem(DownloadSoureURL + "version/" + ver + "/json", minecraftDir + "\\versions\\" + ver,ver+".json"));
            downloadManager.Start(2);
        }
    }
}
