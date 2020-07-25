namespace Abstractions
{
    /// <summary>
    /// Provides different ways to access app options, based on environment.
    /// </summary>
    /// <typeparam name="TOptions">App options.</typeparam>
    public interface IOptionsService<TOptions>
        where TOptions : class, new()
    {
        /// <summary>
        /// App options.
        /// </summary>
        public TOptions Options { get; }
    }
}
