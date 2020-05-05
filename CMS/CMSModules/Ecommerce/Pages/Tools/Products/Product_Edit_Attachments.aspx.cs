using System;

using CMS.Base;
using CMS.DocumentEngine;

using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


[UIElement(ModuleName.ECOMMERCE, "Products.Attachments")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Attachments : CMSProductsPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;

        // Ensure correct padding
        CurrentMaster.PanelContent.CssClass = "";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check read permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(menuElem.Node.GetDocumentName())));
            }

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

    #endregion


    #region "Methods"

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

                // Tree refresh is needed only if node was archived or published
                WorkflowManager wm = WorkflowManager.GetInstance(Tree);
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
