using log4net;

using Microsoft.Extensions.Logging;

namespace Yee.Common.Library.LogHelper
{
    public class Log
    {
        private string _logName;
        public Log(string logName)
        {
            _logName = logName;
        }
        public void WriteLog(LogLevel level,object message)
        {
            ILog logger = CustomRollingFileLogger.GetCustomLogger(_logName);

            switch (level)
            {
                case LogLevel.Error:
                    logger.Error(message);
                    break;
                case LogLevel.Warning:
                    logger.Warn(message);
                    break;
                case LogLevel.Information:
                    logger.Info(message);
                    break;
                default:
                    logger.Info(message);
                    break;
            }    
        }
    }
}
