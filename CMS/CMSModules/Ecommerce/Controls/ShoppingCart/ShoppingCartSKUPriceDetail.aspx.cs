using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("ProductPriceDetail.Title")]
[UIElement(ModuleName.ECOMMERCE, "order.productpricedetail")]
public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartSKUPriceDetail : CMSEcommerceModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize product price detail
        InitializeSKUPriceDetailControl();
    }


    /// <summary>
    /// Initializes properties of the control which display current sku price detail.
    /// </summary>
    private void InitializeSKUPriceDetailControl()
    {
        // Set current SKU ID
        ucSKUPriceDetail.CartItemGuid = QueryHelper.GetGuid("itemguid", Guid.Empty);

        // Get local shopping cart session name
        string sessionName = QueryHelper.GetString("cart", String.Empty);
        if (sessionName != String.Empty)
        {
            // Get local shopping cart when in CMSDesk
            object obj = SessionHelper.GetValue(sessionName);
            if (obj != null)
            {
                ucSKUPriceDetail.ShoppingCart = (ShoppingCartInfo)obj;
            }
        }
    }
}