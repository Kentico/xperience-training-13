using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;

using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Permissions_Controls_PermissionsMatrix : CMSAdminControl
{
    #region "Constants"

    /// <summary>
    /// CSS class for highlighted permission matrix rows.
    /// </summary>
    private const string HIGHLIGHTED_ROW_CSS = "highlighted";

    #endregion


    #region "Variables"

    private string mSelectedType = string.Empty;

    private UserInfo mSelectedUser;
    private SiteInfo mSelectedSite;

    private CurrentUserInfo currentUser;

    #endregion


    #region "Properties"

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


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets Site ID.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Role ID.
    /// </summary>
    public int RoleID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets ID selected in the moduleSelector or docTypeSelector or customTableSelector according to the selected PermissionType.
    /// </summary>
    public string SelectedID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets type constant according to the selected value in the moduleSelector or docTypeSelector or customTableSelector and the selected PermissionType.
    /// </summary>
    public string SelectedType
    {
        get
        {
            return mSelectedType;
        }
        set
        {
            mSelectedType = value;
        }
    }


    /// <summary>
    /// Currently selected user ID for report
    /// </summary>
    public int SelectedUserID
    {
        get;
        set;
    }


    /// <summary>
    /// Query name to get the data.
    /// </summary>
    public string QueryName
    {
        get
        {
            return gridMatrix.QueryName;
        }
        set
        {
            gridMatrix.QueryName = value;
        }
    }


    /// <summary>
    /// ID column of the row.
    /// </summary>
    public string RowItemIDColumn
    {
        get
        {
            return gridMatrix.RowItemIDColumn;
        }
        set
        {
            gridMatrix.RowItemIDColumn = value;
        }
    }


    /// <summary>
    /// Display name column of the row.
    /// </summary>
    public string RowItemDisplayNameColumn
    {
        get
        {
            return gridMatrix.RowItemDisplayNameColumn;
        }
        set
        {
            gridMatrix.RowItemDisplayNameColumn = value;
        }
    }


    /// <summary>
    /// Tooltip column of the row.
    /// </summary>
    public string RowItemTooltipColumn
    {
        get
        {
            return gridMatrix.RowItemTooltipColumn;
        }
        set
        {
            gridMatrix.RowItemTooltipColumn = value;
        }
    }


    /// <summary>
    /// Display name column of the column.
    /// </summary>
    public string ColumnItemDisplayNameColumn
    {
        get
        {
            return gridMatrix.ColumnItemDisplayNameColumn;
        }
        set
        {
            gridMatrix.ColumnItemDisplayNameColumn = value;
        }
    }


    /// <summary>
    /// ID column of the column.
    /// </summary>
    public string ColumnItemIDColumn
    {
        get
        {
            return gridMatrix.ColumnItemIDColumn;
        }
        set
        {
            gridMatrix.ColumnItemIDColumn = value;
        }
    }


    /// <summary>
    /// Tooltip column of the column.
    /// </summary>
    public string ColumnItemTooltipColumn
    {
        get
        {
            return gridMatrix.ColumnItemTooltipColumn;
        }
        set
        {
            gridMatrix.ColumnItemTooltipColumn = value;
        }
    }


    /// <summary>
    /// Tooltip column of the item.
    /// </summary>
    public string ItemTooltipColumn
    {
        get
        {
            return gridMatrix.ItemTooltipColumn;
        }
        set
        {
            gridMatrix.ItemTooltipColumn = value;
        }
    }


    /// <summary>
    /// Gets or sets the text displayed in the upper left corner, if filter is not shown.
    /// </summary>
    public string CornerText
    {
        get
        {
            return gridMatrix.CornerText;
        }
        set
        {
            gridMatrix.CornerText = value;
        }
    }


    /// <summary>
    /// Indicates whether show global roles.
    /// </summary>
    public bool GlobalRoles
    {
        get;
        set;
    }


    /// <summary>
    /// Gets UserInfo object for selected user ID.
    /// </summary>
    private UserInfo SelectedUser
    {
        get
        {
            if (mSelectedUser == null)
            {
                mSelectedUser = UserInfo.Provider.Get(SelectedUserID);
            }
            return mSelectedUser;
        }
    }


    /// <summary>
    /// Gets SiteInfo object for selected site ID.
    /// </summary>
    private SiteInfo SelectedSite
    {
        get
        {
            if (mSelectedSite == null)
            {
                mSelectedSite = SiteInfo.Provider.Get(SiteID);
            }
            return mSelectedSite;
        }
    }


    /// <summary>
    /// Indicates if only selected user roles should be displayed.
    /// </summary>
    public bool UserRolesOnly
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if filter was changed.
    /// </summary>
    public bool FilterChanged
    {
        get;
        set;
    }


    /// <summary>
    /// If true, the permissions are used as rows
    /// </summary>
    public bool PermissionsAsRows
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when data has been loaded. Allows manipulation with data.
    /// </summary>
    public delegate void OnMatrixDataLoaded(DataSet ds);

    public event OnMatrixDataLoaded OnDataLoaded;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridMatrix.OrderBy = "Matrix.RoleDisplayName, Matrix.RoleID, Matrix.PermissionOrder";
        gridMatrix.CssClass = "permission-matrix";
        gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;
        gridMatrix.DataLoaded += gridMatrix_DataLoaded;

        // Warning icon for disabled permissions
        CMSIcon iconWarning = new CMSIcon
        {         
            CssClass = "icon-exclamation-triangle warning-icon",
            ToolTip = GetString("PermissionMatrix.GlobalAdminOnly"),
            AlternativeText = GetString("PermissionMatrix.GlobalAdminOnly")
        };
        
        if (PermissionsAsRows)
        {
            gridMatrix.CheckRowPermissions += gridMatrix_CheckPermissions;
            gridMatrix.DisabledRowMark = iconWarning.GetRenderedHTML();
        }
        else
        {
            gridMatrix.CheckColumnPermissions += gridMatrix_CheckPermissions;
            gridMatrix.DisabledColumnMark = iconWarning.GetRenderedHTML();
        }

        gridMatrix.ContentBeforeRowCssClass = "content-before";
        gridMatrix.OnGetRowItemCssClass += gridMatrix_OnGetRowItemCssClass;
        gridMatrix.NoRecordsMessage = GetString("general.emptymatrix");

        currentUser = MembershipContext.AuthenticatedUser;

        // Is current user authorized to manage permissions? 
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Permissions", "Manage"))
        {
            gridMatrix.Enabled = false;
            ShowWarning(string.Format(GetString("general.accessdeniedonpermissionname"), "Manage"), null, null);
        }
    }


    protected bool gridMatrix_CheckPermissions(object permId)
    {
        int permissionId = ValidationHelper.GetInteger(permId, 0);

        // Check how the permission can be edited
        PermissionNameInfo pni = PermissionNameInfo.Provider.Get(permissionId);
        if (pni != null)
        {
            return currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) || !pni.PermissionEditableByGlobalAdmin;
        }

        return true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Initialize value dependant properties of uni matrix
        gridMatrix.ShowContentBeforeRow = SelectedUserID > 0;
        gridMatrix.QueryName = GetQueryNameByType();
        gridMatrix.QueryParameters = GetQueryParameters();
        gridMatrix.WhereCondition = GetWhereCondition();

        if (FilterChanged || (gridMatrix.Pager.CurrentPage <= 0))
        {
            gridMatrix.Pager.CurrentPage = 1;
        }
        if (gridMatrix.Pager.CurrentPage > gridMatrix.Pager.PageCount)
        {
            gridMatrix.Pager.CurrentPage = gridMatrix.Pager.PageCount;
        }

        int selectedId = ValidationHelper.GetInteger(SelectedID, 0);
        ShowInformation(String.Empty);

        if (!gridMatrix.HasData)
        {
            if (UserRolesOnly && (selectedId > 0))
            {
                lblInfo.Text = GetString("general.norolemember");
            }
        }
        else
        {
            // Inform user that global admin was selected
            if ((SelectedUserID > 0) && SelectedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                ShowInformation(GetString("Administration-Permissions_Matrix.GlobalAdministrator"));
            }
        }
        lblInfo.Visible = !string.IsNullOrEmpty(lblInfo.Text);

        // Set content before rows and refresh matrix content
        GenerateBeforeRowsContent(SiteID, selectedId, SelectedType);
        pnlUpdMat.Update();

        base.OnPreRender(e);
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Gets where condition for the matrix according to the RoleID.
    /// </summary>
    /// <returns>String representing where condition for the matrix.</returns>
    private string GetWhereCondition()
    {
        string where = null;
        if (RoleID > 0)
        {
            where = string.Format("RoleID = {0}", RoleID);
        }

        if (UserRolesOnly && (SelectedUserID > 0))
        {
            // Get selected site name
            string siteName;
            if (SiteID > 0)
            {
                siteName = SelectedSite.SiteName.ToLowerCSafe();
            }
            else
            {
                siteName = UserInfo.GLOBAL_ROLES_KEY;
            }

            string rolesWhere = SelectedUser.GetRoleIdList((SiteID <= 0), true, siteName);

            // Add roles where condition
            if (!String.IsNullOrEmpty(rolesWhere))
            {
                where = SqlHelper.AddWhereCondition(where, "RoleID IN(" + rolesWhere + ")");
            }
            else
            {
                where = SqlHelper.NO_DATA_WHERE;
                gridMatrix.StopProcessing = true;
            }
        }

        return where;
    }


    /// <summary>
    /// Gets query parameters for the permission matrix.
    /// </summary>
    /// <returns>Two dimensional object array of query parameters.</returns>
    private QueryDataParameters GetQueryParameters()
    {
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ID", ValidationHelper.GetInteger(SelectedID, 0));
        parameters.Add("@SiteID", (GlobalRoles ? 0 : SiteID == 0 ? SiteContext.CurrentSiteID : SiteID));
        parameters.Add("@DisplayInMatrix", true);

        return parameters;
    }


    /// <summary>
    /// Gets name of the query which will be used for the matrix according to the selected type.
    /// </summary>
    /// <returns>String representing query name.</returns>
    private string GetQueryNameByType()
    {
        switch (SelectedType)
        {
            case "r":
                return "cms.permission.getResourcePermissionMatrix";

            default:
                return "cms.permission.getClassPermissionMatrix";
        }
    }


    /// <summary>
    /// Gets user effective permissions HTML content.
    /// </summary>
    /// <param name="siteId">Site ID</param>
    /// <param name="resourceId">ID of particular resource</param>
    /// <param name="selectedType">Permission type</param>
    private void GenerateBeforeRowsContent(int siteId, int resourceId, string selectedType)
    {
        // Check if selected users exists
        UserInfo user = SelectedUser;
        if (user == null)
        {
            gridMatrix.ShowContentBeforeRow = false;
            return;
        }

        string columns = "PermissionID";

        // Ensure tooltip column
        if (!String.IsNullOrEmpty(gridMatrix.ItemTooltipColumn))
        {
            columns += ",Matrix." + gridMatrix.ItemTooltipColumn;
        }

        // Get permission data
        DataSet dsPermissions;
        switch (selectedType)
        {
            case "r":
                dsPermissions = UserInfoProvider.GetUserResourcePermissions(user, siteId, resourceId, true, columns);
                break;

            default:
                dsPermissions = UserInfoProvider.GetUserDataClassPermissions(user, siteId, resourceId, true, columns);
                break;
        }

        if (!DataHelper.DataSourceIsEmpty(dsPermissions))
        {
            // Initialize variables used during rendering
            DataRowCollection rows = dsPermissions.Tables[0].Rows;
            string userName = Functions.GetFormattedUserName(user.UserName, user.FullName);
            var title = GetString("Administration-Permissions_Matrix.NotAdjustable");

            // Table header cell with user name
            var tcHeader = new TableCell
            {
                CssClass = "matrix-header",
                Text = HTMLHelper.HTMLEncode(TextHelper.LimitLength(userName, 50)),
                ToolTip = HTMLHelper.HTMLEncode(userName)
            };

            var tr = gridMatrix.ContentBeforeRow;
            
            tr.Cells.Add(tcHeader);

            // Process permissions according to matrix order
            foreach (int index in gridMatrix.ColumnOrderIndex)
            {
                DataRow dr = rows[index];

                // Table cell for each permission
                TableCell tc = new TableCell();
                var chk = new CMSCheckBox
                {
                    ID="chk_perm_" + index,
                    ClientIDMode = ClientIDMode.Static,
                    Enabled = false,
                    Checked = (SelectedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || Convert.ToInt32(dr["Allowed"]) == 1),
                    ToolTip = title,
                };

                tc.Controls.Add(chk);
                tr.Cells.Add(tc);
            }

            UniMatrix.AddFillingCell<TableCell>(tr);
        }
    }

    #endregion


    #region "Event Handlers"

    protected void gridMatrix_OnItemChanged(object sender, int rowItemId, int colItemId, bool allow)
    {
        // roleId and permissionId positions differ according to the page where control is used
        int roleId = (RoleID > 0) ? colItemId : rowItemId;
        int permissionId = (RoleID > 0) ? rowItemId : colItemId;

        // Check "Manage" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Permissions", "Manage"))
        {
            CMSPage.RedirectToAccessDenied("CMS.Permissions", "Manage");
        }

        // Check permission for the given column
        if (!gridMatrix_CheckPermissions(permissionId))
        {
            CMSPage.RedirectToAccessDenied("CMS.Permissions", "Manage");
        }

        if (allow)
        {
            RolePermissionInfo.Provider.Add(roleId, permissionId);
        }
        else
        {
            RolePermissionInfo.Provider.Remove(roleId, permissionId);
        }

        // Reload content before rows
        GenerateBeforeRowsContent(SiteID, ValidationHelper.GetInteger(SelectedID, 0), SelectedType);
    }


    protected void gridMatrix_DataLoaded(DataSet ds)
    {
        if (OnDataLoaded != null)
        {
            OnDataLoaded(ds);
        }
    }


    protected string gridMatrix_OnGetRowItemCssClass(object sender, DataRow dr)
    {
        string roleName = ValidationHelper.GetString(dr["RoleName"], String.Empty);

        // Check if all necessary data are available
        if (!String.IsNullOrEmpty(roleName) && (SelectedUser != null))
        {
            string siteName = string.Empty;

            // Get site name if not global
            if (SelectedSite != null)
            {
                siteName = SelectedSite.SiteName;
            }

            // Check if user is in specified role
            if (SelectedUser.IsInRole(roleName, siteName, (SiteID <= 0), true))
            {
                return HIGHLIGHTED_ROW_CSS;
            }
        }
        return String.Empty;
    }

    #endregion
}