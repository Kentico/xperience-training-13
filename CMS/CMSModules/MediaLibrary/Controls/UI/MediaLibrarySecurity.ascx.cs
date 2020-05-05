using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_UI_MediaLibrarySecurity : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private int mMediaLibraryID;
    private bool mEnable = true;

    private MediaLibraryInfo mLibraryInfo;
    private ResourceInfo mResLibrary;

    // HashTable holding information on all permissions that 'OnlyAuthorizedRoles' access is selected for
    private Hashtable onlyAuth = new Hashtable();

    private ArrayList permissionArray = new ArrayList();

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current library info.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (MediaLibraryID > 0))
            {
                mLibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(MediaLibraryID);
            }
            return mLibraryInfo;
        }
    }


    /// <summary>
    /// Current library resource info.
    /// </summary>
    private ResourceInfo ResLibrary
    {
        get
        {
            if (mResLibrary == null)
            {
                mResLibrary = ResourceInfoProvider.GetResourceInfo("CMS.MediaLibrary");
            }
            return mResLibrary;
        }
    }

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
    /// Indicates if control is used on a live site
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
    /// Gets or sets the ID of the library to edit.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return mMediaLibraryID;
        }
        set
        {
            mMediaLibraryID = value;
            mLibraryInfo = null;
        }
    }


    /// <summary>
    /// Indicates whether permissions matrix is enabled.
    /// </summary>
    public bool Enable
    {
        get
        {
            return mEnable;
        }
        set
        {
            mEnable = value;
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        if (MediaLibraryID > 0)
        {
            // Render permission matrix
            CreateMatrix();
        }

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);

        if (!Visible)
        {
            EnableViewState = false;
        }

        if (MediaLibraryID > 0)
        {
            // Get information on current library            
            permissionArray.Add("filecreate");
            permissionArray.Add("foldercreate");
            permissionArray.Add("filedelete");
            permissionArray.Add("folderdelete");
            permissionArray.Add("filemodify");
            permissionArray.Add("foldermodify");
            permissionArray.Add("libraryaccess");

            if ((ResLibrary != null) && (LibraryInfo != null))
            {
                // Retrieve permission matrix data
                QueryDataParameters parameters = new QueryDataParameters();
                parameters.Add("@ID", ResLibrary.ResourceID);
                parameters.Add("@LibraryID", MediaLibraryID);
                parameters.Add("@SiteID", LibraryInfo.LibrarySiteID);

                // Exclude generic roles from matrix
                string where = "(RoleName NOT IN ('_authenticated_', '_everyone_', '_notauthenticated_')) AND ";

                if (LibraryInfo.LibraryGroupID > 0)
                {
                    where += "RoleGroupID=" + LibraryInfo.LibraryGroupID.ToString();
                }
                else
                {
                    where += "RoleGroupID IS NULL";
                }

                if (permissionArray != null)
                {
                    where += " AND PermissionName IN (";
                    foreach (string permission in permissionArray)
                    {
                        where += "'" + permission + "',";
                    }
                    where = where.TrimEnd(',');
                    where += ") ";
                }

                // Setup matrix control            
                gridMatrix.QueryParameters = parameters;
                gridMatrix.WhereCondition = where;
                gridMatrix.CssClass = "permission-matrix";
                gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;

                // Check 'Modify' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "manage"))
                {
                    Enable = false;
                    gridMatrix.Enabled = false;
                    ShowError(String.Format(GetString("general.accessdeniedonpermissionname"), "Manage"));
                }
            }
        }
    }


    /// <summary>
    /// Generates the permission matrix for the current library.
    /// </summary>
    private void CreateMatrix()
    {
        // Get library resource info             
        if ((ResLibrary != null) && (LibraryInfo != null))
        {
            // Get permissions for the current library resource                       
            DataSet permissions = PermissionNameInfoProvider.GetResourcePermissions(ResLibrary.ResourceID);
            if (DataHelper.DataSourceIsEmpty(permissions))
            {
                lblInfo.ResourceString = "general.emptymatrix";
                lblInfo.Visible = true;
            }
            else
            {
                TableRow headerRow = new TableRow();
                headerRow.TableSection = TableRowSection.TableHeader;
                headerRow.CssClass = "unigrid-head";

                TableHeaderCell newHeaderCell = new TableHeaderCell();
                newHeaderCell.CssClass = "first-column";
                headerRow.Cells.Add(newHeaderCell);

                DataView dv = permissions.Tables[0].DefaultView;
                dv.Sort = "PermissionDisplayName ASC";

                // Generate header cells                
                foreach (DataRowView drv in dv)
                {
                    string permissionName = drv.Row["PermissionName"].ToString();
                    if (permissionArray.Contains(permissionName.ToLowerCSafe()))
                    {
                        newHeaderCell = new TableHeaderCell();
                        newHeaderCell.CssClass = "matrix-header";
                        newHeaderCell.Text = HTMLHelper.HTMLEncode(drv.Row["PermissionDisplayName"].ToString());
                        newHeaderCell.ToolTip = Convert.ToString(drv.Row["PermissionDescription"]);

                        headerRow.Cells.Add(newHeaderCell);
                    }
                }

                tblMatrix.Rows.Add(headerRow);

                // Render library access permissions
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

                TableRow newRow;
                int rowIndex = 0;

                for (int access = 0; access <= accessNames.GetUpperBound(0); access++)
                {
                    SecurityAccessEnum currentAccess = ((SecurityAccessEnum)accessNames[access, 1]);
                    // If the security isn't displayed as part of group section
                    if (((currentAccess == SecurityAccessEnum.GroupAdmin) || (currentAccess == SecurityAccessEnum.GroupMembers)) && (!(LibraryInfo.LibraryGroupID > 0)))
                    {
                        // Do not render this access item
                    }
                    else
                    {
                        // Generate cell holding access item name
                        newRow = new TableRow();
                        TableCell newCell = new TableCell();
                        newCell.CssClass = "matrix-header";
                        newCell.Text = accessNames[access, 0].ToString();
                        newRow.Cells.Add(newCell);
                        rowIndex++;

                        // Render the permissions access items
                        int permissionIndex = 0;
                        for (int permission = 0; permission < (tblMatrix.Rows[0].Cells.Count - 1); permission++)
                        {
                            newCell = new TableCell();
                            int accessEnum = Convert.ToInt32(accessNames[access, 1]);
                            // Check if the currently processed access is applied for permission
                            bool isAllowed = CheckPermissionAccess(accessEnum, permission, tblMatrix.Rows[0].Cells[permission + 1].Text);

                            // Disable column in roles grid if needed
                            if ((currentAccess == SecurityAccessEnum.AuthorizedRoles) && !isAllowed)
                            {
                                gridMatrix.DisableColumn(permissionIndex);
                            }

                            // Insert the radio button for the current permission
                            var radio = new CMSRadioButton
                            {
                                Checked = isAllowed,
                                Enabled = Enable,
                            };
                            radio.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(this, permission + "|" + accessEnum));
                            newCell.Controls.Add(radio);

                            newRow.Cells.Add(newCell);
                            permissionIndex++;
                        }

                        // Add the access row to the table
                        tblMatrix.Rows.Add(newRow);
                    }
                }

                // Check if media library has some roles assigned            
                headTitle.Visible = gridMatrix.HasData;

            }
        }
    }


    /// <summary>
    /// Indicates the permission access.
    /// </summary>
    /// <param name="currentAccess">Currently processed integer representation of item from SecurityAccessEnum</param>    
    /// <param name="currentPermission">Currently processed integer representation of permission to check</param>    
    private bool CheckPermissionAccess(int currentAccess, int currentPermission, string currentPermissionName)
    {
        bool result = false;

        if (LibraryInfo != null)
        {
            switch (currentPermission)
            {
                case 6:
                    result = ((int)LibraryInfo.Access == currentAccess);
                    break;

                case 0:
                    result = ((int)LibraryInfo.FileCreate == currentAccess);
                    break;

                case 2:
                    result = ((int)LibraryInfo.FileDelete == currentAccess);
                    break;

                case 4:
                    result = ((int)LibraryInfo.FileModify == currentAccess);
                    break;

                case 1:
                    result = ((int)LibraryInfo.FolderCreate == currentAccess);
                    break;

                case 3:
                    result = ((int)LibraryInfo.FolderDelete == currentAccess);
                    break;

                case 5:
                    result = ((int)LibraryInfo.FolderModify == currentAccess);
                    break;

                default:
                    break;
            }
        }

        // Make note about type of permission with access set to 'OnlyAuthorizedRoles'
        if (result && (currentAccess == 2))
        {
            onlyAuth[currentPermissionName] = true;
        }
        return result;
    }


    /// <summary>
    /// On item changed event.
    /// </summary>    
    private void gridMatrix_OnItemChanged(object sender, int roleId, int permissionId, bool allow)
    {
        // Check 'Modify' permission
        if (MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "manage"))
        {
            if (allow)
            {
                MediaLibraryRolePermissionInfoProvider.AddRoleToLibrary(roleId, MediaLibraryID, permissionId);
            }
            else
            {
                MediaLibraryRolePermissionInfoProvider.RemoveRoleFromLibrary(roleId, MediaLibraryID, permissionId);
            }
        }
    }


    #region "PostBack event handler"

    public void RaisePostBackEvent(string eventArgument)
    {
        // Check 'Modify' permission
        if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "manage"))
        {
            return;
        }

        string[] args = eventArgument.Split('|');

        if (args.Length == 2)
        {
            // Get info on currently selected item
            int permission = Convert.ToInt32(args[0]);
            int access = Convert.ToInt32(args[1]);

            if (LibraryInfo != null)
            {
                // Update library permission access information
                switch (permission)
                {
                    case 0:
                        LibraryInfo.FileCreate = ((SecurityAccessEnum)access);
                        break;

                    case 2:
                        LibraryInfo.FileDelete = (SecurityAccessEnum)access;
                        break;

                    case 4:
                        LibraryInfo.FileModify = ((SecurityAccessEnum)access);
                        break;

                    case 1:
                        LibraryInfo.FolderCreate = (SecurityAccessEnum)access;
                        break;

                    case 3:
                        LibraryInfo.FolderDelete = (SecurityAccessEnum)access;
                        break;

                    case 5:
                        LibraryInfo.FolderModify = (SecurityAccessEnum)access;
                        break;

                    case 6:
                        LibraryInfo.Access = (SecurityAccessEnum)access;
                        break;

                    default:
                        break;
                }

                // Save changes to the library
                MediaLibraryInfoProvider.SetMediaLibraryInfo(LibraryInfo);
            }
        }
    }

    #endregion
}