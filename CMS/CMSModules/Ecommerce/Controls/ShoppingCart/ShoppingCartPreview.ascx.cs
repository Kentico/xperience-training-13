using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPreview : ShoppingCartStep
{
    #region "ViewState Constants"

    private const string ORDER_NOTE = "OrderNote";

    #endregion


    private SiteInfo currentSite;
    private int mAddressCount = 1;


    protected void Page_Load(object sender, EventArgs e)
    {
        currentSite = SiteContext.CurrentSite;

        ShoppingCartControl.ButtonNext.Text = GetString("Ecommerce.OrderPreview.NextButtonText");

        // Addresses initialization
        FillBillingAddressForm(ShoppingCart.ShoppingCartBillingAddress);
        FillShippingAddressForm(ShoppingCart.ShoppingCartShippingAddress);

        // Load company address
        if (ShoppingCart.ShoppingCartCompanyAddress != null)
        {
            lblCompany.Text = GetAddressHTML(ShoppingCart.ShoppingCartCompanyAddress);
            mAddressCount++;
            tdCompanyAddress.Visible = true;
        }
        else
        {
            tdCompanyAddress.Visible = false;
        }

        // Enable sending order notifications when creating order from CMSDesk
        if ((ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrder) ||
            ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskCustomer)
        {
            chkSendEmail.Visible = true;
            chkSendEmail.Checked = ECommerceSettings.SendOrderNotification(currentSite.SiteName);
            chkSendEmail.Text = GetString("ShoppingCartPreview.SendNotification");
        }

        // Show tax registration ID and organization ID
        InitializeIDs();

        // shopping cart content table initialization
        gridData.Columns[4].HeaderText = GetString("Ecommerce.ShoppingCartContent.SKUName");
        gridData.Columns[5].HeaderText = GetString("Ecommerce.ShoppingCartContent.SKUUnits");
        gridData.Columns[6].HeaderText = GetString("Ecommerce.ShoppingCartContent.UnitPrice");
        gridData.Columns[7].HeaderText = GetString("Ecommerce.ShoppingCartContent.Subtotal");

        // Product tax summary table initialization
        gridTaxSummary.Columns[0].HeaderText = GetString("Ecommerce.CartContent.TaxDisplayName");
        gridTaxSummary.Columns[1].HeaderText = GetString("Ecommerce.CartContent.TaxSummary");

        ReloadData();

        // Order note initialization
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            // Try to select payment from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(ORDER_NOTE);
            txtNote.Text = (viewStateValue != null) ? Convert.ToString(viewStateValue) : ShoppingCart.ShoppingCartNote;
        }

        if (mAddressCount == 2)
        {
            tblAddressPreview.Attributes["class"] = "AddressPreviewWithTwoColumns";
        }
        else if (mAddressCount == 3)
        {
            tblAddressPreview.Attributes["class"] = "AddressPreviewWithThreeColumns";
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Hide columns with identifiers
        gridData.Columns[0].Visible = false;
        gridData.Columns[1].Visible = false;
        gridData.Columns[2].Visible = false;
        gridData.Columns[3].Visible = false;

        // Disable default button in the order preview to
        // force approval of the order by mouse click
        if (ShoppingCartControl.ShoppingCartContainer != null)
        {
            ShoppingCartControl.ShoppingCartContainer.DefaultButton = "";
        }

        // Display/hide error message
        lblError.Visible = !string.IsNullOrEmpty(lblError.Text.Trim());
    }


    protected void ReloadData()
    {
        // Recalculate shopping cart
        ShoppingCart.Evaluate();

        gridData.DataSource = ShoppingCart.ContentTable;
        gridData.DataBind();

        gridTaxSummary.DataSource = ShoppingCart.TaxSummary;
        gridTaxSummary.DataBind();

        // Show order related discounts
        if (ShoppingCart.OrderDiscountSummary != null)
        {
            plcOrderDiscounts.Visible = ShoppingCart.OrderDiscountSummary.Count > 0;
            ShoppingCart.OrderDiscountSummary.ToList().ForEach(d =>
            {
                plcOrderDiscountNames.Controls.Add(GetDiscountNameLiteral(d));
                plcOrderDiscountValues.Controls.Add(GetDiscountValueLiteral(d));
            });
        }

        // Show gift card related discounts
        if (ShoppingCart.OtherPaymentsSummary != null)
        {
            plcOtherPayments.Visible = ShoppingCart.OtherPaymentsSummary.Count > 0;
            ShoppingCart.OtherPaymentsSummary.ToList().ForEach(d =>
            {
                plcOtherPaymentsNames.Controls.Add(GetDiscountNameLiteral(d));
                plcOtherPaymentsValues.Controls.Add(GetDiscountValueLiteral(d));
            });
        }

        // shipping option, payment method initialization
        InitPaymentShipping();
    }


    private static Literal GetDiscountNameLiteral(SummaryItem summaryItem)
    {
        return new Literal { Text = $"{HTMLHelper.HTMLEncode(ResHelper.LocalizeString(summaryItem.Name))}:<br />", EnableViewState = false };
    }


    private Literal GetDiscountValueLiteral(SummaryItem summaryItem)
    {
        return new Literal { Text = $"- {CurrencyInfoProvider.GetFormattedPrice(summaryItem.Value, ShoppingCart.Currency)}<br />", EnableViewState = false };
    }


    /// <summary>
    /// Fills billing address form.
    /// </summary>
    /// <param name="address">Billing address</param>
    protected void FillBillingAddressForm(AddressInfo address)
    {
        lblBill.Text = GetAddressHTML(address);
    }


    /// <summary>
    /// Fills shipping address form.
    /// </summary>
    /// <param name="address">Shipping address</param>
    protected void FillShippingAddressForm(AddressInfo address)
    {
        lblShip.Text = GetAddressHTML(address);
    }


    /// <summary>
    /// Back button actions.
    /// </summary>
    public override void ButtonBackClickAction()
    {
        // Save the values to ShoppingCart ViewState
        string note = TextHelper.LimitLength(txtNote.Text.Trim(), txtNote.MaxLength);

        ShoppingCartControl.SetTempValue(ORDER_NOTE, note);

        base.ButtonBackClickAction();
    }


    /// <summary>
    /// Validates shopping cart content.
    /// </summary>
    public override bool IsValid()
    {
        // Force loading current values
        ShoppingCart.Evaluate();

        // Validate inventory
        var cartValidationErrors = ShoppingCartInfoProvider.ValidateShoppingCart(ShoppingCart);
        var shippingAndPaymentValidator = new ShippingAndPaymentValidator(ShoppingCart);

        shippingAndPaymentValidator.Validate();

        if (cartValidationErrors.Any() || !shippingAndPaymentValidator.IsValid)
        {
            lblError.Text = cartValidationErrors
                .Union(shippingAndPaymentValidator.Errors)
                .Select(e => HTMLHelper.HTMLEncode(e.GetMessage()))
                .Join("<br />");

            return false;
        }

        return true;
    }


    /// <summary>
    /// Saves order information from ShoppingCartInfo object to database as new order.
    /// </summary>
    public override bool ProcessStep()
    {
        // Load first step if there is no address
        if (ShoppingCart.ShoppingCartBillingAddress == null)
        {
            ShoppingCartControl.LoadStep(0);
            return false;
        }

        // Deal with order note
        ShoppingCartControl.SetTempValue(ORDER_NOTE, null);
        ShoppingCart.ShoppingCartNote = TextHelper.LimitLength(txtNote.Text.Trim(), txtNote.MaxLength);

        try
        {
            // Set order culture
            ShoppingCart.ShoppingCartCulture = LocalizationContext.PreferredCultureCode;

            // Create order
            ShoppingCartInfoProvider.SetOrder(ShoppingCart);
        }
        catch (Exception ex)
        {
            // Show error
            lblError.Text = GetString("Ecommerce.OrderPreview.ErrorOrderSave");

            // Log exception
            Service.Resolve<IEventLogService>().LogException("Shopping cart", "SAVEORDER", ex, ShoppingCart.ShoppingCartSiteID);
            return false;
        }

        // Track order activity
        string siteName = SiteContext.CurrentSiteName;

        if (LogActivityForCustomer)
        {
            var mainCurrency = CurrencyInfoProvider.GetMainCurrency(ShoppingCart.ShoppingCartSiteID);
            var grandTotalInMainCurrency = Service.Resolve<ICurrencyConverterService>().Convert(ShoppingCart.GrandTotal, ShoppingCart.Currency.CurrencyCode, mainCurrency.CurrencyCode, ShoppingCart.ShoppingCartSiteID);
            var formattedPrice = CurrencyInfoProvider.GetFormattedPrice(grandTotalInMainCurrency, mainCurrency);

            ShoppingCartControl.TrackActivityPurchasedProducts(ShoppingCart, siteName, ContactID);
            ShoppingCartControl.TrackActivityPurchase(ShoppingCart.OrderId, ContactID,
                                                      SiteContext.CurrentSiteName, RequestContext.CurrentRelativePath,
                                                      grandTotalInMainCurrency, formattedPrice);
        }

        // Raise finish order event
        ShoppingCartControl.RaiseOrderCompletedEvent();

        if (chkSendEmail.Checked)
        {
            // Send order notification emails
            OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
            OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
        }

        return true;
    }


    protected void InitPaymentShipping()
    {
        if (currentSite != null)
        {
            // Get shipping option name
            ShippingOptionInfo shippingObj = ShoppingCart.ShippingOption;
            if (shippingObj != null)
            {
                mAddressCount++;
                tdShippingAddress.Visible = true;
                plcShipping.Visible = true;
                plcShippingOption.Visible = true;
                lblShippingOptionValue.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(shippingObj.ShippingOptionDisplayName));
                lblShippingValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalShipping, ShoppingCart.Currency);
            }
            else
            {
                tdShippingAddress.Visible = false;
                plcShippingOption.Visible = false;
                plcShipping.Visible = false;
            }
        }

        // Get payment method name
        PaymentOptionInfo paymentObj = PaymentOptionInfo.Provider.Get(ShoppingCart.ShoppingCartPaymentOptionID);
        if (paymentObj != null)
        {
            lblPaymentMethodValue.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(paymentObj.PaymentOptionDisplayName));
        }

        // Total price initialization
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.GrandTotal, ShoppingCart.Currency);
        lblTotalTaxValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalTax, ShoppingCart.Currency);
    }


    /// <summary>
    /// Initializes tax registration ID and organization ID.
    /// </summary>
    protected void InitializeIDs()
    {
        SiteInfo si = SiteContext.CurrentSite;
        if (si != null)
        {
            if ((ECommerceSettings.ShowOrganizationID(si.SiteName)) && (ShoppingCart.Customer != null) && (ShoppingCart.Customer.CustomerOrganizationID != string.Empty))
            {
                // Initialize organization ID
                plcIDs.Visible = true;
                lblOrganizationIDVal.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ShoppingCart.Customer.CustomerOrganizationID));
            }
            else
            {
                lblOrganizationID.Visible = false;
                lblOrganizationIDVal.Visible = false;
            }
            if ((ECommerceSettings.ShowTaxRegistrationID(si.SiteName)) && (ShoppingCart.Customer != null) && (ShoppingCart.Customer.CustomerTaxRegistrationID != string.Empty))
            {
                // Initialize tax registration ID
                plcIDs.Visible = true;
                lblTaxRegistrationIDVal.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ShoppingCart.Customer.CustomerTaxRegistrationID));
            }
            else
            {
                lblTaxRegistrationID.Visible = false;
                lblTaxRegistrationIDVal.Visible = false;
            }
        }
    }


    /// <summary>
    /// Returns formatted value string.
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
    /// <param name="value">SKU name</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="itemText">Text parameter of product. Will be appended in quotes at the end of SKU name</param>
    protected string GetSKUName(object value, object isProductOption, object itemText)
    {
        string name = ResHelper.LocalizeString((string)value);

        // If it is a product option or bundle item
        if (ValidationHelper.GetBoolean(isProductOption, false))
        {
            string text = itemText as string;
            return $"<span style=\"font-size:90%\"> - {HTMLHelper.HTMLEncode(name)}{(!string.IsNullOrEmpty(text) ? " '" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(text)) + "'" : "")}</span>";
        }

        // If it is a parent product
        return HTMLHelper.HTMLEncode(name);
    }


    #region "Address formatting"

    /// <summary>
    /// Returns html code that represents address. Used for generating of invoice.
    /// </summary>
    /// <param name="address">Address to be formatted</param>
    private string GetAddressHTML(AddressInfo address)
    {
        if (address == null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder("<table class=\"TextLeft\">");

        // Personal name
        sb.AppendFormat("<tr><td>{0}</td></tr>", HTMLHelper.HTMLEncode(address.AddressPersonalName));

        // Line 1
        if (address.AddressLine1 != "")
        {
            sb.AppendFormat("<tr><td>{0}</td></tr>", HTMLHelper.HTMLEncode(address.AddressLine1));
        }

        // Line 2
        if (address.AddressLine2 != "")
        {
            sb.AppendFormat("<tr><td>{0}</td></tr>", HTMLHelper.HTMLEncode(address.AddressLine2));
        }

        // City + (State) + Postal Code
        sb.Append("<tr><td>", HTMLHelper.HTMLEncode(address.AddressCity));

        var state = StateInfo.Provider.Get(address.AddressStateID);
        if (state != null)
        {
            sb.Append(", ", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(state.StateDisplayName)));
        }

        sb.AppendFormat(" {0}</td></tr>", HTMLHelper.HTMLEncode(address.AddressZip));

        // Country
        var country = CountryInfo.Provider.Get(address.AddressCountryID);
        if (country != null)
        {
            sb.AppendFormat("<tr><td>{0}</td></tr>", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(country.CountryDisplayName)));
        }

        // Phone
        sb.AppendFormat("<tr><td>{0}</td></tr></table>", HTMLHelper.HTMLEncode(address.AddressPhone));

        return sb.ToString();
    }

    #endregion
}