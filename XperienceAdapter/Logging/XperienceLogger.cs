using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

using CMS.Core;
using CMS.Base;

namespace XperienceAdapter.Logging
{
    public class XperienceLogger : ILogger
    {
        private readonly string _name;

        private readonly IEventLogService _eventLogService;

        private readonly ISiteService _siteService;

        public XperienceLogger(string name, IEventLogService eventLogService, ISiteService siteService)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
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

            // Sadly, we can't pass around the original format of "state" into the processor.
            // There wouldn't be any point in making the whole processor class generic.\
            // We need to format the state here.
            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                var eventType = MapLogLevel(logLevel);
                var siteId = _siteService?.CurrentSite?.SiteID ?? default;

                var eventData = new EventLogData(eventType, _name, message)
                {
                    SiteID = siteId,
                    Exception = exception
                };

                // Xperience buffers all logged events in the memory, up to the point when
                // the database is available. No need to buffer or ask about the database
                // availability.
                try
                {
                    _eventLogService.LogEvent(eventData);
                }
                catch
                {
                }
            }
        }

        private EventTypeEnum MapLogLevel(LogLevel logLevel) =>
            logLevel switch
            {
                LogLevel.Trace => EventTypeEnum.Information,
                LogLevel.Debug => EventTypeEnum.Information,
                LogLevel.Information => EventTypeEnum.Information,
                LogLevel.Warning => EventTypeEnum.Warning,
                LogLevel.Error => EventTypeEnum.Error,
                LogLevel.Critical => EventTypeEnum.Error,
                LogLevel.None => EventTypeEnum.Error,
                _ => EventTypeEnum.Information,
            };
    }
}
