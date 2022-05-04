using log4net;

namespace NCLCore;

public class NCLException : Exception
{
    private static readonly ILog log = LogManager.GetLogger("NCLExcetion");

    public NCLException(string message) : base(message)
    {
        log.Error(message);
    }
}