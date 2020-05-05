using System;

using CMS.UIControls;


[Security(Resource = "CMS.Content", UIElements = "Properties.Versions")]
public partial class CMSModules_Content_CMSDesk_Properties_Versions : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        versionsElem.Node = Node;

        EnableSplitMode = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node == null)
        {
            // Hide all if no node is specified
            versionsElem.Visible = false;
        }

        versionsElem.Enabled = !DocumentManager.ProcessingAction;

        DocumentManager.LocalDocumentPanel = pnlDocInfo;
    }

    #endregion
}