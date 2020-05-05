using System;
using System.Collections;
using System.Data;
using System.Linq;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Users_User_Edit_Roles : CMSUsersPage
{
    #region "Protected variables"

    protected int mSiteId;
    protected int mUserId;
    protected UserInfo mUserInfo;
    protected string mCurrentValues = string.Empty;

    #endregion


    #region "Events"

    /// <summary>
    /// Page_load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions and UI elements
        var user = MembershipContext.AuthenticatedUser;
        if (user != null)
        {
            if (!user.IsAuthorizedPerUIElement("CMS.Users", "CmsDesk.Roles"))
            {
                RedirectToUIElementAccessDenied("CMS.Users", "CmsDesk.Roles");
            }

            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Roles", "Read"))
            {
                RedirectToAccessDenied("CMS.Roles", "Read");
            }
        }

        ScriptHelper.RegisterJQuery(Page);

        // Get user id and site Id from query
        mUserId = QueryHelper.GetInteger("userid", 0);

        // Show content placeholder where site selector can be shown
        CurrentMaster.DisplaySiteSelectorPanel = true;

        if ((SiteID > 0) && !MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            plcSites.Visible = false;
            CurrentMaster.DisplaySiteSelectorPanel = false;
        }

        if (mUserId > 0)
        {
            // Check that only global administrator can edit global administrator's accounts
            mUserInfo = UserInfoProvider.GetUserInfo(mUserId);
            CheckUserAvaibleOnSite(mUserInfo);
            EditedObject = mUserInfo;

            if (!CheckGlobalAdminEdit(mUserInfo))
            {
                plcTable.Visible = false;
                ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
                return;
            }

            // Set site selector
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.AllowAll = false;
            siteSelector.AllowEmpty = false;

            // Global roles only for global admin
            if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                siteSelector.AllowGlobal = true;
            }

            // Only sites assigned to user
            siteSelector.UserId = mUserId;
            siteSelector.OnlyRunningSites = false;
            siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

            if (!RequestHelper.IsPostBack())
            {
                mSiteId = SiteContext.CurrentSiteID;

                // If user is member of current site
                if (UserSiteInfoProvider.GetUserSiteInfo(mUserId, mSiteId) != null)
                {
                    // Force uniselector to preselect current site
                    siteSelector.Value = mSiteId;
                }

                // Force to load data
                siteSelector.Reload(true);
            }

            // Get truly selected item
            mSiteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
        }

        usRoles.OnSelectionChanged += usRoles_OnSelectionChanged;
        string siteIDWhere = (mSiteId <= 0) ? " SiteID IS NULL " : " SiteID =" + mSiteId;
        usRoles.WhereCondition = siteIDWhere + " AND RoleGroupID IS NULL";

        usRoles.SelectItemPageUrl = "~/CMSModules/Membership/Pages/Users/User_Edit_Add_Item_Dialog.aspx";
        usRoles.ListingWhereCondition = siteIDWhere + " AND RoleGroupID IS NULL AND UserID=" + mUserId;
        usRoles.ReturnColumnName = "RoleID";
        usRoles.DynamicColumnName = false;
        usRoles.GridName = "User_Role_List.xml";
        usRoles.AdditionalColumns = "ValidTo";
        usRoles.OnAdditionalDataBound += usMemberships_OnAdditionalDataBound;
        usRoles.DialogWindowHeight = 760;

        // Exclude generic roles
        string genericWhere = String.Empty;
        ArrayList genericRoles = RoleInfoProvider.GetGenericRoles();
        if (genericRoles.Count != 0)
        {
            foreach (string role in genericRoles)
            {
                genericWhere += "'" + SqlHelper.EscapeQuotes(role) + "',";
            }

            genericWhere = genericWhere.TrimEnd(',');
            usRoles.WhereCondition += " AND ( RoleName NOT IN (" + genericWhere + ") )";
        }

        // Get the active roles for this site
        var roleIds = new IDQuery<RoleInfo>().Where(siteIDWhere).Column("RoleID");
        var data = UserRoleInfoProvider.GetUserRoles().WhereEquals("UserID", mUserId).And().WhereIn("RoleID", roleIds).Columns("RoleID").TypedResult;
        if (data.Any())
        {
            mCurrentValues = TextHelper.Join(";", data.Select(i => i.RoleID));
        }

        // If not postback or site selection changed
        if (!RequestHelper.IsPostBack() || (mSiteId != Convert.ToInt32(ViewState["rolesOldSiteId"])))
        {
            // Set values
            usRoles.Value = mCurrentValues;
        }

        // Store selected site id
        ViewState["rolesOldSiteId"] = mSiteId;

        string script = "function setNewDateTime(date) {$cmsj('#" + hdnDate.ClientID + "').val(date);}";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "key", ScriptHelper.GetScript(script));

        string eventTarget = Request[postEventSourceID];
        string eventArgument = Request[postEventArgumentID];
        if (eventTarget == ucCalendar.DateTimeTextBox.UniqueID)
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "ManageUserRoles"))
            {
                RedirectToAccessDenied("CMS.Users", "Manage user roles");
            }

            int id = ValidationHelper.GetInteger(hdnDate.Value, 0);
            if (id != 0)
            {
                DateTime dt = ValidationHelper.GetDateTime(eventArgument, DateTimeHelper.ZERO_TIME);
                UserRoleInfo uri = UserRoleInfoProvider.GetUserRoleInfo(mUserId, id);
                if (uri != null)
                {
                    uri.ValidTo = dt;
                    UserRoleInfoProvider.SetUserRoleInfo(uri);

                    // Invalidate user
                    UserInfoProvider.InvalidateUser(mUserId);

                    ShowChangesSaved();
                }
            }
        }
    }


    /// <summary>
    /// Callback event for create calendar icon.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="sourceName">Event source name</param>
    /// <param name="parameter">Event parameter</param>
    /// <param name="val">Value from basic external data bound event</param>
    private object usMemberships_OnAdditionalDataBound(object sender, string sourceName, object parameter, object val)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "calendar":
                DataRowView drv = (parameter as DataRowView);
                string itemID = drv[usRoles.ReturnColumnName].ToString();
                string iconID = "icon_" + itemID;
                string date = drv["ValidTo"].ToString();
                string postback = ControlsHelper.GetPostBackEventReference(ucCalendar.DateTimeTextBox, "#").Replace("'#'", "$cmsj('#" + ucCalendar.DateTimeTextBox.ClientID + "').val()");
                string onClick = String.Empty;

                ucCalendar.DateTimeTextBox.Attributes["OnChange"] = postback;

                if (!ucCalendar.UseCustomCalendar)
                {
                    onClick = "$cmsj('#" + hdnDate.ClientID + "').val('" + itemID + "');" + ucCalendar.GenerateNonCustomCalendarImageEvent();
                }
                else
                {
                    onClick = "$cmsj('#" + hdnDate.ClientID + "').val('" + itemID + "'); var dt = $cmsj('#" + ucCalendar.DateTimeTextBox.ClientID + "'); dt.val('" + date + "'); dt.cmsdatepicker('setLocation','" + iconID + "'); dt.cmsdatepicker('show');";
                }

                 var button = new CMSGridActionButton
                {
                    ToolTip = GetString("membership.changevalidity"),
                    IconCssClass = "icon-calendar",
                    OnClientClick = onClick + "return false;",
                    ID = iconID
                };

                val = button.GetRenderedHTML();

                break;
        }

        return val;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Display message if user has no site
        if ((!siteSelector.UniSelector.HasData) && (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)))
        {
            ShowError(GetString("Administration-User_Edit_Roles.UserWithNoSites"));
        }
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        pnlUpdate.Update();
    }


    protected void usRoles_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveData();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Check whether current user is allowed to modify another user.
    /// </summary>
    /// <returns>"" or error message.</returns>
    protected string ValidateGlobalAndDeskAdmin()
    {
        string result = String.Empty;

        if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            return result;
        }

        if (mUserInfo == null)
        {
            result = GetString("Administration-User.WrongUserId");
        }
        else
        {
            if (mUserInfo.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                result = GetString("Administration-User.NotAllowedToModify");
            }
        }
        return result;
    }


    /// <summary>
    /// Saves data.
    /// </summary>
    private void SaveData()
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "ManageUserRoles"))
        {
            RedirectToAccessDenied("CMS.Users", "Manage user roles");
        }

        bool saved = false;
        string result = ValidateGlobalAndDeskAdmin();
        if (result != String.Empty)
        {
            ShowError(result);
            return;
        }
        
        string selectorValues = ValidationHelper.GetString(usRoles.Value, null);

        // Remove old items
        string items = DataHelper.GetNewItemsInList(selectorValues, mCurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems.Any())
            {
                // Remove all old items from site
                foreach (string item in newItems)
                {
                    int roleID = ValidationHelper.GetInteger(item, 0);

                    var uri = UserRoleInfoProvider.GetUserRoleInfo(mUserId, roleID);
                    UserRoleInfoProvider.DeleteUserRoleInfo(uri);
                }

                saved = true;
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(mCurrentValues, selectorValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems.Any())
            {
                DateTime dt = ValidationHelper.GetDateTime(hdnDate.Value, DateTimeHelper.ZERO_TIME);

                // Add all new items to site
                foreach (string item in newItems)
                {
                    int roleID = ValidationHelper.GetInteger(item, 0);
                    UserRoleInfoProvider.AddUserToRole(mUserId, roleID, dt);
                }

                saved = true;
            }
        }

        if (saved)
        {
            ShowChangesSaved();
            usRoles.Reload(true);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (RequestHelper.IsPostBack())
        {
            pnlBasic.Update();
        }

        base.OnPreRender(e);
    }

    #endregion
}
