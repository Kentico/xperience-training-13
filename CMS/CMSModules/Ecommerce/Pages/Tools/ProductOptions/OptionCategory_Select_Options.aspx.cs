using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "ProductOptions.SelectOptions")]
public partial class CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Select_Options : CMSEcommerceModalPage
{
    #region "Variables and constants"

    private SKUOptionCategoryInfo optionSKUCategoryInfo;
    private List<string> mSelectedItems;
    private List<string> mOptionIdsUsedInVariant;
    private List<string> mDisabledOptionsIds;
    private const string ALLOW_ALL = "0";
    private const string SELECTED_ONLY = "1";

    #endregion


    #region "Properties"

    /// <summary>
    /// Category ID
    /// </summary>
    private int CategoryID
    {
        get
        {
            return QueryHelper.GetInteger("objectid", 0);
        }
    }


    /// <summary>
    /// Product ID
    /// </summary>
    private int ProductID
    {
        get
        {
            return QueryHelper.GetInteger("productId", 0);
        }
    }


    /// <summary>
    /// Collection of the options which are allowed for the SKU (SKUAllowedOption relationship)
    /// and options which are not allowed for specific SKU, but they are used in variant.
    /// </summary>
    private List<string> SelectedOptionsIds
    {
        get
        {
            return mSelectedItems ?? (mSelectedItems = GetAllowedOptionIds().Union(OptionIdsUsedInVariant).ToList());
        }
    }


    /// <summary>
    /// Collection of option Ids used in variants.
    /// </summary>
    private List<string> OptionIdsUsedInVariant
    {
        get
        {
            return mOptionIdsUsedInVariant ?? (mOptionIdsUsedInVariant = GetOptionIdsUsedInVariant());
        }
    }


