using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_FormControls_Users_SelectUser : FormEngineUserControl
{
    #region "Variables"

    private string mResourcePrefix = String.Empty;
    private int mSiteId = int.MinValue;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets underlying control.
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return usUsers;
        }
    }


    /// <summary>
    /// Gets or sets return column name.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            EnsureChildControls();
            return usUsers.ReturnColumnName;
        }
        set
        {
            EnsureChildControls();
            usUsers.ReturnColumnName = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the site. User of this site are displayed
    /// Use 0 for current siteid, -1 for all sites(no filter)
    /// Note: SiteID is not used if site filter is enabled
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId == int.MinValue)
            {
                mSiteId = GetValue("siteid", 0);
            }
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets ID of the role. User of this role are displayed.
    /// </summary>
    public int RoleID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value which determines whether to allow more than one user to select.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            EnsureChildControls();
            return usUsers.SelectionMode;
        }
        set
        {
            EnsureChildControls();
            usUsers.SelectionMode = value;
        }
    }
     

    /// <summary>
    /// Gets or sets the text displayed if there are no data.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            EnsureChildControls();
            return usUsers.ZeroRowsText;
        }
        set
        {
            EnsureChildControls();
            usUsers.ZeroRowsText = value;
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
            EnsureChildControls();

            base.Enabled = value;
            usUsers.Enabled = value;
        }
    }


    ///<summary>
    /// Gets or sets field value.
    ///</summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usUsers.Value;
        }
        set
        {
            EnsureChildControls();
            usUsers.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSiteFilter"), true);
        }
        set
        {
            SetValue("ShowSiteFilter", value);
        }
    }


    /// <summary>
    /// Gets the current UniSelector instance.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return usUsers;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the UniSelector should apply WhereCondition for the selected value (default: true). This does not affect the modal dialog.
    /// </summary>
    public bool ApplyValueRestrictions
    {
        get
        {
            EnsureChildControls();
            return usUsers.ApplyValueRestrictions;
        }
        set
        {
            EnsureChildControls();
            usUsers.ApplyValueRestrictions = value;
        }
    }


    /// <summary>
    /// Gets the single select drop down field.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return usUsers.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets the resource prefix of uni-selector. If not set default values are used.
    /// </summary>
    public override string ResourcePrefix
    {
        get
        {
            return mResourcePrefix;
        }
        set
        {
            mResourcePrefix = value;
            usUsers.ResourcePrefix = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            EnsureChildControls();
            return usUsers.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            usUsers.IsLiveSite = value;
        }
    }


    /// <summary>
    /// If enabled disabled users aren't shown in selector.
    /// </summary>
    public bool HideDisabledUsers
    {
        get
        {
            return GetValue("hidedisabledusers", false);
        }
        set
        {
            SetValue("hidedisabledusers", value);
        }
    }


    /// <summary>
    /// If enabled hidden users aren't shown in selector (does not apply to global administrators, these are shown regardless of this property).
    /// </summary>
    public bool HideHiddenUsers
    {
        get
        {
            return GetValue("hidehiddenusers", false);
        }
        set
        {
            SetValue("hidehiddenusers", value);
        }
    }


    /// <summary>
    /// If enabled non-approved users aren't shown is selector.
    /// </summary>
    public bool HideNonApprovedUsers
    {
        get
        {
            return GetValue("hidenonapprovedusers", false);
        }
        set
        {
            SetValue("hidenonapprovedusers", value);
        }
    }


    /// <summary>
    /// If true selector will always display users from all sites.
    /// Suited for selecting users having access to the site.
    /// </summary>
    public bool DisplayUsersFromAllSites
    {
        get
        {
            return GetValue("displayusersfromallsites", false);
        }
        set
        {
            SetValue("displayusersfromallsites", value);
        }
    }


    /// <summary>
    /// Includes global administrators into site users.
    /// </summary>
    public bool TreatGlobalAdminsAsSiteUsers
    {
        get
        {
            return GetValue("treatglobaladminsassiteusers", false);
        }
        set
        {
            SetValue("treatglobaladminsassiteusers", value);
        }
    }


    /// <summary>
    /// Include global administrators no matter what.
    /// </summary>
    public bool AlwaysShowGlobalAdministrators
    {
        get
        {
            return GetValue("alwaysshowglobaladministrators", false);
        }
        set
        {
            SetValue("alwaysshowglobaladministrators", value);
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
            return usUsers.AllowAll;
        }
        set
        {
            EnsureChildControls();
            usUsers.AllowAll = value;
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
            return usUsers.AllowEmpty;
        }
        set
        {
            EnsureChildControls();
            usUsers.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Enables or disables (default) item in selector.
    /// </summary>
    public bool AllowDefault
    {
        get
        {
            EnsureChildControls();
            return usUsers.AllowDefault;
        }
        set
        {
            EnsureChildControls();
            usUsers.AllowDefault = value;
        }
    }


    /// <summary>
    /// Additional users to show identified by user IDs.
    /// </summary>
    public int[] AdditionalUsers
    {
        get
        {
            return (int[])GetValue("AdditionalUsers");
        }
        set
        {
            SetValue("AdditionalUsers", value);
        }
    }


    /// <summary>
    /// Where condition used to filter control data.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            EnsureChildControls();
            return usUsers.WhereCondition;
        }
        set
        {
            EnsureChildControls();
            usUsers.WhereCondition = value;
        }
    }

    #endregion


    #region "Control methods"

    protected override void OnInit(EventArgs e)
    {
        usUsers.DisplayNameFormat = UniSelector.USER_DISPLAY_FORMAT;
        usUsers.UniGrid.Pager.UniPager.PageSize = 10;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            usUsers.StopProcessing = true;
            return;
        }
        
        SetupControls();

        if (!RequestHelper.IsCallback())
        {
            // Don't reload data when request is call back
            ReloadData();
        }

        // Reload data again, if page changed
        if (usUsers.UniGrid.Pager != null && usUsers.UniGrid.Pager.UniPager != null)
        {
            usUsers.UniGrid.Pager.UniPager.OnPageChanged += UniPager_OnPageChanged;
        }
    }


    private void UniPager_OnPageChanged(object sender, int pageNumber)
    {
        ReloadData();
    }


    protected void SetupControls()
    {
        // If current control context is widget or livesite hide site selector
        if (ControlsHelper.CheckControlContext(this, ControlContext.WIDGET_PROPERTIES))
        {
            ShowSiteFilter = false;
        }

        // Set prefix if not set
        if (ResourcePrefix == String.Empty)
        {
            // Set resource prefix based on mode
            if ((SelectionMode == SelectionModeEnum.Multiple) || (SelectionMode == SelectionModeEnum.MultipleButton) || (SelectionMode == SelectionModeEnum.MultipleTextBox))
            {
                usUsers.ResourcePrefix = "selectusers";
            }
        }

        // Add sites filter
        if (ShowSiteFilter)
        {
            usUsers.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
            usUsers.SetValue("DefaultFilterValue", (SiteID > 0) ? SiteID : SiteContext.CurrentSiteID);
            usUsers.SetValue("FilterMode", "user");
        }

        // Generate WhereCondtion based on SelectUser's properties
        var usersWhereCondition = new WhereCondition();
        var userTypeInfo = ObjectTypeManager.GetTypeInfo(UserInfo.OBJECT_TYPE);

        // Hide hidden users
        if (HideHiddenUsers)
        {
            var apparentUsersCondition = new WhereCondition().WhereEqualsOrNull("UserIsHidden", 0);
            usersWhereCondition.And(apparentUsersCondition);
        }

        // Hide disabled users
        if (HideDisabledUsers)
        {
            var enabledUsersCondition = new WhereCondition(UserInfoProvider.UserEnabledWhereCondition);
            usersWhereCondition.And(enabledUsersCondition);
        }

        // Hide non-approved users
        if (HideNonApprovedUsers)
        {
            var approvedUserIDs = UserSettingsInfo.Provider
                                                .Get()
                                                .WhereEqualsOrNull("UserWaitingForApproval", 0)
                                                .Column("UserSettingsUserID");

            var approvedUsersCondition = new WhereCondition().WhereIn(userTypeInfo.IDColumn, approvedUserIDs);
            usersWhereCondition.And(approvedUsersCondition);
        }

        // Select users in role
        if (RoleID > 0)
        {
            var usersInRoleIDs = UserRoleInfo.Provider
                                        .Get()
                                        .WhereEquals("RoleID", RoleID)
                                        .Column("UserID");

            var usersInRoleCondition = new WhereCondition().WhereIn(userTypeInfo.IDColumn, usersInRoleIDs);
            usersWhereCondition.And(usersInRoleCondition);
        }

        // Select users depending on site; if filter enabled, where condition is added from filter itself
        if (!ShowSiteFilter && (SiteID >= 0) && !DisplayUsersFromAllSites)
        {
            int siteID = (SiteID == 0) ? SiteContext.CurrentSiteID : SiteID;
            var siteUserIDs = UserSiteInfo.Provider
                                    .Get()
                                    .WhereEquals("SiteID", siteID)
                                    .Column("UserID");

            var siteUsersCondition = new WhereCondition().WhereIn(userTypeInfo.IDColumn, siteUserIDs);

            if (TreatGlobalAdminsAsSiteUsers)
            {
                siteUsersCondition.Or(GetPrivilegeLevelCondition());
            }

            usersWhereCondition.And(siteUsersCondition);
        }

        if (AlwaysShowGlobalAdministrators)
        {
            // New instance has to be created in order to produce brackets
            usersWhereCondition = new WhereCondition(usersWhereCondition).Or(GetPrivilegeLevelCondition());
        }

        // Add additional users
        if ((AdditionalUsers != null) && (AdditionalUsers.Length > 0))
        {
            var additionalUsersCondition = new WhereCondition().WhereIn(userTypeInfo.IDColumn, AdditionalUsers);

            new WhereCondition(usersWhereCondition).Or(additionalUsersCondition);
        }

        // Control where condition
        if (!String.IsNullOrEmpty(WhereCondition))
        {
            usersWhereCondition = new WhereCondition(usersWhereCondition).And(new WhereCondition(WhereCondition));
        }

        // Append generated where condition
        usUsers.WhereCondition = SqlHelper.AddWhereCondition(usUsers.WhereCondition, usersWhereCondition.ToString(expand: true));

        pnlUpdate.ShowProgress = (SelectionMode == SelectionModeEnum.Multiple);
    }


    /// <summary>
    /// Returns condition that checks if user has at least <see cref="UserPrivilegeLevelEnum.Admin"/> privilege level.
    /// </summary>
    private static WhereCondition GetPrivilegeLevelCondition()
    {
        return new WhereCondition().WhereGreaterOrEquals("UserPrivilegeLevel", (int)UserPrivilegeLevelEnum.Admin);
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usUsers == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }


    /// <summary>
    /// Reloads the data of the UniSelector.
    /// </summary>
    public void ReloadData()
    {
        usUsers.Reload(true);
        pnlUpdate.Update();
    }


    /// <summary>
    /// Reloads whole control including control settings and data.
    /// </summary>
    public void Reload()
    {
        SetupControls();
        ReloadData();
    }

    #endregion
}