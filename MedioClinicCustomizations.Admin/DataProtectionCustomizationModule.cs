using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.UIControls;

using MedioClinicCustomizations.Cookies;
using MedioClinicCustomizations.DataProtection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: RegisterModule(typeof(DataProtectionCustomizationModule))]

namespace MedioClinicCustomizations.DataProtection
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
            DataProtectionControlsRegister.Instance.RegisterErasureConfigurationControl("~/CMSModules/DataProtectionCustomization/ContactDataEraserConfiguration.ascx");

            // Registers online marketing cookies at their respective cookie levels.
            CookieManager.RegisterOnlineMarketingCookies();
        }
    }
}
