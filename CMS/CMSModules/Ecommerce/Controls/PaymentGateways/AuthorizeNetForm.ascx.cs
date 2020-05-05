using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Ecommerce_Controls_PaymentGateways_AuthorizeNetForm : CMSPaymentGatewayForm
{
    #region "Private properties"

    /// <summary>
    /// Credit card number.
    /// </summary>
    private string CreditCardNumber => txtCardNumber.Text.Trim();


    /// <summary>
    /// Credit card CCV (Card Code Verification).
    /// </summary>
    private string CreditCardCCV => txtCCV.Text.Trim();


    /// <summary>
    /// Credit card expiration date.
    /// </summary>
    private DateTime CreditCardExpiration
    {
        get
        {
            if (string.IsNullOrEmpty(drpMonths.SelectedValue) || string.IsNullOrEmpty(drpYears.SelectedValue))
            {
                return DateTimeHelper.ZERO_TIME;
            }

            var expiration = DateTime.MinValue;
            expiration = expiration.AddYears(Convert.ToInt32(drpYears.SelectedValue) - 1);
            expiration = expiration.AddMonths(Convert.ToInt32(drpMonths.SelectedValue) - 1);

            return expiration;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        lblCCV.Attributes["style"] = "cursor:help;";

        // Load dropdown lists
        if ((drpMonths.Items.Count == 0) || (drpYears.Items.Count == 0))
        {
            InitializeLists();
        }
    }


    /// <summary>
    /// Returns payment related data stored in form.
    /// </summary>
    public override IDictionary<string, object> GetPaymentGatewayData()
    {
        var data = base.GetPaymentGatewayData();

        data[AuthorizeNetParameters.CARD_NUMBER] = CreditCardNumber;
        data[AuthorizeNetParameters.CARD_CCV] = CreditCardCCV;
        data[AuthorizeNetParameters.CARD_EXPIRATION] = CreditCardExpiration;

        return data;
    }


    /// <summary>
    /// Validates customer payment data.
    /// </summary>
    public override string ValidateData()
    {
        var errorMessage = base.ValidateData();

        if (!string.IsNullOrEmpty(errorMessage))
        {
            lblError.Visible = true;
            lblError.Text = HTMLHelper.EnsureHtmlLineEndings(HTMLHelper.HTMLEncode(errorMessage));
        }
        else
        {
            lblError.Visible = false;
        }

        return errorMessage;
    }


    /// <summary>
    /// Loads data to dropdownlists.
    /// </summary>
    private void InitializeLists()
    {
        // Load years
        for (int i = 0; i < 10; i++)
        {
            string year = Convert.ToString(DateTime.Now.Year + i);
            drpYears.Items.Add(new ListItem(year, year));
        }
        drpYears.Items.Insert(0, new ListItem("-", ""));


        // Load months
        for (int i = 1; i <= 12; i++)
        {
            string text = (i < 10) ? "0" + i : i.ToString();
            drpMonths.Items.Add(new ListItem(text, i.ToString()));
        }
        drpMonths.Items.Insert(0, new ListItem("-", ""));
    }
}