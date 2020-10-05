using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_User_ManageRoles : CMSModalPage
{
    #region "Variables"

    private int userId;
    private int siteId;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        var user = MembershipContext.AuthenticatedUser;
        if (user != null)
        {
            // Check UI elements
            if (!user.IsAuthorizedPerUIElement("CMS.Users", "CmsDesk.Roles"))
            {
                RedirectToUIElementAccessDenied("CMS.Users", "CmsDesk.Roles");
            }

            if (!user.IsAuthorizedPerUIElement("CMS.Roles", "Users"))
            {
                RedirectToUIElementAccessDenied("CMS.Roles", "Users");
            }

            // Check "read" permissions
            if (!user.IsAuthorizedPerResource("CMS.Users", "Read"))
            {
                RedirectToAccessDenied("CMS.Users", "Read");
            }

            if (!user.IsAuthorizedPerResource("CMS.Roles", "Read"))
            {
                RedirectToAccessDenied("CMS.Roles", "Read");
            }
        }

        userId = QueryHelper.GetInteger("userId", 0);

        // Check that only global administrator can edit global administrator's accounts
        UserInfo ui = UserInfo.Provider.Get(userId);
        var currentUser = MembershipContext.AuthenticatedUser;
        CheckUserAvaibleOnSite(ui);
        if ((ui != null) && !currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) && ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && (ui.UserID != currentUser.UserID))
        {
            plcTable.Visible = false;
            ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
            return;
        }

        // Only global admin can choose the site
        if (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            // Show site selector
            CurrentMaster.DisplaySiteSelectorPanel = true;

            // Set site selector
            siteSelector.DropDownSingleSelect.AutoPostBack = true;
            siteSelector.AllowAll = false;

            if (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                siteSelector.AllowGlobal = true;
            }

            siteSelector.UserId = userId;
            siteSelector.OnlyRunningSites = false;
            siteSelector.UniSelector.OnSelectionChanged += UniSelector_OnSelectionChanged;

            if (!RequestHelper.IsPostBack())
            {
                if ((ui != null) && ui.IsInSite(SiteContext.CurrentSiteName))
                {
                    siteSelector.Value = SiteContext.CurrentSiteID;
                }
                else
                {
                    siteSelector.Value = siteSelector.GlobalRecordValue;
                }
            }

            siteId = siteSelector.SiteID;
        }
        else
        {
            siteSelector.StopProcessing = true;
            siteId = SiteContext.CurrentSiteID;
        }

        // Init UI
        itemSelectionElem.LeftColumnLabel.Text = GetString("user.manageroles.availableroles");
        itemSelectionElem.RightColumnLabel.Text = GetString("user.manageroles.userinroles");
        PageTitle.TitleText = GetString("user.manageroles.header");
        // Register the events
        itemSelectionElem.OnMoveLeft += itemSelectionElem_OnMoveLeft;
        itemSelectionElem.OnMoveRight += itemSelectionElem_OnMoveRight;
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Load the roles into ItemSelection Control
        if (!RequestHelper.IsPostBack())
        {
            LoadRoles();
            itemSelectionElem.fill();
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Test if edited user belongs to current site
    /// </summary>
    /// <param name="ui">User info object</param>
    public void CheckUserAvaibleOnSite(UserInfo ui)
    {
        if (ui != null)
        {
            if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && !ui.IsInSite(SiteContext.CurrentSiteName))
            {
                RedirectToInformation(GetString("user.notinsite"));
            }
        }
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        LoadRoles();
        itemSelectionElem.fill();
        pnlUpdate.Update();
    }


    #region "ItemSelection events"

    protected void itemSelectionElem_OnMoveRight(object sender, CommandEventArgs e)
    {
        // Permissions check
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "ManageUserRoles"))
        {
            RedirectToAccessDenied("CMS.Users", "Manage user roles");
        }

        string argument = ValidationHelper.GetString(e.CommandArgument, "");
        if (!string.IsNullOrEmpty(argument))
        {
            string[] ids = argument.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in ids)
            {
                UserInfoProvider.AddUserToRole(userId, ValidationHelper.GetInteger(id, 0));
            }
        }
    }


    protected void itemSelectionElem_OnMoveLeft(object sender, CommandEventArgs e)
    {
        // Permissions check
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "ManageUserRoles"))
        {
            RedirectToAccessDenied("CMS.Users", "Manage user roles");
        }

        string argument = ValidationHelper.GetString(e.CommandArgument, "");
        if (!string.IsNullOrEmpty(argument))
        {
            string[] ids = argument.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in ids)
            {
                UserInfoProvider.RemoveUserFromRole(userId, ValidationHelper.GetInteger(id, 0));
            }
        }
    }

    #endregion


    #region "Private helper functions"

    private void LoadRoles()
    {
        itemSelectionElem.LeftItems = GetSelectionControlItems(GetRoleDataset(false));
        itemSelectionElem.RightItems = GetSelectionControlItems(GetRoleDataset(true));
    }


    /// <summary>
    /// Returns the dataset with roles for specified site.
    /// </summary>
    /// <param name="users">Determines whether to return only roles to which the user is assigned</param>
    private ObjectQuery<RoleInfo> GetRoleDataset(bool users)
    {
        string siteWhere = (siteId.ToString() == siteSelector.GlobalRecordValue) ? "SiteID IS NULL" : "SiteID = " + siteId;
        string where = "(RoleID " + (users ? "" : "NOT ") + "IN (SELECT RoleID FROM CMS_UserRole WHERE UserID = " + userId + ")) AND " + siteWhere;

        // Exclude generic roles
        string genericWhere = null;
        ArrayList genericRoles = RoleInfoProvider.GetGenericRoles();
        if (genericRoles.Count != 0)
        {
            foreach (string role in genericRoles)
            {
                genericWhere += "'" + SqlHelper.GetSafeQueryString(role, false) + "',";
            }

            if (genericWhere != null)
            {
                genericWhere = genericWhere.TrimEnd(',');
            }
            where += " AND ( RoleName NOT IN (" + genericWhere + ") )";
        }

        return RoleInfo.Provider.Get().Where(where).OrderBy("RoleDisplayName").Columns("RoleDisplayName, RoleID");
    }


    /// <summary>
    /// Returns the 2 dimensional array which can be used in ItemSelection control.
    /// </summary>
    private static string[,] GetSelectionControlItems(DataSet ds)
    {
        string[,] retval = null;
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            retval = new string[ds.Tables[0].Rows.Count, 2];
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                retval[i, 1] = Convert.ToString(dr["RoleDisplayName"]);
                retval[i, 0] = Convert.ToString(dr["RoleID"]);
            }
        }
        return retval;
    }

    #endregion
}