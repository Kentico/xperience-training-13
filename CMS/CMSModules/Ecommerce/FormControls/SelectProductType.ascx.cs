using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;


public partial class CMSModules_Ecommerce_FormControls_SelectProductType : FormEngineUserControl
{
    #region "Variables"

    private bool mAutoPostback = false;
    private string mValue = null;
    private bool mAllowAll = false;
    private string mAllItemResourceString = null;
    private bool mAllowNone = false;
    private string mNoneItemResourceString = null;
    private bool mAllowStandardProduct = true;
    private bool mAllowMembership = true;
    private bool mAllowEproduct = true;
    private bool mAllowBundle = true;
    private bool mAllowText = false;

    #endregion


    #region "Properties - general"

    /// <summary>
    /// Gets or sets enabled state of the control.
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
            drpProductType.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating whether a postback to the server automatically occurs when the selection is changed by the user.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoPostBack"), mAutoPostback);
        }
        set
        {
            SetValue("AutoPostBack", value);
            mAutoPostback = value;
            drpProductType.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected product type as a string.
    /// </summary>
    public override object Value
    {
        get
        {
            return mValue;
        }
        set
        {
            mValue = ValidationHelper.GetString(value, null);
            drpProductType.SelectedValue = mValue;
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


    public bool IsTextProductOption
    {
        get
        {
            // Try to get the value from SKU form data
            if ((Form != null) && Form.Data.ContainsColumn("SKUOptionCategoryID"))
            {
                var optionCategoryId = ValidationHelper.GetInteger(Form.Data.GetValue("SKUOptionCategoryID"), 0);
                var optionCategory = OptionCategoryInfo.Provider.Get(optionCategoryId);
                if (optionCategory != null)
                {
                    return optionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextBox || optionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextArea;
                }
            }

            return false;
        }
    }

    #endregion


    #region "Properties - options"

    /// <summary>
    /// Gets or sets a value that indicates if "all" option is available for selection.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowAll"), mAllowAll);
        }
        set
        {
            SetValue("AllowAll", value);
            mAllowAll = value;
        }
    }


    /// <summary>
    /// Gets or sets the text for "all" option.
    /// </summary>
    public string AllItemResourceString
    {
        get
        {
            string text = ValidationHelper.GetString(GetValue("AllItemResourceString"), mAllItemResourceString);
            return text ?? "general.selectall";
        }
        set
        {
            SetValue("AllItemResourceString", value);
            mAllItemResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets a value that indicates if "none" option is available for selection.
    /// </summary>
    public bool AllowNone
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowNone"), mAllowNone);
        }
        set
        {
            SetValue("AllowNone", value);
            mAllowNone = value;
        }
    }


    /// <summary>
    /// Gets or sets the text for "none" option.
    /// </summary>
    public string NoneItemResourceString
    {
        get
        {
            string text = ValidationHelper.GetString(GetValue("NoneItemResourceString"), mNoneItemResourceString);
            return text ?? "general.selectnone";
        }
        set
        {
            SetValue("NoneItemResourceString", value);
            mNoneItemResourceString = value;
        }
    }


    /// <summary>
    /// Indicates if standard product type option is available for selection.
    /// </summary>
    public bool AllowStandardProduct
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowStandardProduct"), mAllowStandardProduct);
        }
        set
        {
            SetValue("AllowStandardProduct", value);
            mAllowStandardProduct = value;
        }
    }


    /// <summary>
    /// Indicates if membership product type option is available for selection.
    /// </summary>
    public bool AllowMembership
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowMembership"), mAllowMembership);
        }
        set
        {
            SetValue("AllowMembership", value);
            mAllowMembership = value;
        }
    }


    /// <summary>
    /// Indicates if e-product product type option is available for selection.
    /// </summary>
    public bool AllowEproduct
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEproduct"), mAllowEproduct);
        }
        set
        {
            SetValue("AllowEproduct", value);
            mAllowEproduct = value;
        }
    }


    /// <summary>
    /// Indicates if bundle product type option is available for selection.
    /// </summary>
    public bool AllowBundle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowBundle"), mAllowBundle);
        }
        set
        {
            SetValue("AllowBundle", value);
            mAllowBundle = value;
        }
    }


    /// <summary>
    /// Indicates if text product type option is available for selection.
    /// </summary>
    public bool AllowText
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowText"), mAllowText);
        }
        set
        {
            SetValue("AllowText", value);
            mAllowText = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Selection changed event.
    /// </summary>
    public event EventHandler SelectionChanged;


    private void RaiseSelectionChanged()
    {
        if (SelectionChanged != null)
        {
            SelectionChanged(this, EventArgs.Empty);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (drpProductType.Items.Count == 0)
        {
            InitOptions();
            TryPreselectByForm();
            drpProductType.SelectedValue = ValidationHelper.GetString(Value, null);
        }
        else
        {
            Value = drpProductType.SelectedValue;
        }

        drpProductType.Enabled = Enabled;
        drpProductType.AutoPostBack = AutoPostBack;

        drpProductType.SelectedIndexChanged += (sender, args) =>
        {
            Value = drpProductType.SelectedValue;
            RaiseSelectionChanged();
        };
    }

    #endregion


    #region "Initialization"

    /// <summary>
    /// Initializes the selector options.
    /// </summary>
    public void InitOptions()
    {
        if (AllowNone && !IsTextProductOption)
        {
            AddStringOption(GetString(NoneItemResourceString), "NONE");
        }

        if (AllowAll && !IsTextProductOption)
        {
            AddStringOption(GetString(AllItemResourceString), "ALL");
        }

        if (AllowStandardProduct && !IsTextProductOption)
        {
            AddProductTypeOption(SKUProductTypeEnum.Product);
        }

        // Add membership option when membership feature is available and membership option is allowed
        if (LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.Membership) && AllowMembership && !IsTextProductOption)
        {
            AddProductTypeOption(SKUProductTypeEnum.Membership);
        }

        if (AllowEproduct && !IsTextProductOption)
        {
            AddProductTypeOption(SKUProductTypeEnum.EProduct);
        }

        if (AllowBundle && !IsProductOption && !IsTextProductOption)
        {
            AddProductTypeOption(SKUProductTypeEnum.Bundle);
        }

        if (AllowText && IsTextProductOption)
        {
            AddProductTypeOption(SKUProductTypeEnum.Text);
        }
    }


    private void AddStringOption(string text, string value)
    {
        drpProductType.Items.Add(new ListItem(text, value));
    }


    private void AddProductTypeOption(SKUProductTypeEnum value)
    {
        AddStringOption(value.ToLocalizedString("com.producttype"), value.ToStringRepresentation());
    }


    private void TryPreselectByForm()
    {
        if (Form == null)
        {
            return;
        }

        if (Form.AdditionalData.ContainsKey("DataClassID"))
        {
            var dataClassId = ValidationHelper.GetInteger(Form.AdditionalData["DataClassID"], 0);
            var dataClass = DataClassInfoProvider.GetDataClassInfo(dataClassId);
            if (dataClass != null)
            {
                Value = dataClass.ClassSKUDefaultProductType;
            }
            else
            {
                // Select first visible item in dropdown if Value has not been set yet
                if ((Value == null) && (drpProductType.Items.Count > 0))
                {
                    Value = drpProductType.Items[0].Value;
                }
            }
        }
    }

    #endregion
}