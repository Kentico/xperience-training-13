using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "Products.Tags")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Tags : CMSProductsPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Enable split mode
        EnableSplitMode = true;

        DocumentManager.UseDocumentHelper = false;


        // Register the scripts
        ScriptHelper.RegisterLoader(Page);

        MetaDataControlExtender extender = new MetaDataControlExtender();
        extender.UIModuleName = ModuleName.ECOMMERCE;
        extender.UITagsElementName = "Products.Tags";
        extender.Init(editForm);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlContent.Enabled = DocumentManager.AllowSave;
    }
}
