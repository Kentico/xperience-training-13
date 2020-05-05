using System;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;


public partial class CMSModules_Ecommerce_Controls_PaymentGateways_CreditPaymentForm : CMSPaymentGatewayForm
{
    private CMSCreditPaymentProvider CreditPaymentProvider => PaymentProvider as CMSCreditPaymentProvider;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (CreditPaymentProvider == null)
        {
            return;
        }

        // If user is not authorized to finish payment
        if (!CreditPaymentProvider.IsUserAuthorizedToFinishPayment(true))
        {
            // Display error message
            lblError.Visible = true;
            lblError.Text = CreditPaymentProvider.ErrorMessage;

            lblCredit.Visible = false;
            lblCreditValue.Visible = false;
        }
        else
        {
            CreditPaymentProvider.ReloadPaymentData();
        }
    }


    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (CreditPaymentProvider != null)
        {
            CreditPaymentProvider.ReloadAction = DisplayAvailableCredit;
        }
    }


    /// <summary>
    /// Displays available credit.
    /// </summary>
    private void DisplayAvailableCredit()
    {
        if (CreditPaymentProvider?.MainCurrencyObj == null || CreditPaymentProvider.OrderCurrencyObj == null)
        {
            return;
        }

        // Order currency is different from main currency
        if (CreditPaymentProvider.MainCurrencyObj.CurrencyID != CreditPaymentProvider.OrderCurrencyObj.CurrencyID)
        {
            // Set available credit string
            lblCreditValue.Text = CreditPaymentProvider.OrderCurrencyObj.FormatPrice(CreditPaymentProvider.AvailableCreditInOrderCurrency);
            lblCreditValue.Text += "(" + CreditPaymentProvider.MainCurrencyObj.FormatPrice(CreditPaymentProvider.AvailableCreditInMainCurrency) + ")";
        }
        // Order currency is equal to main currency
        else
        {
            lblCreditValue.Text = CreditPaymentProvider.MainCurrencyObj.FormatPrice(CreditPaymentProvider.AvailableCreditInMainCurrency);
        }
    }
}