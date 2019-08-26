using System;
using System.Collections.Generic;
using System.Text;

namespace Time.Core.Logging
{
    public static class LoggerExtension
    {
        #region Debug level extensions

        public static void LogDebug(this ILogger logger, string message, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Debug, message, logContext, exception);
        }

        public static void LogDebug(this ILogger logger, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Debug, logContext, exception);
        }

        public static void LogDebug(this ILogger logger, string message, Exception exception = null)
        {
            logger.Log(LogLevel.Debug, message, exception);
        }

        #endregion End of Debug level extensions

        #region Info level extensions

        public static void LogInfo(this ILogger logger, string message, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Info, message, logContext, exception);
        }

        public static void LogInfo(this ILogger logger, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Info, logContext, exception);
        }

        public static void LogInfo(this ILogger logger, string message, Exception exception = null)
        {
            logger.Log(LogLevel.Info, message, exception);
        }

        #endregion end of Info level extensions

        #region Warning level extensions

        public static void LogWarning(this ILogger logger, string message, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Warning, message, logContext, exception);
        }

        public static void LogWarning(this ILogger logger, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Warning, logContext, exception);
        }

        public static void LogWarning(this ILogger logger, string message, Exception exception = null)
        {
            logger.Log(LogLevel.Warning, message, exception);
        }

        #endregion End of Warning level extensions

        #region Error level extensions

        public static void LogError(this ILogger logger, string message, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Error, message, logContext, exception);
        }

        public static void LogError(this ILogger logger, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Error, logContext, exception);
        }

        public static void LogError(this ILogger logger, string message, Exception exception = null)
        {
            logger.Log(LogLevel.Error, message, exception);
        }

        #endregion End of Error level extensions

        #region Fatal Fatal level extensions

        public static void LogFatal(this ILogger logger, string message, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Fatal, message, logContext, exception);
        }

        public static void LogFatal(this ILogger logger, ILogContext logContext, Exception exception = null)
        {
            logger.Log(LogLevel.Fatal, logContext, exception);
        }

        public static void LogFatal(this ILogger logger, string message, Exception exception = null)
        {
            logger.Log(LogLevel.Fatal, message, exception);
        }

        #endregion End of Fatal level extensions
    }
}
