using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


/// <summary>
/// This form control must be used with name 'showurl'. Another blank form control must be registered with name 'showadvancedurl'.
/// </summary>
public partial class CMSFormControls_Dialogs_LinkDialogSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return radUrlSimple.Enabled;
        }
        set
        {
            radUrlNo.Enabled = value;
            radUrlSimple.Enabled = value;
            radUrlAdvanced.Enabled = value;
        }
    }


    /// <summary>
    /// Radiobutton 'simple' selected value.
    /// </summary>
    public override object Value
    {
        get
        {
            return radUrlSimple.Checked;
        }
        set
        {
            radUrlSimple.Checked = ValidationHelper.GetBoolean(value, false);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadOtherValues();
        radUrlNo.Checked = !(radUrlAdvanced.Checked || radUrlSimple.Checked);
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if (ContainsColumn("showadvancedurl"))
        {
            radUrlAdvanced.Checked = ValidationHelper.GetBoolean(GetColumnValue("ShowAdvancedUrl"), false);
        }
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        // Set properties names
        object[,] values = new object[1,2];

        values[0, 0] = "showadvancedurl";
        values[0, 1] = radUrlAdvanced.Checked;

        return values;
    }


    /// <summary>
    /// Validates control.
    /// </summary>
    public override bool IsValid()
    {
        bool isValid = true;

        if (!ContainsColumn("showurl"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "showurl", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }

        if (!ContainsColumn("showadvancedurl"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "showadvancedurl", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }

        return isValid;
    }

    #endregion
}