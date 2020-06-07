using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

using CMS.Activities;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.ONLINEMARKETING, "Activities", false, true)]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_Activities_Pages_Tools_Activities_Activity_Delete : CMSContactManagementPage
{
    #region "Variables"

    private Hashtable mParameters;
    private string mReturnScript;
    private int mSiteID;
    private int mContactID;

    #endregion


    #region "Properties"

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
    /// Returns script for returning back to list page.
    /// </summary>
    private string ReturnScript
    {
        get
        {
            if (string.IsNullOrEmpty(mReturnScript) && (Parameters != null))
            {
                mReturnScript = ValidationHelper.GetString(Parameters["returnlocation"], null);
                if (String.IsNullOrEmpty(mReturnScript))
                {
                    mReturnScript = "document.location.href = 'List.aspx?siteid=" + SiteID + "';";
                }
                else
                {
                    mReturnScript = "document.location.href = '" + mReturnScript + "';";
                }
            }

            return mReturnScript;
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


    /// <summary>
    /// Contact ID retrieved from dialog parameters.
    /// </summary>
    public int ContactID
    {
        get
        {
            if ((mContactID == 0) && (Parameters != null))
            {
                mContactID = ValidationHelper.GetInteger(Parameters["contactid"], 0);
            }
            return mContactID;
        }
    }

    #endregion


    #region "Page methods"

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

            pnlContent.Visible = true;
            pnlLog.Visible = false;

            if (!RequestHelper.IsCallback())
            {
                // Setup page title text and image
                PageTitle.TitleText = GetString("om.activity.deletetitle");
                ctlAsyncLog.TitleText = GetString("om.activity.deleting");
            }
        }
        else
        {
            pnlContent.Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        btnNo.OnClientClick = ReturnScript + "return false;";

        base.OnPreRender(e);
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        CMS.Activities.Web.UI.AuthorizationHelper.AuthorizedManageActivity(SiteID, true);

        EnsureAsyncLog();
        RunAsyncDelete();
    }

    #endregion


    #region "Async control event handlers"

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
            if (!String.IsNullOrEmpty(CurrentError))
            {
                ShowError(CurrentError);
            }
        }

        if (ctlAsyncLog.Parameter != null)
        {
            // Return to the list page after successful deletion
            ltlScript.Text += ScriptHelper.GetScript(ctlAsyncLog.Parameter.ToString());
        }
    }

    #endregion


    #region "Delete methods"

    /// <summary>
    /// Starts asynchronous deleting of contacts.
    /// </summary>
    private void RunAsyncDelete()
    {
        ctlAsyncLog.EnsureLog();
        ctlAsyncLog.Parameter = ReturnScript;
        ctlAsyncLog.RunAsync((object parameter) => Delete(), WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Deletes activities.
    /// </summary>
    private void Delete()
    {
        var whereCondition = new WhereCondition(WhereCondition);
        try
        {
            var restrictedSitesCondition = CheckSitePermissions(whereCondition);
            DeleteActivities(whereCondition.Where(restrictedSitesCondition));
        }
        catch (ThreadAbortException ex)
        {
            if (!CMSThread.Stopped(ex))
            {
                LogExceptionToEventLog(ex);
            }
        }
        catch (Exception ex)
        {
            LogExceptionToEventLog(ex);
        }
    }


    /// <summary>
    /// Checks activity permissions.
    /// Returns restricted sites condition.
    /// </summary>
    private WhereCondition CheckSitePermissions(IWhereCondition whereCondition)
    {
        var restrictedSitesCondition = new WhereCondition();
        var activitiesSites = ActivityInfo.Provider.Get()
                                                  .Distinct()
                                                  .Column("ActivitySiteID")
                                                  .Where(whereCondition);
        foreach (var activity in activitiesSites)
        {
            if (!CurrentUser.IsAuthorizedPerObject(PermissionsEnum.Modify, "om.activity", SiteInfoProvider.GetSiteName(activity.ActivitySiteID)))
            {
                SiteInfo notAllowedSite = SiteInfo.Provider.Get(activity.ActivitySiteID);
                AddError(String.Format(GetString("accessdeniedtopage.info"), ResHelper.LocalizeString(notAllowedSite.DisplayName)));

                restrictedSitesCondition.WhereNotEquals("ActivitySiteID", activity.ActivitySiteID);
            }
        }

        return restrictedSitesCondition;
    }


    /// <summary>
    /// Delete items.
    /// </summary>
    private void DeleteActivities(IWhereCondition whereCondition)
    {
        var activitiesToDelete = ActivityInfo.Provider.Get()
                                                     .Columns("ActivityID", "ActivityType", "ActivityTitle")
                                                     .Where(whereCondition);
        foreach (var activity in activitiesToDelete)
        {
            var safeLog = HTMLHelper.HTMLEncode(string.Format("{0} - {1}", activity.ActivityTitle, activity.ActivityType));
            AddLog(safeLog);
            ActivityInfo.Provider.Delete(activity);
        }
    }

    #endregion


    #region "Log methods"

    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        Service.Resolve<IEventLogService>().LogException("Contact management", "DELETEACTIVITY", ex);
        AddError(GetString("om.activity.deletefailed") + ": " + ex.Message);
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
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    private void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
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
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);

        return log;
    }

    #endregion
}
