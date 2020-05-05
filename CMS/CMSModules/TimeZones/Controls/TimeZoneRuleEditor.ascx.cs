using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_TimeZones_Controls_TimeZoneRuleEditor : CMSUserControl
{
    private string mRule;
    private bool mEnabled = true;


    /// <summary>
    /// Gets or sets the value that indicates whether control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Returns time zone rule.
    /// </summary>
    public string Rule
    {
        get
        {
            if (mRule == null)
            {
                GetRule();
            }

            return mRule;
        }
        set
        {
            mRule = value;
        }
    }


    /// <summary>
    /// Gets or sets the title of the control.
    /// </summary>
    public string TitleText
    {
        get;
        set;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        headText.Text = TitleText;

        if (!RequestHelper.IsPostBack())
        {
            // Initialize dropdowns values
            InitDropDowns();
            // Set rule
            SetRule(mRule);
        }

        // Switch between day and value dropdown
        CheckCondition();

        drpMonth.Enabled = Enabled;
        drpCondition.Enabled = Enabled;
        if (!Enabled)
        {
            drpDay.Enabled = Enabled;
            drpDayValue.Enabled = Enabled;
        }
        txtAtHour.Enabled = Enabled;
        txtAtMinute.Enabled = Enabled;
        txtValue.Enabled = Enabled;
    }


    protected void drpCondition_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Switch between day and value dropdown
        CheckCondition();
    }


    /// <summary>
    /// Initialize dropdowns.
    /// </summary>
    private void InitDropDowns()
    {
        // Days array
        string[,] days = new string[7, 2];
        days[0, 0] = GetString("general.monday");
        days[0, 1] = "MON";
        days[1, 0] = GetString("general.tuesday");
        days[1, 1] = "TUE";
        days[2, 0] = GetString("general.wednesday");
        days[2, 1] = "WED";
        days[3, 0] = GetString("general.thursday");
        days[3, 1] = "THU";
        days[4, 0] = GetString("general.friday");
        days[4, 1] = "FRI";
        days[5, 0] = GetString("general.saturday");
        days[5, 1] = "SAT";
        days[6, 0] = GetString("general.sunday");
        days[6, 1] = "SUN";

        // Months array
        string[,] months = new string[12, 2];
        months[0, 0] = GetString("general.january");
        months[0, 1] = "JAN";
        months[1, 0] = GetString("general.february");
        months[1, 1] = "FEB";
        months[2, 0] = GetString("general.march");
        months[2, 1] = "MAR";
        months[3, 0] = GetString("general.april");
        months[3, 1] = "APR";
        months[4, 0] = GetString("general.may");
        months[4, 1] = "MAY";
        months[5, 0] = GetString("general.june");
        months[5, 1] = "JUN";
        months[6, 0] = GetString("general.july");
        months[6, 1] = "JUL";
        months[7, 0] = GetString("general.august");
        months[7, 1] = "AUG";
        months[8, 0] = GetString("general.september");
        months[8, 1] = "SEP";
        months[9, 0] = GetString("general.october");
        months[9, 1] = "OCT";
        months[10, 0] = GetString("general.november");
        months[10, 1] = "NOV";
        months[11, 0] = GetString("general.december");
        months[11, 1] = "DEC";

        // Fill month, day and value dropdowns
        for (int i = 0; i <= days.GetUpperBound(0); i++)
        {
            drpDay.Items.Add(new ListItem(days[i, 0], days[i, 1]));
        }
        for (int i = 0; i <= months.GetUpperBound(0); i++)
        {
            drpMonth.Items.Add(new ListItem(months[i, 0], months[i, 1]));
        }
        for (int i = 1; i <= 31; i++)
        {
            drpDayValue.Items.Add(new ListItem(i.ToString(), i.ToString()));
        }

        // Localize first/last condition options
        drpCondition.Items[0].Text = GetString("settings.first").ToUpperCSafe();
        drpCondition.Items[1].Text = GetString("settings.last").ToUpperCSafe();
    }


    /// <summary>
    /// Returns true if all values are correct.
    /// </summary>
    public bool IsValid()
    {
        // Check if hour is between 0 and 23
        int hour = ValidationHelper.GetInteger(txtAtHour.Text, -1);
        if ((hour < 0) || (hour >= 24))
        {
            return false;
        }

        // Check if minute is between 0 and 59
        int minute = ValidationHelper.GetInteger(txtAtMinute.Text, -1);
        if ((minute < 0) || (minute >= 60))
        {
            return false;
        }

        // Check if value is between -12 and 13
        double value = ValidationHelper.GetDouble(txtValue.Text, 24.0);
        if ((value < -12.0) || (value > 13.0))
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Method to change visibility of dropdown lists.
    /// </summary>
    protected void CheckCondition()
    {
        string condition = Server.HtmlDecode(drpCondition.SelectedValue);
        switch (condition)
        {
            case "FIRST":
            case "LAST":
                drpDay.Enabled = true;
                drpDayValue.Enabled = false;
                break;

            case ">=":
            case "<=":
                drpDay.Enabled = true;
                drpDayValue.Enabled = true;
                break;

            case "=":
                drpDay.Enabled = false;
                drpDayValue.Enabled = true;
                break;
        }
    }


    /// <summary>
    /// Sets rule editor values depend on selected rule.
    /// </summary>
    private void SetRule(string value)
    {
        if (value != null)
        {
            string[] val = value.Split('|');
            if (val.Length == 7)
            {
                // Clear selected items
                drpMonth.SelectedIndex = -1;
                drpDay.SelectedIndex = -1;
                drpDayValue.SelectedIndex = -1;
                drpCondition.SelectedIndex = -1;

                // Select items by value
                drpMonth.Items.FindByValue(val[0]).Selected = true;
                drpDay.Items.FindByValue(val[1]).Selected = true;
                drpDayValue.Items.FindByValue(val[2]).Selected = true;
                drpCondition.Items.FindByValue(val[3]).Selected = true;

                // Fill text boxes
                txtAtHour.Text = val[4];
                // If minutes is from 0 to 9 add 0 before
                txtAtMinute.Text = (val[5].Length > 1) ? val[5] : "0" + val[5];
                txtValue.Text = val[6];
            }
        }
    }


    /// <summary>
    /// Returns rule.
    /// </summary>
    private void GetRule()
    {
        string[] values = new string[7];

        // Create rule string
        values[0] = ValidationHelper.GetString(drpMonth.SelectedValue, "");
        values[1] = ValidationHelper.GetString(drpDay.SelectedValue, "");
        values[2] = ValidationHelper.GetInteger(drpDayValue.SelectedValue, 0).ToString();
        values[3] = ValidationHelper.GetString(Server.HtmlDecode(drpCondition.SelectedValue), "");
        values[4] = ValidationHelper.GetInteger(txtAtHour.Text, 0).ToString();
        values[5] = ValidationHelper.GetInteger(txtAtMinute.Text, 0).ToString();
        values[6] = ValidationHelper.GetInteger(txtValue.Text, 0).ToString();

        mRule = String.Join("|", values);
    }
}