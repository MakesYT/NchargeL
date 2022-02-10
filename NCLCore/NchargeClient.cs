using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace NCLCore
{
    public class NchargeClient
    {
        public string name { get; set; }
        public string Cname { get; set; }
        public string time { get; set; }
        public string version { get; set; }
        public string NchargeVersion { get; set; }
        public string forgeVersion { get; set; }
        public string modsize { get; set; }
        public JArray mods { get; set; }
    }
}
