using System;
using System.Collections.Generic;
using System.Text;

namespace Time.Core.Logging
{
    public interface ILogger
    {
        void Log(LogLevel level, string message, ILogContext logContext, Exception exception = null);
        void Log(LogLevel level, ILogContext logContext, Exception exception = null);
        void Log(LogLevel level, string message, Exception exception = null);
    }
}
