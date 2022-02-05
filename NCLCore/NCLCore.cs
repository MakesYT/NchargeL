using System;
using System.Collections.ObjectModel;


namespace NCLCore
{
    public class NCLcore
    {
        public SDK sDK = new SDK();
        public ClientDownload clientDownload = new ClientDownload();
        public string GameDir { get; set; }
        public ObservableCollection<Client> Clients { get; set; }
        public NCLcore(string ds, string dir)
        {
            // sDK.DownloadSoureURL = sDK.GetDownloadSoure(ds);
            sDK.DownloadSoureURL = ds;
            clientDownload.DownloadSoureURL = ds;
            clientDownload.minecraftDir = dir;
            GameDir = dir;
            Clients = sDK.GetALLClient(dir);
        }
        

    }
}
