using System;
using System.Text;
using Newtonsoft.Json;

namespace Time.Core.Logging
{
    public class JsonLogContext<T> : ILogContext
    {
        public string Message { get; private set; }
        public T LogObject { get; set; }
        public JsonLogContext(T logObject, string message = null)
        {
            if (logObject == null)
                logObject = default(T);

            LogObject = logObject;
            Message = message;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Message))
                sb.AppendLine(string.Format("Message: {0}", Message));

            if (LogObject != null)
                sb.AppendLine(string.Format("Object Json Content type={0}, hash={1}, body={2}",
                    typeof(T).Name,
                    LogObject.GetHashCode(),
                    JsonConvert.SerializeObject(LogObject)));
            else
                sb.AppendLine("Object Json Content: NULL");

            return sb.ToString();
        }
    }
}
