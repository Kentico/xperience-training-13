using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;


public partial class CMSModules_Membership_FormControls_Passwords_ChangePassword : CMSUserControl
{
    #region "Variables"

    protected bool mAllowEmptyPassword = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether to allow to save empty password.
    /// </summary>
    public bool AllowEmptyPassword
    {
        get
        {
            return mAllowEmptyPassword;
        }
        set
        {
            mAllowEmptyPassword = value;
        }
    }


    /// <summary>
    /// Gets confirmation button
    /// </summary>
    public CMSButton ConfirmationButton
    {
        get
        {
            return btnOk;
        }
    }


    /// <summary>
    /// Indicates if new password must be different than previous password
    /// </summary>
    public bool ForceDifferentPassword
    {
        get;
        set;
    }


    /// <summary>
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["StopProcessing"], false);
        }
        set
        {
            ViewState["StopProcessing"] = value;
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


    #region "Events"

    /// <summary>
    /// Event which occurs after successful password change
    /// </summary>
    public event OnPasswordChangeHandler OnPasswordChange;

    /// <summary>
    /// Successful password change event handler
    /// </summary>
    public delegate void OnPasswordChangeHandler(object sender, EventArgs e);

    #endregion


    #region "Control events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Show only to authenticated users, if intended to be displayed
            if (Visible)
            {
                Visible = AuthenticationHelper.IsAuthenticated();
            }

            lblExistingPassword.Text = GetString("MyAccount.Password.ExistingPassword");
            lblPassword1.Text = GetString("MyAccount.Password.NewPassword");
            lblPassword2.Text = GetString("MyAccount.Password.ConfirmPassword");
            btnOk.Text = GetString("MyAccount.Password.SetPassword");
        }
    }


    /// <summary>
    /// On btnOK click, save changed password.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        UserInfo ui = MembershipContext.AuthenticatedUser;

        if (ui != null)
        {
            // Check match of old password
            if (!UserInfoProvider.IsUserPasswordDifferent(ui, txtExistingPassword.Text))
            {
                if ((!mAllowEmptyPassword) && (DataHelper.IsEmpty(passStrength.Text.Trim())))
                {
                    ShowError(GetString("myaccount.password.emptypassword"));
                }
                else
                {
                    if (passStrength.Text == txtPassword2.Text)
                    {
                        // Check policy
                        if (!passStrength.IsValid())
                        {
                            ShowError(AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName));
                        }
                        else
                        {
                            // Check if different password is required
                            if (ForceDifferentPassword)
                            {
                                if (!UserInfoProvider.IsUserPasswordDifferent(ui, passStrength.Text))
                                {
                                    ShowError(GetString("passreset.newpasswordrequired"));
                                    return;
                                }
                            }

                            UserInfoProvider.SetPassword(ui.UserName, passStrength.Text);
                            ShowChangesSaved();

                            AuthenticationHelper.SendPasswordResetConfirmation(ui, SiteContext.CurrentSiteName, "Change password control", "Membership.PasswordResetConfirmation");
                            
                            // Call Password change event
                            if (OnPasswordChange != null)
                            {
                                OnPasswordChange(this, null);
                            }
                        }
                    }
                    else
                    {
                        // New and confirmed password are not equal
                        ShowError(GetString("Administration-User_Edit_Password.PasswordsDoNotMatch"));
                    }
                }
            }
            else
            {
                // Incorrect existing password
                ShowError(GetString("myaccount.password.incorrectexistingpassword"));
            }
        }
    }

    #endregion
}