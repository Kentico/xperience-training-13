using System;

using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;

// Set edited object
[EditedObject(WorkflowInfo.OBJECT_TYPE_AUTOMATION, "processid")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessSteps")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Steps : CMSAutomationPage
{
    #region "Constants"

    private const string SERVICEURL = "~/CMSModules/Automation/Services/AutomationDesignerService.svc";

    #endregion


    #region "Variables"

    private int mProcessID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current workflow ID
    /// </summary>
    public int ProcessID
    {
        get
        {
            if (mProcessID <= 0)
            {
                mProcessID = QueryHelper.GetInteger("processid", 0);
            }
            return mProcessID;
        }
    }

    #endregion


    #region "Event handlers"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        designerElem.ServiceUrl = SERVICEURL;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (ProcessID <= 0)
        {
            designerElem.StopProcessing = true;
            return;
        }

        designerElem.WorkflowID = ProcessID;

        bool licenseFail = !WorkflowInfoProvider.IsMarketingAutomationAllowed();
        if (licenseFail || !WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName))
        {
            designerElem.ReadOnly = true;
            MessagesPlaceHolder.OffsetY = 10;
            MessagesPlaceHolder.UseRelativePlaceHolder = false;
            ShowInformation(GetString(licenseFail ? "wf.licenselimitation" : "general.modifynotallowed"));
        }

        ScriptHelper.HideVerticalTabs(this);
    }

    #endregion
}
