using System;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_User_Edit_General : CMSUsersPage
{
    #region "Protected variables"

    protected int userId = 0;
    protected string password;
    protected string myCulture = string.Empty;
    protected string myUICulture = string.Empty;
    private UserInfo ui = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        userId = QueryHelper.GetInteger("objectid", 0);

        // Get user info object and check if UI should be displayed
        ui = UserInfo.Provider.Get(userId);
        CheckUserAvaibleOnSite(ui);

        // Check that only global administrator can edit other administrator's accounts
        if (!CheckGlobalAdminEdit(ui))
        {
            plcTable.Visible = false;
            ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
            return;
        }

        EditedObject = ui;

        ucUserName.UseDefaultValidationGroup = false;
        cultureSelector.DisplayAllCultures = true;
        lblResetToken.Text = GetString("mfauthentication.token.reset");

        // Register picture delete script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PictDelConfirm",
                                               ScriptHelper.GetScript("function DeleteConfirmation(){ return confirm(" + ScriptHelper.GetString(GetString("MyProfile.PictDeleteConfirm")) + ");}"));

        if (!RequestHelper.IsPostBack())
        {
            LoadData();
        }

        // Set hide action if user extend validity of his own account
        if (ui.UserID == CurrentUser.UserID)
        {
            btnExtendValidity.OnClientClick = "window.top.HideWarning()";
        }

        // Register help variable for user is external confirmation
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "IsExternal", ScriptHelper.GetScript("var isExternal = " + chkIsExternal.Checked.ToString().ToLowerCSafe() + ";"));

        // Javascript code for "Is external user" confirmation
        string javascript = ScriptHelper.GetScript(
            @"function CheckExternal() {
                            var checkbox = document.getElementById('" + chkIsExternal.ClientID + @"')
                            if(checkbox.checked && !isExternal) {
                                if(!confirm('" + GetString("user.confirmexternal") + @"')) {
                                    checkbox.checked = false ;
                                }
                            }}");

        // Register script to the page
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ClientID + "CheckExternal", javascript);

        // Assign to ok button
        if (!chkIsExternal.Checked)
        {
            btnOk.OnClientClick = "CheckExternal()";
        }

        // Display impersonation link if current user is global administrator and edited user is not global admin
        if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && (ui.UserID != CurrentUser.UserID) && !ui.IsPublic() && ui.Enabled)
        {
            string message = GetImpersonalMessage(ui);

            HeaderAction action = new HeaderAction();
            action.Text = GetString("Membership.Impersonate");
            action.Tooltip = GetString("Membership.Impersonate");
            action.OnClientClick = "if (!confirm('" + message + "')) { return false; }";
            action.CommandName = "impersonate";
            CurrentMaster.HeaderActions.AddAction(action);

            CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (ui != null)
        {
            // Reset flag
            CheckBoxEnabled.Enabled = true;

            // Show warning message
            if (!ui.Enabled)
            {
                string description = null;
                if (ui.UserSettings.UserWaitingForApproval)
                {
                    description = GetString("Administration-User_List.AccountLocked.WaitingForApproval");
                }
                else
                {
                    switch (UserAccountLockCode.ToEnum(ui.UserAccountLockReason))
                    {
                        case UserAccountLockEnum.MaximumInvalidLogonAttemptsReached:
                            description = GetString("Administration-User_List.AccountLocked.MaximumInvalidPasswordAttempts");
                            CheckBoxEnabled.Enabled = false;
                            break;

                        case UserAccountLockEnum.PasswordExpired:
                            description = GetString("Administration-User_List.AccountLocked.PasswordExpired");
                            CheckBoxEnabled.Enabled = false;
                            break;

                        case UserAccountLockEnum.DisabledManually:
                            description = GetString("Administration-User_List.AccountLocked.Disabledmanually");
                            break;
                    }
                }
                ShowWarning(description);
            }

            // Check "modify" permission
            if (!CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
            {
                btnExtendValidity.Enabled = btnResetLogonAttempts.Enabled = false;
                btnResetToken.Enabled = false;
            }


            // Display impersonation link if current user is global administrator
            if (CurrentMaster.HeaderActions.ActionsList != null)
            {
                var impersonateAction = CurrentMaster.HeaderActions.ActionsList.Find(a => a.CommandName == "impersonate");

                if (impersonateAction != null)
                {
                    if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)
                        && (ui != null)
                        && (ui.UserID != CurrentUser.UserID)
                        && !ui.IsPublic()
                        && (!ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)))
                    {
                        string message = GetImpersonalMessage(ui);
                        impersonateAction.OnClientClick = "if (!confirm('" + message + "')) { return false; }";
                    }
                    else
                    {
                        impersonateAction.Visible = false;
                    }
                }
            }
        }
    }


    /// <summary>
    /// Users actions.
    /// </summary>
    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "impersonate":
                // Use user impersonate
                UserInfo ui = UserInfo.Provider.Get(userId);
                AuthenticationHelper.ImpersonateUser(ui);
                break;

            case ComponentEvents.SAVE:
                btnOk_Click(sender, EventArgs.Empty);
                break;
        }
    }


    /// <summary>
    /// Saves data of edited user from TextBoxes into DB.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        UserPrivilegeLevelEnum privilegeLevel = (UserPrivilegeLevelEnum)drpPrivilege.Value.ToInteger(0);

        // Check "modify" permission
        if (!CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        string result = ValidateGlobalAndDeskAdmin(userId);

        var isExternal = chkIsExternal.Checked;

        // Find whether user name is valid (external users are not checked as their user names can contain various special characters)
        if (result == String.Empty)
        {
            if (!isExternal && !ucUserName.IsValid())
            {
                result = ucUserName.ValidationError;
            }
        }

        String userName = ValidationHelper.GetString(ucUserName.Value, String.Empty);

        // Store the old display name
        var oldDisplayName = ui.Generalized.ObjectDisplayName;

        if ((result == String.Empty) && (ui != null))
        {
            // Ensure same password
            password = ui.GetValue("UserPassword").ToString();

            // Test for unique username
            UserInfo uiTest = UserInfo.Provider.Get(userName);
            if ((uiTest == null) || (uiTest.UserID == userId))
            {
                if (ui == null)
                {
                    ui = new UserInfo();
                }

                bool globAdmin = ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
                bool editor = ui.SiteIndependentPrivilegeLevel == UserPrivilegeLevelEnum.Editor;

                // Email format validation
                if (!txtEmail.IsValid())
                {
                    ShowError(GetString("Administration-User_New.WrongEmailFormat"));
                    return;
                }

                bool oldGlobal = ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
                bool oldEditor = ui.SiteIndependentPrivilegeLevel == UserPrivilegeLevelEnum.Editor;

                // Define domain variable
                string domains = null;

                // Get all user sites
                var userSites = UserInfoProvider.GetUserSites(userId).Column("SiteDomainName");
                if (userSites.Count > 0)
                {
                    foreach (var userSite in userSites)
                    {
                        domains += ValidationHelper.GetString(userSite["SiteDomainName"], string.Empty) + ";";
                    }

                    // Remove  ";" at the end
                    if (domains != null)
                    {
                        domains = domains.Remove(domains.Length - 1);
                    }
                }
                else
                {

                    DataSet siteDs = SiteInfo.Provider.Get().Columns("SiteDomainName");
                    if (!DataHelper.DataSourceIsEmpty(siteDs))
                    {
                        // Create list of available site domains
                        domains = TextHelper.Join(";", DataHelper.GetStringValues(siteDs.Tables[0], "SiteDomainName"));
                    }
                }

                // Check limitations for Global administrator
                if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && ((privilegeLevel == UserPrivilegeLevelEnum.GlobalAdmin) || (privilegeLevel == UserPrivilegeLevelEnum.Admin)) && !oldGlobal)
                {
                    if (!UserInfoProvider.LicenseVersionCheck(domains, FeatureEnum.Administrators, ObjectActionEnum.Insert, globAdmin))
                    {
                        ShowError(GetString("License.MaxItemsReachedGlobal"));
                        return;
                    }
                }

                // Check limitations for editors
                if ((privilegeLevel == UserPrivilegeLevelEnum.Editor) && !oldEditor && userSites.Count > 0)
                {
                    if (!UserInfoProvider.LicenseVersionCheck(domains, FeatureEnum.Editors, ObjectActionEnum.Insert, editor))
                    {
                        ShowError(GetString("License.MaxItemsReachedEditor"));
                        return;
                    }
                }

                // Check whether email is unique if it is required
                string email = txtEmail.Text.Trim();
                if (!UserInfoProvider.IsEmailUnique(email, ui))
                {
                    ShowError(GetString("UserInfo.EmailAlreadyExist"));
                    return;
                }

                // Set properties
                ui.Email = email;
                ui.FirstName = txtFirstName.Text.Trim();
                ui.FullName = txtFullName.Text.Trim();
                ui.LastName = txtLastName.Text.Trim();
                ui.MiddleName = txtMiddleName.Text.Trim();
                ui.UserName = userName;
                UserInfoProvider.SetEnabled(ui, CheckBoxEnabled.Checked);
                ui.UserIsHidden = chkIsHidden.Checked;
                ui.IsExternal = isExternal;
                ui.UserIsDomain = chkIsDomain.Checked;
                ui.SetValue("UserPassword", password);
                ui.UserID = userId;
                ui.UserStartingAliasPath = txtUserStartingPath.Text.Trim();
                ui.UserMFRequired = chkIsMFRequired.Checked;


                var isCurrentUserGlobalAdmin = CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
                // Global admin can set anything
                if (isCurrentUserGlobalAdmin
                    // Other users can set only editor and non privileges
                    || ((privilegeLevel != UserPrivilegeLevelEnum.Admin) && (privilegeLevel != UserPrivilegeLevelEnum.GlobalAdmin))
                    // Admin can manage his own privilege
                    || ((privilegeLevel == UserPrivilegeLevelEnum.Admin) && (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && (CurrentUser.UserID == ui.UserID))))
                {
                    ui.SiteIndependentPrivilegeLevel = privilegeLevel;
                }

                LoadUserLogon(ui);

                // Set values of cultures.
                string culture = ValidationHelper.GetString(cultureSelector.Value, "");
                ui.PreferredCultureCode = culture;

                if (lstUICulture.SelectedValue == "0")
                {
                    ui.PreferredUICultureCode = "";
                }
                else
                {
                    // Set preferred UI culture
                    CultureInfo ci = CultureInfo.Provider.Get(ValidationHelper.GetInteger(lstUICulture.SelectedValue, 0));
                    ui.PreferredUICultureCode = ci.CultureCode;
                }

                // Refresh page breadcrumbs if display name changed
                if (ui.Generalized.ObjectDisplayName != oldDisplayName)
                {
                    ScriptHelper.RefreshTabHeader(Page, ui.FullName);
                }

                using (CMSActionContext context = new CMSActionContext())
                {
                    // Check whether the username of the currently logged user has been changed
                    if (CurrentUserChangedUserName())
                    {
                        // Ensure that an update search task will be created but NOT executed when updating the user
                        context.EnableSmartSearchIndexer = false;
                    }

                    try
                    {
                        using (var transaction = new CMSLateBoundTransaction())
                        {
                            // Update the user
                            UserInfo.Provider.Set(ui);

                            if (isCurrentUserGlobalAdmin)
                            {
                                UserMacroIdentityHelper.SetMacroIdentity(ui, drpMacroIdentity.Value.ToInteger(0));
                            }

                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Service.Resolve<IEventLogService>().LogException("Users", "SAVE", ex);
                        ShowError(GetString("general.errorsaving"));
                        return;
                    }

                    // Check whether the username of the currently logged user has been changed
                    if (CurrentUserChangedUserName())
                    {
                        // Ensure that current user is not logged out if he changes his user name
                        if (AuthenticationMode.IsFormsAuthentication())
                        {
                            FormsAuthentication.SetAuthCookie(ui.UserName, false);

                            // Update current user
                            MembershipContext.AuthenticatedUser = new CurrentUserInfo(ui, true);

                            // Reset current user
                            CurrentUser = null;
                        }
                    }
                }

                ShowChangesSaved();

            }
            else
            {
                // If user exists
                ShowError(GetString("Administration-User_New.UserExists"));
            }
        }
        else
        {
            ShowError(result);
        }

        if ((ui.UserInvalidLogOnAttempts == 0) && (ui.UserAccountLockReason != UserAccountLockCode.FromEnum(UserAccountLockEnum.MaximumInvalidLogonAttemptsReached)))
        {
            btnResetLogonAttempts.Enabled = false;
        }

        LoadPasswordExpiration(ui);
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Returns the impersonalization message for current user.
    /// </summary>
    /// <param name="ui">User info</param>
    protected string GetImpersonalMessage(UserInfo ui)
    {
        string message = String.Empty;

        // Editor message
        if (ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, CurrentSiteName))
        {
            message = GetString("Membership.ImperConfirmEditor");
        }
        // Default user message
        else
        {
            message = GetString("Membership.ImperConfirmDefault");
        }

        return message;
    }


    /// <summary>
    /// Loads data of edited user from DB into TextBoxes.
    /// </summary>
    protected void LoadData()
    {
        // Fill lstUICulture (loop over and localize them first)
        DataSet uiCultures = CultureInfoProvider.GetUICultures().OrderBy("CultureName");
        LocalizeCultureNames(uiCultures);
        lstUICulture.DataSource = uiCultures.Tables[0].DefaultView;
        lstUICulture.DataTextField = "CultureName";
        lstUICulture.DataValueField = "CultureID";
        lstUICulture.DataBind();

        lstUICulture.Items.Insert(0, GetString("Administration-User_Edit.Default"));
        lstUICulture.Items[0].Value = "0";

        if (ui != null)
        {
            // Get user info properties
            txtEmail.Text = ui.Email;
            txtFirstName.Text = ui.FirstName;
            txtFullName.Text = ui.FullName;
            txtLastName.Text = ui.LastName;
            txtMiddleName.Text = ui.MiddleName;
            ucUserName.Value = ui.UserName;

            CheckBoxEnabled.Checked = ui.Enabled;
            chkIsExternal.Checked = ui.IsExternal;
            chkIsDomain.Checked = ui.UserIsDomain;
            chkIsHidden.Checked = ui.UserIsHidden;
            drpMacroIdentity.Value = UserMacroIdentityInfoProvider.GetUserMacroIdentityInfo(ui)?.UserMacroIdentityMacroIdentityID;
            chkIsMFRequired.Checked = ui.UserMFRequired;

            // Privilege and macro signature identity drop down check
            if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                drpMacroIdentity.Enabled = false;

                // Disable for global admins
                if (ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
                {
                    drpPrivilege.Enabled = false;
                }
                else
                    // Only global admin can manage other admins.
                    if (ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
                    {
                        // Allow manage only for user himself
                        if (ui.UserID != CurrentUser.UserID)
                        {
                            drpPrivilege.Enabled = false;
                        }
                        else
                        {
                            drpPrivilege.ExcludedValues = ((int)UserPrivilegeLevelEnum.GlobalAdmin).ToString();
                        }
                    }
                    else
                    {
                        drpPrivilege.ExcludedValues = (int)UserPrivilegeLevelEnum.GlobalAdmin + ";" + (int)UserPrivilegeLevelEnum.Admin;
                    }
            }

            drpPrivilege.Value = (int)ui.SiteIndependentPrivilegeLevel;

            password = ui.GetValue("UserPassword").ToString();

            // Disable username textbox for public user
            if (ui.IsPublic())
            {
                ucUserName.Enabled = false;
            }

            myCulture = ui.PreferredCultureCode;
            myUICulture = ui.PreferredUICultureCode;

            lblInvalidLogonAttemptsNumber.Text = string.Format(GetString("general.attempts"), ui.UserInvalidLogOnAttempts);
            if (ui.UserInvalidLogOnAttempts > 0)
            {
                lblInvalidLogonAttemptsNumber.Style.Add(HtmlTextWriterStyle.Color, "Red");
            }
            else
            {
                btnResetLogonAttempts.Enabled = (ui.UserAccountLockReason == UserAccountLockCode.FromEnum(UserAccountLockEnum.MaximumInvalidLogonAttemptsReached));
            }

            LoadPasswordExpiration(ui);

            txtUserStartingPath.Text = ui.UserStartingAliasPath;
        }

        // Set content culture
        cultureSelector.Value = myCulture;

        if (!string.IsNullOrEmpty(myUICulture))
        {
            // Set UI culture
            try
            {
                CultureInfo ciUI = CultureInfo.Provider.Get(myUICulture);
                lstUICulture.SelectedIndex = lstUICulture.Items.IndexOf(lstUICulture.Items.FindByValue(ciUI.CultureID.ToString()));
            }
            catch
            {
                lstUICulture.SelectedIndex = lstUICulture.Items.IndexOf(lstUICulture.Items.FindByValue("0"));
            }
        }
        else
        {
            lstUICulture.SelectedIndex = lstUICulture.Items.IndexOf(lstUICulture.Items.FindByValue("0"));
        }

        if (ui != null)
        {
            // If new user
            lblCreatedInfo.Text = ui.UserCreated.ToString();
            lblLastLogonTime.Text = ui.LastLogon.ToString();

            LoadUserLogon(ui);

            if (ui.UserCreated == DateTimeHelper.ZERO_TIME)
            {
                lblCreatedInfo.Text = GetString("general.na");
            }

            if (ui.LastLogon == DateTimeHelper.ZERO_TIME)
            {
                lblLastLogonTime.Text = GetString("general.na");
            }
        }
    }


    /// <summary>
    /// Displays user's last logon information.
    /// </summary>
    /// <param name="ui">User info</param>
    protected void LoadUserLogon(UserInfo ui)
    {
        if ((ui.UserLastLogonInfo != null) && (ui.UserLastLogonInfo.ColumnNames != null) && (ui.UserLastLogonInfo.ColumnNames.Count > 0))
        {
            foreach (string column in ui.UserLastLogonInfo.ColumnNames)
            {
                // Create new control to display last logon information
                Panel grp = new Panel
                {
                    CssClass = "control-group-inline"
                };
                plcUserLastLogonInfo.Controls.Add(grp);
                Label lbl = new Label();
                grp.Controls.Add(lbl);
                lbl.CssClass = "form-control-text";
                lbl.Text = HTMLHelper.HTMLEncode(TextHelper.LimitLength((string)ui.UserLastLogonInfo[column], 80, "..."));
                lbl.ToolTip = HTMLHelper.HTMLEncode(column + " - " + (string)ui.UserLastLogonInfo[column]);
            }
        }
        else
        {
            plcUserLastLogonInfo.Controls.Add(new LocalizedLabel
            {
                ResourceString = "general.na",
                CssClass = "form-control-text"
            });
        }
    }


    /// <summary>
    /// Check whether current user is allowed to modify another user. Return "" or error message.
    /// </summary>
    /// <param name="userId">Modified user</param>
    protected string ValidateGlobalAndDeskAdmin(int userId)
    {
        var editedUser = UserInfo.Provider.Get(userId);
        if (editedUser == null)
        {
            return GetString("Administration-User.WrongUserId");
        }

        if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            // User is global admin - can edit anyone
            return String.Empty;
        }

        if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            // Site admin can't edit other admins except himself
            if (editedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && userId != CurrentUser.UserID)
            {
                return GetString("Administration-User.NotAllowedToModify");
            }

            // Site admin can edit other lower privilege users
            return String.Empty;
        }

        if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor) && !editedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            // Editor is able to edit other editors and normal users
            return String.Empty;
        }

        return GetString("Administration-User.NotAllowedToModify");
    }


    /// <summary>
    /// Localizes culture names and sorts them in ascending order.
    /// </summary>
    /// <param name="uiCultures">DataSet containing the UI cultures</param>
    private void LocalizeCultureNames(DataSet uiCultures)
    {
        // Localize all available UI cultures
        if (!DataHelper.DataSourceIsEmpty(uiCultures))
        {
            for (int i = 0; i < uiCultures.Tables[0].Rows.Count; i++)
            {
                uiCultures.Tables[0].Rows[i]["CultureName"] = ResHelper.LocalizeString(uiCultures.Tables[0].Rows[i]["CultureName"].ToString());
            }
        }

        uiCultures.Tables[0].DefaultView.Sort = "CultureName ASC";
    }


    /// <summary>
    /// Load user password expiration
    /// </summary>
    /// <param name="ui">User info</param>
    protected void LoadPasswordExpiration(UserInfo ui)
    {
        int expDays = 0;

        lblExpireIn.Style.Clear();
        lblPassExpiration.Text = GetString("Administration-User_Edit_General.PasswordExpireIn");

        if (!AuthenticationHelper.IsPasswordExpirationEnabled(SiteContext.CurrentSiteName, out expDays))
        {
            lblExpireIn.Text = GetString("security.never");
            btnExtendValidity.Enabled = (ui.UserAccountLockReason == UserAccountLockCode.FromEnum(UserAccountLockEnum.PasswordExpired));
        }
        else
        {
            // Get password expiration, negative number means not expired, positive means expired, DateTime.Min means not expired but never changed password
            int dayDiff = (ui.UserPasswordLastChanged == DateTime.MinValue) ? -expDays : ((DateTime.Now - ui.UserPasswordLastChanged).Days - expDays);
            if (dayDiff >= 0)
            {
                lblPassExpiration.Text = GetString("Administration-User_Edit_General.PasswordExpired");
                lblExpireIn.Style.Add(HtmlTextWriterStyle.Color, "Red");
            }

            lblExpireIn.Text = string.Format(GetString("general.validity.days"), Math.Abs(dayDiff));
        }
    }


    /// <summary>
    /// Reset user account lock status
    /// </summary>
    protected void btnResetLogonAttempts_Click(object sender, EventArgs e)
    {
        if (!CheckGlobalAdminEdit(ui))
        {
            plcTable.Visible = false;
            ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));

            return;
        }

        // Check "modify" permission
        if (!CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        bool unlocked = false;

        if (ui.UserAccountLockReason == UserAccountLockCode.FromEnum(UserAccountLockEnum.MaximumInvalidLogonAttemptsReached))
        {
            AuthenticationHelper.UnlockUserAccount(ui);
            unlocked = true;
        }
        else
        {
            ui.UserInvalidLogOnAttempts = 0;
            UserInfo.Provider.Set(ui);
        }

        LoadData();
        lblInvalidLogonAttemptsNumber.Style.Clear();

        ShowConfirmation(unlocked ? GetString("Administration-User.InvalidLogonAttemptsResetUnlock") : GetString("Administration-User.InvalidLogonAttemptsReset"));
    }


    /// <summary>
    /// Reset user account lock status
    /// </summary>
    protected void btnExtendValidity_Click(object sender, EventArgs e)
    {
        if (!CheckGlobalAdminEdit(ui))
        {
            plcTable.Visible = false;
            ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));

            return;
        }

        // Check "modify" permission
        if (!CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        bool unlocked = false;

        ui.UserPasswordLastChanged = DateTime.Now;
        if (ui.UserAccountLockReason == UserAccountLockCode.FromEnum(UserAccountLockEnum.PasswordExpired))
        {
            AuthenticationHelper.UnlockUserAccount(ui);
            unlocked = true;
        }
        else
        {
            UserInfo.Provider.Set(ui);
        }

        LoadData();

        if (unlocked)
        {
            ShowConfirmation(GetString("Administration-User.ExtendPasswordUnlock"));
        }
        else
        {
            ShowConfirmation(GetString("Administration-User.ExtendPassword"));
        }
    }


    /// <summary>
    /// Reset token to initial state.
    /// </summary>
    protected void btnResetToken_Click(object sender, EventArgs e)
    {
        if (!CheckGlobalAdminEdit(ui))
        {
            plcTable.Visible = false;
            ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));

            return;
        }

        // Check "modify" permission
        if (!CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        MFAuthenticationHelper.ResetSecretForUser(ui);
        LoadData();
        ShowConfirmation(GetString("administration-user.token.reset"));
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Checks if currently logged user changes his user name.
    /// </summary>
    private bool CurrentUserChangedUserName()
    {
        return (CurrentUser != null) && (CurrentUser.UserID == ui.UserID) && (CurrentUser.UserName != ui.UserName);
    }

    #endregion
}
