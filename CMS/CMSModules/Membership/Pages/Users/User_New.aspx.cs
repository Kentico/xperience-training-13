using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_User_New : CMSUsersPage
{
    #region "Variables"

    private String userName = String.Empty;
    private bool error;

    #endregion


    private bool AllowAssignToWebsite
    {
        get
        {
            return (SiteID <= 0) && (SiteContext.CurrentSiteID > 0) && CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
        }
    }


    #region "Public methods"

    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        base.ShowError(text, description, tooltipText, persistent);
        error = true;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check "modify" permission
        if (!CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        if (!RequestHelper.IsPostBack())
        {
            chkEnabled.Checked = true;

            if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                // Remove global and site admin options for non global admins
                drpPrivilegeLevel.ExcludedValues = (int)UserPrivilegeLevelEnum.GlobalAdmin + ";" + (int)UserPrivilegeLevelEnum.Admin;
                plcMacroIdentity.Visible = false;
            }

            drpPrivilegeLevel.Value = (int)UserPrivilegeLevelEnum.Editor;
        }

        if (AllowAssignToWebsite)
        {
            chkAssignToSite.Text = HTMLHelper.HTMLEncode(SiteContext.CurrentSite.DisplayName);
            plcAssignToSite.Visible = true;
        }

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("general.users"),
            RedirectUrl = UIContextHelper.GetElementUrl("CMS.Users", QueryHelper.GetString("ParentElem", ""), false),
            Target = "_parent"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Administration-User_New.CurrentUser")
        });
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        // Email format validation
        if (!txtEmailAddress.IsValid())
        {
            ShowError(GetString("Administration-User_New.WrongEmailFormat"));
            return;
        }

        // Find whether user name is valid
        string result = null;
        if (!ucUserName.IsValid())
        {
            result = ucUserName.ValidationError;
        }

        userName = ValidationHelper.GetString(ucUserName.Value, String.Empty).Trim();

        // Check if user with the same user name exists
        if (UserInfo.Provider.Get(userName) != null)
        {
            ShowError(GetString("Administration-User_New.UserExists"));
            return;
        }

        SiteInfo siteInfo = SiteContext.CurrentSite;
        
        if (String.IsNullOrEmpty(result))
        {
            if (txtConfirmPassword.Text == passStrength.Text)
            {
                // Check whether password is valid according to policy
                if (passStrength.IsValid())
                {
                    int userId = SaveNewUser();
                    if (userId != -1)
                    {
                        var uiElementUrl = UIContextHelper.GetElementUrl("CMS.Users", QueryHelper.GetString("editelem", ""), false);
                        var url = URLHelper.AppendQuery(uiElementUrl, "siteid=" + SiteID + "&objectid=" + userId);
                        URLHelper.Redirect(url);
                    }
                }
                else
                {
                    ShowError(AuthenticationHelper.GetPolicyViolationMessage(siteInfo.SiteName));
                }
            }
            else
            {
                ShowError(GetString("Administration-User_Edit_Password.PasswordsDoNotMatch"));
            }
        }
        else
        {
            ShowError(result);
        }
    }


    /// <summary>
    /// Saves new user's data into DB.
    /// </summary>
    /// <returns>Returns ID of created user</returns>
    protected int SaveNewUser()
    {
        UserInfo ui = new UserInfo();

        // Load default values
        FormHelper.LoadDefaultValues("cms.user", ui);

        string emailAddress = txtEmailAddress.Text.Trim();
        ui.PreferredCultureCode = "";
        ui.Email = emailAddress;
        ui.FirstName = "";
        ui.FullName = txtFullName.Text;
        ui.LastName = "";
        ui.MiddleName = "";
        ui.UserName = userName;
        ui.Enabled = chkEnabled.Checked;
        ui.IsExternal = false;

        // Set privilege level, global admin may set all levels, rest only editor
        UserPrivilegeLevelEnum privilegeLevel = (UserPrivilegeLevelEnum)drpPrivilegeLevel.Value.ToInteger(0);
        var isCurrentUserGlobalAdmin = CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
        if (isCurrentUserGlobalAdmin
            || (privilegeLevel == UserPrivilegeLevelEnum.None) || (privilegeLevel == UserPrivilegeLevelEnum.Editor))
        {
            ui.SiteIndependentPrivilegeLevel = privilegeLevel;
        }

        bool assignUserToSite = chkAssignToSite.Checked && AllowAssignToWebsite;

        // Check license limitations
        if (SiteID > 0 || assignUserToSite)
        {
            string errorMessage = String.Empty;
            UserInfoProvider.CheckLicenseLimitation(ui, ref errorMessage);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
            }
        }

        // Check whether email is unique if it is required
        string siteName = SiteName;       
        if (assignUserToSite)
        {
            siteName = SiteContext.CurrentSiteName;
        }

        if (!UserInfoProvider.IsEmailUnique(emailAddress, siteName, 0))
        {
            ShowError(GetString("UserInfo.EmailAlreadyExist"));
            return -1;
        }

        if (!error)
        {
            using (var transaction = new CMSLateBoundTransaction())
            {
                // Set password and save object
                UserInfoProvider.SetPassword(ui, passStrength.Text);

                if (isCurrentUserGlobalAdmin)
                {
                    UserMacroIdentityHelper.SetMacroIdentity(ui, drpMacroIdentity.Value.ToInteger(0));
                }

                // Add user to current site
                if ((SiteID > 0) || assignUserToSite)
                {
                    UserInfoProvider.AddUserToSite(ui.UserName, siteName);
                }

                transaction.Commit();
            }
            return ui.UserID;
        }

        return -1;
    }
}