using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[Breadcrumb(1, "Order_New.HeaderCaption", "", "")]
[UIElement(ModuleName.ECOMMERCE, "Orders", false, true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_New : CMSEcommercePage
{
    private int customerId = 0;


    /// <summary>
    /// Shopping cart to use.
    /// </summary>
    private ShoppingCartInfo ShoppingCart
    {
        get
        {
            ShoppingCartInfo sci = SessionHelper.GetValue(SessionKey) as ShoppingCartInfo;
            if (sci == null)
            {
                sci = GetNewCart();
                SessionHelper.SetValue(SessionKey, sci);
            }

            return sci;
        }
        set
        {
            SessionHelper.SetValue(SessionKey, value);
        }
    }


    /// <summary>
    /// Shopping cart session key.
    /// </summary>
    private string SessionKey
    {
        get
        {
            return (customerId > 0) ? "CMSDeskNewCustomerOrderShoppingCart" : "CMSDeskNewOrderShoppingCart";
        }
    }


    protected ShoppingCartInfo GetNewCart()
    {
        ShoppingCartInfo cart = null;
        var siteId = SiteContext.CurrentSiteID;

        if (customerId > 0)
        {
            var customer = CustomerInfo.Provider.Get(customerId);
            if (customer != null)
            {
                cart = ShoppingCartFactory.CreateCart(siteId, customer.CustomerUser);
                cart.ShoppingCartCustomerID = customerId;
            }
        }

        cart = cart ?? ShoppingCartFactory.CreateCart(siteId);

        return cart;
    }


    protected override void OnPreInit(EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerid", 0);

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            ShoppingCart = GetNewCart();
        }

        Cart.LocalShoppingCart = ShoppingCart;
        Cart.EnableProductPriceDetail = true;
        Cart.OnPaymentCompleted += (s, ea) => GoToOrderDetail();
        Cart.OnPaymentSkipped += (s, ea) => GoToOrderDetail();
        Cart.OnCheckPermissions += Cart_OnCheckPermissions;

        Cart.CheckoutProcessType = (customerId > 0) ? CheckoutProcessEnum.CMSDeskCustomer : CheckoutProcessEnum.CMSDeskOrder;
    }


    private void Cart_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check ecommerce permission
        if (!ECommerceContext.IsUserAuthorizedForPermission(permissionType))
        {
            string message = permissionType;
            if (permissionType.StartsWith("modify", StringComparison.OrdinalIgnoreCase))
            {
                message = "EcommerceModify OR " + message;
            }

            RedirectToAccessDenied(ModuleName.ECOMMERCE, message);
        }
    }


    private void GoToOrderDetail()
    {
        URLHelper.Redirect(UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "OrderProperties", false, ShoppingCart.OrderId) + "&customerid=" + customerId);
    }


    protected void Page_Prerender()
    {
        // Create breadcrumbs
        CreateBreadcrumbs();
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    private void CreateBreadcrumbs()
    {
        string url = (customerId > 0) ? "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_List.aspx?customerId=" + customerId : "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_List.aspx";

        // Set breadcrumb
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Index = 0,
            Text = GetString("Order_New.Orders"),
            RedirectUrl = ResolveUrl(url),
        });

        if (customerId <= 0)
        {
            PageTitle.TitleText = GetString("Order_New.HeaderCaption");
        }

        UIHelper.SetBreadcrumbsSuffix("");
    }
}