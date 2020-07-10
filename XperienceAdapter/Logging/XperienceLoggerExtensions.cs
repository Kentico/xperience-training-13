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
    }
}
