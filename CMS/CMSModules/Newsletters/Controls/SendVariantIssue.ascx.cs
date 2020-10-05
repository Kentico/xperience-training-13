using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Core;
using CMS.Core.Internal;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Scheduler;
using CMS.UIControls;


public partial class CMSModules_Newsletters_Controls_SendVariantIssue : CMSAdminControl
{
    #region "Constants"

    private const int DEFAULT_TEST_GROUP_SIZE_PERCENTAGE = 10;

    #endregion


    #region "Private variables"

    private IssueInfo mParentIssue;
    private ABTestInfo mABTest;
    private bool mEnabled;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of newsletter issue that should be sent, required for template based newsletters.
    /// </summary>
    public int IssueID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether control is enabled for editing.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }

        set
        {
            mEnabled = ucGroupSlider.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets current state of control ("waiting to send", "testing finished" etc.)
    /// </summary>
    private VariantStatusEnum CurrentState
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when info message needs to be redrawn.
    /// </summary>
    public event EventHandler OnChanged;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible || StopProcessing || (IssueID <= 0))
        {
            return;
        }
        
        ReloadData(false);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads control data.
    /// </summary>
    /// <param name="forceReload">Indicates if force reload should be used</param>
    public override void ReloadData(bool forceReload)
    {
        if (StopProcessing && !forceReload)
        {
            return;
        }

        var parentIssueId = 0;
        mParentIssue = IssueInfoProvider.GetOriginalIssue(IssueID);
        if (mParentIssue != null)
        {
            parentIssueId = mParentIssue.IssueID;
        }

        // Get A/B test configuration
        mABTest = ABTestInfoProvider.GetABTestInfoForIssue(parentIssueId);
        if (mABTest == null)
        {
            // Ensure A/B test object with default settings
            mABTest = new ABTestInfo
                { TestIssueID = parentIssueId, TestSizePercentage = 50, TestWinnerOption = ABTestWinnerSelectionEnum.Manual };
            ABTestInfo.Provider.Set(mABTest);
        }

        CurrentState = GetCurrentState(mParentIssue);
        if (mParentIssue.IssueForAutomation)
        {
            InitControlsForIssueUsedInAutomation(CurrentState, forceReload);
        }
        else
        {
            InitControls(CurrentState, forceReload);
        }

        ucMailout.ParentIssueID = parentIssueId;
        ucMailout.ReloadData(forceReload);

        InfoMessage = GetInfoMessage(CurrentState, mParentIssue, mABTest?.TestWinnerOption ?? ABTestWinnerSelectionEnum.Manual);

        // Init test group slider
        InitTestGroupSlider(mParentIssue, mABTest, forceReload);
    }


    private void InitTestGroupSlider(IssueInfo parentIssue, ABTestInfo abTest, bool forceReload)
    {
        var variants = IssueHelper.GetIssueVariants(parentIssue);
        ucGroupSlider.Variants = variants;

        ucGroupSlider.NumberOfSubscribers = GetNumberOfSubscribers(parentIssue, variants);
        
        if (forceReload || !ucGroupSlider.Enabled)
        {
            ucGroupSlider.CurrentSize = abTest?.TestSizePercentage ?? DEFAULT_TEST_GROUP_SIZE_PERCENTAGE;
        }
        ucGroupSlider.ReloadData(forceReload);
    }


    private static int GetNumberOfSubscribers(IssueInfo parentIssue, IReadOnlyCollection<IssueABVariantItem> variants)
    {
        if (parentIssue.IssueStatus != IssueStatusEnum.Finished && !AreAllVariantsSent(variants))
        {
            return NewsletterHelper.GetEmailAddressCount(parentIssue);
        }

        if (parentIssue.IssueStatus == IssueStatusEnum.Finished)
        {
            // Issue was sent => get number of subscribers from number of sent issues
            return parentIssue.IssueSentEmails;
        }

        // Only variants was sent => get current number of subscribers
        return NewsletterHelper.GetEmailAddressCount(parentIssue);
    }


    private static bool AreAllVariantsSent(IssueInfo parentIssue)
    {
        var variants = IssueHelper.GetIssueVariants(parentIssue);
        return AreAllVariantsSent(variants);
    }


    private static bool AreAllVariantsSent(IReadOnlyCollection<IssueABVariantItem> variants)
    {
        var allVariantsSent = true;
        if (variants != null)
        {
            allVariantsSent = variants.All(item => item.IssueStatus == IssueStatusEnum.Finished);
        }
        return allVariantsSent;
    }


    /// <summary>
    /// Collects data from controls and fills A/B test info.
    /// </summary>
    /// <param name="abi">A/B test info</param>
    private bool SaveABTestInfo(ABTestInfo abi)
    {
        if (abi == null)
        {
            return false;
        }

        var selectionChanged = (abi.TestWinnerOption != ucWO.WinnerSelection) || (abi.TestSelectWinnerAfter != ucWO.TimeInterval)
            || (abi.TestSizePercentage != ucGroupSlider.CurrentSize);

        abi.TestWinnerOption = ucWO.WinnerSelection;
        abi.TestSelectWinnerAfter = ucWO.TimeInterval;
        abi.TestSizePercentage = ucGroupSlider.CurrentSize;

        return selectionChanged;
    }


    /// <summary>
    /// Initializes controls
    /// </summary>
    /// <param name="currentState">Current state of the control (controls are initializes according this value)</param>
    /// <param name="forceReload">Indicates if force data reload should be performed</param>
    private void InitControls(VariantStatusEnum currentState, bool forceReload)
    {
        switch (currentState)
        {
            case VariantStatusEnum.Finished:
            case VariantStatusEnum.ReadyForSending:
                ucGroupSlider.Enabled = false;

                InitMailoutControlForIssue(false, false, false, false);
                InitWinnerOption(false, mABTest, forceReload);
                
                lblAdditionalInfo.Visible = true;
                lblAdditionalInfo.Text = GetString("newsletterissue_send.variantsendingfinished");
                break;
            
            case VariantStatusEnum.ReadyForTesting:
                ucGroupSlider.Enabled = Enabled;

                InitMailoutControlForIssue(false, true, true, false);
                InitWinnerOption(Enabled, mABTest, forceReload);
                
                lblAdditionalInfo.Visible = true;
                lblAdditionalInfo.Text = GetString("newsletterissue_send.infovariantsending");
                break;

            case VariantStatusEnum.WaitingToSelectWinner:
                ucGroupSlider.Enabled = false;

                InitMailoutControlForIssue(true, false, true, false);
                InitWinnerOption(Enabled, mABTest, forceReload);
                
                lblAdditionalInfo.Visible = true;
                lblAdditionalInfo.Text = GetString("newsletterissue_send.infowaitingtoselwinner");
                break;

            case VariantStatusEnum.WaitingToSend:
                ucGroupSlider.Enabled = Enabled;

                InitMailoutControlForIssue(false, true, true, true);
                InitWinnerOption(Enabled, mABTest, forceReload);
                break;
        }
    }


    private void InitControlsForIssueUsedInAutomation(VariantStatusEnum currentState, bool forceReload)
    {
        lblAdditionalInfo.Visible = false;
        lhdSizeOfTestGroup.Visible = false;
        pnlSlider.Visible = false;

        switch (currentState)
        {
            case VariantStatusEnum.WaitingToSend:
                InitMailoutControlForIssueUsedInAutomation(false, false);
                InitWinnerOption(Enabled, mABTest, forceReload);
                break;

            case VariantStatusEnum.ReadyForTesting:
            case VariantStatusEnum.WaitingToSelectWinner:
                InitMailoutControlForIssueUsedInAutomation(true, true);
                InitWinnerOption(Enabled, mABTest, forceReload);
                break;

            case VariantStatusEnum.ReadyForSending:
            case VariantStatusEnum.Finished:
                InitMailoutControlForIssueUsedInAutomation(true, false);
                InitWinnerOption(false, mABTest, forceReload);
                break;
        }
    }


    private void InitMailoutControlForIssueUsedInAutomation(bool visible, bool showSelectWinnerAction)
    {
        pnlMailout.Visible = visible;
        ucMailout.Visible = visible;
        ucMailout.ShowSelectWinnerAction = showSelectWinnerAction && Enabled;
        ucMailout.ShowSentEmails = true;
        ucMailout.ShowDeliveredEmails = true;
        ucMailout.ShowOpenedEmails = true;
        ucMailout.ShowUniqueClicks = true;
        ucMailout.ShowIssueStatus = true;
        ucMailout.ShowSelectionColumn = false;
        ucMailout.EnableMailoutTimeSetting = false;
    }


    private void InitMailoutControlForIssue(bool showSelectWinnerAction, bool showSelectionColumn, bool enableMailoutTimeSetting, bool useGroupingText)
    {
        ucMailout.ShowSelectWinnerAction = showSelectWinnerAction && Enabled;
        ucMailout.ShowSentEmails = true;
        ucMailout.ShowDeliveredEmails = true;
        ucMailout.ShowOpenedEmails = true;
        ucMailout.ShowUniqueClicks = true;
        ucMailout.ShowIssueStatus = true;
        ucMailout.ShowSelectionColumn = showSelectionColumn && Enabled;
        ucMailout.EnableMailoutTimeSetting = enableMailoutTimeSetting && Enabled;
        ucMailout.UseGroupingText = useGroupingText;
        ucMailout.OnChanged -= ucMailout_OnChanged;
        ucMailout.OnChanged += ucMailout_OnChanged;
    }


    protected void ucMailout_OnChanged(object sender, EventArgs e)
    {
        InfoMessage = GetInfoMessage(CurrentState, mParentIssue, mABTest.TestWinnerOption);

        OnChanged?.Invoke(this, EventArgs.Empty);
    }


    /// <summary>
    /// Returns information message according to current state, issue and A/B test.
    /// </summary>
    /// <param name="currState">Current state</param>
    /// <param name="issue">Issue</param>
    /// <param name="winnerOption">Winner option</param>
    private string GetInfoMessage(VariantStatusEnum currState, IssueInfo issue, ABTestWinnerSelectionEnum winnerOption)
    {
        if (issue == null)
        {
            return null;
        }

        switch (currState)
        {
            case VariantStatusEnum.WaitingToSend:
                return GetString("Newsletter_Issue_Header.NotSentYet");
            case VariantStatusEnum.WaitingToSelectWinner:
                return GetWaitingToSelectWinnerInfoMessage(issue, winnerOption);
            case VariantStatusEnum.ReadyForSending:
                return String.Format(GetString("newsletterinfo.issuescheduledwinnerselmanually"), GetTimeOrNA(issue.IssueMailoutTime), GetWinnerSelectionTime());
            case VariantStatusEnum.ReadyForTesting:
                return GetReadyForTestingInfoMessage(winnerOption);
            case VariantStatusEnum.Finished:
                return GetFinishedInfoMessage(issue, winnerOption);
            default:
                return null;
        }
    }


    private string GetWaitingToSelectWinnerInfoMessage(IssueInfo issue, ABTestWinnerSelectionEnum winnerOption)
    {
        // Get current planned winner selection task
        var taskToSelectWinner = TaskInfo.Provider.Get(mABTest.TestWinnerScheduledTaskID);
        var plannedWinnerSelectionTime = taskToSelectWinner?.TaskNextRunTime ?? DateTimeHelper.ZERO_TIME;

        switch (winnerOption)
        {
            case ABTestWinnerSelectionEnum.Manual:
                if (issue.IssueMailoutTime > DateTime.Now)
                {
                    return String.Format(GetString("newsletterinfo.issuesentwaitingtosentwinner"), GetTimeOrNA(issue.IssueMailoutTime), GetWinnerSelectionTime());
                }
                return String.Format(GetString("newsletterinfo.issuesentwaitingtoselwinnermanually"), GetTimeOrNA(issue.IssueMailoutTime));
            case ABTestWinnerSelectionEnum.OpenRate:
                return String.Format(GetString("newsletterinfo.issuesentwaitingtoselwinneropen"), GetTimeOrNA(issue.IssueMailoutTime), GetTimeOrNA(plannedWinnerSelectionTime));
            case ABTestWinnerSelectionEnum.TotalUniqueClicks:
                return String.Format(GetString("newsletterinfo.issuesentwaitingtoselwinnerclicks"), GetTimeOrNA(issue.IssueMailoutTime), GetTimeOrNA(plannedWinnerSelectionTime));
            default:
                return null;
        }
    }


    private string GetReadyForTestingInfoMessage(ABTestWinnerSelectionEnum winnerOption)
    {
        if (winnerOption.Equals(ABTestWinnerSelectionEnum.Manual))
        {
            return GetString("newsletter_issue_header.issuesending.manualwinner");
        }

        var plannedMailoutTime = GetPlannedMailoutTime(ucMailout.HighestMailoutTime);
        return String.Format(GetString("newsletter_issue_header.issuesending"), GetTimeOrNA(plannedMailoutTime));
    }


    private string GetFinishedInfoMessage(IssueInfo issue, ABTestWinnerSelectionEnum winnerOption)
    {
        switch (winnerOption)
        {
            case ABTestWinnerSelectionEnum.Manual:
                return String.Format(GetString("newsletterinfo.issuesentwinnerselmanually"), GetTimeOrNA(issue.IssueMailoutTime), GetWinnerSelectionTime());
            case ABTestWinnerSelectionEnum.OpenRate:
                return String.Format(GetString("newsletterinfo.issuesentwinnerselopen"), GetWinnerSelectionTime());
            case ABTestWinnerSelectionEnum.TotalUniqueClicks:
                return String.Format(GetString("newsletterinfo.issuesentwinnerselclicks"), GetWinnerSelectionTime());
            default:
                return null;
        }
    }


    /// <summary>
    /// Get time of winner selection converted to string.
    /// </summary>
    /// <returns>Returns time of winner selection or N/A when time is not known or abtest does not exist</returns>
    private string GetWinnerSelectionTime()
    {
        if (mABTest != null && (mABTest.TestWinnerSelected > DateTimeHelper.ZERO_TIME))
        {
            return mABTest.TestWinnerSelected.ToString();
        }

        return GetString("general.na");
    }


    /// <summary>
    /// Converts time to string. Returns N/A when time is not known or incorrect.
    /// </summary>
    private string GetTimeOrNA(DateTime dateTime)
    {
        return dateTime > DateTimeHelper.ZERO_TIME ? dateTime.ToString() : GetString("general.na");
    }


    /// <summary>
    /// Initialize winner option control.
    /// </summary>
    /// <param name="enable">Control state (enabled/disabled)</param>
    /// <param name="abi">A-B test info to load to control</param>
    /// <param name="forceReload">TRUE if force load should be performed</param>
    private void InitWinnerOption(bool enable, ABTestInfo abi, bool forceReload)
    {
        if (abi == null)
        {
            return;
        }

        ucWO.Visible = true;
        ucWO.Enabled = enable;
        if (forceReload)
        {
            ucWO.WinnerSelection = abi.TestWinnerOption;
            ucWO.TimeInterval = abi.TestSelectWinnerAfter;
        }
    }


    /// <summary>
    /// Calculates planned mail out time.
    /// </summary>
    private DateTime GetPlannedMailoutTime(DateTime highestMailoutTime)
    {
        if (highestMailoutTime == DateTimeHelper.ZERO_TIME)
        {
            highestMailoutTime = DateTime.Now;
        }

        if ((mABTest != null) && (mABTest.TestWinnerOption != ABTestWinnerSelectionEnum.Manual))
        {
            return highestMailoutTime.AddMinutes(mABTest.TestSelectWinnerAfter);
        }

        return highestMailoutTime;
    }


    /// <summary>
    /// "Sends" variant issue (enables all scheduled task associated to each variant).
    /// </summary>
    public bool SendIssue()
    {
        // Check current state before sending
        switch (CurrentState)
        {
            case VariantStatusEnum.ReadyForTesting:
                ErrorMessage = GetString("newsletterissue_send.sendissuereadytobesent");
                return false;
            case VariantStatusEnum.WaitingToSelectWinner:
            case VariantStatusEnum.Finished:
                ErrorMessage = GetString("newsletterissue_send.sendissuehasbeensent");
                return false;
        }

        if (!SaveIssue())
        {
            return false;
        }

        // Enable scheduled tasks and set 'Ready for sending' status to all variants
        var now = Service.Resolve<IDateTimeNowService>().GetDateTimeNow();
        Service.Resolve<IIssueScheduler>().ScheduleIssue(mParentIssue, now);

        return true;
    }


    /// <summary>
    /// Saves current newsletter settings.
    /// </summary>
    public bool SaveIssue()
    {
        try
        {
            switch (CurrentState)
            {
                case VariantStatusEnum.WaitingToSend:
                case VariantStatusEnum.ReadyForSending:
                case VariantStatusEnum.ReadyForTesting:
                case VariantStatusEnum.WaitingToSelectWinner:
                    if (mABTest == null)
                    {
                        mABTest = ABTestInfoProvider.GetABTestInfoForIssue(mParentIssue.IssueID);
                    }

                    // Get A/B test settings from controls
                    var abTestChanged = SaveABTestInfo(mABTest);

                    if (mABTest == null)
                    {
                        return false;
                    }

                    if (mABTest.TestWinnerOption != ABTestWinnerSelectionEnum.Manual)
                    {
                        // Check minimal time interval
                        if (mABTest.TestSelectWinnerAfter < 5)
                        {
                            ErrorMessage = GetString("newsletterissue_send.saveissuewrongwinnerselectioninterval");
                            return false;
                        }
                    }

                    // Check if test options has changed
                    if (abTestChanged && mABTest.TestWinnerIssueID > 0)
                    {
                        // Options has been changed => reset previously selected winner
                        NewsletterTasksManager.DeleteMailoutTask(mParentIssue.IssueGUID, mParentIssue.IssueSiteID);
                        mABTest.TestWinnerIssueID = 0;
                        mABTest.TestWinnerSelected = DateTimeHelper.ZERO_TIME;
                        // Hide/reload winner selection in issue mail-out grid
                        ucMailout.ReloadData(false);
                    }                    

                    ABTestInfo.Provider.Set(mABTest);

                    if (CurrentState == VariantStatusEnum.WaitingToSelectWinner)
                    {
                        NewsletterTasksManager.EnsureWinnerSelectionTask(mABTest, mParentIssue, true, ucMailout.HighestMailoutTime);
                    }

                    // Update info message for parent control
                    var currentState = GetCurrentState(mParentIssue);
                    InfoMessage = GetInfoMessage(currentState, mParentIssue, mABTest.TestWinnerOption);
                    return true;
                case VariantStatusEnum.Finished:
                    ErrorMessage = GetString("newsletterissue_send.saveissuehasbeensent");
                    break;
            }
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            return false;
        }
        return true;
    }


    /// <summary>
    /// Determines current state of newsletter A/B test.
    /// </summary>
    /// <param name="issue">Parent issue</param>
    private static VariantStatusEnum GetCurrentState(IssueInfo issue)
    {
        if (issue == null)
        {
            return VariantStatusEnum.Unknown;
        }

        switch (issue.IssueStatus)
        {
            case IssueStatusEnum.Idle:
                return VariantStatusEnum.WaitingToSend;
            case IssueStatusEnum.ReadyForSending:
                return AreAllVariantsSent(issue) ? VariantStatusEnum.ReadyForSending : VariantStatusEnum.ReadyForTesting;
            case IssueStatusEnum.TestPhase:
                return IssueHelper.IsWinnerSelected(issue) ? VariantStatusEnum.ReadyForSending : VariantStatusEnum.WaitingToSelectWinner;
            case IssueStatusEnum.PreparingData:
            case IssueStatusEnum.Sending:
            case IssueStatusEnum.Finished:
                return VariantStatusEnum.Finished;
            default:
                return VariantStatusEnum.Unknown;
        }
    }

    #endregion


    private enum VariantStatusEnum
    {
        Unknown,
        WaitingToSend,
        WaitingToSelectWinner,
        ReadyForTesting,
        ReadyForSending,
        Finished
    }
}