    /// <summary>
    /// Collection of the all disabled options.
    /// </summary>
    private List<string> DisabledOptionsIds
    {
        get
        {
            return mDisabledOptionsIds ?? (mDisabledOptionsIds = GetDisabledOptionsIds());
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // SKU option category info to select options from
        optionSKUCategoryInfo = SKUOptionCategoryInfo.Provider.Get(CategoryID, ProductID);

        // Redirect user if edited option category for this product does not exist
        if (optionSKUCategoryInfo == null)
        {
            EditedObject = null;
        }
        else
        {
            // Set title and help
            var optionCategoryInfo = OptionCategoryInfo.Provider.Get(optionSKUCategoryInfo.CategoryID);
            var categoryFullName = optionCategoryInfo.CategoryFullName;
            string titleText = string.Format(GetString("com.optioncategory.select"), HTMLHelper.HTMLEncode(categoryFullName));

            SetTitle(titleText);
        }

        // Register event handlers
        rbAllowAllOption.SelectedIndexChanged += rbAllowAllOption_SelectedIndexChanged;
        ugOptions.OnExternalDataBound += ugOptions_OnExternalDataBound;
        ugOptions.OnBeforeDataReload += ugOptions_OnBeforeDataReload;

        Save += btnOk_Click;

        // Initialize value of controls
        if (!RequestHelper.IsPostBack())
        {
            rbAllowAllOption.SelectedValue = optionSKUCategoryInfo.AllowAllOptions ? ALLOW_ALL : SELECTED_ONLY;
            ugOptions.SelectedItems = SelectedOptionsIds;
        }

        // Hide selection if all options are allowed
        ugOptions.GridOptions.ShowSelection = rbAllowAllOption.SelectedValue != ALLOW_ALL;

        // Display only options for particular category
        ugOptions.WhereCondition = "SKUOptionCategoryID=" + CategoryID;

        // Navigate user if there is nothing to select from
        ugOptions.ZeroRowsText = GetString("com.selectableoptions.nodata");
    }

    #endregion


    #region "Event handlers"

    // Load allowed option in unigrid
    private void rbAllowAllOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rbAllowAllOption.SelectedValue == SELECTED_ONLY)
        {
            ugOptions.SelectedItems = SelectedOptionsIds;
            ugOptions.ReloadData();
        }
    }


    /// <summary>
    /// Change product allowed options
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        SaveChanges();

        // Close pop-up dialog
        ScriptHelper.RegisterStartupScript(this, typeof(string), "SelectOptions", "if (wopener) { wopener.location.replace(wopener.location); } CloseDialog();", true);
    }


    private object ugOptions_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView option = (DataRowView)parameter;

        switch (sourceName)
        {
            // Create formatted SKU price
            case "SKUPrice":

                var value = ValidationHelper.GetDecimal(option["SKUPrice"], 0);

                if (value.Equals(0))
                {
                    return null;
                }

                // Format price
                int siteId = ValidationHelper.GetInteger(option["SKUSiteID"], 0);
                return CurrencyInfoProvider.GetRelativelyFormattedPrice(value, siteId);

            case UniGrid.SELECTION_EXTERNAL_DATABOUND:

                string optionId = ValidationHelper.GetString(option["SKUID"], "");
                var chkBox = (CMSCheckBox)sender;

                if (OptionIdsUsedInVariant.Contains(optionId))
                {
                    // Inform user that option is used in variants a make sure it is checked 
                    chkBox.Enabled = false;
                    chkBox.Style["cursor"] = "help";

                    chkBox.ToolTip = GetString("com.optioncategory.usedinvariant");
                }
                else if (DisabledOptionsIds.Contains(optionId))
                {
                    // Inform user that option is not allowed and cannot by checked
                    chkBox.Enabled = false;
                    chkBox.Style["cursor"] = "help";

                    chkBox.ToolTip = GetString("com.optioncategory.assignonlyenabled");
                }

                break;

            case UniGrid.SELECTALL_EXTERNAL_DATABOUND:

                if (OptionIdsUsedInVariant.Any() || DisabledOptionsIds.Any())
                {
                    // Hide 'select all' checkbox to prevent removing of options used in variants or adding of disabled options
                    var chkBoxHeader = (CMSCheckBox)sender;
                    chkBoxHeader.Visible = false;
                }

                break;
        }

        return parameter;
    }


    /// <summary>
    /// Hide/rename columns in uniGrid before loading data.
    /// </summary>
    void ugOptions_OnBeforeDataReload()
    {
        // Hide price adjustment if category is used in product variants
        if (VariantHelper.AreCategoriesUsedInVariants(ProductID, new[] { CategoryID }))
        {
            ugOptions.NamedColumns["SKUPrice"].Visible = false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets collection of the options from specific category which are allowed for the SKU (SKUAllowedOption relationship).
    /// </summary>
    /// <returns>Collection of allowed option Ids for the SKU.</returns>
    private List<string> GetAllowedOptionIds()
    {
        var result = SKUAllowedOptionInfo.Provider.Get().Source(s => s.Join<SKUInfo>("OptionSKUID", "COM_SKU.SKUID"))
                                                                         .WhereEquals("SKUOptionCategoryID", optionSKUCategoryInfo.CategoryID)
                                                                         .WhereEquals("COM_SKUAllowedOption.SKUID", ProductID)
                                                                         .Column("OptionSKUID")
                                                                         .OrderBy("COM_SKU.SKUID")
                                                                         .ToList();

        return result.Select(o => o.OptionSKUID.ToString()).ToList();
    }


    /// <summary>
    /// Gets all options used in variant.
    /// </summary>
    /// <returns>Collection of option Ids used in variants.</returns>
    private List<string> GetOptionIdsUsedInVariant()
    {
        var optionsInCategory = SKUInfoProvider.GetSKUOptions(optionSKUCategoryInfo.CategoryID, false).Column("SKUID");
        var optionsInVariants = VariantOptionInfo.Provider.Get()
                                                      .WhereIn("VariantSKUID", VariantHelper.GetVariants(ProductID).Column("SKUID"))
                                                      .Column("OptionSKUID");
   

        // Check if some variant was created from the option
        return optionsInCategory.WhereIn("SKUID", optionsInVariants).ToList()
            .Select(o => o.SKUID.ToString())
            .ToList();
    }


    /// <summary>
    /// Gets all disabled options.
    /// </summary>
    /// <returns>Collection of the all disabled option.</returns>
    private List<string> GetDisabledOptionsIds()
    {
        var query = SKUInfoProvider.GetSKUOptions(optionSKUCategoryInfo.CategoryID, false)
                        .WhereFalse("SKUEnabled")
                        .Column("SKUID")
                        .ToList();

        return query.Select(x => x.SKUID.ToString()).ToList();
    }


    /// <summary>
    /// Save changes in product allowed options
    /// </summary>
    private void SaveChanges()
    {
        SKUInfo product = SKUInfo.Provider.Get(ProductID);

        if (product == null)
        {
            return;
        }

        // Check if user is authorized to modify product
        if (!ECommerceContext.IsUserAuthorizedToModifySKU(product))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, product.IsGlobal ? EcommercePermissions.ECOMMERCE_MODIFYGLOBAL : "EcommerceModify OR ModifyProducts");

            return;
        }

        // Allow all option is enabled
        if (rbAllowAllOption.SelectedValue == ALLOW_ALL)
        {
            ProductHelper.AllowAllOptions(ProductID, CategoryID);
        }
        else
        {
            // Join users selection with previously selected items and options used in variants + remove deselected items
            var optionsToBeAllowedArray = ugOptions.SelectedItems.Union(SelectedOptionsIds).Except(ugOptions.DeselectedItems).ToArray();
            var optionsToBeAllowed = ValidationHelper.GetIntegers(optionsToBeAllowedArray, 0);
            var optionsToBeRemoved = ValidationHelper.GetIntegers(ugOptions.DeselectedItems.ToArray(), 0);

            // Add selected options to product
            if (optionsToBeAllowed.Count() > 0)
            {
                ProductHelper.AllowOptions(ProductID, CategoryID, optionsToBeAllowed);
            }
            // Disable allow all options if any option was deselected
            else
            {
                optionSKUCategoryInfo.AllowAllOptions = false;
                optionSKUCategoryInfo.Update();
            }

            // Remove deselected options from product
            if (optionsToBeRemoved.Count() > 0)
            {
                ProductHelper.RemoveOptions(ProductID, CategoryID, optionsToBeRemoved);
            }
        }
    }

    #endregion
}
