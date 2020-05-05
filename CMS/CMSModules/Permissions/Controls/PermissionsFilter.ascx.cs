using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CustomTables;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Permissions_Controls_PermissionsFilter : CMSAdminControl
{
    #region "Variables"

    private bool globalRecord = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if site selector contain sites.
    /// </summary>
    public bool HasSites
    {
        get
        {
            if (SiteID > 0)
            {
                return (true);
            }
            return siteSelector.DropDownSingleSelect.Items.Count > 0;
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
    /// If false site selector is hidden no matter what.
    /// </summary>
    public bool HideSiteSelector
    {
        get;
        set;
    }


    /// <summary>
    /// Value for (global) item record.
    /// </summary>
    public string GlobalRecordValue
    {
        get
        {
            return siteSelector.GlobalRecordValue;
        }
        set
        {
            siteSelector.GlobalRecordValue = value;
        }
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
    /// Gets ID selected in the moduleSelector or docTypeSelector or customTableSelector according to the selected PermissionType.
    /// </summary>
    public string SelectedID
    {
        get
        {
            if (drpPermissionType.SelectedIndex > -1)
            {
                if ((drpPermissionType.SelectedValue == PermissionTypes.Module.ToString()) && (moduleSelector.UniSelector.HasData))
                {
                    return ValidationHelper.GetString(moduleSelector.Value, "0");
                }
                else if ((drpPermissionType.SelectedValue == PermissionTypes.DocumentType.ToString()) && (docTypeSelector.UniSelector.HasData))
                {
                    return ValidationHelper.GetString(docTypeSelector.Value, "0");
                }
                else if ((drpPermissionType.SelectedValue == PermissionTypes.CustomTable.ToString()) && (customTableSelector.UniSelector.HasData))
                {
                    return ValidationHelper.GetString(customTableSelector.Value, "0");
                }
                else
                {
                    return "0";
                }
            }
            else
            {
                return "0";
            }
        }
    }


    /// <summary>
    /// Gets type constant according to the selected value in the moduleSelector or docTypeSelector or customTableSelector and the selected PermissionType.
    /// </summary>
    public string SelectedType
    {
        get
        {
            if (drpPermissionType.SelectedIndex > -1)
            {
                if ((drpPermissionType.SelectedValue == PermissionTypes.Module.ToString()) && (moduleSelector.UniSelector.HasData))
                {
                    return "r";
                }
                else if (((drpPermissionType.SelectedValue == PermissionTypes.DocumentType.ToString()) && (docTypeSelector.UniSelector.HasData)) || (customTableSelector.UniSelector.HasData))
                {
                    return "c";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }


    /// <summary>
    /// Gets selected site ID.
    /// </summary>
    public int SelectedSiteID
    {
        get
        {
            return ValidationHelper.GetInteger(siteSelector.Value, 0);
        }
    }


    /// <summary>
    /// Indicates if user selector will be displayed.
    /// </summary>
    public bool ShowUserSelector
    {
        get
        {
            return plcUser.Visible;
        }
        set
        {
            plcUser.Visible = value;
        }
    }


    /// <summary>
    /// Gets selected user ID.
    /// </summary>
    public int SelectedUserID
    {
        get
        {
            return ValidationHelper.GetInteger(userSelector.Value, 0);
        }
    }


    /// <summary>
    /// Gets or sets live site mode.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return siteSelector.IsLiveSite;
        }
        set
        {
            customTableSelector.IsLiveSite = value;
            moduleSelector.IsLiveSite = value;
            docTypeSelector.IsLiveSite = value;
            siteSelector.IsLiveSite = value;
            userSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if only user roles should be displayed.
    /// </summary>
    public bool UserRolesOnly
    {
        get
        {
            return chkUserOnly.Checked;
        }
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
    /// Indicates whether singledropdownlist uses autocomplete mode.
    /// </summary>
    public bool UseUniSelectorAutocomplete
    {
        get
        {
            return moduleSelector.UseUniSelectorAutocomplete;
        }
        set
        {
            moduleSelector.UseUniSelectorAutocomplete = value;
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


    #region "Enums"

    private enum PermissionTypes
    {
        DocumentType = 1,
        Module,
        CustomTable
    };

    #endregion


    #region "Page Evenets"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize site selector, if no site supplied        
        if (SiteID <= 0)
        {
            // Set site selector
            siteSelector.OnlyRunningSites = false;
            siteSelector.AllowAll = false;
            siteSelector.AllowEmpty = false;
            siteSelector.AllowGlobal = true;

            if (!RequestHelper.IsPostBack())
            {
                SiteID = 0;
                siteSelector.Value = ValidationHelper.GetInteger(siteSelector.GlobalRecordValue, 0);

                // Force siteselector to reload
                siteSelector.Reload(false);
            }
        }
        else
        {
            plcSite.Visible = false;
            siteSelector.Value = SiteID;
        }

        // Hide site selector used from role edit
        if (HideSiteSelector)
        {
            plcSite.Visible = false;
        }

        if (ValidationHelper.GetString(siteSelector.Value, String.Empty) == siteSelector.GlobalRecordValue)
        {
            globalRecord = true;
        }

        if (!RequestHelper.IsPostBack())
        {
            InitializeDropDownListPermissionType();
        }

        // Get truly selected value
        SiteID = ValidationHelper.GetInteger(siteSelector.Value, 0);

        // Inicialize user selector
        userSelector.SiteID = (SiteID > 0) ? SiteID : 0;
        userSelector.ShowSiteFilter = false;
        userSelector.DisplayUsersFromAllSites = (userSelector.SiteID <= 0);
        userSelector.UniSelector.OnSelectionChanged += userSelector_OnSelectionChanged;

        moduleSelector.DisplayOnlyForGivenSite = !globalRecord;
        InitializeDropDownListPermissionMatrix();

        siteSelector.UniSelector.OnSelectionChanged += siteSelector_OnSelectionChanged;

        // Set auto postback for selector
        siteSelector.DropDownSingleSelect.AutoPostBack = true;
        moduleSelector.DropDownSingleSelect.AutoPostBack = true;
        docTypeSelector.DropDownSingleSelect.AutoPostBack = true;
        customTableSelector.DropDownSingleSelect.AutoPostBack = true;
        userSelector.DropDownSingleSelect.AutoPostBack = true;
        moduleSelector.UniSelector.OnSelectionChanged += moduleSelector_SelectedIndexChanged;
        chkUserOnly.Text = GetString("Administration-Permissions_Header.UserRoles");
        chkUserOnly.CheckedChanged += chkUserOnly_CheckedChanged;
    }


    /// <summary>
    /// Page pre render event.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Disable empty uniselectors
        moduleSelector.Enabled = moduleSelector.UniSelector.HasData;
        docTypeSelector.Enabled = docTypeSelector.UniSelector.HasData;
    }

    #endregion


    #region "Private & protected methods"

    /// <summary>
    /// Initialize permission type drop down list.
    /// </summary>
    private void InitializeDropDownListPermissionType()
    {
        // Initialize drop down list with types
        drpPermissionType.Items.Clear();
        drpPermissionType.Items.Add(new ListItem(GetString("objecttype.cms_resource"), PermissionTypes.Module.ToString()));
        drpPermissionType.Items.Add(new ListItem(GetString("general.documenttype"), PermissionTypes.DocumentType.ToString()));

        // Check if any custom table available under site
        if (CustomTableHelper.GetCustomTableClasses(SiteID).HasResults()
            || (ValidationHelper.GetString(siteSelector.Value, String.Empty) == siteSelector.GlobalRecordValue)
            || (globalRecord && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin)))
        {
            drpPermissionType.Items.Add(new ListItem(GetString("general.customtable"), PermissionTypes.CustomTable.ToString()));
        }
    }


    /// <summary>
    /// Initialize permission matrix drop down list.
    /// </summary>
    private void InitializeDropDownListPermissionMatrix()
    {
        string permissionType = null;
        if (drpPermissionType.SelectedIndex > -1)
        {
            permissionType = drpPermissionType.SelectedValue;
        }

        if (!string.IsNullOrEmpty(permissionType))
        {
            moduleSelector.Visible = (permissionType == PermissionTypes.Module.ToString());
            // Ensure module selection from query string
            if ((moduleSelector.Visible) && (!RequestHelper.IsPostBack()))
            {
                string selectedModule = QueryHelper.GetString("module", null);
                if (!String.IsNullOrEmpty(selectedModule))
                {
                    ResourceInfo ri = ResourceInfoProvider.GetResourceInfo(selectedModule);
                    if (ri != null)
                    {
                        moduleSelector.Value = ri.ResourceID;
                    }
                }
            }
            docTypeSelector.Visible = (permissionType == PermissionTypes.DocumentType.ToString());
            customTableSelector.Visible = (permissionType == PermissionTypes.CustomTable.ToString());

            if (SiteID > 0)
            {
                string where = "ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + SiteID + ")";
                moduleSelector.SiteID = SiteID;
                customTableSelector.WhereCondition = where;
                docTypeSelector.WhereCondition = where;
            }
        }
    }

    #endregion


    #region "Event Handlers"

    protected void siteSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        userSelector.ReloadData();
        userSelector_OnSelectionChanged(null, null);
        InitializeDropDownListPermissionType();
        InitializeDropDownListPermissionMatrix();
        ReloadSelectors();
        FilterChanged = true;
    }


    protected void userSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Disable check box if no user selected
        int selUser = ValidationHelper.GetInteger(userSelector.Value, 0);
        if (selUser > 0)
        {
            chkUserOnly.Enabled = true;
        }
        else
        {
            chkUserOnly.Checked = false;
            chkUserOnly.Enabled = false;
        }
        FilterChanged = true;
    }


    protected void drpPermissionType_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitializeDropDownListPermissionMatrix();
        ReloadSelectors();
        FilterChanged = true;
    }


    protected void moduleSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        FilterChanged = true;
    }


    protected void chkUserOnly_CheckedChanged(object sender, EventArgs e)
    {
        FilterChanged = true;
    }


    private void ReloadSelectors()
    {
        globalRecord = (ValidationHelper.GetString(siteSelector.Value, String.Empty) == siteSelector.GlobalRecordValue);

        if (drpPermissionType.SelectedValue == PermissionTypes.Module.ToString())
        {
            moduleSelector.DisplayOnlyForGivenSite = !globalRecord;
            moduleSelector.ReloadData(true);
            if (moduleSelector.DropDownSingleSelect.Items.Count > 0)
            {
                moduleSelector.Value = null;
            }
        }
        else if (drpPermissionType.SelectedValue == PermissionTypes.DocumentType.ToString())
        {
            docTypeSelector.ReloadData(true);
            if (moduleSelector.DropDownSingleSelect.Items.Count > 0)
            {
                docTypeSelector.Value = null;
            }
        }
        else if (drpPermissionType.SelectedValue == PermissionTypes.CustomTable.ToString())
        {
            customTableSelector.ReloadData(true);
            if (moduleSelector.DropDownSingleSelect.Items.Count > 0)
            {
                customTableSelector.Value = null;
            }
        }
    }

    #endregion
}