using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[Security(Resource = ModuleName.ECOMMERCE, ResourceSite = true)]
[UIElement("CMS.Content", "EditForm", ValidateDialogHash = false)]
public partial class CMSModules_Ecommerce_Pages_Content_Product_Product_Edit_General : CMSEcommercePage
{
    #region "Lifecycle"

    protected override void OnPreInit(EventArgs e)
    {
        // Ensure document manager
        EnsureDocumentManager = true;

        base.OnPreInit(e);

        EnableSplitMode = true;

        // If not EditLive view mode, switch to form mode to keep editing the form
        if (PortalContext.ViewMode != ViewModeEnum.EditLive)
        {
            PortalContext.ViewMode = ViewModeEnum.EditForm;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        CMSContentPage.CheckSecurity();

        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.PRODUCTS_READ))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceRead OR ReadProducts");
        }

        SKUInfo sku = null;
        if (Node != null)
        {
            sku = SKUInfo.Provider.Get(Node.NodeSKUID);
        }

        if ((sku != null) && (sku.SKUSiteID != SiteContext.CurrentSiteID) && ((sku.SKUSiteID != 0) || !ECommerceSettings.AllowGlobalProducts(SiteContext.CurrentSiteName)))
        {
            EditedObject = null;
        }

        productEditElem.ProductSaved += productEditElem_ProductSaved;

        string action = QueryHelper.GetString("action", string.Empty).ToLowerCSafe();
        if (action == "newculture")
        {
            // Ensure breadcrumb for new culture version of product
            EnsureDocumentBreadcrumbs(PageBreadcrumbs, action: GetString("content.newcultureversiontitle"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterEditScript(Page);
        ScriptHelper.RegisterLoader(Page);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the ProductEdit.ProductSaved event.
    /// </summary>
    protected void productEditElem_ProductSaved(object sender, EventArgs e)
    {
        if ((Action == "newculture") || (Action == "bindsku") || productEditElem.ProductSavedSkuBindingRemoved)
        {
            var node = (TreeNode)productEditElem.Product;

            ScriptHelper.RefreshTree(this, node.NodeID, node.NodeID);

            var selectNodeScript = ScriptHelper.GetScript(string.Format("SelectNode({0});", node.NodeID));
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "Refresh", selectNodeScript);
        }
    }

    #endregion
}
