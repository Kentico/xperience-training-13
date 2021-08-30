using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.MacroEngine;


public partial class CMSFormControls_Basic_CalendarControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return timePicker.Enabled;
        }
        set
        {
            timePicker.Enabled = value;
        }
    }


    /// <summary>
    /// If true, macros are allowed 
    /// </summary>
    public bool AllowMacros
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowMacros"), false);
        }
        set
        {
            SetValue("AllowMacros", value);
        }
    }


    /// <summary>
    /// If true, the range of the entered date/time is validated against possible minimum / maximum value
    /// </summary>
    public bool CheckRange
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckRange"), true);
        }
        set
        {
            SetValue("CheckRange", value);
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            String value = timePicker.DateTimeTextBox.Text;
            if (AllowMacros && (MacroProcessor.ContainsMacro(value) || DateTimeHelper.IsNowOrToday(value)))
            {
                return value;
            }

            var selectedDateTime = timePicker.SelectedDateTime;
            if (selectedDateTime == DateTimeHelper.ZERO_TIME)
            {
                return null;
            }

            return selectedDateTime;
        }
        set
        {
            if (GetValue("timezonetype") != null)
            {
                timePicker.TimeZone = EnumStringRepresentationExtensions.ToEnum<TimeZoneTypeEnum>(GetValue("timezonetype", String.Empty));
            }
            if (GetValue("timezone") != null)
            {
                timePicker.CustomTimeZone = CMS.Globalization.TimeZoneInfo.Provider.Get(GetValue("timezone", String.Empty));
            }

            string strValue = ValidationHelper.GetString(value, string.Empty);

            var isNowOrToday = DateTimeHelper.IsNowOrToday(strValue);

            if (AllowMacros && (MacroProcessor.ContainsMacro(strValue) || isNowOrToday))
            {
                timePicker.DateTimeTextBox.Text = strValue;
                return;
            }

            if (isNowOrToday)
            {
                timePicker.SelectedDateTime = DateTime.Now;
            }
            else
            {
                timePicker.SelectedDateTime = ValidationHelper.GetDateTime(value, DateTimeHelper.ZERO_TIME, CultureHelper.PreferredUICultureInfo);
            }
        }
    }


    /// <summary>
    /// Gets or sets if calendar control enables to edit time.
    /// </summary>
    public bool EditTime
    {
        get
        {
            return timePicker.EditTime;
        }
        set
        {
            timePicker.EditTime = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup control
        if (FieldInfo != null)
        {
            timePicker.AllowEmptyValue = FieldInfo.AllowEmpty;
        }
        timePicker.DisplayNow = ValidationHelper.GetBoolean(GetValue("displaynow"), true);
        timePicker.EditTime = ValidationHelper.GetBoolean(GetValue("edittime"), EditTime);
        timePicker.DateTimeTextBox.AddCssClass("EditingFormCalendarTextBox");
        timePicker.IsLiveSite = IsLiveSite;
        if (HasDependingFields)
        {
            timePicker.DateTimeTextBox.AutoPostBack = true;
            timePicker.DateTimeTextBox.TextChanged += (o, args) => RaiseOnChanged();
        }

        if (!String.IsNullOrEmpty(CssClass))
        {
            timePicker.AddCssClass(CssClass);
            CssClass = null;
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            timePicker.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }

        CheckFieldEmptiness = false;
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check value
        string strValue = timePicker.DateTimeTextBox.Text.Trim();
        bool required = (FieldInfo != null) && !FieldInfo.AllowEmpty;
        bool checkEmptiness = (Form == null) || Form.CheckFieldEmptiness;

        if (required && String.IsNullOrEmpty(strValue) && checkEmptiness)
        {
            // Empty error
            if ((ErrorMessage != null) && !ErrorMessage.EqualsCSafe(ResHelper.GetString("BasicForm.InvalidInput"), true))
            {
                ValidationError = ErrorMessage;
            }
            else
            {
                ValidationError += ResHelper.GetString("BasicForm.ErrorEmptyValue");
            }
            return false;
        }

        if (AllowMacros && (MacroProcessor.ContainsMacro(strValue) || DateTimeHelper.IsNowOrToday(strValue)))
        {
            return true;
        }

        if (((required && checkEmptiness) || !String.IsNullOrEmpty(strValue)) && (ValidationHelper.GetDateTime(strValue, DateTimeHelper.ZERO_TIME) == DateTimeHelper.ZERO_TIME))
        {
            if (timePicker.EditTime)
            {
                // Error invalid DateTime
                ValidationError += String.Format("{0} {1}.", ResHelper.GetString("BasicForm.ErrorInvalidDateTime"), DateTime.Now);
            }
            else
            {
                // Error invalid date
                ValidationError += String.Format("{0} {1}.", ResHelper.GetString("BasicForm.ErrorInvalidDate"), DateTime.Today.ToString("d"));
            }

            return false;
        }

        if (CheckRange && !timePicker.IsValidRange())
        {
            ValidationError += GetString("general.errorinvaliddatetimerange");
            return false;
        }

        return true;
    }


    /// <summary>
    /// Sets the property value of the control.
    /// </summary>
    /// <param name="propertyName">Name of the property</param>
    /// <param name="value">Value of the property</param>
    public override bool SetValue(string propertyName, object value)
    {
        switch (propertyName.ToLowerCSafe())
        {
            case "edittime":
                EditTime = ValidationHelper.GetBoolean(value, EditTime);
                break;
        }

        return base.SetValue(propertyName, value);
    }

    #endregion
}