using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLCore
{
    public class NCLException:Exception
    {
        private static readonly ILog log = LogManager.GetLogger("NCLExcetion");
        public NCLException(string message) : base(message)
        {
            log.Error(message);
        }
    }
}
