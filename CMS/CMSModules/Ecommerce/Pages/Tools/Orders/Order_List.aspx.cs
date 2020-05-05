using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("Order_New.Orders")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_List : CMSEcommercePage
{
    #region "Page Events"

    protected override void OnPreInit(EventArgs e)
    {
        var customerId = QueryHelper.GetInteger("customerId", 0);
        var elementName = customerId == 0 ? "Orders" : "Customers.Orders";

        var uiElement = new UIElementAttribute(ModuleName.ECOMMERCE, elementName);
        uiElement.Check(this);

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {       
        // New order action
        AddHeaderAction(new HeaderAction
        {
            Text = GetString("Order_List.NewItemCaption"),
            RedirectUrl = "Order_New.aspx?customerid=" +  QueryHelper.GetInteger("customerid", 0),
            Enabled = ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY)
        });
    }

    #endregion
}