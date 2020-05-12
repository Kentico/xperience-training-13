using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

using CMS.Core;

namespace XperienceLogging
{
    class XperienceLoggerProvider : ILoggerProvider
    {
        private readonly XperienceLoggerProcessor _processor;

        private readonly ConcurrentDictionary<string, XperienceLogger> _loggers = new ConcurrentDictionary<string, XperienceLogger>();

        public XperienceLoggerProvider(IEventLogService eventLogService)
        {
            _processor = new XperienceLoggerProcessor(eventLogService);
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new XperienceLogger(name, _processor));

        public void Dispose() =>_loggers.Clear();
    }
}
