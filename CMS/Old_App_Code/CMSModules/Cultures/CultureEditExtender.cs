using System;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine.Web.UI;

[assembly: RegisterCustomClass("CultureEditExtender", typeof(CultureEditExtender))]

/// <summary>
/// Culture uiform extender
/// </summary>
public class CultureEditExtender : ControlExtender<UIForm>
{
    /// <summary>
    /// Init event of the UI form.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAfterValidate += OnAfterValidate;
    }


    /// <summary>
    /// Handles OnAfterValidate event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
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
}