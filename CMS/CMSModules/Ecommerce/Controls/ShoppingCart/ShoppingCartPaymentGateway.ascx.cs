using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;


public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPaymentGateway : ShoppingCartStep, IGatewayFormLoader
{
    private const string ORDER_PROPERTIES_UI_ELEMENT = "OrderProperties";
    private const string ORDER_BILLING_UI_ELEMENT = "Orders.Billing";

    private CMSPaymentGatewayForm mPaymentDataForm;
    private PaymentResultInfo mPaymentResult;


    protected void Page_Load(object sender, EventArgs e)
    {
        // No payment provider loaded -> skip payment
        if (ShoppingCartControl.PaymentGatewayProvider == null)
        {
            // Clean current order payment result when editing existing order and payment was skipped
            if ((ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems) &&
                !ShoppingCartControl.IsCurrentStepPostBack)
            {
                CleanUpOrderPaymentResult();
            }

            // Raise payment skipped
            ShoppingCartControl.RaisePaymentSkippedEvent();
        }
        else if (ShoppingCart != null)
        {
            LoadData();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Set buttons properties
        if (!IsPaymentCompletedOrAuthorized() || ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        {
            // Show 'Skip payment' button
            ShoppingCartControl.ButtonBack.ButtonStyle = ButtonStyle.Default;
            ShoppingCartControl.ButtonBack.Text = GetString("ShoppingCart.PaymentGateway.SkipPayment");

            // Show 'Finish payment' button
            ShoppingCartControl.ButtonNext.ButtonStyle = ButtonStyle.Primary;
            ShoppingCartControl.ButtonNext.Text = GetString("ShoppingCart.PaymentGateway.FinishPayment");
        }
    }


    public override void ButtonNextClickAction()
    {
        // Standard action - Process payment
        base.ButtonNextClickAction();

        // Payment was authorized or completed
        if (IsPaymentCompletedOrAuthorized())
        {
            // Remove current shopping cart data from session and from database
            ShoppingCartControl.CleanUpShoppingCart();
        }
        else
        {
            // Payment is not yet completed or authorized, but requires approval
            var approvalUrl = mPaymentResult?.PaymentApprovalUrl;
            if (!string.IsNullOrEmpty(approvalUrl))
            {
                RedirectPayment();
            }
        }
    }


    public override void ButtonBackClickAction()
    {
        // Payment was skipped
        ShoppingCartControl.RaisePaymentSkippedEvent();

        // Remove current shopping cart data from session and from database
        ShoppingCartControl.CleanUpShoppingCart();
    }


    public override bool IsValid()
    {
        return mPaymentDataForm != null && string.IsNullOrEmpty(mPaymentDataForm.ValidateData());
    }


    public override bool ProcessStep()
    {
        if (ShoppingCartControl.PaymentGatewayProvider != null)
        {
            // Skip payment when already paid except when editing existing order or user is not authorized
            if (((!IsPaymentCompletedOrAuthorized()) || (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)) &&
                ShoppingCartControl.PaymentGatewayProvider.IsUserAuthorizedToFinishPayment(MembershipContext.AuthenticatedUser, ShoppingCartControl.ShoppingCartInfoObj, true))
            {
                var data = mPaymentDataForm.GetPaymentGatewayData();
                mPaymentResult = Service.Resolve<IPaymentGatewayProcessor>().ProcessPayment(ShoppingCartControl.PaymentGatewayProvider, data);
            }

            // Show info message
            if (!string.IsNullOrEmpty(ShoppingCartControl.PaymentGatewayProvider.InfoMessage))
            {
                lblInfo.Visible = true;
                lblInfo.Text = ShoppingCartControl.PaymentGatewayProvider.InfoMessage;
            }

            // Show error message
            if (!string.IsNullOrEmpty(ShoppingCartControl.PaymentGatewayProvider.ErrorMessage))
            {
                lblError.Visible = true;
                lblError.Text = ShoppingCartControl.PaymentGatewayProvider.ErrorMessage;
                return false;
            }

            if (IsPaymentCompleted())
            {
                // Delete current shopping cart after successful payment attempt
                ShoppingCartControl.CleanUpShoppingCart();

                // Raise payment completed event
                ShoppingCartControl.RaisePaymentCompletedEvent();

                return true;
            }

            if (IsPaymentAuthorized())
            {
                ShoppingCartControl.CleanUpShoppingCart();

                return true;
            }
        }

        return false;
    }


    private void LoadData()
    {
        // Payment summary
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.GrandTotal, ShoppingCart.Currency);
        lblOrderIdValue.Text = Convert.ToString(ShoppingCart.OrderId);
        if (ShoppingCart.PaymentOption != null)
        {
            lblPaymentValue.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ShoppingCart.PaymentOption.PaymentOptionDisplayName));
        }

        // Add payment gateway custom data
        var provider = ShoppingCartControl.PaymentGatewayProvider;
        mPaymentDataForm = Service.Resolve<IPaymentGatewayFormFactory>().GetPaymentGatewayForm(provider, this);

        if (mPaymentDataForm != null)
        {
            PaymentDataContainer.Controls.Clear();
            PaymentDataContainer.Controls.Add(mPaymentDataForm);
        }

        var customerUser = ShoppingCartControl.ShoppingCartInfoObj?.Customer?.CustomerUser;

        // Disable next button for unauthorized payment method
        if (!ShoppingCartControl.PaymentGatewayProvider.IsUserAuthorizedToFinishPayment(customerUser, ShoppingCartControl.ShoppingCartInfoObj, true))
        {
            ShoppingCartControl.ButtonNext.Enabled = false;
        }

        // Show "Order saved" info message
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            lblInfo.Text = GetString("ShoppingCart.PaymentGateway.OrderSaved");
            lblInfo.Visible = true;
        }
        else
        {
            lblInfo.Text = "";
        }
    }


    private bool IsPaymentCompletedOrAuthorized()
    {
        return IsPaymentCompleted() || IsPaymentAuthorized();
    }


    private bool IsPaymentCompleted()
    {
        return ShoppingCartControl?.PaymentGatewayProvider != null && ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted;
    }


    private bool IsPaymentAuthorized()
    {
        if (ShoppingCartControl?.PaymentGatewayProvider == null)
        {
            return false;
        }

        var delayedProvider = ShoppingCartControl.PaymentGatewayProvider as IDelayedPaymentGatewayProvider;
        return delayedProvider != null && delayedProvider.IsPaymentAuthorized;
    }


    /// <summary>
    /// Redirects request to order detail.
    /// </summary>
    private void RedirectPayment()
    {
        var url = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, ORDER_PROPERTIES_UI_ELEMENT, false, ShoppingCartControl.PaymentGatewayProvider.OrderId);
        url = URLHelper.AddParameterToUrl(url, "tabname", ORDER_BILLING_UI_ELEMENT);

        URLHelper.Redirect(UrlResolver.ResolveUrl(url));
    }


    /// <summary>
    /// Clean up current order payment result.
    /// </summary>
    private void CleanUpOrderPaymentResult()
    {
        if (ShoppingCart != null)
        {
            OrderInfo oi = OrderInfo.Provider.Get(ShoppingCart.OrderId);
            if (oi != null)
            {
                oi.OrderPaymentResult = null;
                OrderInfo.Provider.Set(oi);
            }
        }
    }


    public CMSPaymentGatewayForm LoadFormControl(string path)
    {
        return LoadControl(path) as CMSPaymentGatewayForm;
    }
}