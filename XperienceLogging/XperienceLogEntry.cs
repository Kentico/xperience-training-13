using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace XperienceLogging
{
    public class XperienceLogEntry
    {
        public LogLevel LogLevel { get; }

        public string Name { get; }

        public int EventId { get; }

        public string? Message { get; }

        public Exception? Exception { get; }

        public XperienceLogEntry(LogLevel logLevel, string name, int eventId, string message, Exception exception)
        {
            LogLevel = logLevel;
            Name = name;
            EventId = eventId;
            Message = message;
            Exception = exception;
        }
    }
}
