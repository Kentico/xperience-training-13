using CMS.DataEngine;

namespace MedioClinicCustomizations.DataProtection.ConsentCustomizations
{
    /// <summary>
    /// Class providing <see cref="ConsentCookieLevelInfo"/> management.
    /// </summary>
    [ProviderInterface(typeof(IConsentCookieLevelInfoProvider))]
    public class ConsentCookieLevelInfoProvider : AbstractInfoProvider<ConsentCookieLevelInfo, ConsentCookieLevelInfoProvider>, IConsentCookieLevelInfoProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentCookieLevelInfoProvider"/> class.
        /// </summary>
        public ConsentCookieLevelInfoProvider()
            : base(ConsentCookieLevelInfo.TYPEINFO)
        {
        }
    }
}