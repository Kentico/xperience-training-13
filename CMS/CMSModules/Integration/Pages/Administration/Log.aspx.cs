using System;

using CMS.Base;

using System.Text;

using CMS.Helpers;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_Integration_Pages_Administration_Log : CMSIntegrationPage
{
    #region "Variables"

    private IntegrationSynchronizationInfo mSynchronizationInfo = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the synchronization identifier.
    /// </summary>
    private int SynchronizationID
    {
        get
        {
            return QueryHelper.GetInteger("synchronizationid", 0);
        }
    }


    /// <summary>
    /// Gets the synchronization info object.
    /// </summary>
    private IntegrationSynchronizationInfo SynchronizationInfo
    {
        get
        {
            return mSynchronizationInfo ?? (mSynchronizationInfo = IntegrationSynchronizationInfoProvider.GetIntegrationSynchronizationInfo(SynchronizationID));
        }
    }

    #endregion"


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register modal dialog scripts
        RegisterModalPageScripts();

        gridLog.OnAction += gridLog_OnAction;
        gridLog.ZeroRowsText = GetString("Task.LogNoEvents");
        gridLog.WhereCondition = "SyncLogSynchronizationID = " + SynchronizationID;

        PageTitle.TitleText = GetString("Task.LogHeader");
        CurrentMaster.DisplayControlsPanel = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (SynchronizationInfo != null)
        {
            IntegrationTaskInfo ti = IntegrationTaskInfoProvider.GetIntegrationTaskInfo(SynchronizationInfo.SynchronizationTaskID);
            IntegrationConnectorInfo si = IntegrationConnectorInfoProvider.GetIntegrationConnectorInfo(SynchronizationInfo.SynchronizationConnectorID);
            // Prepare task description
            StringBuilder sbTaskInfo = new StringBuilder();
            sbTaskInfo.Append("<div class=\"form-horizontal\">");
            if ((ti != null) || (si != null))
            {
                if (ti != null)
                {
                    sbTaskInfo.Append("<div class=\"form-group\"><div class=\"editing-form-label-cell\"><span class=\"control-label\">" + GetString("integration.tasktitle") + ":</span></div><div class=\"editing-form-value-cell\"><span class=\"form-control-text\">" + HTMLHelper.HTMLEncode(ti.TaskTitle) + "</span></div></div>");
                }
                if (si != null)
                {
                    sbTaskInfo.Append("<div class=\"form-group\"><div class=\"editing-form-label-cell\"><span class=\"control-label\">" + GetString("integration.connectorname") + ":</span></div><div class=\"editing-form-value-cell\"><span class=\"form-control-text\">" + HTMLHelper.HTMLEncode(si.ConnectorDisplayName) + "</span></div></div>");
                }
            }
            sbTaskInfo.Append("</div>");
            lblInfo.Text = sbTaskInfo.ToString();
        }
        lblInfo.Visible = (lblInfo.Text != "");
        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// UniGrid action event handler.
    /// </summary>
    protected void gridLog_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                int logid = ValidationHelper.GetInteger(actionArgument, 0);
                if (logid > 0)
                {
                    IntegrationSyncLogInfoProvider.DeleteIntegrationSyncLogInfo(logid);
                }
                break;
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        IntegrationSyncLogInfoProvider.DeleteIntegrationSyncLogs(SynchronizationID);
        gridLog.ReloadData();
    }

    #endregion
}