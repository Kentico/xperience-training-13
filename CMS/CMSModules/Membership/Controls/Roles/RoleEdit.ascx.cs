using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_Roles_RoleEdit : CMSAdminEditControl
{
    #region "Private Variables"

    private int mSiteId;
    private int mRoleId;
    private bool mGlobalRole;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether the role is global (independent on site).
    /// </summary>
    public bool GlobalRole
    {
        get
        {
            return mGlobalRole;
        }
        set
        {
            mGlobalRole = value;
        }
    }


    /// <summary>
    /// Gets or sets the site ID for which the roles should be displayed.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets the role ID of currently processed role.
    /// </summary>
    public int RoleID
    {
        get
        {
            return mRoleId;
        }
        set
        {
            mRoleId = value;
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

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Do not edit code name in simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
        }

        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");

        txtRoleDisplayName.IsLiveSite = IsLiveSite;
        txtDescription.IsLiveSite = IsLiveSite;

        ReloadData(false);
    }


    /// <summary>
    /// Reloads textboxes with new data.
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        RoleInfo ri = ItemID > 0 ? RoleInfo.Provider.Get(ItemID) : new RoleInfo();

        // Set edited object
        EditedObject = ri;

        if (ri.RoleID > 0)
        {
            var user = MembershipContext.AuthenticatedUser;
            // Security test
            if (!user.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                if (ri.SiteID == 0)
                {
                    RedirectToAccessDenied(GetString("general.actiondenied"));
                }
                else
                {
                    SiteInfo si = SiteInfo.Provider.Get(ri.SiteID);
                    if (si != null)
                    {
                        if (!user.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && !MembershipContext.AuthenticatedUser.IsInSite(si.SiteName))
                        {
                            RedirectToAccessDenied(GetString("general.actiondenied"));
                        }
                    }
                }
            }
        }

        if ((!RequestHelper.IsPostBack()) || (forceReload))
        {
            string roleName = string.Empty;
            if (ri.RoleID > 0)
            {
                LoadData(ri);
                SiteID = ri.SiteID;
                roleName = ri.RoleName;
            }
            else
            {
                txtRoleCodeName.Text = null;
                txtRoleDisplayName.Text = null;
                txtDescription.Text = null;
                chkIsDomain.Checked = false;
            }
            bool displayIsDomain = (roleName != RoleName.EVERYONE) && (roleName != RoleName.AUTHENTICATED) && (roleName != RoleName.NOTAUTHENTICATED);
            plcIsDomain.Visible = displayIsDomain;
        }
    }


    /// <summary>
    /// Loads data of edited role from DB into TextBoxes.
    /// </summary>
    protected void LoadData(RoleInfo ri)
    {
        txtRoleCodeName.Text = ri.RoleName;
        txtRoleDisplayName.Text = ri.RoleDisplayName;
        txtDescription.Text = ri.RoleDescription;
        chkIsDomain.Checked = ri.RoleIsDomain;
    }


    /// <summary>
    /// Saves data of edited role from TextBoxes into DB.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.roles", PERMISSION_MODIFY))
        {
            return;
        }

        // Generate code name in simple mode
        string codeName = txtRoleCodeName.Text.Trim();
        string displayName = txtRoleDisplayName.Text.Trim();

        // Check whether required fields are not empty
        string errorMessage = new Validator().NotEmpty(displayName, GetString("general.requiresdisplayname"))
            .NotEmpty(codeName, GetString("general.requirescodename"))
            .IsCodeName(codeName, GetString("general.invalidcodename")).Result;
        if (errorMessage == string.Empty)
        {
            txtRoleCodeName.Text = codeName;
            txtRoleDisplayName.Text = displayName;
            if (GlobalRole && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                RoleInfo ri = RoleInfo.Provider.Get(codeName, 0);
                if ((ri == null) || (ri.RoleID == ItemID) || (codeName == InfoHelper.CODENAME_AUTOMATIC))
                {
                    SaveRole(ri, codeName, displayName);
                }
                else
                {
                    ShowError(GetString("Administration-Role_New.RoleExists"));
                }
            }
            else
            {
                SiteInfo si = SiteInfo.Provider.Get(SiteID);
                if (si != null)
                {
                    // Check unique name
                    RoleInfo ri = RoleInfo.Provider.Get(codeName, si.SiteID);
                    if ((ri == null) || (ri.RoleID == ItemID) || (codeName == InfoHelper.CODENAME_AUTOMATIC))
                    {
                        SaveRole(ri, codeName, displayName);
                    }
                    else
                    {
                        ShowError(GetString("Administration-Role_New.RoleExists"));
                    }
                }
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }


    private void SaveRole(RoleInfo ri, string codeName, string displayName)
    {
        bool newRole = false;
        // Get object
        if (ri == null)
        {
            ri = RoleInfo.Provider.Get(ItemID);
            if (ri == null)
            {
                ri = new RoleInfo();
                // indicate this is new role and should be redirected after safe
                newRole = true;
            }
        }

        if (ri.RoleDisplayName != displayName)
        {
            // Refresh a breadcrumb if used in the tabs layout
            ScriptHelper.RefreshTabHeader(Page, displayName);
        }

        // Set the fields
        ri.RoleDisplayName = displayName;
        ri.RoleName = codeName;
        ri.RoleID = ItemID;
        ri.RoleDescription = txtDescription.Text;
        ri.SiteID = mSiteId;
        ri.RoleIsDomain = chkIsDomain.Checked;

        RoleInfo.Provider.Set(ri);
        ItemID = ri.RoleID;

        ShowChangesSaved();

        // if new group was created redirect to edit page
        if (newRole)
        {
            RoleID = ri.RoleID;
            RaiseOnSaved();
        }
    }
}
