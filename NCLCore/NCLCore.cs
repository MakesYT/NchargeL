using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NCLCore;


namespace NCLCore
{
    public class NCLcore
    { 
        SDK sDK=new SDK();
        public string DownloadSoureURL { get; set; }
        public string GameDir { get; set; }
        public ObservableCollection<Client> Clients { get; set; }
       public NCLcore(SDK.DownloadSource ds,string dir)
        {
            DownloadSoureURL = sDK.GetDownloadSoure(ds);
            GameDir = dir;
            Clients = sDK.GetALLClient(dir);
        }

       
    }
}
