using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_SelectModule : FormEngineUserControl
{
    #region "Variables"

    private int mSiteID = 0;
    private ResourceInfo mSelectedModule;

    #endregion


    #region "Public properties"

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
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Indicates whether singledropdownlist uses autocomplete mode.
    /// </summary>
    public virtual bool UseUniSelectorAutocomplete
    {
        get
        {
            return uniSelector.UseUniSelectorAutocomplete;
        }
        set
        {
            uniSelector.UseUniSelectorAutocomplete = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the DLL with module.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
            mSelectedModule = null;
        }
    }


    /// <summary>
    /// If true, All is avaible
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return uniSelector.AllowAll;
        }
        set
        {
            uniSelector.AllowAll = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site for which the modules should be returned. 0 means current site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
            if (uniSelector != null)
            {
                uniSelector.WhereCondition = GetProperWhereCondition();
            }
        }
    }


    /// <summary>
    /// Gets the inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets the inner DDL control.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// If true, displays only modules which are assigned to the given site.
    /// </summary>
    public bool DisplayOnlyForGivenSite
    {
        get;
        set;
    }


    /// <summary>
    /// If true, displays only modules which have some permissions to be displayed in permission matrix.
    /// </summary>
    public bool DisplayOnlyWithPermission
    {
        get;
        set;
    }


    /// <summary>
    /// If true, displays all modules. If false, displays only modules with some UI Elements.
    /// </summary>
    public bool DisplayAllModules
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAllModules"), false);
        }
        set
        {
            SetValue("DisplayAllModules", value);
        }
    }



    /// <summary>
    /// If true, the selector works in filter mode and ignores development mode.
    /// </summary>
    public bool FilterMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only modules in development mode should be displayed
    /// </summary>
    public bool DisplayOnlyModulesInDevelopmentMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyModulesInDevelopmentMode"), false);
        }
        set
        {
            SetValue("DisplayOnlyModulesInDevelopmentMode", value);
        }
    }


    /// <summary>
    /// If true, displays the none option, returning 0 as the value.
    /// </summary>
    public bool DisplayNone
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayNone"), false);
        }
        set
        {
            SetValue("DisplayNone", value);
        }
    }


    /// <summary>
    /// Return column name for unigrid
    /// </summary>
    public String ReturnColumnName
    {
        get
        {
            return GetValue("ReturnColumnName", "ResourceID");
        }
        set
        {
            SetValue("ReturnColumnName", value);
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
            EnsureChildControls();
            base.IsLiveSite = value;
            UniSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Selected/assigned module or null if none is selected.
    /// </summary>
    private ResourceInfo SelectedModule
    {
        get
        {
            return mSelectedModule ?? (mSelectedModule = ResourceInfoProvider.GetResourceInfo(ValidationHelper.GetInteger(Value, 0)));
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        uniSelector.ReturnColumnName = ReturnColumnName;
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData(false);
        }

        if (!FilterMode && (SelectedModule != null) && !SelectedModule.ResourceIsInDevelopment)
        {
            // Disable the control handling the module assignment
            Enabled = false;
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    /// <param name="forcedReload">Indicates whether the UniSelector should be reloaded</param>
    public void ReloadData(bool forcedReload)
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.WhereCondition = GetProperWhereCondition();
        uniSelector.OrderBy = "ResourceDisplayName";

        if (DisplayNone)
        {
            uniSelector.AllowEmpty = true;
            uniSelector.NoneRecordValue = string.Empty;
        }

        if (forcedReload)
        {
            uniSelector.Reload(true);
       } 
    }


    /// <summary>
    /// Returns proper where condition.
    /// </summary>
    private string GetProperWhereCondition()
    {
        // Check module development only when is not in filter mode
        if (!FilterMode && (SelectedModule != null) && !SelectedModule.ResourceIsInDevelopment)
        {
            // Load just the selected module
            return "ResourceID = " + Value;
        }

        string where = "";

        if (!DisplayAllModules)
        {
            if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                where = "(ResourceID IN (SELECT DISTINCT ElementResourceID FROM CMS_UIElement)) AND NOT ResourceName = 'CMS.WYSIWYGEditor'";
            }
            else
            {
                where = "(ResourceID IN (SELECT ResourceID FROM CMS_ResourceSite WHERE SiteID = " + (SiteID == 0 ? SiteContext.CurrentSiteID : SiteID) + ")) AND (ResourceID IN (SELECT ElementResourceID FROM CMS_UIElement WHERE ElementParentID IS NULL AND ElementChildCount > 0)) AND NOT ResourceName = 'CMS.WYSIWYGEditor'";
            }
        }

        if (DisplayOnlyForGivenSite)
        {
            if (where != "")
            {
                where += " AND ";
            }
            where += "(ResourceID IN (SELECT ResourceID FROM CMS_ResourceSite WHERE SiteID = " + (SiteID == 0 ? SiteContext.CurrentSiteID : SiteID) + "))";
        }

        if (DisplayOnlyWithPermission)
        {
            if (where != "")
            {
                where += " AND ";
            }
            where += "EXISTS (SELECT PermissionID FROM CMS_Permission WHERE CMS_Permission.ResourceID = CMS_Resource.ResourceID AND PermissionDisplayInMatrix = 1)";
        }

        if (DisplayOnlyModulesInDevelopmentMode)
        {
            where = SqlHelper.AddWhereCondition(where, "ISNULL(ResourceIsInDevelopment, 0) = 1 OR ResourceName='" + ModuleName.CUSTOMSYSTEM + "'");
        }

        return where;
    }
}