using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[EditedObject(OptionCategoryInfo.OBJECT_TYPE, "categoryid")]
public partial class CMSModules_Ecommerce_Controls_UI_ProductOptions_OptionCategoryEdit : CMSAdminEditControl
{
    #region "Variables"

    private SKUInfo mParentProduct;
    private bool? mCategoryHasOptions;

    // Additional action type.
    private enum AdditionalActionEnum
    {
        None = 0,
        DeleteOptionsAndCreateTextOption = 1,
        DeleteTextOption = 2,
        ConvertProductToAttribute = 3,
        ConvertAttributeToProduct = 4,
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Category SiteID.
    /// </summary>
    public int CategorySiteID
    {
        get
        {
            return ValidationHelper.GetInteger(editOptionCategory.FieldControls["CategorySiteID"].Value, 0);
        }
    }


    /// <summary>
    /// ID of product to which category should be assigned to.
    /// </summary>
    public int ProductID
    {
        get;
        set;
    }


    #endregion


    #region "Private properties"

    /// <summary>
    ///  Decides if category has options.
    /// </summary>
    private bool CategoryHasOptions
    {
        get
        {
            if (mCategoryHasOptions == null)
            {
                if (EditedCategory == null)
                {
                    mCategoryHasOptions = false;
                }
                else
                {
                    var options = SKUInfoProvider.GetSKUOptions(EditedCategory.CategoryID, false).TopN(1).Column("SKUID");
                    mCategoryHasOptions = !DataHelper.DataSourceIsEmpty(options);
                }
            }

            return mCategoryHasOptions.Value;
        }
        set
        {
            mCategoryHasOptions = value;
        }
    }


    /// <summary>
    /// Indicates type of additional action. By default any action is executed.
    /// </summary>
    private AdditionalActionEnum SelectedAdditionalAction
    {
        get;
        set;
    }


    /// <summary>
    /// Category which is being edited.
    /// </summary>
    private OptionCategoryInfo EditedCategory
    {
        get
        {
            return editOptionCategory.EditedObject as OptionCategoryInfo;
        }
    }


    /// <summary>
    /// Selected option category type in category which is being edited.
    /// </summary>
    private OptionCategoryTypeEnum SelectedCategoryType
    {
        get
        {
            return EnumStringRepresentationExtensions.ToEnum<OptionCategoryTypeEnum>(ValidationHelper.GetString(editOptionCategory.FieldControls["CategoryType"].Value, ""));
        }
    }


    /// <summary>
    /// Option category type stored in database.
    /// </summary>
    private OptionCategoryTypeEnum OriginalCategoryType
    {
        get
        {
            return EnumStringRepresentationExtensions.ToEnum<OptionCategoryTypeEnum>(ValidationHelper.GetString(editOptionCategory.Data.GetValue("CategoryType"), ""));
        }
    }


    /// <summary>
    /// Category type was changed during editing.
    /// </summary>
    private bool CategoryTypeChanged
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether control is used in editing page or in new page.
    /// </summary>
    private bool Editing
    {
        get;
        set;
    }


