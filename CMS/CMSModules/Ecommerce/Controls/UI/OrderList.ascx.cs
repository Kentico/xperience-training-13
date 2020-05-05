using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Controls_UI_OrderList : CMSAdminListControl
{
    #region "Variables"

    private int customerId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerId", 0);

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.WhereCondition = GetWhereCondition();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (customerId > 0)
        {
            gridElem.NamedColumns["Customer"].Visible = false;
        }

        gridElem.NamedColumns["OrderPrice"].Visible = ECommerceContext.MoreCurrenciesUsedOnSite;
    }

    #endregion


    #region "Event handlers"

    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView dr = null;

        switch (sourceName.ToLowerInvariant())
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

            case "grandtotalinmaincurrency":
                dr = (DataRowView)parameter;
                var grandTotalInMainCurrency = ValidationHelper.GetDecimal(dr["OrderGrandTotalInMainCurrency"], 0);
                int siteId = SiteContext.CurrentSiteID;

                // Format currency
                var priceInMainCurrencyFormatted = CurrencyInfoProvider.GetFormattedPrice(grandTotalInMainCurrency, siteId);

                return HTMLHelper.HTMLEncode(priceInMainCurrencyFormatted);

            case "grandtotalinordercurrency":
                dr = (DataRowView)parameter;
                int currencyId = ValidationHelper.GetInteger(dr["OrderCurrencyID"], 0);
                var currency = CurrencyInfo.Provider.Get(currencyId);

                // If order is not in main currency, show order price
                if ((currency != null) && !currency.CurrencyIsMain)
                {
                    var grandTotal = ValidationHelper.GetDecimal(dr["OrderGrandTotal"], 0);
                    var priceFormatted = currency.FormatPrice(grandTotal);

                    // Formatted currency
                    return HTMLHelper.HTMLEncode(priceFormatted);
                }
                return string.Empty;

            case "note":
                string note = ValidationHelper.GetString(parameter, "");

                // Display link, note is in tooltip
                if (!string.IsNullOrEmpty(note))
                {
                    return "<a>" + GetString("general.view") + "</a>";
                }
                return parameter;

            case "statusname":
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
        }
        return parameter;
    }


    /// <summary>
    /// Handles the grid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int orderId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerInvariant())
        {
            case "edit":
                string redirectToUrl = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "OrderProperties", false, orderId);
                if (customerId > 0)
                {
                    redirectToUrl += "&customerId=" + customerId;
                }
                URLHelper.Redirect(redirectToUrl);
                break;

            case "previous":
            {
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
                {
                    AccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
                }

                var oi = OrderInfo.Provider.Get(orderId);
                if (oi != null)
                {
                    var osi = OrderStatusInfoProvider.GetPreviousEnabledStatus(oi.OrderStatusID);
                    if (osi != null)
                    {
                        oi.OrderStatusID = osi.StatusID;
                        // Save order status changes
                        OrderInfo.Provider.Set(oi);
                    }
                }

                break;
            }

            case "next":
            {
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
                {
                    AccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
                }

                var oi = OrderInfo.Provider.Get(orderId);
                if (oi != null)
                {
                    var osi = OrderStatusInfoProvider.GetNextEnabledStatus(oi.OrderStatusID);
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
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Creates where condition based on query string.
    /// </summary>
    private string GetWhereCondition()
    {
        string where = "";
        if (customerId > 0)
        {
            where = "OrderCustomerID = " + customerId;
        }

        return where;
    }

    #endregion
}