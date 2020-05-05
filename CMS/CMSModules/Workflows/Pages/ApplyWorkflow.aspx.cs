using System;

using CMS.Helpers;

using System.Text;

using CMS.Base.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Workflows_Pages_ApplyWorkflow : CMSModalPage
{
    #region "Events"

    protected override void OnPreInit(EventArgs e)
    {
        EnsureDocumentManager = true;
        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        if (DocumentManager.IsActionAllowed(DocumentComponentEvents.APPLY_WORKFLOW))
        {
            // Initialize header
            InitHeader();

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
function RefreshParent() { 
    if(wopener.RefreshTree) { wopener.RefreshTree(", Node.NodeID, ", ", Node.NodeID, @"); }
    if(wopener.SelectNode) { wopener.SelectNode(", Node.NodeID, @"); }
}"
                );
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "action", sb.ToString(), true);

            Save += (s, ea) => Apply();
        }
        else
        {
            pnlContent.Visible = false;
        }

        // Prevent registering 'SaveChanges' script
        DocumentManager.RegisterSaveChangesScript = false;

        base.OnLoad(e);
    }


    private void Apply()
    {
        var path = Node.NodeAliasPath;

        var scope = new WorkflowScopeInfo
        {
            ScopeStartingPath = path,
            ScopeExcludeChildren = radDocument.Checked,
            ScopeExcluded = false,
            ScopeWorkflowID = ValidationHelper.GetInteger(ucWorkflow.Value, 0),
            ScopeSiteID = SiteContext.CurrentSiteID
        };

        scope.Insert();

        ScriptHelper.RegisterStartupScript(this, typeof(string), "CloseApplyWorkflowDialog", ScriptHelper.GetScript("RefreshParent();CloseDialog();"));
    }


    private void InitHeader()
    {
        PageTitle.TitleText = GetString("WorkflowProperties.ApplyTitle");
    }

    #endregion
}
