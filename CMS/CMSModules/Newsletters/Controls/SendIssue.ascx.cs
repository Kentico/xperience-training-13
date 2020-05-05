using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI.Controls;

public partial class CMSModules_Newsletters_Controls_SendIssue : IssueSenderControl
{
    #region "Properties"


    /// <summary>
    /// Newsletter ID, required for dynamic newsletters.
    /// </summary>
    public int NewsletterID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Schedule newsletter mail-out' option should be displayed.
    /// </summary>
    public bool ShowScheduler
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Send draft' option should be displayed.
    /// </summary>
    public bool ShowSendDraft
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Send later' option should be displayed.
    /// </summary>
    public bool ShowSendLater
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || ((IssueID <= 0) && (NewsletterID <= 0)))
        {
            return;
        }

        if (!RequestHelper.IsPostBack())
        {
            int newsletterId = NewsletterID;
            if (newsletterId <= 0)
            {
                IssueInfo issue = IssueInfo.Provider.Get(IssueID);
                if (issue != null)
                {
                    newsletterId = issue.IssueNewsletterID;
                }
                else
                {
                    StopProcessing = true;
                    return;
                }
            }

            // Fill draft emails box
            NewsletterInfo newsletter = NewsletterInfo.Provider.Get(newsletterId);
            if (newsletter != null)
            {
                txtSendDraft.Text = newsletter.NewsletterDraftEmails;
            }
            else
            {
                StopProcessing = true;
            }
        }

        calendarControl.DateTimeTextBox.AddCssClass("EditingFormCalendarTextBox");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Show/hide specific send options
        radSchedule.Visible = plcSendScheduled.Visible = ShowScheduler;
        radSendDraft.Visible = plcSendDraft.Visible = ShowSendDraft;
        radSendLater.Visible = ShowSendLater;
    }


    protected void radGroupSend_CheckedChanged(object sender, EventArgs e)
    {
        calendarControl.Enabled = radSchedule.Checked;
        txtSendDraft.Enabled = radSendDraft.Checked;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Sends the issue according to sending options.
    /// </summary>
    public bool SendIssue()
    {
        bool result = radSendLater.Checked;

        if (IssueID > 0)
        {
            // Depending on action chosen, send the issue
            if (radSendNow.Checked)
            {
                result = SendNow();
            }
            else if (radSchedule.Checked)
            {
                result = SendScheduled(calendarControl.SelectedDateTime);
            }
            else if (radSendDraft.Checked)
            {
                result = SendDraft();
            }
        }

        return result;
    }


    /// <summary>
    /// Sends issue to testing email addresses.
    /// </summary>
    /// <returns>True when everything went ok</returns>
    private bool SendDraft()
    {
        string draftEmails = txtSendDraft.Text.Trim();

        if (String.IsNullOrEmpty(draftEmails))
        {
            ErrorMessage = GetString("newsletter.recipientsmissing");
        }
        else if (!ValidationHelper.AreEmails(draftEmails))
        {
            ErrorMessage = GetString("newsletter.wrongemailformat");
        }
        else
        {
            Service.Resolve<IDraftSender>().SendAsync(IssueInfo.Provider.Get(IssueID), draftEmails);
        }

        return String.IsNullOrEmpty(ErrorMessage);
    }
    
    #endregion
}