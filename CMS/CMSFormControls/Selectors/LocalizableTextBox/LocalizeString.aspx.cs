using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.UIControls;


[Title("localizable.string")]
public partial class CMSFormControls_Selectors_LocalizableTextBox_LocalizeString : CMSModalPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        Save += btnOk_Click;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (resEditor.Save())
        {
            string parentTextbox = QueryHelper.GetString("parentTextbox", String.Empty);
            string parentHidden = QueryHelper.GetString("hiddenValueControl", String.Empty);

            string resStringKey = resEditor.ResourceStringKey;
            string cultureCode = CultureHelper.PreferredUICultureCode;
            if (String.IsNullOrEmpty(cultureCode))
            {
                cultureCode = CultureHelper.DefaultUICultureCode;
            }

            using (LocalizationActionContext context = new LocalizationActionContext())
            {
                context.ResolveSubstitutionMacros = false;
                string defaultTranslation = RemoveMacroSignatures(GetString(resStringKey, cultureCode));

                string script = null;

                if (!String.IsNullOrEmpty(parentTextbox))
                {
                    script = String.Format("wopener.SetTranslation({0}, {1}, {2}, {3}); CloseDialog();",
                        ScriptHelper.GetString(parentTextbox), ScriptHelper.GetString(defaultTranslation), ScriptHelper.GetString(parentHidden), ScriptHelper.GetString(resStringKey));
                }
                else
                {
                    script = "CloseDialog();";
                }

                ScriptHelper.RegisterStartupScript(this, typeof(string), "localizeString", ScriptHelper.GetScript(script));
            }
        }
    }


    /// <summary>
    /// Removes the macro signatures from the input text
    /// </summary>
    /// <param name="input">Input text</param>
    private static string RemoveMacroSignatures(string input)
    {
        if (!string.IsNullOrEmpty(input) && QueryHelper.GetBoolean("processMacroSecurity", false))
        {
            bool containsMacro;

            var valueIsMacro = QueryHelper.GetBoolean("valueIsMacro", false);

            // If value is macro, convert to full macro expression
            if (valueIsMacro)
            {
                input = "{%" + input + "%}";
                containsMacro = true;
            }
            else
            {
                containsMacro = input.Contains("{");
            }

            // Remove the signatures
            if (containsMacro)
            {
                input = MacroSecurityProcessor.RemoveSecurityParameters(input, true, null);
            }
            
            // If value is macro, trim back the full expression
            if (valueIsMacro)
            {
                input = input.Substring(2, input.Length - 4);
            }
        }

        return input;
    }
}
