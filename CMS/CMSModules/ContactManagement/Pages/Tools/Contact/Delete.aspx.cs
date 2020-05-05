using System;
using System.Collections;
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


[UIElement(ModuleName.CONTACTMANAGEMENT, "Contacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Delete : CMSContactManagementPage
{
    #region "Private variables"

    private Hashtable mParameters;
    private string mReturnScript;
    private int mSiteID;
    private DataSet ds;

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
                mReturnScript = "document.location.href = 'List.aspx?siteid=" + SiteID + "';";
            }

            return mReturnScript;
        }
    }


    /// <summary>
    /// Returns script for returning back to list page.
    /// </summary>
    private string ReturnScriptDeleteAsync
    {
        get
        {
            if (string.IsNullOrEmpty(mReturnScript) && (Parameters != null))
            {
                mReturnScript = "document.location.href = 'List.aspx?siteid=" + SiteID + "&deleteasync=1';";
            }

            return mReturnScript;
        }
    }


    /// <summary>
    /// Site ID retrieved from dialog parameters.
    /// </summary>
    public override int SiteID
    {
        get
        {
            if ((mSiteID == 0) && (Parameters != null))
            {
                mSiteID = ValidationHelper.GetInteger(Parameters["siteid"], 0);
            }
            return mSiteID;
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
            ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
            ctlAsyncLog.OnError += ctlAsyncLog_OnError;
            ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

            ctlAsyncLog.MaxLogLines = 1000;

            if (!RequestHelper.IsCallback())
            {
                // Setup page title text and image
                PageTitle.TitleText = GetString("om.contact.deletetitle");
                ctlAsyncLog.TitleText = GetString("om.contact.deleting");
                // Set visibility of panels
                pnlContent.Visible = true;
                pnlLog.Visible = false;

                // Get names of deleted contacts
                ds = ContactInfo.Provider.Get()
                                        .TopN(500)
                                        .Where(WhereCondition)
                                        .OrderBy("ContactLastName");

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    DataRowCollection rows = ds.Tables[0].Rows;

                    // Data set contains only one item...
                    if (rows.Count == 1)
                    {
                        // Get full contact name and use it in the title
                        string fullName = GetFullName(rows[0]);
                        if (!string.IsNullOrEmpty(fullName))
                        {
                            PageTitle.TitleText += " \"" + HTMLHelper.HTMLEncode(fullName) + "\"";
                        }
                    }
                    else if (rows.Count > 1)
                    {
                        // Modify title and question for multiple items
                        PageTitle.TitleText = GetString("om.contact.deletetitlemultiple");
                        headQuestion.ResourceString = "om.contact.deletemultiplequestion";
                        // Display list with names of deleted items
                        pnlContactList.Visible = true;
                        
                        StringBuilder builder = new StringBuilder();

                        // Display top 500 records
                        for (int i = 0; i < (rows.Count); i++)
                        {
                            builder.Append("<div>");
                            var name = GetFullName(rows[i]);
                            if (!string.IsNullOrEmpty(name))
                            {
                                builder.Append(HTMLHelper.HTMLEncode(name));
                            }
                            else
                            {
                                builder.Append("N/A");
                            }
                            builder.Append("</div>");
                        }
                        // Display three dots after last record
                        if (rows.Count >= 500)
                        {
                            builder.Append("...");
                        }

                        lblContacts.Text = builder.ToString();
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
        AuthorizationHelper.AuthorizedModifyContact(true);

        EnsureAsyncLog();
        RunAsyncDelete();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns contact name in the form 'lastname firstname' or null.
    /// </summary>
    /// <param name="row">Data row with contact info</param>
    protected string GetFullName(DataRow row)
    {
        string fullName = null;

        if (row != null)
        {
            // Compose full contact name
            fullName = string.Format("{0} {1}", DataHelper.GetStringValue(row, "ContactLastName"),
                                     DataHelper.GetStringValue(row, "ContactFirstName")).Trim();
        }

        return fullName;
    }


    /// <summary>
    /// Delete contacts on SQL server.
    /// </summary>
    private void BatchDeleteOnSql()
    {
        while (!DataHelper.DataSourceIsEmpty(ds))
        {
            ContactInfoProvider.DeleteContactInfos(WhereCondition, 200);
            ds = ContactInfo.Provider.Get()
                                    .TopN(1)
                                    .Column("ContactID")
                                    .Where(WhereCondition);
        }

        // Return to the list page with info label displayed
        ltlScript.Text += ScriptHelper.GetScript(ReturnScriptDeleteAsync);
    }


    /// <summary>
    /// Delete items one by one.
    /// </summary>
    private void DeleteItems()
    {
        while (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Delete the contacts
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var ci = new ContactInfo(dr);
                AddLog(HTMLHelper.HTMLEncode(ci.ContactLastName + " " + ci.ContactFirstName).Trim());
                ContactInfo.Provider.Delete(ci);
            }

            ds = ContactInfo.Provider.Get()
                    .TopN(500)
                    .Where(WhereCondition)
                    .OrderBy("ContactLastName");
        }
    }


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

    #endregion


    #region "Async methods"

    /// <summary>
    /// Deletes document(s).
    /// </summary>
    private void Delete(object parameter)
    {
        if ((parameter == null) || (ds.Tables[0].Rows.Count < 1))
        {
            return;
        }

        try
        {
            // Begin log
            AddLog(GetString("om.contact.deleting"));
            AddLog(string.Empty);

            // Mass delete without logging items
            if (ds.Tables[0].Rows.Count > 1)
            {
                BatchDeleteOnSql();
            }
            // Delete items
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


    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
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


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
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


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
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
        Service.Resolve<IEventLogService>().LogException("Contact management", "DELETECONTACT", ex);
        AddError(GetString("om.contact.deletefailed") + ": " + ex.Message);
    }

    #endregion
}
