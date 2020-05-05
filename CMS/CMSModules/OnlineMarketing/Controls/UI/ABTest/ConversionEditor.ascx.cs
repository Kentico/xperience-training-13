using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.OnlineMarketing;

public partial class CMSModules_OnlineMarketing_Controls_UI_ABTest_ConversionEditor : FormEngineUserControl
{
    #region "Variables"

    private ABTestConversion mConversion;
    private FormEngineUserControl mItemSelector;
    private bool mParamsInitialized;
    private const string DEFAULT_PLACEHOLDER_VALUE = "1";

    #endregion


    #region "Properties"

    /// <summary>
    /// Control works with <see cref="ABTestConversion"/>.
    /// </summary>
    public override object Value
    {
        get
        {
            return mConversion = CreateConversion();
        }
        set
        {
            mConversion = (ABTestConversion)value;
            if (mConversion != null)
            {
                SelectedConversion = mConversion.ConversionOriginalName;
            }
            else
            {
                // Reset inner controls
                SelectedConversion = null;

                drpConversions.ClearSelection();
            }

            mParamsInitialized = false;
        }
    }


    /// <summary>
    /// Codename of selected conversion type.
    /// </summary>
    private string SelectedConversion
    {
        get
        {
            string selectedValue = ValidationHelper.GetString(ViewState["ConversionType"], string.Empty);
            if (String.IsNullOrEmpty(selectedValue))
            {
                selectedValue = drpConversions.SelectedValue;
                ViewState["ConversionType"] = selectedValue;
            }

            return selectedValue;
        }
        set
        {
            ViewState["ConversionType"] = value;
        }
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Customized LoadViewState.
    /// </summary>
    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(((Pair)savedState).First);

        if (!StopProcessing)
        {
            InitializeControl();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!mParamsInitialized && !StopProcessing)
        {
            InitializeControl(true);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!mParamsInitialized && !StopProcessing)
        {
            InitializeControl(true);
        }

        pnlItem.Visible = (mItemSelector != null);
    }


    /// <summary>
    /// Handles conversion selector change.
    /// </summary>
    protected void selector_OnSelectionChanged(object sender, EventArgs e)
    {
        SelectedConversion = drpConversions.SelectedValue;
        InitializeControl(true);
    }


