﻿using System.Collections.Generic;

using CMS;
using CMS.Activities;
using CMS.ContactManagement;
using CMS.DataEngine;

using MedioClinic.Customizations.Admin.CMSModules.Macros;
using MedioClinic.Customizations.Macros;

[assembly: RegisterModule(typeof(CustomMacrosModule))]

namespace MedioClinic.Customizations.Admin.CMSModules.Macros
{
    public class CustomMacrosModule : Module
    {
        public CustomMacrosModule() : base("CustomMacros")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            var clickedLinkInEmailInLastDaysMetadata = new MacroRuleMetadata(ContactInfoMethods.ContactHasClickedLinkInEmailInLastDaysName,
                                     new ContactHasClickedLinkInEmailInLastDaysTranslator(),
                                     new List<string>
                                     {
                                         PredefinedActivityType.NEWSLETTER_CLICKTHROUGH
                                     },
                                     new List<string>(0));

            MacroRuleMetadataContainer.RegisterMetadata(clickedLinkInEmailInLastDaysMetadata);
        }
    }
}