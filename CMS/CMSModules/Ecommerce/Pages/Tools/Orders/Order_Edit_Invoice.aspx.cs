using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "Orders.Invoice")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Invoice : CMSEcommercePage
{
    #region "Variables"

    private int orderId;
    private OrderInfo order;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the dialog script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.NEWWINDOW_SCRIPT_KEY, ScriptHelper.NewWindowScript);

        btnGenerate.Text = GetString("order_invoice.btnGenerate");
        btnPrintPreview.Text = GetString("order_invoice.btnPrintPreview");

        if (QueryHelper.GetInteger("orderid", 0) != 0)
        {
            orderId = QueryHelper.GetInteger("orderid", 0);
        }
        order = OrderInfo.Provider.Get(orderId);

        if (order == null)
        {
            btnGenerate.Enabled = false;
            btnPrintPreview.Enabled = false;
            return;
        }

        // Check order site ID
        CheckEditedObjectSiteID(order.OrderSiteID);

        // Get invoice url
        var invoiceUrl = URLHelper.ResolveUrl("~/CMSModules/Ecommerce/CMSPages/GetInvoice.aspx");
        invoiceUrl = URLHelper.AddParameterToUrl(invoiceUrl, "orderId", orderId.ToString());

        if (!RequestHelper.IsPostBack())
        {
            txtInvoiceNumber.Text = order.OrderInvoiceNumber;
            invoiceFrame.Attributes.Add("src", invoiceUrl);
        }

        // Print dialog
        invoiceUrl = URLHelper.AddParameterToUrl(invoiceUrl, "print", "1");
        ltlScript.Text = ScriptHelper.GetScript("function showPrintPreview() { NewWindow('" + invoiceUrl + "', 'InvoicePrint', 650, 700);}");
    }

    #endregion


    #region "Event handlers"

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        // check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
        }

        // Load the shopping cart from order
        ShoppingCartInfo sci = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(orderId);

        // Update order invoice number 
        order.OrderInvoiceNumber = txtInvoiceNumber.Text.Truncate(txtInvoiceNumber.MaxLength);
        order.OrderInvoice = ShoppingCartInfoProvider.GetOrderInvoice(sci);
 
        OrderInfo.Provider.Set(order);

        // Show message
        ShowChangesSaved();
    }

    #endregion
}
