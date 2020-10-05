using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;

public partial class CMSModules_Modules_Pages_Module_Permission_Roles : GlobalAdminPage
{
    #region "Constants"

    /// <summary>
    /// CSS class for highlighted UI permission matrix rows.
    /// </summary>
    private const string HIGHLIGHTED_ROW_CSS = "highlighted";

    #endregion


    #region "Variables"

    private int mPermissionId = 0;
    private UserInfo mSelectedUser = null;
    private PermissionNameInfo mPermission = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets selected user ID.
    /// </summary>
    private int SelectedUserID
    {
        get
        {
            return ValidationHelper.GetInteger(userSelector.Value, 0);
        }
    }


    /// <summary>
    /// Gets UserInfo object for selected user.
    /// </summary>
    private UserInfo SelectedUser
    {
        get
        {
            if (mSelectedUser == null && SelectedUserID > 0)
            {
                mSelectedUser = UserInfo.Provider.Get(SelectedUserID);
            }
            return mSelectedUser;
        }
    }


    /// <summary>
    /// Gets PermissionNameInfo object for selected permission element.
    /// </summary>
    private PermissionNameInfo Permission
    {
        get
        {
            if (mPermission == null)
            {
                mPermissionId = QueryHelper.GetInteger("permissionId", 0);
                mPermission = PermissionNameInfo.Provider.Get(mPermissionId);
            }
            return mPermission;
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


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize matrix control
        gridMatrix.ColumnsCount = 1;
        gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;
        gridMatrix.CornerText = GetString("administration-module_edit_permissionnames.role");
        gridMatrix.ContentBeforeRowCssClass = "content-before";
        gridMatrix.OnGetRowItemCssClass += gridMatrix_OnGetRowItemCssClass;
        gridMatrix.NoRecordsMessage = GetString("general.emptymatrix");

        // Show site selector placeholder on locations where the site selector can be shown
        CurrentMaster.DisplaySiteSelectorPanel = true;

        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        siteSelector.UniSelector.OnSelectionChanged += UniSelector_SelectedIndexChanged;

        // Initialize user selector
        userSelector.SiteID = (siteSelector.SiteID > 0) ? siteSelector.SiteID : 0;
        userSelector.DropDownSingleSelect.AutoPostBack = true;

        // Display all users if global
        if (userSelector.SiteID <= 0)
        {
            userSelector.DisplayUsersFromAllSites = true;
        }
        else
        {
            userSelector.DisplayUsersFromAllSites = false;
        }

        chkUserOnly.Text = GetString("Administration-Permissions_Header.UserRoles");
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Load the matrix
        if (Permission != null)
        {
            // Disable check box if no user selected
            if (SelectedUserID > 0)
            {
                chkUserOnly.Enabled = true;
            }
            else
            {
                chkUserOnly.Checked = false;
                chkUserOnly.Enabled = false;
            }

            headTitle.Text = String.Format(GetString("administration-module_edit_permissionnames.rolesinfo"), HTMLHelper.HTMLEncode(Permission.PermissionDisplayName));

            gridMatrix.QueryParameters = GetQueryParameters(siteSelector.SiteID, Permission.PermissionId, Permission.PermissionDisplayName);
            gridMatrix.WhereCondition = GetWhereCondition();
            gridMatrix.ShowContentBeforeRow = (SelectedUser != null);

            GenerateBeforeRowsContent();
            ShowInformation(String.Empty);

            if (!gridMatrix.HasData)
            {
                plcUpdate.Visible = false;
                headTitle.Text = (chkUserOnly.Checked) ? GetString("general.norolemember") : GetString("general.emptymatrix");
            }
            else
            {
                // Inform user that global admin was selected
                if ((SelectedUserID > 0) && (SelectedUser != null) && SelectedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
                {
                    ShowInformation(GetString("Administration-Permissions_Matrix.GlobalAdministrator"));
                }

                plcUpdate.Visible = true;
            }
        }
        base.OnPreRender(e);
    }

    #endregion


    #region "Event Handlers"

    protected void UniSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Reload user selector
        userSelector.ReloadData();
        mSelectedUser = null;

        // Set matrix current page to first     
        gridMatrix.Pager.CurrentPage = 1;
    }


    protected void gridMatrix_OnItemChanged(object sender, int rowItemId, int colItemId, bool newState)
    {
        if (newState)
        {
            RolePermissionInfo.Provider.Add(rowItemId, colItemId);
        }
        else
        {
            RolePermissionInfo.Provider.Remove(rowItemId, colItemId);
        }
        // Invalidate all users
        UserInfo.TYPEINFO.InvalidateAllObjects();

        // Update content before rows
        GenerateBeforeRowsContent();
    }


    protected string gridMatrix_OnGetRowItemCssClass(object sender, DataRow dr)
    {
        string roleName = ValidationHelper.GetString(dr["RoleName"], String.Empty);

        // Check if all necessary data are available
        if (!String.IsNullOrEmpty(roleName) && (SelectedUser != null))
        {
            if (SelectedUser.IsInRole(roleName, siteSelector.SiteName))
            {
                return HIGHLIGHTED_ROW_CSS;
            }
        }

        return String.Empty;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns query parameters for permission matrix.
    /// </summary>
    /// <param name="siteId">Site ID</param>
    /// <param name="permissionId">Permission ID</param>
    /// <param name="permissionName">Permission display name</param>
    /// <returns>Two dimensional object array.</returns>
    private QueryDataParameters GetQueryParameters(int siteId, int permissionId, string permissionName)
    {
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@SiteID", (ValidationHelper.GetString(siteSelector.Value, String.Empty) == siteSelector.GlobalRecordValue) ? 0 : siteId);
        parameters.Add("@PermissionID", permissionId);
        parameters.Add("@PermissionDisplayName", ResHelper.LocalizeString(permissionName));

        return parameters;
    }


    /// <summary>
    /// Gets user effective resource permission HTML content.
    /// </summary>
    private void GenerateBeforeRowsContent()
    {
        ResourceInfo resource = ResourceInfo.Provider.Get(Permission.ResourceId);

        // Check if every necessary property is set
        if ((SelectedUser != null) && (Permission != null) && (resource != null))
        {
            // Initialize variables used during rendering
            string userName = HTMLHelper.HTMLEncode(TextHelper.LimitLength(Functions.GetFormattedUserName(SelectedUser.UserName, SelectedUser.FullName), 50));
            bool authorizedPerResource = UserInfoProvider.IsAuthorizedPerResource(resource.ResourceName, Permission.PermissionName, siteSelector.SiteName, SelectedUser);

            // Create header table cell
            var tcHeader = new TableCell
            {
                CssClass = "matrix-header",
                ToolTip = userName,
                Text = userName
            };

            var tr = gridMatrix.ContentBeforeRow;
            tr.Cells.Add(tcHeader);

            // Create resource permission cell
            var tc = new TableCell();
            var chk = new CMSCheckBox
            {
                Checked = SelectedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || authorizedPerResource,
                Enabled = false,
                ToolTip = GetString("Administration-Permissions_Matrix.NotAdjustable")
            };
            tc.Controls.Add(chk);

            tr.Cells.Add(tc);

            UniMatrix.AddFillingCell<TableCell>(tr);
        }
    }


    /// <summary>
    /// Gets where condition for the matrix.
    /// </summary>
    /// <returns>String representing where condition for the matrix</returns>
    private string GetWhereCondition()
    {
        string where = null;

        if (chkUserOnly.Checked && (SelectedUserID > 0))
        {
            // Get selected site name
            string siteName = UserInfo.GLOBAL_ROLES_KEY;
            if (siteSelector.SiteID > 0)
            {
                siteName = siteSelector.SiteName.ToLowerCSafe();
            }

            // Build roles by comma string
            StringBuilder sbRolesWhere = new StringBuilder();
            foreach (int roleId in ((Hashtable)SelectedUser.SitesRoles[siteName]).Values)
            {
                sbRolesWhere.Append(",");
                sbRolesWhere.Append(roleId);
            }
            string rolesWhere = sbRolesWhere.ToString();

            // Add roles where condition
            if (!String.IsNullOrEmpty(rolesWhere))
            {
                rolesWhere = rolesWhere.Remove(0, 1);
                where = "RoleID IN (" + rolesWhere + ")";
            }
            else
            {
                where = SqlHelper.NO_DATA_WHERE;
                gridMatrix.StopProcessing = true;
            }
        }

        return where;
    }

    #endregion
}