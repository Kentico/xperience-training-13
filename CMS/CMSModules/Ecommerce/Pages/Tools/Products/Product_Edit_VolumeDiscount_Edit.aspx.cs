using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

// Edited object
[EditedObject(VolumeDiscountInfo.OBJECT_TYPE, "volumediscountid")]
[ParentObject(SKUInfo.OBJECT_TYPE_SKU, "productId")]

// Breadcrumbs
[Breadcrumb(0, "Product_Edit_VolumeDiscount_Edit.ItemListLink", "Product_Edit_VolumeDiscount_List.aspx?productId={?productId?}&siteId={?siteId?}&nodeId={?nodeId?}&dialog={?dialog?}", null)]
[Breadcrumb(1, "product_edit_volumediscount_edit.edittitletext", ExistingObject = true)]
[Breadcrumb(1, "Product_Edit_VolumeDiscount_Edit.NewItemCaption", NewObject = true)]

// Security
[UIElement(ModuleName.ECOMMERCE, "Products.VolumeDiscounts")]

// Title
[Title("product_edit_volumediscount_edit.edittitletext", ExistingObject = true)]
[Title("product_edit_volumediscount_edit.newitemcaption", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_VolumeDiscount_Edit : CMSProductsPage
{
    #region "Properties"

    /// <summary>
    /// Product which volume discount belongs to.
    /// </summary>
    protected SKUInfo Product
    {
        get
        {
            return EditedObjectParent as SKUInfo;
        }
    }


    /// <summary>
    /// The volume discount which is being edited.
    /// </summary>
    private VolumeDiscountInfo EditedDiscount
    {
        get
        {
            return EditForm.EditedObject as VolumeDiscountInfo;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        EnsureDocumentManager = false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if ((Product == null) || ((EditedDiscount.VolumeDiscountID > 0) && (EditedDiscount.VolumeDiscountSKUID != Product.SKUID)))
        {
            EditedObject = null;
            return;        
        }

        // Register handlers
        EditForm.OnCheckPermissions += EditForm_OnCheckPermissions;
        EditForm.OnItemValidation += EditForm_OnItemValidation;

        CheckEditedObjectSiteID(Product.SKUSiteID);

        // Display product price
        headProductPriceInfo.Text = string.Format(GetString("com_sku_volume_discount.skupricelabel"), CurrencyInfoProvider.GetFormattedPrice(Product.SKUPrice, Product.SKUSiteID));

    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Checks permissions and redirects to access denied page if access is denied
    /// </summary>
    private void EditForm_OnCheckPermissions(object sender, EventArgs args)
    {
        // Check module permissions
        CheckProductModifyAndRedirect(Product);
    }


    protected void EditForm_OnItemValidation(object sender, ref string errorMessage)
    {
        // Look for record with the same minimal amount
        FormEngineUserControl ctrl = sender as FormEngineUserControl;

        if ((ctrl != null) && (ctrl.FieldInfo.Name == "VolumeDiscountMinCount"))
        {
            int mincount = ValidationHelper.GetInteger(ctrl.Value, 0);

            VolumeDiscountInfo discount = VolumeDiscountInfoProvider.GetVolumeDiscountInfo(Product.SKUID, mincount);

            if ((discount != null) && (mincount == discount.VolumeDiscountMinCount) && (EditedDiscount.VolumeDiscountID != discount.VolumeDiscountID))
            {
                errorMessage = GetString("product_edit_volumediscount_edit.minamountexists");
                EditForm.StopProcessing = false;
            }
        }
    }

    #endregion
}