using log4net;

namespace NCLCore
{
    public class Info
    {
        public string msg;
        public string TYPE="info";
        private static readonly ILog log = LogManager.GetLogger("Info");
        public Info(string msg, string TYPE)
        {
            this.msg = msg;
            this.TYPE = TYPE;
            log.Debug("["+TYPE+"]"+msg);
        }


    }
}
