using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("Order_Edit_AddItems.Title")]
[UIElement(ModuleName.ECOMMERCE, "order.AddOrderItems")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_AddItems : CMSEcommerceModalPage
{
    #region "Variables"

    private ShoppingCartInfo mShoppingCartObj;
    private ICatalogPriceCalculator mPriceCalculator;
    private readonly Dictionary<int, TextBox> quantityControls = new Dictionary<int, TextBox>();

    #endregion


    #region "Properties"

    /// <summary>
    /// Shopping cart object with order data.
    /// </summary>
    protected ShoppingCartInfo ShoppingCartObj
    {
        get
        {
            if (mShoppingCartObj == null)
            {
                string cartSessionName = QueryHelper.GetString("cart", "");
                if (cartSessionName != "")
                {
                    mShoppingCartObj = SessionHelper.GetValue(cartSessionName) as ShoppingCartInfo;
                }
            }
            return mShoppingCartObj;
        }
    }


    /// <summary>
    /// Shopping cart items selector SKU ID.
    /// </summary>
    private int SKUID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["SKUID"], 0);
        }
        set
        {
            ViewState["SKUID"] = value;
        }
    }


    /// <summary>
    /// Product price calculator.
    /// </summary>
    private ICatalogPriceCalculator PriceCalculator
    {
        get
        {
            return mPriceCalculator ?? (mPriceCalculator = Service.Resolve<ICatalogPriceCalculatorFactory>().GetCalculator(ShoppingCartObj.ShoppingCartSiteID));
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Save += OnSave;

        // Init labels
        SetSaveResourceString("Order_Edit_AddItems.Add");

        if (!RequestHelper.IsPostBack())
        {
            SwitchToProducts();
        }

        InitializeGrid();

        // Initialize shopping cart item selector
        cartItemSelector.SKUID = SKUID;
        cartItemSelector.ShoppingCart = ShoppingCartObj;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (plcSelector.Visible)
        {
            // Remove css class dialog-content-scrollable to turn off position:relative. Our Selector control needs to overflow content container and hide dialog footer with his footer button.            
            CurrentMaster.PanelContent.RemoveCssClass("dialog-content-scrollable");
            // Get product name and price
            SKUInfo skuObj = SKUInfo.Provider.Get(SKUID);
            string skuName = "";

            if (skuObj != null)
            {
                skuName = ResHelper.LocalizeString(skuObj.SKUName);
            }

            // Set SKU name label
            headSKUName.Text = HTMLHelper.HTMLEncode(skuName);

            // Set breadcrumb
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
            {
                Text = GetString("Order_Edit_AddItems.Products"),
                RedirectUrl = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit_AddItems.aspx?cart=" + HTMLHelper.HTMLEncode(QueryHelper.GetString("cart", ""))),
            });

            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
            {
                Text = skuName
            });
        }
    }

    #endregion


    #region "Event handlers"

    private DataSet gridProducts_OnAfterRetrieveData(DataSet ds)
    {
        // Clear dictionary containing textboxes for entering quantities
        quantityControls.Clear();

        // No manipulation with data
        return ds;
    }


    private object gridProducts_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = null;

        switch (sourceName.ToLowerInvariant())
        {
            case "skuname":
                row = (DataRowView)parameter;
                string skuName = ValidationHelper.GetString(row["SKUName"], "");
                int skuId = ValidationHelper.GetInteger(row["SKUID"], 0);

                // Create link for adding one item using product name
                LinkButton link = new LinkButton();
                link.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(skuName));
                link.Click += btnAddOneUnit_Click;
                link.CommandArgument = skuId.ToString();

                return link;

            case "price":
                // Format product price
                row = (DataRowView)parameter;
                return GetSKUFormattedPrice(new SKUInfo(row.Row));

            case "quantity":
                int id = ValidationHelper.GetInteger(parameter, 0);

                // Create textbox for entering quantity
                CMSTextBox tb = new CMSTextBox();
                tb.MaxLength = 9;
                tb.Width = 50;
                tb.ID = "txtQuantity" + id;

                // Add textbox to dictionary under SKUID key
                quantityControls.Add(id, tb);

                return tb;
        }

        return parameter;
    }


    private string GetSKUFormattedPrice(SKUInfo sku)
    {
        var prices = PriceCalculator.GetPrices(sku, null, ShoppingCartObj);

        return CurrencyInfoProvider.GetFormattedPrice(prices.Price, ShoppingCartObj.Currency);
    }


    private void btnAddOneUnit_Click(object sender, EventArgs e)
    {
        // Get SKU ID
        int skuId = ValidationHelper.GetInteger(((LinkButton)sender).CommandArgument, 0);

        bool hasProductOptions = SKUInfoProvider.HasSKUEnabledOptions(skuId);

        // If product has product options
        // -> abort inserting products to the shopping cart
        if (hasProductOptions)
        {

            lblTitle.ResourceString = "order_edit_additems.productoptions";

            // Save SKU ID to the viewstate
            SKUID = skuId;

            // Initialize shopping cart item selector
            cartItemSelector.SKUID = SKUID;
            cartItemSelector.ShowProductOptions = true;
            cartItemSelector.ReloadData();

            SwitchToOptions();
        }
        else
        {
            // Add product to shopping cart and close dialog window
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "addproduct", ScriptHelper.GetScript("AddProducts(" + skuId + ", 1);"));
        }
    }


    /// <summary>
    /// Handles <see cref="CMSModalPage.Save"/> event.
    /// </summary>
    private void OnSave(object sender, EventArgs e)
    {
        // Check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
        }

        string allUnits = null;
        string allSkuId = null;

        foreach (KeyValuePair<int, TextBox> item in quantityControls)
        {
            // Get params
            int skuId = item.Key;
            TextBox txtUnits = item.Value;

            // Get unit count
            int units = ValidationHelper.GetInteger(txtUnits.Text, 0);

            if (units > 0)
            {
                // Get product and localized name
                SKUInfo product = SKUInfo.Provider.Get(skuId);
                if (product != null)
                {
                    string skuName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(product.SKUName));

                    // Abort inserting products to the shopping cart ifIf product has some product options
                    if (!DataHelper.DataSourceIsEmpty(OptionCategoryInfoProvider.GetProductOptionCategories(skuId, true)))
                    {
                        // Show error message
                        ShowError(string.Format(GetString("Order_Edit_AddItems.ProductOptionsRequired"), skuName));

                        return;
                    }

                    // Create strings with SKU IDs and units separated by ';'
                    allSkuId += skuId + ";";
                    allUnits += units + ";";
                }
            }
        }

        // Close this modal window and refresh parent values in window
        CloseWindow(allSkuId, allUnits);
    }

    #endregion


    #region "Methods"

    private void SwitchToProducts()
    {
        // Display products
        plcProducts.Visible = true;

        // Hide shopping cart item selector
        plcSelector.Visible = false;
    }


    private void SwitchToOptions()
    {
        // Hide products
        plcProducts.Visible = false;

        // Show shopping cart item selector
        plcSelector.Visible = true;
    }


    /// <summary>
    /// Grid initialization.
    /// </summary>
    protected void InitializeGrid()
    {
        gridProducts.OnExternalDataBound += gridProducts_OnExternalDataBound;
        gridProducts.OnAfterRetrieveData += gridProducts_OnAfterRetrieveData;
        gridProducts.ShowActionsMenu = false;
    }


    /// <summary>
    /// Generates script that closes the window and refreshes the parent page.
    /// </summary>
    /// <param name="skuIds">String with SKU IDs separated by ';'</param>
    /// <param name="units">String with SKU units separated by ';'</param>
    private void CloseWindow(string skuIds, string units)
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "addproductClose", ScriptHelper.GetScript("AddProducts('" + skuIds + "','" + units + "');"));
    }

    #endregion
}
