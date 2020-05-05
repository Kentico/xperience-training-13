using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject("ecommerce.optioncategory", "categoryId")]
public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Edit_General : CMSProductOptionCategoriesPage
{
    #region "Page Events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        // Get parent product from url
        var parentProductId = QueryHelper.GetInteger("productId", 0);
        // Check UI permissions
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, (parentProductId <= 0) ? "ProductOptions.General" : "Products.ProductOptions.General");

        var categoryObject = EditedObject as BaseInfo;
        if (categoryObject != null)
        {
            CheckEditedObjectSiteID(categoryObject.Generalized.ObjectSiteID);
        }
        // Register event handler to check permissions
        OptionCategoryEditElem.OnCheckPermissions += OptionCategoryEditElem_OnCheckPermissions;
    }


    /// <summary>
    /// Event handler to check permissions.
    /// </summary>
    protected void OptionCategoryEditElem_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        var categoryObject = EditedObject as BaseInfo;
        var global = (categoryObject != null) && categoryObject.IsGlobal;
        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedToModifyOptionCategory(global))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, global ? EcommercePermissions.ECOMMERCE_MODIFYGLOBAL : "EcommerceModify OR ModifyProducts");
        }
    }

    #endregion
}