using System;
using System.Security.Principal;
using System.Threading;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.Internal;

using IOExceptions = System.IO;

public partial class CMSModules_System_System_Deployment : GlobalAdminPage
{
    #region "Variables"

    DeploymentManager mManager;
    
    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the instance of the current deployment manager
    /// </summary>
    private DeploymentManager CurrentDeploymentManager
    {
        get
        {
            return mManager ?? (mManager = new DeploymentManager());
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

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Handle Deployment manager events (Info/Error messages)
        CurrentDeploymentManager.Log += CurrentDeploymentManager_Log;
        CurrentDeploymentManager.Error += CurrentDeploymentManager_Error;

        // Handle Async control
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        ctlAsyncLog.TitleText = GetString("Deployment.Processing");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
                
        chkSaveLayouts.Checked = LayoutInfoProvider.StoreLayoutsInExternalStorage;
        chkSavePageTemplate.Checked = PageTemplateInfoProvider.StorePageTemplatesInExternalStorage;
        chkSaveWebpartLayout.Checked = WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage;
        chkSaveTransformation.Checked = TransformationInfoProvider.StoreTransformationsInExternalStorage;
        chkSaveAltFormLayouts.Checked = AlternativeFormInfoProvider.StoreAlternativeFormsInExternalStorage;
        chkSaveFormLayouts.Checked = DataClassInfoProvider.StoreFormLayoutsInExternalStorage;

        if (chkSaveLayouts.Checked || chkSavePageTemplate.Checked || chkSaveWebpartLayout.Checked
            || chkSaveTransformation.Checked || chkSaveAltFormLayouts.Checked || chkSaveFormLayouts.Checked)
        {
            lblSynchronization.Visible = true;
            btnSynchronize.Visible = true;
        }

        bool deploymentMode = SettingsKeyInfoProvider.DeploymentMode;
        chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled = chkSaveWebpartLayout.Enabled
            = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = !deploymentMode;

        if (SystemContext.IsRunningOnAzure)
        {
            ShowWarning(GetString("Deployment.AzureDisabled"));
            btnSaveAll.Enabled = false;
            btnSourceControl.Enabled = false;
            chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled = chkSaveWebpartLayout.Enabled
                = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = false;
        }

        if (SettingsKeyInfoProvider.DeploymentMode)
        {
            lblDeploymentInfo.Text = GetString("Deployment.SaveAllToDBInfo");
            btnSaveAll.ResourceString = "Deployment.SaveAllToDB";
            lblSourceControlInfo.Text = GetString("Deployment.SourceControlInfoDeploymentMode");
        }
        else
        {
            lblDeploymentInfo.Text = GetString("Deployment.SaveAllInfo");
            btnSaveAll.ResourceString = "Deployment.SaveAll";
            lblSourceControlInfo.Text = GetString("Deployment.SourceControlInfo");
        }

        if (SystemContext.DevelopmentMode)
        {
            ShowInformation(GetString("Deployment.DevelopmentMode"));
            btnSaveAll.Enabled = btnSourceControl.Enabled = btnSynchronize.Enabled = false;
            chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveWebpartLayout.Enabled = false;    
            chkSaveTransformation.Enabled = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = false;
        }
    }

    #endregion


    #region "Deployment methods"


    protected void btnSynchronize_Click(object sender, EventArgs e)
    {
        RunAsyncInternal(Synchronize);
    }


    protected void btnSourceControl_Click(object sender, EventArgs e)
    {
        RunAsyncInternal(SaveExternally);
    }


    protected void btnSaveAll_Click(object sender, EventArgs e)
    {
        RunAsyncInternal(Deploy);
    }


    protected void btnTest_Click(object sender, EventArgs e)
    {
        RunAsyncInternal(Test);
    }

    private void RunAsyncInternal(AsyncAction action)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        
        var parameter = GetParameters();

        ctlAsyncLog.RunAsync(p => action(parameter), WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Gets the parameters object for current controls
    /// </summary>
    private DeploymentParameters GetParameters()
    {
        return new DeploymentParameters
        {
            SaveAlternativeFormLayout = chkSaveAltFormLayouts.Checked,
            SaveFormLayout = chkSaveFormLayouts.Checked,
            SavePageTemplate = chkSavePageTemplate.Checked,
            SaveLayout = chkSaveLayouts.Checked,
            SaveTransformation = chkSaveTransformation.Checked,
            SaveWebPartLayout = chkSaveWebpartLayout.Checked
        };
    }


    /// <summary>
    /// Encapsulates the action with try catch block and logging
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <param name="cancelResString">Optional resource string used for canceled info message</param>
    private void RunWithTryCatch(Action action, string cancelResString = "Deployment.DeploymentCanceled")
    {
        try
        {
            action();
        }
        catch (ThreadAbortException ex)
        {
            if (CMSThread.Stopped(ex))
            {
                // When canceled
                AddError(GetString(cancelResString));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    private void Deploy(object parameter)
    {
        RunWithTryCatch(() => CurrentDeploymentManager.Deploy(parameter as DeploymentParameters));
    }


    private void Test(object parameter)
    {
        RunWithTryCatch(() => CurrentDeploymentManager.CompileVirtualObjects(null), "general.actioncanceled");
    }


    private void SaveExternally(object parameter)
    {
        RunWithTryCatch(() => CurrentDeploymentManager.SaveExternally(parameter as DeploymentParameters));
    }


    private void Synchronize(object parameter)
    {
        RunWithTryCatch(() => CurrentDeploymentManager.Synchronize(parameter as DeploymentParameters));
    }

    #endregion


    #region "Deployment manager events"

    private void CurrentDeploymentManager_Error(object sender, DeploymentManagerLogEventArgs e)
    {
        AddError(e.Message);
    }

    private void CurrentDeploymentManager_Log(object sender, DeploymentManagerLogEventArgs e)
    {
        AddLog(e.Message);
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    private void LogExceptionToEventLog(Exception ex)
    {
        var logData = new EventLogData(EventTypeEnum.Error, "System deployment", "DEPLOYMENT")
        {
            EventDescription = EventLogProvider.GetExceptionLogMessage(ex),
            EventUrl = RequestContext.RawURL,
            UserID = CurrentUser.UserID,
            UserName = CurrentUser.UserName,
            IPAddress = RequestContext.UserHostAddress,
            SiteID = SiteContext.CurrentSiteID
        };
        
        Service.Resolve<IEventLogService>().LogEvent(logData);
        AddError(GetString("Deployment.DeploymentFailed") + ": " + ex.Message);
    }


    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        pnlLog.Visible = false;

        string cancel = GetString("general.actioncanceled");
        AddLog(cancel);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array(); RefreshCurrent();");

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
            return;
        }

        ShowConfirmation(cancel);
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        if (ctlAsyncLog.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsyncLog.Stop();
        }

        ShowError(CurrentError);
        
        pnlLog.Visible = false;
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        pnlLog.Visible = false;

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
            return;
        }

        if (SettingsKeyInfoProvider.DeploymentMode)
        {
            ShowConfirmation(GetString("Deployment.ObjectsSavedSuccessfully"));
        }
        else
        {
            ShowChangesSaved();
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
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}
