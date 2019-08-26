using System;
using System.Collections.Generic;
using System.Text;

namespace Time.Core.Logging
{
    public class DefaultLogContext : ILogContext
    {
        public string Message { get; private set; }

        public DefaultLogContext(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Message) ? string.Empty : Message;
        }
    }
}
