using System;
using System.Linq;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_UnlockUserAccount : CMSUserControl
{
    #region "Private variables"

    private string mInvalidAttemptsHash = null;
    private UserInfo mUserAccount = null;
    private string returnUrl = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets successful unlock text.
    /// </summary>
    public string SuccessfulUnlockText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets unsuccessful unlock text.
    /// </summary>
    public string UnsuccessfulUnlockText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the unlock info label text.
    /// </summary>
    public string UnlockInfoText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the unlock button text.
    /// </summary>
    public string UnlockButtonText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the unlock label CSS class.
    /// </summary>
    public string UnlockTextCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the unlock button CSS class.
    /// </summary>
    public string UnlockButtonCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the URL to which user will be redirected after successful account unlock.
    /// </summary>
    public string RedirectionURL
    {
        get;
        set;
    }


    /// <summary>
    /// Get user account object
    /// </summary>
    public UserInfo UserAccount
    {
        get
        {
            if (mUserAccount == null)
            {
                mUserAccount = UserInfoProvider.GetUsersDataWithSettings()
                    .WhereEquals("UserInvalidLogOnAttemptsHash", mInvalidAttemptsHash)
                    .TopN(1)
                    .FirstOrDefault();
            }

            return mUserAccount;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Get data from query string
        mInvalidAttemptsHash = QueryHelper.GetString("unlockaccounthash", string.Empty);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string defaultReturnUrl = UrlResolver.ResolveUrl(AuthenticationHelper.DEFAULT_LOGON_PAGE);
        returnUrl = QueryHelper.GetString("returnurl", null);

        // Security check to allow only relative returnUrl from query
        if (!URLHelper.IsLocalUrl(returnUrl))
        {
            returnUrl = defaultReturnUrl;
        }

        // If StopProcessing flag is set, do nothing
        if (StopProcessing)
        {
            Visible = false;
            return;
        }
        else
        {

            bool controlPb = false;

            if (RequestHelper.IsPostBack())
            {
                Control pbCtrl = ControlsHelper.GetPostBackControl(Page);
                if (pbCtrl == btnConfirm)
                {
                    controlPb = true;
                }
            }

            // Setup controls
            SetupControls(!controlPb);

            if (!controlPb)
            {
                if (CheckAndUnlock(true))
                {
                    ShowInformation(DataHelper.GetNotEmpty(UnlockInfoText, GetString("invalidlogonattempts.unlockaccount.unlocktext")));
                }
            }
        }
    }


    /// <summary>
    /// Button unlock click event
    /// </summary>
    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        CheckAndUnlock(false);
    }


    /// <summary>
    /// Initialize controls properties
    /// </summary>
    private void SetupControls(bool forceReload)
    {
        btnConfirm.CssClass = UnlockButtonCssClass;

        if (forceReload)
        {
            btnConfirm.Text = DataHelper.GetNotEmpty(UnlockButtonText, GetString("general.unlock"));
        }
    }


    /// <summary>
    /// Check that unlock hash is valid
    /// </summary>
    /// <param name="checkOnly">Indicates if only check will be performed</param>
    private bool CheckAndUnlock(bool checkOnly)
    {
        if ((UserAccount != null) && !UserAccount.Enabled && (UserAccount.UserInvalidLogOnAttempts > 0))
        {
            if (!checkOnly)
            {
                AuthenticationHelper.UnlockUserAccount(UserAccount);

                // Show information message or redirect to specified URL
                if (string.IsNullOrEmpty(RedirectionURL))
                {
                    btnConfirm.Visible = false;

                    if(!string.IsNullOrEmpty(SuccessfulUnlockText))
                    {
                        ShowConfirmation(SuccessfulUnlockText);
                    }
                    else
                    {
                        ShowConfirmation(GetString("invalidlogonattempts.unlockaccount.accountunlocked"));
                        lblInfo.Text = String.Format(GetString("invalidlogonattempts.unlockaccount.login"), HTMLHelper.EncodeForHtmlAttribute(returnUrl));
                    }
                }
                else
                {
                    URLHelper.Redirect(UrlResolver.ResolveUrl(RedirectionURL));
                }
            }
        }
        else
        {
            DisplayError(DataHelper.GetNotEmpty(UnsuccessfulUnlockText, GetString("invalidlogonattempts.unlockaccount.errortext")));
            return false;
        }

        return true;
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Display error message
    /// </summary>
    /// <param name="errorText">Error text to display</param>
    private void DisplayError(string errorText)
    {
        ShowError(errorText);
        btnConfirm.Visible = false;
    }

    #endregion
}