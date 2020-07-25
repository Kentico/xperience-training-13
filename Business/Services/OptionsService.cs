using Microsoft.Extensions.Options;

using Abstractions;

namespace Business.Services
{
    /// <summary>
    /// Provides app options in development.
    /// </summary>
    /// <typeparam name="TOptions">App options.</typeparam>
    public class DevelopmentOptionsService<TOptions> : IOptionsService<TOptions>
        where TOptions : class, new()
    {
        /// <summary>
        /// Reloads options for each request.
        /// </summary>
        protected IOptionsSnapshot<TOptions> OptionsSnapshot { get; }

        public DevelopmentOptionsService(IOptionsSnapshot<TOptions> optionsSnapshot)
        {
            OptionsSnapshot = optionsSnapshot;
        }

        public TOptions Options => OptionsSnapshot.Value;
    }

    /// <summary>
    /// Provides app options in production.
    /// </summary>
    /// <typeparam name="TOptions">App options.</typeparam>
    public class ProductionOptionsService<TOptions> : IOptionsService<TOptions>
        where TOptions : class, new()
    {
        /// <summary>
        /// Uses a singleton to provide options.
        /// </summary>
        protected IOptionsMonitor<TOptions> OptionsMonitor { get; }

        public ProductionOptionsService(IOptionsMonitor<TOptions> optionsMonitor)
        {
            OptionsMonitor = optionsMonitor;
        }

        public TOptions Options => OptionsMonitor.CurrentValue;
    }
}
