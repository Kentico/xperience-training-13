using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;


public partial class CMSWebParts_Ecommerce_Orders : CMSAbstractWebPart
{
    #region "Constants"

    private const int SQL_DATE_TIME_LIMIT = 73000;

    private const string PAY_STATUS_NOT_PAID = "0";
    private const string PAY_STATUS_PAID = "1";

    private const string ORDERBY_DATE_DESC = "OrderDate DESC";
    private const string ORDERBY_DATE_ASC = "OrderDate ASC";
    private const string ORDERBY_PRICE_DESC = "OrderTotalPriceInMainCurrency ASC";
    private const string ORDERBY_PRICE_ASC = "OrderTotalPriceInMainCurrency DESC";

    private const string ACTION_EDIT = "edit";
    private const string ACTION_DELETE = "delete";
    private const string ACTION_STATUS_MOVE_PREVIOUS = "statusmoveprevious";
    private const string ACTION_STATUS_MOVE_NEXT = "statusmovenext";
    private const string ACTIONS_STATUS_MOVE = "statusmove";

    private const string COLUMN_ID_AND_INVOICE = "IDANDINVOICE";
    private const string COLUMN_CUSTOMER = "CUSTOMER";
    private const string COLUMN_DATE = "DATE";
    private const string COLUMN_PRICE = "PRICE";
    private const string COLUMN_STATUS = "STATUS";
    private const string COLUMN_PAYMENT = "PAYMENT";
    private const string COLUMN_IS_PAID = "ISPAID";
    private const string COLUMN_SHIPPING = "SHIPPING";
    private const string COLUMN_TRACKING_NUMBER = "TRACKINGNUMBER";
    private const string COLUMN_NOTE = "NOTE";

    #endregion


    #region "Variables"

    private readonly List<string> visibleActionsList = new List<string>()
        {
            ACTION_EDIT,
            ACTION_DELETE,
            ACTIONS_STATUS_MOVE
        };

    #endregion


    #region "Properties"