    /// <summary>
    /// Customized SaveViewState.
    /// </summary>
    protected override object SaveViewState()
    {
        return new Pair(base.SaveViewState(), null);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true when control's inputs are valid.
    /// </summary>
    public override bool IsValid()
    {
        bool result = true;

        if (!IsSelectorValid(out string errorMsg))
        {
            lblItemError.Visible = true;
            lblItemError.Text = errorMsg;
            result = false;
        }

        if ((txtValue.Text.Trim() != String.Empty) && (txtValue.Text != txtValue.WatermarkText) && !ValidationHelper.IsDecimal(txtValue.Text))
        {
            lblValueError.Visible = true;
            result = false;
        }

        return result;
    }


    /// <summary>
    /// Validates related item selector for emptiness and correct selection.
    /// </summary>
    /// <param name="result">Out parameter for error message</param>
    private bool IsSelectorValid(out string result)
    {
        result = null;

        if (mItemSelector == null)
        {
            return true;
        }

        var value = mItemSelector.ValueForValidation;
        if ((value == null) || String.IsNullOrWhiteSpace(Convert.ToString(value)))
        {
            result = GetString("BasicForm.ErrorEmptyValue");
            return false;
        }

        if (!mItemSelector.IsValid())
        {
            result = mItemSelector.ValidationError;
            return false;
        }

        return true;
    }


    /// <summary>
    /// Initializes inner controls.
    /// </summary>
    /// <param name="forceReload">Indicates if inner controls should be reloaded</param>
    private void InitializeControl(bool forceReload = false)
    {
        InitializeConversionSelector();

        InitializeItemSelector(forceReload);

        SetConversionValuePlaceholder();
        txtValue.Text = String.Empty;

        if (mConversion != null)
        {
            if (mItemSelector != null)
            {
                mItemSelector.Value = mConversion.RelatedItemIdentifier;
            }
            if (mConversion.Value != 0)
            {
                txtValue.Text = mConversion.Value.ToString();
                txtValue.WatermarkText = String.Empty;
            }
        }

        mParamsInitialized = true;
    }


    /// <summary>
    /// Initializes selector with available conversion types ordered by the conversion display name.
    /// </summary>
    private void InitializeConversionSelector()
    {
        if (drpConversions.Items.Count == 0)
        {
            var items = ABTestConversionDefinitionRegister.Instance.Items
                .Select(i => new ListItem(ResHelper.LocalizeString(i.ConversionDisplayName), i.ConversionName))
                .OrderBy(i => i.Text)
                .ToArray();
            drpConversions.Items.AddRange(items);
        }

        drpConversions.SelectedValue = SelectedConversion;
    }


    /// <summary>
    /// Initializes item selector form control based on selected conversion definition.
    /// </summary>
    /// <param name="force">Forces reload of the selector if <c>true</c></param>
    private void InitializeItemSelector(bool force)
    {
        var conversionDefinition = ABTestConversionDefinitionRegister.Instance.Get(SelectedConversion);
        var controlName = conversionDefinition?.FormControlDefinition?.FormControlName;
        var formControlType = conversionDefinition?.FormControlDefinition?.FormControlType;

        if (force)
        {
            plcItemControl.Controls.Clear();
            mItemSelector = null;
        }

        var formUserControl = FormUserControlInfoProvider.GetFormUserControlInfo(controlName);
        if (formUserControl == null && formControlType == null)
        {
            return;
        }

        mItemSelector = formControlType != null
            ? (FormEngineUserControl)Activator.CreateInstance(formControlType)
            : FormUserControlLoader.LoadFormControl(Page, controlName, "", loadDefaultProperties: false);

        if (mItemSelector != null)
        {
            mItemSelector.ID = "item" + SelectedConversion;
            mItemSelector.IsLiveSite = false;

            var controlParameters = FormHelper.GetFormControlParameters(controlName, formUserControl?.UserControlMergedParameters, false);
            if (controlParameters != null)
            {
                mItemSelector.LoadDefaultProperties(controlParameters);
            }

            var parameters = conversionDefinition?.FormControlDefinition?.FormControlParameters;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    mItemSelector.SetValue(parameter.Key, parameter.Value);
                }
            }

            plcItemControl.Controls.Clear();
            plcItemControl.Controls.Add(mItemSelector);

            lblItem.AssociatedControlID = mItemSelector.ID;
            SetItemSelectorCaption(conversionDefinition?.FormControlDefinition?.FormControlCaption);
        }
    }


    private void SetItemSelectorCaption(string selectorCaption)
    {
        var caption = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(selectorCaption));
        if (String.IsNullOrWhiteSpace(caption))
        {
            caption = GetString("abtest.conversion.relatedobject");
        }

        lblItem.Text = caption;
    }


    private void SetConversionValuePlaceholder()
    {
        var conversionDefinition = ABTestConversionDefinitionRegister.Instance.Get(SelectedConversion);
        txtValue.WatermarkText = ResHelper.LocalizeString(conversionDefinition.DefaultValuePlaceholderText ?? DEFAULT_PLACEHOLDER_VALUE);
    }


    /// <summary>
    /// Creates new <see cref="ABTestConversion"/> object based on inputs.
    /// </summary>
    private ABTestConversion CreateConversion()
    {
        if (!IsValid())
        {
            return null;
        }

        var conversionValue = ValidationHelper.GetDecimal(txtValue.Text, txtValue.WatermarkText == DEFAULT_PLACEHOLDER_VALUE ? 1m : 0m);
        var conversion = new ABTestConversion(SelectedConversion, mItemSelector?.Value.ToString(), conversionValue);
        conversion.RelatedItemDisplayName = mItemSelector?.ValueDisplayName;

        return conversion;
    }

    #endregion
}