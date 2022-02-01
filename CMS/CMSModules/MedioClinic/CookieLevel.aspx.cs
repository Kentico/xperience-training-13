using System;
using System.Linq;

using CMS.Core;
using CMS.DataProtection;
using CMS.Helpers;
using CMS.UIControls;

using MedioClinicCustomizations.Cookies;
using MedioClinicCustomizations.DataProtection.ConsentCustomizations;

namespace CMSApp.CMSModules.MedioClinic
{
    [UIElement("MedioClinic.ConsentCustomizations", "MedioClinic.ConsentCustomizations.CookieLevel")]
    [EditedObject(ConsentInfo.OBJECT_TYPE, "objectid")]
    public partial class CookieLevel : CMSPage
    {
        private readonly IConsentCookieLevelInfoProvider _consentCookieLevelInfoProvider = Service.Resolve<IConsentCookieLevelInfoProvider>();

        private ConsentInfo Consent => EditedObject as ConsentInfo;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ConsentCookieLevelInfo consentCookieLevelInfo = GetConsentCookieLevelInfo(Consent.ConsentID);

            if (consentCookieLevelInfo != null)
            {
                clsCookieLevel.Value = consentCookieLevelInfo.CookieLevel;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var cookieLevel = ValidationHelper.GetInteger(clsCookieLevel.Value, CookieManager.NullIntegerValue);
            var existingConsentCookieLevelInfo = GetConsentCookieLevelInfo(Consent.ConsentID);

            if (existingConsentCookieLevelInfo is null)
            {
                var newConsentCookieLevelInfo = new ConsentCookieLevelInfo
                {
                    ConsentID = Consent.ConsentID,
                    CookieLevel = cookieLevel
                };

                _consentCookieLevelInfoProvider.Set(newConsentCookieLevelInfo);
            }
            else
            {
                existingConsentCookieLevelInfo.CookieLevel = cookieLevel;
                _consentCookieLevelInfoProvider.Set(existingConsentCookieLevelInfo);
            }
        }

        private ConsentCookieLevelInfo GetConsentCookieLevelInfo(int consentId) => 
            _consentCookieLevelInfoProvider.Get()
                .WhereEquals(ConsentManager.ConsentIdColumnName, consentId)
                .TopN(1)
                .FirstOrDefault();
    }
}