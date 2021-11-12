using Core;

namespace XperienceAdapter.Cookies
{
    public interface ICookieManager : IService
    {
        // TODO: Document.
        bool IsDefaultCookieLevel { get; }

        bool VisitorCookiesEnabled { get; }
    }
}