using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_Users_UserFilter : CMSAbstractBaseFilterControl, IUserFilter
{
    #region "Constants"

    private const string ONLINE_USERS_MODE = "online";

    private const string GROUP_SELECTOR_PATH = "~/CMSModules/Groups/FormControls/MembershipGroupSelector.ascx";
    private const string SCORE_SELECTOR_PATH = "~/CMSModules/Scoring/FormControls/SelectScore.ascx";

    private const string SESSION_USER_NAME_COLUMN = "SessionUserName";
    private const string SESSION_FULL_NAME_COLUMN = "SessionFullName";
    private const string SESSION_NICK_NAME_COLUMN = "SessionNickName";
    private const string SESSION_EMAIL_COLUMN = "SessionEmail";

    private static readonly string[] SESSION_USER_COLUMNS =
    {
        SESSION_USER_NAME_COLUMN,
        SESSION_EMAIL_COLUMN,
        SESSION_FULL_NAME_COLUMN,
        SESSION_NICK_NAME_COLUMN
    };


    private static readonly string[] DATABASE_USER_COLUMNS =
    {
        "UserName",
        "Email",
        "FullName",
        "UserNickName"
    };

    #endregion


    #region "Variables"

    private bool mIsAdvancedMode;
    private FormEngineUserControl mScoreSelector;
    private bool? mDisplayGuestsByDefault;
    private bool mShowPrivilegeFilter;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return GenerateWhereCondition().ToString(true);
        }
        set
        {
            base.WhereCondition = value;
        }
    }



    /// <summary>
    /// Gets or sets if anonymous visitors should be displayed by default
    /// </summary>
    public bool DisplayGuestsByDefault
    {
        get
        {
            if (mDisplayGuestsByDefault == null)
            {
                return QueryHelper.GetBoolean("guest", false);
            }
            return (bool)mDisplayGuestsByDefault;
        }
        set
        {
            mDisplayGuestsByDefault = value;
        }
    }


    /// <summary>
    /// Indicates if guests should be displayed.
    /// </summary>
    public bool DisplayGuests
    {
        get
        {
            return (chkDisplayAnonymous.Checked && chkDisplayAnonymous.Visible && mIsAdvancedMode) || (chkDisplayAnonymous.Checked && DisplayGuestsByDefault) && ContactManagementPermission;
        }
    }


    /// <summary>
    /// Indicates if user has permission to display contact data.
    /// </summary>
    public bool ContactManagementPermission
    {
        get
        {
            return ResourceSiteInfoProvider.IsResourceOnSite("CMS.ContactManagement", SiteContext.CurrentSiteName)
                     && CurrentUser.IsAuthorizedPerUIElement("CMS.OnlineMarketing", "Contacts")
                     && CurrentUser.IsAuthorizedPerResource("CMS.ContactManagement", "Read")
                     && ModuleEntryManager.IsModuleLoaded(ModuleName.CONTACTMANAGEMENT);
        }
    }


    /// <summary>
    /// Indicates if 'user enabled' filter should be hidden.
    /// </summary>
    public bool DisplayUserEnabled
    {
        get
        {
            return plcUserEnabled.Visible;
        }
        set
        {
            plcUserEnabled.Visible = value;
        }
    }


    /// <summary>
    /// Selected score.
    /// </summary>
    public int SelectedScore
    {
        get
        {
            if ((mScoreSelector != null) && mScoreSelector.Enabled)
            {
                return ValidationHelper.GetInteger(mScoreSelector.Value, 0);
            }
            return 0;
        }
    }


    /// <summary>
    /// Selected site.
    /// </summary>
    public int SelectedSite
    {
        get
        {
            return (siteSelector.Visible ? siteSelector.SiteID : 0);
        }
    }


    /// <summary>
    /// Indicates if checkbox for hiding/displaying hidden users should be visible.
    /// </summary>
    public bool EnableDisplayingHiddenUsers
    {
        get
        {
            return plcHidden.Visible;
        }
        set
        {
            plcHidden.Visible = value;
        }
    }


    /// <summary>
    /// Indicates if checkbox for displaying guests should be visible.
    /// </summary>
    public bool EnableDisplayingGuests
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if filter is working with CMS_Session table instead of CMS_User.
    /// </summary>
    public bool SessionInsteadOfUser
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets filter mode for various type of users list.
    /// </summary>
    public string CurrentMode
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if contacts can be displayed.
    /// </summary>
    public bool DisplayContacts
    {
        get
        {
            return ResourceSiteInfoProvider.IsResourceOnSite("CMS.ContactManagement", SiteContext.CurrentSiteName)
             && ModuleEntryManager.IsModuleLoaded(ModuleName.CONTACTMANAGEMENT);
        }
    }


    /// <summary>
    /// Indicates if score can be displayed.
    /// </summary>
    public bool DisplayScore
    {
        get
        {
            return CurrentUser.IsAuthorizedPerUIElement("CMS.Scoring", "Scoring")
                && ResourceSiteInfoProvider.IsResourceOnSite("CMS.Scoring", SiteContext.CurrentSiteName)
                && LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.LeadScoring)
                && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Scoring", "Read")
                && ModuleEntryManager.IsModuleLoaded(ModuleName.ONLINEMARKETING);
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        grid?.Reset();
    }


    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        grid?.ApplyFilter(sender, e);
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// OnInit event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        int querySiteID = QueryHelper.GetInteger("siteid", 0);

        // Do not allow other than current site ID out of global scope.
        var uiContext = UIContextHelper.GetUIContext(this);
        SiteID = ApplicationUIHelper.IsAccessibleOnlyByGlobalAdministrator(uiContext.UIElement) ? querySiteID : SiteContext.CurrentSiteID;

        if (DisplayScore && SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSEnableOnlineMarketing"))
        {
            Control ctrl = LoadUserControl(SCORE_SELECTOR_PATH);
            if (ctrl != null)
            {
                ctrl.ID = "selectScore";
                mScoreSelector = ctrl as FormEngineUserControl;
                if (mScoreSelector != null)
                {
                    plcUpdateContent.Controls.Add(mScoreSelector);
                    mScoreSelector.SetValue("AllowAll", false);
                    mScoreSelector.SetValue("AllowEmpty", true);
                }
            }
        }
        else
        {
            plcScore.Visible = false;
            lblScore.AssociatedControlID = null;
        }

        InitializeDropDownLists();

        base.OnInit(e);

        plcDisplayAnonymous.Visible = ContactManagementPermission && EnableDisplayingGuests;
        if (!RequestHelper.IsPostBack())
        {
            chkDisplayAnonymous.Checked = DisplayGuestsByDefault;
        }

        siteSelector.DropDownSingleSelect.AutoPostBack = true;
    }


    /// <summary>
    /// Load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeForm();

        // Score selector is not null only if DisplayScore is true
        if (mScoreSelector != null)
        {
            if (IsOnlineUsersUI() && DisplayContacts)
            {
                plcScore.Visible = true;

                int siteId = QueryHelper.GetInteger("siteid", 0);
                mScoreSelector.Enabled = (siteId > 0) || ((siteId == 0) && (siteSelector.SiteID > 0));

                if (siteId == 0)
                {
                    mScoreSelector.SetValue("SiteID", siteSelector.SiteID);
                }
            }
            else
            {
                // Disable loading not visible control
                mScoreSelector.StopProcessing = true;
            }
        }

        // Show correct filter panel
        SetCorrectFilterMode();

        // Set reset link button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null && grid.RememberState)
        {
            if (mIsAdvancedMode)
            {
                btnAdvancedReset.Click += btnReset_Click;
            }
            else
            {
                btnReset.Click += btnReset_Click;
            }
        }
        else
        {
            if (mIsAdvancedMode)
            {
                btnAdvancedReset.Visible = false;
            }
            else
            {
                btnReset.Visible = false;
            }
        }

        // Set privilege level filter
        if (ShowPrivilegeLevelFilter(grid))
        {
            plcPrivilegeLevel.Visible = true;
            mShowPrivilegeFilter = true;
        }
        else
        {
            mShowPrivilegeFilter = false;
        }

        // Setup role selector
        selectNotInRole.SiteID = SiteID;
        selectRoleElem.SiteID = SiteID;
        selectRoleElem.CurrentSelector.ResourcePrefix = "addroles";
        selectNotInRole.CurrentSelector.ResourcePrefix = "addroles";
        selectRoleElem.UseFriendlyMode = true;
        selectNotInRole.UseFriendlyMode = true;

        if (SessionInsteadOfUser && DisplayGuestsByDefault)
        {
            plcNickName.Visible = false;
            plcUserName.Visible = false;
        }

        if (QueryHelper.GetBoolean("isonlinemarketing", false))
        {
            // Set disabled modules info (only on On-line marketing tab)
            ucDisabledModule.TestSettingKeys = "CMSEnableOnlineMarketing";
            ucDisabledModule.Visible = true;
        }
    }

    /// <summary>
    /// PreRender event.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        drpLockReason.Enabled = chkEnabled.Checked;

        if (!drpLockReason.Enabled)
        {
            drpLockReason.SelectedIndex = 0;
        }
    }

    #endregion


    #region "UI methods"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("AdvancedMode", mIsAdvancedMode);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        mIsAdvancedMode = state.GetBoolean("AdvancedMode");
        ViewState["IsAdvancedMode"] = mIsAdvancedMode;
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtSearch.Text = String.Empty;
        ViewState["IsAdvancedMode"] = mIsAdvancedMode;

        fltEmail.ResetFilter();
        fltUserName.ResetFilter();
        fltNickName.ResetFilter();
        fltFullName.ResetFilter();

        drpTypeSelectInRoles.SelectedIndex = 0;
        drpTypeSelectNotInRoles.SelectedIndex = 0;
        drpPrivilege.SelectedIndex = 0;

        if (siteSelector.DropDownSingleSelect.Items.Count > 0)
        {
            siteSelector.DropDownSingleSelect.SelectedIndex = 0;
        }
        if (mScoreSelector != null)
        {
            mScoreSelector.Value = 0;
            mScoreSelector.Enabled = (SiteID != 0);
        }

        selectRoleElem.Value = "";
        selectNotInRole.Value = "";

        chkDisplayAnonymous.Checked = DisplayGuestsByDefault;
        chkEnabled.Checked = false;
    }


    /// <summary>
    /// Returns <c>false</c> if <paramref name="grid"/> or any of its members is <c>null</c> and if its object type is not <see cref="PredefinedObjectType.USER"/> otherwise returns <c>true</c>.
    /// </summary>
    private static bool ShowPrivilegeLevelFilter(UniGrid grid)
    {
        return (grid?.InfoObject?.TypeInfo?.OriginalObjectType == PredefinedObjectType.USER);
    }


    /// <summary>
    /// Initializes advanced filter dropdown lists.
    /// </summary>
    private void InitializeDropDownLists()
    {
        if (!RequestHelper.IsPostBack())
        {
            AddItemsAllAny(drpTypeSelectInRoles);
            AddItemsAllAny(drpTypeSelectNotInRoles);

            // Init lock account reason filter
            AddItemsFromEnum<UserAccountLockEnum>(drpLockReason, "userlist.account");

            // Initialize privilege level filter
            AddItemsFromEnum<UserPrivilegeLevelEnum>(drpPrivilege);
        }
    }


    /// <summary>
    /// Initializes items in "all/any" dropdown list
    /// </summary>
    /// <param name="dropDownList">Dropdown list to initialize</param>
    private void AddItemsAllAny(CMSDropDownList dropDownList)
    {
        AddItemAll(dropDownList);
        dropDownList.Items.Add(new ListItem(GetString("General.Any"), "##ANY##"));
    }


    /// <summary>
    /// Initializes item "all" in dropdown list
    /// </summary>
    private void AddItemAll(CMSDropDownList dropDownList)
    {
        dropDownList.Items.Add(new ListItem(GetString("General.selectall"), UniGrid.ALL));
    }


    /// <summary>
    /// Initializes dropdown list with enum and (all) options.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="dropDownList">Dropdown list to initialize</param>
    /// <param name="resourcePrefix">The resource prefix used for the item text localization</param>
    private void AddItemsFromEnum<TEnum>(CMSDropDownList dropDownList, string resourcePrefix = null)
    {
        AddItemAll(dropDownList);
        ControlsHelper.FillListControlWithEnum<TEnum>(dropDownList, resourcePrefix);
    }


    /// <summary>
    /// Initializes the layout of the form.
    /// </summary>
    private void InitializeForm()
    {
        // General UI
        pnlSimpleFilter.Visible = !mIsAdvancedMode;
        pnlAdvancedFilter.Visible = mIsAdvancedMode;

        // Checkbox javascript
        string script = "var drpEnabled = document.getElementById('" + drpLockReason.ClientID + "'); if(drpEnabled) {drpEnabled.disabled = !this.checked; if(drpEnabled.disabled){drpEnabled.selectedIndex = 0;}}";
        chkEnabled.Attributes.Add("onclick", script);

        int siteId = QueryHelper.GetInteger("siteid", 0);
        plcSite.Visible = siteId == 0;

        // Rename columns for online users saved in database
        if (SessionInsteadOfUser)
        {
            fltUserName.Column = SESSION_USER_NAME_COLUMN;
            fltNickName.Column = SESSION_NICK_NAME_COLUMN;
            fltFullName.Column = SESSION_FULL_NAME_COLUMN;
            fltEmail.Column = SESSION_EMAIL_COLUMN;
        }
    }


    /// <summary>
    /// Sets correct filter mode flag if filter mode was just changed.
    /// </summary>
    private void SetCorrectFilterMode()
    {
        if (RequestHelper.IsPostBack())
        {
            // Get current event target
            string uniqieId = ValidationHelper.GetString(Request.Params[Page.postEventSourceID], String.Empty);

            // If postback was fired by mode switch, update isAdvancedMode variable
            if (uniqieId == lnkShowAdvancedFilter.UniqueID)
            {
                SetFilterMode(true);
            }
            else if (uniqieId == lnkShowSimpleFilter.UniqueID)
            {
                chkDisplayAnonymous.Checked = DisplayGuestsByDefault;
                SetFilterMode(false);
            }
            else
            {
                SetFilterMode(ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false));
            }
        }
    }


    /// <summary>
    /// Sets the advanced mode.
    /// </summary>
    protected void lnkShowAdvancedFilter_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        grid?.Reset();
        SetFilterMode(true);
    }


    /// <summary>
    /// Sets the simple mode.
    /// </summary>
    protected void lnkShowSimpleFilter_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        grid?.Reset();
        SetFilterMode(false);
    }


    private void SetFilterMode(bool isAdvanced)
    {
        mIsAdvancedMode = isAdvanced;
        ViewState["IsAdvancedMode"] = mIsAdvancedMode;
        pnlSimpleFilter.Visible = !mIsAdvancedMode;
        pnlAdvancedFilter.Visible = mIsAdvancedMode;
    }

    #endregion


    #region "Search methods - where condition"

    /// <summary>
    /// Generates where condition for advanced filter.
    /// </summary>
    private WhereCondition GetAdvancedSearchCondition()
    {
        var whereCondition = new WhereCondition()
            .Where(fltUserName.GetCondition())
            .Where(fltFullName.GetCondition())
            .Where(fltEmail.GetCondition())
            .Where(fltNickName.GetCondition());

        AddRoleCondition(whereCondition);

        if (mShowPrivilegeFilter)
        {
            AddPrivilegeLevelCondition(whereCondition);
        }

        AddAccountLockCondition(whereCondition);
        return whereCondition;
    }


    /// <summary>
    /// Generates where condition for simple filter.
    /// </summary>
    private WhereCondition GetSimpleSearchCondition()
    {
        if (txtSearch.Text == String.Empty)
        {
            // There is no text to search for
            return new WhereCondition();
        }

        // identify operation and prepare search expression
        string searchExpression = txtSearch.Text.Trim();
        QueryOperator queryOperator = GetQueryOperator(searchExpression);
        searchExpression = PrepareSearchedText(searchExpression, queryOperator);

        // Search among either online or persistent users
        string[] columns = SessionInsteadOfUser ? SESSION_USER_COLUMNS : DATABASE_USER_COLUMNS;

        // Assemble the where condition
        WhereCondition whereCondition = new WhereCondition();
        foreach (var column in columns)
        {
            whereCondition
                .Or()
                .Where(column, queryOperator, searchExpression);
        }

        return whereCondition;
    }


    /// <summary>
    /// Prepares <paramref name="searchExpression"/> for further processing based on <paramref name="queryOperator"/>.
    /// </summary>    
    private string PrepareSearchedText(string searchExpression, QueryOperator queryOperator)
    {
        switch (queryOperator)
        {
            case QueryOperator.Equals:
                searchExpression = searchExpression.Trim('"');
                break;
            case QueryOperator.Like:
                searchExpression = $"%{SqlHelper.EscapeLikeText(searchExpression)}%";
                break;
        }

        return searchExpression;
    }


    /// <summary>
    /// Returns query operator depending on whether <see cref="searchExpression"/> is quoted or not.
    /// </summary>
    private QueryOperator GetQueryOperator(string searchExpression)
    {
        // Choose the operator (if surrounded with quotes use '=' operator instead of LIKE)            
        if (searchExpression.StartsWith("\"", StringComparison.OrdinalIgnoreCase) &&
            searchExpression.EndsWith("\"", StringComparison.OrdinalIgnoreCase) &&
            searchExpression.Length > 2)
        {
            return QueryOperator.Equals;
        }

        return QueryOperator.Like;
    }


    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private WhereCondition GenerateWhereCondition()
    {
        // Get mode from view state
        SetCorrectFilterMode();

        // Create first where condition depending on mode
        WhereCondition whereCondition = mIsAdvancedMode ? GetAdvancedSearchCondition() : GetSimpleSearchCondition();

        int siteId = SiteID;
        if (SelectedSite > 0)
        {
            siteId = SelectedSite;
        }

        if (SessionInsteadOfUser)
        {
            AddSessionCondition(ref whereCondition);
        }
        else
        {
            AddUserCondition(ref whereCondition, siteId);
        }

        return whereCondition;
    }


    /// <summary>
    /// Adds session condition to given <see paramref="whereCondition"/>.
    /// </summary>
    private void AddSessionCondition(ref WhereCondition whereCondition)
    {
        var sessionCondition = new WhereCondition();

        if (SelectedSite > 0 && IncludeSiteCondition())
        {
            sessionCondition.WhereEquals("SessionSiteID", SelectedSite);
        }

        if (!DisplayGuests)
        {
            sessionCondition.WhereGreaterThan("SessionUserID", 0);
        }

        if (chkDisplayHidden.Visible && !chkDisplayHidden.Checked)
        {
            sessionCondition.Where(new WhereCondition()
                .WhereEquals("SessionUserIsHidden", 0)
                .Or()
                .WhereNull("SessionUserID"));
        }

        whereCondition = new WhereCondition(whereCondition, sessionCondition);
    }


    /// <summary>
    /// Adds user condition to given <see paramref="whereCondition"/>.
    /// </summary>
    private void AddUserCondition(ref WhereCondition whereCondition, int siteID)
    {
        var userCondition = new WhereCondition().WhereGreaterThan("UserID", 0);

        if (siteID > 0 && IncludeSiteCondition())
        {
            userCondition.WhereIn("UserID", UserSiteInfo.Provider.Get()
                .Column("UserID")
                .WhereEquals("SiteID", siteID));
        }

        if (chkDisplayHidden.Visible && !chkDisplayHidden.Checked)
        {
            userCondition.WhereIn("UserID", UserInfo.Provider.Get()
                .Column("UserID")
                .WhereEquals("UserIsHidden", 0)
                .Or()
                .WhereNull("UserIsHidden"));
        }

        whereCondition = new WhereCondition(whereCondition, userCondition);
    }


    /// <summary>
    /// Adds account lock condition to given <paramref name="whereCondition"/> when necessary.
    /// </summary>
    private void AddAccountLockCondition(WhereCondition whereCondition)
    {
        if (!DisplayUserEnabled)
        {
            return;
        }

        if (chkEnabled.Checked)
        {
            whereCondition.WhereEquals("UserEnabled", 0);

            int lockReason = ValidationHelper.GetInteger(drpLockReason.SelectedValue, -1);
            if (lockReason >= 0)
            {
                whereCondition.WhereEquals("ISNULL(UserAccountLockReason, 0)", lockReason);
            }
        }
    }


    /// <summary>
    /// Adds role condition to given <paramref name="whereCondition"/>.
    /// </summary>
    private void AddRoleCondition(WhereCondition whereCondition)
    {
        var assignedRolesSelectionMode = drpTypeSelectInRoles.SelectedValue;
        var unassignedRolesSelectionMode = drpTypeSelectNotInRoles.SelectedValue;

        var assignedRoles = selectRoleElem.Value.ToString();
        var unassignedRoles = selectNotInRole.Value.ToString();

        whereCondition
            .Where(GetRolesSelectorCondition(assignedRolesSelectionMode, assignedRoles))
            .WhereNot(GetRolesSelectorCondition(unassignedRolesSelectionMode, unassignedRoles));
    }


    /// <summary>
    /// Adds condition based on privilege level dropdown to given <paramref name="whereCondition"/>.
    /// </summary>
    /// <remarks>Logic in this method mirrors <see cref="UserInfo.SiteIndependentPrivilegeLevel"/> behavior to get correct condition.</remarks>
    private void AddPrivilegeLevelCondition(WhereCondition whereCondition)
    {
        string drpPrivilegeSelectedItemValue = drpPrivilege.SelectedItem.Value;
        if (drpPrivilegeSelectedItemValue == UniGrid.ALL)
        {
            return;
        }

        var privilegeLevel = drpPrivilegeSelectedItemValue.ToInteger(0);
        whereCondition.WhereEquals("UserPrivilegeLevel", privilegeLevel);
    }


    /// <summary>
    /// Returns where condition for specialized role conditions or <c>null</c> in case no roles were selected.
    /// </summary>
    /// <param name="selector">Condition to use (ANY/ALL)</param>
    /// <param name="selectedRoles">Values separated with semicolon</param>
    /// <remarks>
    /// <c>null</c> is returned in order to allow calling <see cref="WhereConditionBase{TParent}.WhereNot"/> on method's result 
    /// (empty <see cref="WhereCondition"/> would cause appending "NOT" to SQL query).
    /// </remarks>
    private WhereCondition GetRolesSelectorCondition(string selector, string selectedRoles)
    {
        if (String.IsNullOrEmpty(selectedRoles))
        {
            return null;
        }

        string[] roles = selectedRoles.Split(';');

        var globalRolesCondition = GetGlobalRolesCondition(roles);

        var roleCondition = GetSiteRolesCondition(roles).Or(globalRolesCondition);

        var query = UserRoleInfo.Provider.Get()
            .Column("UserID")
            .WhereIn("RoleID", RoleInfo.Provider.Get()
                .Column("RoleID")
                .Where(roleCondition))
            .Where(new WhereCondition()
                .WhereNull("ValidTo")
                .Or()
                .WhereGreaterThan("ValidTo", DateTime.Now))
            .GroupBy("UserID");

        if (selector.Equals(UniGrid.ALL, StringComparison.OrdinalIgnoreCase))
        {
            query.Having(condition => condition.WhereEquals(new CountColumn("RoleID"), roles.Length));
        }

        var userIdColumn = SessionInsteadOfUser ? "SessionUserID" : "UserID";

        return new WhereCondition().WhereIn(userIdColumn, query);
    }


    /// <summary>
    /// Returns <see cref="WhereCondition"/> for filtering global roles.
    /// </summary>
    /// <param name="roles">Role names</param>
    private WhereCondition GetGlobalRolesCondition(string[] roles)
    {
        var globalRolesCondition = new WhereCondition();
        var globalRoles = roles.Where(item => item.StartsWith(".", StringComparison.OrdinalIgnoreCase)).Select(item => item.TrimStart('.')).ToArray();

        if (!globalRoles.Any())
        {
            return globalRolesCondition;
        }

        return globalRolesCondition.WhereIn("RoleName", globalRoles).WhereNull("SiteID");
    }


    /// <summary>
    /// Returns <see cref="WhereCondition"/> for filtering site roles.
    /// </summary>
    /// <param name="roles">Role names</param>
    private WhereCondition GetSiteRolesCondition(string[] roles)
    {
        var siteRolesCondition = new WhereCondition();
        var siteRoles = roles.Where(item => !item.StartsWith(".", StringComparison.OrdinalIgnoreCase)).ToArray();

        if (!siteRoles.Any())
        {
            return siteRolesCondition;
        }

        if (SelectedSite > 0)
        {
            siteRolesCondition.WhereEquals("SiteID", SelectedSite);
        }

        return siteRolesCondition.WhereIn("RoleName", siteRoles);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns true, if site condition should be included in the filter.
    /// Site condition should not be applied, if the user is global admin as well as the filter is placed 
    /// in online marketing UI within 'Users' application with 'sharing site accounts amongst sites' setting turned on.
    /// </summary>
    private bool IncludeSiteCondition()
    {
        return !(IsOnlineUsersUI() &&
                (CMSActionContext.CurrentUser?.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) ?? false) &&
                SettingsKeyInfoProvider.GetBoolValue("CMSSiteSharedAccounts"));
    }


    /// <summary>
    /// Returns true, if the filter is placed in Online Users UI.
    /// </summary>
    private bool IsOnlineUsersUI()
    {
        return ONLINE_USERS_MODE.Equals(CurrentMode, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}