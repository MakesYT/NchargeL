using log4net;

namespace NCLCore
{
    public class Info
    {
        public string msg;
        public double process;
        public string TYPE = "info";
        private readonly ILog log = LogManager.GetLogger("Info");
        public Info(string msg, string TYPE)
        {
            this.msg = msg;
            this.TYPE = TYPE;
            log.Debug("[" + TYPE + "]" + msg);
        }
        public Info(double process, string msg)
        {
            this.process = process;
            this.msg = msg;
            //this.TYPE = TYPE;
            log.Debug("[进度条]" + msg);
        }


    }
}
