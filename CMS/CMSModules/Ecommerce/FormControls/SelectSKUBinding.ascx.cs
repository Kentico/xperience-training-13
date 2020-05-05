using System;

using CMS.Base;

using System.Linq;

using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_FormControls_SelectSKUBinding : FormEngineUserControl
{
    #region "Constants and variables"

    public const string CREATE_NEW = "CREATE_NEW";
    public const string CREATE_NEW_GLOBAL = "CREATE_NEW_GLOBAL";
    public const string USE_EXISTING = "USE_EXISTING";
    public const string DO_NOT_CREATE = "DO_NOT_CREATE";

    private bool mAllowDoNotCreate = true;
    private bool mAllowUseExisting = true;
    private bool mAllowCreateNewGlobal = true;
    private bool mAllowCreateNew = true;

    #endregion


    #region "Properties - general"

    /// <summary>
    /// Gets or sets the selected SKU binding option value as string.
    /// Use the defined string constants to compare or set the binding value.
    /// </summary>
    public override object Value
    {
        get
        {
            return Binding;
        }
        set
        {
            Binding = ValidationHelper.GetString(value, null);
        }
    }

    /// <summary>
    /// Get or sets enabled property to base and containing radio buttons.
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
            
            // change if not checked
            radCreateNewGlobal.Enabled = radCreateNewGlobal.Checked || value;
            radUseExisting.Enabled = radUseExisting.Checked || value;
            radDoNotCreate.Enabled = radDoNotCreate.Checked || value;
            radCreateNew.Enabled = radCreateNew.Checked || value;
        }
    }


    /// <summary>
    /// Gets or sets the selected SKU binding option value as string.
    /// Use the defined string constants to compare or set the binding value.
    /// </summary>
    public string Binding
    {
        get
        {
            if (radCreateNew.Checked)
            {
                return CREATE_NEW;
            }
            else if (radCreateNewGlobal.Checked)
            {
                return CREATE_NEW_GLOBAL;
            }
            else if (radUseExisting.Checked)
            {
                return USE_EXISTING;
            }
            else if (radDoNotCreate.Checked)
            {
                return DO_NOT_CREATE;
            }
            return CREATE_NEW;
        }
        set
        {
            radCreateNew.Checked = value == CREATE_NEW;
            radCreateNewGlobal.Checked = value == CREATE_NEW_GLOBAL;
            radUseExisting.Checked = value == USE_EXISTING;
            radDoNotCreate.Checked = value == DO_NOT_CREATE;
        }
    }

    /// <summary>
    /// Gets the SKU ID of the selected SKU for the "Use an existing SKU" binding option.
    /// </summary>
    public int SelectedProduct
    {
        get
        {
            return ValidationHelper.GetInteger(skuSelectorElem.Value, 0);
        }
    }


    /// <summary>
    /// Gets or sets information text which is displayed above the binding options.
    /// </summary>
    public string InfoText
    {
        get
        {
            return lblInfo.Text;
        }
        set
        {
            lblInfo.Text = value;
        }
    }

    #endregion


    #region "Properties - options"

    /// <summary>
    /// Allows the "Create a new SKU" option to be available for selection.
    /// </summary>
    public bool AllowCreateNew
    {
        get
        {
            return mAllowCreateNew;
        }
        set
        {
            mAllowCreateNew = value;
        }
    }


    /// <summary>
    /// Allows the "Create a new global SKU" option to be available for selection.
    /// </summary>
    public bool AllowCreateNewGlobal
    {
        get
        {
            return ECommerceSettings.AllowGlobalProducts(SiteContext.CurrentSiteName) && mAllowCreateNewGlobal;
        }
        set
        {
            mAllowCreateNewGlobal = value;
        }
    }


    /// <summary>
    /// Allows the "Use an existing SKU" option to be available for selection.
    /// </summary>
    public bool AllowUseExisting
    {
        get
        {
            return mAllowUseExisting;
        }
        set
        {
            mAllowUseExisting = value;
        }
    }


    /// <summary>
    /// Allows the "Do not create an SKU" option to be available for selection.
    /// </summary>
    public bool AllowDoNotCreate
    {
        get
        {
            return mAllowDoNotCreate;
        }
        set
        {
            mAllowDoNotCreate = value;
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        InitSkuSelector();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        InitOptions();
    }

    #endregion


    #region "Initialization"

    private void InitOptions()
    {
        // Display or hide info text
        lblInfo.Visible = !string.IsNullOrEmpty(InfoText);

        // Display or hide options
        radCreateNew.Visible = AllowCreateNew;
        pnlCreateNewGlobal.Visible = AllowCreateNewGlobal;
        radUseExisting.Visible = AllowUseExisting;
        pnlDoNotCreate.Visible = AllowDoNotCreate;
    }


    private void InitSkuSelector()
    {
        skuSelectorElem.SiteID = SiteContext.CurrentSiteID;
        skuSelectorElem.UniSelector.AddGlobalObjectSuffix = true;
        skuSelectorElem.DisplayGlobalProducts = AllowCreateNewGlobal;
        pnlSkuSelector.Visible = (Binding == USE_EXISTING);
    }

    #endregion


    #region "Validation"

    /// <summary>
    /// Validates the input data and returns true if it is valid, otherwise returns false.
    /// </summary>
    public override bool IsValid()
    {
        if ((Binding == USE_EXISTING) && (SelectedProduct <= 0))
        {
            ValidationError = GetString("com.selectskubinding.useexistingempty");
            return false;
        }

        return true;
    }

    #endregion


    #region "Other"

    /// <summary>
    /// On post back gets the SKU binding value directly from the request.
    /// Returns null if the value is not present.
    /// </summary>
    public string GetValueFromRequest()
    {
        if (RequestHelper.IsPostBack())
        {
            string radioGoupKey = Request.Form.AllKeys.FirstOrDefault(k => (!string.IsNullOrEmpty(k)) && k.EndsWithCSafe("SelectSKUBindingRadioGroup"));

            if (!string.IsNullOrEmpty(radioGoupKey))
            {
                string skuBinding = Request.Form[radioGoupKey];
                if (skuBinding == radCreateNew.ID)
                {
                    return CREATE_NEW;
                }
                else if (skuBinding == radCreateNewGlobal.ID)
                {
                    return CREATE_NEW_GLOBAL;
                }
                else if (skuBinding == radUseExisting.ID)
                {
                    return USE_EXISTING;
                }
                else if (skuBinding == radDoNotCreate.ID)
                {
                    return DO_NOT_CREATE;
                }
            }
        }

        return Binding;
    }


    protected void radioButton_CheckedChanged(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }

    #endregion
}