using CMS.DataEngine;

namespace MedioClinic.Customizations.DataProtection.Consent
{
    /// <summary>
    /// Declares members for <see cref="ConsentCookieLevelInfo"/> management.
    /// </summary>
    public partial interface IConsentCookieLevelInfoProvider : IInfoProvider<ConsentCookieLevelInfo>, IInfoByIdProvider<ConsentCookieLevelInfo>
    {
    }
}