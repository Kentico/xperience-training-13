using System;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine.Routing;
using CMS.DocumentEngine.Routing.Internal;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

[assembly: RegisterCustomClass("CultureEditExtender", typeof(CultureEditExtender))]

/// <summary>
/// Culture uiform extender.
/// </summary>
public class CultureEditExtender : ControlExtender<UIForm>
{
    private string originalCultureAliasValue;

    /// <summary>
    /// Init event of the UI form.
    /// </summary>
    public override void OnInit()
    {
        Control.SubmitButton.OnClientClick = String.Format("return window.confirm({0})", ScriptHelper.GetString(ResHelper.GetString("culture.editcultureconfirmation")));
        Control.OnAfterValidate += OnAfterValidate;
        Control.OnBeforeSave += OnBeforeSave;
        Control.OnAfterSave += OnAfterSave;
    }


    /// <summary>
    /// Handles OnAfterValidate event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event argument.</param>
    protected void OnAfterValidate(object sender, EventArgs e)
    {
        int cultureId = Control.EditedObject.Generalized.ObjectID;
        string cultureCode = ValidationHelper.GetString(Control.GetFieldValue("CultureCode"), String.Empty).Trim();
        string cultureAlias = ValidationHelper.GetString(Control.GetFieldValue("CultureAlias"), String.Empty).Trim();

        // Check validity of culture code
        if (!CultureHelper.IsValidCultureInfoName(cultureCode))
        {
            Control.ShowError(Control.GetString("Culture.ErrorNoGlobalCulture"));
            Control.StopProcessing = true;
        }

        // Neutral culture cannot be used
        if (CultureHelper.IsNeutralCulture(cultureCode) && !Control.StopProcessing)
        {
            Control.ShowError(Control.GetString("culture.neutralculturecannotbeused"));
            Control.StopProcessing = true;
        }

        // Check if culture already exists for new created cultures
        var cultureByCode = CultureInfoProvider.GetCultureInfoForCulture(cultureCode);
        if ((cultureByCode != null) && (cultureByCode.CultureID != cultureId) && !Control.StopProcessing)
        {
            Control.ShowError(Control.GetString("culture_new.cultureexists"));
            Control.StopProcessing = true;
        }

        // Check whether culture alias is unique
        if (!String.IsNullOrEmpty(cultureAlias) && !Control.StopProcessing)
        {
            CultureInfo cultureByAlias = CultureInfoProvider.GetCultureInfoForCulture(cultureAlias);
            if (((cultureByAlias != null) && (cultureByAlias.CultureID != cultureId)) || CMSString.Equals(cultureAlias, cultureCode, true))
            {
                Control.ShowError(Control.GetString("Culture.AliasNotUnique"));
                Control.StopProcessing = true;
            }
        }

        // Show warning if culture is UI and there is no resx file
        bool isUiCulture = ValidationHelper.GetBoolean(Control.GetFieldValue("CultureIsUICulture"), false);
        if (!Control.StopProcessing && !LocalizationHelper.ResourceFileExistsForCulture(cultureCode) && isUiCulture)
        {
            string url = "https://devnet.kentico.com/download/localization-packs";
            string downloadPage = String.Format(@"<a href=""{0}"" target=""_blank"" >{1}</a> ", url, HTMLHelper.HTMLEncode(url));
            Control.ShowWarning(String.Format(Control.GetString("culture.downloadlocalization"), downloadPage));
        }
    }


    /// <summary>
    /// Handles OnBeforeSave event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event argument.</param>
    protected void OnBeforeSave(object sender, EventArgs e)
    {
        originalCultureAliasValue = Control.EditedObject.GetOriginalValue("CultureAlias").ToString(string.Empty);
    }


    /// <summary>
    /// Handles OnAfterSave event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="e">Event argument.</param>
    protected void OnAfterSave(object sender, EventArgs e)
    {
        var newCultureAlias = Control.EditedObject.GetValue("CultureAlias").ToString(string.Empty);

        if (originalCultureAliasValue.Equals(newCultureAlias, StringComparison.InvariantCulture))
        {
            return;
        }

        var cultureCode = ValidationHelper.GetString(Control.GetFieldValue("CultureCode"), string.Empty).Trim();

        var sites = CultureSiteInfoProvider.GetCultureSites(cultureCode);

        foreach (var site in sites)
        {
            if (IsLanguagePrefixChangeRequired(site, cultureCode))
            {
                new PageUrlPathLanguagePrefixFormatChanger(site.SiteID).ChangeForCulture(PageRoutingUrlLanguagePrefixFormatEnum.CultureAlias, cultureCode);
            }
        }
    }


    private static bool IsLanguagePrefixChangeRequired(SiteInfo site, string cultureCode)
    {
        return PageRoutingHelper.GetRoutingMode(site.SiteID) == PageRoutingModeEnum.BasedOnContentTree &&
            PageRoutingHelper.GetUrlCultureFormat(site.SiteID) == PageRoutingUrlCultureFormatEnum.LanguagePrefix &&
            PageRoutingHelper.UseCultureAliasAsLanguagePrefixInUrl(site.SiteID) &&
            (!string.Equals(CultureHelper.GetDefaultCultureCode(site.SiteName), cultureCode, StringComparison.InvariantCultureIgnoreCase) || !PageRoutingHelper.HideLanguagePrefixForDefaultCultureUrl(site.SiteID));
    }
}