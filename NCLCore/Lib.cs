using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLCore
{
    public class Lib
    {
        public string path { get; set; }
        public string url { get; set; }
        public string sha1 { get; set; }
        public bool native { get; set; }= false;
    }
}
