using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("com.newproductorsection")]
[UIElement(ModuleName.ECOMMERCE, "Products")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_New_ProductOrSection : CMSProductsPage
{
    private const string CONTENT_CMSDESK_FOLDER = "~/CMSModules/Content/CMSDesk/";


    protected override void OnPreInit(EventArgs e)
    {
        CheckExploreTreePermission();

        // New UI element
        var newElement = new UIElementAttribute(ModuleName.CONTENT, "New", false, true);
        newElement.Check(this);

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get parameters
        int parentNodeId = QueryHelper.GetInteger("parentnodeid", 0);
        string parentCulture = QueryHelper.GetString("parentculture", null);

        var newSectionUrl = ResolveUrl(CONTENT_CMSDESK_FOLDER + "Edit/Edit.aspx");
        newSectionUrl = URLHelper.AddParameterToUrl(newSectionUrl, "action", "new");
        newSectionUrl = URLHelper.AddParameterToUrl(newSectionUrl, "mode", "productssection");

        // Init product type selector
        ProductTypes.ProductSelectionUrl = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx");
        ProductTypes.SelectionUrl = newSectionUrl;
        ProductTypes.NoDataMessage = GetString("com.products.noproducttypeallowed");
        ProductTypes.MainCaption = GetString("content.newinfo.maincaption");
        ProductTypes.Caption = GetString("com.NewProductOrSectionInfo");
        ProductTypes.ParentNodeID = parentNodeId;
        ProductTypes.ParentCulture = parentCulture;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script
        ScriptHelper.RegisterJQuery(this);
        ScriptHelper.RegisterScriptFile(this, CONTENT_CMSDESK_FOLDER + "New/New.js");

        EnsureProductBreadcrumbs(PageBreadcrumbs, "com.newproductorsection", false, true, false);
    }
}
