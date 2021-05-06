using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Core;

[UIElement(ModuleName.CMS, "E-mailQueue")]
public partial class CMSModules_EmailQueue_EmailQueue : EmailQueuePage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = GetString("emailqueue.queue.title");

        // Load drop-down lists
        if (!RequestHelper.IsPostBack())
        {
            pnlBodyFilter.Visible = UserIsAdmin;
            InitializeFilterDropdowns();

            btnShowFilter.Text = icShowFilter.AlternativeText = GetString("emailqueue.displayfilter");
        }

        gridEmailQueue.EmailGrid.WhereCondition = GetWhereCondition();

        if (EmailHelper.Queue.SendingInProgess)
        {
            ShowInformation(GetString("emailqueue.sendingemails"));
        }

        // Initialize top menu
        InitializeActionMenu();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes action menu in master page.
    /// </summary>
    protected void InitializeActionMenu()
    {
        var enabled = !EmailHelper.Queue.SendingInProgess && UserHasModify;
        var canStopSending = EmailHelper.Queue.SendingInProgess && UserHasModify;

        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionsList.Clear();

        string confirmScript = "if (!confirm({0})) return false;";

        // Resend all failed
        HeaderAction resendAction = new HeaderAction
        {
            Text = GetString("emailqueue.queue.resendfailed"),
            OnClientClick = enabled ? string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.ResendAllFailedConfirmation"))) : null,
            CommandName = "resendallfailed",
            Enabled = enabled
        };
        actions.ActionsList.Add(resendAction);

        // Resend selected
        resendAction.AlternativeActions.Add(new HeaderAction
        {
            Text = GetString("emailqueue.queue.resendselected"),
            OnClientClick = enabled ? string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.ResendSelectedConfirmation"))) : null,
            CommandName = "resendselected",
            Enabled = enabled
        });

        // Resend all
        resendAction.AlternativeActions.Add(new HeaderAction
        {
            Text = GetString("emailqueue.queue.resend"),
            OnClientClick = enabled ? string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.ResendAllConfirmation"))) : null,
            CommandName = "resendall",
            Enabled = enabled
        });

        // Delete all failed
        HeaderAction deleteAction = new HeaderAction
        {
            Text = GetString("emailqueue.queue.deletefailed"),
            OnClientClick = enabled ? string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.DeleteAllFailedConfirmation"))) : null,
            CommandName = "deleteallfailed",
            Enabled = enabled
        };
        actions.ActionsList.Add(deleteAction);

        // Delete selected
        deleteAction.AlternativeActions.Add(new HeaderAction
        {
            Text = GetString("emailqueue.queue.deleteselected"),
            OnClientClick = enabled ? string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.DeleteSelectedConfirmation"))) : null,
            CommandName = "deleteselected",
            Enabled = enabled
        });

        // Delete all
        deleteAction.AlternativeActions.Add(new HeaderAction
        {
            Text = GetString("emailqueue.queue.delete"),
            OnClientClick = enabled ? string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.DeleteAllConfirmation"))) : null,
            CommandName = "deleteall",
            Enabled = enabled
        });

        // Stop send
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("emailqueue.queue.stop"),
            OnClientClick = canStopSending ? string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.StopConfirmation"))) : null,
            CommandName = "stop",
            Enabled = canStopSending
        });

        // Refresh
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.refresh"),
            CommandName = "refresh"
        });

        actions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    private void InitializeFilterDropdowns()
    {
        drpPriority.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        drpPriority.Items.Add(new ListItem(GetString("emailpriority.low"), EmailPriorityEnum.Low.ToString("D")));
        drpPriority.Items.Add(new ListItem(GetString("emailpriority.normal"), EmailPriorityEnum.Normal.ToString("D")));
        drpPriority.Items.Add(new ListItem(GetString("emailpriority.high"), EmailPriorityEnum.High.ToString("D")));

        drpStatus.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        drpStatus.Items.Add(new ListItem(GetString("emailstatus.created"), EmailStatusEnum.Created.ToString("D")));
        drpStatus.Items.Add(new ListItem(GetString("emailstatus.sending"), EmailStatusEnum.Sending.ToString("D")));
        drpStatus.Items.Add(new ListItem(GetString("emailstatus.waiting"), EmailStatusEnum.Waiting.ToString("D")));
    }

    #endregion


    #region "Button events"

    /// <summary>
    /// Displays/hides filter.
    /// </summary>
    protected void btnShowFilter_Click(object sender, EventArgs e)
    {
        // Hide filter
        if (plcFilter.Visible)
        {
            plcFilter.Visible = false;
            btnShowFilter.Text = icShowFilter.AlternativeText = GetString("emailqueue.displayfilter");
            icShowFilter.CssClass = "icon-caret-down cms-icon-30";
        }
        // Display filter
        else
        {
            plcFilter.Visible = true;
            btnShowFilter.Text = icShowFilter.AlternativeText = GetString("emailqueue.hidefilter");
            icShowFilter.CssClass = "icon-caret-up cms-icon-30";
        }
    }


    /// <summary>
    /// Filter button clicked.
    /// </summary>
    protected void btnFilter_Clicked(object sender, EventArgs e)
    {
        gridEmailQueue.EmailGrid.WhereCondition = GetWhereCondition();
    }

    #endregion


    #region "Header action event"

    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        var commandName = e.CommandName.ToLowerInvariant();
        var isRefreshAction = commandName.Equals("refresh", StringComparison.OrdinalIgnoreCase);

        if ((isRefreshAction && !UserHasRead) || (!isRefreshAction && !UserHasModify))
        {
            RedirectToAccessDenied(ModuleName.EMAILENGINE, isRefreshAction ? READ_PERMISSION : MODIFY_PERMISSION);
        }

        switch (commandName)
        {
            case "resendallfailed":
                if (SiteId == UniSelector.US_GLOBAL_RECORD)
                {
                    EmailHelper.Queue.SendAllFailed();
                }
                else
                {
                    EmailHelper.Queue.SendAllFailed(SiteId);
                }
                ShowInformation(GetString("emailqueue.sendingemails"));
                break;

            case "resendselected":
                EmailHelper.Queue.Send(gridEmailQueue.GetSelectedEmailIDs());
                gridEmailQueue.EmailGrid.ResetSelection();
                ShowInformation(GetString("emailqueue.sendingemails"));

                break;

            case "resendall":
                if (SiteId == UniSelector.US_GLOBAL_RECORD)
                {
                    EmailHelper.Queue.SendAll();
                }
                else
                {
                    EmailHelper.Queue.SendAll(SiteId);
                }
                ShowInformation(GetString("emailqueue.sendingemails"));

                break;

            case "deleteallfailed":
                EmailHelper.Queue.DeleteAllFailed(SiteId);
                break;

            case "deleteselected":
                EmailHelper.Queue.Delete(gridEmailQueue.GetSelectedEmailIDs());
                gridEmailQueue.EmailGrid.ResetSelection();
                break;

            case "deleteall":
                EmailHelper.Queue.DeleteAll(SiteId);
                break;
            case "stop":
                EmailHelper.Queue.CancelSending();
                break;
        }

        gridEmailQueue.ReloadData();

        // Change to the first page if no data has been found after performing action
        if (!isRefreshAction && DataHelper.DataSourceIsEmpty(gridEmailQueue.EmailGrid.GridView.DataSource))
        {
            gridEmailQueue.EmailGrid.Pager.UniPager.CurrentPage = 1;
            gridEmailQueue.ReloadData();
        }
    }

    #endregion


    #region "Filter methods"

    /// <summary>
    /// Returns WHERE condition.
    /// </summary>
    protected string GetWhereCondition()
    {
        string where = string.Empty;

        if (UserIsAdmin)
        {
            where = SqlHelper.AddWhereCondition(where, fltBody.GetCondition());
        }

        where = SqlHelper.AddWhereCondition(where, fltFrom.GetCondition());
        where = SqlHelper.AddWhereCondition(where, fltSubject.GetCondition());
        where = SqlHelper.AddWhereCondition(where, fltLastResult.GetCondition());

        // EmailTo condition
        string emailTo = fltTo.FilterText.Trim();
        if (!String.IsNullOrEmpty(emailTo))
        {
            if (!String.IsNullOrEmpty(where))
            {
                where += " AND ";
            }
            string toText = SqlHelper.EscapeQuotes(emailTo);
            string op = fltTo.FilterOperator;
            if (op.Contains(WhereBuilder.LIKE))
            {
                toText = "%" + SqlHelper.EscapeLikeText(toText) + "%";
            }
            toText = " N'" + toText + "'";
            string combineOp = " OR ";
            bool includeNullCondition = false;
            if ((op == "<>") || op.Contains("NOT"))
            {
                combineOp = " AND ";
                includeNullCondition = true;
            }
            where += string.Format("(EmailTo {0}{1}{2}(EmailCc {0}{1}{3}){2}(EmailBcc {0}{1}{4}))",
                                   op, toText, combineOp, includeNullCondition ? " OR EmailCc IS NULL" : string.Empty, includeNullCondition ? " OR EmailBcc IS NULL" : string.Empty);
        }

        // Condition for priority
        int priority = ValidationHelper.GetInteger(drpPriority.SelectedValue, -1);
        if (priority >= 0)
        {
            if (!String.IsNullOrEmpty(where))
            {
                where += " AND ";
            }
            where += "EmailPriority=" + priority;
        }

        // Condition for e-mail status
        int status = ValidationHelper.GetInteger(drpStatus.SelectedValue, -1);
        if (status >= 0)
        {
            if (!string.IsNullOrEmpty(where))
            {
                where += " AND ";
            }

            where += "EmailStatus=" + drpStatus.SelectedValue;
        }

        // Condition for site
        if (!string.IsNullOrEmpty(where))
        {
            where += " AND ";
        }
        where += string.Format("(NOT EmailStatus = {0:D})", EmailStatusEnum.Archived);

        if (SiteId == UniSelector.US_GLOBAL_RECORD)
        {
            // Global
            where += " AND (EmailSiteID IS NULL OR  EmailSiteID = 0)";
        }
        else if (SiteId > 0)
        {
            where += string.Format(" AND (EmailSiteID = {0})", SiteId);
        }

        return where;
    }

    #endregion
}
