using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Polls;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Polls_Controls_PollSecurity : CMSAdminEditControl
{
    #region "Private variables"

    private PollInfo poll = null;

    private int groupId = 0;

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
    /// Enables or disables the panel.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return pnlBody.Enabled;
        }
        set
        {
            pnlBody.Enabled = value;
            btnOk.Enabled = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        poll = PollInfoProvider.GetPollInfo(ItemID);
        if ((ItemID > 0) && !IsLiveSite)
        {
            EditedObject = poll;
        }

        if (poll != null)
        {
            groupId = poll.PollGroupID;
        }

        // Roles control settings
        addRoles.PollID = ItemID;
        addRoles.IsLiveSite = IsLiveSite;
        addRoles.Changed += addRoles_Changed;
        addRoles.GroupID = groupId;
        addRoles.ShowSiteFilter = false;

        if (!RequestHelper.IsPostBack() && (poll != null) && !IsLiveSite)
        {
            ReloadData();
        }
        else
        {
            if (radOnlyRoles.Checked)
            {
                addRoles.CurrentSelector.Enabled = true;
                int currentSiteID = 0;
                string roles = string.Empty;

                // Get current site ID using CMSContext
                if (SiteContext.CurrentSite != null)
                {
                    currentSiteID = SiteContext.CurrentSiteID;
                }

                DataSet ds = PollInfoProvider.GetPollRoles(ItemID, null, null, -1, "CMS_Role.SiteID, CMS_Role.RoleID");
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        // Include roles associated to current site only
                        if ((ValidationHelper.GetInteger(row["SiteID"], 0) == currentSiteID))
                        {
                            // Insert an item to the listbox
                            roles += ValidationHelper.GetString(row["RoleID"], null) + ";";
                        }
                    }
                }

                addRoles.CurrentValues = roles;
            }
            else
            {
                addRoles.CurrentSelector.Enabled = false;
            }
        }
    }


    /// <summary>
    /// Disable buttons when permissions are not sufficient.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!CheckModifyPermissions())
        {
            addRoles.CurrentSelector.Enabled = false;
            btnRemoveRole.Enabled = false;
            lstRoles.Enabled = false;
        }
    }

    #endregion


    #region "Event handler methods"

    /// <summary>
    /// Roles control changed event handling.
    /// </summary>
    protected void addRoles_Changed(object sender, EventArgs e)
    {
        ReloadRolesList();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Enables access for all users.
    /// </summary>
    protected void radAllUsers_CheckedChanged(object sender, EventArgs e)
    {
        if (radAllUsers.Checked)
        {
            DisableRoleSelector();
        }
    }


    /// <summary>
    /// Enables access for authenticated users only.
    /// </summary>
    protected void radOnlyUsers_CheckedChanged(object sender, EventArgs e)
    {
        if (radOnlyUsers.Checked)
        {
            DisableRoleSelector();
        }
    }


    /// <summary>
    /// Enables access for group members only.
    /// </summary>
    protected void radGroupMembers_CheckedChanged(object sender, EventArgs e)
    {
        if (radGroupMembers.Checked)
        {
            DisableRoleSelector();
        }
    }


    /// <summary>
    /// Disables list and buttons for role selection.
    /// </summary>
    protected void DisableRoleSelector()
    {
        addRoles.CurrentSelector.Enabled = false;
        btnRemoveRole.Enabled = false;
        lstRoles.Enabled = false;
    }


    /// <summary>
    /// Enables access for users in authorized roles only.
    /// </summary>
    protected void radOnlyRoles_CheckedChanged(object sender, EventArgs e)
    {
        if (radOnlyRoles.Checked)
        {
            // Reload role list
            ReloadRolesList();

            addRoles.CurrentSelector.Enabled = true;
            btnRemoveRole.Enabled = true;
            lstRoles.Enabled = true;
        }
    }


    /// <summary>
    /// Removes role from the list of authorized roles.
    /// </summary>
    protected void btnRemoveRole_Click(object sender, EventArgs e)
    {
        if (!CheckModifyPermissions())
        {
            return;
        }

        foreach (ListItem item in lstRoles.GetSelectedItems())
        {
            int roleId = ValidationHelper.GetInteger(item.Value, 0);
            PollInfoProvider.RemoveRoleFromPoll(roleId, ItemID);
        }
        // Reload listbox with roles
        ReloadRolesList();
    }


    /// <summary>
    /// Button OK click handler.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (!CheckModifyPermissions())
        {
            return;
        }

        if (poll != null)
        {
            if (radAllUsers.Checked)
            {
                poll.PollAccess = SecurityAccessEnum.AllUsers;
            }
            else if (radOnlyUsers.Checked)
            {
                poll.PollAccess = SecurityAccessEnum.AuthenticatedUsers;
            }
            else if (radGroupMembers.Checked)
            {
                poll.PollAccess = SecurityAccessEnum.GroupMembers;
            }
            else if (radOnlyRoles.Checked)
            {
                poll.PollAccess = SecurityAccessEnum.AuthorizedRoles;
            }
            PollInfoProvider.SetPollInfo(poll);

            ShowChangesSaved();
        }
        else
        {
            throw new Exception("Poll with given ID not found!");
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads list of roles authorized for poll access into lstRoles control.
    /// Lists roles associated to current site.
    /// </summary>
    private void ReloadRolesList()
    {
        lstRoles.Items.Clear();
        // Get allowed roles of the poll
        DataSet ds = PollInfoProvider.GetPollRoles(ItemID);

        string roles = string.Empty;

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Get current site ID using CMSContext
            int currentSiteID = 0;
            if (SiteContext.CurrentSite != null)
            {
                currentSiteID = SiteContext.CurrentSiteID;
            }

            DataRowCollection rows = ds.Tables[0].Rows;
            foreach (DataRow row in rows)
            {
                RoleInfo roleInfo = new RoleInfo(row);
                // Include roles associated to current site only
                if ((roleInfo.SiteID == currentSiteID) || (roleInfo.SiteID == 0))
                {
                    string roleID = roleInfo.RoleID.ToString();
                    string name = roleInfo.RoleDisplayName;

                    if (roleInfo.SiteID == 0)
                    {
                        name += " " + GetString("general.global");
                    }
                    // Insert an item to the listbox
                    lstRoles.Items.Add(new ListItem(name, roleID));
                    roles += roleID + ";";
                }
            }
        }

        btnRemoveRole.Enabled = lstRoles.Enabled;
        addRoles.CurrentSelector.Enabled = lstRoles.Enabled;
        addRoles.CurrentSelector.Value = roles;
    }


    /// <summary>
    /// Uncheck all radio buttons.
    /// </summary>
    private void UncheckAllRadio()
    {
        radAllUsers.Checked = false;
        radOnlyUsers.Checked = false;
        radGroupMembers.Checked = false;
        radOnlyRoles.Checked = false;
    }


    /// <summary>
    /// Clears data.
    /// </summary>
    public override void ClearForm()
    {
        base.ClearForm();
        radAllUsers.Checked = false;
        radOnlyUsers.Checked = false;
        radGroupMembers.Checked = false;
        radOnlyRoles.Checked = false;
        lstRoles.DataSource = null;
        addRoles.Value = null;
    }


    /// <summary>
    /// Reloads form data.
    /// </summary>
    public override void ReloadData()
    {
        ClearForm();

        if (poll == null)
        {
            // Get poll object and set group ID
            poll = PollInfoProvider.GetPollInfo(ItemID);
        }

        if (poll != null)
        {
            groupId = poll.PollGroupID;
            // Reload role list
            ReloadRolesList();

            switch (poll.PollAccess)
            {
                    // If access is enabled for all users
                case SecurityAccessEnum.AllUsers:
                    UncheckAllRadio();
                    radAllUsers.Checked = true;
                    DisableRoleSelector();
                    break;

                    // If access is enabled for authenticated users only
                case SecurityAccessEnum.AuthenticatedUsers:
                    UncheckAllRadio();
                    radOnlyUsers.Checked = true;
                    DisableRoleSelector();
                    break;

                    // If access is enabled for group members only
                case SecurityAccessEnum.GroupMembers:
                    UncheckAllRadio();
                    radGroupMembers.Checked = true;
                    DisableRoleSelector();
                    break;

                    // Access is enabled for users in authorized roles only
                case SecurityAccessEnum.AuthorizedRoles:
                    UncheckAllRadio();
                    radOnlyRoles.Checked = true;
                    btnRemoveRole.Enabled = true;
                    lstRoles.Enabled = true;
                    break;
            }
        }
        else
        {
            DisableRoleSelector();
        }
    }


    /// <summary>
    /// Check modify permission for poll.
    /// </summary>
    private bool CheckModifyPermissions()
    {
        if (poll == null)
        {
            return false;
        }

        return ((poll.PollSiteID > 0) && CheckPermissions("cms.polls", PERMISSION_MODIFY) ||
                (poll.PollSiteID <= 0) && CheckPermissions("cms.polls", PERMISSION_GLOBALMODIFY));
    }

    #endregion
}