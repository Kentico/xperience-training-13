using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using System.Text;
using System.Threading;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CONTACTMANAGEMENT, "Accounts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Delete : CMSContactManagementPage
{
    #region "Private variables"

    private IList<string> accountIds;
    private Hashtable mParameters;
    private string mReturnScript ;
    private int numberOfDeletedAccounts;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }


    /// <summary>
    /// Where condition used for multiple actions.
    /// </summary>
    private string WhereCondition
    {
        get
        {
            string where = string.Empty;
            if (Parameters != null)
            {
                where = ValidationHelper.GetString(Parameters["where"], string.Empty);
            }
            return where;
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }


    /// <summary>
    /// Returns script for returning back to list page.
    /// </summary>
    private string ReturnScript
    {
        get
        {
            if (string.IsNullOrEmpty(mReturnScript) && (Parameters != null))
            {
                mReturnScript = "document.location.href = 'List.aspx';";
            }

            return mReturnScript;
        }
    }


    /// <summary>
    /// Returns script for returning back to list page with information that deleting process has been started.
    /// </summary>
    private string ReturnScriptDeleteAsync
    {
        get
        {
            if (string.IsNullOrEmpty(mReturnScript) && (Parameters != null))
            {
                mReturnScript = "document.location.href = 'List.aspx?deleteasync=1';";
            }

            return mReturnScript;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check hash validity
        if (QueryHelper.ValidateHash("hash"))
        {
            // Initialize events
            ctlAsyncLog.OnFinished += ctlAsync_OnFinished;
            ctlAsyncLog.OnError += ctlAsync_OnError;
            ctlAsyncLog.OnCancel += ctlAsync_OnCancel;

            ctlAsyncLog.MaxLogLines = 1000;

            if (!RequestHelper.IsCallback())
            {
                // Setup page title text and image
                PageTitle.TitleText = GetString("om.account.deletetitle");

                ctlAsyncLog.TitleText = GetString("om.account.deleting");
                // Set visibility of panels
                pnlContent.Visible = true;
                pnlLog.Visible = false;

                // Get names of the accounts that are to be deleted
                DataSet ds = AccountInfo.Provider.Get()
                                                .Where(WhereCondition)
                                                .OrderBy("AccountName")
                                                .TopN(1000)
                                                .Columns("AccountID", "AccountName");

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    DataRowCollection rows = ds.Tables[0].Rows;

                    // Data set contains only one item
                    if (rows.Count == 1)
                    {
                        PageTitle.TitleText += " \"" + HTMLHelper.HTMLEncode(DataHelper.GetStringValue(rows[0], "AccountName", "N/A")) + "\"";
                        accountIds = new List<string>(1);
                        accountIds.Add(DataHelper.GetStringValue(rows[0], "AccountID"));
                        numberOfDeletedAccounts = 1;
                    }
                    else if (rows.Count > 1)
                    {
                        // Modify title and question for multiple items
                        PageTitle.TitleText = GetString("om.account.deletetitlemultiple");
                        headQuestion.ResourceString = "om.account.deletemultiplequestion";

                        // Display list with names of deleted items
                        pnlAccountList.Visible = true;

                        StringBuilder builder = new StringBuilder();

                        for (int i = 0; i < rows.Count; i++)
                        {
                            string name = DataHelper.GetStringValue(rows[i], "AccountName");

                            builder.Append("<div>");
                            builder.Append(HTMLHelper.HTMLEncode(name));
                            builder.Append("</div>");
                        }

                        // Display three dots after last record
                        if (rows.Count == 1000)
                        {
                            builder.Append("...");
                        }

                        lblAccounts.Text = builder.ToString();

                        // Get all IDs of deleted items
                        ds = AccountInfo.Provider.Get()
                                                .Where(WhereCondition)
                                                .OrderBy("AccountID")
                                                .Column("AccountID");

                        accountIds = DataHelper.GetStringValues(ds.Tables[0], "AccountID");
                        numberOfDeletedAccounts = ds.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    // Hide everything
                    pnlContent.Visible = false;
                }
            }
        }
        else
        {
            pnlDelete.Visible = false;
            ShowError(GetString("dialogs.badhashtext"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        btnNo.OnClientClick = ReturnScript + "return false;";

        base.OnPreRender(e);
    }

    #endregion


    #region "Button actions"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check permissions
        if (AuthorizationHelper.AuthorizedModifyContact(true))
        {
            EnsureAsyncLog();
            RunAsyncDelete();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Ensures log for asynchronous control
    /// </summary>
    private void EnsureAsyncLog()
    {
        pnlLog.Visible = true;
        pnlContent.Visible = false;

        CurrentError = string.Empty;
    }


    /// <summary>
    /// Starts asynchronous deleting of contacts.
    /// </summary>
    private void RunAsyncDelete()
    {
        // Run the async method
        ctlAsyncLog.EnsureLog();
        ctlAsyncLog.Parameter = ReturnScript;
        ctlAsyncLog.RunAsync(Delete, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Deletes document(s).
    /// </summary>
    private void Delete(object parameter)
    {
        if (parameter == null || accountIds.Count < 1)
        {
            return;
        }

        try
        {
            // Begin log
            AddLog(GetString("om.account.deleting"));
            AddLog(string.Empty);

            // When deleting children and not removing relations then we can run
            if (numberOfDeletedAccounts > 1)
            {
                BatchDeleteOnSql();
            }
            else
            {
                DeleteItems();
            }
        }
        catch (ThreadAbortException ex)
        {
            if (CMSThread.Stopped(ex))
            {
                // When canceled
                AddError(GetString("om.deletioncanceled"));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    /// <summary>
    /// Delete accounts on SQL server.
    /// </summary>
    private void BatchDeleteOnSql()
    {
        StringBuilder where = new StringBuilder("AccountID IN (");

        // Create where condition for deleting accounts
        int i;
        for (i = 0; i < accountIds.Count; i++)
        {
            where.Append(accountIds[i] + ",");

            // Delete accounts by 100's
            if ((i + 1) % 100 == 0)
            {
                BatchDeleteItems(where);
                where = new StringBuilder("AccountID IN (");
            }
        }

        // Delete rest of the accounts
        if (i % 100 != 0)
        {
            BatchDeleteItems(where);
        }

        // Return to the list page with info label displayed
        ltlScript.Text += ScriptHelper.GetScript(ReturnScriptDeleteAsync);
    }


    /// <summary>
    /// Deletes group of accounts on SQL server.
    /// </summary>
    /// <param name="where">WHERE specifying group of accounts</param>
    private void BatchDeleteItems(StringBuilder where)
    {
        where.Remove(where.Length - 1, 1);
        where.Append(")");
        AccountInfoProvider.DeleteAccountInfos(where.ToString(), chkBranches.Checked);
    }


    /// <summary>
    /// Delete items.
    /// </summary>
    private void DeleteItems()
    {
        // Delete the accounts
        foreach (string accountId in accountIds)
        {
            var ai = AccountInfo.Provider.Get(ValidationHelper.GetInteger(accountId, 0));
            if (ai != null)
            {
                // Display name of deleted account
                AddLog(ai.AccountName);

                // Delete account with its dependencies
                AccountHelper.Delete(ai, chkBranches.Checked);
            }
        }
    }

    #endregion


    #region "Async methods"

    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        ctlAsyncLog.Parameter = null;
        string canceled = GetString("om.deletioncanceled");
        AddLog(canceled);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array();RefreshCurrent();");
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        ShowConfirmation(canceled);
    }


    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (ctlAsyncLog.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsyncLog.Stop();
        }
        ctlAsyncLog.Parameter = null;
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(CurrentError))
        {
            ctlAsyncLog.Parameter = null;
            ShowError(CurrentError);
        }

        if (ctlAsyncLog.Parameter != null)
        {
            // Return to the list page after successful deletion
            ltlScript.Text += ScriptHelper.GetScript(ctlAsyncLog.Parameter);
        }
    }
    

    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = ("<div>" + error + "</div><div>" + CurrentError + "</div>");
    }


    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        Service.Resolve<IEventLogService>().LogException("Contact management", "DELETEACCOUNT", ex);
        AddError(GetString("om.account.deletefailed") + ": " + ex.Message);
    }

    #endregion
}
