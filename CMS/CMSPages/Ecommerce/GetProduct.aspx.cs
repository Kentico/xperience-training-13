using System;
using System.Globalization;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.Ecommerce;

public partial class CMSPages_Ecommerce_GetProduct : CMSPage
{
    #region "Variables"

    private const string SKU_GUID = "skuguid";
    private const string PRODUCT_ID = "productId";

    protected Guid skuGuid = Guid.Empty;
    protected int productId;
    protected SiteInfo currentSite;
    protected string url = string.Empty;

    #endregion


    public CMSPages_Ecommerce_GetProduct()
    {
        SetLiveCulture();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialization
        productId = QueryHelper.GetInteger(PRODUCT_ID, 0);
        skuGuid = QueryHelper.GetGuid(SKU_GUID, Guid.Empty);
        currentSite = SiteContext.CurrentSite;

        var skuObj = SKUInfo.Provider.Get(productId);

        if ((skuObj != null) && skuObj.IsProductVariant)
        {
            // Get parent product of variant
            var parent = skuObj.Parent as SKUInfo;

            if (parent != null)
            {
                productId = parent.SKUID;
                skuGuid = parent.SKUGUID;
            }
        }

        string where = null;
        if (productId > 0)
        {
            where = "NodeSKUID = " + productId;
        }
        else if (skuGuid != Guid.Empty)
        {
            where = "SKUGUID = '" + skuGuid + "'";
        }

        if ((where != null) && (currentSite != null))
        {
            var node = DocumentHelper.GetDocuments()
                         .Path("/", PathTypeEnum.Section)
                         .Culture(CultureInfo.CurrentCulture.Name)
                         .CombineWithDefaultCulture()
                         .Where(where)
                         .Published()
                         .TopN(1)
                         .FirstOrDefault();

            if (node != null)
            {
                // Get specified product url
                url = DocumentURLProvider.GetUrl(node);
            }
        }

        if ((url != string.Empty) && (currentSite != null))
        {
            url = AppendQueryStringParameters(url);
            // Redirect to specified product
            URLHelper.Redirect(UrlResolver.ResolveUrl(url));
        }
        else
        {
            // Display error message
            lblInfo.Visible = true;
            lblInfo.Text = GetString("GetProduct.NotFound");
        }
    }


    private string AppendQueryStringParameters(string targetUrl)
    {
        var queryString = QueryHelper.GetParameterString();
        targetUrl = URLHelper.AppendQuery(targetUrl, queryString);

        return URLHelper.RemoveParametersFromUrl(targetUrl, SKU_GUID, PRODUCT_ID);
    }
}