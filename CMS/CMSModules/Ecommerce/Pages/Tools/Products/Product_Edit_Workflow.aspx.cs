using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.UIControls;
using CMS.WorkflowEngine;


[Title("com.ui.productsworkflow")]
[UIElement(ModuleName.ECOMMERCE, "Products.Workflow")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Workflow : CMSProductsPage
{
    #region "Variables"

    private WorkflowInfo workflow = null;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

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
