using System;
using System.Collections.Generic;
using System.Text;

namespace Time.Core.Logging
{
    public class NullLogger : ILogger
    {
        public void Log(LogLevel level, string message, Exception exception = null)
        {
            return;
        }

        public void Log(LogLevel level, ILogContext logContext, Exception exception = null)
        {
            return;
        }

        public void Log(LogLevel level, string message, ILogContext logContext, Exception exception = null)
        {
            return;
        }
    }
}
