using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_Boards_BoardSecurity : CMSAdminEditControl
{
    #region "Variables"

    private int mBoardId;
    private BoardInfo mBoard;

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
    /// ID of the current message board.
    /// </summary>
    public int BoardID
    {
        get
        {
            if (mBoard != null)
            {
                return mBoard.BoardID;
            }

            return mBoardId;
        }
        set
        {
            mBoardId = value;

            mBoard = null;
        }
    }


    /// <summary>
    /// Current message board info object.
    /// </summary>
    public BoardInfo Board
    {
        get
        {
            return mBoard ?? (mBoard = BoardInfoProvider.GetBoardInfo(BoardID));
        }
        set
        {
            mBoard = value;

            mBoardId = 0;
        }
    }


    /// <summary>
    /// ID of the current group.
    /// </summary>
    public int GroupID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Initializes the controls
        SetupControls();

        if (!RequestHelper.IsPostBack() && !IsLiveSite)
        {
            ReloadData();
        }
        else
        {
            addRoles.CurrentValues = GetRoles();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Check permissions and disable controls
        if (CheckMessageBoardsPermissions())
        {
            return;
        }

        if (CheckGroupPermissions())
        {
            return;
        }

        btnRemoveRole.Enabled = false;
        addRoles.CurrentSelector.Enabled = false;
        lstRoles.Enabled = false;
    }


    protected void radOnlyOwner_CheckedChanged(object sender, EventArgs e)
    {
        if (radOnlyOwner.Checked)
        {
            // Disable controls
            addRoles.CurrentSelector.Enabled = false;
            btnRemoveRole.Enabled = false;
            lstRoles.Enabled = false;
        }
    }


    protected void radGroupMembers_CheckedChanged(object sender, EventArgs e)
    {
        if (radGroupMembers.Checked)
        {
            // Disable controls
            addRoles.CurrentSelector.Enabled = false;
            btnRemoveRole.Enabled = false;
            lstRoles.Enabled = false;
        }
    }


    protected void radOnlyGroupAdmin_CheckedChanged(object sender, EventArgs e)
    {
        if (radOnlyGroupAdmin.Checked)
        {
            // Disable controls
            addRoles.CurrentSelector.Enabled = false;
            btnRemoveRole.Enabled = false;
            lstRoles.Enabled = false;
        }
    }


    protected void radAllUsers_CheckedChanged(object sender, EventArgs e)
    {
        if (radAllUsers.Checked)
        {
            // Disable controls
            addRoles.CurrentSelector.Enabled = false;
            btnRemoveRole.Enabled = false;
            lstRoles.Enabled = false;
        }
    }


    protected void radOnlyUsers_CheckedChanged(object sender, EventArgs e)
    {
        if (radOnlyUsers.Checked)
        {
            // Disable controls
            btnRemoveRole.Enabled = false;
            addRoles.CurrentSelector.Enabled = false;
            lstRoles.Enabled = false;
        }
    }


    protected void radOnlyRoles_CheckedChanged(object sender, EventArgs e)
    {
        if (radOnlyRoles.Checked)
        {
            // Enable controls
            addRoles.CurrentSelector.Enabled = true;
            btnRemoveRole.Enabled = true;
            lstRoles.Enabled = true;
        }
    }


    protected void btnRemoveRole_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
        {
            return;
        }

        foreach (ListItem item in lstRoles.GetSelectedItems())
        {
            // Delete selected item
            int roleId = Convert.ToInt32(item.Value);
            BoardRoleInfoProvider.RemoveRoleFromBoard(roleId, BoardID);
        }

        ReloadRoles();
    }


    protected void addRoles_Changed(object sender, EventArgs e)
    {
        ReloadRoles();
        pnlUpdate.Update();
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Check permissions
        if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
        {
            return;
        }

        // Check if board exists
        if (Board != null)
        {
            if (radOnlyUsers.Checked)
            {
                Board.BoardAccess = SecurityAccessEnum.AuthenticatedUsers;
            }

            if (radAllUsers.Checked)
            {
                Board.BoardAccess = SecurityAccessEnum.AllUsers;
            }

            if (radOnlyRoles.Checked)
            {
                Board.BoardAccess = SecurityAccessEnum.AuthorizedRoles;
            }

            if (radOnlyGroupAdmin.Visible && radOnlyGroupAdmin.Checked)
            {
                Board.BoardAccess = SecurityAccessEnum.GroupAdmin;
            }

            if (radGroupMembers.Visible && radGroupMembers.Checked)
            {
                Board.BoardAccess = SecurityAccessEnum.GroupMembers;
            }

            if (radOnlyOwner.Visible && radOnlyOwner.Checked)
            {
                Board.BoardAccess = SecurityAccessEnum.Owner;
            }

            Board.BoardUseCaptcha = chkUseCaptcha.Checked;

            // Save changes
            BoardInfoProvider.SetBoardInfo(Board);

            ShowChangesSaved();
        }
    }

    #endregion


    #region "General methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControls()
    {
        radAllUsers.Text = GetString("security.allusers");
        radOnlyRoles.Text = GetString("board.security.onlyroles");
        radOnlyUsers.Text = GetString("board.security.onlyusers");
        radGroupMembers.Text = GetString("board.security.onlygroupmembers");
        radOnlyGroupAdmin.Text = GetString("board.security.groupadmin");
        radOnlyOwner.Text = GetString("board.security.owner");
        btnRemoveRole.Text = GetString("general.remove");

        addRoles.BoardID = BoardID;
        addRoles.CurrentSelector.IsLiveSite = IsLiveSite;
        addRoles.Changed += addRoles_Changed;
        addRoles.GroupID = GroupID;
        addRoles.IsLiveSite = IsLiveSite;
        addRoles.ShowSiteFilter = false;
    }


    /// <summary>
    /// Reloads the listbox with roles.
    /// </summary>
    private void ReloadRoles()
    {
        // Load the roles into the ListBox
        var roles = GetBoardRoles("RoleID", "RoleDisplayName", "SiteID");

        lstRoles.Items.Clear();

        foreach (var role in roles)
        {
            string name = role.RoleDisplayName;
            if (role.SiteID == 0)
            {
                name += " " + GetString("general.global");
            }
            lstRoles.Items.Add(new ListItem(name, role.RoleID.ToString()));
        }

        addRoles.CurrentSelector.Value = TextHelper.Join(";", DataHelper.GetStringValues(roles.Tables[0], "RoleID"));
    }


    private ObjectQuery<RoleInfo> GetBoardRoles(params string[] columns)
    {
        var roles = RoleInfo.Provider.Get()
                                    .Columns(columns)
                                    .WhereIn("RoleID", BoardRoleInfoProvider.GetBoardRoles()
                                                                            .Column("RoleID")
                                                                            .WhereEquals("BoardID", BoardID));
        return roles;
    }


    /// <summary>
    /// Returns board roles separated by semicolon.
    /// </summary>    
    private string GetRoles()
    {
        // Load the roles into the ListBox
        var roles = GetBoardRoles("RoleID").GetListResult<int>();

        return String.Join(";", roles);
    }


    private bool CheckMessageBoardsPermissions()
    {
        return GroupID == 0 && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.messageboards", PERMISSION_MODIFY);
    }


    private bool CheckGroupPermissions()
    {
        return GroupID > 0 && CMSDeskPage.CheckGroupPermissions(GroupID, PERMISSION_MANAGE, false);
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        ReloadRoles();

        EditedObject = Board;

        if (Board != null)
        {
            plcOnlyGroupAdmin.Visible = (GroupID > 0);
            plcGroupMembers.Visible = (GroupID > 0);
            plcOnlyOwner.Visible = (Board.BoardUserID > 0);

            switch (Board.BoardAccess)
            {
                // All users
                case SecurityAccessEnum.AllUsers:
                    radAllUsers.Checked = true;
                    break;

                // Authenticated users
                case SecurityAccessEnum.AuthenticatedUsers:
                    radOnlyUsers.Checked = true;
                    break;

                //Selected roles
                case SecurityAccessEnum.AuthorizedRoles:
                    radOnlyRoles.Checked = true;
                    radOnlyRoles_CheckedChanged(null, null);
                    break;

                // Group members
                case SecurityAccessEnum.GroupMembers:
                    radGroupMembers.Checked = true;
                    radGroupMembers_CheckedChanged(null, null);
                    break;

                case SecurityAccessEnum.GroupAdmin:
                    radOnlyGroupAdmin.Checked = true;
                    radOnlyGroupAdmin_CheckedChanged(null, null);
                    break;

                case SecurityAccessEnum.Owner:
                    radOnlyOwner.Checked = true;
                    radOnlyOwner_CheckedChanged(null, null);
                    break;

                default:
                    radAllUsers.Checked = true;
                    break;
            }

            lstRoles.Enabled = radOnlyRoles.Checked;
            btnRemoveRole.Enabled = radOnlyRoles.Checked;
            addRoles.CurrentSelector.Enabled = radOnlyRoles.Checked;
            chkUseCaptcha.Checked = Board.BoardUseCaptcha;

            addRoles.ReloadData();
            addRoles.DataBind();
        }
    }

    #endregion
}
