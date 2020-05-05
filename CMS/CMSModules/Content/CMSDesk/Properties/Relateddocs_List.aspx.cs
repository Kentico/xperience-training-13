using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Relationships;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_Properties_Relateddocs_List : CMSPropertiesPage
{
    #region "Properties"
    
    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.RelatedDocs"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.RelatedDocs");
        }

        // Initialize node
        relatedDocuments.TreeNode = Node;

        CurrentMaster.PanelContent.CssClass = "";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if any relationship exists
        DataSet dsRel = RelationshipNameInfoProvider.GetRelationshipNames("RelationshipAllowedObjects LIKE '%" + ObjectHelper.GROUP_DOCUMENTS + "%' AND RelationshipNameID IN (SELECT RelationshipNameID FROM CMS_RelationshipNameSite WHERE SiteID = " + SiteContext.CurrentSiteID + ")", null, 1, "RelationshipNameID");
        if (DataHelper.DataSourceIsEmpty(dsRel))
        {
            relatedDocuments.Visible = false;
            ShowInformation(GetString("relationship.norelationship"));
        }
        else
        {
            if (Node != null)
            {
                bool enabled = true;

                // Check modify permissions
                if (!DocumentUIHelper.CheckDocumentPermissions(Node, PermissionsEnum.Modify))
                {
                    relatedDocuments.Enabled = enabled = false;
                }

                menuElem.AddExtraAction(new HeaderAction()
                {
                    Enabled = enabled,
                    Text = GetString("relationship.addrelateddocs"),
                    RedirectUrl = "~/CMSModules/Content/CMSDesk/Properties/Relateddocs_Add.aspx?nodeid=" + NodeID
                });
            }
        }

        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }

    #endregion
}