using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.UIControls;


[Title("general.pages")]
[UIElement(ModuleName.ECOMMERCE, "Products.Documents")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Documents : CMSProductsPage
{
    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        productDocuments.ProductID = ProductID;
        var sku = SKUInfo.Provider.Get(ProductID);
        EditedObject = sku;

        if (sku != null)
        {
            CheckEditedObjectSiteID(sku.SKUSiteID);
        }
    }

    #endregion
}