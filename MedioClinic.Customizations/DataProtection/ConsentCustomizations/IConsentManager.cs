using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;

using Common;

namespace MedioClinic.Customizations.DataProtection.Consent
{
    public interface IConsentManager : IService
    {
        /// <summary>
        /// Gets agreed consents of the current contact that have been assigned a higher than the given cookie level.
        /// </summary>
        /// <param name="contact">Contact.</param>
        /// <param name="cookieLevel">Cookie level</param>
        /// <returns>Consents.</returns>
        ObjectQuery<ConsentInfo> GetAgreedConsentsWithHigherCookieLevel(ContactInfo contact, int cookieLevel);

        /// <summary>
        /// Gets all consents that have been assigned a given cookie level or a lower one.
        /// </summary>
        /// <param name="cookieLevel">Cookie level.</param>
        /// <returns>Consents.</returns>
        ObjectQuery<ConsentInfo> GetAllConsentsWithSameOrLowerCookieLevel(int cookieLevel);
    }
}