using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace XperienceLogging
{
    public class XperienceLogger : ILogger
    {
        private readonly string _name;

        private readonly XperienceLoggerProcessor _processor;

        public XperienceLogger(string name, XperienceLoggerProcessor processor)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                var logEntry = new XperienceLogEntry(logLevel, _name, eventId.Id, message, exception);
                _processor.Enqueue(logEntry);
            }
        }
    }
}
