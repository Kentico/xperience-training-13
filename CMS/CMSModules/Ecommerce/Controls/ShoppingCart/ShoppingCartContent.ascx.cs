using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartContent : ShoppingCartStep
{
    #region "Variables"

    protected Button btnAddProduct;
    protected HiddenField hidProductID;
    protected HiddenField hidQuantity;
    protected HiddenField hidOptions;

    protected bool checkInventory;
    protected bool? mCanReadProducts;

    #endregion


    #region Properties

    /// <summary>
    /// Indicates whether there are another checkout process steps after the current step, except payment.
    /// </summary>
    private bool ExistAnotherStepsExceptPayment
    {
        get
        {
            return (ShoppingCartControl.CurrentStepIndex + 2 <= ShoppingCartControl.CheckoutProcessSteps.Count - 1);
        }
    }


    /// <summary>
    /// Indicates if current user is authorized to read product details.
    /// </summary>
    private bool CanReadProducts
    {
        get
        {
            if (!mCanReadProducts.HasValue)
            {
                mCanReadProducts = ECommerceHelper.IsUserAuthorizedForPermission(EcommercePermissions.PRODUCTS_READ, SiteContext.CurrentSiteName, CurrentUser);
            }

            return mCanReadProducts.Value;
        }
    }


    /// <summary>
    /// Indicates that order exists and is paid.
    /// </summary>
    private bool OrderIsPaid
    {
        get
        {
            return (ShoppingCart.Order != null) && ShoppingCart.Order.OrderIsPaid;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        selectCurrency.Changed += (sender, args) => btnUpdate_Click(null, null);

        // Add product button
        btnAddProduct = new CMSButton();
        btnAddProduct.Attributes["style"] = "display: none";
        Controls.Add(btnAddProduct);
        btnAddProduct.Click += btnAddProduct_Click;

        // Add the hidden fields for productId, quantity and product options
        hidProductID = new HiddenField { ID = "hidProductID" };
        Controls.Add(hidProductID);

        hidQuantity = new HiddenField { ID = "hidQuantity" };
        Controls.Add(hidQuantity);

        hidOptions = new HiddenField { ID = "hidOptions" };
        Controls.Add(hidOptions);
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Register add product script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AddProductScript",
                                               ScriptHelper.GetScript(
                                                   "function setProduct(val) { document.getElementById('" + hidProductID.ClientID + "').value = val; } \n" +
                                                   "function setQuantity(val) { document.getElementById('" + hidQuantity.ClientID + "').value = val; } \n" +
                                                   "function setOptions(val) { document.getElementById('" + hidOptions.ClientID + "').value = val; } \n" +
                                                   "function AddProduct(productIDs, quantities, options, price) { \n" +
                                                   "setProduct(productIDs); \n" +
                                                   "setQuantity(quantities); \n" +
                                                   "setOptions(options); \n" +
                                                   Page.ClientScript.GetPostBackEventReference(btnAddProduct, null) +
                                                   ";} \n" +
                                                   "function RefreshCart() {" +
                                                   Page.ClientScript.GetPostBackEventReference(btnAddProduct, null) +
                                                   ";} \n"
                                                   ));

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Hide columns with identifiers
        gridData.Columns[0].Visible = false;
        gridData.Columns[1].Visible = false;
        gridData.Columns[2].Visible = false;
        gridData.Columns[3].Visible = false;

        // Hide actions column
        gridData.Columns[5].Visible = (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems) ||
                                      (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrder);

        // Disable specific controls
        if (!Enabled)
        {
            lnkNewItem.Enabled = false;
            lnkNewItem.OnClientClick = "";
            selectCurrency.Enabled = false;
            btnEmpty.Enabled = false;
            btnUpdate.Enabled = false;
            txtCoupon.Enabled = false;
            chkSendEmail.Enabled = false;

            ScriptHelper.RegisterStartupScript(this, typeof(string), "ShoppingCartContent_DisableCouponCodes", "$cmsj(\".cart-coupon-code .button > input\").prop(\"disabled\", true)", true);
        }

        // Show/Hide dropdownlist with currencies
        pnlCurrency.Visible &=
            (ShoppingCartControl.CheckoutProcessType != CheckoutProcessEnum.CMSDeskOrderItems)
            && selectCurrency.HasData
            && (selectCurrency.DropDownSingleSelect.Items.Count > 1);

        // Check inventory
        if (checkInventory)
        {
            var validationErrors = ShoppingCartInfoProvider.ValidateShoppingCart(ShoppingCart);

            if (validationErrors.Any())
            {
                lblError.Text = validationErrors
                    .Select(error => HTMLHelper.HTMLEncode(error.GetMessage()))
                    .Join("<br />");
            }
        }

        // Display messages if required
        lblError.Visible = !string.IsNullOrEmpty(lblError.Text.Trim());
        lblInfo.Visible = !string.IsNullOrEmpty(lblInfo.Text.Trim());

        rptrCouponCodes.DataSource = new CouponCodesViewModel(ShoppingCart.CouponCodes);
        rptrCouponCodes.DataBind();

        base.OnPreRender(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        if (ShoppingCart != null)
        {
            // Set currency selectors site ID
            selectCurrency.SiteID = ShoppingCart.ShoppingCartSiteID;
        }

        lnkNewItem.Text = GetString("ecommerce.shoppingcartcontent.additem");
        btnUpdate.Text = GetString("ecommerce.shoppingcartcontent.btnupdate");
        btnEmpty.Text = GetString("ecommerce.shoppingcartcontent.btnempty");
        btnEmpty.OnClientClick = string.Format("return confirm({0});", ScriptHelper.GetString(GetString("ecommerce.shoppingcartcontent.emptyconfirm")));

        // Add new product dialog
        string addProductUrl = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "order.addorderitems", 0, GetCMSDeskShoppingCartSessionNameQuery());
        lnkNewItem.OnClientClick = ScriptHelper.GetModalDialogScript(addProductUrl, "addproduct", 1000, 620);

        gridData.Columns[4].HeaderText = GetString("general.remove");
        gridData.Columns[5].HeaderText = GetString("ecommerce.shoppingcartcontent.actions");
        gridData.Columns[6].HeaderText = GetString("ecommerce.shoppingcartcontent.skuname");
        gridData.Columns[7].HeaderText = GetString("ecommerce.shoppingcartcontent.skuunits");
        gridData.Columns[8].HeaderText = GetString("ecommerce.shoppingcartcontent.unitprice");
        gridData.Columns[9].HeaderText = GetString("ecommerce.shoppingcartcontent.itemdiscount");
        gridData.Columns[10].HeaderText = GetString("ecommerce.shoppingcartcontent.subtotal");
        gridData.RowDataBound += gridData_RowDataBound;

        // Set sending order notification when editing existing order according to the site settings
        if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        {
            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                if (!string.IsNullOrEmpty(ShoppingCart.SiteName))
                {
                    chkSendEmail.Checked = ECommerceSettings.SendOrderNotification(ShoppingCart.SiteName);
                }
            }
            // Show send email checkbox
            chkSendEmail.Visible = true;
            chkSendEmail.Text = GetString("shoppingcartcontent.sendemail");

            // Setup buttons
            ShoppingCartControl.ButtonBack.Visible = false;
            ShoppingCartControl.ButtonNext.Text = GetString("general.next");

            if ((!ExistAnotherStepsExceptPayment) && (ShoppingCartControl.PaymentGatewayProvider == null))
            {
                ShoppingCartControl.ButtonNext.Text = GetString("general.ok");
            }
        }

        // Fill dropdownlist
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            selectCurrency.SelectedID = ShoppingCart.ShoppingCartCurrencyID;

            ReloadData();
        }

        // Ensure order currency in selector
        if ((ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems) && (ShoppingCart.Order != null))
        {
            selectCurrency.AdditionalItems += ShoppingCart.Order.OrderCurrencyID + ";";
        }

        HandlePostBack();

        base.OnLoad(e);
    }


    private void gridData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // Set enabled for order item units editing
            e.Row.Cells[7].Enabled = Enabled;
        }
    }


    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        if (ShoppingCart != null)
        {
            // Do not update order if it is already paid
            if (OrderIsPaid)
            {
                return;
            }

            if (selectCurrency.SelectedID > 0)
            {
                ShoppingCart.ShoppingCartCurrencyID = selectCurrency.SelectedID;
            }

            // Skip if method was called by btnAddProduct
            if (sender != btnAddProduct)
            {
                foreach (GridViewRow row in gridData.Rows)
                {
                    // Get shopping cart item Guid
                    Guid cartItemGuid = ValidationHelper.GetGuid(((Label)row.Cells[1].Controls[1]).Text, Guid.Empty);

                    // Try to find shopping cart item in the list
                    var cartItem = ShoppingCartInfoProvider.GetShoppingCartItem(ShoppingCart, cartItemGuid);
                    if (cartItem != null)
                    {
                        // If product and its product options should be removed
                        if (((CMSCheckBox)row.Cells[4].Controls[1]).Checked && (sender != null))
                        {
                            // Remove product and its product option from list
                            ShoppingCartInfoProvider.RemoveShoppingCartItem(ShoppingCart, cartItemGuid);
                        }
                        // If product units has changed
                        else if (!cartItem.IsProductOption)
                        {
                            // Get number of units
                            int itemUnits = ValidationHelper.GetInteger(((TextBox)(row.Cells[7].Controls[1])).Text.Trim(), 0);
                            if ((itemUnits > 0) && (cartItem.CartItemUnits != itemUnits))
                            {
                                // Update units of the parent product
                                ShoppingCartItemInfoProvider.UpdateShoppingCartItemUnits(cartItem, itemUnits);
                            }
                        }
                    }
                }
            }

            var couponCode = txtCoupon.Text.Trim();

            if (!string.IsNullOrEmpty(couponCode))
            {
                if (!ShoppingCart.AddCouponCode(couponCode))
                {
                    // Discount coupon is not valid                
                    lblError.Text = GetString("ecommerce.error.couponcodeisnotvalid");
                }
                else
                {
                    txtCoupon.Text = string.Empty;
                }
            }

            ShoppingCart.Evaluate();
            ReloadData();

            // Inventory should be checked
            checkInventory = true;
        }
    }


    protected void btnEmpty_Click(object sender, EventArgs e)
    {
        if (ShoppingCart != null)
        {
            // Do not empty cart if order is paid
            if (OrderIsPaid)
            {
                return;
            }

            ShoppingCartInfoProvider.EmptyShoppingCart(ShoppingCart);

            ReloadData();
        }
    }


    /// <summary>
    /// Validates this step.
    /// </summary>
    public override bool IsValid()
    {
        // Check modify permissions
        if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        {
            // Check 'ModifyOrders' permission
            if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
            {
                CMSPage.RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
            }
        }

        // Allow to go to the next step only if shopping cart contains some products
        bool IsValid = !ShoppingCart.IsEmpty;

        if (!IsValid)
        {
            HideCartContent();
        }

        if (ShoppingCart.IsCreatedFromOrder)
        {
            IsValid = true;
        }

        if (!IsValid)
        {
            lblError.Text = GetString("ecommerce.error.insertsomeproducts");
        }

        return IsValid;
    }


    /// <summary>
    /// Process this step.
    /// </summary>
    public override bool ProcessStep()
    {
        // Do not process step if order is paid
        if (OrderIsPaid)
        {
            return false;
        }

        // Shopping cart units are already saved in database (on "Update" or on "btnAddProduct_Click" actions) 
        bool isOK = false;

        if (ShoppingCart != null)
        {
            // Reload data
            ReloadData();

            // Validate available items before "Check out"
            var cartValidator = new ShoppingCartValidator(ShoppingCart, SKUInfo.Provider);

            cartValidator.Validate();

            if (!cartValidator.IsValid)
            {
                lblError.Text = cartValidator.GetErrorMessages()
                    .Select(e => HTMLHelper.HTMLEncode(e))
                    .Join("<br />");
            }
            else if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
            {
                // Indicates whether order saving process is successful
                isOK = true;

                try
                {
                    ShoppingCartInfoProvider.SetOrder(ShoppingCart);
                }
                catch (Exception ex)
                {
                    // Log exception
                    Service.Resolve<IEventLogService>().LogException("Shopping cart", "SAVEORDER", ex, ShoppingCart.ShoppingCartSiteID);
                    isOK = false;
                }

                if (isOK)
                {
                    lblInfo.Text = GetString("general.changessaved");

                    // Send order notification when editing existing order
                    if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
                    {
                        if (chkSendEmail.Checked)
                        {
                            OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                            OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
                        }
                    }
                    // Send order notification emails when on the live site
                    else if (ECommerceSettings.SendOrderNotification(SiteContext.CurrentSite.SiteName))
                    {
                        OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                        OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
                    }
                }
                else
                {
                    lblError.Text = GetString("ecommerce.orderpreview.errorordersave");
                }
            }
            // Go to the next step
            else
            {
                isOK = true;
            }
        }

        return isOK;
    }


    private void btnAddProduct_Click(object sender, EventArgs e)
    {
        // Do not add item if order is paid
        if (OrderIsPaid)
        {
            return;
        }

        // Get strings with productIDs and quantities separated by ';'
        string productIDs = ValidationHelper.GetString(hidProductID.Value, "");
        string quantities = ValidationHelper.GetString(hidQuantity.Value, "");
        string options = ValidationHelper.GetString(hidOptions.Value, "");

        // Add new products to shopping cart
        if ((productIDs != "") && (quantities != ""))
        {
            int[] arrID = ValidationHelper.GetIntegers(productIDs.TrimEnd(';').Split(';'), 0);
            int[] arrQuant = ValidationHelper.GetIntegers(quantities.TrimEnd(';').Split(';'), 0);
            int[] intOptions = ValidationHelper.GetIntegers(options.Split(','), 0);

            // Check site binding
            if (!CheckSiteBinding(arrID) || !CheckSiteBinding(intOptions))
            {
                return;
            }

            lblError.Text = "";

            for (int i = 0; i < arrID.Length; i++)
            {
                int skuId = arrID[i];

                SKUInfo skuInfo = SKUInfo.Provider.Get(skuId);
                if ((skuInfo != null) && !skuInfo.IsProductOption)
                {
                    int quantity = arrQuant[i];

                    ShoppingCartItemParameters cartItemParams = new ShoppingCartItemParameters(skuId, quantity, intOptions);

                    // Add product to the shopping cart
                    ShoppingCartInfoProvider.SetShoppingCartItem(ShoppingCart, cartItemParams);

                    // Show empty button
                    btnEmpty.Visible = true;
                }
            }
        }

        // Invalidate values
        hidProductID.Value = "";
        hidOptions.Value = "";
        hidQuantity.Value = "";

        ShoppingCart.Evaluate();

        // Update values in table
        btnUpdate_Click(btnAddProduct, e);

        if (ShoppingCart.ContentTable.Any())
        {
            // Inventory should be checked
            checkInventory = true;
        }
        else
        {
            // Hide cart content when empty
            HideCartContent();
        }
    }


    /// <summary>
    /// Checks whether skuIds are from shopping cart site.
    /// </summary>
    private bool CheckSiteBinding(IEnumerable<int> skuIds)
    {
        foreach (var skuId in skuIds)
        {
            SKUInfo skuInfo = SKUInfo.Provider.Get(skuId);
            if (skuInfo != null)
            {
                // Check if product belongs to site
                if (!skuInfo.IsGlobal && (skuInfo.SKUSiteID != ShoppingCart.ShoppingCartSiteID))
                {
                    return false;
                }
            }
        }

        return true;
    }


    // Displays total price
    protected void DisplayTotalPrice()
    {
        pnlPrice.Visible = true;
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.GrandTotal, ShoppingCart.Currency);
        lblTotalPrice.Text = GetString("ecommerce.cartcontent.totalpricelabel");

        lblShippingPrice.Text = GetString("ecommerce.cartcontent.shippingpricelabel");
        lblShippingPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalShipping, ShoppingCart.Currency);

        tblTotalTax.Text = GetString("ecommerce.shoppingcartcontent.totaltax");
        lblTotalTaxValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalTax, ShoppingCart.Currency);
    }


    /// <summary>
    /// Hides cart content controls and displays given message.
    /// </summary>
    protected void HideCartContent()
    {
        pnlNewItem.Visible = true;
        pnlPrice.Visible = false;
        btnEmpty.Visible = false;
        plcCoupon.Visible = false;
        plcCouponCodes.Visible = false;
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    protected void ReloadData()
    {
        var lines = ShoppingCart.ContentTable.ToList();
        gridData.DataSource = lines;

        // Bind data
        gridData.DataBind();

        if (lines.Any())
        {
            // Display total price
            DisplayTotalPrice();
        }
        else
        {
            // Hide some items
            HideCartContent();
        }

        // Show order related discounts
        if (ShoppingCart.OrderDiscountSummary != null)
        {
            plcOrderDiscounts.Visible = ShoppingCart.OrderDiscountSummary.Count > 0;
            ShoppingCart.OrderDiscountSummary.ToList().ForEach(d =>
            {
                plcOrderDiscountNames.Controls.Add(GetDiscountNameLabel(d));
                plcOrderDiscountNames.Controls.Add(GetEmptyLiteral());
                plcOrderDiscountValues.Controls.Add(GetDiscountValueLabel(d));
                plcOrderDiscountValues.Controls.Add(GetEmptyLiteral());
            });
        }

        // Show gift card related discounts
        if (ShoppingCart.OtherPaymentsSummary != null)
        {
            plcOtherPayments.Visible = ShoppingCart.OtherPaymentsSummary.Count > 0;
            ShoppingCart.OtherPaymentsSummary.ToList().ForEach(d =>
            {
                plcOtherPaymentsNames.Controls.Add(GetDiscountNameLabel(d));
                plcOtherPaymentsNames.Controls.Add(GetEmptyLiteral());
                plcOrderOtherPaymentsValues.Controls.Add(GetDiscountValueLabel(d));
                plcOrderOtherPaymentsValues.Controls.Add(GetEmptyLiteral());
            });
        }

        plcShippingPrice.Visible = ShoppingCart.IsShippingNeeded;
    }


    private static LocalizedLabel GetDiscountNameLabel(SummaryItem summaryItem)
    {
        return new LocalizedLabel { Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(summaryItem.Name)) + ":", EnableViewState = false };
    }


    private Label GetDiscountValueLabel(SummaryItem summaryItem)
    {
        return new Label { Text = "- " + CurrencyInfoProvider.GetFormattedPrice(summaryItem.Value, ShoppingCart.Currency), EnableViewState = false };
    }


    private static Literal GetEmptyLiteral()
    {
        return new Literal { Text = "<br />", EnableViewState = false };
    }


    /// <summary>
    /// Returns price detail link.
    /// </summary>
    protected string GetPriceDetailLink(object value)
    {
        var priceDetailElemHtml = "";

        if (ShoppingCartControl.EnableProductPriceDetail)
        {
            Guid cartItemGuid = ValidationHelper.GetGuid(value, Guid.Empty);
            if (cartItemGuid != Guid.Empty)
            {
                var query = "itemguid=" + cartItemGuid + GetCMSDeskShoppingCartSessionNameQuery();

                string adminUrl = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "order.productpricedetail", 0, query);

                var priceDetailButton = new CMSGridActionButton
                {
                    IconCssClass = "icon-eye",
                    ToolTip = GetString("shoppingcart.productpricedetail"),
                    OnClientClick = ScriptHelper.GetModalDialogScript(adminUrl, "ProductPriceDetail", 750, 570)
                };

                priceDetailElemHtml = priceDetailButton.GetRenderedHTML();
            }
        }

        return priceDetailElemHtml;
    }


    /// <summary>
    /// Returns shopping cart session name in cart query parameter.
    /// </summary>
    private string GetCMSDeskShoppingCartSessionNameQuery()
    {
        var cartSessionName = "";

        switch (ShoppingCartControl.CheckoutProcessType)
        {
            case CheckoutProcessEnum.CMSDeskOrder:
                cartSessionName = "CMSDeskNewOrderShoppingCart";
                break;
            case CheckoutProcessEnum.CMSDeskCustomer:
                cartSessionName = "CMSDeskNewCustomerOrderShoppingCart";
                break;
            case CheckoutProcessEnum.CMSDeskOrderItems:
                cartSessionName = "CMSDeskOrderItemsShoppingCart";
                break;
        }

        return (!string.IsNullOrEmpty(cartSessionName)) ? "&cart=" + cartSessionName : "";
    }


    private void HandlePostBack()
    {
        if (!RequestHelper.IsPostBack())
        {
            return;
        }

        var eventArgument = Page.Request.Params.Get("__EVENTARGUMENT");
        if (!string.IsNullOrEmpty(eventArgument))
        {
            var data = eventArgument.Split(':');
            if (data.Length == 2 && data[0] == "couponCode")
            {
                var couponCode = data[1];
                ShoppingCart.RemoveCouponCode(couponCode);

                ReloadData();
            }
        }
    }


    #region "Binding methods"

    /// <summary>
    /// Returns formatted currency value.
    /// </summary>
    /// <param name="value">Value to format</param>
    protected string GetFormattedValue(object value)
    {
        var price = ValidationHelper.GetDecimal(value, 0);
        return CurrencyInfoProvider.GetFormattedValue(price, ShoppingCart.Currency);
    }


    /// <summary>
    /// Returns formatted and localized SKU name.
    /// </summary>
    /// <param name="value">SKU Name</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="itemText">Inserted text for product option of 'text' type</param>
    protected string GetSKUName(object value, object isProductOption, object itemText)
    {
        string name = ResHelper.LocalizeString((string)value);
        string text = itemText as string;
        StringBuilder skuName = new StringBuilder();

        // If it is a product option or bundle item
        if (ValidationHelper.GetBoolean(isProductOption, false))
        {
            skuName.Append("<span style=\"font-size: 90%\"> - ");
            skuName.Append(HTMLHelper.HTMLEncode(name));

            if (!string.IsNullOrEmpty(text))
            {
                skuName.Append(" '" + HTMLHelper.HTMLEncode(text) + "'");
            }

            skuName.Append("</span>");
        }
        // If it is a parent product
        else
        {
            skuName.Append(HTMLHelper.HTMLEncode(name));
        }

        return skuName.ToString();
    }


    protected bool GetBoolean(object value)
    {
        return ValidationHelper.GetBoolean(value, false);
    }


    /// <summary>
    /// Returns order item edit action HTML.
    /// </summary>
    protected string GetOrderItemEditAction(object value)
    {
        var editOrderItemElemHtml = "";
        Guid itemGuid = ValidationHelper.GetGuid(value, Guid.Empty);

        if (itemGuid != Guid.Empty)
        {
            var item = ShoppingCartInfoProvider.GetShoppingCartItem(ShoppingCart, itemGuid);

            // Hide edit link for attribute product option
            if (item.IsAttributeOption)
            {
                return null;
            }

            var query = "itemguid=" + itemGuid + GetCMSDeskShoppingCartSessionNameQuery();
            var editItemUrl = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "order.OrderItemProperties", 0, query);

            var priceDetailButton = new CMSGridActionButton
            {
                IconCssClass = "icon-edit",
                IconStyle = GridIconStyle.Allow,
                ToolTip = GetString("shoppingcart.editorderitem"),
                OnClientClick = ScriptHelper.GetModalDialogScript(editItemUrl, "OrderItemEdit", 720, 420)
            };

            editOrderItemElemHtml = priceDetailButton.GetRenderedHTML();
        }

        return editOrderItemElemHtml;
    }


    /// <summary>
    /// Returns SKU edit action HTML.
    /// </summary>
    protected string GetSKUEditAction(object skuId, object skuSiteId, object skuParentSkuId, object isProductOption)
    {
        var editSKUElemHtml = "";

        if (!ValidationHelper.GetBoolean(isProductOption, false))
        {
            // Do not render product detail link, when not authorized
            if (!CanReadProducts)
            {
                return editSKUElemHtml;
            }

            // Show variants tab for product variant, otherwise general tab
            int parentSkuId = ValidationHelper.GetInteger(skuParentSkuId, 0);
            string productIdParam = (parentSkuId == 0) ? skuId.ToString() : parentSkuId.ToString();
            string tabParam = (parentSkuId == 0) ? "Products.General" : "Products.Variants";

            var query = URLHelper.AddParameterToUrl("", "productid", productIdParam);
            query = URLHelper.AddParameterToUrl(query, "tabName", tabParam);
            query = URLHelper.AddParameterToUrl(query, "siteid", skuSiteId.ToString());
            query = URLHelper.AddParameterToUrl(query, "dialog", "1");
            var url = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "Products.Properties", additionalQuery: query);

            // Different tooltip for product than for product variant
            string tooltip = (parentSkuId == 0) ? GetString("shoppingcart.editproduct") : GetString("shoppingcart.editproductvariant");

            var priceDetailButton = new CMSGridActionButton
            {
                IconCssClass = "icon-box",
                ToolTip = tooltip,
                OnClientClick = ScriptHelper.GetModalDialogScript(url, "SKUEdit", "95%", "95%"),
            };

            editSKUElemHtml = priceDetailButton.GetRenderedHTML();
        }

        return editSKUElemHtml;
    }


    /// <summary>
    /// Returns formatted child cart item units. Returns empty string if it is not product option or bundle item.
    /// </summary>
    /// <param name="skuUnits">SKU units</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    protected static string GetChildCartItemUnits(object skuUnits, object isProductOption)
    {
        if (ValidationHelper.GetBoolean(isProductOption, false))
        {
            return string.Format("<span>{0}</span>", skuUnits);
        }

        return "";
    }


    /// <summary>
    /// Returns button usable for removing shopping cart coupon code.
    /// </summary>
    /// <param name="couponCode">Discount coupon code value</param>
    protected static string GetDiscountCouponCodeRemoveButton(string couponCode)
    {
        var text = ResHelper.GetString("general.remove");
        var css = "btn btn-default";

        return $"<input type=\"submit\" value=\"{text}\" class=\"{css}\" id=\"btn_{couponCode}\" onclick=\"{{ __doPostBack('', 'couponCode:{couponCode}'); }} return false;\" />";
    }

    #endregion
}
