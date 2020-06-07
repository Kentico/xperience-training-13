using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;


public partial class CMSAdminControls_Calendar_Calendar : CMSPage
{
    #region "Variable"

    protected string pagetitle = null;
    protected string mCulture = "";

    protected bool editTime = false;
    protected string selDate = null;
    protected bool allowEmpty = false;
    protected string controlId = null;

    #endregion


    /// <summary>
    /// Constructor.
    /// </summary>
    public CMSAdminControls_Calendar_Calendar()
    {
        mCulture = Page.UICulture;

        // Get UI culture parameter from querystring
        string uiCulture = QueryHelper.GetText("UILang", "");
        if (uiCulture != "")
        {
            // If exists set this culture
            UICulture = uiCulture;
            mCulture = uiCulture;
        }
        else
        {
            if (DocumentContext.CurrentDocument != null)
            {
                // If this parameter doesn't exist, set document culture
                mCulture = DocumentContext.CurrentDocument.DocumentCulture;
            }
            else
            {
                // Or system UI culture
                mCulture = CultureInfo.CurrentUICulture.Name;
            }
        }

        // Set culture to current thread
        Thread.CurrentThread.CurrentCulture = CultureHelper.GetCultureInfo(mCulture);
        Thread.CurrentThread.CurrentUICulture = CultureHelper.GetCultureInfo(mCulture);
    }


    protected void Page_Load(Object sender, EventArgs e)
    {
        // Get parameters
        selDate = QueryHelper.GetString("selDate", "");
        allowEmpty = QueryHelper.GetBoolean("allowempty", false);
        editTime = QueryHelper.GetBoolean("editTime", false);
        controlId = QueryHelper.GetText("controlid", "");

        SetBrowserClass();

        ScriptHelper.RegisterTooltip(Page);

        // Disable caching
        Response.Cache.SetCacheability(HttpCacheability.NoCache);

        pagetitle = GetString("Calendar.Title");

        // Fill in the dropdown values
        if (drpHours.Items.Count == 0)
        {
            FillNumbers(drpHours, 0, 23);
        }
        if (drpMinutes.Items.Count == 0)
        {
            FillNumbers(drpMinutes, 0, 59);
        }
        if (drpSeconds.Items.Count == 0)
        {
            FillNumbers(drpSeconds, 0, 59);
        }

        // Fill in the month selector
        if (drpMonth.Items.Count == 0)
        {
            FillMonths(drpMonth, mCulture);
        }

        // Setup the buttons
        btnCancel.Text = GetString("general.cancel");
        btnCancel.Attributes.Add("onclick", "return CloseDialog();");

        if (editTime)
        {
            btnNow.Text = GetString("Calendar.Now");
        }
        else
        {
            btnNow.Text = GetString("Calendar.Today");
        }

        btnOk.Text = GetString("general.ok");
        btnNA.Text = GetString("general.na");
        btnNA.Attributes.Add("onclick", "CloseWindow(" + ScriptHelper.GetString(controlId) + ", ''); return false;");

        if (!RequestHelper.IsPostBack())
        {
            DateTime now = TimeZoneHelper.GetUserDateTime(MembershipContext.AuthenticatedUser);

            // Selected date
            if (selDate == "")
            {
                SetTime(now, true);
            }
            else
            {
                SetTime(ValidationHelper.GetDateTime(selDate, now), true);
            }

            // Allow empty ?
            if (!allowEmpty)
            {
                btnNA.Visible = false;
            }
            else
            {
                btnNA.Visible = true;
            }

            // Edit time ?
            if (editTime)
            {
                pnlTime.Visible = true;
            }
            else
            {
                pnlTime.Visible = false;
            }
        }

        // Display time zone in form 'GMT+1:00'
        int timeZoneId = QueryHelper.GetInteger("timezoneid", 0);
        if (timeZoneId > 0)
        {
            TimeZoneInfo tzi = TimeZoneInfo.Provider.Get(timeZoneId);
            if (tzi != null)
            {
                lblGMTShift.Visible = true;
                lblGMTShift.Text = TimeZoneHelper.GetUTCStringOffset(tzi);
                ScriptHelper.AppendTooltip(lblGMTShift, TimeZoneHelper.GetUTCLongStringOffset(tzi), "help");
            }
        }

        // Next and previous month images
        calDate.NextMonthText = "<img src=\"" + GetImageUrl("Design/Controls/Calendar/next.png") + "\" />";
        calDate.PrevMonthText = "<img src=\"" + GetImageUrl("Design/Controls/Calendar/previous.png") + "\" />";

        RegisterModalPageScripts();
        RegisterEscScript();
    }


    /// <summary>
    /// Fills the CMSDropDownList with the numbers.
    /// </summary>
    /// <param name="drp">CMSDropDownList to fill</param>
    /// <param name="fromnumber">Starting number</param>
    /// <param name="tonumber">Edning number</param>
    private void FillNumbers(CMSDropDownList drp, int fromNumber, int toNumber)
    {
        FillNumbers(drp, fromNumber, toNumber, fromNumber);
    }


