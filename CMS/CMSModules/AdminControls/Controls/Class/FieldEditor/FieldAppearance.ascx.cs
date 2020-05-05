using System;

using CMS.Base;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_FieldAppearance : CMSUserControl
{
    #region "Events"

    /// <summary>
    /// Fired when DDL with form controls has been changed.
    /// </summary>
    public event EventHandler OnFieldSelected
    {
        add
        {
            drpControl.OnSelectionChanged += value;
        }
        remove
        {
            drpControl.OnSelectionChanged -= value;
        }
    }

    #endregion


    #region "Variables"

    public Func<FieldEditorControlsEnum, FieldEditorModeEnum, bool, FieldEditorControlsEnum> GetControls;

    #endregion


    #region "Properties"

    /// <summary>
    /// Type of field.
    /// </summary>
    public string AttributeType
    {
        get
        {
            return ValidationHelper.GetString(ViewState["AttributeType"], null);
        }
        set
        {
            drpControl.DataType = value;
            ViewState["AttributeType"] = value;
        }
    }


    /// <summary>
    /// Gets or sets selected control name. Use constants defined in <see cref="FormFieldControlName"/> class, or code name of a custom control.
    /// </summary>
    public string FieldType
    {
        get
        {
            return drpControl.Text;
        }
        set
        {
            if (!String.IsNullOrEmpty(value))
            {
                drpControl.Value = value;
            }
        }
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get;
        set;
    }


    /// <summary>
    /// Class name.
    /// </summary>
    public string ClassName
    {
        get;
        set;
    }


    /// <summary>
    /// FormFieldInfo of given field.
    /// </summary>
    public FormFieldInfo FieldInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Type of custom controls that can be selected from the control list in FieldEditor.
    /// </summary>
    public FieldEditorControlsEnum DisplayedControls { get; set; } = FieldEditorControlsEnum.ModeSelected;


    /// <summary>
    /// Gets or sets value indicating if control is inheritable or not.
    /// </summary>
    public bool ControlInheritable
    {
        get
        {
            return chkControlInheritable.Checked;
        }
        set
        {
            chkControlInheritable.Checked = value;
        }
    }


    /// <summary>
    /// Enables or disables to edit <see cref="CMS.FormEngine.FormFieldInfo.Inheritable"/> settings.
    /// </summary>
    public bool ShowInheritanceSettings
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if field editor works in development mode.
    /// </summary>
    public bool DevelopmentMode
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets macro resolver used in macro editor.
    /// </summary>
    public string ResolverName
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        drpControl.ReturnColumnName = FormUserControlInfo.TYPEINFO.CodeNameColumn;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Disable autosave on LocalizableTextBox controls
        FormEngineUserControl descriptionControl = (FormEngineUserControl)txtDescription.NestedControl;
        if (descriptionControl != null)
        {
            descriptionControl.SetValue("AutoSave", false);
        }
        FormEngineUserControl fieldCaptionControl = (FormEngineUserControl)txtFieldCaption.NestedControl;
        if (fieldCaptionControl != null)
        {
            fieldCaptionControl.SetValue("AutoSave", false);
        }
        FormEngineUserControl explanationTextControl = (FormEngineUserControl)txtExplanationText.NestedControl;
        if (explanationTextControl != null)
        {
            explanationTextControl.SetValue("AutoSave", false);
        }

        drpControl.DataType = AttributeType;

        plcInheritance.Visible = ShowInheritanceSettings;

        txtFieldCaption.ResolverName = ResolverName;
        txtDescription.ResolverName = ResolverName;
        txtExplanationText.ResolverName = ResolverName;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblFieldCaption.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtFieldCaption.NestedControl.Controls);
        lblFieldDescription.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtDescription.NestedControl.Controls);
        lblExplanationText.AssociatedControlClientID = EditingFormControl.GetInputClientID(txtExplanationText.NestedControl.Controls);
        lblField.AssociatedControlClientID = drpControl.InputClientID;

        // Check if the form control is allowed for selected data type
        var control = FormUserControlInfoProvider.GetFormUserControlInfo(drpControl.Value.ToString());

        string dataTypeColumn = FormHelper.GetDataTypeColumn(AttributeType);
        if ((control != null) && !String.IsNullOrEmpty(dataTypeColumn))
        {
            bool allowType = ValidationHelper.GetBoolean(control[dataTypeColumn], false);
            if (!allowType)
            {
                var warning = HTMLHelper.HTMLEncode(String.Format(GetString("fieldeditor.controlnotallowedfordatatype"), control.UserControlDisplayName));

                ShowWarning(warning);
            }
        }
    }


    /// <summary>
    /// Loads field with values from FormFieldInfo.
    /// </summary>
    public void Reload()
    {
        if (FieldInfo != null)
        {
            bool isMacro;
            txtDescription.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.FieldDescription, out isMacro), isMacro);
            txtFieldCaption.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, out isMacro), isMacro);
            txtExplanationText.SetValue(FieldInfo.GetPropertyValue(FormFieldPropertyEnum.ExplanationText, out isMacro), isMacro);

            if (ShowInheritanceSettings)
            {
                chkControlInheritable.Checked = FieldInfo.Inheritable;
            }

            var selectedItem = String.Empty;
            var controlName = FieldInfo.GetControlName();
            // Get control name from settings
            if (!String.IsNullOrEmpty(controlName))
            {
                selectedItem = controlName;
            }
            
            var fi = FormUserControlInfoProvider.GetFormUserControlInfo(selectedItem);

            FieldType = (fi != null) ? selectedItem : FormHelper.GetFormFieldDefaultControlName(AttributeType, GetControlsToDisplay());

            LoadFieldTypes(FieldInfo.PrimaryKey);
        }
        // If FormFieldInfo is not specified then clear form
        else
        {
            txtFieldCaption.SetValue(null);
            txtDescription.SetValue(null);
            txtExplanationText.SetValue(null);
            drpControl.Reload(false);
            drpControl.Value = FormHelper.GetFormFieldDefaultControlName(AttributeType, GetControlsToDisplay());
        }
    }


    /// <summary>
    /// Fill field types list. Form control types will be restricted to actual selection in Form control types drop-down list.
    /// </summary>
    /// <param name="isPrimary">Determines whether the attribute is primary key</param>
    public void LoadFieldTypes(bool isPrimary)
    {
        var controls = GetControlsToDisplay();

        drpControl.WhereCondition = FormHelper.GetWhereConditionForDataType(AttributeType, controls, isPrimary);

        var originalValue = FieldType;

        drpControl.Reload(true);

        var newValue = FieldType;

        // If previously selected control is not available, load default control
        if (!String.Equals(originalValue, newValue, StringComparison.InvariantCultureIgnoreCase))
        {
            FieldType = FormHelper.GetFormFieldDefaultControlName(AttributeType, controls);
        }
    }


    private FieldEditorControlsEnum GetControlsToDisplay()
    {
        return GetControls(DisplayedControls, Mode, DevelopmentMode);
    }


    /// <summary>
    /// Save the properties to form field info.
    /// </summary>
    /// <returns>True if success</returns>
    public bool Save()
    {
        if (FieldInfo != null)
        {
            // Save LocalizableTextBox controls
            LocalizableFormEngineUserControl descriptionControl = (LocalizableFormEngineUserControl)txtDescription.NestedControl;
            if (descriptionControl != null)
            {
                descriptionControl.Save();
            }
            LocalizableFormEngineUserControl fieldCaptionControl = (LocalizableFormEngineUserControl)txtFieldCaption.NestedControl;
            if (fieldCaptionControl != null)
            {
                fieldCaptionControl.Save();
            }
            LocalizableFormEngineUserControl explanationTextControl = (LocalizableFormEngineUserControl)txtExplanationText.NestedControl;
            if (explanationTextControl != null)
            {
                explanationTextControl.Save();
            }

            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, ValidationHelper.GetString(txtFieldCaption.Value, String.Empty), txtFieldCaption.IsMacro);
            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, ValidationHelper.GetString(txtDescription.Value, String.Empty), txtDescription.IsMacro);
            FieldInfo.SetPropertyValue(FormFieldPropertyEnum.ExplanationText, ValidationHelper.GetString(txtExplanationText.Value, String.Empty), txtExplanationText.IsMacro);

            if (ShowInheritanceSettings)
            {
                FieldInfo.Inheritable = ControlInheritable;
            }

            return true;
        }

        return false;
    }

    #endregion
}