using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_WebAnalytics_Controls_SelectGraphPeriod : CMSAdminControl
{
    /// <summary>
    /// Start of itnerval.
    /// </summary>
    public DateTime From
    {
        get
        {
            return ucRangeDatePicker.SelectedDateTime;
        }
        set
        {
            ucRangeDatePicker.SelectedDateTime = value;
        }
    }


    /// <summary>
    /// Full date time representation (from) (return precise datetime, not start date of interval (end of year,month,day...).
    /// </summary>
    public DateTime FullFrom
    {
        get
        {
            return ucRangeDatePicker.SelectedFullDateTime;
        }
        set
        {
            ucRangeDatePicker.SelectedFullDateTime = value;
        }
    }


    /// <summary>
    /// Full date time representation (to) - (return precise datetime, not end date of interval (end of year,month,day...).
    /// </summary>
    public DateTime FullTo
    {
        get
        {
            return ucRangeDatePicker.AlternateSelectedFullDateTime;
        }
        set
        {
            ucRangeDatePicker.AlternateSelectedFullDateTime = value;
        }
    }


    /// <summary>
    /// End of interval.
    /// </summary>
    public DateTime To
    {
        get
        {
            return ucRangeDatePicker.AlternateSelectedDateTime;
        }
        set
        {
            ucRangeDatePicker.AlternateSelectedDateTime = value;
        }
    }


    /// <summary>
    /// Graph type interval.
    /// </summary>
    public HitsIntervalEnum Interval
    {
        get
        {
            return HitsIntervalEnumFunctions.StringToHitsConversion(ValidationHelper.GetString(ViewState["GraphTypePeriod"], "Month"));
        }
        set
        {
            ViewState["GraphTypePeriod"] = value;
            SetDateTimePickerSettings();
        }
    }


    /// <summary>
    /// Enables/disables range date time picker
    /// </summary>
    public bool Enabled
    {
        get
        {
            return ucRangeDatePicker.Enabled;
        }
        set
        {
            ucRangeDatePicker.Enabled = value;
            btnUpdate.Enabled = value;
        }
    }
    

    protected void Page_Load(object sender, EventArgs e)
    {
        SetDateTimePickerSettings();
        pnlRange.DefaultButton = btnUpdate.ID;
    }


    /// <summary>
    /// Sets settings for datetime picker depending on interval.
    /// </summary>
    private void SetDateTimePickerSettings()
    {
        ucRangeDatePicker.DisableDaySelect = false;
        ucRangeDatePicker.DisableMonthSelect = false;
        ucRangeDatePicker.EditTime = false;

        switch (Interval)
        {
            case HitsIntervalEnum.Month:
                ucRangeDatePicker.DisableDaySelect = true;
                break;

            case HitsIntervalEnum.Year:
                ucRangeDatePicker.DisableMonthSelect = true;
                break;

            case HitsIntervalEnum.Hour:
                ucRangeDatePicker.EditTime = true;
                break;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
    }
}