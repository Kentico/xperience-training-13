using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("optioncategory_edit.itemlistlink")]
[Action(0, "OptionCategory_List.newitemcaption", "OptionCategory_New.aspx")]
[UIElement(ModuleName.ECOMMERCE, "ProductOptions")]
public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_List : CMSProductOptionCategoriesPage
{
    #region "Properties"

    /// <summary>
    /// Local page messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        OptionCategoryGrid.OnAction += OptionCategoryGrid_OnAction;
        OptionCategoryGrid.OnExternalDataBound += OptionCategoryGrid_OnExternalDataBound;
        OptionCategoryGrid.OrderBy = "CategoryDisplayName ASC";
        HandleGridsSiteIDColumn(OptionCategoryGrid);
        OptionCategoryGrid.WhereCondition = InitSiteWhereCondition("CategorySiteID").ToString(true);
    }

    #endregion


    #region "Event Handlers"

    protected object OptionCategoryGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Convert selection type to text
            case "categoryselectiontype":
                switch (ValidationHelper.GetString(parameter, "").ToLowerCSafe())
                {
                    case "dropdown":
                        return GetString("OptionCategory_List.DropDownList");

                    case "checkboxhorizontal":
                        return GetString("OptionCategory_List.checkboxhorizontal");

                    case "checkboxvertical":
                        return GetString("OptionCategory_List.checkboxvertical");

                    case "radiobuttonhorizontal":
                        return GetString("OptionCategory_List.radiobuttonhorizontal");

                    case "radiobuttonvertical":
                        return GetString("OptionCategory_List.radiobuttonvertical");

                    case "textbox":
                        return GetString("optioncategory_selectiontype.textbox");

                    case "textarea":
                        return GetString("optioncategory_selectiontype.textarea");
                }
                break;

            case "categorytype":
                return EnumStringRepresentationExtensions.ToEnum<OptionCategoryTypeEnum>(ValidationHelper.GetString(parameter, "")).ToLocalizedString("com.optioncategorytype");

            case "categorydisplayname":
                OptionCategoryInfo category = new OptionCategoryInfo(((DataRowView)parameter).Row);

                return HTMLHelper.HTMLEncode(category.CategoryFullName);
        }

        return parameter;
    }


    /// <summary>
    /// Handles the OptionCategoryGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void OptionCategoryGrid_OnAction(string actionName, object actionArgument)
    {
        int categoryId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                URLHelper.Redirect(UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "EditOptionCategory", false, categoryId));

                break;

            case "delete":

                OptionCategoryInfo categoryObj = OptionCategoryInfo.Provider.Get(categoryId);

                if (categoryObj == null)
                {
                    break;
                }

                // Check permissions
                if (!ECommerceContext.IsUserAuthorizedToModifyOptionCategory(categoryObj))
                {
                    // Check module permissions
                    if (categoryObj.CategoryIsGlobal)
                    {
                        RedirectToAccessDenied(ModuleName.ECOMMERCE, EcommercePermissions.ECOMMERCE_MODIFYGLOBAL);
                    }
                    else
                    {
                        RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyProducts");
                    }
                }

                // Check category dependencies
                if (categoryObj.Generalized.CheckDependencies())
                {
                    // Show error message
                    ShowError(EcommerceUIHelper.GetDependencyMessage(categoryObj));
                    return;
                }

                DataSet options = SKUInfoProvider.GetSKUOptions(categoryId, false);

                // Check option category options dependencies
                if (!DataHelper.DataSourceIsEmpty(options))
                {
                    // Check if some attribute option is not used in variant 
                    if (categoryObj.CategoryType == OptionCategoryTypeEnum.Attribute)
                    {
                        var optionIds = DataHelper.GetIntegerValues(options.Tables[0], "SKUID");

                        // Check if some variant is defined by this option
                        DataSet variants = VariantOptionInfo.Provider.Get()
                                                .TopN(1)
                                                .Column("VariantSKUID")
                                                .WhereIn("OptionSKUID", optionIds);

                        if (!DataHelper.DataSourceIsEmpty(variants))
                        {
                            // Option is used in some variant
                            ShowError(GetString("com.option.categoryoptiosusedinvariant"));

                            return;
                        }
                    }

                    // Check other dependencies (shopping cart, order)
                    foreach (DataRow option in options.Tables[0].Rows)
                    {
                        var skuid = ValidationHelper.GetInteger(option["SKUID"], 0);
                        var sku = SKUInfo.Provider.Get(skuid);

                        if (SKUInfoProvider.CheckDependencies(skuid))
                        {
                            // Show error message
                            ShowError(EcommerceUIHelper.GetDependencyMessage(sku));

                            return;
                        }
                    }
                }

                // Delete option category from database
                OptionCategoryInfo.Provider.Delete(categoryObj);

                break;
        }
    }

    #endregion
}