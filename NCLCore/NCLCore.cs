using System;
using System.Collections.ObjectModel;


namespace NCLCore
{
    public class NCLcore
    {
        public SDK sDK = new SDK();
        
        public string GameDir { get; set; }
        public ObservableCollection<Client> Clients { get; set; }
        public NCLcore(SDK.DownloadSource ds, string dir)
        {
            sDK.DownloadSoureURL = sDK.GetDownloadSoure(ds);

            GameDir = dir;
            Clients = sDK.GetALLClient(dir);
        }
        

    }
}
