using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.WorkflowEngine.Web.UI;


public partial class CMSModules_Workflows_Workflow_Designer : CMSWorkflowPage
{
    private const string SERVICEURL = "~/CMSModules/Workflows/Services/WorkflowDesignerService.svc";


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        designerElem.ServiceUrl = SERVICEURL;

        CssRegistration.RegisterBootstrap(this);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (WorkflowId <= 0)
        {
            designerElem.StopProcessing = true;
            return;
        }

        designerElem.WorkflowID = WorkflowId;

        if(!LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.AdvancedWorkflow))
        {
            designerElem.ReadOnly = true;
            MessagesPlaceHolder.OffsetY = 10;
            MessagesPlaceHolder.UseRelativePlaceHolder = false;
            ShowInformation(GetString("wf.licenselimitation"));
        }

        EditedObject = CurrentWorkflow;

        ScriptHelper.HideVerticalTabs(this);
    }
}
