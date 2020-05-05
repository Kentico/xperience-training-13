using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_Controls_UI_PriceSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtPrice.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the price value.
    /// When set, the price value is formatted according to the specified currency.
    /// </summary>
    public override object Value
    {
        get
        {
            if (string.IsNullOrEmpty(txtPrice.Text))
            {
                return null;
            }
            return ValidationHelper.GetDecimal(txtPrice.Text, 0);
        }
        set
        {
            txtPrice.Text = ValidationHelper.IsDecimal(value)
                ? GetFormattedPrice(ValidationHelper.GetDecimal(value, 0))
                : null;
        }
    }


    /// <summary>
    /// Gets a value that indicates if price is being edited for product option.
    /// </summary>
    public bool IsProductOption
    {
        get
        {
            // Try to get the value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUOptionCategoryID"))
            {
                return ValidationHelper.GetInteger(Form.Data.GetValue("SKUOptionCategoryID"), 0) > 0;
            }

            return false;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site specifying which main currency has to be used for the price formatting.
    /// The default value is the current site ID.
    /// </summary>
    public int CurrencySiteID
    {
        get
        {
            if (mCurrencySiteID >= 0)
            {
                return mCurrencySiteID;
            }

            // Try to get the value from SKU form data
            if (Form != null)
            {
                if (Form.Data.ContainsColumn("SKUSiteID"))
                {
                    int siteId = ValidationHelper.GetInteger(Form.Data.GetValue("SKUSiteID"), 0);
                    if (siteId >= 0)
                    {
                        return siteId;
                    }
                }

                // Get ID of the site of edited object
                BaseInfo info = Form.Data as BaseInfo;
                if ((info != null) && (info.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
                {
                    return info.Generalized.ObjectSiteID;
                }

                // Get ID of the site from parent of edited object
                UIForm uiForm = Form as UIForm;

                // Get ID of the site of parent object
                BaseInfo parent = uiForm?.ParentObject;
                if (parent != null && (parent.TypeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN))
                {
                    return parent.Generalized.ObjectSiteID;
                }
            }

            return SiteContext.CurrentSiteID;
        }
        set
        {
            mCurrencySiteID = value;
            mCurrency = null;
        }
    }
    private int mCurrencySiteID = -1;


    /// <summary>
    /// Gets or sets the currency used for the price formatting.
    /// </summary>
    public CurrencyInfo Currency
    {
        get
        {
            return mCurrency ?? (mCurrency = CurrencyInfoProvider.GetMainCurrency(CurrencySiteID));
        }
        set
        {
            mCurrency = value;
            mCurrencySiteID = value != null ? mCurrency.CurrencySiteID : -1;
        }
    }
    private CurrencyInfo mCurrency;


    /// <summary>
    /// Gets or sets the message which is displayed by the required field validator of the price textbox.
    /// </summary>
    public string EmptyErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmptyErrorMessage"), GetString("general.requiresvalue"));
        }
        set
        {
            SetValue("EmptyErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed by the range field validator of the price textbox.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RangeErrorMessage"), GetString("com.invalidprice"));
        }
        set
        {
            SetValue("RangeErrorMessage", value);
        }
    }


    /// <summary>
    /// Indicates if the currency code is displayed next to the price textbox.
    /// The default value is true.
    /// </summary>
    public bool ShowCurrencyCode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCurrencyCode"), lblCurrencyCode.Visible);
        }
        set
        {
            SetValue("ShowCurrencyCode", value);
            lblCurrencyCode.Visible = value;
        }
    }


    /// <summary>
    /// Indicates if zero price is considered to be a valid value.
    /// The default value is true.
    /// </summary>
    public bool AllowZero
    {
        get
        {
            bool allowZero = ValidationHelper.GetBoolean(ViewState["AllowZero"], true);
            return ValidationHelper.GetBoolean(GetValue("AllowZero"), allowZero);
        }
        set
        {
            SetValue("AllowZero", value);
            ViewState["AllowZero"] = value;
        }
    }


    /// <summary>
    /// Indicates if zero price is considered to be a valid value.
    /// The default value is false.
    /// </summary>
    public bool AllowNegative
    {
        get
        {
            bool allowNegative = ValidationHelper.GetBoolean(ViewState["AllowNegative"], false);
            return ValidationHelper.GetBoolean(GetValue("AllowNegative"), allowNegative);
        }
        set
        {
            SetValue("AllowNegative", value);
            ViewState["AllowNegative"] = value;
        }
    }


    /// <summary>
    /// Indicates if empty price is considered to be a valid value.
    /// The default value is false.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEmpty"), mAllowEmpty);
        }
        set
        {
            SetValue("AllowEmpty", value);
            mAllowEmpty = value;
        }
    }
    private bool mAllowEmpty;


    /// <summary>
    /// Indicates if price should be formatted.
    /// The default value is false.
    /// </summary>
    public bool FormattedPrice
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FormattedPrice"), false);
        }
        set
        {
            SetValue("FormattedPrice", value);
        }
    }


    /// <summary>
    /// Indicates if value is formatted as integer in case it is a whole number.
    /// The default is false.
    /// </summary>
    public bool FormatValueAsInteger
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FormatValueAsInteger"), mFormatValueAsInteger);
        }
        set
        {
            SetValue("FormatValueAsInteger", value);
            mFormatValueAsInteger = value;
        }
    }
    private bool mFormatValueAsInteger;

    #endregion


    #region "Lifecycle methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblCurrencyCode.Visible = ShowCurrencyCode;
        lblCurrencyCode.Text = HTMLHelper.HTMLEncode(Currency?.CurrencyCode);

        // Pass the CSS class defined in form definition to the inner textbox
        if (!String.IsNullOrEmpty(CssClass))
        {
            txtPrice.AddCssClass(CssClass);
            CssClass = null;
        }
    }

    #endregion


    #region "Validation methods"

    /// <summary>
    /// Returns true if the price value is valid, otherwise returns false.
    /// </summary>
    public override bool IsValid()
    {
        ValidationError = Validate(IsProductOption).FirstOrDefault();

        return String.IsNullOrEmpty(ValidationError);
    }


    private IEnumerable<string> Validate(bool isProductOption)
    {
        string price = txtPrice.Text.Trim();

        // Validate empty value
        if (string.IsNullOrEmpty(price))
        {
            if (AllowEmpty)
            {
                yield break;
            }

            yield return GetString(EmptyErrorMessage);
        }

        if (isProductOption || AllowNegative)
        {
            // The price value can be negative for a product option
            if (!ValidationHelper.IsDecimal(price))
            {
                yield return GetValidationErrorMessage();
            }
        }
        else if (ValidationHelper.GetDecimal(price, -1) < 0)
        {
            // The price value cannot be negative for a basic product
            yield return GetValidationErrorMessage();
        }

        var decimalPrice = ValidationHelper.GetDecimal(price, 0);

        // Validate zero value
        if (!AllowZero && decimalPrice == 0)
        {
            yield return GetValidationErrorMessage();
        }

        if (Currency != null)
        {
            var decimals = Currency.CurrencyRoundTo;
            var roundedDecimalPrice = Math.Round(decimalPrice, decimals);

            // Price cannot have more decimal fields than the currency configuration allows
            if (decimalPrice != roundedDecimalPrice)
            {
                yield return GetValidationErrorMessage();
            }
        }
    }


    private string GetValidationErrorMessage()
    {
        var message = GetString(ValidationErrorMessage);

        return String.Format(message, Currency.CurrencyCode, Currency.CurrencyRoundTo);
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns the formatted price value with the number of digits according to the currency significant digits settings.
    /// Formats the price value as an integer if enabled and if the price value is a whole number.
    /// </summary>
    /// <param name="value">The price value to be formatted</param>
    private string GetFormattedPrice(decimal value)
    {
        // Do not format price if it is not required
        if (!FormattedPrice)
        {
            return ValidationHelper.GetString(value, "0");
        }

        // Format the price value as an integer
        if (FormatValueAsInteger)
        {
            var truncatedValue = Math.Truncate(value);
            if (value == truncatedValue)
            {
                return truncatedValue.ToString("0");
            }
        }

        return CurrencyInfoProvider.GetFormattedValue(value, Currency);
    }

    #endregion
}