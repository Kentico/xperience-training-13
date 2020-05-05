using System;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSFormControls_Inputs_TextBoxDefaultSetting : FormEngineUserControl
{
    #region "Public properties"

    public enum ValueSourceType
    {
        Settings,
        ResourceString,
        None
    }


    /// <summary>
    /// Gets or sets encoded textbox value.
    /// </summary>
    public override object Value
    {
        get
        {
            return Trim ? txtValue.Text.Trim() : txtValue.Text;
        }
        set
        {
            txtValue.Text = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Gets or sets the codename of record with default value. This value will be used as watermark.
    /// </summary>
    public string WatermarkValueKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WatermarkValueKey"), null);
        }
        set
        {
            SetValue("WatermarkValueKey", value);
        }
    }


    /// <summary>
    /// Gets or sets the type of default watermark value (setting, resource string).
    /// </summary>
    public ValueSourceType WatermarkValueSourceType
    {
        get
        {
            int val = ValidationHelper.GetInteger(GetValue("WatermarkValueSourceType"), -1);
            switch (val)
            {
                // Get value from settings
                case 0:
                    return ValueSourceType.Settings;

                // Get value from resource string
                case 1:
                    return ValueSourceType.ResourceString;
                
                default:
                    return ValueSourceType.None;
            }
        }
        set
        {
            SetValue("WatermarkValueSourceType", value);
        }
    }


    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckMinMaxLength = true;
        CheckRegularExpression = true;

        string watermark = null;
        
        // Get default value
        if (!String.IsNullOrEmpty(WatermarkValueKey))
        {
            switch (WatermarkValueSourceType)
            {
                // Get value from settings
                case ValueSourceType.Settings:
                    watermark = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + "." + WatermarkValueKey);
                    break;

                // Get value from resource strings 
                case ValueSourceType.ResourceString:
                    watermark = ResHelper.GetString(WatermarkValueKey);
                    break;
            }
        }

        // Set default value as watermark
        if (!String.IsNullOrEmpty(watermark))
        {
            txtValue.WatermarkText = watermark;
        }
    }

    #endregion
}