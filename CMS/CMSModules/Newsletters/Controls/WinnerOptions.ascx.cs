using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.UIControls;


public partial class CMSModules_Newsletters_Controls_WinnerOptions : CMSAdminControl
{

    #region "Constants"

    private const string INTERVAL_MINUTES = "-1";
    private const string INTERVAL_HOURS = "0";
    private const string INTERVAL_DAYS = "1";
    private const string INTERVAL_WEEKS = "2";
    private const string INTERVAL_MONTHS = "3";

    #endregion


    private bool mEnabled = true;


    #region "Properties"

    /// <summary>
    /// Gets or sets value that indicates how to select a winner.
    /// </summary>
    public ABTestWinnerSelectionEnum WinnerSelection
    {
        get { return GetWinnerSelection(); }
        set { SetWinnerSelection(value); }
    }


    /// <summary>
    /// Gets or sets time interval in minutes.
    /// </summary>
    public int TimeInterval
    {
        get { return GetTimeInterval(); }
        set { SetTimeInterval(value); }
    }


    /// <summary>
    /// Enables/disables control.
    /// </summary>
    public bool Enabled
    {
        get { return mEnabled; }
        set { mEnabled = value; }
    }

    #endregion


    #region "Events"

    public event EventHandler OnChange;

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        radClicks.CheckedChanged += new EventHandler(radClicks_CheckedChanged);
        radOpen.CheckedChanged += new EventHandler(radClicks_CheckedChanged);
        radManually.CheckedChanged += new EventHandler(radClicks_CheckedChanged);

        ReloadData();
    }


    /// <summary>
    /// Forcibly reloads control data.
    /// </summary>
    /// <param name="forceReload">Enforce reload</param>
    public override void ReloadData(bool forceReload)
    {
        if (forceReload || !RequestHelper.IsPostBack())
        {
            SetWinnerSelection(WinnerSelection);
            SetTimeInterval(TimeInterval);
        }
        radClicks.Enabled = radOpen.Enabled = radManually.Enabled = drpInterval.Enabled = txtInterval.Enabled = Enabled;
        radClicks_CheckedChanged(this, EventArgs.Empty);
    }


    protected void radClicks_CheckedChanged(object sender, EventArgs e)
    {
        // Disable interval settings if winner will be selected manually
        txtInterval.Enabled = !radManually.Checked;
        txtInterval.ReadOnly = radManually.Checked;
        drpInterval.Enabled = !radManually.Checked;

        if (OnChange != null)
        {
            OnChange(this, EventArgs.Empty);
        }
    }


    /// <summary>
    /// Loads specified time interval to control.
    /// </summary>
    /// <param name="timeInterval">Time interval in hours</param>
    private void SetTimeInterval(int timeInterval)
    {
        // Load dropdown list for the first time
        if (drpInterval.Items.Count <= 0)
        {
            drpInterval.Items.Add(new ListItem(GetString("newsletterissue_send.minutes"), INTERVAL_MINUTES));
            drpInterval.Items.Add(new ListItem(GetString("newsletterissue_send.hours"), INTERVAL_HOURS));
            drpInterval.Items.Add(new ListItem(GetString("newsletterissue_send.days"), INTERVAL_DAYS));
            drpInterval.Items.Add(new ListItem(GetString("newsletterissue_send.weeks"), INTERVAL_WEEKS));
            drpInterval.Items.Add(new ListItem(GetString("newsletterissue_send.months"), INTERVAL_MONTHS));
        }

        Dictionary<string, int> intervals = new Dictionary<string,int>();
        intervals.Add(INTERVAL_MONTHS, 60 * 24 * 30);
        intervals.Add(INTERVAL_WEEKS, 60 * 24 * 7);
        intervals.Add(INTERVAL_DAYS, 60 * 24);
        intervals.Add(INTERVAL_HOURS, 60);
        intervals.Add(INTERVAL_MINUTES, 1);

        foreach (string key in intervals.Keys)
        {
            if ((timeInterval % intervals[key]) == 0)
            {
                drpInterval.SelectedValue = key;
                txtInterval.Text = Convert.ToString(timeInterval / intervals[key]);
                break;
            }
        }
    }


    /// <summary>
    /// Returns time interval from control.
    /// </summary>
    private int GetTimeInterval()
    {
        int value = ValidationHelper.GetInteger(txtInterval.Text, 0);
        switch (drpInterval.SelectedValue)
        {
            case INTERVAL_HOURS:
                return value * 60;
            case INTERVAL_DAYS:
                return value * 60 * 24;
            case INTERVAL_WEEKS:
                return value * 60 * 24 * 7;
            case INTERVAL_MONTHS:
                return value * 60 * 24 * 30; 
            default:
                return value;
        }
    }


    /// <summary>
    /// Load specified option to controls.
    /// </summary>
    /// <param name="winnerSelection">Winner selection option</param>
    private void SetWinnerSelection(ABTestWinnerSelectionEnum winnerSelection)
    {
        radClicks.Checked = radOpen.Checked = radManually.Checked = false;
        radClicks.Checked = winnerSelection == ABTestWinnerSelectionEnum.TotalUniqueClicks;
        radOpen.Checked = winnerSelection == ABTestWinnerSelectionEnum.OpenRate;
        radManually.Checked = winnerSelection == ABTestWinnerSelectionEnum.Manual;
    }


    /// <summary>
    /// Returns winner selection option from controls.
    /// </summary>
    private ABTestWinnerSelectionEnum GetWinnerSelection() 
    {
        if (radClicks.Checked) return ABTestWinnerSelectionEnum.TotalUniqueClicks;
        else if (radOpen.Checked) return ABTestWinnerSelectionEnum.OpenRate;
        return ABTestWinnerSelectionEnum.Manual;
    }
}