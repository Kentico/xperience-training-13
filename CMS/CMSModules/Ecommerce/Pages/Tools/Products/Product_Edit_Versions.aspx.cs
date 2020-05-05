using System;

using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.UIControls;


[Title("com.ui.productsversions")]
[UIElement(ModuleName.ECOMMERCE, "Products.Versions")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Versions : CMSProductsPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        EditedDocument = Node;

        DocumentManager.LocalDocumentPanel = pnlDocInfo;
    }
}
