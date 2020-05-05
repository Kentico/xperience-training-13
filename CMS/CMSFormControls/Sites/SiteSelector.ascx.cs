using System;
using System.Data;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_Sites_SiteSelector : FormEngineUserControl
{
    #region "Variables"

    private int mUserId;
    private bool? mUseCodeNameForSelection;
    
    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether singledropdownlist uses autocomplete mode.
    /// </summary>
    public bool UseUniSelectorAutocomplete
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
    /// Returns control's client ID based on usage autocomplete. Ussually used in javascript in combination with value property.
    /// </summary>
    public string DropDownControlID
    {
        get
        {
            return uniSelector.DropDownControlID;
        }
    }


    /// <summary>
    /// Inner update panel
    /// </summary>
    public CMSUpdatePanel UpdatePanel
    {
        get
        {
            return pnlUpdate;
        }
    }


    /// <summary>
    /// Additional CSS class for drop down list control.
    /// </summary>
    public String AdditionalDropDownCSSClass
    {
        get
        {
            return uniSelector.AdditionalDropDownCSSClass;
        }
        set
        {
            uniSelector.AdditionalDropDownCSSClass = value;
        }
    }


    /// <summary>
    /// Underlying form control
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            EnsureChildControls();
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets or sets selected items.
    /// </summary>
    public override object Value
    {
        get
        {
            if (AllowMultipleSelection)
            {
                return uniSelector.Value;
            }
            if (UseCodeNameForSelection)
            {
                return SiteName;
            }
            return SiteID;
        }
        set
        {
            if ((value == null) || AllowMultipleSelection)
            {
                uniSelector.Value = value;
            }
            else if (UseCodeNameForSelection)
            {
                SiteName = ValidationHelper.GetString(value, String.Empty);
            }
            else
            {
                SiteID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Returns client ID of the textbox.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.ClientID;
        }
    }


    /// <summary>
    /// Enables or disables multiple site selection.
    /// </summary>
    public bool AllowMultipleSelection
    {
        get
        {
            EnsureChildControls();
            return GetValue("AllowMultipleSelection", false);
        }
        set
        {
            EnsureChildControls();
            SetValue("AllowMultipleSelection", value);
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether to use SiteID or SiteName for value.
    /// </summary>
    public bool UseCodeNameForSelection
    {
        get
        {
            EnsureChildControls();
            if (mUseCodeNameForSelection == null)
            {
                mUseCodeNameForSelection = (((FieldInfo != null) && (FieldInfo.DataType != FieldDataType.Integer)) || GetValue("UseCodeNameForSelection", false));
            }
            return mUseCodeNameForSelection.Value;
        }
        set
        {
            EnsureChildControls();
            mUseCodeNameForSelection = value;
        }
    }


    /// <summary>
    /// Enables or disables (all) item in selector.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            EnsureChildControls();
            return uniSelector.AllowAll;
        }
        set
        {
            EnsureChildControls();
            uniSelector.AllowAll = value;
        }
    }


    /// <summary>
    /// Value for the (all) item.
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            EnsureChildControls();
            return uniSelector.AllRecordValue;
        }
        set
        {
            EnsureChildControls();
            uniSelector.AllRecordValue = value;
        }
    }


    /// <summary>
    /// Enables or disables (empty) item in selector.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            EnsureChildControls();
            return uniSelector.AllowEmpty;
        }
        set
        {
            EnsureChildControls();
            uniSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Value for the "(none)" item.
    /// </summary>
    public string NoneRecordValue
    {
        get
        {
            EnsureChildControls();
            return uniSelector.NoneRecordValue;
        }
        set
        {
            EnsureChildControls();
            uniSelector.NoneRecordValue = value;
        }
    }


    /// <summary>
    /// Enables or disables (global) item in selector. Uses uniSelector's SpecialFields property.
    /// </summary>
    public bool AllowGlobal
    {
        get
        {
            return GetValue("AllowGlobal", false);
        }
        set
        {
            SetValue("AllowGlobal", value);
        }
    }


    /// <summary>
    /// Value for (global) item, by default set to -4.
    /// </summary>
    public string GlobalRecordValue
    {
        get
        {
            return GetValue("GlobalRecordValue", UniSelector.US_GLOBAL_RECORD.ToString());
        }
        set
        {
            SetValue("GlobalRecordValue", value);
        }
    }


    /// <summary>
    /// Indicates if only running sites should be displayed.
    /// </summary>
    public bool OnlyRunningSites
    {
        get
        {
            return GetValue("OnlyRunningSites", false);
        }
        set
        {
            SetValue("OnlyRunningSites", value);
        }
    }


    /// <summary>
    /// Object type to filter using this site selector.
    /// </summary>
    public string TargetObjectType
    {
        get
        {
            return GetValue<string>("TargetObjectType", null);
        }
        set
        {
            SetValue("TargetObjectType", value);
        }
    }


    /// <summary>
    /// If true, full post-back is called when site changed
    /// </summary>
    public bool PostbackOnDropDownChange
    {
        get
        {
            return GetValue("PostbackOnDropDownChange", false);
        }
        set
        {
            SetValue("PostbackOnDropDownChange", value);
        }
    }


    /// <summary>
    /// Gets or sets user ID. If set site selector shows only sites assigned to user.
    /// </summary>
    public int UserId
    {
        get
        {
            if ((mUserId == 0) && !string.IsNullOrEmpty(UserName))
            {
                var user = UserInfoProvider.GetUserInfo(UserName);
                if (user != null)
                {
                    mUserId = user.UserID;
                }
            }

            return mUserId;
        }
        set
        {
            mUserId = value;
        }
    }


    /// <summary>
    /// Gets or sets DisplayNameFormat property of uni-selector.
    /// </summary>
    public string DisplayNameFormat
    {
        get
        {
            EnsureChildControls();
            return uniSelector.DisplayNameFormat;
        }
        set
        {
            EnsureChildControls();
            uniSelector.DisplayNameFormat = value;
        }
    }


    /// <summary>
    /// Gets or sets SiteID of current selected site.
    /// </summary>
    public int SiteID
    {
        get
        {
            EnsureChildControls();
            if (UseCodeNameForSelection)
            {
                string siteName = ValidationHelper.GetString(uniSelector.Value, null);
                SiteInfo si = SiteInfoProvider.GetSiteInfo(siteName);
                return (si != null) ? si.SiteID : 0;
            }

            return ValidationHelper.GetInteger(uniSelector.Value, 0);
        }
        set
        {
            EnsureChildControls();
            if (UseCodeNameForSelection)
            {
                string siteName = ValidationHelper.GetString(uniSelector.Value, null);
                SiteInfo si = SiteInfoProvider.GetSiteInfo(siteName);
                if (si != null)
                {
                    uniSelector.Value = si.SiteName;
                }
            }
            else
            {
                uniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets Site name of current selected site.
    /// </summary>
    public string SiteName
    {
        get
        {
            EnsureChildControls();
            if (UseCodeNameForSelection)
            {
                return ValidationHelper.GetString(uniSelector.Value, String.Empty);
            }

            int siteId = ValidationHelper.GetInteger(uniSelector.Value, 0);
            SiteInfo si = SiteInfoProvider.GetSiteInfo(siteId);
            return (si != null) ? si.SiteName : String.Empty;
        }
        set
        {
            EnsureChildControls();
            if (UseCodeNameForSelection)
            {
                uniSelector.Value = value;
            }
            else
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(value);
                if (si != null)
                {
                    uniSelector.Value = si.SiteID;
                }
            }
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets user name. If set site selector shows only sites assigned to user.
    /// </summary>
    private string UserName
    {
        get
        {
            return GetResolvedValue<string>("UserName", null);
        }
    }

    #endregion


    #region "Inner controls"

    /// <summary>
    /// Gets the inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets the single select drop down field.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            EnsureChildControls();
            return uniSelector.DropDownSingleSelect;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            UniSelector.OnAfterRetrieveData += UniSelector_OnAfterRetrieveData;

            // Setup controls
            SetupControls();

            // Generate where condition
            UpdateWhereCondition();
        }
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }

    #endregion


    #region "Private methods"

    private void SetupControls()
    {
        // Set other properties
        uniSelector.ReturnColumnName = UseCodeNameForSelection ? "SiteName" : "SiteID";

        // Register for post back
        if (PostbackOnDropDownChange)
        {
            uniSelector.DropDownSingleSelect.AutoPostBack = true;
            ScriptManager manager = ScriptManager.GetCurrent(Page);
            if (manager != null)
            {
                manager.RegisterPostBackControl(uniSelector);
            }
        }

        if (HasDependingFields)
        {
            uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
        }
    }


    protected void uniSelector_OnSpecialFieldsLoaded(object sender, EventArgs e)
    {
        HandleSpecialFields();
    }


    private void HandleSpecialFields()
    {
        // Ensure correct value for code name mode
        if (UseCodeNameForSelection)
        {
            uniSelector.AllRecordValue = TreeProvider.ALL_SITES;
            uniSelector.NoneRecordValue = String.Empty;
        }

        // Ensure global item
        if (AllowGlobal)
        {
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.global"), Value = GlobalRecordValue });
        }
    }


    /// <summary>
    /// Handles the OnSelectionChanged event of the usSites control.
    /// </summary>
    private void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }


    /// <summary>
    /// Updates uni-selector where condition based on current properties values.
    /// </summary>
    private void UpdateWhereCondition()
    {
        // Running sites where condition
        if (OnlyRunningSites)
        {
            // Running status where condition
            var siteStatusWhere = new WhereCondition().WhereEquals("SiteStatus", SiteStatusEnum.Running.ToStringRepresentation());

            // Combine where conditions
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, siteStatusWhere.ToString(true));
        }

        // Only sites assigned to user
        if (UserId != 0)
        {
            // User's site where condition
            string where = "SiteID IN (SELECT SiteID FROM CMS_UserSite WHERE UserID = " + UserId + ")";

            // Combine where conditions
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, where);
        }
    }


    /// <summary>
    /// After data retrieve event
    /// </summary>
    private DataSet UniSelector_OnAfterRetrieveData(DataSet ds)
    {
        // For empty site selector - change text of none from (currentsite) -> (no site)
        if (DataHelper.DataSourceIsEmpty(ds) && (UniSelector.SelectionMode == SelectionModeEnum.SingleDropDownList))
        {
            UniSelector.ResourcePrefix = "emptysiteselect";
        }

        return ds;
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Adds class to autcomplete menu list.
    /// </summary>
    /// <param name="cssClass">Class to add</param>
    public void AddClassToAutocompleteWidget(String cssClass)
    {
        uniSelector.AddClassToAutocompleteWidget(cssClass);
    }


    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName)
        {
            case "AllowMultipleSelection":
                uniSelector.SelectionMode = AllowMultipleSelection ? SelectionModeEnum.MultipleTextBox : SelectionModeEnum.SingleDropDownList;
                uniSelector.ResourcePrefix = AllowMultipleSelection ? "sitesselect" : "siteselect";
                break;
        }

        return true;
    }


    /// <summary>
    /// Returns WHERE condition for specified column.
    /// </summary>
    /// <param name="siteColumnName">Site column name</param>
    public string GetWhereCondition(string siteColumnName)
    {
        if (!StopProcessing)
        {
            switch (SiteID)
            {
                // (all)
                case (UniSelector.US_ALL_RECORDS):
                    break;

                // (global and this site)
                case (UniSelector.US_GLOBAL_AND_SITE_RECORD):
                    return UseCodeNameForSelection ? String.Format("{0} IS NULL OR {0} = N'{1}'", siteColumnName, SiteContext.CurrentSiteName) : string.Format("{0} IS NULL OR {0} = {1}", siteColumnName, SiteContext.CurrentSiteID);

                // (global)
                case (UniSelector.US_GLOBAL_RECORD):
                case (UniSelector.US_NONE_RECORD):
                    return UseCodeNameForSelection ? String.Format("{0} IS NULL OR {0} = N''", siteColumnName) : string.Format("{0} IS NULL", siteColumnName);

                default:
                    return UseCodeNameForSelection ? String.Format("{0} = N'{1}'", siteColumnName, SiteName) : string.Format("{0} = {1}", siteColumnName, SiteID);
            }
        }

        return null;
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        if (FieldInfo != null)
        {
            return GetWhereCondition(FieldInfo.Name);
        }
        
        if (!string.IsNullOrEmpty(TargetObjectType))
        {
            // Get site where condition based on target object type
            var obj = ModuleManager.GetReadOnlyObject(TargetObjectType);
            if ((obj != null) && (obj.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
            {
                return GetWhereCondition(obj.TypeInfo.SiteIDColumn);
            }
        }

        return base.GetWhereCondition();
    }


    /// <summary>
    /// Reloads all controls.
    /// </summary>
    /// <param name="forceReload">Indicates if data should be loaded from DB</param>
    public void Reload(bool forceReload)
    {
        // Setup controls
        SetupControls();

        // Generate where condition
        UpdateWhereCondition();

        uniSelector.Reload(forceReload);
    }

    #endregion
}