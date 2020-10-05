using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_ShippingSelector : BaseObjectSelector
{
    #region "Public properties"

    /// <summary>
    /// Indicates the site the shipping options should be loaded from. If shopping cart is given use its site ID.
    /// Default value is current site ID.
    /// </summary>
    public override int SiteID
    {
        get
        {
            // If shopping cart given use its site ID
            if ((ShoppingCart != null) && (base.SiteID != ShoppingCart.ShoppingCartSiteID))
            {
                base.SiteID = ShoppingCart.ShoppingCartSiteID;
            }

            return base.SiteID;
        }
        set
        {
            base.SiteID = value;
        }
    }


    /// <summary>
    /// Shopping cart.
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get
        {
            return GetValue("ShoppingCart") as ShoppingCartInfo;
        }
        set
        {
            SetValue("ShoppingCart", value);
        }
    }


    /// <summary>
    /// Decides if shipping option price should be displayed next to shipping option in the shipping selector. By default is true.
    /// </summary>
    public bool DisplayShippingOptionPrice
    {
        get;
        set;
    } = true;


    /// <summary>
    /// If true, selected value is ShippingOptionName, if false, selected value is ShippingOptionID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseShippingNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseShippingNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Allows to access selector object.
    /// </summary>
    public override UniSelector UniSelector => uniSelector;

    #endregion


    #region "Methods"

    /// <summary>
    /// Display only site shipping options.
    /// </summary>
    protected override string AppendExclusiveWhere(string where)
    {
        return new WhereCondition(base.AppendExclusiveWhere(where))
            .WhereEquals(SiteIDColumn, SiteID)
            .ToString(true);
    }


    /// <summary>
    /// Convert given shipping name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the shipping to be converted.</param>
    /// <param name="siteName">Name of the site of the shipping.</param>
    protected override int GetID(string name, string siteName)
    {
        var shipping = ShippingOptionInfo.Provider.Get(name, SiteInfoProvider.GetSiteID(siteName));

        return shipping?.ShippingOptionID ?? 0;
    }


    /// <summary>
    /// Ensures that only applicable shipping options are displayed in selector.
    /// </summary>
    /// <param name="ds">Dataset with shipping options.</param>
    protected override DataSet OnAfterRetrieveData(DataSet ds)
    {
        if (DataHelper.IsEmpty(ds) || (ShoppingCart == null))
        {
            return ds;
        }

        foreach (DataRow shippingRow in ds.Tables[0].Select())
        {
            ShippingOptionInfo shippingOptionInfo;

            if (UseNameForSelection)
            {
                var shippingName = DataHelper.GetStringValue(shippingRow, "ShippingOptionName");
                shippingOptionInfo = ShippingOptionInfo.Provider.Get(shippingName, ShoppingCart.ShoppingCartSiteID);
            }
            else
            {
                var shippingID = DataHelper.GetIntValue(shippingRow, "ShippingOptionID");
                shippingOptionInfo = ShippingOptionInfo.Provider.Get(shippingID);
            }

            // Do not remove already selected item even if the option is not applicable anymore 
            // The user would see a different value in UI as is actually stored in the database
            var canBeRemoved = !EnsureSelectedItem || (ShoppingCart.ShoppingCartShippingOptionID != shippingOptionInfo.ShippingOptionID);
            if (canBeRemoved && !ShippingOptionInfoProvider.IsShippingOptionApplicable(ShoppingCart, shippingOptionInfo))
            {
                ds.Tables[0].Rows.Remove(shippingRow);
            }
        }

        return ds;
    }


    /// <summary>
    /// Adds price to individual shipping options when shopping cart object supplied.
    /// </summary>
    /// <param name="item">Shipping option item to add price to.</param>
    protected override void OnListItemCreated(ListItem item)
    {
        // Adding price to shipping option is not required
        if (!DisplayShippingOptionPrice)
        {
            return;
        }

        if ((item != null) && (ShoppingCart != null))
        {
            // Calculate hypothetical shipping cost for shipping option from supplied list item
            var optionID = ValidationHelper.GetInteger(item.Value, 0);
            var option = ShippingOptionInfo.Provider.Get(optionID);

            var shippingPrice = CalculateShipping(option);
            var formattedPrice = CurrencyInfoProvider.GetFormattedPrice(shippingPrice, ShoppingCart.Currency, false);

            if (shippingPrice > 0)
            {
                var detailInfo = $"({formattedPrice})";

                if (CultureHelper.IsUICultureRTL())
                {
                    item.Text = $"{detailInfo} {item.Text}";
                }
                else
                {
                    item.Text += $" {detailInfo}";
                }
            }        
        }
    }


    private decimal CalculateShipping(ShippingOptionInfo option)
    {
        if (option == null || (ShoppingCart.Currency == null))
        {
            return 0m;
        }

        var request = Service.Resolve<IShoppingCartAdapterService>().GetCalculationRequest(ShoppingCart);
        request.ShippingOption = option;

        var result = Service.Resolve<IShoppingCartAdapterService>().GetCalculationResult(ShoppingCart);

        var shippingService = Service.Resolve<IShippingPriceService>();
        return shippingService.GetShippingPrice(new CalculatorData(request, result), ShoppingCart.TotalItemsPrice - ShoppingCart.OrderDiscount).Price;
    }

    #endregion
}