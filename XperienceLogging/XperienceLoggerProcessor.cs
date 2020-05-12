using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using CMS.Core;

namespace XperienceLogging
{
    public class XperienceLoggerProcessor
    {
        private readonly ConcurrentQueue<XperienceLogEntry> _queue = new ConcurrentQueue<XperienceLogEntry>();

        private readonly IEventLogService _eventLogService;

        public XperienceLoggerProcessor(IEventLogService eventLogService)
        {
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            Task.Run(() => ProcessQueue());
        }

        public void Enqueue(XperienceLogEntry logEntry)
        {
            _queue.Enqueue(logEntry);
        }

        private Task ProcessQueue()
        {
            do
            {
                Thread.Sleep(5000);

                for (var i = 0; i < _queue.Count; i++)
                {
                    if (_queue.TryDequeue(out var logEntry))
                    {
                        var eventType = MapLogLevel(logEntry.LogLevel);
                        var eventData = new EventLogData(eventType, logEntry.Name, logEntry.Message);

                        try
                        {
                            _eventLogService.LogEvent(eventData);
                        }
                        catch
                        {
                        }
                    }
                }
            } while (true);
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
