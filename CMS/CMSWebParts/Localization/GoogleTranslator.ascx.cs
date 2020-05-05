using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_Localization_GoogleTranslator : CMSAbstractWebPart
{
    #region "Page events"

    /// <summary>
    /// Loads the web part content.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Sets up the control.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (PortalContext.ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
            {
                string culture = CultureHelper.GetShortCultureCode(DocumentContext.CurrentDocumentCulture.CultureCode);

                // Registers Google Translator scripts
                string translateScript = "function googleTranslateElementInit() {new google.translate.TranslateElement({pageLanguage: '" + culture + "'}, 'google_translate_element');}";
                ScriptHelper.RegisterClientScriptBlock(Page, Page.GetType(), "GoogleTranslatorScript", translateScript, true);
                ScriptHelper.RegisterScriptFile(Page, "http://translate.google.com/translate_a/element.js?cb=googleTranslateElementInit");
            }
        }
    }

    #endregion
}
