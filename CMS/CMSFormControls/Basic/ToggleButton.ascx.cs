using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Basic_ToggleButton : FormEngineUserControl
{
    #region "Constants"

    private const string EVENT_SOURCE = "Toggle button";
    private const string EVENT_CODE = "UNSUPPORTEDVALUE";

    #endregion


    #region "Variables"

    private object mInnerValue;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return checkbox.Enabled;
        }
        set
        {
            checkbox.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if ((FieldInfo != null) && (FieldInfo.DataType == FieldDataType.Boolean))
            {
                return checkbox.Checked;
            }
            else
            {
                return checkbox.Checked ? CheckedValue : UncheckedValue;
            }
        }
        set
        {
            if ((FieldInfo != null) && (FieldInfo.DataType == FieldDataType.Boolean))
            {
                checkbox.Checked = ValidationHelper.GetBoolean(value, false);
            }
            else
            {
                mInnerValue = value;
            }
        }
    }

    #endregion


    #region "Toggle button properties"

    /// <summary>
    /// Indicates whether required properties have valid values.
    /// </summary>
    private bool ValidProperties
    {
        get
        {
            return !((ImageHeight == 0) || (ImageWidth == 0) || String.IsNullOrEmpty(CheckedImageUrl) || String.IsNullOrEmpty(UncheckedImageUrl));
        }
    }


    /// <summary>
    /// The height of the image.
    /// </summary>
    public int ImageHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ImageHeight"), 0);
        }
        set
        {
            SetValue("ImageHeight", value);
        }
    }


    /// <summary>
    /// The width of the image.
    /// </summary>
    public int ImageWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ImageWidth"), 0);
        }
        set
        {
            SetValue("ImageWidth", value);
        }
    }


    /// <summary>
    /// The URL of the image to show when the toggle button is in the checked state.
    /// </summary>
    public string CheckedImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CheckedImageUrl"), null);
        }
        set
        {
            SetValue("CheckedImageUrl", value);
        }
    }


    /// <summary>
    /// The URL of the image to show when the toggle button is in the unchecked state.
    /// </summary>
    public string UncheckedImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UncheckedImageUrl"), null);
        }
        set
        {
            SetValue("UncheckedImageUrl", value);
        }
    }


    /// <summary>
    /// The URL of the image to show when the toggle button is disabled and in the checked state.
    /// </summary>
    public string DisabledCheckedImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisabledCheckedImageUrl"), null);
        }
        set
        {
            SetValue("DisabledCheckedImageUrl", value);
        }
    }


    /// <summary>
    /// The URL of the image to show when the toggle button is disabled and in the unchecked state.
    /// </summary>
    public string DisabledUncheckedImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisabledUncheckedImageUrl"), null);
        }
        set
        {
            SetValue("DisabledUncheckedImageUrl", value);
        }
    }


    /// <summary>
    /// The URL of the image to show when the toggle button is in the checked state and the mouse is over the button.
    /// </summary>
    public string CheckedImageOverUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CheckedImageOverUrl"), null);
        }
        set
        {
            SetValue("CheckedImageOverUrl", value);
        }
    }


    /// <summary>
    /// The URL of the image to show when the toggle button is in the unchecked state and the mouse is over the button.
    /// </summary>
    public string UncheckedImageOverUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UncheckedImageOverUrl"), null);
        }
        set
        {
            SetValue("UncheckedImageOverUrl", value);
        }
    }


    /// <summary>
    /// The alt text to show when the toggle button is in the checked state.
    /// </summary>
    public string CheckedImageAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CheckedImageAlternateText"), null);
        }
        set
        {
            SetValue("CheckedImageAlternateText", value);
        }
    }


    /// <summary>
    /// The alt text to show when the toggle button is in the unchecked state.
    /// </summary>
    public string UncheckedImageAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UncheckedImageAlternateText"), null);
        }
        set
        {
            SetValue("UncheckedImageAlternateText", value);
        }
    }


    /// <summary>
    /// The alt text to show when the toggle button is in the checked state and the mouse is over the button.
    /// </summary>
    public string CheckedImageOverAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CheckedImageOverAlternateText"), null);
        }
        set
        {
            SetValue("CheckedImageOverAlternateText", value);
        }
    }


    /// <summary>
    /// The alt text to show when the toggle button is in the unchecked state and the mouse is over the button.
    /// </summary>
    public string UncheckedImageOverAlternateText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UncheckedImageOverAlternateText"), null);
        }
        set
        {
            SetValue("UncheckedImageOverAlternateText", value);
        }
    }


    /// <summary>
    /// Special value used for the checked state.
    /// </summary>
    public object CheckedValue
    {
        get;
        set;
    }


    /// <summary>
    /// Special value used for the unchecked state.
    /// </summary>
    public object UncheckedValue
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ControlsHelper.EnsureScriptManager(Page);

        // Initialize checkbox for special values
        if ((FieldInfo != null) && (FieldInfo.DataType != FieldDataType.Boolean))
        {
            if (CheckedValue == null)
            {
                CheckedValue = DataHelper.GetNotEmpty(GetValue("CheckedValue"), "");
            }
            if (UncheckedValue == null)
            {
                UncheckedValue = DataHelper.GetNotEmpty(GetValue("UncheckedValue"), "");
            }

            if (FieldInfo.DataType == FieldDataType.Decimal)
            {
                HandleDecimalValue();
                return;
            }

            if (FieldInfo.DataType == FieldDataType.Double)
            {
                HandleDoubleValue();
                return;
            }

            HandleStringValue();
        }
    }


    /// <summary>
    /// Compares inner decimal value with checked value and sets the check box accordingly.
    /// </summary>
    private void HandleDecimalValue()
    {
        try
        {
            // Number of decimal digits could vary depending on field precision.
            var innerValueDecimal = Convert.ToDecimal(mInnerValue);
            var checkedValueDecimal = Convert.ToDecimal(CheckedValue);

            checkbox.Checked = checkedValueDecimal == innerValueDecimal;
        }
        catch (Exception e)
        {
            LogException(e);
        }
    }


    /// <summary>
    /// Compares inner double value with checked value and sets the check box accordingly.
    /// </summary>
    private void HandleDoubleValue()
    {
        try
        {
            // Number of double digits could vary depending on field precision.
            var innerValueDouble = Convert.ToDouble(mInnerValue);
            var checkedValueDouble = Convert.ToDouble(CheckedValue);

            checkbox.Checked = (Math.Abs(checkedValueDouble - innerValueDouble) < Double.Epsilon);
        }
        catch (Exception e)
        {
            LogException(e);
        }
    }


    /// <summary>
    /// Compares inner string value with checked value and sets the check box accordingly.
    /// </summary>
    private void HandleStringValue()
    {
        var innerValueString = ValidationHelper.GetString(mInnerValue, null);
        checkbox.Checked = CMSString.Equals(CheckedValue.ToString(), innerValueString);
    }


    /// <summary>
    /// Logs given exception into event log.
    /// </summary>
    private static void LogException(Exception e)
    {
        Service.Resolve<IEventLogService>().LogException(EVENT_SOURCE, EVENT_CODE, e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize extender
        if (ValidProperties)
        {
            checkbox.Width = ImageWidth;
            checkbox.Height = ImageHeight;

            exToggle.ImageHeight = ImageHeight;
            exToggle.ImageWidth = ImageWidth;
            exToggle.CheckedImageUrl = CheckedImageUrl;
            exToggle.UncheckedImageUrl = UncheckedImageUrl;
            exToggle.DisabledCheckedImageUrl = DisabledCheckedImageUrl;
            exToggle.DisabledUncheckedImageUrl = DisabledUncheckedImageUrl;
            exToggle.CheckedImageOverUrl = CheckedImageOverUrl;
            exToggle.UncheckedImageOverUrl = UncheckedImageOverUrl;
            exToggle.CheckedImageAlternateText = CheckedImageAlternateText;
            exToggle.UncheckedImageAlternateText = UncheckedImageAlternateText;
            exToggle.CheckedImageOverAlternateText = CheckedImageOverAlternateText;
            exToggle.UncheckedImageOverAlternateText = UncheckedImageOverAlternateText;
        }
        else
        {
            exToggle.Enabled = false;
            lblError.Visible = true;
            lblError.Text = "Toggle button is missing required property value.";
        }

        // Apply CSS styles
        if (!String.IsNullOrEmpty(CssClass))
        {
            checkbox.CssClass = CssClass;
            CssClass = null;
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            checkbox.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;
    }

    #endregion
}