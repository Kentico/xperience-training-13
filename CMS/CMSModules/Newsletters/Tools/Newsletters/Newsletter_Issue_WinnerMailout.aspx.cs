using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.UIControls;


[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.NEWSLETTER, "Newsletters")]
[Title("newsletter_winnermailout.title")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_WinnerMailout : CMSNewsletterDialog
{
    private IssueInfo mParentIssue;
    private ABTestInfo mABTest;


    private IssueInfo WinnerVariant => Issue;
    

    private IssueInfo ParentIssue => mParentIssue ?? (mParentIssue = IssueInfoProvider.GetOriginalIssue(WinnerVariant?.IssueVariantOfIssueID ?? 0));
    

    private ABTestInfo ABTest => mABTest ?? (mABTest = ABTestInfoProvider.GetABTestInfoForIssue(ParentIssue?.IssueID ?? 0));
    

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        PageTitle.ShowFullScreenButton = false;

        if ((WinnerVariant == null) || (ParentIssue == null))
        {
            return;
        }

        if (!CheckPermissionsOnIssueSite(WinnerVariant, "authorissues"))
        {
            RedirectToResourceNotAvailableOnSite(WinnerVariant.IssueDisplayName);
        }

        if (ParentIssue.IssueForAutomation)
        {
            btnSend.Click += SelectABTestWinnerUsedInMarketingAutomation;
            lblInfo.Text = String.Format(GetString("newsletter_winnermailout.marketingautomation.question"), HTMLHelper.HTMLEncode(WinnerVariant.GetVariantName()));
        }
        else
        {
            btnSend.Click += SelectABTestWinner;
            lblInfo.Text = String.Format(GetString("newsletter_winnermailout.question"), HTMLHelper.HTMLEncode(WinnerVariant.GetVariantName()));
        }

        pnlMailoutTime.Visible = !ParentIssue.IssueForAutomation;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        RegisterModalPageScripts();
        RegisterEscScript();

        // Register script for refreshing parent page
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "RefreshParent", "function RefreshPage() {if((wopener)&&(wopener.RefreshPage!=null)){wopener.RefreshPage();}}", true);
    }


    protected void SelectABTestWinnerUsedInMarketingAutomation(object sender, EventArgs e)
    {
        using (new CMSActionContext { LogSynchronization = false })
        {
            SetABTestWinnerManually(ABTest, ParentIssue, WinnerVariant);

            CopyDataFromWinnerToParent(WinnerVariant, ParentIssue);
        }

        CloseDialogAndRefreshParentPage();
    }


    protected void SelectABTestWinner(object sender, EventArgs e)
    {
        // Validate date/time
        if (dtpMailout.SelectedDateTime == DateTimeHelper.ZERO_TIME)
        {
            ShowError(GetString("newsletterissue_send.invaliddatetime"));
            
            return;
        }

        // Check if winner has already been selected and sent
        if (ABTest.TestWinnerIssueID != 0)
        {
            if ((ParentIssue.IssueStatus == IssueStatusEnum.Finished) || (ParentIssue.IssueStatus == IssueStatusEnum.Sending))
            {
                CloseDialogAndRefreshParentPage();
                
                return;
            }
        }

        using (new CMSActionContext { LogSynchronization = false })
        {

            SetABTestWinnerManually(ABTest, ParentIssue, WinnerVariant);

            NewsletterSendingStatusModifier.ResetAllEmailsInQueueForIssue(ParentIssue.IssueID);

            CopyDataFromWinnerToParent(WinnerVariant, ParentIssue);

            ScheduleSendingOfIssue(ParentIssue);
        }
        
        CloseDialogAndRefreshParentPage();
    }

    
    private void SetABTestWinnerManually(ABTestInfo abTest, IssueInfo parentIssue, IssueInfo winnerVariant)
    {
        // Update A/B test info and winner selection task (if exist)
        abTest.TestWinnerOption = ABTestWinnerSelectionEnum.Manual;
        NewsletterTasksManager.EnsureWinnerSelectionTask(abTest, parentIssue, false, DateTime.Now);

        abTest.TestSelectWinnerAfter = 0;
        abTest.TestWinnerSelected = DateTime.Now;
        abTest.TestWinnerIssueID = winnerVariant.IssueID;
        ABTestInfo.Provider.Set(abTest);
    }


    private void CopyDataFromWinnerToParent(IssueInfo winner, IssueInfo parent)
    {
        IssueHelper.CopyWinningVariantIssueProperties(winner, parent);
        IssueInfo.Provider.Set(parent);
    }


    private void ScheduleSendingOfIssue(IssueInfo issue)
    {
        // Remove previous scheduled task of this issue
        NewsletterTasksManager.DeleteMailoutTask(issue.IssueGUID, issue.IssueSiteID);

        DateTime mailoutTime = dtpMailout.SelectedDateTime;
        Service.Resolve<IIssueScheduler>().ScheduleIssue(issue, mailoutTime);
    }


    private void CloseDialogAndRefreshParentPage()
    {
        ScriptHelper.RegisterStartupScript(this, GetType(), "ClosePage", "RefreshPage(); setTimeout('CloseDialog()',200);", true);
    }    
}
