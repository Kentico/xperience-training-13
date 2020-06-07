using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Web.UI;


[ParentObject("cms.workflow", "workflowid")]
[EditedObject("cms.workflowscope", "scopeid")]

[Help("newedit_scope")]

[CheckLicence(FeatureEnum.WorkflowVersioning)]
public partial class CMSModules_Workflows_Workflow_Scope_Edit : CMSWorkflowPage
{
    private int siteId;

    #region "Private properties"

    private WorkflowScopeInfo CurrentScope
    {
        get
        {
            return (WorkflowScopeInfo)editForm.EditedObject;
        }
    }


    private new WorkflowInfo CurrentWorkflow
    {
        get
        {
            return (WorkflowInfo)editForm.ParentObject;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get site ID
        siteId = GetSiteId();

        // Check languages on site
        LicenseCheck();

        // Initialize page title
        InitializeMasterPage();

        editForm.RedirectUrlAfterCreate = URLHelper.AppendQuery(
            UIContextHelper.GetElementUrl(ModuleName.CMS, "EditWorkflowScope", false),
            "objectId={%EditedObject.ID%}&saved=1&scopeId={%EditedObject.Id%}&parentObjectId=" + CurrentWorkflow.WorkflowID + "&workflowId=" + CurrentWorkflow.WorkflowID + "&siteId=" + siteId);
    }

    #endregion


    #region "Other methods"

    protected void editForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        // Set site for inner controls
        editForm.ObjectSiteID = GetSiteId();
    }


    private int GetSiteId()
    {
        return CurrentScope.ScopeID == 0 ? QueryHelper.GetInteger("siteid", 0) : CurrentScope.ScopeSiteID;
    }


    private void LicenseCheck()
    {
        SiteInfo si = SiteInfo.Provider.Get(siteId);
        if (si != null)
        {
            // Check whether workflow is enabled for specified site
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetDomainName(si.DomainName), FeatureEnum.WorkflowVersioning);

            if (!CultureSiteInfoProvider.LicenseVersionCheck(si.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Edit))
            {
                ShowError(GetString("licenselimitation.siteculturesexceeded"));
                editForm.Enabled = false;
            }
        }
    }

    
    private void InitializeMasterPage()
    {
        string workflowScopes = GetString("Development-Workflow_Scope_New.Scopes");
        string workflowScopesUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Workflows.Scope", false), "parentObjectID=" + CurrentWorkflow.WorkflowID + "&workflowID=" + CurrentWorkflow.WorkflowID + "&siteId=" + siteId);
        string currentScope = CurrentScope.ScopeID > 0 ? CurrentScope.ScopeStartingPath : GetString("Development-Workflow_Scope_New.Scope");

        // Init breadcrumbs
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
            Text = workflowScopes,
            RedirectUrl = workflowScopesUrl
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = currentScope,
        });
    }

    #endregion
}