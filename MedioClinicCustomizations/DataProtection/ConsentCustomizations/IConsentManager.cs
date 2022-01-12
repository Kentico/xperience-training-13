using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DataProtection;

using Common;

namespace MedioClinicCustomizations.DataProtection.ConsentCustomizations
{
    public interface IConsentManager : IService
    {
        ObjectQuery<ConsentInfo> GetAgreedConsentsWithHigherCookieLevel(ContactInfo contact, int cookieLevel);

        ObjectQuery<ConsentInfo> GetAllConsentsWithSameOrLowerCookieLevel(int cookieLevel);
    }
}