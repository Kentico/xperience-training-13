using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.SocialMarketing;


public partial class CMSModules_SocialMarketing_FormControls_LinkedInCompanyAccessToken : FormEngineUserControl
{
    #region "Private variables and properties"

    private LinkedInAccountInfo mLinkedInAccount;

    
    /// <summary>
    /// Gets edited LinkedIn account info.
    /// </summary>
    private LinkedInAccountInfo LinkedInAccount
    {
        get
        {
            return mLinkedInAccount ?? (mLinkedInAccount = Form.Data as LinkedInAccountInfo);
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets enabled state.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return btnGetToken.Enabled;
        }
        set
        {
            btnGetToken.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value of company ID.
    /// </summary>
    public override object Value
    {
        get
        {
            return hdnCompanyId.Value;
        }
        set
        {
            hdnCompanyId.Value = (string)value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load event.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Form == null)
        {
            StopProcessing = true;

            return;
        }

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);

        InitControl();
        
        Form.OnAfterSave += Form_OnAfterSave;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!String.IsNullOrEmpty(hdnCompanies.Value))
        {
            btnGetToken.CssClass = "hide";
            drpCompany.CssClass = "form-control";
        }
        else
        {
            btnGetToken.CssClass = String.Empty;
            drpCompany.CssClass = "form-control hide";
        }
    }

    
    /// <summary>
    /// After form is saved.
    /// </summary>
    void Form_OnAfterSave(object sender, EventArgs e)
    {
        pnlCompanySelector.CssClass = "hide";
        pnlCompanyInfo.CssClass = String.Empty;
    }


