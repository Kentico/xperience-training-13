using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

using CMS.Core;
using CMS.Base;

namespace XperienceAdapter.Logging
{
    public class XperienceLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, XperienceLogger> _loggers = new ConcurrentDictionary<string, XperienceLogger>();

        private readonly IEventLogService _eventLogService;

        private readonly ISiteService _siteService;

        public XperienceLoggerProvider(IEventLogService eventLogService, ISiteService siteService)
        {
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new XperienceLogger(name, _eventLogService, _siteService));

        public void Dispose() => _loggers.Clear();
    }
}
