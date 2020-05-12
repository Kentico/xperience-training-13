using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace XperienceLogging
{
    public static class XperienceLoggerExtensions
    {
        public static ILoggingBuilder AddXperience(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, XperienceLoggerProvider>());

            return builder;
        }
    }
}
