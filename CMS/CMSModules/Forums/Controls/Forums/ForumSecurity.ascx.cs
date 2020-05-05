using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Helpers;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Forums_ForumSecurity : CMSAdminEditControl, IPostBackEventHandler
{
    #region "Variables"

    private int mForumId;
    private bool mIsGroupForum;
    private bool process = true;
    private bool createMatrix;
    private bool mEnable = true;

    private string[] allowedPermissions = new string[6] { "accesstoforum", "attachfiles", "markasanswer", "post", "reply", "subscribe" };

    protected ForumInfo forum = null;
    protected ResourceInfo resForums = null;

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
    /// Gets or sets the ID of the forum to edit.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumId;
        }
        set
        {
            mForumId = value;
        }
    }


    /// <summary>
    /// Indicates whether the forum security is displayed as a part of group module.
    /// </summary>
    public bool IsGroupForum
    {
        get
        {
            return mIsGroupForum;
        }
        set
        {
            mIsGroupForum = value;
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


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        if (ForumID > 0)
        {
            chkChangeName.Checked = forum.ForumAllowChangeName;

            gridMatrix.StopProcessing = true;

            if (!IsLiveSite && process)
            {
                gridMatrix.StopProcessing = false;
                // Render permission matrix
                CreateMatrix();
            }
            else if (createMatrix)
            {
                gridMatrix.StopProcessing = false;
                CreateMatrix();
                createMatrix = false;
            }
            else if (IsLiveSite && process && RequestHelper.IsPostBack())
            {
                gridMatrix.StopProcessing = false;
                CreateMatrix();
                createMatrix = false;
            }
        }

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        process = true;
        if (!Visible || StopProcessing)
        {
            EnableViewState = false;
            process = false;
        }

        chkChangeName.CheckedChanged += chkChangeName_CheckedChanged;

        if (ForumID > 0)
        {
            // Get information on current forum
            forum = ForumInfoProvider.GetForumInfo(ForumID);

            // Check whether the forum still exists
            EditedObject = forum;
        }

        // Get forum resource
        resForums = ResourceInfoProvider.GetResourceInfo("CMS.Forums");

        if ((resForums != null) && (forum != null))
        {
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@ID", resForums.ResourceID);
            parameters.Add("@ForumID", forum.ForumID);
            parameters.Add("@SiteID", SiteContext.CurrentSiteID);

            string where = string.Empty;
            int groupId = 0;
            if (IsGroupForum)
            {
                ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(forum.ForumGroupID);
                groupId = fgi.GroupGroupID;
            }

            // Build where condition
            if (groupId > 0)
            {
                where = "RoleGroupID=" + groupId + " AND PermissionDisplayInMatrix = 0";
            }
            else
            {
                where = "RoleGroupID IS NULL AND PermissionDisplayInMatrix = 0";
            }

            // Setup matrix control    
            gridMatrix.IsLiveSite = IsLiveSite;
            gridMatrix.QueryParameters = parameters;
            gridMatrix.WhereCondition = where;
            gridMatrix.CssClass = "permission-matrix";
            gridMatrix.OnItemChanged += gridMatrix_OnItemChanged;
            
            // Disable permission matrix if user has no Modify rights
            if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
            {
                Enable = false;
                gridMatrix.Enabled = false;
                ShowError(String.Format(GetString("general.accessdeniedonpermissionname"), PERMISSION_MODIFY));
            }
        }
    }

    #endregion


    /// <summary>
    /// Change name checkbox handler.
    /// </summary>
    protected void chkChangeName_CheckedChanged(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if (forum != null)
        {
            forum.ForumAllowChangeName = chkChangeName.Checked;
            ForumInfoProvider.SetForumInfo(forum);
        }
    }


    /// <summary>
    /// Generates the permission matrix for the current forum.
    /// </summary>
    private void CreateMatrix()
    {
        // Get forum resource info     
        if (resForums == null)
        {
            resForums = ResourceInfoProvider.GetResourceInfo("CMS.Forums");
        }

        // Get forum object
        if ((forum == null) && (ForumID > 0))
        {
            forum = ForumInfoProvider.GetForumInfo(ForumID);
        }

        if ((resForums != null) && (forum != null))
        {
            // Get permissions for the current forum resource                       
            DataSet permissions = PermissionNameInfoProvider.GetResourcePermissions(resForums.ResourceID);
            if (DataHelper.DataSourceIsEmpty(permissions))
            {
                ShowInformation(GetString("general.emptymatrix"));
            }
            else
            {
                TableHeaderRow headerRow = new TableHeaderRow();
                headerRow.CssClass = "unigrid-head";
                headerRow.TableSection = TableRowSection.TableHeader;
                TableCell newCell = new TableCell();
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

                // Render forum access permissions
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
                for (int access = 0; access <= accessNames.GetUpperBound(0); access++)
                {
                    SecurityAccessEnum currentAccess = ((SecurityAccessEnum)accessNames[access, 1]);

                    // If the security isn't displayed as part of group section
                    if ((currentAccess == SecurityAccessEnum.GroupMembers) && (!IsGroupForum))
                    {
                        // Do not render this access item
                    }
                    else
                    {
                        // Generate cell holding access item name
                        newRow = new TableRow();
                        newCell = new TableCell();
                        newCell.Text = accessNames[access, 0].ToString();
                        newCell.CssClass = "matrix-header";
                        newRow.Cells.Add(newCell);

                        // Render the permissions access items
                        bool isAllowed = false;
                        bool isEnabled = true;
                        int permissionIndex = 0;
                        for (int permission = 0; permission < (tblMatrix.Rows[0].Cells.Count - 1); permission++)
                        {
                            newCell = new TableCell();

                            // Check if the currently processed access is applied for permission
                            isAllowed = CheckPermissionAccess(currentAccess, permission, tblMatrix.Rows[0].Cells[permission + 1].Text);
                            isEnabled = ((currentAccess != SecurityAccessEnum.AllUsers) || (permission != 1)) && Enable;

                            // Disable column in roles grid if needed
                            if ((currentAccess == SecurityAccessEnum.AuthorizedRoles) && !isAllowed)
                            {
                                gridMatrix.DisableColumn(permissionIndex);
                            }

                            // Insert the radio button for the current permission
                            var radio = new CMSRadioButton
                            {
                                Checked = isAllowed,
                                Enabled = isEnabled,
                            };
                            radio.Attributes.Add("onclick", ControlsHelper.GetPostBackEventReference(this, permission + ";" + Convert.ToInt32(currentAccess)));
                            newCell.Controls.Add(radio);

                            newRow.Cells.Add(newCell);
                            permissionIndex++;
                        }

                        // Add the access row to the table
                        tblMatrix.Rows.Add(newRow);
                    }
                }

                // Check if forum has some roles assigned           
                headTitle.Visible = gridMatrix.HasData;
            }
        }
    }


    /// <summary>
    /// On item changed event.
    /// </summary>    
    private void gridMatrix_OnItemChanged(object sender, int roleId, int permissionId, bool allow)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if (allow)
        {
            ForumRoleInfoProvider.AddRoleToForum(roleId, ForumID, permissionId);
        }
        else
        {
            ForumRoleInfoProvider.RemoveRoleFromForum(roleId, ForumID, permissionId);
        }
    }


    /// <summary>
    /// Indicates the permission acess.
    /// </summary>
    /// <param name="currentAccess">Currently processed integer representation of item from SecurityAccessEnum</param>    
    /// <param name="currentPermission">Currently processed integer representation of permission to check</param>    
    private bool CheckPermissionAccess(SecurityAccessEnum currentAccess, int currentPermission, string currentPermissionName)
    {
        bool result = false;

        if (forum != null)
        {
            switch (currentPermission)
            {
                case 0:
                    // Process 'AccessToForum' permission and check by current access
                    result = (forum.AllowAccess == currentAccess);
                    break;

                case 1:
                    // Process 'AttachFiles' permission and check by current access
                    result = (forum.AllowAttachFiles == currentAccess);
                    break;

                case 3:
                    // Process 'Post' permission and check by current access
                    result = (forum.AllowPost == currentAccess);
                    break;

                case 2:
                    // Process 'MarkAsAnswer' permission and check by current access
                    result = (forum.AllowMarkAsAnswer == currentAccess);
                    break;

                case 4:
                    // Process 'Reply' permission and check by current access
                    result = (forum.AllowReply == currentAccess);
                    break;

                case 5:
                    // Process 'Subscribe' permission and check by current access
                    result = (forum.AllowSubscribe == currentAccess);
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
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        string[] args = eventArgument.Split(';');
        if (args.Length == 2)
        {
            // Get info on currently selected item
            int permission = Convert.ToInt32(args[0]);
            int access = Convert.ToInt32(args[1]);

            if (forum != null)
            {
                // Update forum permission access information
                switch (permission)
                {
                    case 0:
                        // Set 'AllowAccess' permission to specified access
                        forum.AllowAccess = (SecurityAccessEnum)access;
                        break;

                    case 1:
                        // Set 'AttachFiles' permission to specified access
                        forum.AllowAttachFiles = ((SecurityAccessEnum)access);
                        break;

                    case 2:
                        // Set 'MarkAsAnswer' permission to specified access
                        forum.AllowMarkAsAnswer = (SecurityAccessEnum)access;
                        break;

                    case 3:
                        // Set 'Post' permission to specified access
                        forum.AllowPost = ((SecurityAccessEnum)access);
                        break;

                    case 4:
                        // Set 'Reply' permission to specified access
                        forum.AllowReply = (SecurityAccessEnum)access;
                        break;

                    case 5:
                        // Set 'Subscribe' permission to specified access
                        forum.AllowSubscribe = (SecurityAccessEnum)access;
                        break;
                }

                // Save changes to the forum
                ForumInfoProvider.SetForumInfo(forum);

                createMatrix = true;
            }
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        createMatrix = true;

        // Ensure viewstate
        EnableViewState = true;
    }

    #endregion
}