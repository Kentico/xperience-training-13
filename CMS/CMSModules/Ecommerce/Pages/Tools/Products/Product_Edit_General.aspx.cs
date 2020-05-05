using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_General : CMSProductsPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "products";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the value of the 'categoryId' URL parameter.
    /// </summary>
    public int OptionCategoryID
    {
        get
        {
            return QueryHelper.GetInteger("categoryId", 0);
        }
    }


    /// <summary>
    /// Gets the edited SKU.
    /// </summary>
    public SKUInfo SKU
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets parent product ID, if option is edited from product options page
    /// </summary>
    public int ParentProductID
    {
        get
        {
            return QueryHelper.GetInteger("parentProductId", 0);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnPreInit(EventArgs e)
    {
        IsProductOption = OptionCategoryID > 0;

        base.OnPreInit(e);

        CheckDocPermissions = true;
        EnableSplitMode = NodeID > 0;

        PortalContext.ViewMode = ViewModeEnum.EditForm;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SKU = SKUInfo.Provider.Get(ProductID);
        if (SKU != null)
        {
            CheckEditedObjectSiteID(SKU.SKUSiteID);
        }

        // Redirected from another page with saved flag
        if ((QueryHelper.GetInteger("saved", 0) == 1) && !RequestHelper.IsPostBack())
        {
            ShowChangesSaved();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Setup help
        object options = new
        {
            helpName = "lnkProductEditHelp",
            helpUrl = DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_LINK)
        };
        ScriptHelper.RegisterModule(this, "CMS/DialogContextHelpChange", options);

        if (IsProductOption)
        {
            // Check UI personalization for product option
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "ProductOptions.Options.General");
        }
        else
        {
            // Check UI personalization for product
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Products.General");
        }
    }


    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        SKU = SKUInfo.Provider.Get(ProductID);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        CreateBreadcrumbs();

        ScriptHelper.RegisterEditScript(Page);
        ScriptHelper.RegisterLoader(Page);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates breadcrumbs.
    /// </summary>
    private void CreateBreadcrumbs()
    {
        if (!IsProductOption)
        {
            // Ensure correct suffix
            UIHelper.SetBreadcrumbsSuffix(GetString("objecttype.com_sku"));
        }
    }

    #endregion
}
