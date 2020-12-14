using FsChat.Interfaces;
using FsChat.Interfaces.Logging;
using NLog;
using System;

namespace FsChat.Providers.Logging
{
    public class NLogProvider : ILogProvider
    {
        private static Logger logger;

        public NLogProvider(string loggerName)
        {
            logger = LogManager.GetLogger(loggerName);
        }

        public void Trace(string methodName, string message)
        {
            logger.Log(LogLevel.Info, $"{methodName}: { message }");
        }

        public void Trace(Exception ex, string methodName, string message = null)
        {
            if (message != null)
            {
                logger.Error(ex, $"{methodName}: { message }");
                return;
            }

            logger.Error(ex, $"{methodName}");
        }

        public void Debug(string methodName, string message)
        {
#if DEBUG
            logger.Log(LogLevel.Info, $"{methodName}: { message }");
#endif
        }

        public void Debug(Exception ex, string methodName, string message = null)
        {
#if DEBUG
            if (message != null)
            {
                logger.Error(ex, $"{methodName}: { message }");
                return;
            }

            logger.Error(ex, $"{methodName}");
#endif
        }
    }
}
