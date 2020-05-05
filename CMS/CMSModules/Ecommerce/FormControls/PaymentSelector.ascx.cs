using System;
using System.Data;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_PaymentSelector : SiteSeparatedObjectSelector
{
    #region "Properties"

    /// <summary>
    ///  If true, selected value is PaymentName, if false, selected value is PaymentOptionID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UsePaymentNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UsePaymentNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Gets a column name for shipping option ID column.
    /// </summary>
    public string ShippingOptionIDColumnName
    {
        get
        {
            return GetValue("ShippingOptionIDColumnName", String.Empty);
        }
        set
        {
            SetValue("ShippingOptionIDColumnName", value);
        }
    }


    /// <summary>
    /// Allows to access selector object.
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Indicates if only payment options that are allowed to be used without shipping are displayed.
    /// </summary>
    public bool DisplayOnlyAllowedIfNoShipping
    {
        get;
        set;
    }


    /// <summary>
    /// Shopping cart.
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get
        {
            return GetValue("ShoppingCart") as ShoppingCartInfo;
        }
        set
        {
            SetValue("ShoppingCart", value);
        }
    }

    #endregion


    #region "Life cycle"

    /// <summary>
    /// Sets up ShippingOptionIDColumnName property if shipping option id column name is configured.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!String.IsNullOrEmpty(ShippingOptionIDColumnName))
        {
            var shippingID = (Form != null) ? ValidationHelper.GetInteger(Form.Data.GetValue(ShippingOptionIDColumnName), 0) : 0;
            DisplayOnlyAllowedIfNoShipping = (shippingID == 0);
        }
    }
    
    #endregion


    #region "Methods"

    /// <summary>
    /// Convert given payment option name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the payment option to be converted.</param>
    /// <param name="siteName">Name of the site of the payment option.</param>
    protected override int GetID(string name, string siteName)
    {
        var payment = PaymentOptionInfo.Provider.Get(name, SiteInfoProvider.GetSiteID(siteName));

        return (payment != null) ? payment.PaymentOptionID : 0;
    }


    /// <summary>
    /// Appends where condition filtering only payments marked with PaymentOptionAllowIfNoShipping flag when requested.
    /// </summary>
    /// <param name="where">Original where condition.</param>
    protected override string AppendExclusiveWhere(string where)
    {
        // Filter out only payment options that are allowed to be used without shipping
        if (DisplayOnlyAllowedIfNoShipping)
        {
            where = SqlHelper.AddWhereCondition(where, "PaymentOptionAllowIfNoShipping = 1");
        }

        return base.AppendExclusiveWhere(where);
    }


    /// <summary>
    /// Ensures that only applicable payment options are displayed in selector.
    /// </summary>
    /// <param name="ds">Dataset with payment options.</param>
    protected override DataSet OnAfterRetrieveData(DataSet ds)
    {
        if (DataHelper.IsEmpty(ds) || (ShoppingCart == null))
        {
            return ds;
        }

        foreach (DataRow paymentRow in ds.Tables[0].Select())
        {
            PaymentOptionInfo paymentOptionInfo;

            if (UseNameForSelection)
            {
                var paymentName = DataHelper.GetStringValue(paymentRow, "PaymentOptionName");
                paymentOptionInfo = PaymentOptionInfo.Provider.Get(paymentName, ShoppingCart.ShoppingCartSiteID);
            }
            else
            {
                var paymentID = DataHelper.GetIntValue(paymentRow, "PaymentOptionID");
                paymentOptionInfo = PaymentOptionInfo.Provider.Get(paymentID);
            }

            // Do not remove already selected item even if the option is not applicable anymore 
            // The user would see a different value in UI as is actually stored in the database
            var canBeRemoved = !EnsureSelectedItem || (ShoppingCart.ShoppingCartPaymentOptionID != paymentOptionInfo.PaymentOptionID);
            if (canBeRemoved && !PaymentOptionInfoProvider.IsPaymentOptionApplicable(ShoppingCart, paymentOptionInfo))
            {
                // Remove not applicable payment methods from the selector
                ds.Tables[0].Rows.Remove(paymentRow);
            }
        }

        return ds;
    }

    #endregion
}