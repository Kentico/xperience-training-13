using System;
using System.Collections;
using System.Globalization;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.SocialMarketing;


public partial class CMSModules_SocialMarketing_FormControls_FacebookPageAccessToken : FormEngineUserControl
{
    #region "Private variables and properties"

    private FacebookAccountInfo mFacebookAccount;

    
    /// <summary>
    /// Gets edited facebook account info.
    /// </summary>
    private FacebookAccountInfo FacebookAccount
    {
        get
        {
            return mFacebookAccount ?? (mFacebookAccount = Form.Data as FacebookAccountInfo);
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
    /// Gets or sets the value of access token.
    /// </summary>
    public override object Value
    {
        get
        {
            return hdnToken.Value;
        }
        set
        {
            hdnToken.Value = (string) value;
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

        if (Form != null)
        {
            CheckFieldEmptiness = false;

            if (!RequestHelper.IsPostBack() && (FacebookAccount != null))
            {
                var expiration = FacebookAccount.FacebookPageAccessToken.Expiration;
                hdnTokenExpiration.Value = expiration.HasValue ? expiration.Value.ToString("g", CultureInfo.InvariantCulture) : String.Empty;
                hdnTokenAppId.Value = FacebookAccount.FacebookAccountFacebookApplicationID.ToString();
                hdnTokenPageId.Value = FacebookAccount.FacebookAccountPageID;
            }
            ValidateToken();
        }

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);
    }


    /// <summary>
    /// Button get token clicked event.
    /// </summary>
    protected void btnGetToken_OnClick(object sender, EventArgs e)
    {
        if (Form != null)
        {
            // Control data, show error if something is missing
            int appId = ValidationHelper.GetInteger(Form.GetFieldValue("FacebookAccountFacebookApplicationID"), 0);
            FacebookApplicationInfo appInfo = FacebookApplicationInfoProvider.GetFacebookApplicationInfo(appId);
            if (appInfo == null)
            {
                ShowError(GetString("sm.facebook.account.msg.appnotset"));
                return;
            }

            string pageId = Form.GetFieldValue("FacebookAccountPageID").ToString();
            if (String.IsNullOrEmpty(pageId))
            {
                ShowError(GetString("sm.facebook.account.msg.pageidnotset"));
                return;
            }

            // Store data in session
            Hashtable parameters = new Hashtable
            {
                    {"AppId", appInfo.FacebookApplicationConsumerKey},
                    {"AppSecret", appInfo.FacebookApplicationConsumerSecret},
                    {"PageId", pageId},
                    {"TokenCntId", hdnToken.ClientID},
                    {"TokenExpirationCntId", hdnTokenExpiration.ClientID},
                    {"InfoLblId", lblMessage.ClientID},
                    {"TokenPageIdCntId", hdnTokenPageId.ClientID},
                    {"TokenAppIdCntId", hdnTokenAppId.ClientID},
                    {"TokenAppInfoId", appInfo.FacebookApplicationID.ToString()}
                };
            WindowHelper.Add(FacebookHelper.PAGE_ACCESS_TOKEN_SESSION_KEY, parameters);

            // Open client dialog script
            string openDialogScript = "fbOpenModalDialog();";
            ScriptHelper.RegisterStartupScript(this, GetType(), "FBAccessTokenOpenModal" + ClientID, openDialogScript, true);
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        if (String.IsNullOrWhiteSpace(hdnToken.Value))
        {
            ValidationError = ResHelper.GetString("sm.facebook.account.msg.accesstokennotset");
            return false;
        }

        return ValidateToken();
    }



    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if (ContainsColumn("FacebookAccountPageAccessTokenExpiration"))
        {
            var expiration = ValidationHelper.GetDateTime(GetColumnValue("FacebookAccountPageAccessTokenExpiration"), DateTime.MinValue);
            if (expiration != DateTime.MinValue)
            {
                hdnTokenExpiration.Value = expiration.ToString("u");
            }
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

        return new object[,] { { "FacebookAccountPageAccessTokenExpiration", expiration } };
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Validates access token and shows error if access token is not set or it is expired. 
    /// Shows information about access token expiration if token is valid.
    /// </summary>
    /// <returns>True if access token is valid and false otherwise.</returns>
    private bool ValidateToken()
    {
        // Check if page id or app id has been changed
        int appId = ValidationHelper.GetInteger(Form.GetFieldValue("FacebookAccountFacebookApplicationID"), 0);
        string pageId = Form.GetFieldValue("FacebookAccountPageId").ToString();
        if ((appId.ToString() != hdnTokenAppId.Value) || (pageId != hdnTokenPageId.Value))
        {
            return false;
        }

        // Get token expiration
        DateTime tokenExpiration;
        DateTime.TryParse(hdnTokenExpiration.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out tokenExpiration);

        // Set token expiration message if token seems to be valid
        if (!String.IsNullOrEmpty(hdnToken.Value))
        {
            if (tokenExpiration == DateTimeHelper.ZERO_TIME)
            {
                ShowInformation(GetString("sm.facebook.account.msg.tokenneverexpire"));
                
                return true;
            }
            
            if (tokenExpiration <= DateTime.Now.AddMinutes(5))
            {
                ShowInformation(GetString("sm.facebook.account.msg.accesstokenexpired"));
            }
            else
            {
                string formattedExpiration = TimeZoneHelper.ConvertToUserTimeZone(tokenExpiration, true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
                ShowInformation(String.Format(GetString("sm.facebook.account.msg.tokenwillexpire"), formattedExpiration));
                return true;
            }
        }
        
        return false;
    }


    /// <summary>
    /// Shows an information message.
    /// </summary>
    /// <param name="message">Message to be shown.</param>
    private void ShowInformation(string message)
    {
        lblMessage.Text = message;
    }

    #endregion
}