    /// <summary>
    /// Button get token clicked event.
    /// </summary>
    protected void btnGetToken_OnClick(object sender, EventArgs e)
    {
        if (Form != null)
        {
            // Control data, show error if something is missing
            int appId = ValidationHelper.GetInteger(Form.GetFieldValue("LinkedInAccountLinkedInApplicationID"), 0);
            LinkedInApplicationInfo appInfo = LinkedInApplicationInfoProvider.GetLinkedInApplicationInfo(appId);
            if (appInfo == null)
            {
                Form.AddError(GetString("sm.linkedin.account.msg.appnotset"));

                return;
            }

            // Store data in session
            string sessionKey = Guid.NewGuid().ToString();
            Hashtable parameters = new Hashtable
            {
                {"ApiKey", appInfo.LinkedInApplicationConsumerKey},
                {"ApiSecret", appInfo.LinkedInApplicationConsumerSecret},
                {"AppInfoId", appInfo.LinkedInApplicationID},
                {"ClientID", ClientID}
            };
            WindowHelper.Add(sessionKey, parameters);

            // Open client dialog script
            string openDialogScript = String.Format("modalDialog('{0}?dataKey={1}', 'LinkedInCompanyAccessToken', 600, 600, null, null, null, true);",
                URLHelper.GetAbsoluteUrl("~/CMSModules/SocialMarketing/Pages/LinkedInCompanyAccessTokenDialog.aspx"),
                sessionKey);
            ScriptHelper.RegisterStartupScript(this, GetType(), "LinkedInAccessTokenOpenModal" + ClientID, openDialogScript, true);
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        if (String.IsNullOrWhiteSpace(hdnCompanyId.Value))
        {
            ValidationError = ResHelper.GetString("sm.linkedin.account.msg.selectcompany");

            return false;
        }

        return ValidateToken();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if (ContainsColumn("LinkedInAccountAccessToken"))
        {
            hdnToken.Value = ValidationHelper.GetString(GetColumnValue("LinkedInAccountAccessToken"), "");
        }
        if (ContainsColumn("LinkedInAccountAccessTokenExpiration"))
        {
            var expiration = ValidationHelper.GetDateTime(GetColumnValue("LinkedInAccountAccessTokenExpiration"), DateTime.MinValue);
            if (expiration != DateTime.MinValue)
            {
                hdnTokenExpiration.Value = expiration.ToString("u");
            }
        }
        if (ContainsColumn("LinkedInAccountProfileID"))
        {
            hdnCompanyId.Value = ValidationHelper.GetString(GetColumnValue("LinkedInAccountProfileID"), "");
        }
        if (ContainsColumn("LinkedInAccountProfileName"))
        {
            hdnCompanyName.Value = ValidationHelper.GetString(GetColumnValue("LinkedInAccountProfileName"), "");
        }
    }
    

    /// <summary>
    /// Returns an array of values of any other fields returned by the control.
    /// </summary>
    /// <remarks>It returns an array where first dimension is attribute name and the second dimension is its value.</remarks>
    public override object[,] GetOtherValues()
    {
        DateTime? expiration = null;
        DateTime tokenExpiration;
        if (DateTime.TryParse(hdnTokenExpiration.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out tokenExpiration))
        {
            expiration = tokenExpiration.ToUniversalTime();
        }

        return new object[,]
        {
            { "LinkedInAccountAccessToken", hdnToken.Value },
            { "LinkedInAccountAccessTokenExpiration", expiration },
            { "LinkedInAccountProfileID", hdnCompanyId.Value },
            { "LinkedInAccountProfileName", hdnCompanyName.Value },
        };
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the control.
    /// </summary>
    private void InitControl()
    {
        CheckFieldEmptiness = false;

        if (!RequestHelper.IsPostBack())
        {
            if ((LinkedInAccount != null) && (LinkedInAccount.LinkedInAccountID > 0))
            {
                hdnTokenAppId.Value = LinkedInAccount.LinkedInAccountLinkedInApplicationID.ToString();
                hdnToken.Value = LinkedInAccount.LinkedInAccountAccessToken;
                hdnCompanyId.Value = LinkedInAccount.LinkedInAccountProfileID;
                hdnCompanyName.Value = LinkedInAccount.LinkedInAccountProfileName;
                lblCompanyName.Text = HTMLHelper.HTMLEncode(LinkedInAccount.LinkedInAccountProfileName);
                
                var expiration = LinkedInAccount.LinkedInAccountAccessTokenExpiration;
                if (expiration.HasValue)
                {
                    hdnTokenExpiration.Value = expiration.Value.ToString("g", CultureInfo.InvariantCulture);
                    string formattedExpiration = TimeZoneHelper.ConvertToUserTimeZone(expiration.Value, true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
                    hdnTokenExpirationString.Value = formattedExpiration;
                    lblExpiration.Text = String.Format(GetString("sm.linkedin.account.msg.tokenwillexpire"), formattedExpiration);
                }
                
                pnlCompanySelector.CssClass = "hide";
            }
            else
            {
                pnlCompanyInfo.CssClass = "hide";
            }
        }

        drpCompany.Items.Add(new ListItem(GetString("sm.linkedin.account.msg.selectcompany"), String.Empty));

        ScriptHelper.RegisterModule(Page, "CMS.SocialMarketing/LinkedInCompanyAccessToken", new
        {
            clientId = ClientID,
            tokenControlId = hdnToken.ClientID,
            tokenExpirationControlId = hdnTokenExpiration.ClientID,
            tokenExpirationStringControlId = hdnTokenExpirationString.ClientID,
            tokenExpirationInfoControlId = lblExpiration.ClientID,
            appIdControlId = hdnTokenAppId.ClientID,
            companyIdControlId = hdnCompanyId.ClientID,
            companyNameControlId = hdnCompanyName.ClientID,
            companyDropdownControlId = drpCompany.ClientID,
            companiesControlId = hdnCompanies.ClientID,
            // Pass identifier of the message label that is currently visible
            infoLabelControlId = pnlCompanyInfo.CssClass.Contains("hide") ? lblMessageAuthorize.ClientID : lblMessageReauthorize.ClientID,
            getTokenButtonId = btnGetToken.ClientID,
            msgExpiration = GetString("sm.linkedin.account.msg.tokenwillexpire"),
            msgNoCompany = GetStatusMessage(GetString("sm.linkedin.account.msg.nocompany")),
            msgNoCompanyAccess = GetStatusMessage(GetString("sm.linkedin.account.msg.nocompanyaccess"))
        });
    }


    /// <summary>
    /// Validates access token and shows error if access token is not set or it is expired. 
    /// Shows information about access token expiration if token is valid.
    /// </summary>
    /// <returns>True if access token is valid and false otherwise.</returns>
    private bool ValidateToken()
    {
        // Check if page id or app id has been changed
        int appId = ValidationHelper.GetInteger(Form.GetFieldValue("LinkedInAccountLinkedInApplicationID"), 0);
        if (appId.ToString() != hdnTokenAppId.Value)
        {
            // Token was retrieved for another application - show company selector.
            hdnCompanies.Value = String.Empty;
            hdnCompanyId.Value = String.Empty;
            pnlCompanyInfo.CssClass = "hide";
            pnlCompanySelector.CssClass = String.Empty;

            return false;
        }

        if (!String.IsNullOrEmpty(hdnToken.Value))
        {
            return true;
        }
        
        return false;
    }


    /// <summary>
    /// Gets markup for displaying success or error message in a form.
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="success">True if test succeeded, false otherwise</param>
    /// <returns>Status message markup.</returns>
    private string GetStatusMessage(string message, bool success = false)
    {
        StringBuilder status = new StringBuilder();
        if (success)
        {
            status.Append(UIHelper.GetAccessibleIconTag("icon-check-circle", message, FontIconSizeEnum.NotDefined, "linkedin-status-success-icon")).
                Append("<span class=\"linkedin-status-success-text\">");
        }
        else
        {
            status.Append(UIHelper.GetAccessibleIconTag("icon-times-circle", message, FontIconSizeEnum.NotDefined, "linkedin-status-error-icon")).
                Append("<span class=\"linkedin-status-error-text\">");
        }
        status.Append(HTMLHelper.HTMLEncode(message)).Append("</span>");

        return status.ToString();
    }

    #endregion
}