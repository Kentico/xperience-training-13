using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

using CMS.Core;

namespace XperienceAdapter.Logging
{
    /// <summary>
    /// Provides <see cref="XperienceLogger"/> as an implementation of <see cref="ILogger{TCategoryName}"/>.
    /// </summary>
    public class XperienceLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, XperienceLogger> _loggers = new ConcurrentDictionary<string, XperienceLogger>();

        private readonly IEventLogService _eventLogService;

        public XperienceLoggerProvider(IEventLogService eventLogService)
        {
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }

        /// <summary>
        /// Creates the logger instance for a category.
        /// </summary>
        /// <param name="categoryName">Logging category.</param>
        /// <returns>The logger.</returns>
        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new XperienceLogger(name, _eventLogService));

        public void Dispose() => _loggers.Clear();
    }
}
