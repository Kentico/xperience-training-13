using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject(OptionCategoryInfo.OBJECT_TYPE, "categoryId")]
[Title("OptionCategory_List.NewItemCaption")]
[UIElement(ModuleName.ECOMMERCE, "ProductOptions")]
public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_New : CMSProductOptionCategoriesPage
{
    #region "Variables"

    private int productID = QueryHelper.GetInteger("productid", 0);

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize editing control
        OptionCategoryEditElem.OnCheckPermissions += OptionCategoryEditElem_OnCheckPermissions;
        OptionCategoryEditElem.ProductID = productID;

        PageTitle.IsDialog = QueryHelper.GetBoolean("dialog", false);

        CreateBreadcrumbs();
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Event handler to check permissions.
    /// </summary>
    /// <param name="permissionType"></param>
    /// <param name="sender"></param>
    protected void OptionCategoryEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        CheckPermissions();
    }

    #endregion


    #region "Methods"

    private void CheckPermissions()
    {
        // Check module permissions
        bool globalCategory = (OptionCategoryEditElem.CategorySiteID <= 0);

        if (productID > 0)
        {
            var skuInfo = SKUInfo.Provider.Get(productID);
            bool globalSKU = skuInfo.IsGlobal;

            // Check product permissions
            if (!ECommerceContext.IsUserAuthorizedToModifySKU(globalSKU))
            {
                if (globalSKU)
                {
                    RedirectToAccessDenied(ModuleName.ECOMMERCE, EcommercePermissions.ECOMMERCE_MODIFYGLOBAL);
                }
                else
                {
                    RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyProducts");
                }
            }
        }

        // Check product option category permissions
        if (!ECommerceContext.IsUserAuthorizedToModifyOptionCategory(globalCategory))
        {
            if (globalCategory)
            {
                RedirectToAccessDenied(ModuleName.ECOMMERCE, EcommercePermissions.ECOMMERCE_MODIFYGLOBAL);
            }
            else
            {
                RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyProducts");
            }
        }
    }


    /// <summary>
    /// Creates breadcrumbs.
    /// </summary>
    private void CreateBreadcrumbs()
    {
        string breadcrumbName = "";

        var optionCategory = EditedObject as OptionCategoryInfo;
        if ((optionCategory != null) && (optionCategory.CategoryID > 0))
        {
            breadcrumbName = FormatBreadcrumbObjectName(optionCategory.CategoryDisplayName, ConfiguredSiteID);
        }
        else
        {
            breadcrumbName = FormatBreadcrumbObjectName(GetString("optioncategory_list.newitemcaption"), ConfiguredSiteID);
        }

        // Update breadcrumb
        SetBreadcrumb(0, GetString("optioncategory_edit.itemlistlink"), GenerateBreadCrumbUrl(), null, null);
        SetBreadcrumb(1, breadcrumbName, null, null, null);
    }


    /// <summary>
    /// Builds breadcrumb URL.
    /// </summary>
    private string GenerateBreadCrumbUrl()
    {
        string returnUrl;

        if (QueryHelper.Contains("productid"))
        {
            returnUrl = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_Options.aspx");
            returnUrl = URLHelper.AddParameterToUrl(returnUrl, "nodeid", NodeID.ToString());
            returnUrl = URLHelper.AddParameterToUrl(returnUrl, "documentid", DocumentID.ToString());
            returnUrl = URLHelper.AddParameterToUrl(returnUrl, "productid", HTMLHelper.HTMLEncode(QueryHelper.GetString("productid", string.Empty)));
            returnUrl = URLHelper.AddParameterToUrl(returnUrl, "dialog", QueryHelper.GetString("dialog", "0"));
        }
        else
        {
            returnUrl = "~/CMSModules/Ecommerce/Pages/Tools/ProductOptions/OptionCategory_List.aspx";
        }

        return returnUrl;
    }

    #endregion
}