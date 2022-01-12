using Common;

using System.Collections.Generic;

namespace MedioClinicCustomizations.Cookies
{
    public interface ICookieManager : IService
    {
        IEnumerable<CookieLevel> GetCookieLevels();

        IEnumerable<CookieLevel> GetCookieLevels(IEnumerable<string> cookieNames);
        
        IEnumerable<Cookie> GetCookies();

        IEnumerable<Cookie> GetCookies(IEnumerable<string> cookieNames);

        IEnumerable<CookieLevel> GetSystemCookieLevels();

        bool IsDefaultCookieLevel { get; }

        bool VisitorCookiesEnabled { get; }
    }
}