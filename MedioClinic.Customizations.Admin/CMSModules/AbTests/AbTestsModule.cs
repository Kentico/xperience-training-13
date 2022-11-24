using System;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.OnlineMarketing;

using Common.Configuration;
using MedioClinic.Customizations.AbTests;

[assembly: RegisterModule(typeof(AbTestsModule))]

namespace MedioClinic.Customizations.AbTests
{
    public class AbTestsModule : Module
    {
        public AbTestsModule() : base("MedioClinic.AbTests")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            var colors = string.Join(Environment.NewLine, MaterializeCss.Colors.Keys);

            // Prepares a definition for a 'Drop-down list' form control containing the specified options
            var colorFormControlDefinition = new ABTestFormControlDefinition(
                "DropDownListControl",
                "{$MedioClinic.AbTest.Conversion.ButtonColor.Title$}",
                new Dictionary<string, object> 
                { 
                    { "options", colors }
                }
            );

            // Defines a custom conversion type
            var conversionTypeDefinition = new ABTestConversionDefinition(
                ButtonColorConversion.ConversionName, 
                ButtonColorConversion.ConversionTitle,
                colorFormControlDefinition);

            // Registers the custom conversion type
            ABTestConversionDefinitionRegister.Instance.Register(conversionTypeDefinition);
        }
    }
}