    /// <summary>
    /// Order status.
    /// </summary>
    public string OrderStatus
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderStatus"), "");
        }
        set
        {
            SetValue("OrderStatus", value);
        }
    }


    /// <summary>
    /// Customer or company like.
    /// </summary>
    public string CustomerOrCompany
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CustomerOrCompany"), "");
        }
        set
        {
            SetValue("CustomerOrCompany", value);
        }
    }


    /// <summary>
    /// Orders with note only.
    /// </summary>
    public bool HasNote
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HasNote"), false);
        }
        set
        {
            SetValue("HasNote", value);
        }
    }


    /// <summary>
    /// Payment method.
    /// </summary>
    public string PaymentMethod
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PaymentMethod"), "");
        }
        set
        {
            SetValue("PaymentMethod", value);
        }
    }


    /// <summary>
    /// Payment status.
    /// </summary>
    public string PaymentStatus
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PaymentStatus"), "all");
        }
        set
        {
            SetValue("PaymentStatus", value);
        }
    }


    /// <summary>
    /// Currency.
    /// </summary>
    public string Currency
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Currency"), "");
        }
        set
        {
            SetValue("Currency", value);
        }
    }


    /// <summary>
    /// Total price in main currency from.
    /// </summary>
    public decimal MinPriceInMainCurrency
    {
        get
        {
            return ValidationHelper.GetDecimalSystem(GetValue("MinPriceInMainCurrency"), 0);
        }
        set
        {
            SetValue("MinPriceInMainCurrency", value);
        }
    }


    /// <summary>
    /// Total price in main currency to.
    /// </summary>
    public decimal MaxPriceInMainCurrency
    {
        get
        {
            return ValidationHelper.GetDecimalSystem(GetValue("MaxPriceInMainCurrency"), 0);
        }
        set
        {
            SetValue("MaxPriceInMainCurrency", value);
        }
    }


    /// <summary>
    /// Shipping option.
    /// </summary>
    public string ShippingOption
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ShippingOption"), "");
        }
        set
        {
            SetValue("ShippingOption", value);
        }
    }


    /// <summary>
    /// Shipping country.
    /// </summary>
    public string ShippingCountry
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ShippingCountry"), "");
        }
        set
        {
            SetValue("ShippingCountry", value);
        }
    }


    /// <summary>
    /// How old orders.
    /// </summary>
    public int HowOld
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("HowOld"), 365);
        }
        set
        {
            SetValue("HowOld", value);
        }
    }


    /// <summary>
    /// Older than (days).
    /// </summary>
    public int OlderThan
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("OlderThan"), 0);
        }
        set
        {
            SetValue("OlderThan", value);
        }
    }


    /// <summary>
    /// If you specify this number, paging will be disabled.
    /// </summary>
    public int TopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("TopN"), 0);
        }
        set
        {
            SetValue("TopN", value);
        }
    }


    /// <summary>
    /// Sort orders by.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "");
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 0);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }


    /// <summary>
    /// Visible columns in listing.
    /// </summary>
    public string VisibleColumns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("VisibleColumns"), "");
        }
        set
        {
            SetValue("VisibleColumns", value);
        }
    }


    /// <summary>
    /// Visible order actions.
    /// </summary>
    public string VisibleActions
    {
        get
        {
            return ValidationHelper.GetString(GetValue("VisibleActions"), "");
        }
        set
        {
            SetValue("VisibleActions", value);
        }
    }


    /// <summary>
    /// Gets the messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // If no action is visible, hides actions column
        if (visibleActionsList.Count <= 0)
        {
            gridElem.GridView.Columns[0].Visible = false;
        }

        DisplayColumns();
    }

    #endregion


    #region "Event handlers"

    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView dr;

        switch (sourceName.ToLowerCSafe())
        {
            case "idandinvoice":
                dr = (DataRowView)parameter;
                int orderId = ValidationHelper.GetInteger(dr["OrderID"], 0);
                string invoiceNumber = ValidationHelper.GetString(dr["OrderInvoiceNumber"], "");

                // Show OrderID and invoice number in brackets if InvoiceNumber is different from OrderID
                if (!string.IsNullOrEmpty(invoiceNumber) && (invoiceNumber != orderId.ToString()))
                {
                    return HTMLHelper.HTMLEncode(orderId + " (" + invoiceNumber + ")");
                }
                return orderId;

            case "customer":
                dr = (DataRowView)parameter;
                string customerName = dr["CustomerFirstName"] + " " + dr["CustomerLastName"];
                string customerCompany = ValidationHelper.GetString(dr["CustomerCompany"], "");

                // Show customer name and company in brackets, if company specified
                if (!string.IsNullOrEmpty(customerCompany))
                {
                    return HTMLHelper.HTMLEncode(customerName + " (" + customerCompany + ")");
                }
                return HTMLHelper.HTMLEncode(customerName);

            case "totalpriceinmaincurrency":
                dr = (DataRowView)parameter;
                var totalPriceInMainCurrency = ValidationHelper.GetDecimalSystem(dr["OrderTotalPriceInMainCurrency"], 0);
                int siteId = ValidationHelper.GetInteger(dr["OrderSiteID"], 0);

                // Format currency
                var priceInMainCurrencyFormatted = CurrencyInfoProvider.GetFormattedPrice(totalPriceInMainCurrency, siteId);

                return HTMLHelper.HTMLEncode(priceInMainCurrencyFormatted);

            case "totalpriceinorderprice":
                dr = (DataRowView)parameter;
                int currencyId = ValidationHelper.GetInteger(dr["OrderCurrencyID"], 0);
                var currency = CurrencyInfo.Provider.Get(currencyId);

                // If order is not in main currency, show order price
                if ((currency != null) && !currency.CurrencyIsMain)
                {
                    var orderTotalPrice = ValidationHelper.GetDecimalSystem(dr["OrderTotalPrice"], 0);
                    var priceFormatted = currency.FormatPrice(orderTotalPrice);

                    // Formatted currency
                    return HTMLHelper.HTMLEncode(priceFormatted);
                }
                return string.Empty;

            case "note":
                string note = ValidationHelper.GetString(parameter, "");

                // Display link, note is in tooltip
                if (!string.IsNullOrEmpty(note))
                {
                    return "<span style=\"text-decoration: underline\">" + GetString("general.view") + "</span>";
                }
                return parameter;

            case "orderstatus":
                var statusId = ValidationHelper.GetInteger(parameter, 0);
                var status = OrderStatusInfo.Provider.Get(statusId);
                if (status != null)
                {
                    return new Tag
                           {
                               Text = status.StatusDisplayName,
                               Color = status.StatusColor
                           };
                }

                return string.Empty;

            case ACTION_EDIT:
                ShowOrHideAction(sender, ACTION_EDIT);
                break;

            case ACTION_DELETE:
                ShowOrHideAction(sender, ACTION_DELETE);
                break;

            case ACTION_STATUS_MOVE_PREVIOUS:
            case ACTION_STATUS_MOVE_NEXT:
                ShowOrHideAction(sender, ACTIONS_STATUS_MOVE);
                break;
        }
        return parameter;
    }


    /// <summary>
    /// Handles the gridElem's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int orderId = ValidationHelper.GetInteger(actionArgument, 0);
        OrderInfo oi;
        OrderStatusInfo osi;

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                string redirectToUrl = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "orderproperties", false, orderId);
                URLHelper.Redirect(redirectToUrl);
                break;

            case "delete":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
                {
                    return;
                }

                // Delete OrderInfo object from database
                OrderInfo.Provider.Delete(OrderInfo.Provider.Get(orderId));
                break;

            case "previous":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
                {
                    return;
                }

                oi = OrderInfo.Provider.Get(orderId);
                if (oi != null)
                {
                    osi = OrderStatusInfoProvider.GetPreviousEnabledStatus(oi.OrderStatusID);
                    if (osi != null)
                    {
                        oi.OrderStatusID = osi.StatusID;
                        // Save order status changes
                        OrderInfo.Provider.Set(oi);
                    }
                }
                break;

            case "next":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
                {
                    return;
                }

                oi = OrderInfo.Provider.Get(orderId);
                if (oi != null)
                {
                    osi = OrderStatusInfoProvider.GetNextEnabledStatus(oi.OrderStatusID);
                    if (osi != null)
                    {
                        oi.OrderStatusID = osi.StatusID;
                        // Save order status changes
                        OrderInfo.Provider.Set(oi);
                    }
                }
                break;
        }
    }

    #endregion


    #region "Methods - WebPart"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();

        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_READ))
        {
            ShowError(String.Format(GetString("CMSMessages.AccessDeniedResource"), "EcommerceRead OR ReadOrders"));
            gridElem.Visible = false;
            return;
        }

        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        var where = GetWhereCondition();

        // Assign filter to unigrid
        gridElem.WhereCondition = where.ToString(false);
        gridElem.QueryParameters = where.Parameters;
        gridElem.OrderBy = GetOrderBy();
        SetTopN();
        gridElem.Pager.DefaultPageSize = GetPageSize();
    }

    #endregion


    #region "Methods - private"

    /// <summary>
    /// Hides specific order action (edit, delete, ...) from unigrid, if it should be hidden.
    /// </summary>
    /// <param name="sender">Sender from unigrid's ExternalDataBound.</param>
    /// <param name="actionName">Name of action to show/hide.</param>
    private void ShowOrHideAction(object sender, string actionName)
    {
        string[] visibleActionsArray = VisibleActions.Split('|');
        bool hideAction = true;

        // Determine, if action should be shown or hidden
        foreach (var action in visibleActionsArray)
        {
            if (action == actionName)
            {
                hideAction = false;
            }
        }
        // Do not hide only if user has permissions
        if (!hideAction && ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
        {
            return;
        }

        // Hiding
        CMSGridActionButton btn = sender as CMSGridActionButton;
        if (btn != null)
        {
            btn.Visible = false;
            visibleActionsList.Remove(actionName);
        }
    }


    /// <summary>
    /// Displays or hides columns based on VisibleColumns property.
    /// </summary>
    private void DisplayColumns()
    {
        string[] visibleColumns = VisibleColumns.Split('|');

        // Hide all first
        foreach (var item in gridElem.NamedColumns.Values)
        {
            item.Visible = false;
        }

        // Show columns that should be visible
        foreach (var item in visibleColumns)
        {
            string key = null;
            switch (item)
            {
                case COLUMN_ID_AND_INVOICE:
                    key = "IDAndInvoice";
                    break;

                case COLUMN_CUSTOMER:
                    key = "Customer";
                    break;

                case COLUMN_DATE:
                    key = "Date";
                    break;

                case COLUMN_PRICE:
                    key = "MainCurrencyPrice";
                    gridElem.NamedColumns["OrderPrice"].Visible = ECommerceContext.MoreCurrenciesUsedOnSite;
                    break;

                case COLUMN_STATUS:
                    key = "OrderStatus";
                    break;

                case COLUMN_PAYMENT:
                    key = "PaymentOption";
                    break;

                case COLUMN_IS_PAID:
                    key = "IsPaid";
                    break;

                case COLUMN_SHIPPING:
                    key = "ShippingOption";
                    break;

                case COLUMN_TRACKING_NUMBER:
                    key = "TrackingNumber";
                    break;

                case COLUMN_NOTE:
                    key = "Note";
                    break;
            }

            // Show column
            if (key != null)
            {
                gridElem.NamedColumns[key].Visible = true;
            }
        }
    }


    /// <summary>
    /// Returns where condition based on webpart fields.
    /// </summary>
    private WhereCondition GetWhereCondition()
    {
        // Orders from current site
        var where = new WhereCondition()
            .WhereEquals("OrderSiteID", SiteContext.CurrentSiteID);

        // Order status filter
        var status = OrderStatusInfo.Provider.Get(OrderStatus, SiteContext.CurrentSiteID);
        if (status != null)
        {
            where.WhereEquals("OrderStatusID", status.StatusID);
        }

        // Customer or company like filter
        if (!string.IsNullOrEmpty(CustomerOrCompany))
        {
            where.WhereIn("OrderCustomerID", new IDQuery<CustomerInfo>()
                .Where("CustomerFirstName + ' ' + CustomerLastName + ' ' + CustomerFirstName LIKE N'%"+ SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(CustomerOrCompany)) + "%'")
                .Or()
                .WhereContains("CustomerCompany", CustomerOrCompany));
        }

        // Filter for orders with note
        if (HasNote)
        {
            where.WhereNotEmpty("OrderNote");
        }

        // Payment method filter
        var payment = PaymentOptionInfo.Provider.Get(PaymentMethod, SiteInfoProvider.GetSiteID(SiteContext.CurrentSiteName));
        if (payment != null)
        {
            where.WhereEquals("OrderPaymentOptionID", payment.PaymentOptionID);
        }

        // Payment status filter
        switch (PaymentStatus.ToLowerCSafe())
        {
            case PAY_STATUS_NOT_PAID:
                where.Where(new WhereCondition().WhereFalse("OrderIsPaid").Or().WhereNull("OrderIsPaid"));
                break;

            case PAY_STATUS_PAID:
                where.WhereTrue("OrderIsPaid");
                break;
        }

        // Currency filter
        var currencyObj = CurrencyInfo.Provider.Get(Currency, SiteContext.CurrentSiteID);
        if (currencyObj != null)
        {
            where.WhereEquals("OrderCurrencyID", currencyObj.CurrencyID);
        }

        // Min price in main currency filter
        if (MinPriceInMainCurrency > 0)
        {
            where.Where("OrderTotalPriceInMainCurrency", QueryOperator.LargerOrEquals, MinPriceInMainCurrency);
        }

        // Max price in main currency filter
        if (MaxPriceInMainCurrency > 0)
        {
            where.Where("OrderTotalPriceInMainCurrency", QueryOperator.LessOrEquals, MaxPriceInMainCurrency);
        }

        // Shipping option filter
        var shipping = ShippingOptionInfo.Provider.Get(ShippingOption, SiteContext.CurrentSiteID);
        if (shipping != null)
        {
            where.WhereEquals("OrderShippingOptionID", shipping.ShippingOptionID);
        }

        // Shipping country filter
        if (!string.IsNullOrEmpty(ShippingCountry) && ShippingCountry != "0")
        {
            AddCountryWhereCondition(where);
        }

        // Date filter
        AddDateWhereCondition(where);

        return where;
    }


    /// <summary>
    /// Returns where condition filtering ShippingAddress, Country and State.
    /// </summary>
    private void AddCountryWhereCondition(WhereCondition where)
    {
        var addressWhere = new IDQuery<OrderAddressInfo>("AddressOrderID").WhereEquals("AddressType", (int)AddressType.Shipping);

        string[] split = ShippingCountry.Split(';');

        if ((split.Length >= 1) && (split.Length <= 2))
        {
            // Country filter
            var country = CountryInfo.Provider.Get(split[0]);
            if (country != null)
            {
                addressWhere.WhereEquals("AddressCountryID", country.CountryID);

                if (split.Length == 2)
                {
                    // State filter
                    var state = StateInfo.Provider.Get(split[1]);
                    if (state != null)
                    {
                        addressWhere.WhereEquals("AddressStateID", state.StateID);
                    }
                }
            }
        }

        where.WhereIn("OrderID", addressWhere);
    }


    /// <summary>
    /// Returns where condition filtering OlderThan or HowOld.
    /// </summary>
    private void AddDateWhereCondition(WhereCondition where)
    {
        if (OlderThan > 0)
        {
            // OlderThan parameter
            if (OlderThan > SQL_DATE_TIME_LIMIT)
            {
                OlderThan = SQL_DATE_TIME_LIMIT;
            }

            var olderThan = DateTime.Now.AddDays(-Math.Abs(OlderThan));
            where.Where("OrderDate", QueryOperator.LessOrEquals, olderThan);
        }
        else if ((HowOld > 0) && (HowOld < SQL_DATE_TIME_LIMIT))
        {
            // HowOld parameter
            var from = DateTime.Now.AddDays(-Math.Abs(HowOld));
            where.Where("OrderDate", QueryOperator.LargerOrEquals, from);
        }
    }


    /// <summary>
    /// Set TopN property to unigrid and disables Pager, if TopN field is specified.
    /// </summary>
    private void SetTopN()
    {
        if (TopN > 0)
        {
            gridElem.TopN = TopN;
            gridElem.Pager.DisplayPager = false;
        }
    }


    /// <summary>
    /// Returns page size for unigrid.
    /// </summary>
    private int GetPageSize()
    {
        switch (PageSize)
        {
            case -1:
            case 10:
            case 25:
            case 50:
            case 100:
                return PageSize;
            default:
                return 25;
        }
    }


    /// <summary>
    /// Returns string for ORDER BY clause and disables remembering unigrid state.
    /// </summary>
    private string GetOrderBy()
    {
        string orderBy;

        // OrderBy specified by drop-down list
        switch (OrderBy)
        {
            case ORDERBY_DATE_ASC:
            case ORDERBY_PRICE_ASC:
            case ORDERBY_PRICE_DESC:
            case ORDERBY_DATE_DESC:
                orderBy = OrderBy;
                break;

            default:
                orderBy = ORDERBY_DATE_DESC;
                break;
        }

        // Disables remembering unigrid state
        gridElem.RememberState = false;
        return orderBy;
    }

    #endregion
}