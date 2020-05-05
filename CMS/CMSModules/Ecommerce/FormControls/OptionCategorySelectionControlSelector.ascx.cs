using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Ecommerce_FormControls_OptionCategorySelectionControlSelector : FormEngineUserControl
{
    #region "Private properties"

    /// <summary>
    /// Type of option category. Used to indicate if category type is being changed.
    /// </summary>
    private OptionCategoryTypeEnum OriginalOptionCategoryType
    {
        get
        {
            return ValidationHelper.GetString(ViewState["OriginalOptionCategoryType"], "").ToEnum<OptionCategoryTypeEnum>();
        }
        set
        {
            ViewState["OriginalOptionCategoryType"] = ValidationHelper.GetString(value.ToStringRepresentation(), "");
        }
    }


    /// <summary>
    /// Type of option category which is selected right now in the form.
    /// </summary>
    private OptionCategoryTypeEnum OptionCategoryType
    {
        get;
        set;
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Value of option category selection control.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpSelectionTypeEnum.SelectedValue;
        }
        set
        {
            if (drpSelectionTypeEnum.Items.Count == 0)
            {
                // Init selection options based on option category type
                OptionCategoryType = ValidationHelper.GetString(Form.Data.GetValue(OptionCategoryTypeColumn), "").ToEnum<OptionCategoryTypeEnum>();
                ConfigureSelectionTypeControl();

                // Remember category type to be able to compare it with currently selected option category type
                OriginalOptionCategoryType = OptionCategoryType;
            }

            drpSelectionTypeEnum.SelectedValue = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Name of column with option category type value.
    /// </summary>
    public string OptionCategoryTypeColumn
    {
        get
        {
            return GetValue("OptionCategoryTypeColumn", string.Empty);
        }
        set
        {
            SetValue("OptionCategoryTypeColumn", value);
        }
    }

    #endregion


    #region "Life cycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Form != null)
        {
            // Option category type currently selected in the form
            OptionCategoryType = ValidationHelper.GetString(Form.FieldControls[OptionCategoryTypeColumn].Value, "").ToEnum<OptionCategoryTypeEnum>();

            if (OptionCategoryType != OriginalOptionCategoryType)
            {
                // Delete all options and fill drop down list according to the current category type
                drpSelectionTypeEnum.Items.Clear();
                ConfigureSelectionTypeControl();

                // Remember category type
                OriginalOptionCategoryType = OptionCategoryType;
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Populates drop down list with available enum options.
    /// </summary>
    private void ConfigureSelectionTypeControl()
    {
        // Display only appropriate selection types based on selected option category type
        ControlsHelper.FillListControlWithEnum(drpSelectionTypeEnum, typeof(OptionCategorySelectionTypeEnum), "optioncategory_selectiontype", false, true, GetExcludedOptions(OptionCategoryType));
    }


    /// <summary>
    /// Find OptionCategorySelectionTypeEnum option which are not available for given option category type
    /// </summary>
    /// <param name="optionCategoryType">Currently selected category type in the form</param>
    private List<string> GetExcludedOptions(OptionCategoryTypeEnum optionCategoryType)
    {
        var excludedOptions = new List<string>();

        switch (optionCategoryType)
        {
            case OptionCategoryTypeEnum.Attribute:
                excludedOptions.Add(OptionCategorySelectionTypeEnum.TextArea.ToStringRepresentation());
                excludedOptions.Add(OptionCategorySelectionTypeEnum.TextBox.ToStringRepresentation());
                excludedOptions.Add(OptionCategorySelectionTypeEnum.CheckBoxesHorizontal.ToStringRepresentation());
                excludedOptions.Add(OptionCategorySelectionTypeEnum.CheckBoxesVertical.ToStringRepresentation());

                break;

            case OptionCategoryTypeEnum.Products:
                excludedOptions.Add(OptionCategorySelectionTypeEnum.TextArea.ToStringRepresentation());
                excludedOptions.Add(OptionCategorySelectionTypeEnum.TextBox.ToStringRepresentation());

                break;

            case OptionCategoryTypeEnum.Text:
                excludedOptions.Add(OptionCategorySelectionTypeEnum.CheckBoxesHorizontal.ToStringRepresentation());
                excludedOptions.Add(OptionCategorySelectionTypeEnum.CheckBoxesVertical.ToStringRepresentation());
                excludedOptions.Add(OptionCategorySelectionTypeEnum.Dropdownlist.ToStringRepresentation());
                excludedOptions.Add(OptionCategorySelectionTypeEnum.RadioButtonsHorizontal.ToStringRepresentation());
                excludedOptions.Add(OptionCategorySelectionTypeEnum.RadioButtonsVertical.ToStringRepresentation());

                break;
        }

        return excludedOptions;
    }

    #endregion
}