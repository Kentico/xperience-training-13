using System;

using CMS;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Helpers;
using CMS.MacroEngine;

[assembly: RegisterCustomClass("OnlineMarketingMacroRuleEditExtender", typeof(OnlineMarketingMacroRuleEditExtender))]

/// <summary>
/// Extends Macro rule UIForm in Online Marketing - Contact Management - Configuration - Macro rules
/// </summary>
public class OnlineMarketingMacroRuleEditExtender : MacroRuleEditExtender
{
    public override void OnInit()
    {
        // Use PreRender event to handle the warning on both loading the page and saving new values
        Control.PreRender += ShowMacroWarning;
        base.OnInit();
    }


    private void ShowMacroWarning(object sender, EventArgs e)
    {
        MacroRuleInfo info = Control.EditedObject as MacroRuleInfo;
        if (info != null)
        {
            string macroName = info.MacroRuleName;
            if (!MacroRuleMetadataContainer.IsTranslatorAvailable(macroName))
            {
                var text = string.Format(Service.Resolve<ILocalizationService>().GetString("om.configuration.macro.slow"), DocumentationHelper.GetDocumentationTopicUrl("om_macro_performance"));
                Control.ShowWarning(text);
            }
        }
    }
}