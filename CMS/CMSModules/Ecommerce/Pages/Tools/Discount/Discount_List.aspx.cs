using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.Base;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Pages_Tools_Discount_Discount_List : CMSEcommercePage
{
    #region "Variables & Properties"

    private ObjectTransformationDataProvider couponCountsDataProvider;

    private readonly Hashtable discounts = new Hashtable();


    /// <summary>
    /// Type of discounts which are being listed. By default Order discounts are listed.
    /// </summary>
    private DiscountApplicationEnum DiscountType
    {
        get;
        set;
    }

    #endregion


    #region "Page Events"

    protected override void OnPreInit(EventArgs e)
    {
        SetDiscountType();

        string elementName = "";
        switch (DiscountType)
        {
            case DiscountApplicationEnum.Products:
                elementName = "CatalogDiscounts";
                break;
            case DiscountApplicationEnum.Order:
                elementName = "OrderDiscounts";
                break;
            case DiscountApplicationEnum.Shipping:
                elementName = "ShippingDiscounts";
                break;
        }

        // Check UI personalization
        var uiElement = new UIElementAttribute(ModuleName.ECOMMERCE, elementName);
        uiElement.Check(this);

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        couponCountsDataProvider = new ObjectTransformationDataProvider();
        couponCountsDataProvider.SetDefaultDataHandler(GetCountsDataHandler);

        // Init Unigrid
        SetDiscountStatusFilterType();
        ugDiscounts.OnAction += ugDiscounts_OnAction;
        ugDiscounts.OnExternalDataBound += ugDiscounts_OnExternalDataBound;
        ugDiscounts.GridView.AllowSorting = false;

        ugDiscounts.WhereCondition = new WhereCondition()
            .WhereEquals("DiscountSiteID", SiteID)
            .WhereEquals("DiscountApplyTo", DiscountType.ToStringRepresentation())
            .ToString(true);

        switch (DiscountType)
        {
            case DiscountApplicationEnum.Products:
                AddHeaderAction(new HeaderAction
                {
                    Text = GetString("com.discount.newcatalogdiscount"),
                    RedirectUrl = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "NewCatalogDiscount", false)
                });

                SetTitle(GetString("com.discount.catalogdiscounts"));

                // Set empty grid messages
                ugDiscounts.ZeroRowsText = GetString("com.nocatalogdiscounts");
                ugDiscounts.FilteredZeroRowsText = GetString("com.nocatalogdiscounts.filtered");
                break;

            case DiscountApplicationEnum.Order:
                AddHeaderAction(new HeaderAction
                {
                    Text = GetString("com.discount.neworderdiscount"),
                    RedirectUrl = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "NewOrderDiscount", false)
                });

                SetTitle(GetString("com.discount.orderdiscounts"));

                // Set empty grid messages
                ugDiscounts.ZeroRowsText = GetString("com.noorderdiscounts");
                ugDiscounts.FilteredZeroRowsText = GetString("com.noorderdiscounts.filtered");
                break;

            case DiscountApplicationEnum.Shipping:
                AddHeaderAction(new HeaderAction
                {
                    Text = GetString("com.discount.newshippingdiscount"),
                    RedirectUrl = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "NewShippingDiscount", false)
                });

                SetTitle(GetString("com.discount.shippingdiscounts"));

                // Set empty grid messages
                ugDiscounts.ZeroRowsText = GetString("com.noshippingdiscounts");
                ugDiscounts.FilteredZeroRowsText = GetString("com.noshippingdiscounts.filtered");
                break;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        switch (DiscountType)
        {
            case DiscountApplicationEnum.Products:
                ugDiscounts.NamedColumns["OrderAmount"].Visible = false;
                ugDiscounts.NamedColumns["Application"].Visible = false;
                break;

            case DiscountApplicationEnum.Order:
                break;

            case DiscountApplicationEnum.Shipping:
                ugDiscounts.NamedColumns["ApplyFurtherDiscounts"].Visible = false;
                ugDiscounts.NamedColumns["Value"].Visible = false;
                ugDiscounts.NamedColumns["Order"].Visible = false;
                break;
        }
    }


    #endregion


    #region "Event Handlers"

    protected void ugDiscounts_OnAction(string actionName, object actionArgument)
    {
        if (actionName.ToLowerInvariant() == "edit")
        {
            string elementName = null;
            switch (DiscountType)
            {
                case DiscountApplicationEnum.Products:
                    elementName = "EditCatalogDiscount";
                    break;

                case DiscountApplicationEnum.Order:
                    elementName = "EditOrderDiscount";
                    break;

                case DiscountApplicationEnum.Shipping:
                    elementName = "EditShippingDiscount";
                    break;
            }

            if (!string.IsNullOrEmpty(elementName))
            {
                var url = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, elementName, false, actionArgument.ToInteger(0));
                URLHelper.Redirect(url);
            }
        }
    }


    protected object ugDiscounts_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        var discountRow = parameter as DataRowView;
        var discountInfo = GetDiscountInfo(discountRow?.Row);

        if (discountInfo == null)
        {
            return String.Empty;
        }

        switch (sourceName.ToLowerInvariant())
        {
            // Append to a value field char '%' or site currency in case of flat discount
            case "value":
                return GetDiscountGridValue(discountInfo);

            // Display discount status
            case "status":
                // Ensure correct values for unigrid export
                if (sender == null)
                {
                    return discountInfo.DiscountStatus.ToLocalizedString("com.discountstatus");
                }

                return new DiscountStatusTag(couponCountsDataProvider, discountInfo);

            case "discountorder":
                // Ensure correct values for unigrid export
                if ((sender == null) || !ECommerceContext.IsUserAuthorizedToModifyDiscount())
                {
                    return discountInfo.DiscountOrder;
                }

                return new PriorityInlineEdit
                {
                    PrioritizableObject = discountInfo,
                    Unigrid = ugDiscounts
                };

            case "application":
                // Display blank value if discount don't use coupons 
                if (!discountInfo.DiscountUsesCoupons)
                {
                    return "&mdash;";
                }

                return discountInfo.HasCoupons ? CouponCodeInfoProvider.GetCouponUsageInfoMessage(discountInfo, true) : GetString("com.discount.notcreated");

            case "orderamount":
                decimal totalPriceInMainCurrency = ValidationHelper.GetDecimal(discountInfo.DiscountItemMinOrderAmount, 0m);

                // Display blank value in the discount listing if order amount is not configured
                if (totalPriceInMainCurrency == 0)
                {
                    return string.Empty;
                }

                // Format currency
                string priceInMainCurrencyFormatted = CurrencyInfoProvider.GetFormattedPrice(totalPriceInMainCurrency, SiteContext.CurrentSiteID);
                return HTMLHelper.HTMLEncode(priceInMainCurrencyFormatted);
        }

        return parameter;
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Based on type of displayed discount <see cref="DiscountApplicationEnum"/> sets correct discount type <see cref="DiscountTypeEnum"/> for status filter in UniGrid.
    /// </summary>
    private void SetDiscountStatusFilterType()
    {
        DiscountTypeEnum discountStatusFilterType = DiscountTypeEnum.Discount;

        switch (DiscountType)
        {
            case DiscountApplicationEnum.Products:
                discountStatusFilterType = DiscountTypeEnum.CatalogDiscount;
                break;
            case DiscountApplicationEnum.Order:
                discountStatusFilterType = DiscountTypeEnum.OrderDiscount;
                break;
            case DiscountApplicationEnum.Shipping:
                discountStatusFilterType = DiscountTypeEnum.ShippingDiscount;
                break;
        }

        // In UniGrid, find column Status, its filter and its parameter for discount type
        var discountFilterTypeParam = ugDiscounts?.GridColumns?
            .Columns.SingleOrDefault(c => string.Equals(c.Name, "Status", StringComparison.OrdinalIgnoreCase))?
            .Filter?.CustomFilterParameters?.Parameters.SingleOrDefault(p => string.Equals(p.Name, "discounttype", StringComparison.OrdinalIgnoreCase));

        // If this parameter has been found, set its value
        if (discountFilterTypeParam != null)
        {
            discountFilterTypeParam.Value = discountStatusFilterType.ToStringRepresentation();
        }
    }


    /// <summary>
    /// Returns dictionary of discount coupon use count and limit. Key of the dictionary is the ID of discount.
    /// </summary>
    /// <param name="type">Object type (ignored).</param>
    /// <param name="discountIDs">IDs of discount which the dictionary is to be filled with.</param>
    private SafeDictionary<int, IDataContainer> GetCountsDataHandler(string type, IEnumerable<int> discountIDs)
    {
        DataSet counts = CouponCodeInfoProvider.GetCouponCodeUseCount(discountIDs);

        return counts.ToDictionaryById("CouponCodeDiscountID");
    }


    /// <summary>
    /// Sets type of discounts which are being listed.
    /// </summary>
    private void SetDiscountType()
    {
        // Return Discount type
        var enumValueIndex = QueryHelper.GetInteger("type", 0);

        if (Enum.IsDefined(typeof(DiscountApplicationEnum), enumValueIndex))
        {
            DiscountType = (DiscountApplicationEnum)enumValueIndex;
        }
        else
        {
            DiscountType = EnumHelper.GetDefaultValue<DiscountApplicationEnum>();
        }
    }


    private static string GetDiscountGridValue(DiscountInfo discount)
    {
        return discount.DiscountIsFlat
            ? CurrencyInfoProvider.GetFormattedPrice(discount.DiscountValue, discount.DiscountSiteID)
            : ECommerceHelper.GetFormattedPercentageValue(discount.DiscountValue, CultureHelper.PreferredUICultureInfo);
    }


    private DiscountInfo GetDiscountInfo(DataRow row)
    {
        if (row == null)
        {
            return null;
        }

        var discountId = DataHelper.GetIntValue(row, DiscountInfo.TYPEINFO.IDColumn);

        var discount = discounts[discountId] as DiscountInfo;
        if (discount == null)
        {
            discount = new DiscountInfo(row);
            discounts[discount.DiscountID] = discount;
        }

        return discount;
    }

    #endregion
}