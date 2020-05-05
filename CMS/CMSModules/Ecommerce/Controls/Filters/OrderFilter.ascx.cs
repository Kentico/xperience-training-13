using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Ecommerce;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine.Query;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;


/// <summary>
/// Enables the user to specify filter criteria to filter the order list.
/// </summary>
public partial class CMSModules_Ecommerce_Controls_Filters_OrderFilter : CMSAbstractDataFilterControl
{
    #region "Private properties"

    /// <summary>
    /// Gets the customer identifier from the context.
    /// </summary>
    /// <remarks>
    /// The presence of the customer identifier affects the control's appearance.
    /// </remarks>
    private int FilterCustomerID => QueryHelper.GetInteger("customerId", 0);


    /// <summary>
    /// Gets the site identifier for filtering.
    /// </summary>
    /// <remarks>
    /// The site identifier is always the current site id.
    /// </remarks>
    private int FilterSiteID => SiteContext.CurrentSiteID;


    /// <summary>
    /// Gets or sets a value indicating whether filtering by customer's properties is enabled.
    /// </summary>
    private bool CustomerFilterEnabled
    {
        get
        {
            return plcCustomerFilter.Visible;
        }
        set
        {
            plcCustomerFilter.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the advanced filter is displayed or not. 
    /// </summary>
    private bool AdvancedFilterEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["AdvancedFilterEnabled"], false);
        }
        set
        {
            ViewState["AdvancedFilterEnabled"] = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the SQL condition for filtering the order list.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetFilterWhereCondition().ToString(true);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Retrieve customer from the database
        CustomerInfo customer = CustomerInfo.Provider.Get(FilterCustomerID);

        // If a valid customer is specified, disable filtering by customer's properties
        if (customer != null)
        {
            CustomerFilterEnabled = false;
        }

        // Initialize the control with values All, Yes and No
        InitializeThreeStateDropDownList(drpOrderIsPaid);

        // If this control is associated with an UniGrid control, disable UniGrid's buttons and initialize the Reset button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.HideFilterButton = true;
            // Reset button is available only when UniGrid remembers its state
            if (grid.RememberState)
            {
                btnReset.Text = GetString("general.reset");
                btnReset.Click += btnReset_Click;
            }
            else
            {
                btnReset.Visible = false;
            }
        }

        // Initialize the Show button
        btnFilter.Text = GetString("general.filter");
        btnFilter.Click += btnFilter_Click;

        // Hide unwanted elements
        plcAdvancedGroup.Visible = false;
        plcSimpleFilter.Visible = false;

        // Use timezones for DateTimePickers
        TimeZoneInfo timeZone = TimeZoneHelper.GetTimeZoneInfo(MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
        dtpFrom.TimeZone = TimeZoneTypeEnum.Custom;
        dtpFrom.CustomTimeZone = timeZone;
        dtpCreatedTo.TimeZone = TimeZoneTypeEnum.Custom;
        dtpCreatedTo.CustomTimeZone = timeZone;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        AdvancedFilterShowHide();
    }

    #endregion


