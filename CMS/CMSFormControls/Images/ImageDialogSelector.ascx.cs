using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


/// <summary>
/// This form control must be used with name 'showimage'. Another blank form control with name 'showadvancedimage' must be used as well.
/// </summary>
public partial class CMSFormControls_Images_ImageDialogSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return radImageSimple.Enabled;
        }
        set
        {
            radImageNo.Enabled = value;
            radImageSimple.Enabled = value;
            radImageAdvanced.Enabled = value;
        }
    }


    /// <summary>
    /// Radiobutton 'simple' selected value.
    /// </summary>
    public override object Value
    {
        get
        {
            return radImageSimple.Checked;
        }
        set
        {
            radImageSimple.Checked = ValidationHelper.GetBoolean(value, false);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadOtherValues();

        radImageNo.Checked = !(radImageAdvanced.Checked || radImageSimple.Checked);
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if (ContainsColumn("ShowAdvancedImage"))
        {
            radImageAdvanced.Checked = ValidationHelper.GetBoolean(Form.Data.GetValue("ShowAdvancedImage"), false);
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

        values[0, 0] = "showadvancedimage";
        values[0, 1] = radImageAdvanced.Checked;

        return values;
    }


    /// <summary>
    /// Validates control.
    /// </summary>
    public override bool IsValid()
    {
        bool isValid = true;

        if (!ContainsColumn("showimage"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "showimage", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }

        if (!ContainsColumn("showadvancedimage"))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), "showadvancedimage", GetString("templatedesigner.fieldtypes.boolean"));
            isValid = false;
        }

        return isValid;
    }

    #endregion
}