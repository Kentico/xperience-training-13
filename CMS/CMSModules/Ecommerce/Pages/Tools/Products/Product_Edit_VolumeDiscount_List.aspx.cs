using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("com.ui.productsvolumediscounts")]
[UIElement(ModuleName.ECOMMERCE, "Products.VolumeDiscounts")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_VolumeDiscount_List : CMSProductsPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "volume_discounts";

    #endregion


    #region "Variables"

    private SKUInfo product;
    private CurrencyInfo productCurrency;

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        product = SKUInfo.Provider.Get(ProductID);

        // Setup help
        object options = new
        {
            helpName = "lnkProductEditHelp",
            helpUrl = DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_LINK)
        };
        ScriptHelper.RegisterModule(this, "CMS/DialogContextHelpChange", options);

        EditedObject = product;

        if (product != null)
        {
            // Check site id
            CheckEditedObjectSiteID(product.SKUSiteID);

            // Load product currency
            productCurrency = CurrencyInfoProvider.GetMainCurrency(product.SKUSiteID);

            // Display product price
            headProductPriceInfo.Text = string.Format(GetString("com_sku_volume_discount.skupricelabel"), CurrencyInfoProvider.GetFormattedPrice(product.SKUPrice, productCurrency));
        }

        // Set unigrid properties
        SetUnigridProperties();

        // Initialize the master page elements
        InitializeMasterPage();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display product price only if grid is not empty
        headProductPriceInfo.Visible = UniGrid.GridView.Rows.Count > 0;
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Initializes the master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        string newUrl = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_VolumeDiscount_Edit.aspx";
        newUrl = URLHelper.AddParameterToUrl(newUrl, "ProductID", ProductID.ToString());
        newUrl = URLHelper.AddParameterToUrl(newUrl, "siteId", SiteID.ToString());
        newUrl = URLHelper.AddParameterToUrl(newUrl, "dialog", QueryHelper.GetString("dialog", "0"));

        // Set new volume discount action
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("Product_Edit_VolumeDiscount_List.NewItemCaption"),
            RedirectUrl = AddNodeIDParameterToUrl(newUrl)
        });
    }


    /// <summary>
    /// Initializes unigrid properties.
    /// </summary>
    private void SetUnigridProperties()
    {
        UniGrid.OnAction += UniGrid_OnAction;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
        UniGrid.WhereCondition = "VolumeDiscountSKUID = " + ProductID;
        UniGrid.OrderBy = "VolumeDiscountMinCount ASC";
        // Set empty grid message
        UniGrid.ZeroRowsText = GetString("com.novolumediscount");
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName.ToLowerInvariant() == "edit")
        {
            string editUrl = "Product_Edit_VolumeDiscount_Edit.aspx";
            editUrl = URLHelper.AddParameterToUrl(editUrl, "productID", ProductID.ToString());
            editUrl = URLHelper.AddParameterToUrl(editUrl, "volumeDiscountID", Convert.ToString(actionArgument));
            editUrl = URLHelper.AddParameterToUrl(editUrl, "siteId", SiteID.ToString());
            editUrl = URLHelper.AddParameterToUrl(editUrl, "dialog", QueryHelper.GetString("dialog", "0"));

            URLHelper.Redirect(UrlResolver.ResolveUrl(AddNodeIDParameterToUrl(editUrl)));
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "discountvalue":
                DataRowView row = (DataRowView)parameter;
                decimal value = ValidationHelper.GetDecimal(row["VolumeDiscountValue"], 0);
                bool isFlat = ValidationHelper.GetBoolean(row["VolumeDiscountIsFlatValue"], false);

                // If value is relative, add "%" next to the value.
                if (isFlat)
                {
                    return CurrencyInfoProvider.GetFormattedPrice(value, productCurrency);
                }

                return ECommerceHelper.GetFormattedPercentageValue(value, CultureHelper.PreferredUICultureInfo);
        }

        return parameter;
    }

    #endregion
}
