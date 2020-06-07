using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_User_Edit_Languages : CMSUsersPage
{
    #region "Protected variables"

    protected int userId = 0;
    protected UserInfo ui = null;
    protected string currentValues = "";
    private int siteID = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // If languages on site not ok, redirect
        if (!CultureSiteInfoProvider.LicenseVersionCheck())
        {
            URLHelper.Redirect(ResolveUrl("~/CMSMessages/LicenseLimit.aspx"));
        }

        userId = QueryHelper.GetInteger("userid", 0);
        siteID = SiteID;

        if (userId > 0)
        {
            ui = UserInfo.Provider.Get(userId);
            CheckUserAvaibleOnSite(ui);
            EditedObject = ui;

            if (!CheckGlobalAdminEdit(ui))
            {
                pnlLanguages.Visible = false;
                ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
                return;
            }

            // Set site selector
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.AllowAll = false;
            siteSelector.OnlyRunningSites = false;
            siteSelector.UserId = ui.UserID;
            siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

            if (!RequestHelper.IsPostBack())
            {
                // Initialize radio buttons
                radAllLanguages.Checked = !ui.UserHasAllowedCultures;
                radSelectedLanguages.Checked = ui.UserHasAllowedCultures;

                // Load the site selector with the data in order to preselect current site (if available in the list)
                siteSelector.UniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
                siteSelector.UniSelector.AllowEmpty = false;
                siteSelector.Reload(true);

                // Select the current site if it is in the list, otherwise select the first site in the list
                if (siteSelector.DropDownSingleSelect.Items.FindByValue(SiteContext.CurrentSiteID.ToString()) != null)
                {
                    siteSelector.Value = SiteContext.CurrentSiteID;
                }
                else if (siteSelector.DropDownSingleSelect.Items.Count > 0)
                {
                    siteSelector.Value = siteSelector.DropDownSingleSelect.Items[0].Value;
                }
                siteSelector.DropDownSingleSelect.SelectedValue = siteSelector.Value.ToString();
            }


            // Show site selection in administration only for global administrator            
            if ((SiteID > 0) && (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)))
            {
                plcSite.Visible = false;
            }
            else
            {
                plcSite.Visible = radSelectedLanguages.Checked;
                siteID = siteSelector.SiteID;
            }

            // Set grid visibility
            plcCultures.Visible = radSelectedLanguages.Checked;

            // Load user cultures
            DataTable dt = UserCultureInfoProvider.GetUserCultures(userId, siteID);
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(dt, "CultureID"));

            if (!RequestHelper.IsPostBack())
            {
                uniSelector.Value = currentValues;
                uniSelector.Reload(true);
            }
        }

        uniSelector.WhereCondition = "CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = " + siteID + ")";
        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;

        radAllLanguages.CheckedChanged += radAllLanguages_CheckedChanged;
        radSelectedLanguages.CheckedChanged += radSelectedLanguages_CheckedChanged;
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            // Causes site selector to load data
            if (!siteSelector.UniSelector.HasData)
            {
                pnlLanguages.Visible = false;
                plcSite.Visible = false;
                ShowError(GetString("Administration-User_Edit_Roles.UserWithNoSites"));
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        bool reloadNeeded = !CheckModifyPermissions();

        if (!reloadNeeded)
        {
            bool invalidateUser = false;

            // Remove old items
            string newValues = ValidationHelper.GetString(uniSelector.Value, null);
            string items = DataHelper.GetNewItemsInList(newValues, currentValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                
                // Add all new cultures to word
                foreach (string item in newItems)
                {
                    int cultureId = ValidationHelper.GetInteger(item, 0);
                    UserCultureInfo.Provider.Remove(userId, cultureId, siteID);
                }
                invalidateUser = true;
            }

            // Add new items
            items = DataHelper.GetNewItemsInList(currentValues, newValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                
                // Add all new cultures to word
                foreach (string item in newItems)
                {
                    int cultureId = ValidationHelper.GetInteger(item, 0);
                    UserCultureInfo.Provider.Add(userId, cultureId, siteID);
                }
                invalidateUser = true;
            }

            if (invalidateUser)
            {
                ui.Generalized.Invalidate(false);
            }

            ShowChangesSaved();
        }
        else
        {
            ReloadCultures();
        }
    }


    protected void radSelectedLanguages_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckModifyPermissions())
        {
            // Set value indicating whether the user have allowed cultures
            if (ui != null)
            {
                ui.UserHasAllowedCultures = radSelectedLanguages.Checked;
                UserInfo.Provider.Set(ui);
                ui.Generalized.Invalidate(false);

                uniSelector.Value = "";
                uniSelector.Reload(true);
                plcCultures.Visible = true;

                ShowChangesSaved();
            }
        }
    }


    protected void radAllLanguages_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckModifyPermissions())
        {
            // Set value indicating whether the user have allowed cultures
            if (ui != null)
            {
                // Remove user's allowed cultures
                UserCultureInfoProvider.RemoveUserFromAllCultures(userId);
                ui.UserHasAllowedCultures = radSelectedLanguages.Checked;
                UserInfo.Provider.Set(ui);
                ui.Generalized.Invalidate(false);

                plcCultures.Visible = false;

                ShowChangesSaved();
            }
        }
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        uniSelector.Value = currentValues;
        uniSelector.Reload(true);
        pnlUpdate.Update();
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Reloads the cultures in UniSelector.
    /// </summary>
    protected void ReloadCultures()
    {
        DataTable dt = UserCultureInfoProvider.GetUserCultures(userId, SiteID);
        if (!DataHelper.DataSourceIsEmpty(dt))
        {
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(dt, "CultureID"));
            uniSelector.Value = currentValues;
            uniSelector.Reload(true);
        }
    }


    /// <summary>
    /// Checks modify permissions.
    /// </summary>
    protected bool CheckModifyPermissions()
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        // Check permissions
        string result = ValidateGlobalAndDeskAdmin(userId);
        if (result != String.Empty)
        {
            ShowError(result);
            return false;
        }

        return true;
    }


    /// <summary>
    /// Check whether current user is allowed to modify another user.
    /// </summary>
    /// <param name="userId">Modified user</param>
    /// <returns>"" or error message.</returns>
    protected static string ValidateGlobalAndDeskAdmin(int userId)
    {
        string result = String.Empty;

        if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            return result;
        }

        UserInfo userInfo = UserInfo.Provider.Get(userId);
        if (userInfo == null)
        {
            result = GetString("Administration-User.WrongUserId");
        }
        else
        {
            if (userInfo.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                result = GetString("Administration-User.NotAllowedToModify");
            }
        }
        return result;
    }

    #endregion
}
