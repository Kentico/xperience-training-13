using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Modules;
using CMS.Newsletters;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Newsletters_Controls_VariantMailout : CMSAdminControl
{
    #region "Constants"

    private const string WINNER_BACKGROUND_COLOR = "#B9FFB9";
    private const string DDLIST_SETSELECTED = "-1";
    private const string DDLIST_SETALL = "0";

    private const string ZERO_PERCENT = "0%";
    private const string ZERO = "0";

    #endregion


    #region "Private variables"

    private bool mShowSelectionColumn = true;
    private bool mEnableMailoutTimeSetting = true;
    private bool mShowSelectWinnerAction;
    private bool mBounceMonitoringEnabled;
    private DateTime mHighestMailoutTime = DateTime.MinValue;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets parent issue ID.
    /// </summary>
    public int ParentIssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Shows/hides selection column in the grid
    /// </summary>
    public bool ShowSelectionColumn
    {
        get { return mShowSelectionColumn; }
        set { mShowSelectionColumn = value; }
    }


    /// <summary>
    /// Enables/disables mailout setting.
    /// </summary>
    public bool EnableMailoutTimeSetting
    {
        get { return mEnableMailoutTimeSetting; }
        set { mEnableMailoutTimeSetting = value; }
    }


    /// <summary>
    /// Shows/hides action column with winner selection.
    /// </summary>
    public bool ShowSelectWinnerAction
    {
        get { return mShowSelectWinnerAction; }
        set { mShowSelectWinnerAction = value; }
    }


    /// <summary>
    /// Shows/hides issue status column.
    /// </summary>
    public bool ShowIssueStatus
    {
        get;
        set;
    }


    /// <summary>
    /// Shows/hides opened e-mail counters.
    /// </summary>
    public bool ShowOpenedEmails
    {
        get;
        set;
    }


    /// <summary>
    /// Shows/hides unique clicks counters.
    /// </summary>
    public bool ShowUniqueClicks
    {
        get;
        set;
    }


    /// <summary>
    /// Shows/hides sent e-mails counters.
    /// </summary>
    public bool ShowSentEmails
    {
        get;
        set;
    }


    /// <summary>
    /// Shows/hides delivered e-mails counters.
    /// </summary>
    public bool ShowDeliveredEmails

    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value, that indicates whether grouping text for entire control is used
    /// instead of ordinary label.
    /// </summary>
    public bool UseGroupingText
    {
        get;
        set;
    }


    /// <summary>
    /// Highest mail-out time in the grid.
    /// </summary>
    public DateTime HighestMailoutTime
    {
        get { return mHighestMailoutTime; }
    }


    /// <summary>
    /// Gets or sets ID of the winner.
    /// </summary>
    private int WinnerIssueID
    {
        get;
        set;
    }


    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when error message is shown.
    /// </summary>
    public event EventHandler OnShowError;


    /// <summary>
    /// Occurs when mailout time has been changed.
    /// </summary>
    public event EventHandler OnChanged;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Show/hide additional columns as needed
        grdElem.NamedColumns["opened"].Visible = grdElem.NamedColumns["openRate"].Visible = ShowOpenedEmails;
        grdElem.NamedColumns["clicks"].Visible = grdElem.NamedColumns["clickRate"].Visible = ShowUniqueClicks;
        grdElem.NamedColumns["status"].Visible = ShowIssueStatus;
        grdElem.NamedColumns["sentEmails"].Visible = ShowSentEmails;
        grdElem.NamedColumns["delivered"].Visible = grdElem.NamedColumns["deliveryRate"].Visible = ShowDeliveredEmails && mBounceMonitoringEnabled;
    }


    public override void ReloadData(bool forceReload)
    {
        if (StopProcessing && !forceReload)
        {
            return;
        }
        InitControls(forceReload);

        // Javascript for handling winner mailout time
        var scriptBlock = string.Format(@"function SelWinner(id) {{ modalDialog('{0}?objectid=' + id, 'NewsletterWinnerMailout', '700px', '425px'); return false; }}",
            ResolveUrl(@"~\CMSModules\Newsletters\Tools\Newsletters\Newsletter_Issue_WinnerMailout.aspx"));

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Actions", scriptBlock, true);
        ScriptHelper.RegisterTooltip(Page);

        // Register handlers
        grdElem.OnExternalDataBound -= grdElem_OnExternalDataBound;
        grdElem.OnExternalDataBound += grdElem_OnExternalDataBound;
        grdElem.GridView.RowDataBound -= GridView_RowDataBound;
        grdElem.GridView.RowDataBound += GridView_RowDataBound;

        // Get winner ID if any
        var abTestInfo = ABTestInfoProvider.GetABTestInfoForIssue(ParentIssueID);
        if (abTestInfo != null)
        {
            WinnerIssueID = abTestInfo.TestWinnerIssueID;
        }

        mBounceMonitoringEnabled = NewsletterHelper.MonitorBouncedEmails(SiteContext.CurrentSiteName);

        grdElem.OrderBy = "IssueVariantName, IssueID";
        grdElem.WhereCondition = GetWhereCondition(ParentIssueID, false);
        grdElem.ShowActionsMenu = false;
        grdElem.ShowObjectMenu = false;
        if (!ShowSelectWinnerAction)
        {
            grdElem.GridActions = null;
        }
        grdElem.ReloadData();
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Validate date/time (blank date/time textbox is allowed)
        if ((dtpMailout.SelectedDateTime == DateTimeHelper.ZERO_TIME) && !string.IsNullOrEmpty(dtpMailout.DateTimeTextBox.Text.Trim()))
        {
            ShowErrorInternal(GetString("newsletterissue_send.invaliddatetime"));
            return;
        }

        // Get variants (IDs) to set
        var selItems = new List<string>();

        if (drpAllSelected.SelectedValue == DDLIST_SETALL)
        {
            selItems = IssueInfo.Provider.Get()
                                        .Where(GetWhereCondition(ParentIssueID, true))
                                        .Column("IssueID")
                                        .TypedResult
                                        .Select(s => s.IssueID.ToString())
                                        .ToList();

        }
        else if (drpAllSelected.SelectedValue == DDLIST_SETSELECTED)
        {
            selItems = grdElem.SelectedItems;
        }
        else
        {
            selItems.Add(drpAllSelected.SelectedValue);
        }

        // Check selected variants
        if (!selItems.Any())
        {
            if (drpAllSelected.SelectedValue != DDLIST_SETALL)
            {
                ShowErrorInternal(GetString("newsletterissue_send.novariantsselected"));
            }
            else
            {
                pMOut.Visible = false;
            }
            return;
        }

        var when = (dtpMailout.SelectedDateTime == DateTimeHelper.ZERO_TIME) ? DateTime.Now : dtpMailout.SelectedDateTime;
        foreach (var itemId in selItems)
        {
            var issue = IssueInfo.Provider.Get(ValidationHelper.GetInteger(itemId, 0));
            if (issue == null) continue;
            var task = NewsletterTasksManager.EnsureMailoutTask(issue, DateTime.Now, false);
            task.TaskNextRunTime = when;
            TaskInfo.Provider.Set(task);
            if (issue.IssueScheduledTaskID != task.TaskID)
            {
                using (new CMSActionContext { LogSynchronization = false })
                { 
                    issue.IssueScheduledTaskID = task.TaskID;
                    IssueInfo.Provider.Set(issue);
                }
            }
        }

        mHighestMailoutTime = DateTime.MinValue;
        grdElem.ResetSelection();
        grdElem.ReloadData();

        OnChanged?.Invoke(this, EventArgs.Empty);
    }


    #region "Unigrid handlers"

    protected object grdElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "IssueVariantName":
                return GetIssueVariantName(parameter as DataRowView);

            case "MailoutTime":
                return GetMailoutTime(parameter as DataRowView);

            case "IssueStatus":
                return GetIssueStatus(parameter);

            case "IssueSentEmails":
                var num = ValidationHelper.GetInteger(parameter, 0);
                return (num > 0) ? num.ToString() : string.Empty;

            case "IssueOpenedEmails":
                return GetOpenedEmails(parameter as DataRowView);

            case "IssueOpenedEmailsRate":
                var opeRateTooltip = GetString(mBounceMonitoringEnabled ? "newsletter.openratetooltip.delivered" : "newsletter.openratetooltip.sent");
                AddTooltip(opeRateTooltip, sender as WebControl);

                return GetOpenedEmailsRate(parameter as DataRowView);

            case "UniqueClicks":
                return GetUniqueClicks(parameter as DataRowView);

            case "UniqueClicksRate":
                var uniqueRateTooltip = GetString(mBounceMonitoringEnabled ? "newsletter.clickratetooltip.delivered" : "newsletter.clickratetooltip.sent");
                AddTooltip(uniqueRateTooltip, sender as WebControl);

                return GetUniqueClicksRate(parameter as DataRowView);

            case "Delivered":
                return GetDeliveryCount(parameter as DataRowView);

            case "DeliveryRate":
                return GetDeliveryRate(parameter as DataRowView);

            default:
                return parameter;
        }
    }


    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
        {
            return;
        }

        if (WinnerIssueID == ValidationHelper.GetInteger(((DataRowView)e.Row.DataItem).Row["IssueID"], 0))
        {
            e.Row.Style.Add("background-color", WINNER_BACKGROUND_COLOR);
        }
    }

    #endregion


    /// <summary>
    /// Initializes controls
    /// </summary>
    protected void InitControls(bool forceReload)
    {
        grdElem.GridOptions.ShowSelection = ShowSelectionColumn;
        pMOut.Visible = EnableMailoutTimeSetting;

        if (EnableMailoutTimeSetting)
        {
            if (forceReload || (drpAllSelected.Items.Count <= 0))
            {
                drpAllSelected.Items.Clear();
                if (ShowSelectionColumn)
                {
                    drpAllSelected.Items.Add(new ListItem(GetString("general.selectall"), DDLIST_SETALL));
                    drpAllSelected.Items.Add(new ListItem(GetString("newsletterissue_send.selected"), DDLIST_SETSELECTED));
                }

                var items = IssueHelper.GetIssueVariants(ParentIssueID, "IssueMailoutTime IS NULL");
                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        drpAllSelected.Items.Add(new ListItem(item.IssueVariantName, item.IssueID.ToString()));
                    }
                }
            }

            // Hide entire panel if no variant available
            if (drpAllSelected.Items.Count <= 0)
            {
                pMOut.Visible = false;
            }
        }

        pnlMailoutHeading.ResourceString = UseGroupingText ? "newsletterissue_send.schedulemailout" : "newsletterissue_send.testresults";
    }


    /// <summary>
    /// Shows error message.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    private void ShowErrorInternal(string errorMessage)
    {
        ErrorMessage = errorMessage;
        if (OnShowError != null)
        {
            OnShowError(this, EventArgs.Empty);
        }
        else
        {
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Returns WHERE condition
    /// </summary>
    /// <param name="issueId">Issue ID</param>
    /// <param name="notSentOnly">If TRUE additional condition is included</param>
    private string GetWhereCondition(int issueId, bool notSentOnly)
    {
        return $"IssueVariantOfIssueID={issueId}{(notSentOnly ? " AND IssueMailoutTime IS NULL" : string.Empty)}";
    }


    private void AddTooltip(string tooltipText, WebControl webControl)
    {
        if (webControl != null && !string.IsNullOrEmpty(tooltipText))
        {
            ScriptHelper.AppendTooltip(webControl, tooltipText, null);
        }
    }


    private object GetIssueVariantName(DataRowView rowView)
    {
        var issue = new IssueInfo(rowView.Row);
        var result = issue.GetVariantName();

        // Issue has not been sent yet => get mail out time from scheduled task
        if (issue.IssueID == WinnerIssueID)
        {
            result += " " + GetString("newsletterabtest.winner");
        }

        return HTMLHelper.HTMLEncode(result);
    }


    private object GetMailoutTime(DataRowView rowView)
    {
        string result;
        var mailoutTime = ValidationHelper.GetDateTime(rowView["IssueMailoutTime"], DateTimeHelper.ZERO_TIME);

        if (mailoutTime == DateTimeHelper.ZERO_TIME)
        {
            mailoutTime = GetMailoutTimeFromScheduledTask(rowView);

            result = FormatMailoutTime(mailoutTime);
        }
        else
        {
            result = mailoutTime.ToString();
        }

        EnsureHighestMailoutTime(mailoutTime);

        return HTMLHelper.HTMLEncode(result);
    }


    private DateTime GetMailoutTimeFromScheduledTask(DataRowView rowView)
    {
        var taskId = ValidationHelper.GetInteger(rowView["IssueScheduledTaskID"], 0);
        var task = TaskInfo.Provider.Get(taskId);
        if (task != null && task.TaskNextRunTime > DateTimeHelper.ZERO_TIME)
        {
            return task.TaskNextRunTime;
        }

        return DateTimeHelper.ZERO_TIME;
    }


    private string FormatMailoutTime(DateTime mailoutTime)
    {
        if (mailoutTime == DateTimeHelper.ZERO_TIME)
        {
            return GetString("general.na");
        }

        return mailoutTime < DateTime.Now ? $"{mailoutTime} {GetString("newsletterissue_send.asap")}" : mailoutTime.ToString();
    }


    private void EnsureHighestMailoutTime(DateTime mailoutTime)
    {
        if (mHighestMailoutTime < mailoutTime)
        {
            mHighestMailoutTime = mailoutTime;
        }
    }


    private object GetIssueStatus(object issueStatus)
    {
        var status = IssueStatusEnum.Idle;
        if (issueStatus != DBNull.Value && issueStatus != null)
        {
            status = (IssueStatusEnum)issueStatus;
        }
        return IssueHelper.GetStatusFriendlyName(status, null);
    }


    /// <summary>
    /// Gets a clickable opened emails counter based on the values from datasource.
    /// </summary>
    /// <param name="rowView">A <see cref="DataRowView" /> that represents one row from UniGrid's source</param>
    private string GetOpenedEmails(DataRowView rowView)
    {
        var issueSentEmails = DataHelper.GetIntValue(rowView.Row, "IssueSentEmails");
        if (issueSentEmails == 0)
        {
            return string.Empty;
        }

        // Get issue ID
        var issueId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueID"), 0);

        // Get opened emails count from issue record
        var openedEmails = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueOpenedEmails"), 0);

        if (openedEmails <= 0)
        {
            return ZERO;
        }

        var url = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.NEWSLETTER, "Newsletter.Issue.Reports.Opens", issueId);
        return $@"<a href=""#"" onclick=""modalDialog('{url}', 'NewsletterOpenedEmails', '1000px', '700px'); return false;"">{openedEmails}</a>";
    }


    private string GetOpenedEmailsRate(DataRowView rowView)
    {
        var issueSentEmails = DataHelper.GetIntValue(rowView.Row, "IssueSentEmails");
        if (issueSentEmails == 0)
        {
            return string.Empty;
        }

        var openedEmails = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueOpenedEmails"), 0);
        var delivered = GetDelivered(rowView.Row);

        return FormatRate(openedEmails, delivered);
    }


    /// <summary>
    /// Gets a clickable click links counter based on the values from datasource.
    /// </summary>
    /// <param name="rowView">A <see cref="DataRowView" /> that represents one row from UniGrid's source</param>
    private string GetUniqueClicks(DataRowView rowView)
    {
        var issueSentEmails = DataHelper.GetIntValue(rowView.Row, "IssueSentEmails");
        if (issueSentEmails == 0)
        {
            return string.Empty;
        }

        var issueId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueID"), 0);
        var clicks = IssueHelper.GetIssueTotalUniqueClicks(issueId);

        if (clicks <= 0)
        {
            return ZERO;
        }

        var url = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.NEWSLETTER, "Newsletter.Issue.Reports.Clicks", issueId);
        return $@"<a href=""#"" onclick=""modalDialog('{url}', 'NewsletterTrackedLinks', '1000px', '700px'); return false;"">{clicks}</a>";
    }


    private string GetUniqueClicksRate(DataRowView rowView)
    {
        var issueSentEmails = DataHelper.GetIntValue(rowView.Row, "IssueSentEmails");
        if (issueSentEmails == 0)
        {
            return string.Empty;
        }

        var issueId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(rowView, "IssueID"), 0);
        var clicks = IssueHelper.GetIssueTotalUniqueClicks(issueId);
        var delivered = GetDelivered(rowView.Row);

        return FormatRate(clicks, delivered);
    }


    private string GetDeliveryCount(DataRowView rowView)
    {
        var sent = DataHelper.GetIntValue(rowView.Row, "IssueSentEmails");

        return sent == 0 ? string.Empty : GetDelivered(rowView.Row).ToString();
    }


    private string GetDeliveryRate(DataRowView rowView)
    {
        var sent = DataHelper.GetIntValue(rowView.Row, "IssueSentEmails");

        return sent == 0 ? string.Empty : FormatRate(GetDelivered(rowView.Row), sent);
    }


    private static string FormatRate(int currentValue, int maxValue)
    {
        if (currentValue > 0 && maxValue > 0)
        {
            return $"{((double)currentValue / maxValue) * 100:F2}%";
        }

        return ZERO_PERCENT;
    }


    private int GetDelivered(DataRow dataRow)
    {
        var sent = DataHelper.GetIntValue(dataRow, "IssueSentEmails");

        if (!mBounceMonitoringEnabled)
        {
            return sent;
        }

        var bounces = DataHelper.GetIntValue(dataRow, "IssueBounces");
        return sent - bounces;
    }
}
