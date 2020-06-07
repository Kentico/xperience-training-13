using System;

using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;

[EditedObject(WorkflowInfo.OBJECT_TYPE_AUTOMATION, "processid")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessSteps")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_AutomationDesignerPage : CMSAutomationPage
{
    private const string SERVICE_URL = "~/CMSModules/Automation/Services/AutomationDesignerService.svc";


    private int mWorkflowID;


    /// <summary>
    /// Current workflow ID.
    /// </summary>
    public int WorkflowID
    {
        get
        {
            if (mWorkflowID <= 0)
            {
                mWorkflowID = QueryHelper.GetInteger("processid", 0);
            }
            return mWorkflowID;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        designerElem.ServiceUrl = SERVICE_URL;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (WorkflowID <= 0)
        {
            designerElem.StopProcessing = true;
            return;
        }

        designerElem.WorkflowID = WorkflowID;

        if (!LicenseIsSufficient || !AuthorizedToManageAutomation)
        {
            designerElem.ReadOnly = true;
        }
    }
}
