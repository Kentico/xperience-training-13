using System;
using System.Web.UI.WebControls;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

using TextSimpleFilter = CMSAdminControls_UI_UniGrid_Filters_TextSimpleFilter;

public partial class CMSModules_ContactManagement_Controls_UI_Account_Filter : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return GenerateWhereCondition();
        }
    }


    /// <summary>
    /// Indicates if AccountStatus filter should be displayed.
    /// </summary>
    public bool DisplayAccountStatus
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if filter is in advanced mode.
    /// </summary>
    protected bool IsAdvancedMode
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false);
        }
        set
        {
            ViewState["IsAdvancedMode"] = value;
        }
    }


    /// <summary>
    /// Gets button used to toggle filter's advanced mode.
    /// </summary>
    public override IButtonControl ToggleAdvancedModeButton
    {
        get
        {
            return lnkToggleFilter;
        }
    }


    /// <summary>
    /// Indicates if  filter is used on live site or in UI.
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
            fltAccountStatus.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether "show children" checkbox should be visible.
    /// </summary>
    public bool ShowChildren
    {
        get;
        set;
    }
    
    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        fltPhone.Columns = new[] { 
            "AccountPhone", 
            "AccountFax" 
        };
        fltContactName.Columns = new[] { 
            "PrimaryContactFullName",
            "SecondaryContactFullName"
        };
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InitializeForm();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        EnsureCorrectToggleFilterText();
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.FilterIsSet = true;
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    protected override void ToggleAdvancedModeButton_Click(object sender, EventArgs e)
    {
        IsAdvancedMode = !IsAdvancedMode;
        ShowFilterElements();

        base.ToggleAdvancedModeButton_Click(sender, e);
    }

    #endregion


    #region "UI methods"

    /// <summary>
    /// Shows/hides all elements for advanced or simple mode.
    /// </summary>
    private void ShowFilterElements()
    {
        plcAdvancedSearch.Visible = IsAdvancedMode;
    }


    /// <summary>
    /// Initializes the layout of the form.
    /// </summary>
    private void InitializeForm()
    {
        // General UI
        fltAccountStatus.AdditionalDropDownCSSClass = "DropDownFieldFilter";
        
        ShowFilterElements();
    }


    private void EnsureCorrectToggleFilterText()
    {
        lnkToggleFilter.Text = IsAdvancedMode ? GetString("general.displaysimplefilter") : GetString("general.displayadvancedfilter");
    }

    #endregion


    #region "Search methods - where condition"

    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        var whereCondition = new WhereCondition();

        // Create WHERE condition for basic filter
        int contactStatus = ValidationHelper.GetInteger(fltAccountStatus.Value, -1);
        if (fltAccountStatus.Value == null)
        {
            whereCondition = whereCondition.WhereNull("AccountStatusID");
        }
        else if (contactStatus > 0)
        {
            whereCondition = whereCondition.WhereEquals("AccountStatusID", contactStatus);
        }

        whereCondition = whereCondition
            .Where(fltName.GetCondition())
            .Where(fltEmail.GetCondition())
            .Where(fltContactName.GetCondition());

        if (IsAdvancedMode)
        {
            whereCondition = whereCondition
                .Where(fltCity.GetCondition())
                .Where(fltPhone.GetCondition())
                .Where(fltCreated.GetCondition())
                .Where(GetOwnerCondition(fltOwner))
                .Where(GetCountryCondition(fltCountry))
                .Where(GetStateCondition(fltState));
        }
        
        return whereCondition.ToString(true);
    }

    #endregion


    #region "Additional filter conditions"

    /// <summary>
    /// Gets Where condition for filtering by the country. When using separated database, materializes the nested query on the main DB.
    /// </summary>
    private string GetCountryCondition(TextSimpleFilter filter)
    {
        string originalQuery = filter.WhereCondition;
        if (String.IsNullOrEmpty(originalQuery))
        {
            return string.Empty;
        }

        // Query with AccountInfo context has to be used in order to be able to determine DB context of the query (otherwise the materialization would not perform).
        var query = AccountInfo.Provider.Get()
            .WhereIn("AccountCountryID", CountryInfo.Provider
                .Get()
                .Where(originalQuery)
                .Column(CountryInfo.TYPEINFO.IDColumn)
            );

        if (filter.FilterOperator == WhereBuilder.NOT_LIKE || filter.FilterOperator == WhereBuilder.NOT_EQUAL)
        {
            query = query.Or(new WhereCondition().WhereNull("AccountCountryID"));
        }

        query.EnsureParameters();
        return query.Parameters.Expand(query.WhereCondition);
    }


    /// <summary>
    /// Gets Where condition for filtering by the user. When using separated database, materializes the nested query on the other DB.
    /// </summary>
    private string GetOwnerCondition(TextSimpleFilter filter)
    {
        string originalQuery = filter.WhereCondition;
        if (String.IsNullOrEmpty(originalQuery))
        {
            return string.Empty;
        }

        // Query with AccountInfo context has to be used in order to be able to determine DB context of the query (otherwise the materialization would not perform).
        var query = AccountInfo.Provider.Get()
            .WhereIn("AccountOwnerUserID", UserInfo.Provider
                .Get()
                .Where(originalQuery)
                .Column(UserInfo.TYPEINFO.IDColumn)
            );

        if (filter.FilterOperator == WhereBuilder.NOT_LIKE || filter.FilterOperator == WhereBuilder.NOT_EQUAL)
        {
            query = query.Or(new WhereCondition().WhereNull("AccountOwnerUserID"));
        }

        query.EnsureParameters();
        return query.Parameters.Expand(query.WhereCondition);
    }


    /// <summary>
    /// Gets Where condition for filtering by the state. When using separated database, materializes the nested query on the other DB.
    /// </summary>
    private string GetStateCondition(TextSimpleFilter filter)
    {
        string originalQuery = filter.WhereCondition;
        if (String.IsNullOrEmpty(originalQuery))
        {
            return string.Empty;
        }

        // Query with AccountInfo context has to be used in order to be able to determine DB context of the query (otherwise the materialization would not perform).
        var query = AccountInfo.Provider.Get()
            .WhereIn("AccountStateID", StateInfo.Provider
                .Get()
                .Where(originalQuery)
                .Column(StateInfo.TYPEINFO.IDColumn)
            );

        if (filter.FilterOperator == WhereBuilder.NOT_LIKE || filter.FilterOperator == WhereBuilder.NOT_EQUAL)
        {
            query = query.Or(new WhereCondition().WhereNull("AccountStateID"));
        }

        query.EnsureParameters();
        return query.Parameters.Expand(query.WhereCondition);
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        fltAccountStatus.Value = UniSelector.US_ALL_RECORDS;
        fltCity.ResetFilter();
        fltContactName.ResetFilter();
        fltCountry.ResetFilter();
        fltCreated.Clear();
        fltEmail.ResetFilter();
        fltName.ResetFilter();
        fltOwner.ResetFilter();
        fltPhone.ResetFilter();
        fltState.ResetFilter();
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("AdvancedMode", IsAdvancedMode);
        state.AddValue("Status", fltAccountStatus.Value);
        state.AddValue("ToTime", fltCreated.ValueToTime);
        state.AddValue("FromTime", fltCreated.ValueFromTime);
        base.StoreFilterState(state);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        IsAdvancedMode = state.GetBoolean("AdvancedMode");
        fltAccountStatus.Value = state.GetString("Status");
        fltCreated.ValueFromTime = state.GetDateTime("FromTime");
        fltCreated.ValueToTime = state.GetDateTime("ToTime");
    }

    #endregion
}