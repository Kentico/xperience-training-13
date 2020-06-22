using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Relationships;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("com.ui.productsrelated")]
[UIElement(ModuleName.ECOMMERCE, "Products.RelatedProducts")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Related : CMSProductsPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        EditedDocument = Node;

        // Get default relationship name from settings
        string defaultRelName = ECommerceSettings.RelatedProductsRelationshipName(SiteContext.CurrentSiteName);

        // Check if relationship exists
        bool anyRelationshipsFound = true;
        RelationshipNameInfo defaultRelNameInfo = RelationshipNameInfo.Provider.Get(defaultRelName);
        if (defaultRelNameInfo != null)
        {
            relatedDocuments.RelationshipName = defaultRelName;
        }
        else
        {
            // Check if any relationship exists
            DataSet dsRel = RelationshipNameInfoProvider.GetRelationshipNames("RelationshipAllowedObjects LIKE '%" + ObjectHelper.GROUP_DOCUMENTS + "%' AND RelationshipNameID IN (SELECT RelationshipNameID FROM CMS_RelationshipNameSite WHERE SiteID = " + SiteContext.CurrentSiteID + ")", null, 1, "RelationshipNameID");
            if (DataHelper.DataSourceIsEmpty(dsRel))
            {
                relatedDocuments.Visible = false;
                ShowInformation(GetString("relationship.norelationship"));

                anyRelationshipsFound = false;
            }
        }

        if (anyRelationshipsFound && (Node != null))
        {
            // Check read permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Denied)
            {
                RedirectToAccessDenied(string.Format(GetString("cmsdesk.notauthorizedtoreaddocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName())));
            }
            // Check modify permissions
            else if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
            {
                pnlDocInfo.Label.Text = string.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), HTMLHelper.HTMLEncode(Node.GetDocumentName()));
                relatedDocuments.Enabled = false;
                CurrentMaster.HeaderActions.Enabled = false;
            }
        }

        // Set tree node
        relatedDocuments.TreeNode = Node;

        // Set starting path
        if (!string.IsNullOrEmpty(ProductsStartingPath))
        {
            relatedDocuments.StartingPath = ProductsStartingPath;
        }

        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("relationship.addrelateddocs"),
            OnClientClick = relatedDocuments.GetAddRelatedDocumentScript()
        });
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ScriptHelper.RegisterDialogScript(Page);
    }
}
