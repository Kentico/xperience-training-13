using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[EditedObject(SKUInfo.OBJECT_TYPE_VARIANT, "variantId")]
[ParentObject(SKUInfo.OBJECT_TYPE_SKU, "productId")]
[Title("com.products.variantproperties")]
[Breadcrumb(0, "com.products.variants", "Product_Edit_Variants.aspx?productId={%EditedObjectParent.ID%}&dialog={?dialog?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[UIElement(ModuleName.ECOMMERCE, "Products.Variants")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Variant_Edit : CMSProductsPage
{
    #region "Properties"

    private SKUInfo Variant
    {
        get
        {
            return EditedObject as SKUInfo;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        editForm.OnBeforeDataLoad += editForm_OnBeforeDataLoad;
        editForm.OnCheckPermissions += editForm_OnCheckPermissions;
        editForm.OnGetControlValue += editForm_OnGetControlValue;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Get label for info text field
        LocalizedLabel infoTextLabel = editForm.FieldLabels["InfoText"];

        // Set label to render without colon. It is used for info message through whole form width.
        if (infoTextLabel != null)
        {
            infoTextLabel.DisplayColon = false;
        }
    }


    /// <summary>
    /// Checks if current user is authorized to modify product variant.
    /// </summary>
    protected void editForm_OnCheckPermissions(object sender, EventArgs e)
    {
        if (Variant?.Parent != null)
        {
            // Check parent product permissions
            CheckProductModifyAndRedirect((SKUInfo)Variant.Parent);
        }
    }


    protected void editForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        if ((Variant == null) || !Variant.IsProductVariant)
        {
            EditedObject = null;
            return;
        }

        CheckEditedObjectSiteID(Variant.SKUSiteID);

        var optionsList = SKUInfo.Provider.Get()
              .WhereIn("SKUID",
                  VariantOptionInfo.Provider.Get()
                    .Column("OptionSKUID")
                    .WhereEquals("VariantSKUID", Variant.SKUID)
               )
               .ToList();

        SetVariantAttributes(optionsList);
    }


    private void editForm_OnGetControlValue(object sender, FormEngineUserControlEventArgs e)
    {
        if (e.ColumnName.Equals("SKUCollectionID", StringComparison.OrdinalIgnoreCase))
        {
            var value = ValidationHelper.GetInteger(e.Value, 0);
            if (value == 0)
            {
                // Reset value if "(none)" option with value "0" was selected
                e.Value = null;
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates category with variant attributes.
    /// </summary>
    /// <param name="optionsList">Product options</param>
    private void SetVariantAttributes(List<SKUInfo> optionsList)
    {
        // Get attributes category index - just before representing category
        var attrPos = editForm.FormInformation.ItemsList.FindIndex(f =>
            (f is FormCategoryInfo) && ((FormCategoryInfo)f).CategoryName.Equals("com.sku.representingcategory", StringComparison.OrdinalIgnoreCase));

        // Create attributes category
        var attCategory = new FormCategoryInfo
        {
            CategoryName = "Attributes",
            IsDummy = true,
        };

        attCategory.SetPropertyValue(FormCategoryPropertyEnum.Caption, HTMLHelper.HTMLEncode(GetString("com.variant.attributes")));
        editForm.FormInformation.AddFormItem(attCategory, attrPos++);

        // Get variant categories
        var variantCategories = VariantHelper.GetProductVariantsCategories(ProductID);

        foreach (var category in variantCategories)
        {
            var option = optionsList.FirstOrDefault(o => o.SKUOptionCategoryID == category.CategoryID);
            if (option != null)
            {
                var ffOption = new FormFieldInfo
                {
                    Name = category.CategoryName,
                    AllowEmpty = true,
                    Size = 400,
                    DataType = FieldDataType.Text,
                    IsDummyField = true,
                };

                ffOption.SetControlName(FormFieldControlName.LABEL);
                ffOption.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(option.SKUName)));

                // Show category live site display name instead of category display name in case it is available
                ffOption.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(category.CategoryTitle)));

                //Insert field to the form on specified position
                editForm.FormInformation.AddFormItem(ffOption, attrPos++);
            }
        }
    }

    #endregion
}