using CMS;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.UIControls;

using MedioClinic.Customizations.Cookies;
using MedioClinic.Customizations.DataProtection;

[assembly: RegisterModule(typeof(DataProtectionCustomizationModule))]

namespace MedioClinic.Customizations.DataProtection
{
    public class DataProtectionCustomizationModule : Module
    {
        public DataProtectionCustomizationModule() : base("MedioClinic.DataProtection")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            // Adds the ContactIdentityCollector to the collection of registered identity collectors.
            IdentityCollectorRegister.Instance.Add(new ContactIdentityCollector());

            // Adds the ContactDataCollector to the collection of registered personal data collectors.
            PersonalDataCollectorRegister.Instance.Add(new ContactDataCollector());

            // Adds the ContactDataEraser to the collection of registered personal data erasers.
            PersonalDataEraserRegister.Instance.Add(new ContactDataEraser());

            // Registers the custom eraser configuration control.
            DataProtectionControlsRegister.Instance.RegisterErasureConfigurationControl("~/CMSModules/MedioClinic.DataProtection/ContactDataEraserConfiguration.ascx");

            // Registers online marketing cookies at their respective cookie levels.
            CookieManager.RegisterOnlineMarketingCookies();
        }
    }
}
