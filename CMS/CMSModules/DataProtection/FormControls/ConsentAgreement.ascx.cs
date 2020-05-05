using System;

using CMS.FormEngine.Web.UI;
using CMS.Core;
using CMS.DataProtection;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Localization;
using CMS.Helpers;

public partial class CMSModules_DataProtection_FormControls_ConsentAgreement : FormEngineUserControl
{
    private ConsentInfo mSelectedConsent;


    private ConsentInfo SelectedConsent
    {
        get
        {
            if (mSelectedConsent != null)
            {
                return mSelectedConsent;
            }

            return mSelectedConsent = ConsentInfoProvider.GetConsentInfo(FieldInfo?.Settings["Consent"] as string);
        }
    }


    public override object Value
    {
        get
        {
            return string.IsNullOrEmpty(hdnValueHolder.Value) ? null : hdnValueHolder.Value;
        }
        set
        {
            hdnValueHolder.Value = value?.ToString();
        }
    }


    public override bool Enabled
    {
        get
        {
            return chkConsent.Enabled;
        }
        set
        {
            base.Enabled = value;
            chkConsent.Enabled = value;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Form != null)
        {
            Form.OnBeforeSave += Form_OnBeforeSave;
        }

        if (FieldInfo != null)
        {
            litConsentText.Text = GetConsentReferenceMarkup();
        }
    }


    private void Form_OnBeforeSave(object sender, EventArgs e)
    {
        var info = Form.Data as BaseInfo;
        if (info == null)
        {
            return;
        }

        if (SelectedConsent != null && IsLiveSite)
        {
            var service = Service.Resolve<IFormConsentAgreementService>();
            var contact = ContactManagementContext.GetCurrentContact();
            ConsentAgreementInfo agreement;

            if (chkConsent.Checked)
            {
                agreement = service.Agree(contact, SelectedConsent, info);
            }
            else
            {
                agreement = service.Revoke(contact, SelectedConsent, info);
            }

            StoreAgreementGuidInData(info, agreement);
        }
    }


    private void StoreAgreementGuidInData(BaseInfo info, ConsentAgreementInfo agreement)
    {
        if (FieldInfo == null)
        {
            return;
        }

        var agreementGuid = agreement.ConsentAgreementGuid;
        info.SetValue(FieldInfo.Name, agreementGuid);
        Value = agreementGuid;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Enabled = SelectedConsent != null && IsLiveSite;
        chkConsent.Text = GetConsentText();

        if (Value == null)
        {
            return;
        }

        chkConsent.Checked = IsConsentAgreed();
    }


    private bool IsConsentAgreed()
    {
        var agreementGuid = ValidationHelper.GetGuid(Value, Guid.Empty);
        var isRevoked = ConsentAgreementInfoProvider.GetConsentAgreements()
                                                    .WithGuid(agreementGuid)
                                                    .Column("ConsentAgreementRevoked")
                                                    .GetScalarResult(true);
        return !isRevoked;
    }


    private string GetConsentText()
    {
        if (SelectedConsent == null)
        {
            return ResHelper.GetString("dataprotection.consentagreement.consentnotavailable");
        }

        var currentCulture = LocalizationContext.CurrentCulture.CultureCode;

        return SelectedConsent.GetConsentText(currentCulture).ShortText;
    }


    private string GetConsentReferenceMarkup()
    {
        var consentReferenceMarkupFieldName = "ConsentReferenceMarkup";

        if (FieldInfo.SettingIsMacro(consentReferenceMarkupFieldName))
        {
            return ContextResolver.ResolveMacros(FieldInfo.SettingsMacroTable[consentReferenceMarkupFieldName] as string);
        }

        return FieldInfo.Settings[consentReferenceMarkupFieldName] as string;
    }
}