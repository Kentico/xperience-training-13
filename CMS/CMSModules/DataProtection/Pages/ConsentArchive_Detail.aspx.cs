using System;

using CMS.DataProtection;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement("CMS.DataProtection", "Consents.ConsentArchive.Detail")]
[EditedObject(ConsentArchiveInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_DataProtection_Pages_ConsentArchive_Detail : CMSPage
{
    private ConsentArchiveInfo ConsentArchive => EditedObject as ConsentArchiveInfo;

    private bool SiteHasMultipleCultures => SiteContext.CurrentSite?.HasMultipleCultures ?? false;

    private string SelectedCulture => SiteHasMultipleCultures ? cultureSelector.Value.ToString() : LocalizationContext.PreferredCultureCode;


    protected void Page_Load(object sender, EventArgs e)
    {
        var parentConsent = ConsentArchive?.Parent as ConsentInfo;
        if (parentConsent != null)
        {
            SetBreadcrumb(0, parentConsent.ConsentDisplayName, GetParentConsentUrl(parentConsent), null, null);
            SetBreadcrumb(1, GetString("dataprotection.consents.consentarchive.archive"), string.Empty, null, null);
        }

        if (SiteHasMultipleCultures)
        {
            SetupCultureSelector(); 
        }
    }


    private static string GetParentConsentUrl(ConsentInfo consent)
    {
        var uiElementUrl = UIContextHelper.GetElementUrl("CMS.DataProtection", "Consents.ConsentArchive");
        return $"{uiElementUrl}&parentobjectid={consent.ConsentID}&displaytitle={false}";
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        htmlConsentFullText.ResolvedValue = ConsentArchive.GetConsentText(SelectedCulture).FullText;
        htmlConsentShortText.ResolvedValue = ConsentArchive.GetConsentText(SelectedCulture).ShortText;
    }


    private void SetupCultureSelector()
    {
        CurrentMaster.DisplaySiteSelectorPanel = true;
        cultureSelector.UniSelector.DropDownSingleSelect.AutoPostBack = true;

        if (!RequestHelper.IsPostBack())
        {
            cultureSelector.Value = LocalizationContext.PreferredCultureCode;
        }
    }
}