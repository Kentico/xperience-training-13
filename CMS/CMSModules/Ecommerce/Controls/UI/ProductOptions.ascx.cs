using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Controls_UI_ProductOptions : CMSAdminControl
{
    #region "Variables"

    private string currentValues = string.Empty;
    private int mProductId;
    private SKUInfo mProduct;
    private bool mDisplayOnlyEnabled = true;
    bool allowedGlobalCat;
    private bool mDisplayAddSharedCategoryButton = true;
    private ObjectTransformationDataProvider countsDataProvider;

    #endregion


    #region "Properties"

    /// <summary>
    /// Product ID.
    /// </summary>
    public int ProductID
    {
        get
        {
            return mProductId;
        }
        set
        {
            mProductId = value;
            mProduct = null;
        }
    }


    /// <summary>
    /// Product info object.
    /// </summary>
    public SKUInfo Product
    {
        get
        {
            return mProduct ?? (mProduct = SKUInfo.Provider.Get(mProductId));
        }
        set
        {
            mProduct = value;

            mProductId = 0;
            if (value != null)
            {
                mProductId = value.SKUID;
            }
        }
    }


    /// <summary>
    /// Indicates whether form is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return uniSelector.Enabled;
        }
        set
        {
            uniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates whether only enabled product options are to be listed. Default value is true.
    /// </summary>
    public bool DisplayOnlyEnabled
    {
        get
        {
            return mDisplayOnlyEnabled;
        }
        set
        {
            mDisplayOnlyEnabled = value;
        }
    }


    /// <summary>
    ///  Display uniselector button. Default value is true.
    /// </summary>
    public bool DisplayAddSharedCategoryButton
    {
        get
        {
            return mDisplayAddSharedCategoryButton;
        }
        set
        {
            mDisplayAddSharedCategoryButton = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        allowedGlobalCat = ECommerceSettings.AllowGlobalProductOptions(SiteContext.CurrentSiteName);

        // Attach Event Handlers for uniGrid & uniSelector
        categoryGrid.OnExternalDataBound += categoryGrid_OnExternalDataBound;
        categoryGrid.OnAction += categoryGrid_OnAction;
        uniSelector.OnItemsSelected += (sender, args) => SaveItems();

        countsDataProvider = new ObjectTransformationDataProvider();
        countsDataProvider.SetDefaultDataHandler(GetCountsDataHandler);

        // Hide AddSharedCategoryButton
        if (!DisplayAddSharedCategoryButton)
        {
            uniSelector.DialogButton.Visible = false;
        }

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { window.location.replace(window.location.href); }"));

        btnRemove.Text = GetString("general.removeselected");
        btnRemove.OnClientClick += "return confirm(" + ScriptHelper.GetLocalizedString("com.productoptions.clearconfirm") + ");";

        // Order uniGrid by SKUCategoryOrder
        categoryGrid.OrderBy = "SKUCategoryOrder";
        categoryGrid.WhereCondition = String.Format("SKUID = {0}", ProductID);

        ReloadUniSelectorData(false);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide remove button in case unigrid does not contain any records
        btnRemove.Visible = !categoryGrid.IsEmpty;
    }

    #endregion


    #region "Event handlers"

    void categoryGrid_OnAction(string actionName, object actionArgument)
    {
        int categoryId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            case "up":
            case "down":
                // In case Up or Down action was performed permissions are checked
                RaiseOnCheckPermissions(PERMISSION_MODIFY, this);

                var selectedItem = SKUOptionCategoryInfo.Provider.Get(categoryId, ProductID);
                if (selectedItem != null)
                {
                    if (actionName.ToLowerCSafe() == "up")
                    {
                        // Move SKU Category Up
                        selectedItem.Generalized.MoveObjectUp();
                    }
                    else
                    {
                        // Move SKU Category Down
                        selectedItem.Generalized.MoveObjectDown();
                    }
                }

                // Clear selected rows
                categoryGrid.ResetSelection();

                // Reload uniGrid & uniSelector
                categoryGrid.ReloadData();

                break;
        }
    }


    object categoryGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        OptionCategoryInfo category;

        // Formatting columns
        switch (sourceName.ToLowerCSafe())
        {
            case "categorydisplayname":
                category = OptionCategoryInfo.Provider.Get(ValidationHelper.GetInteger(parameter, 0));
                return HTMLHelper.HTMLEncode(category.CategoryFullName);

            case "categorytype":
                return EnumStringRepresentationExtensions.ToEnum<OptionCategoryTypeEnum>(ValidationHelper.GetString(parameter, "")).ToLocalizedString("com.optioncategorytype");

            case "optionscounts":
                category = OptionCategoryInfo.Provider.Get(ValidationHelper.GetInteger(parameter, 0));

                if (category.CategoryType != OptionCategoryTypeEnum.Text)
                {
                    var tr = new ObjectTransformation("OptionsCounts", category.CategoryID)
                    {
                        DataProvider = countsDataProvider,
                        Transformation = "{% if(AllowAllOptions) { GetResourceString(\"general.all\") } else { FormatString(GetResourceString(\"com.ProductOptions.availableXOfY\"), SelectedOptions, AllOptions) } %}",
                        NoDataTransformation = "{$com.productoptions.nooptions$}",
                        EncodeOutput = false
                    };

                    return tr;
                }

                return "";

            case "edititem":
                category = new OptionCategoryInfo(((DataRowView)((GridViewRow)parameter).DataItem).Row);

                CMSGridActionButton btn = sender as CMSGridActionButton;

                // Disable edit button if category is global and global categories are NOT allowed
                if (btn != null)
                {
                    if (!allowedGlobalCat && category.IsGlobal)
                    {
                        btn.Enabled = false;
                    }

                    var query = QueryHelper.BuildQuery("siteId", category.CategorySiteID.ToString(), "productId", ProductID.ToString());
                    string redirectUrl = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "EditProductOptionCategory", category.CategoryID, query);
                    btn.OnClientClick = "modalDialog('" + redirectUrl + "','categoryEdit', '1000', '800');";
                }
                break;

            case "selectoptions":
                category = new OptionCategoryInfo(((DataRowView)((GridViewRow)parameter).DataItem).Row);

                CMSGridActionButton btnSelect = sender as CMSGridActionButton;

                if (btnSelect != null)
                {
                    // Disable select button if category is type of Text
                    if (category.CategoryType == OptionCategoryTypeEnum.Text)
                    {
                        btnSelect.Enabled = false;
                    }
                    else
                    {
                        var query = QueryHelper.BuildQuery("productId", ProductID.ToString());
                        // URL of allowed option selector pop-up
                        string urlSelect = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.ECOMMERCE, "ProductOptions.SelectOptions", category.CategoryID, query);

                        // Open allowed options selection dialog
                        btnSelect.OnClientClick = ScriptHelper.GetModalDialogScript(urlSelect, "selectoptions", 1000, 650);
                    }
                }
                break;
        }

        return parameter;
    }


    protected void btnRemove_Clicked(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_MODIFY, this);

        if (RemoveSKUCategoryBindings(categoryGrid.SelectedItems.Select(Int32.Parse).ToList()))
        {
            ShowChangesSaved();
        }

        ReloadGrid();
    }

    #endregion


    #region "Methods"

    private void ReloadGrid()
    {
        categoryGrid.ReloadData();
        categoryGrid.ResetSelection();
        ReloadUniSelectorData(true);
    }


    /// <summary>
    /// Removes category bindings assigned to SKU.
    /// </summary>
    /// <param name="categoryIDs">ID of categories which should be removed</param>
    /// <returns>Returns true if some category binding was removed.</returns>
    private bool RemoveSKUCategoryBindings(IEnumerable<int> categoryIDs)
    {
        var ids = categoryIDs.ToList();

        var variantCategoryIDs = VariantHelper.GetProductVariantsCategoryIDs(ProductID);

        var categoryIDsToRemove = ids.Except(variantCategoryIDs).ToList();
        foreach (var id in categoryIDsToRemove)
        {
            ProductHelper.RemoveOptionCategory(ProductID, id);
        }

        // Categories are already used in variants -> they cannot be removed
        if (ids.Except(categoryIDsToRemove).Any())
        {
            ShowWarning(GetString("com.optioncategory.remove"));
        }

        return categoryIDsToRemove.Any();
    }


    /// <summary>
    /// Returns dictionary of allowed options count. Key of the dictionary is the ID of option category.
    /// </summary>
    /// <param name="type">Object type (ignored).</param>
    /// <param name="ids">IDs of option categories which the dictionary is to be filled with.</param>
    protected SafeDictionary<int, IDataContainer> GetCountsDataHandler(string type, IEnumerable<int> ids)
    {
        DataSet countsDs = OptionCategoryInfoProvider.GetAllowedOptionsCount(ProductID, ids);

        return countsDs.ToDictionaryById("OptionCategoryID");
    }


    /// <summary>
    /// Reload data for UniSelector.
    /// </summary>
    /// <param name="forceReload">Force Reload Option</param>
    private void ReloadUniSelectorData(bool forceReload)
    {
        if (ProductID > 0)
        {
            currentValues = string.Empty;

            // Get categories assigned to product
            DataSet ds = OptionCategoryInfoProvider.GetProductOptionCategories(ProductID, false);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "CategoryID"));
            }

            uniSelector.WhereCondition = GetWhereCondition();

            if (forceReload || !RequestHelper.IsPostBack())
            {
                uniSelector.Value = currentValues;
            }

            if (forceReload)
            {
                uniSelector.Reload(true);
            }
        }
    }


    /// <summary>
    /// Saves selected item. Permission checks are performed.
    /// </summary>
    public void SaveItems()
    {
        // Check permissions
        RaiseOnCheckPermissions(PERMISSION_MODIFY, this);

        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        var displayChangesSaved = true;

        if (!String.IsNullOrEmpty(items))
        {
            var categoryIds = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();

            // Display Save info message only if some binding was removed
            displayChangesSaved = RemoveSKUCategoryBindings(categoryIds);
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            displayChangesSaved = true;
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string item in newItems)
            {
                int categoryId = ValidationHelper.GetInteger(item, 0);
                SKUOptionCategoryInfo.Provider.Add(categoryId, ProductID);
            }
        }

        // Show message & reload uniGrid
        if (displayChangesSaved)
        {
            ShowChangesSaved();
        }

        ReloadGrid();
    }


    /// <summary>
    /// Returns Where condition according to global object settings.
    /// </summary>
    protected string GetWhereCondition()
    {
        string where = "";

        if (Product != null)
        {
            // Offer global product options for global products or when using global options
            if (Product.IsGlobal)
            {
                where = "CategorySiteID IS NULL";
            }
            else
            {
                where = "CategorySiteID = " + Product.SKUSiteID;

                if (allowedGlobalCat)
                {
                    where = SqlHelper.AddWhereCondition(where, "CategorySiteID IS NULL", "OR");
                }
            }
        }

        if (DisplayOnlyEnabled)
        {
            where = SqlHelper.AddWhereCondition(where, "CategoryEnabled = 1");
        }

        // Include selected values
        if (!string.IsNullOrEmpty(currentValues))
        {
            where = SqlHelper.AddWhereCondition(where, "CategoryID IN (" + currentValues.Replace(';', ',') + ")", "OR");
        }

        return where;
    }


    /// <summary>
    /// Generates JavaScript for Add category button.
    /// </summary>
    public string GetAddCategoryJavaScript()
    {
        return "US_SelectionDialog_" + uniSelector.ClientID + "(); return false;";
    }

    #endregion
}