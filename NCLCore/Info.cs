using log4net;

namespace NCLCore
{
    public class Info
    {
        public string msg;
        public double process;
        public InfoType TYPE = InfoType.error;
        private readonly ILog log = LogManager.GetLogger("Info");
        public Info(string msg, InfoType TYPE)
        {
            this.msg = msg;
            this.TYPE = TYPE;
            log.Debug("[" + GetStringType(TYPE) + "]" + msg);
        }
        public Info(double process, string msg)
        {
            this.process = process;
            this.msg = msg;
            //this.TYPE = TYPE;
            log.Debug("[进度条]" + msg);
        }

        static string GetStringType(InfoType infoType)
        {
            return infoType switch
            {
                InfoType.error => "错误",
                InfoType.info => "提示",
                InfoType.warn => "警告",
                InfoType.success => "成功",
                InfoType.errorDia => "错误弹窗",
                InfoType.successDia => "成功弹窗",
                _ => "未知",
            };
        }
    }


}
