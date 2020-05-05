using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Content_CMSDesk_Properties_Attachments : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Attachments"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.Attachments");
        }

        // Enable split mode
        EnableSplitMode = true;

        // Ensure correct padding
        CurrentMaster.PanelContent.CssClass = "";
        CurrentMaster.MessagesPlaceHolder.OffsetX = 16;
        CurrentMaster.MessagesPlaceHolder.OffsetY = 16;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Register scripts
            string script = "function RefreshForm(){" + Page.ClientScript.GetPostBackEventReference(btnRefresh, "") + " }";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshForm", ScriptHelper.GetScript(script));

            ucAttachments.Container.CssClass = "PageContent";
            ucAttachments.DocumentID = Node.DocumentID;
            ucAttachments.NodeParentNodeID = Node.NodeParentID;
            ucAttachments.NodeClassName = Node.NodeClassName;

            // Resize attachment due to site settings
            string siteName = SiteContext.CurrentSiteName;
            ucAttachments.ResizeToHeight = ImageHelper.GetAutoResizeToHeight(siteName);
            ucAttachments.ResizeToWidth = ImageHelper.GetAutoResizeToWidth(siteName);
            ucAttachments.ResizeToMaxSideSize = ImageHelper.GetAutoResizeToMaxSideSize(siteName);
            ucAttachments.PageSize = "10,25,50,100,##ALL##";

            pnlContent.Enabled = !DocumentManager.ProcessingAction;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Node != null)
        {
            ucAttachments.Enabled = DocumentManager.AllowSave;
        }
    }


    protected override void OnPreRenderComplete(EventArgs e)
    {
        if (!menuElem.HeaderActions.Visible)
        {
            // Display document information, if not ensured by menu
            pnlDocInfo.Visible = true;
            DocumentManager.LocalDocumentPanel = pnlDocInfo;
            DocumentManager.ShowDocumentInfo(true);
        } 

        base.OnPreRenderComplete(e);
    }


    void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Refresh content
        if (e.ActionName == DocumentComponentEvents.UNDO_CHECKOUT)
        {
            ucAttachments.ReloadData();
        }
    }


    /// <summary>
    /// Refresh button click event handler.
    /// </summary>
    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check permission to modify document
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed)
            {
                // Ensure version for later detection whether node is published
                Node.VersionManager.EnsureVersion(Node, Node.IsPublished);

                // Get workflow manager
                WorkflowManager wm = WorkflowManager.GetInstance(Tree);

                // Tree refresh is needed only if node was archived or published
                WorkflowStepInfo currentStep = wm.GetStepInfo(Node);
                bool refreshTree = (currentStep != null) && (currentStep.StepIsArchived || currentStep.StepIsPublished);

                // Move to edit step
                wm.MoveToFirstStep(Node);

                // Refresh frames and tree
                string script = "if(window.FramesRefresh){FramesRefresh(" + refreshTree.ToString().ToLowerCSafe() + ", " + Node.NodeID + ");}";
                ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshAction", ScriptHelper.GetScript(script));
            }
        }
    }

    #endregion
}
