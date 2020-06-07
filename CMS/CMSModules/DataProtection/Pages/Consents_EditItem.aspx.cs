using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataProtection;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement("CMS.DataProtection", "Consents.ConsentText")]
[EditedObject(ConsentInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_DataProtection_Pages_Consents_EditItem : CMSPage
{
    private ConsentInfo Consent => EditedObject as ConsentInfo;

    private bool SiteHasMultipleCultures => SiteContext.CurrentSite?.HasMultipleCultures ?? false;

    private string SelectedCulture => SiteHasMultipleCultures ? cultureSelector.Value.ToString() : LocalizationContext.PreferredCultureCode;


    protected void Page_Load(object sender, EventArgs e)
    {
        SetupSaveButton();

        if (SiteHasMultipleCultures)
        {
            SetupCultureSelector();
        }

        if (HasExistingAgreement())
        {
            DisplayExistingAgreementInfoMessage();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        htmlConsentShortText.DialogCultureCode = SelectedCulture;
        htmlConsentFullText.DialogCultureCode = SelectedCulture;

        var consentText = Consent.GetConsentText(SelectedCulture);
        htmlConsentShortText.ResolvedValue = consentText.ShortText;
        htmlConsentFullText.ResolvedValue = consentText.FullText;
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


    private void SetupSaveButton()
    {
        // Setup Save button
        HeaderActions.AddAction(new SaveAction()
        {
            Text = SiteHasMultipleCultures ? GetString("dataprotection.consents.saveconsenttext") : GetString("general.save"),
            Enabled = Consent.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser)
        });
        HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                if (Consent.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser))
                {
                    Consent.UpsertConsentText(SelectedCulture, htmlConsentShortText.ResolvedValue, htmlConsentFullText.ResolvedValue);
                    ShowChangesSaved();
                }
                break;
        }
    }


    private bool HasExistingAgreement()
    {
        return ConsentAgreementInfo.Provider
                .Get()
                .WhereEquals("ConsentAgreementConsentID", Consent.ConsentID)
                .Count > 0;
    }


    private void DisplayExistingAgreementInfoMessage()
    {
        ShowInformation(GetString("dataprotection.app.consenthasagreement"));
    }
}