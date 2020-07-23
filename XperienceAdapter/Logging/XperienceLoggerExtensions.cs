using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace XperienceAdapter.Logging
{
    /// <summary>
    /// Xperience logger extensions.
    /// </summary>
    public static class XperienceLoggerExtensions
    {
        /// <summary>
        /// Configures the builder to use <see cref="XperienceLoggerProvider"/>.
        /// </summary>
        /// <param name="builder">Logging builder.</param>
        /// <returns>The configured builder.</returns>
        public static ILoggingBuilder AddXperience(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, XperienceLoggerProvider>());

            return builder;
        }

        /// <summary>
        /// Logs events.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="logLevel">Log level.</param>
        /// <param name="methodName">Method name.</param>
        /// <param name="message">Log message.</param>
        /// <param name="exception">Exception.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void LogEvent(this ILogger logger, LogLevel logLevel, string? methodName, string? message = default, Exception? exception = default, params object[] args)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                var eventId = new EventId(0, methodName);

                logger.Log(logLevel, eventId, exception, message, args);
            }
            else
            {
                logger.Log(logLevel, exception, message, args);
            }
        }
    }
}
