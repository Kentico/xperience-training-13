using System;

using CMS.ContactManagement;
using CMS.Core;
using CMS.DataProtection;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_General_CookieLaw : CMSAbstractWebPart
{
    private readonly ICurrentCookieLevelProvider cookieLevelProvider = Service.Resolve<ICurrentCookieLevelProvider>();


    #region "Properties"

    /// <summary>
    /// Compare current cookie level to
    /// </summary>
    public string MatchLevel
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MatchLevel"), "Essential");
        }
        set
        {
            SetValue("MatchLevel", value);
        }
    }


    /// <summary>
    /// If level is below, display
    /// </summary>
    public bool BelowLevelVisible
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BelowLevelVisible"), true);
        }
        set
        {
            SetValue("BelowLevelVisible", value);
        }
    }


    /// <summary>
    /// Text
    /// </summary>
    public string BelowLevelText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BelowLevelText"), "");
        }
        set
        {
            SetValue("BelowLevelText", value);
        }
    }


    /// <summary>
    /// Show deny button
    /// </summary>
    public bool BelowShowDeny
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BelowShowDeny"), false);
        }
        set
        {
            SetValue("BelowShowDeny", value);
        }
    }


    /// <summary>
    /// Show allow specific button
    /// </summary>
    public bool BelowShowSpecific
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BelowShowSpecific"), true);
        }
        set
        {
            SetValue("BelowShowSpecific", value);
        }
    }


    /// <summary>
    /// Show allow all button
    /// </summary>
    public bool BelowShowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("BelowShowAll"), true);
        }
        set
        {
            SetValue("BelowShowAll", value);
        }
    }


    /// <summary>
    /// If level matches, display
    /// </summary>
    public bool ExactLevelVisible
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExactLevelVisible"), true);
        }
        set
        {
            SetValue("ExactLevelVisible", value);
        }
    }


    /// <summary>
    /// Text
    /// </summary>
    public string ExactLevelText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExactLevelText"), "");
        }
        set
        {
            SetValue("ExactLevelText", value);
        }
    }


    /// <summary>
    /// Show deny button
    /// </summary>
    public bool ExactShowDeny
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExactShowDeny"), true);
        }
        set
        {
            SetValue("ExactShowDeny", value);
        }
    }


    /// <summary>
    /// Show allow specific button
    /// </summary>
    public bool ExactShowSpecific
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExactShowSpecific"), false);
        }
        set
        {
            SetValue("ExactShowSpecific", value);
        }
    }


    /// <summary>
    /// Show allow all button
    /// </summary>
    public bool ExactShowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExactShowAll"), true);
        }
        set
        {
            SetValue("ExactShowAll", value);
        }
    }


    /// <summary>
    /// If level is above, display
    /// </summary>
    public bool AboveLevelVisible
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AboveLevelVisible"), true);
        }
        set
        {
            SetValue("AboveLevelVisible", value);
        }
    }


    /// <summary>
    /// Text
    /// </summary>
    public string AboveLevelText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AboveLevelText"), "");
        }
        set
        {
            SetValue("AboveLevelText", value);
        }
    }


    /// <summary>
    /// Show deny button
    /// </summary>
    public bool AboveShowDeny
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AboveShowDeny"), true);
        }
        set
        {
            SetValue("AboveShowDeny", value);
        }
    }


    /// <summary>
    /// Show allow specific button
    /// </summary>
    public bool AboveShowSpecific
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AboveShowSpecific"), true);
        }
        set
        {
            SetValue("AboveShowSpecific", value);
        }
    }


    /// <summary>
    /// Show allow all button
    /// </summary>
    public bool AboveShowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AboveShowAll"), false);
        }
        set
        {
            SetValue("AboveShowAll", value);
        }
    }


    /// <summary>
    /// Deny all button text
    /// </summary>
    public string DenyAllText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DenyAllText"), "Deny all cookies");
        }
        set
        {
            SetValue("DenyAllText", value);
        }
    }


    /// <summary>
    /// Allow specific button text
    /// </summary>
    public string AllowSpecificText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AllowSpecificText"), "Allow only essential cookies");
        }
        set
        {
            SetValue("AllowSpecificText", value);
        }
    }


    /// <summary>
    /// Allow specific button sets level
    /// </summary>
    public string AllowSpecificSetLevel
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AllowSpecificSetLevel"), "Essential");
        }
        set
        {
            SetValue("AllowSpecificSetLevel", value);
        }
    }


    /// <summary>
    /// Allow all button text
    /// </summary>
    public string AllowAllText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AllowAllText"), "Allow all cookies");
        }
        set
        {
            SetValue("AllowAllText", value);
        }
    }


    /// <summary>
    /// Tracking consent code name
    /// </summary>
    public string TrackingConsent
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackingConsent"), null);
        }
        set
        {
            SetValue("TrackingConsent", value);
        }
    }


    /// <summary>
    /// HTML placed after tracking consent
    /// </summary>
    public string ConsentReferenceMarkup
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ConsentReferenceMarkup"), "");
        }
        set
        {
            SetValue("ConsentReferenceMarkup", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            btnAllowAll.Text = AllowAllText;
            btnDenyAll.Text = DenyAllText;
            btnAllowSpecific.Text = AllowSpecificText;
            ltlConsentReferenceMarkup.Text = ConsentReferenceMarkup;
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            if (!string.IsNullOrEmpty(TrackingConsent))
            {
                SetCurrentCookieLevelWhenConsentIsNotAgreed();
            }

            int currentCookieLevel = GetCurrentCookieLevel();

            if (!string.IsNullOrEmpty(TrackingConsent) && currentCookieLevel < CookieLevel.Visitor)
            {
                DisplayTrackingConsent();
            }
            else
            {
                pnlTrackingConsent.Visible = false;
            }

            int matchLevel = CookieHelper.ConvertCookieLevelToIntegerValue(MatchLevel, currentCookieLevel);

            if (currentCookieLevel < matchLevel)
            {
                // Set the components to the Above level
                SetComponents(BelowLevelVisible, BelowLevelText, BelowShowAll, BelowShowSpecific, BelowShowDeny);
            }
            else if (currentCookieLevel == matchLevel)
            {
                // Set the components to the Above level
                SetComponents(ExactLevelVisible, ExactLevelText, ExactShowAll, ExactShowSpecific, ExactShowDeny);
            }
            else
            {
                // Set the components to the Above level
                SetComponents(AboveLevelVisible, AboveLevelText, AboveShowAll, AboveShowSpecific, AboveShowDeny);
            }
        }
    }


    private void SetCurrentCookieLevelWhenConsentIsNotAgreed()
    {
        if (!PortalContext.ViewMode.IsLiveSite())
        {
            return;
        }

        var consent = ConsentInfoProvider.GetConsentInfo(TrackingConsent);
        var contact = ContactManagementContext.CurrentContact;

        if (consent != null && contact != null)
        {
            if (!Service.Resolve<IConsentAgreementService>().IsAgreed(contact, consent))
            {
                cookieLevelProvider.SetCurrentCookieLevel(cookieLevelProvider.GetDefaultCookieLevel());
            }
        }
    }


    private void DisplayTrackingConsent()
    {
        var consent = ConsentInfoProvider.GetConsentInfo(TrackingConsent);
        if (consent != null)
        {
            var cultureCode = LocalizationContext.CurrentCulture.CultureCode;
            ltlTrackingConsentShortText.Text = consent.GetConsentText(cultureCode).ShortText;
            pnlTrackingConsent.Visible = true;
        }
    }


    /// <summary>
    /// Initializes the components based on the given properties
    /// </summary>
    /// <param name="visible">Flag whether this mode should be visible</param>
    /// <param name="text">Information text</param>
    /// <param name="allowAll">Show allow all button</param>
    /// <param name="allowSpecific">Show allow Specific button</param>
    /// <param name="allowDeny">Show allow deny button</param>
    private void SetComponents(bool visible, string text, bool allowAll, bool allowSpecific, bool allowDeny)
    {
        if (!visible)
        {
            Visible = false;
            return;
        }

        lblText.Text = text;

        btnAllowAll.Visible = !String.IsNullOrEmpty(btnAllowAll.Text) && allowAll;
        btnAllowSpecific.Visible = !String.IsNullOrEmpty(btnAllowSpecific.Text) && allowSpecific;
        btnDenyAll.Visible = !String.IsNullOrEmpty(btnDenyAll.Text) && allowDeny;

        // Hide the web part in case no content or button is displayed
        if (String.IsNullOrEmpty(text) && !btnAllowAll.Visible && !btnAllowSpecific.Visible && !btnDenyAll.Visible)
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Deny all click
    /// </summary>
    protected void btnDenyAll_Click(object sender, EventArgs e)
    {
        ChangeLevel(CookieLevel.System);
    }


    /// <summary>
    /// Allow Specific click
    /// </summary>
    protected void btnAllowSpecific_Click(object sender, EventArgs e)
    {
        int specificLevel = CookieHelper.ConvertCookieLevelToIntegerValue(AllowSpecificSetLevel, CookieLevel.Essential);
        ChangeLevel(specificLevel);
    }


    /// <summary>
    /// Allow all click
    /// </summary>
    protected void btnAllowAll_Click(object sender, EventArgs e)
    {
        ChangeLevel(CookieLevel.All);
    }


    /// <summary>
    /// Changes the cookie level
    /// </summary>
    /// <param name="newLevel">New cookie level to set</param>
    private void ChangeLevel(int newLevel)
    {
        if (PortalContext.ViewMode.IsLiveSite())
        {
            // Keep the current contact as this value will be lost once the cookie level gets lower than the "Visitor" level
            var originalContact = ContactManagementContext.CurrentContact;
            var originalCookieLevel = cookieLevelProvider.GetCurrentCookieLevel();

            cookieLevelProvider.SetCurrentCookieLevel(newLevel);

            if (!String.IsNullOrEmpty(TrackingConsent))
            {
                if ((newLevel >= CookieLevel.Visitor) && (originalCookieLevel < CookieLevel.Visitor))
                {
                    AgreeConsent();
                }
                else if ((originalCookieLevel >= CookieLevel.Visitor) && (newLevel < CookieLevel.Visitor))
                {
                    RevokeConsent(originalContact);
                }
            }
        }
    }


    private void AgreeConsent()
    {
        var consent = ConsentInfoProvider.GetConsentInfo(TrackingConsent);
        if ((consent != null) && (ContactManagementContext.CurrentContact != null))
        {
            Service.Resolve<IConsentAgreementService>().Agree(ContactManagementContext.CurrentContact, consent);
        }
    }


    private void RevokeConsent(ContactInfo contact)
    {
        var consent = ConsentInfoProvider.GetConsentInfo(TrackingConsent);
        if ((consent != null) && contact != null)
        {
            Service.Resolve<IConsentAgreementService>().Revoke(contact, consent);
        }
    }


    private int GetCurrentCookieLevel()
    {
        return cookieLevelProvider.GetCurrentCookieLevel();
    }

    #endregion
}
