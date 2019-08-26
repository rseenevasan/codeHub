using log4net;
using log4net.Repository;
using System;
using System.Reflection;

namespace Time.Core.Logging
{
    public class Log4NetLogger : ILogger
    {
        private ILog _logger { get; set; }
        private ILoggerRepository _repository { get; set; }

        private string _loggerName { get; set; }

        public Log4NetLogger()
            : this("defaultlogger")
        {
            _repository = log4net.LogManager.CreateRepository(
            Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
        }

        public Log4NetLogger(string loggerName)
        {
            _loggerName = loggerName;
            _logger = LogManager.GetLogger(_repository.Name, _loggerName);
        }

        public void Log(LogLevel level, string message, ILogContext logContext, Exception exception = null)
        {
            var objMessage = logContext.ToString();
            string logMessage = string.Format("Message: {0} {1} {2}", message, Environment.NewLine, objMessage);
            LogMessage(level, logMessage, exception);
        }

        public void Log(LogLevel level, ILogContext logContext, Exception exception = null)
        {
            var logMessage = logContext.ToString();
            LogMessage(level, logMessage, exception);
        }

        public void Log(LogLevel level, string message, Exception exception = null)
        {
            LogMessage(level, message, exception);
        }

        private void LogMessage(LogLevel logLevel, string message, Exception exception = null)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    _logger.Debug(message, exception);
                    break;
                case LogLevel.Info:
                    _logger.Info(message, exception);
                    break;
                case LogLevel.Warning:
                    _logger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    _logger.Error(message, exception);
                    break;
                case LogLevel.Fatal:
                    _logger.Fatal(message, exception);
                    break;
                default:
                    _logger.Info(message, exception);
                    break;
            }
        }
    }
}
