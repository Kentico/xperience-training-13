using System;
using System.Drawing;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Scheduler;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Selectors_TimeoutSelector : BaseSchedulingControl
{
    #region "Constants

    private const int QUANTITYMAXIMUM = 10000;

    #endregion


    #region "Variables"

    private string mDefaultPeriod = SchedulingHelper.PERIOD_MINUTE;

    private bool mAllowNoTimeout = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if control is used on a live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            timePicker.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates whether no timeout can be selected.
    /// </summary>
    public bool AllowNoTimeout
    {
        get
        {
            return mAllowNoTimeout;
        }
        set
        {
            if (!value)
            {
                // Ensure "none" is not selected
                if (rbNoneMode.Checked)
                {
                    rbNoneMode.Checked = false;
                    rbIntervalMode.Checked = true;
                }
                rbNoneMode.Visible = false;
            }
            mAllowNoTimeout = value;
        }
    }


    /// <summary>
    /// Indicates whether timeout is enabled.
    /// </summary>
    public bool TimeoutEnabled
    {
        get
        {
            if (!AllowNoTimeout)
            {
                return true;
            }

            return !rbNoneMode.Checked;
        }
        set
        {
            if (AllowNoTimeout)
            {
                rbNoneMode.Checked = !value;
            }
        }
    }


    /// <summary>
    /// Timeout selection mode
    /// </summary>
    public override string Mode
    {
        get
        {
            if (rbDateMode.Checked)
            {
                return SchedulingHelper.PERIOD_ONCE;
            }
            else
            {
                return drpScale.SelectedValue;
            }
        }
    }


    /// <summary>
    /// Time unit used
    /// </summary>
    public override CMSDropDownList Period
    {
        get
        {
            return drpScale;
        }
    }


    /// <summary>
    /// Start date & time
    /// </summary>
    public override DateTimePicker StartTime
    {
        get
        {
            return dateTimePicker;
        }
    }


    /// <summary>
    /// Preferred next run date
    /// </summary>
    public override CMSDropDownList MonthDate
    {
        get
        {
            return drpNextDate;
        }
    }


    /// <summary>
    /// Preferred next run order
    /// </summary>
    public override CMSDropDownList MonthOrder
    {
        get
        {
            return drpNextOrder;
        }
    }


    /// <summary>
    /// Preferred next run day of week
    /// </summary>
    public override CMSDropDownList MonthDay
    {
        get
        {
            return drpNextDay;
        }
    }


    /// <summary>
    /// Default period. Allowed values: second, timesecond, minute, hour, day, week, month, once.
    /// </summary>
    public override string DefaultPeriod
    {
        get
        {
            return mDefaultPeriod;
        }
        set
        {
            switch (value.ToLowerCSafe())
            {
                case SchedulingHelper.PERIOD_DAY:
                    mDefaultPeriod = SchedulingHelper.PERIOD_DAY;
                    break;

                case SchedulingHelper.PERIOD_HOUR:
                    mDefaultPeriod = SchedulingHelper.PERIOD_HOUR;
                    break;

                case SchedulingHelper.PERIOD_MINUTE:
                    mDefaultPeriod = SchedulingHelper.PERIOD_MINUTE;
                    break;

                case SchedulingHelper.PERIOD_MONTH:
                    mDefaultPeriod = SchedulingHelper.PERIOD_MONTH;
                    break;

                case SchedulingHelper.PERIOD_YEAR:
                    mDefaultPeriod = SchedulingHelper.PERIOD_YEAR;
                    break;

                case SchedulingHelper.PERIOD_WEEK:
                    mDefaultPeriod = SchedulingHelper.PERIOD_WEEK;
                    break;

                default:
                    mDefaultPeriod = SchedulingHelper.PERIOD_MINUTE;
                    break;
            }
        }
    }


    /// <summary>
    /// For weeks, months and years quantity can be 0 if 'exactly' is not checked. Else quantity minimum is 1.
    /// </summary>
    private int QuantityLowerBound
    {
        get
        {
            string scale = drpScale.SelectedValue;
            if(cbSpecificTime.Checked && (scale == SchedulingHelper.PERIOD_DAY)){
                return 0;
            }
            if (!rbExactly.Checked && ((scale == SchedulingHelper.PERIOD_WEEK) || (scale == SchedulingHelper.PERIOD_MONTH) || (scale == SchedulingHelper.PERIOD_YEAR)))
            {
                return 0;
            }
            return 1;
        }
    }

    #endregion


    #region "Event handlers"

    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureChildControls();
        CMSPage page = Page as CMSPage;
        if (page != null)
        {
            page.EnsureScriptManager();
        }

        if (!RequestHelper.IsPostBack())
        {
            SetupForm(TaskInterval);
        }
    }


    protected void drpScale_SelectedIndexChanged(object sender, EventArgs e)
    {
        ScaleChanged();
    }


    protected void rbMode_CheckedChanged(object sender, EventArgs e)
    {
        if (rbIntervalMode.Checked)
        {
            ShowIntervalPanel();
        }
        else if (rbDateMode.Checked)
        {

            ShowDatePanel();
        }
        else
        {
            HideAllPanels();
        }
    }


    protected void cbSpecificTime_CheckedChanged(object sender, EventArgs e)
    {
        timePicker.Visible = cbSpecificTime.Checked;
    }


    protected void rbNext_CheckedChanged(object sender, EventArgs e)
    {
        DateModeChanged();
    }


    protected void drpNextDate_DaysCountChanged(object sender, EventArgs e){
        SetMonthDays();
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Creates schedule interval string.
    /// </summary>
    protected override string EncodeInterval()
    {
        if (!TimeoutEnabled)
        {
            return String.Empty;
        }

        TaskInterval ti = new TaskInterval();
        string result = string.Empty;

        try
        {
            ti.Period = Mode;

            if (Mode != SchedulingHelper.PERIOD_ONCE)
            {
                result = new Validator().NotEmpty(txtQuantity.Text, GetString("timeoutselector.quantity.errorempty")).IsInteger(txtQuantity.Text, String.Format(GetString("timeoutselector.quantity.wrongformat"), QUANTITYMAXIMUM)).Result;
                    if (String.IsNullOrEmpty(result))
                    {
                        int quantity = ValidationHelper.GetInteger(txtQuantity.Text, -1);
                        if ((quantity >= QuantityLowerBound) && (quantity <= QUANTITYMAXIMUM))
                        {
                            ti.Every = quantity;
                        }
                        else
                        {
                            txtQuantity.ForeColor = Color.Red;
                        }
                    }
            }
            if (String.IsNullOrEmpty(result))
            {
                switch (Mode)
                {
                    case SchedulingHelper.PERIOD_MINUTE:
                    case SchedulingHelper.PERIOD_HOUR:
                    case SchedulingHelper.PERIOD_DAY:
                        ti.BetweenStart = DateTime.MinValue;
                        ti.BetweenEnd = DateTime.MaxValue;

                        // Add all days to match the format
                        AddWeekDays(ti);
                        AddWeekEnd(ti);
                        break;

                    case SchedulingHelper.PERIOD_WEEK:
                        if (rbNextDay.Checked)
                        {
                            ti.Order = drpNextOrder.SelectedValue;
                            ti.Day = drpNextDay.SelectedValue;
                        }
                        break;

                    case SchedulingHelper.PERIOD_MONTH:
                        if (rbNextDate.Checked)
                        {
                            ti.Order = drpNextDate.SelectedValue;
                        }
                        else if (rbNextDay.Checked)
                        {
                            ti.Order = drpNextOrder.SelectedValue;
                            ti.Day = drpNextDay.SelectedValue;
                        }
                        break;

                    case SchedulingHelper.PERIOD_YEAR:
                        if (rbNextDate.Checked)
                        {
                            ti.Order = drpNextDateMonth.SelectedValue;
                            ti.Day = drpNextDate.SelectedValue;
                        }
                        break;

                    case SchedulingHelper.PERIOD_ONCE:
                        if (dateTimePicker.SelectedDateTime != DateTime.MinValue)
                        {
                            ti.StartTime = dateTimePicker.SelectedDateTime;                            
                        }
                        else
                        {
                            result = GetString("timeoutselector.errorinvaliddate");
                        }
                        break;
                }

                // Add specific time to start date
                if (String.IsNullOrEmpty(result) && cbSpecificTime.Visible && cbSpecificTime.Checked)
                {
                    //result = new Validator().NotEmpty(txtSpecificTimeHour.Text, GetString("timeoutselector.specifichour.errorempty")).IsInteger(txtSpecificTimeHour.Text, GetString("timeoutselector.specifichour.wrongformat")).NotEmpty(txtSpecificTimeMinute.Text, GetString("timeoutselector.specificminute.errorempty")).IsInteger(txtSpecificTimeMinute.Text, GetString("timeoutselector.specificminute.wrongformat")).Result;
                    if (timePicker.IsValid())
                    {
                        ti.StartTime = ti.StartTime.AddHours(timePicker.Time.Hour);
                        ti.StartTime = ti.StartTime.AddMinutes(timePicker.Time.Minute);

                        ti.UseSpecificTime = true;
                    }
                    else
                    {
                        result = timePicker.ErrorMessage;
                    }
                }
                else
                {
                    ti.UseSpecificTime = false;
                }
            }
        }
        catch(Exception ex)
        {
            ShowError(ex.Message);
            result = ex.Message;
            dateTimePicker.DateTimeTextBox.ForeColor = Color.Red;
        }

        if (!String.IsNullOrEmpty(result))
        { 
            AddError(result);
            return null;
        }

        WeekListChecked = true;

        return SchedulingHelper.EncodeInterval(ti);
    }

    #endregion


    #region "Protected methods"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        // Control init
        dateTimePicker.DateTimeTextBox.ForeColor = Color.Black;
        txtQuantity.ForeColor = Color.Black;

        if (!RequestHelper.IsPostBack())
        {
            ControlInit();
        }
    }


    protected override void OnPeriodChangeInit()
    {
        if (rbIntervalMode.Checked)
        {
            ShowIntervalPanel();
            ScaleChanged();
        }
        else
        {
            ShowDatePanel();
        }
        timePicker.Visible = cbSpecificTime.Checked;
    }


    protected override void ControlInit()
    {
        base.ControlInit();

        SetMonths();
    }


    /// <summary>
    /// Sets form with values from the schedule interval string.
    /// </summary>
    /// <param name="interval">Schedule interval string</param>
    protected void SetupForm(TaskInterval interval)
    {
        EnsureChildControls();
        if (TimeoutEnabled || !AllowNoTimeout)
        {
            interval = interval ?? new TaskInterval();

            // Set period type
            if (interval.Period == SchedulingHelper.PERIOD_ONCE)
            {
                rbDateMode.Checked = true;
            }
            else
            {
                rbIntervalMode.Checked = true;
                drpScale.SelectedValue = interval.Period;
                OnPeriodChangeInit();
            }

            if (interval.UseSpecificTime)
            {
                cbSpecificTime.Checked = true;
                timePicker.Visible = true;

                timePicker.Time = interval.StartTime;
            }

            rbExactly.Checked = false;
            rbNextDay.Checked = false;
            rbNextDate.Checked = false;

            switch (Mode)
            {
                case SchedulingHelper.PERIOD_MINUTE:
                case SchedulingHelper.PERIOD_HOUR:
                case SchedulingHelper.PERIOD_DAY:
                    txtQuantity.Text = interval.Every.ToString();
                    break;

                case SchedulingHelper.PERIOD_WEEK:
                    txtQuantity.Text = interval.Every.ToString();

                    if (String.IsNullOrEmpty(interval.Day))
                    {
                        rbExactly.Checked = true;
                    }
                    else
                    {
                        drpNextDay.SelectedValue = interval.Day;
                        rbNextDay.Checked = true;
                    }
                    DateModeChanged();
                    break;

                case SchedulingHelper.PERIOD_MONTH:
                    txtQuantity.Text = interval.Every.ToString();

                    if (String.IsNullOrEmpty(interval.Order))
                    {
                        rbExactly.Checked = true;
                    }
                    else if (ValidationHelper.GetInteger(interval.Order, 0) != 0)
                    {
                        rbNextDate.Checked = true;
                        drpNextDate.SelectedValue = interval.Order;
                        DateModeChanged();
                    }
                    else
                    {
                        rbNextDay.Checked = true;
                        drpNextOrder.SelectedValue = interval.Order;
                        DateModeChanged();
                        drpNextDay.SelectedValue = interval.Day;
                    }
                    
                    break;

                case SchedulingHelper.PERIOD_YEAR:
                    txtQuantity.Text = interval.Every.ToString();
                    if (!String.IsNullOrEmpty(interval.Day) && !String.IsNullOrEmpty(interval.Order))
                    {
                        rbNextDate.Checked = true;
                        drpNextDateMonth.SelectedValue = interval.Order;
                        DateModeChanged();
                        drpNextDate.SelectedValue = interval.Day;
                    }
                    else
                    {
                        rbExactly.Checked = true;
                        DateModeChanged();
                    }
                    
                    break;

                case SchedulingHelper.PERIOD_ONCE:
                    ShowDatePanel();
                    dateTimePicker.SelectedDateTime = interval.StartTime;
                    break;
            }
        }
        else
        {
            HideAllPanels();
        }
    }


    protected override void SetPeriods()
    {
        var listItems = new[] 
        {
            new ListItem(GetString("timeoutselector.minutes"), SchedulingHelper.PERIOD_MINUTE),
            new ListItem(GetString("timeoutselector.hours"), SchedulingHelper.PERIOD_HOUR),
            new ListItem(GetString("timeoutselector.days"), SchedulingHelper.PERIOD_DAY),
            new ListItem(GetString("timeoutselector.weeks"), SchedulingHelper.PERIOD_WEEK),
            new ListItem(GetString("timeoutselector.months"), SchedulingHelper.PERIOD_MONTH),
            new ListItem(GetString("timeoutselector.years"), SchedulingHelper.PERIOD_YEAR)
        };

        drpScale.Items.Clear();
        drpScale.Items.AddRange(listItems);
    }

    #endregion


    #region "Private methods"

    private void ScaleChanged()
    {
        switch (drpScale.SelectedValue)
        {
            case SchedulingHelper.PERIOD_MINUTE:
            case SchedulingHelper.PERIOD_HOUR:
                plcDateMode.Visible = false;
                plcSpecificTime.Visible = false;
                lblNextDateOfTheMonth.Visible = false;
                lblNextDayOfTheMonth.Visible = false;
                break;

            case SchedulingHelper.PERIOD_DAY:
                plcDateMode.Visible = false;
                plcSpecificTime.Visible = true;
                lblNextDateOfTheMonth.Visible = false;
                lblNextDayOfTheMonth.Visible = false;
                break;

            case SchedulingHelper.PERIOD_WEEK:
                plcDateMode.Visible = true;
                plcExactly.Visible = true;
                plcNextDate.Visible = false;
                plcNextDay.Visible = true;
                drpNextOrder.Visible = false;
                plcSpecificTime.Visible = true;
                lblNextDateOfTheMonth.Visible = false;
                lblNextDayOfTheMonth.Visible = false;

                // Date will disappear for week
                if (rbNextDate.Checked)
                {
                    rbExactly.Checked = true;
                    rbNextDate.Checked = false;
                }

                DateModeChanged();
                break;

            case SchedulingHelper.PERIOD_MONTH:
                plcDateMode.Visible = true;
                plcExactly.Visible = true;
                plcNextDate.Visible = true;
                rbNextDate.ResourceString = "timeoutselector.thenextday";
                pnlNextDateMonth.Visible = false;
                plcNextDay.Visible = true;
                drpNextOrder.Visible = true;
                plcSpecificTime.Visible = true;
                lblNextDateOfTheMonth.Visible = true;
                lblNextDayOfTheMonth.Visible = true;

                InitializeMonthDays(31, true);
                DateModeChanged();
                break;

            case SchedulingHelper.PERIOD_YEAR:
                plcDateMode.Visible = true;
                plcExactly.Visible = true;
                plcNextDate.Visible = true;
                rbNextDate.ResourceString = "timeoutselector.thenext";
                pnlNextDateMonth.Visible = true;
                plcNextDay.Visible = false;
                plcSpecificTime.Visible = true;
                lblNextDateOfTheMonth.Visible = false;
                lblNextDayOfTheMonth.Visible = false;

                // Day will disappear for year
                if (rbNextDay.Checked)
                {
                    rbNextDay.Checked = false;
                    rbExactly.Checked = true;
                }
                DateModeChanged();
                break;
        }

        drpNextDate.SelectedIndex = 0;
        drpNextOrder.SelectedIndex = 0;
        drpNextDay.SelectedIndex = 0;
    }


    private void ShowIntervalPanel()
    {
        plcDate.Visible = false;
        plcInterval.Visible = true;

        ScaleChanged();
    }


    private void ShowDatePanel()
    {
        plcDate.Visible = true;
        plcSpecificTime.Visible = true;
        plcInterval.Visible = false;
    }


    private void HideAllPanels()
    {
        plcDate.Visible = false;
        plcInterval.Visible = false;
        plcSpecificTime.Visible = false;
    }


    private void DateModeChanged()
    {
        if (rbNextDate.Checked)
        {
            rbExactly.Checked = false;
            drpNextDate.Enabled = true;
            drpNextDateMonth.Enabled = true;
            drpNextOrder.Enabled = false;
            drpNextDay.Enabled = false;
            SetMonthDays();
        }
        else if (rbNextDay.Checked)
        {
            rbExactly.Checked = false;
            drpNextDate.Enabled = false;
            drpNextDateMonth.Enabled = false;
            drpNextOrder.Enabled = true;
            drpNextDay.Enabled = true;
        }
        else
        {
            rbExactly.Checked = true;
            drpNextDate.Enabled = false;
            drpNextDateMonth.Enabled = false;
            drpNextOrder.Enabled = false;
            drpNextDay.Enabled = false;
        }
    }


    private void SetMonths()
    {
        ListItem[] li = new ListItem[12];
        li[0] = new ListItem(GetString("general.january"), "1");
        li[1] = new ListItem(GetString("general.february"), "2");
        li[2] = new ListItem(GetString("general.march"), "3");
        li[3] = new ListItem(GetString("general.april"), "4");
        li[4] = new ListItem(GetString("general.may"), "5");
        li[5] = new ListItem(GetString("general.june"), "6");
        li[6] = new ListItem(GetString("general.july"), "7");
        li[7] = new ListItem(GetString("general.august"), "8");
        li[8] = new ListItem(GetString("general.september"), "9");
        li[9] = new ListItem(GetString("general.october"), "10");
        li[10] = new ListItem(GetString("general.november"), "11");
        li[11] = new ListItem(GetString("general.december"), "12");
        drpNextDateMonth.Items.Clear();
        drpNextDateMonth.Items.AddRange(li);
    }


    private void SetMonthDays()
    {
        if (drpScale.SelectedValue == SchedulingHelper.PERIOD_YEAR)
        {
            // 2012 is a leap year
            int days = DateTime.DaysInMonth(2012, ValidationHelper.GetInteger(drpNextDateMonth.SelectedValue, 0));
            InitializeMonthDays(days, false);
        }
    }

    #endregion
}