    /// <summary>
    /// Product to which category should be assigned to.
    /// </summary>
    private SKUInfo ParentProduct
    {
        get
        {
            return mParentProduct ?? (mParentProduct = SKUInfo.Provider.Get(ProductID));
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        editOptionCategory.OnCheckPermissions += editOptionCategory_OnCheckPermissions;
        editOptionCategory.OnBeforeSave += editOptionCategory_OnBeforeSave;
        editOptionCategory.OnAfterSave += editOptionCategory_OnAfterSave;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Editing = (EditedCategory.CategoryID != 0);

        if (Editing)
        {
            CategoryTypeChanged = (SelectedCategoryType != OriginalCategoryType);

            // Prepare confirmation message for option category type change action
            CreateConfirmationMessage();

            // Decide if additional action will be required after saving the form
            SetAdditionalActionType();
        }
    }


    /// <summary>
    /// Displays information messages over the form
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Existing item was edited
        if (Editing)
        {
            // Show information when no product options were assigned to the option category
            if (!CategoryHasOptions && (SelectedCategoryType != OptionCategoryTypeEnum.Text))
            {
                ShowInformation(GetString("optioncategory_edit.categoryEmptyOptionText.nooptionscreated"));
            }
        }

        // Show information that options can by created after saving the form
        else if (SelectedCategoryType != OptionCategoryTypeEnum.Text)
        {
            ShowInformation(GetString("com.skuoptions.notavailable"));
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Checks department permissions in case option category should be assigned to the product
    /// </summary>
    protected void editOptionCategory_OnCheckPermissions(object sender, EventArgs e)
    {
        // Raise also this control permission check
        RaiseOnCheckPermissions(string.Empty, this);
    }


    protected void editOptionCategory_OnBeforeSave(object sender, EventArgs e)
    {
        if (SelectedCategoryType == OptionCategoryTypeEnum.Text)
        {
            // Check the license
            SKUInfoProvider.CheckLicense(ObjectActionEnum.Insert);

            // CategoryDefaultRecord column is not used for text category
            EditedCategory.SetValue("CategoryDefaultRecord", null);
        }
        else
        {
            // Delete default text length validation if option category type is not text
            EditedCategory.SetValue("CategoryTextMinLength", null);
            EditedCategory.SetValue("CategoryTextMaxLength", null);
        }
    }


    protected void editOptionCategory_OnAfterSave(object sender, EventArgs e)
    {
        if (EditedCategory == null)
        {
            EditedObject = null;
            return;
        }

        // New option category
        if (!Editing)
        {
            // For new TEXT option category create text option
            if (SelectedCategoryType == OptionCategoryTypeEnum.Text)
            {
                CreateTextOption();
            }

            // Assign option category to product
            if (ParentProduct != null)
            {
                SKUOptionCategoryInfo.Provider.Add(EditedCategory.CategoryID, ParentProduct.SKUID);
            }

            // Redirect from new form dialog to option category edit.
            string query = QueryHelper.BuildQuery("saved", "1", "productid", ProductID.ToString(), "siteid", CategorySiteID.ToString());
            if (ParentProduct == null)
            {
                URLHelper.Redirect(UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "EditOptionCategory", false, EditedCategory.CategoryID, query));
            }
            else
            {
                URLHelper.Redirect(ApplicationUrlHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "EditProductOptionCategory", EditedCategory.CategoryID, query));
            }
        }

        if (Editing)
        {
            // Refresh breadcrumbs
            var append = EditedCategory.IsGlobal ? " " + GetString("general.global") : "";
            ScriptHelper.RefreshTabHeader(Page, EditedCategory.CategoryDisplayName + append);

            // Category options
            DataSet options = SKUInfoProvider.GetSKUOptions(EditedCategory.CategoryID, false);

            // Option category type may be changed during editing and additional action is required
            switch (SelectedAdditionalAction)
            {
                case AdditionalActionEnum.DeleteOptionsAndCreateTextOption:
                    DestroyOptions(options);
                    CreateTextOption();

                    break;

                case AdditionalActionEnum.DeleteTextOption:
                    DestroyOptions(options);
                    CategoryHasOptions = false;

                    break;

                case AdditionalActionEnum.ConvertAttributeToProduct:
                    ChangeAttributeToProduct(options);

                    break;

                case AdditionalActionEnum.ConvertProductToAttribute:
                    ChangeProductToAttribute(options);

                    break;

                case AdditionalActionEnum.None:

                    break;
            }

            editOptionCategory.SubmitButton.OnClientClick = "";
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates confirmation message which is displayed on Save button click.
    /// </summary>
    private void CreateConfirmationMessage()
    {
        // Category was not changed, confirmation message is not needed
        if (!CategoryTypeChanged)
        {
            return;
        }

        if (CategoryHasOptions)
        {
            switch (SelectedCategoryType)
            {
                case OptionCategoryTypeEnum.Attribute:
                    // Inform user that that data from product will be removed so it will be a valid attribute option
                    editOptionCategory.SubmitButton.OnClientClick = GetConfirmationScript("com.optioncategory.categorytypechangedtoattribute");
                    break;

                case OptionCategoryTypeEnum.Text:
                    // Inform user that attribute or product options will be deleted
                    editOptionCategory.SubmitButton.OnClientClick = GetConfirmationScript("com.optioncategory.categorytypechangedtotext");
                    break;
            }
        }

        // Inform user that he changed text category to attribute or product
        if (OriginalCategoryType == OptionCategoryTypeEnum.Text)
        {
            editOptionCategory.SubmitButton.OnClientClick = GetConfirmationScript("com.optioncategory.categorytypechangedfromtext");
        }
    }


    /// <summary>
    /// Destroys items from given data set. 
    /// </summary>
    /// <param name="options">Options to be destroyed.</param>
    private void DestroyOptions(DataSet options)
    {
        if (DataHelper.DataSourceIsEmpty(options))
        {
            return;
        }

        // Delete without ensuring the version
        using (new CMSActionContext { CreateVersion = false })
        {
            foreach (DataRow option in options.Tables[0].Rows)
            {
                SKUInfo.Provider.Delete(SKUInfo.Provider.Get(ValidationHelper.GetInteger(option["SKUID"], 0)));
            }
        }
    }


    /// <summary>
    /// Creates SKU for text option category.
    /// </summary>
    private void CreateTextOption()
    {
        // Create text product option
        var option = new SKUInfo
        {
            SKUOptionCategoryID = EditedCategory.CategoryID,
            SKUProductType = SKUProductTypeEnum.Text,
            SKUSiteID = CategorySiteID,
            SKUName = EditedCategory.CategoryTitle,
            SKUDepartmentID = 0,
            SKUPrice = 0,
            SKUNeedsShipping = false,
            SKUWeight = 0,
            SKUEnabled = true
        };

        SKUInfo.Provider.Set(option);
    }


    /// <summary>
    /// Changes SKU option from data set to attribute option. Attribute is an option and it does not need product data like for example itinerary.
    /// </summary>
    /// <param name="options">Data set with product option</param>
    private void ChangeProductToAttribute(DataSet options)
    {
        if (DataHelper.DataSourceIsEmpty(options))
        {
            return;
        }

        foreach (DataRow option in options.Tables[0].Rows)
        {
            SKUInfo product = new SKUInfo(option);

            product.SKUProductType = SKUProductTypeEnum.Product;
            product.SetValue("SKUTrackInventory", null);
            product.SetValue("SKUMaxItemsInOrder", null);
            product.SetValue("SKUMinItemsInOrder", null);
            product.SetValue("SKUAvailableItems", null);
            product.SetValue("SKUValidity", null);
            product.SetValue("SKUReorderAt", null);
            product.SetValue("SKUDescription", null);
            product.SetValue("SKUAvailableInDays", null);
            product.SetValue("SKUNumber", null);
            product.SetValue("SKUValidFor", null);
            product.SetValue("SKURetailPrice", null);
            product.SetValue("SKUEproductFilesCount", null);
            product.SKUDepartmentID = 0;
            product.SKUManufacturerID = 0;
            product.SKUBrandID = 0;
            product.SKUCollectionID = 0;
            product.SKUTaxClassID = 0;
            product.SKUInternalStatusID = 0;
            product.SKUPublicStatusID = 0;
            product.SKUSupplierID = 0;
            product.SKUImagePath = null;
            product.SKUWeight = 0;
            product.SKUWidth = 0;
            product.SKUDepth = 0;
            product.SKUHeight = 0;
            product.SKUSellOnlyAvailable = false;
            product.SKUNeedsShipping = false;
            product.SKUValidUntil = DateTimeHelper.ZERO_TIME;
            product.SKUMembershipGUID = Guid.Empty;
            product.SKUBundleInventoryType = 0;
            product.SKUParentSKUID = 0;
            product.SKUShortDescription = null;

            SKUInfo.Provider.Set(product);
        }
    }


    /// <summary>
    /// Changes attribute option from data set to SKU option and ensures correct tracking and shipping for product.
    /// </summary>
    /// <param name="options">Data set with product option</param>
    private void ChangeAttributeToProduct(DataSet options)
    {
        if (DataHelper.DataSourceIsEmpty(options))
        {
            return;
        }

        foreach (DataRow option in options.Tables[0].Rows)
        {
            SKUInfo productOption = new SKUInfo(option);
            productOption.SKUTrackInventory = TrackInventoryTypeEnum.ByProduct;
            productOption.SKUNeedsShipping = true;

            SKUInfo.Provider.Set(productOption);
        }
    }


    /// <summary>
    /// Helper method which generates confirmation script.
    /// </summary>
    /// <param name="resourceString">Resource string which is used in the confirmation message</param>
    private string GetConfirmationScript(string resourceString)
    {
        if (string.IsNullOrEmpty(resourceString))
        {
            return "";
        }

        return String.Format("return confirm('{0}');", ResHelper.GetString(resourceString));
    }


    /// <summary>
    /// Sets additional action type, i.e. what to do after option category type has changed.
    /// </summary>
    private void SetAdditionalActionType()
    {
        if (OriginalCategoryType != SelectedCategoryType)
        {
            // Products -> Attribute
            if ((OriginalCategoryType == OptionCategoryTypeEnum.Products) && (SelectedCategoryType == OptionCategoryTypeEnum.Attribute))
            {
                SelectedAdditionalAction = AdditionalActionEnum.ConvertProductToAttribute;
            }
            // Attribute -> Products
            else if ((OriginalCategoryType == OptionCategoryTypeEnum.Attribute) && (SelectedCategoryType == OptionCategoryTypeEnum.Products))
            {
                SelectedAdditionalAction = AdditionalActionEnum.ConvertAttributeToProduct;
            }
            // Attribute -> Text
            else if ((OriginalCategoryType == OptionCategoryTypeEnum.Attribute) && (SelectedCategoryType == OptionCategoryTypeEnum.Text))
            {
                SelectedAdditionalAction = AdditionalActionEnum.DeleteOptionsAndCreateTextOption;
            }
            // Product -> Text
            else if ((OriginalCategoryType == OptionCategoryTypeEnum.Products) && (SelectedCategoryType == OptionCategoryTypeEnum.Text))
            {
                SelectedAdditionalAction = AdditionalActionEnum.DeleteOptionsAndCreateTextOption;
            }
            // Text -> Product or Attribute
            else if ((OriginalCategoryType == OptionCategoryTypeEnum.Text) && (SelectedCategoryType != OriginalCategoryType))
            {
                SelectedAdditionalAction = AdditionalActionEnum.DeleteTextOption;
            }
        }
    }

    #endregion
}