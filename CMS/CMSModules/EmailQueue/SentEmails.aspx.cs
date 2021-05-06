using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.Core;

[UIElement(ModuleName.CMS, "SentE-mails")]
public partial class CMSModules_EmailQueue_SentEmails : EmailQueuePage, ICallbackEventHandler
{
    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        mParameters = new Hashtable();
        mParameters["where"] = gridElem.WhereCondition;
        mParameters["orderby"] = gridElem.SortDirect;

        // Get the dialog identifier
        Guid dialogIdentifier = GetDialogIdentifier();

        // Store the dialog identifier with appropriate data in the session
        WindowHelper.Add(dialogIdentifier.ToString(), mParameters);

        string queryString = "?params=" + dialogIdentifier;

        queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        queryString = URLHelper.AddParameterToUrl(queryString, "emailid", EmailID.ToString());

        return queryString;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        EmailID = ValidationHelper.GetInteger(eventArgument, 0);
    }

    #endregion


    #region "Variables"

    private Hashtable mParameters;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the email id.
    /// </summary>
    private int EmailID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        Title = GetString("emailqueue.archive.title");

        // Load drop-down lists
        if (!RequestHelper.IsPostBack())
        {
            pnlBodyFilter.Visible = UserIsAdmin;
            if (drpPriority.Items.Count <= 0)
            {
                drpPriority.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
                drpPriority.Items.Add(new ListItem(GetString("emailpriority.low"), EmailPriorityEnum.Low.ToString("D")));
                drpPriority.Items.Add(new ListItem(GetString("emailpriority.normal"), EmailPriorityEnum.Normal.ToString("D")));
                drpPriority.Items.Add(new ListItem(GetString("emailpriority.high"), EmailPriorityEnum.High.ToString("D")));
            }

            btnShowFilter.Text = icShowFilter.AlternativeText = GetString("emailqueue.displayfilter");
        }

        gridElem.WhereCondition = GetWhereCondition();
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);

        // Register script for modal dialog with mass email recipients
        // and for opening modal dialog displaying email detail
        string script = string.Format(
@"var emailDialogParams_{0} = '';

function DisplayRecipients(emailId) {{
    if ( emailId != 0 ) {{
        modalDialog({1} + '?emailid=' + emailId + '&archive=1', 'emailrecipients', 920, 700);
    }}
}}

function OpenEmailDetail(queryParameters) {{
    modalDialog({2} + queryParameters, 'emaildetails', 1000, 730);
}}", ClientID, ScriptHelper.GetString(ResolveUrl("~/CMSModules/EmailQueue/MassEmails_Recipients.aspx")), ScriptHelper.GetString(ResolveUrl("~/CMSModules/EmailQueue/EmailQueue_Details.aspx")));

        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Email_Dialogs", script, true);

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }

    #endregion


    #region "Button events"

    /// <summary>
    /// Show/Hide filter button click.
    /// </summary>
    protected void btnShowFilter_Click(object sender, EventArgs e)
    {
        // If filter is displayed then hide it
        if (plcFilter.Visible)
        {
            plcFilter.Visible = false;
            btnShowFilter.Text = icShowFilter.AlternativeText = GetString("emailqueue.displayfilter");
            icShowFilter.CssClass = "icon-caret-down cms-icon-30";
        }
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
        gridElem.WhereCondition = GetWhereCondition();
    }

    #endregion


    #region "Unigrid events"

    /// <summary>
    /// Handles Unigrid's OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "subject":
                return HTMLHelper.HTMLEncode(parameter.ToString());

            case "priority":
                return GetEmailPriority(parameter);

            case "emailto":
                return GetEmailRecipients(parameter);

            case "edit":
                CMSGridActionButton viewBtn = (CMSGridActionButton)sender;
                viewBtn.OnClientClick = string.Format("emailDialogParams_{0} = '{1}';{2};return false;",
                    ClientID,
                    viewBtn.CommandArgument,
                    Page.ClientScript.GetCallbackEventReference(this, "emailDialogParams_" + ClientID, "OpenEmailDetail", null));
                break;
            case "delete":
                CMSGridActionButton deleteBtn = (CMSGridActionButton)sender;
                deleteBtn.Enabled = UserHasModify;
                break;
            case "resend":
                CMSGridActionButton resendBtn = (CMSGridActionButton)sender;
                resendBtn.Enabled = UserHasModify;
                break;
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (!UserHasModify)
        {
            RedirectToAccessDenied(ModuleName.EMAILENGINE, MODIFY_PERMISSION);
        }

        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                int deleteEmailId = ValidationHelper.GetInteger(actionArgument, 0);
                EmailHelper.Queue.Delete(deleteEmailId);
                break;

            case "resend":
                int sendEmailId = ValidationHelper.GetInteger(actionArgument, -1);
                EmailHelper.Queue.Send(sendEmailId);
                ShowInformation(GetString("emailqueue.sendingemails"));
                break;
        }
    }


    /// <summary>
    /// Gets the email priority.
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>The e-mail priority</returns>
    private object GetEmailPriority(object parameter)
    {
        switch ((EmailPriorityEnum)parameter)
        {
            case EmailPriorityEnum.Low:
                return GetString("emailpriority.low");

            case EmailPriorityEnum.Normal:
                return GetString("emailpriority.normal");

            case EmailPriorityEnum.High:
                return GetString("emailpriority.high");

            default:
                return string.Empty;
        }
    }


    /// <summary>
    /// Gets the e-mail recipients.
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>The e-mail recipients</returns>
    private object GetEmailRecipients(object parameter)
    {
        DataRowView dr = (DataRowView)parameter;
        if (ValidationHelper.GetBoolean(dr["EmailIsMass"], false))
        {
            return string.Format("<a href=\"#\" onclick=\"javascript: DisplayRecipients({0}); return false; \">{1}</a>",
                                 ValidationHelper.GetInteger(dr["EmailID"], 0),
                                 GetString("emailqueue.queue.massdetails"));
        }
        else
        {
            return HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["EmailTo"], string.Empty));
        }
    }

    #endregion


    #region "Header actions events"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        var commandName = e.CommandName.ToLowerInvariant();
        var isRefreshAction = commandName.Equals("refresh", StringComparison.OrdinalIgnoreCase);

        if ((isRefreshAction && !UserHasRead) || (!isRefreshAction && !UserHasModify))
        {
            RedirectToAccessDenied(ModuleName.EMAILENGINE, isRefreshAction ? READ_PERMISSION : MODIFY_PERMISSION);
        }

        if (isRefreshAction)
        {
            gridElem.ReloadData();
        }

        switch (commandName)
        {
            case "deleteall":
                // Delete all archived e-mails
                EmailHelper.Queue.DeleteArchived(SiteId);

                gridElem.ReloadData();
                break;

            case "deleteselected":
                // Delete selected e-mails
                foreach (string emailId in gridElem.SelectedItems)
                {
                    EmailHelper.Queue.Delete(ValidationHelper.GetInteger(emailId, 0));
                }

                gridElem.ResetSelection();
                gridElem.ReloadData();
                break;
        }

        // Reload on first page if no data found after perfoming action
        if (!isRefreshAction && DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource))
        {
            gridElem.Pager.UniPager.CurrentPage = 1;
            gridElem.ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string confirmScript = "if (!confirm({0})) return false;";
        bool enabled = !DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource) && UserHasModify;

        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionsList.Clear();

        // Delete all
        HeaderAction deleteAction = new HeaderAction
        {
            Text = GetString("emailqueue.queue.delete"),
            OnClientClick = string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.DeleteAllConfirmation"))),
            CommandName = "deleteall",
            Enabled = enabled
        };
        actions.ActionsList.Add(deleteAction);

        // Delete selected
        deleteAction.AlternativeActions.Add(new HeaderAction
        {
            Text = GetString("emailqueue.queue.deleteselected"),
            OnClientClick = string.Format(confirmScript, ScriptHelper.GetString(GetString("EmailQueue.DeleteSelectedConfirmation"))),
            CommandName = "deleteselected",
            Enabled = enabled
        });

        // Refresh
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.refresh"),
            CommandName = "refresh"
        });
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

        // EmailTo condition
        string emailTo = fltTo.FilterText.Trim();
        if (!string.IsNullOrEmpty(emailTo))
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
            if (!string.IsNullOrEmpty(where))
            {
                where += " AND ";
            }
            where += "EmailPriority=" + drpPriority.SelectedValue;
        }

        // Condition for site (and sent/archived e-mails)
        if (!string.IsNullOrEmpty(where))
        {
            where += " AND ";
        }

        where += string.Format("(EmailStatus = {0:D})", EmailStatusEnum.Archived);

        if (SiteId == UniSelector.US_GLOBAL_RECORD)
        {
            where += " AND (EmailSiteID IS NULL OR  EmailSiteID = 0)";
        }
        else if (SiteId > 0)
        {
            where += string.Format(" AND (EmailSiteID = {0})", SiteId);
        }

        return where;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets the dialog identifier used for sharing data between windows.
    /// </summary>
    /// <returns>Dialog identifier</returns>
    private Guid GetDialogIdentifier()
    {
        Guid identifier;

        // Try parse the identifier as a Guid value
        if (!Guid.TryParse(hdnIdentifier.Value, out identifier))
        {
            // If the identifier value is not a valid Guid value, generates a new Guid
            identifier = Guid.NewGuid();
            hdnIdentifier.Value = identifier.ToString();
        }

        return identifier;
    }

    #endregion
}
