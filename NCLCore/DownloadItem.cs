using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLCore
{
    public class DownloadItem
    {
       public string uri;
       public string dir;
        public string name;
        public DownloadItem(string uri, string dir,string name)
        {
            this.uri = uri; 
            this.dir = dir;
            this.name = name;
            
        }
    }
}
