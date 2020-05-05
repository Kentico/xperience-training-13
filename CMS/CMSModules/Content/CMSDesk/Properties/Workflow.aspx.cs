using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Content_CMSDesk_Properties_Workflow : CMSPropertiesPage
{
    #region "Variables"

    private WorkflowInfo workflow;
    
    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Workflow"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.Workflow");
        }

        DocumentManager.OnCheckPermissions += DocumentManager_OnCheckPermissions;


        // Disable confirm changes checking
        DocumentManager.RegisterSaveChangesScript = false;

        // Init node
        workflowElem.Node = Node;

        workflow = DocumentManager.Workflow;
        if (workflow != null)
        {
            menuElem.OnClientStepChanged = ClientScript.GetPostBackEventReference(pnlUp, null);

            // Backward compatibility - Display Archive button for all steps
            menuElem.ForceArchive = workflow.IsBasic;
        }

        // Enable split mode
        EnableSplitMode = true;
    }


    protected void DocumentManager_OnCheckPermissions(object sender, SimpleDocumentManagerEventArgs e)
    {
        e.CheckDefault = false;
        e.ErrorMessage = String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), HTMLHelper.HTMLEncode(e.Node.GetDocumentName()));
        e.IsValid = (CurrentUser.IsAuthorizedPerDocument(e.Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed) || DocumentManager.WorkflowManager.CanUserManageWorkflow(CurrentUser, Node.NodeSiteName);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlContainer.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (workflow != null)
        {
            // Backward compatibility
            if (workflow.WorkflowAutoPublishChanges)
            {
                string message = DocumentManager.GetDocumentInfo(true);
                if (!string.IsNullOrEmpty(message))
                {
                    message += "<br />";
                }
                message += GetString("WorfklowProperties.AutoPublishChanges");
                DocumentManager.DocumentInfo = message;
            }
        }
        else
        {
            menuElem.Visible = false;
        }
        
        // Register the scripts
        if (!DocumentManager.RefreshActionContent)
        {
            ScriptHelper.RegisterLoader(Page);
        }
    }


    protected override void OnPreRenderComplete(EventArgs e)
    {
        if (!menuElem.HeaderActions.Visible)
        {
            // Show info about workflow for non-authorized user
            pnlDocInfo.Visible = true;
            DocumentManager.LocalDocumentPanel = pnlDocInfo;
            DocumentManager.ShowDocumentInfo(true);
        }

        base.OnPreRenderComplete(e);
    }

    #endregion
}
