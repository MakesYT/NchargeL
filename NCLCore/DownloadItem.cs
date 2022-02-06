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
        
        public DownloadItem(string uri, string dir)
        {
            this.uri = uri; 
            this.dir = dir;

        }
    }
}
