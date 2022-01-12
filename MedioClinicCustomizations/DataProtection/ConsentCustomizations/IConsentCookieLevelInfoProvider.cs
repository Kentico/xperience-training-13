using CMS.DataEngine;

using Common;

namespace MedioClinicCustomizations.DataProtection.ConsentCustomizations
{
    /// <summary>
    /// Declares members for <see cref="ConsentCookieLevelInfo"/> management.
    /// </summary>
    public interface IConsentCookieLevelInfoProvider : IInfoProvider<ConsentCookieLevelInfo>, IInfoByIdProvider<ConsentCookieLevelInfo>, IService
    {
    }
}