    #region "Event handlers"

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        ApplyFilter(sender, e);
    }


    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        grid?.Reset();
    }


    protected void btnAdvancedFilter_Click(object sender, EventArgs e)
    {
        AdvancedFilterEnabled = !AdvancedFilterEnabled;
        ApplyFilter(sender, e);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Applies filter to unigrid.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event args.</param>
    private void ApplyFilter(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        grid?.ApplyFilter(sender, e);
    }


    /// <summary>
    /// Initializes the specified controls with values All, Yes and No.
    /// </summary>
    /// <param name="control">A control to initialize.</param>
    private void InitializeThreeStateDropDownList(CMSDropDownList control)
    {
        control.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        control.Items.Add(new ListItem(GetString("general.yes"), "1"));
        control.Items.Add(new ListItem(GetString("general.no"), "0"));
    }


    /// <summary>
    /// Builds a SQL condition for filtering the order list, and returns it.
    /// </summary>
    /// <returns>A SQL condition for filtering the order list.</returns>
    private WhereCondition GetFilterWhereCondition()
    {
        var condition = new WhereCondition();

        // Order status
        if (statusSelector.SelectedID > 0)
        {
            condition.WhereEquals("OrderStatusID", statusSelector.SelectedID);
        }

        // Order site
        if (FilterSiteID > 0)
        {
            condition.WhereEquals("OrderSiteID", FilterSiteID);
        }

        // Order identifier
        var filterOrderId = txtOrderId.Text.Trim().Truncate(txtOrderId.MaxLength);
        if (filterOrderId != String.Empty)
        {
            var orderId = ValidationHelper.GetInteger(filterOrderId, 0);
            // Include also an invoice number
            condition.Where(w => w.WhereEquals("OrderID", orderId).Or().WhereContains("OrderInvoiceNumber", filterOrderId));
        }

        // Customer's properties, applicable only if a valid customer is not specified
        if (CustomerFilterEnabled)
        {
            // First, Last name and Company search
            var customerNameOrCompanyOrEmail = txtCustomerNameOrCompanyOrEmail.Text.Trim().Truncate(txtCustomerNameOrCompanyOrEmail.MaxLength);
            if (customerNameOrCompanyOrEmail != String.Empty)
            {
                condition.WhereIn("OrderCustomerID", new IDQuery<CustomerInfo>("CustomerID").WhereContains("CustomerFirstName", customerNameOrCompanyOrEmail)
                                                                                .Or()
                                                                                .WhereContains("CustomerLastName", customerNameOrCompanyOrEmail)
                                                                                .Or()
                                                                                .WhereContains("CustomerCompany", customerNameOrCompanyOrEmail)
                                                                                .Or()
                                                                                .WhereContains("CustomerEmail", customerNameOrCompanyOrEmail));
            }
        }
        
        // Order is paid
        switch (drpOrderIsPaid.SelectedValue)
        {
            case "0":
                condition.WhereEquals("OrderIsPaid".AsColumn().IsNull(0), 0);
                break;
            case "1":
                condition.WhereEquals("OrderIsPaid".AsColumn().IsNull(0), 1);
                break;
        }

        // Advanced Filter
        if (AdvancedFilterEnabled)
        {
            // Specific currency was selected
            if (CurrencySelector.SelectedID > 0)
            {
                condition.WhereEquals("OrderCurrencyID", CurrencySelector.SelectedID);
            }

            // Specific payment method was selected
            if (PaymentSelector.SelectedID > 0)
            {
                condition.WhereEquals("OrderPaymentOptionID", PaymentSelector.SelectedID);
            }

            // Specific shipping option was selected
            if (ShippingSelector.SelectedID > 0)
            {
                condition.WhereEquals("OrderShippingOptionID", ShippingSelector.SelectedID);
            }

            // Specific tracking number was provided
            var trackingNumber = txtTrackingNumber.Text.Trim().Truncate(txtTrackingNumber.MaxLength);
            if (trackingNumber != String.Empty)
            {
                condition.WhereContains("OrderTrackingNumber", trackingNumber);
            }

            // Specific created date was selected
            if ((dtpCreatedTo.SelectedDateTime < dtpCreatedTo.MaxDate) && (dtpCreatedTo.SelectedDateTime > dtpCreatedTo.MinDate))
            {
                condition.WhereLessOrEquals("OrderDate", dtpCreatedTo.SelectedDateTime);
            }

            // Specific from date was selected
            if ((dtpFrom.SelectedDateTime < dtpFrom.MaxDate) && (dtpFrom.SelectedDateTime > dtpFrom.MinDate))
            {
                condition.WhereGreaterOrEquals("OrderDate", dtpFrom.SelectedDateTime);
            }
        }

        return condition;
    }


    /// <summary>
    /// Reloads selectors.
    /// </summary>
    private void ReloadUniselectors()
    {
        CurrencySelector.Reload();
        PaymentSelector.Reload();
        ShippingSelector.Reload();
        statusSelector.Reload();
    }


    /// <summary>
    /// Resets selectors.
    /// </summary>
    private void ResetSelectors()
    {
        CurrencySelector.SelectedID = -1;
        PaymentSelector.SelectedID = -1;
        ShippingSelector.SelectedID = -1;
        statusSelector.SelectedID = -1;
        drpOrderIsPaid.SelectedIndex = 0;
    }


    /// <summary>
    /// Change visibility of the filter
    /// </summary>
    private void AdvancedFilterShowHide()
    {
        plcAdvancedFilter.Visible = !AdvancedFilterEnabled;
        plcSimpleFilter.Visible = AdvancedFilterEnabled;

        plcAdvancedGroup.Visible = AdvancedFilterEnabled;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);

        // Store additional state properties
        state.AddValue("AdvancedFilterEnabled", AdvancedFilterEnabled);
        state.AddValue("dtpFrom", dtpFrom.SelectedDateTime);
        state.AddValue("dtpCreatedTo", dtpCreatedTo.SelectedDateTime);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);

        // Restore additional state properties
        AdvancedFilterEnabled = state.GetBoolean("AdvancedFilterEnabled");
        dtpFrom.SelectedDateTime = state.GetDateTime("dtpFrom");
        dtpCreatedTo.SelectedDateTime = state.GetDateTime("dtpCreatedTo");
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtOrderId.Text = String.Empty;
        txtCustomerNameOrCompanyOrEmail.Text = String.Empty;
        statusSelector.Value = null;
        dtpFrom.DateTimeTextBox.Text = string.Empty;
        dtpCreatedTo.DateTimeTextBox.Text = string.Empty;
        txtTrackingNumber.Text = string.Empty;
        ResetSelectors();
        ReloadUniselectors();
    }

    #endregion
}