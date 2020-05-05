using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_System_SelectColor : FormEngineUserControl
{
    #region "Constants"

    private const string WHITE_HEX = "#FFFFFF";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value for control.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(clrPicker.SelectedColor, DefaultValue);
        }
        set
        {
            clrPicker.SelectedColor = ValidationHelper.GetString(value, DefaultValue);
        }
    }


    /// <summary>
    /// Gets the default value depends on Allow empty setting for field using this control.
    /// </summary>
    private string DefaultValue
    {
        get
        {
            return ((FieldInfo == null) || FieldInfo.AllowEmpty) ? String.Empty : DataHelper.GetNotEmpty(FieldInfo.DefaultValue, WHITE_HEX);
        }
    }

    #endregion
}