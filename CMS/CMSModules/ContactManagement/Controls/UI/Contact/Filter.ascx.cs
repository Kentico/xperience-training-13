using System;
using System.Web.UI.WebControls;

using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.UIControls;

using TextSimpleFilter = CMSAdminControls_UI_UniGrid_Filters_TextSimpleFilter;

public partial class CMSModules_ContactManagement_Controls_UI_Contact_Filter : CMSAbstractBaseFilterControl
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
    /// Indicates if control is placed on live site.
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
            fltContactStatus.IsLiveSite = value;
        }
    }

    #endregion


    #region "Page methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        fltPhone.Columns = new [] { "ContactMobilePhone", "ContactBusinessPhone" };
        
        HideUiPartsNotRelevantInLowerEditions();
        if (!RequestHelper.IsPostBack())
        {
            radSalesForceLeadReplicationStatus.Items.Clear();
            radSalesForceLeadReplicationStatus.Items.Add(GetString("general.all"));
            radSalesForceLeadReplicationStatus.Items.Add(GetString("om.salesforceleadreplicationstatus.replicated"));
            radSalesForceLeadReplicationStatus.Items.Add(GetString("om.salesforceleadreplicationstatus.notreplicated"));
            radSalesForceLeadReplicationStatus.SelectedIndex = 0;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ShowFilterElements();
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

    private void HideUiPartsNotRelevantInLowerEditions()
    {
        var isFullContactManagementAvailable = LicenseKeyInfoProvider.IsFeatureAvailable(RequestContext.CurrentDomain, FeatureEnum.FullContactManagement);
        fltContactStatus.StopProcessing = !isFullContactManagementAvailable;
        plcFullContactProfile.Visible = isFullContactManagementAvailable;
        plcMiddleFullContactProfile.Visible = isFullContactManagementAvailable;
        plcAdvancedFullContactProfile.Visible = isFullContactManagementAvailable;
        plcMiddle.Visible = isFullContactManagementAvailable;
    }


    private void EnsureCorrectToggleFilterText()
    {
        lnkToggleFilter.Text = IsAdvancedMode ? GetString("general.displaysimplefilter") : GetString("general.displayadvancedfilter");
    }


    /// <summary>
    /// Shows/hides all elements for advanced or simple mode.
    /// </summary>
    private void ShowFilterElements()
    {
        plcAdvancedSearch.Visible = IsAdvancedMode;
        plcMiddle.Visible = IsAdvancedMode;
    }

    #endregion


    #region "Search methods - where condition"

    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        var whereCondition = new WhereCondition();

        whereCondition = whereCondition
            .Where(fltContactStatus.GetWhereCondition())
            .Where(fltFirstName.GetCondition())
            .Where(fltLastName.GetCondition())
            .Where(fltEmail.GetCondition());
        
        // Only contacts that were replicated to SalesForce leads
        if (radSalesForceLeadReplicationStatus.SelectedIndex == 1)
        {
            whereCondition = whereCondition.WhereNotNull("ContactSalesForceLeadID");
        }
        // Only contacts that were not replicated to SalesForce leads
        else if (radSalesForceLeadReplicationStatus.SelectedIndex == 2)
        {
            whereCondition = whereCondition.WhereNull("ContactSalesForceLeadID");
        }

        // Create WHERE condition for advanced filter (id needed)
        if (IsAdvancedMode)
        {
            whereCondition = whereCondition
               .Where(fltMiddleName.GetCondition())
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

        // Query with ContactInfo context has to be used in order to be able to determine DB context of the query (otherwise the materialization would not perform).
        var query = ContactInfo.Provider.Get()
            .WhereIn("ContactCountryID", CountryInfo.Provider
                .Get()
                .Where(originalQuery)
                .Column(CountryInfo.TYPEINFO.IDColumn)
            );

        if (filter.FilterOperator == WhereBuilder.NOT_LIKE || filter.FilterOperator == WhereBuilder.NOT_EQUAL)
        {
            query = query.Or(new WhereCondition().WhereNull("ContactCountryID"));
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

        // Query with ContactInfo context has to be used in order to be able to determine DB context of the query (otherwise the materialization would not perform).
        var query = ContactInfo.Provider.Get()
            .WhereIn("ContactOwnerUserID", UserInfo.Provider
                .Get()
                .Where(originalQuery)
                .Column(UserInfo.TYPEINFO.IDColumn)
            );

        if (filter.FilterOperator == WhereBuilder.NOT_LIKE || filter.FilterOperator == WhereBuilder.NOT_EQUAL)
        {
            query = query.Or(new WhereCondition().WhereNull("ContactOwnerUserID"));
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

        // Query with ContactInfo context has to be used in order to be able to determine DB context of the query (otherwise the materialization would not perform).
        var query = ContactInfo.Provider.Get()
            .WhereIn("ContactStateID", StateInfo.Provider
                .Get()
                .Where(originalQuery)
                .Column(StateInfo.TYPEINFO.IDColumn)
            );

        if (filter.FilterOperator == WhereBuilder.NOT_LIKE || filter.FilterOperator == WhereBuilder.NOT_EQUAL)
        {
            query = query.Or(new WhereCondition().WhereNull("ContactStateID"));
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
        fltFirstName.ResetFilter();
        fltMiddleName.ResetFilter();
        fltLastName.ResetFilter();
        fltEmail.ResetFilter();
        fltContactStatus.Value = UniSelector.US_ALL_RECORDS;
        fltPhone.ResetFilter();
        fltOwner.ResetFilter();
        fltCountry.ResetFilter();
        fltState.ResetFilter();
        fltCity.ResetFilter();
        radSalesForceLeadReplicationStatus.SelectedIndex = 0;
        fltCreated.Clear();
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("radSalesForceLeadReplicationStatus", radSalesForceLeadReplicationStatus.SelectedValue);
        state.AddValue("AdvancedMode", IsAdvancedMode);
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
        radSalesForceLeadReplicationStatus.SelectedValue = state.GetString("radSalesForceLeadReplicationStatus");
        IsAdvancedMode = state.GetBoolean("AdvancedMode");
        fltCreated.ValueFromTime = state.GetDateTime("FromTime");
        fltCreated.ValueToTime = state.GetDateTime("ToTime");
    }

    #endregion
}