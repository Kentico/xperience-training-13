using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartSKUPriceDetail_Control : CMSUserControl
{
    private Guid mCartItemGuid = Guid.Empty;
    private ShoppingCartItemInfo mShoppingCartItem;


    /// <summary>
    /// Shopping cart.
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get;
        set;
    }


    /// <summary>
    /// Shopping cart item GUID.
    /// </summary>
    public Guid CartItemGuid
    {
        get
        {
            return mCartItemGuid;
        }
        set
        {
            mCartItemGuid = value;
        }
    }


    /// <summary>
    /// Shopping cart item.
    /// </summary>
    public ShoppingCartItemInfo ShoppingCartItem
    {
        get
        {
            if (mShoppingCartItem == null)
            {
                if ((CartItemGuid != Guid.Empty) && (ShoppingCart != null))
                {
                    // Get item from shopping cart
                    mShoppingCartItem = ShoppingCartInfoProvider.GetShoppingCartItem(ShoppingCart, CartItemGuid);
                }
            }

            return mShoppingCartItem;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if ShoppingCartItem exists
        if (ShoppingCartItem == null)
        {
            RedirectToInformation("general.ObjectNotFound");
            return;
        }

        // Discount tables header
        gridDiscounts.Columns[0].HeaderText = GetString("ProductPriceDetail.CatalogDiscounts");
        gridItemDiscounts.Columns[0].HeaderText = GetString("ProductPriceDetail.ItemDiscounts");

        // Discounts
        gridDiscounts.DataSource = ShoppingCartItem.UnitDiscountSummary;
        gridDiscounts.DataBind();
        gridItemDiscounts.DataSource = ShoppingCartItem.DiscountSummary;
        gridItemDiscounts.DataBind();

        // Display unit price
        lblPriceValue.Text = FormarPrice(ShoppingCartItem.UnitPrice);
        lblStandardPriceValue.Text = FormarPrice(ShoppingCartItem.UnitPrice + ShoppingCartItem.UnitTotalDiscount);

        lblTotalDiscountValue.Text = FormarPrice(ShoppingCartItem.UnitTotalDiscount);
        lblItemDiscountValue.Text = FormarPrice(ShoppingCartItem.TotalDiscount);
        lblTotalPriceValue.Text = FormarPrice(ShoppingCartItem.TotalPrice);

        // Product name and units
        if (ShoppingCartItem?.SKU != null)
        {
            lblProductName.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ShoppingCartItem.SKU.SKUName));
            lblProductUnitsValue.Text = ShoppingCartItem.CartItemUnits.ToString();
        }
    }


    /// <summary>
    /// Returns formatted tax/discount name.
    /// </summary>
    /// <param name="name">Tax/discount name</param>
    protected string GetFormattedName(object name)
    {
        return HTMLHelper.HTMLEncode(" - " + ResHelper.LocalizeString(Convert.ToString(name)));
    }


    /// <summary>
    /// Returns formatted value string.
    /// </summary>
    /// <param name="value">Value to be formatted</param>
    protected string GetFormattedValue(object value)
    {
        var mValue = ValidationHelper.GetDecimal(value, 0);

        return FormarPrice(mValue);
    }


    private string FormarPrice(decimal price)
    {
        return CurrencyInfoProvider.GetFormattedPrice(price, ShoppingCart.Currency);
    }
}