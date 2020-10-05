using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_Roles_Roles : CMSAdminEditControl
{
    #region "Properties"

    /// <summary>
    /// Gets and sets current site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            int siteId = ValidationHelper.GetInteger(ViewState["siteid"], 0);

            if (siteId <= 0)
            {
                siteId = ValidationHelper.GetInteger(GetValue("SiteID"), 0);
            }

            return siteId;
        }
        set
        {
            ViewState["siteid"] = value;
        }
    }


    /// <summary>
    /// Gets and sets current role ID.
    /// </summary>
    public int RoleID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["roleid"], 0);
        }
        set
        {
            ViewState["roleid"] = value;
        }
    }


    /// <summary>
    /// Gets and sets control to be displayed.
    /// </summary>
    public string SelectedControl
    {
        get
        {
            return ValidationHelper.GetString(ViewState["selectedcontrol"], "general");
        }
        set
        {
            ViewState["selectedcontrol"] = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        #region "Security"

        RoleList.OnCheckPermissions += new CheckPermissionsEventHandler(RoleList_OnCheckPermissions);
        RoleEdit.OnCheckPermissions += new CheckPermissionsEventHandler(RoleEdit_OnCheckPermissions);
        Role.OnCheckPermissions += new CheckPermissionsEventHandler(Role_OnCheckPermissions);

        #endregion


        if (!Visible)
        {
            EnableViewState = false;
        }

        if (StopProcessing)
        {
            Role.StopProcessing = true;
            RoleList.StopProcessing = true;
            RoleEdit.StopProcessing = true;
        }
        else
        {
            // Is live site
            Role.IsLiveSite = IsLiveSite;

            RoleList.SiteID = SiteID;

            RoleEdit.SiteID = SiteID;
            RoleEdit.DisplayMode = DisplayMode;

            Role.SiteID = SiteID;
            Role.DisplayMode = DisplayMode;

            // Setup new role button            
            btnNewRole.Click += new EventHandler(btnNewRole_Click);
            imgNewRole.ImageUrl = GetImageUrl("Objects/CMS_Role/add.png");
            imgNewRole.AlternateText = GetString("Administration-Role_New.NewRole");

            // BreadCrumbs setup            
            lnkBackHidden.Click += new EventHandler(lnkBackHidden_Click);

            RoleList.OnEdit += new EventHandler(RoleList_OnEdit);
            RoleEdit.OnSaved += new EventHandler(RoleEdit_OnSaved);

            DisplayControls(SelectedControl);
        }
    }


    #region "Security handlers"

    private void Role_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void RoleEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void RoleList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    /// <summary>
    /// Displays appropriate controls.
    /// </summary>
    public void DisplayControls(string control)
    {
        // Hide all controls first and set all IDs
        Role.Visible = false;
        RoleList.Visible = false;
        RoleEdit.Visible = false;
        headerLinks.Visible = false;
        pnlRolesBreadcrumbs.Visible = false;

        // Display edit control
        if (RoleID > 0)
        {
            Role.Visible = true;
            pnlRolesBreadcrumbs.Visible = true;
            InitializeBreadcrumbs();

            Role.SiteID = SiteID;
            Role.ItemID = RoleID;
            Role.ReloadData(false);
        }
        else
        {
            switch (control)
            {
                    // Display list control
                case "general":
                default:
                    RoleList.Visible = true;
                    headerLinks.Visible = true;
                    RoleList.SiteID = SiteID;
                    RoleEdit.SiteID = SiteID;
                    RoleEdit.ReloadData(false);
                    break;
                    // Display new control
                case "newrole":
                    RoleEdit.Visible = true;
                    pnlRolesBreadcrumbs.Visible = true;
                    InitializeBreadcrumbs();
                    break;
            }
        }
    }


    /// <summary>
    /// Edit action delegate handler.
    /// </summary>
    private void RoleList_OnEdit(object sender, EventArgs e)
    {
        RoleID = Role.ItemID = RoleList.SelectedItemID;
        Role.ReloadData(true);
        DisplayControls("");
    }


    /// <summary>
    /// New role click handler.
    /// </summary>
    private void btnNewRole_Click(object sender, EventArgs e)
    {
        RoleID = RoleEdit.ItemID = Role.ItemID = 0;
        RoleEdit.ReloadData(true);
        SelectedControl = "newrole";
        DisplayControls(SelectedControl);
    }


    /// <summary>
    /// Breadcrumbs click handler.
    /// </summary>
    private void lnkBackHidden_Click(object sender, EventArgs e)
    {
        RoleID = 0;
        SelectedControl = "general";
        DisplayControls(SelectedControl);
        Role.SelectedTab = 0;
    }


    /// <summary>
    /// OnSave event handler.
    /// </summary>
    private void RoleEdit_OnSaved(object sender, EventArgs e)
    {
        Role.ItemID = RoleID = RoleEdit.ItemID;
        Role.ReloadData(true);
        SelectedControl = "general";
        DisplayControls(SelectedControl);
    }


    /// <summary>
    /// Initializes breadcrumbs items.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("general.roles"),
            Index = 0,
            RedirectUrl = "javascript:" + ControlsHelper.GetPostBackEventReference(lnkBackHidden)
        });

        RoleInfo role = RoleInfo.Provider.Get(RoleID);

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = (role == null) ? GetString("Administration-Role_New.NewRole") : role.RoleDisplayName,
            Index = 1
        });
    }
}