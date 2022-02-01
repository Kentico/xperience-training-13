using CMS.DataEngine;

using Common;

namespace MedioClinic.Customizations.DataProtection.Consent
{
    /// <summary>
    /// Declares members for <see cref="ConsentCookieLevelInfo"/> management.
    /// </summary>
    public interface IConsentCookieLevelInfoProvider : IInfoProvider<ConsentCookieLevelInfo>, IInfoByIdProvider<ConsentCookieLevelInfo>
    {
    }
}