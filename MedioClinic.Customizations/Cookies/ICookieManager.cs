using Common;

using System.Collections.Generic;

namespace MedioClinic.Customizations.Cookies
{
    public interface ICookieManager
    {
        /// <summary>
        /// Gets cookie levels of both system and custom cookies.
        /// </summary>
        /// <returns>Cookie levels.</returns>
        IEnumerable<CookieLevel> GetCookieLevels();

        /// <summary>
        /// Gets cookie levels by names of cookies.
        /// </summary>
        /// <param name="cookieNames">Cookie names.</param>
        /// <returns>Cookie levels.</returns>
        IEnumerable<CookieLevel> GetCookieLevels(IEnumerable<string> cookieNames);

        /// <summary>
        /// Gets all registered cookies.
        /// </summary>
        /// <returns>All cookies.</returns>
        IEnumerable<Cookie> GetCookies();

        /// <summary>
        /// Gets registered cookies by their names.
        /// </summary>
        /// <param name="cookieNames">Cookie names.</param>
        /// <returns>Cookies.</returns>
        IEnumerable<Cookie> GetCookies(IEnumerable<string> cookieNames);

        /// <summary>
        /// Gets system cookie levels.
        /// </summary>
        /// <returns>Cookie levels.</returns>
        IEnumerable<CookieLevel> GetSystemCookieLevels();

        /// <summary>
        /// Determines if the current cookie level is the default one.
        /// </summary>
        bool IsDefaultCookieLevel { get; }

        /// <summary>
        /// Determines if at least visitor cookies are allowed.
        /// </summary>
        bool VisitorCookiesEnabled { get; }
    }
}