    /// <summary>
    /// Fills the CMSDropDownList with the numbers. Text and value of CMSDropDownList are
    /// filled separately.
    /// </summary>
    /// <param name="drp">CMSDropDownList to fill</param>
    /// <param name="fromnumber">Starting number (text)</param>
    /// <param name="tonumber">Ending number (text)</param>
    /// <param name="valFromNumber">Starting number (value)</param>
    private void FillNumbers(CMSDropDownList drp, int fromnumber, int tonumber, int valFromNumber)
    {
        drp.Items.Clear();

        // Get length of number
        int max = tonumber.ToString().Length;
        for (int i = fromnumber; i <= tonumber; i++)
        {
            // Format string number with nulls
            string item = i.ToString().PadLeft(max, '0');

            // Add this number to dropdown list
            drp.Items.Add(new ListItem(item, valFromNumber.ToString()));
            valFromNumber++;
        }
    }


    /// <summary>
    /// Fills the CMSDropDownList with the months values in corect culture format.
    /// </summary>
    /// <param name="drp">CMSDropDownList to fill</param>
    /// <param name="culture">Culture</param>
    private void FillMonths(CMSDropDownList drp, string culture)
    {
        drp.Items.Clear();

        // Get DateTime information
        DateTime dt = DateTimeHelper.GetYearStart(DateTime.Now);
        IFormatProvider ifp = CultureHelper.GetCultureInfo(mCulture);

        string item = null;

        for (int i = 1; i <= 12; i++)
        {
            item = i.ToString();
            if (i < 10)
            {
                item = "0" + item;
            }

            // Fill dropdown list
            drp.Items.Add(new ListItem(item, dt.ToString("MM", ifp)));
            dt = dt.AddMonths(1);
        }
    }


    /// <summary>
    /// Sets the calendar time to the given time.
    /// </summary>
    /// <param name="time">Time to set</param>
    private void SetTime(DateTime time, bool selectCalendar)
    {
        if (selectCalendar)
        {
            calDate.VisibleDate = time.Date;
            calDate.SelectedDate = time.Date;
        }

        // Get culture specific year
        string cultureYear = time.ToString("yyyy", CultureHelper.GetCultureInfo(mCulture));
        int cultureYearInt = ValidationHelper.GetInteger(cultureYear, time.Year);

        FillNumbers(drpYear, cultureYearInt - 5, cultureYearInt + 5, time.Year - 5);

        // Set the new time
        drpHours.SelectedValue = time.Hour.ToString();
        drpMinutes.SelectedValue = time.Minute.ToString();
        drpSeconds.SelectedValue = time.Second.ToString();
        drpYear.SelectedValue = time.Year.ToString();
        drpMonth.SelectedValue = time.ToString("MM", CultureHelper.GetCultureInfo(mCulture));
    }


    protected void btnNow_Click(Object sender, EventArgs e)
    {
        SetTime(TimeZoneHelper.GetUserDateTime(MembershipContext.AuthenticatedUser), true);
    }


    /// <summary>
    /// Gets selected time from calendar
    /// </summary>    
    protected DateTime GetCurrentTime()
    {
        DateTime selTime = DateTime.MinValue;
        selTime = calDate.SelectedDate.Date;
        selTime = selTime.AddHours(drpHours.SelectedIndex);
        selTime = selTime.AddMinutes(drpMinutes.SelectedIndex);
        selTime = selTime.AddSeconds(drpSeconds.SelectedIndex);
        return selTime;
    }


    protected void btnOK_Click(Object sender, EventArgs e)
    {
        DateTime selTime = GetCurrentTime();

        // Add 'Close window' script
        if (editTime)
        {
            ltlScript.Text = ScriptHelper.GetScript("CloseWindow(" + ScriptHelper.GetString(controlId) + ", '" + selTime.ToString("d", CultureHelper.GetCultureInfo(mCulture)) + selTime.ToString(" HH:mm:ss") + "');");
        }
        else
        {
            ltlScript.Text = ScriptHelper.GetScript("CloseWindow(" + ScriptHelper.GetString(controlId) + ", '" + selTime.ToString("d", CultureHelper.GetCultureInfo(mCulture)) + "');");
        }
    }


    protected void calDate_SelectionChanged(object sender, EventArgs e)
    {
        SetTime(GetCurrentTime(), false);
    }


    protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            // Change DateTime to the new value
            DateTime curTime = GetCurrentTime();
            DateTime newTime = new DateTime(Convert.ToInt32(drpYear.SelectedValue), curTime.Month, curTime.Day, curTime.Hour, curTime.Minute, curTime.Second);
            SetTime(newTime, true);
        }
        catch
        {
        }
    }


    protected void drpMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            // Change DateTime to the new value
            DateTime curTime = GetCurrentTime();
            DateTime newTime = new DateTime(curTime.Year, Convert.ToInt32(drpMonth.SelectedValue), curTime.Day, curTime.Hour, curTime.Minute, curTime.Second);
            SetTime(newTime, true);
        }
        catch
        {
        }
    }
}
