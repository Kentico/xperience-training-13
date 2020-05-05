using System;

using CMS.Newsletters;
using CMS.Helpers;
using CMS.Newsletters.Web.UI.Controls;

public partial class CMSModules_Newsletters_Controls_SendIssueTemplateBased : IssueSenderControl
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        IssueInfo issue = IssueInfo.Provider.Get(IssueID);

        // If issue is already scheduled or sent, show the mail-out time
        if (issue.IssueMailoutTime != DateTimeHelper.ZERO_TIME)
        {
            calendarControl.SelectedDateTime = issue.IssueMailoutTime;
        }
        
        // If issue is being send or was already sent, disable the date time picker
        if (issue.IssueStatus == IssueStatusEnum.Finished || 
            issue.IssueStatus == IssueStatusEnum.PreparingData || 
            issue.IssueStatus == IssueStatusEnum.Sending ||
            issue.HasWidgetWithUnfilledRequiredProperty())
        {
            calendarControl.Enabled = false;
        }
    }


    /// <summary>
    /// Schedules mail-out of the issue to the future.
    /// </summary>
    public bool SendScheduled()
    {
        return SendScheduled(calendarControl.SelectedDateTime);
    }
}