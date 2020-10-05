using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_FormControls_Roles_SelectRole : FormEngineUserControl
{
    #region "Variables"

    private int mSiteId = 0;
    private bool mUseCodeNameForSelection = true;
    private bool mGlobalRoles = true;
    private bool mSiteRoles = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether global objects have suffix "(global)" in the grid.
    /// </summary>
    public bool AddGlobalObjectSuffix
    {
        get
        {
            return CurrentSelector.AddGlobalObjectSuffix;
        }
        set
        {
            CurrentSelector.AddGlobalObjectSuffix = value;
        }
    }


    /// <summary>
    /// Indicates if role selector allow empty selection.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return CurrentSelector.AllowEmpty;
        }
        set
        {
            CurrentSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// If true site roles are selected.
    /// </summary>
    public bool SiteRoles
    {
        get
        {
            return mSiteRoles;
        }
        set
        {
            mSiteRoles = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site to display:
    /// (-1 means CurrentSite, 0 means you can choose, > 0 means specific site)
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
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            CurrentSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets the current UniSelector instance.
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            EnsureChildControls();
            return usRoles;
        }
    }


    /// <summary>
    /// Gets or sets role name.
    /// </summary>
    public override object Value
    {
        get
        {
            return Convert.ToString(CurrentSelector.Value);
        }
        set
        {
            CurrentSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return CurrentSelector.ValueDisplayName;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether role should be edited in friendly format.
    /// </summary>
    public bool UseFriendlyMode
    {
        get
        {
            return !CurrentSelector.AllowEditTextBox;
        }
        set
        {
            CurrentSelector.AllowEditTextBox = !value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to use code name as return value.
    /// </summary>
    public bool UseCodeNameForSelection
    {
        get
        {
            return mUseCodeNameForSelection;
        }
        set
        {
            mUseCodeNameForSelection = value;
        }
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return GetValue("ShowSiteFilter", true);
        }
        set
        {
            SetValue("ShowSiteFilter", value);
        }
    }


    /// <summary>
    /// Gets or sets if control works in simple mode (ignores SiteID parameter).
    /// </summary>
    public bool SimpleMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if live site property.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            CurrentSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the drop down control.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return CurrentSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Returns ClientID of the textboxselect.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return CurrentSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// If true global roles are selected.
    /// </summary>
    public bool GlobalRoles
    {
        get
        {
            return mGlobalRoles;
        }
        set
        {
            mGlobalRoles = value;
        }
    }


    /// <summary>
    /// Indicates if generic role 'Everyone' should be displayed even if other generic roles are hidden.
    /// </summary>
    public bool DisplayEveryone
    {
        get;
        set;
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Underlying form control
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return usRoles;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Page_Load event.
    /// </summary>    
    protected void Page_Load(object sender, EventArgs e)
    {
        // If current control context is widget or livesite hide site selector
        if (ControlsHelper.CheckControlContext(this, ControlContext.WIDGET_PROPERTIES))
        {
            ShowSiteFilter = false;
        }

        Reload(false);
    }


    /// <summary>
    /// Reloads the selector's data.
    /// </summary>
    /// <param name="forceReload">Indicates whether data should be forcibly reloaded</param>
    public void Reload(bool forceReload)
    {
        // Set allow empty
        usRoles.AllowEmpty = AllowEmpty;

        // Get siteID
        if (SiteID == 0)
        {
            SiteID = SiteContext.CurrentSiteID;
        }

        // Add sites filter
        if (ShowSiteFilter)
        {
            usRoles.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
            usRoles.SetValue("DefaultFilterValue", SiteID);
            usRoles.SetValue("FilterMode", "role");
        }


        // Build where condition
        string whereCondition = String.Empty;

        if (UseFriendlyMode)
        {
            if (DisplayEveryone)
            {
                whereCondition += "RoleName != '" + RoleName.AUTHENTICATED + "' AND RoleName != '" + RoleName.NOTAUTHENTICATED + "' ";
            }
            else
            {
                whereCondition += "RoleName != '" + RoleName.EVERYONE + "' AND RoleName != '" + RoleName.AUTHENTICATED + "' AND RoleName != '" + RoleName.NOTAUTHENTICATED + "' ";
            }
        }

        if (!ShowSiteFilter)
        {
            if (SiteRoles && GlobalRoles)
            {
                whereCondition = SqlHelper.AddWhereCondition(whereCondition, "SiteID IS NULL OR  SiteID =" + SiteID.ToString());
            }
            else if (SiteRoles)
            {
                whereCondition = SqlHelper.AddWhereCondition(whereCondition, "SiteID =" + SiteID.ToString());
            }
            else if (GlobalRoles)
            {
                whereCondition = SqlHelper.AddWhereCondition(whereCondition, "SiteID IS NULL");
            }
        }

        usRoles.ReturnColumnName = (UseCodeNameForSelection ? "RoleName" : "RoleID");
        usRoles.WhereCondition = whereCondition;

        if (SiteRoles && GlobalRoles && UseCodeNameForSelection)
        {
            usRoles.AddGlobalObjectNamePrefix = true;
        }

        if (forceReload)
        {
            usRoles.Reload(forceReload);
        }
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usRoles == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }

    #endregion
}