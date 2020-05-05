using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Community;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Groups_Controls_Security_GroupSecurity : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private string[] allowedPermissions = new string[3] { "createpages", "deletepages", "editpages" };
    protected GroupInfo group = null;
    protected ResourceInfo resGroups = null;

    // HashTable holding information on all permissions that 'OnlyAuthorizedRoles' access is selected for
    private Hashtable onlyAuth = new Hashtable();

    #endregion


    #region "Public properties"

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
    /// Community group id.
    /// </summary>
    public int GroupID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupID"), 0);
        }
        set
        {
            SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return gridMatrix.Enabled;
        }
        set
        {
            gridMatrix.Enabled = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (GroupID > 0)
            {
                // Render permission matrix
                CreateMatrix();
            }
        }

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible)
        {
            EnableViewState = false;
        }

        // Get group resource info
        resGroups = ResourceInfoProvider.GetResourceInfo("CMS.Groups");
        if (resGroups != null)
        {
            // Retrieve permission matrix data
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@ID", resGroups.ResourceID);
            parameters.Add("@GroupID", GroupID);
            parameters.Add("@SiteID", SiteContext.CurrentSiteID);

            // Setup WHERE condition
            string where = "RoleGroupID=" + GroupID + "AND PermissionDisplayInMatrix = 0";

            // Setup grid control
            gridMatrix.QueryParameters = parameters;
            gridMatrix.WhereCondition = where;
            gridMatrix.CssClass = "permission-matrix";

            gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;

            // Disable permission matrix if user has no MANAGE rights
            if (!MembershipContext.AuthenticatedUser.IsGroupAdministrator(GroupID))
            {
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", PERMISSION_MANAGE))
                {
                    Enabled = false;
                    gridMatrix.Enabled = false;
                    ShowError(String.Format(ResHelper.GetString("general.accessdeniedonpermissionname"), "Manage"));
                }
            }
        }
    }


    private void gridMatrix_OnItemChanged(object sender, int roleID, int permissionID, bool allow)
    {
        if (!CheckPermissions("cms.groups", PERMISSION_MANAGE, GroupID))
        {
            return;
        }

        if (allow)
        {
            GroupRolePermissionInfoProvider.AddRoleToGroup(roleID, GroupID, permissionID);
        }
        else
        {
            GroupRolePermissionInfoProvider.RemoveRoleFromGroup(roleID, GroupID, permissionID);
        }
    }

    #endregion


    /// <summary>
    /// Generates the permission matrix for the current group.
    /// </summary>
    private void CreateMatrix()
    {
        // Get group resource info 
        if (resGroups == null)
        {
            resGroups = ResourceInfoProvider.GetResourceInfo("CMS.Groups");
        }

        if (resGroups != null)
        {
            group = GroupInfoProvider.GetGroupInfo(GroupID);

            // Get permissions for the current group resource                       
            DataSet permissions = PermissionNameInfoProvider.GetResourcePermissions(resGroups.ResourceID);
            if (DataHelper.DataSourceIsEmpty(permissions))
            {
                ShowInformation(GetString("general.emptymatrix"));
            }
            else
            {
                TableRow headerRow = new TableRow();
                headerRow.TableSection = TableRowSection.TableHeader;
                headerRow.CssClass = "unigrid-head";

                TableHeaderCell newHeaderCell = new TableHeaderCell();
                newHeaderCell.CssClass = "first-column";
                headerRow.Cells.Add(newHeaderCell);

                foreach (string permission in allowedPermissions)
                {
                    DataRow[] drArray = permissions.Tables[0].DefaultView.Table.Select("PermissionName = '" + permission + "'");
                    if (drArray.Length > 0)
                    {
                        DataRow dr = drArray[0];
                        newHeaderCell = new TableHeaderCell();
                        newHeaderCell.CssClass = "matrix-header";
                        newHeaderCell.Text = dr["PermissionDisplayName"].ToString();
                        newHeaderCell.ToolTip = dr["PermissionDescription"].ToString();

                        headerRow.Cells.Add(newHeaderCell);
                    }
                    else
                    {
                        throw new Exception("[Security matrix] Column '" + permission + "' cannot be found.");
                    }
                }
                tblMatrix.Rows.Add(headerRow);

                // Render group access permissions
                object[,] accessNames = new object[5, 2];
                accessNames[0, 0] = GetString("security.nobody");
                accessNames[0, 1] = SecurityAccessEnum.Nobody;
                accessNames[1, 0] = GetString("security.allusers");
                accessNames[1, 1] = SecurityAccessEnum.AllUsers;
                accessNames[2, 0] = GetString("security.authenticated");
                accessNames[2, 1] = SecurityAccessEnum.AuthenticatedUsers;
                accessNames[3, 0] = GetString("security.groupmembers");
                accessNames[3, 1] = SecurityAccessEnum.GroupMembers;
                accessNames[4, 0] = GetString("security.authorizedroles");
                accessNames[4, 1] = SecurityAccessEnum.AuthorizedRoles;

                TableRow newRow = null;

                TableCell newCell;
                for (int access = 0; access <= accessNames.GetUpperBound(0); access++)
                {
                    SecurityAccessEnum currentAccess = ((SecurityAccessEnum)accessNames[access, 1]);

                    // Generate cell holding access item name
                    newRow = new TableRow();
                    newCell = new TableCell();
                    newCell.CssClass = "matrix-header";
                    newCell.Text = accessNames[access, 0].ToString();
                    newRow.Cells.Add(newCell);

                    // Render the permissions access items
                    int permissionIndex = 0;
                    for (int permission = 0; permission < (tblMatrix.Rows[0].Cells.Count - 1); permission++)
                    {
                        newCell = new TableCell();
                        newCell.CssClass = "matrix-cell";

                        // Check if the currently processed access is applied for permission
                        bool isAllowed = CheckPermissionAccess(currentAccess, permission, tblMatrix.Rows[0].Cells[permission + 1].Text);

                        // Disable column in roles grid if needed
                        if ((currentAccess == SecurityAccessEnum.AuthorizedRoles) && !isAllowed)
                        {
                            gridMatrix.DisableColumn(permissionIndex);
                        }

                        // Insert the radio button for the current permission
                        var radio = new CMSRadioButton
                        {
                            Checked = isAllowed,
                            Enabled = Enabled,
                        };
                        radio.Attributes.Add("onclick", ControlsHelper.GetPostBackEventReference(this, permission + ";" + Convert.ToInt32(currentAccess)));
                        newCell.Controls.Add(radio);

                        newRow.Cells.Add(newCell);
                        permissionIndex++;
                    }

                    // Add the access row to the table
                    tblMatrix.Rows.Add(newRow);
                }

                // Hide if no roles available 
                headTitle.Visible = gridMatrix.HasData;
            }
        }
    }


    /// <summary>
    /// Indicates the permission access.
    /// </summary>
    /// <param name="currentAccess">Currently processed integer representation of item from SecurityAccessEnum</param>    
    /// <param name="currentPermission">Currently processed integer representation of permission to check</param>
    /// <param name="currentPermissionName">Currently processed permission name</param>
    private bool CheckPermissionAccess(SecurityAccessEnum currentAccess, int currentPermission, string currentPermissionName)
    {
        bool result = false;

        if (group != null)
        {
            switch (currentPermission)
            {
                case 0:
                    // Process 'AllowCreate' permission and check by current access
                    result = (group.AllowCreate == currentAccess);
                    break;

                case 1:
                    // Process 'AllowDelete' permission and check by current access
                    result = (group.AllowDelete == currentAccess);
                    break;

                case 2:
                    // Process 'AllowModify' permission and check by current access
                    result = (group.AllowModify == currentAccess);
                    break;
            }
        }

        // Make note about type of permission with access set to 'OnlyAuthorizedRoles'
        if (result && (currentAccess == SecurityAccessEnum.AuthorizedRoles))
        {
            onlyAuth[currentPermissionName] = true;
        }
        return result;
    }


    #region "PostBack event handler"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (!CheckPermissions("cms.groups", PERMISSION_MANAGE, GroupID))
        {
            return;
        }

        string[] args = eventArgument.Split(';');

        if (args.Length == 2)
        {
            // Get info on currently selected item
            int permission = Convert.ToInt32(args[0]);
            int access = Convert.ToInt32(args[1]);

            GroupInfo group = GroupInfoProvider.GetGroupInfo(GroupID);
            if (group != null)
            {
                // Update forum permission access information
                switch (permission)
                {
                    case 0:
                        // Set 'AllowCreate' permission to specified access
                        group.AllowCreate = (SecurityAccessEnum)access;
                        break;

                    case 1:
                        // Set 'AllowDelete' permission to specified access
                        group.AllowDelete = ((SecurityAccessEnum)access);
                        break;

                    case 2:
                        // Set 'AllowModify' permission to specified access
                        group.AllowModify = (SecurityAccessEnum)access;
                        break;
                }

                // Save changes to the forum
                GroupInfoProvider.SetGroupInfo(group);
            }
        }
    }

    #endregion
}