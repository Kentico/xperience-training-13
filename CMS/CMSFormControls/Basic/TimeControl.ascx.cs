using System;
using System.Globalization;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;


public partial class CMSFormControls_Basic_TimeControl : FormEngineUserControl
{
    #region "Variables"

    private int maxHour = 24;
    private DateTimeFormatInfo mDateTimeFormat = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
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
            txtHour.Enabled = value;
            txtMinute.Enabled = value;
            drpAmPm.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (IsValidEmpty())
            {
                return null;
            }

            return Time;
        }
        set
        {
            if ((value != null) || (FieldInfo == null) || (!FieldInfo.AllowEmpty))
            {
                Time = ValidationHelper.GetDateTime(value, DateTime.Now.Date);
            }
        }
    }


    /// <summary>
    /// Gets or sets type safe form control value.
    /// </summary>
    public DateTime Time
    {
        get
        {
            return GetValue();
        }
        set
        {
            EnsureChildControls();

            Hour = value.Hour;

            // Fill leading zero for minutes
            txtMinute.Text = value.Minute.ToString("00");
        }
    }


    private int Hour
    {
        get
        {
            int hour = ValidationHelper.GetInteger(txtHour.Text, 0);
            if (drpAmPm.Visible)
            {
                // 11 PM is 23:00
                if ((drpAmPm.SelectedValue == DateTimeFormat.PMDesignator) && (hour < 12))
                {
                    hour += 12;
                }

                // 12 AM is 00:00
                if ((drpAmPm.SelectedValue == DateTimeFormat.AMDesignator) && (hour == 12))
                {
                    hour = 0;
                }
            }
            return hour;
        }
        set
        {
            
            if (drpAmPm.Visible)
            {
                // 12:00 is 12 PM
                if (value >= 12)
                {                    
                    drpAmPm.SelectedValue = DateTimeFormat.PMDesignator;
                }

                // 13:00 is 1 PM
                if (value > 12)
                {
                    value -= 12;
                }

                // 00:00 is 12 AM
                if ((value == 0) && (drpAmPm.SelectedValue == DateTimeFormat.AMDesignator))
                {
                    value = 12;
                }

            }
            txtHour.Text = value.ToString();
        }
    }


    /// <summary>
    /// Gets user's preferred culture or preferred UI culture.
    /// </summary>
    private DateTimeFormatInfo DateTimeFormat
    {
        get
        {
            if (mDateTimeFormat == null)
            {
                var ci = CultureHelper.GetCultureInfo(IsLiveSite ? LocalizationContext.PreferredCultureCode : CurrentUser.PreferredUICultureCode);
                mDateTimeFormat = ci.DateTimeFormat;
            }
            return mDateTimeFormat;
        }
    }

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        InitializeDropDownList();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureChildControls();
        CheckFieldEmptiness = false;
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Empty value scenario
        if (IsValidEmpty())
        {
            return true;
        }

        string wrongFormat = GetString("timecontrol.wrongformat");

        // Check if both values are integers greater or equal to 0
        ErrorMessage = new Validator().IsPositiveNumber(txtHour.Text, wrongFormat).IsPositiveNumber(String.IsNullOrEmpty(txtMinute.Text.Trim()) ? "0" : txtMinute.Text, wrongFormat).Result;
        if (String.IsNullOrEmpty(ErrorMessage))
        {
            int hour = ValidationHelper.GetInteger(txtHour.Text, 0);
            int minute = ValidationHelper.GetInteger(txtMinute.Text, 0);

            // 12:00 is valid 12hour format time while 24:00 is not valid 24hour format time. Also 0 hours is valid only for 24hour time format
            if ((minute < 60) && (((hour == maxHour) && (maxHour < 24)) || (hour < maxHour)) && ((maxHour == 24) || (hour > 0)))
            {
                txtMinute.Text = minute.ToString("00");

                return true;
            }

            // Else date was in wrong format
            ErrorMessage = wrongFormat;
        }
        return false;
    }

    #endregion


    #region "Private methods"

    private DateTime GetValue()
    {
        DateTime result = DateTime.MinValue.Date;
        result = result.AddHours(Hour);
        result = result.AddMinutes(ValidationHelper.GetInteger(txtMinute.Text, 0));
        return result;
    }


    /// <summary>
    /// Returns true if value is empty and it is allowed to be empty.
    /// </summary>
    private bool IsValidEmpty()
    {
        return (FieldInfo != null) && FieldInfo.AllowEmpty && String.IsNullOrEmpty(txtHour.Text.Trim()) && String.IsNullOrEmpty(txtMinute.Text.Trim());
    }


    private void InitializeDropDownList()
    {
        // 'H' means 24 hour format, 'h' 12 hour format.
        if (DateTimeFormat.ShortTimePattern.Contains("H"))
        {
            // Hide dropdown list for cultures with 24 hour time format
            drpAmPm.Visible = false;
            maxHour = 24;
        }
        else
        {
            // Show dropdown list for cultures with 12 hour time format
            drpAmPm.Visible = true;
            maxHour = 12;

            // Ensure dropdown list items
            if (drpAmPm.Items.Count != 2)
            {
                drpAmPm.Items.Clear();
                drpAmPm.Items.Add(new ListItem(DateTimeFormat.AMDesignator));
                drpAmPm.Items.Add(new ListItem(DateTimeFormat.PMDesignator));
            }
        }
    }

    #endregion
}