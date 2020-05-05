using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

/// <summary>
/// Dynamically generated filter for product variants. Checkboxes are generated from Product option categories and their options
/// HOWTO USE: 
///  - ensure CMSPage.EditedObject is set to id of variants parent product
///  - set ShowNameNumberSearch to false if you don't want to show(use) Name or Number search field
/// </summary>
public partial class CMSModules_Ecommerce_Controls_Filters_ProductVariantFilter : CMSAbstractDataFilterControl, ISimpleDataContainer
{
    #region "Variables"

    private Dictionary<int, ListItem> optionToCheckBoxMap = new Dictionary<int, ListItem>();
    private Dictionary<int, int> optionToCategoryMap = new Dictionary<int, int>();
    private Dictionary<int, Panel> categoryToPanelMap = new Dictionary<int, Panel>();
    private SKUInfo mProduct;
    private List<int> excludedCategories = new List<int>();
    private bool mShowNameNumberSearch = true;
    private bool mShowOnlyUsedCategories = true;
    private Dictionary<OptionCategoryInfo, List<SKUInfo>> externalDataSource = new Dictionary<OptionCategoryInfo, List<SKUInfo>>();

    #endregion


    #region "Public properties"

    /// <summary>
    /// External data source.
    /// </summary>
    public Dictionary<OptionCategoryInfo, List<SKUInfo>> ExternalDataSource
    {
        get
        {
            return externalDataSource;
        }
        set
        {
            externalDataSource = value;
        }
    }


    /// <summary>
    /// True if you want to show only categories already used in variants.
    /// </summary>    
    public bool ShowOnlyUsedCategories
    {
        get
        {
            return mShowOnlyUsedCategories;
        }
        set
        {
            mShowOnlyUsedCategories = value;
        }
    }


    /// <summary>
    /// True if you want to show Name or Number search field
    /// </summary>
    public bool ShowNameNumberSearch
    {
        get
        {
            return mShowNameNumberSearch;
        }
        set
        {
            mShowNameNumberSearch = value;
        }
    }


    /// <summary>
    /// Excluded categories from filter specified by ids
    /// </summary>    
    public List<int> ExcludedCategories
    {
        get
        {
            return excludedCategories;
        }
        set
        {
            excludedCategories = value;
        }
    }


    /// <summary>
    /// Gets or sets the SQL condition for filtering the order list.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetFilterWhereCondition().ToString(true);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Returns the selected option ids in dictionary with category id as a key.
    /// </summary>
    public Dictionary<int, List<int>> SelectedOptionIDs
    {
        get
        {
            return GetOptionIDs(true);
        }
    }


    /// <summary>
    /// Returns the unselected option ids in dictionary with category id as a key.
    /// </summary>
    public Dictionary<int, List<int>> UnSelectedOptionIDs
    {
        get
        {
            return GetOptionIDs(false);
        }
    }

    #endregion


    #region "Private  properties"

    /// <summary>
    /// Variant parent product
    /// </summary>
    private SKUInfo Product
    {
        get
        {
            var uiContext = UIContextHelper.GetUIContext(this);
            return mProduct ?? (mProduct = uiContext.EditedObject as SKUInfo);
        }
    }

    #endregion


    #region "Life cycle"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        btnAll.Text = GetString("UniSelector.SelectAll");
        btnNone.Text = GetString("UniSelector.DeselectAll");
        lblVariantNameNumber.Text = String.Format("{0}:", GetString("com.sku.ProductNameOrNumber"));

        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.HideFilterButton = true;

            // Reset button is available only when UniGrid remembers its state
            if (grid.RememberState)
            {
                btnReset.Text = GetString("general.reset");
                btnReset.Click += btnReset_Click;
                btnReset.Visible = true;
            }
        }

        // Initialize the Show button
        btnFilter.Text = GetString("general.search");
        btnFilter.Click += btnFilter_Click;

        // Get id of product form which variants are created from
        int productId = (Product == null) ? 0 : Product.SKUID;
        GenerateCheckboxes(productId);

        pnlVariantNameNumber.Visible = ShowNameNumberSearch;
    }


    protected void Page_Load(object sender, EventArgs e)
    { }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        foreach (KeyValuePair<int, Panel> pair in categoryToPanelMap)
        {
            pair.Value.Visible = !ExcludedCategories.Contains(pair.Key);
        }

        string script = "function Check(check) {" +
                            " $cmsj(\"#" + pnlFilterOptions.ClientID + " :input:checkbox\").prop('checked', check);" +
                        "}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Actions", ScriptHelper.GetScript(script));
    }

    #endregion


    #region "Event handlers"

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }

    #endregion


    #region "Private methods"

    private void SetAllCheckboxesTo(bool checkState)
    {
        foreach (KeyValuePair<int, ListItem> pair in optionToCheckBoxMap)
        {
            pair.Value.Selected = checkState;
        }
    }


    /// <summary>
    /// Builds a SQL condition for filtering the variant list, and returns it.
    /// </summary>
    /// <returns>A SQL condition for filtering the variant list.</returns>
    private WhereCondition GetFilterWhereCondition()
    {
        var condition = new WhereCondition();

        bool allChecked = AllCheckboxesChecked();
        string variantNameOrNumber = txtVariantNameNumber.Text;

        // If there are no options/categories in filter or all options are selected and Name-or-Number search box is empty, empty condition is returned
        if (((optionToCheckBoxMap.Keys.Count == 0) || allChecked) && (string.IsNullOrEmpty(variantNameOrNumber)))
        {
            return condition;
        }

        foreach (KeyValuePair<int, List<int>> pair in SelectedOptionIDs)
        {
            // Option ids for current category (pair.Key = current category id)
            List<int> optionIds = pair.Value;

            // If there are no selected options in category, whole category is ignored
            if (optionIds.Count > 0)
            {
                // Where condition for selected options from current category
                condition.WhereIn("SKUID", new IDQuery<VariantOptionInfo>("VariantSKUID").WhereIn("OptionSKUID", optionIds));
            }
        }

        // Variants with SKUName or Number like text in textbox field
        if (!string.IsNullOrEmpty(variantNameOrNumber))
        {
            condition.Where(w => w.WhereContains("SKUNumber", variantNameOrNumber).Or().WhereContains("SKUName", variantNameOrNumber));
        }

        // Condition is empty -> not a single option is checked -> grid will be empty
        if (condition.WhereCondition == null)
        {
            condition.NoResults();
        }

        return condition;
    }


    private bool AllCheckboxesChecked()
    {
        foreach (ListItem checkbox in optionToCheckBoxMap.Values)
        {
            if (!checkbox.Selected)
            {
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Generates the checkboxes.
    /// </summary>
    /// <param name="skuIdOfProductWithCategories">The SKU id of product with categories.</param>
    private void GenerateCheckboxes(int skuIdOfProductWithCategories)
    {
        optionToCheckBoxMap.Clear();
        optionToCategoryMap.Clear();

        Dictionary<int, string> categoriesWithName = GetCategoriesWithNames(skuIdOfProductWithCategories);

        foreach (KeyValuePair<int, string> pair in categoriesWithName)
        {
            int categoryId = pair.Key;
            string categoryName = pair.Value;

            // Ignore excluded categories
            if (ExcludedCategories.Contains(categoryId))
            {
                continue;
            }

            Dictionary<int, string> optionsWithNames = GetOptionsWithNames(skuIdOfProductWithCategories, categoryId);

            if (optionsWithNames == null || optionsWithNames.Count == 0)
            {
                continue;
            }

            var row = new Panel
            {
                CssClass = "form-group"
            };
            pnlFilterOptions.Controls.Add(row);
            // Label text set to CategoryDisplayName
            var lblCategoryDisplayName = new Label
            {
                Text = String.Format("{0}:", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(categoryName))),
                CssClass = "control-label"
            };

            var chbl = new CMSCheckBoxList
            {
                RepeatDirection = RepeatDirection.Horizontal
            };

            var cellLabel = new Panel
            {
                CssClass = "filter-form-label-cell"
            };
            var cellCheckBoxList = new Panel
            {
                CssClass = "filter-form-value-cell-wide"
            };

            cellLabel.Controls.Add(lblCategoryDisplayName);
            cellCheckBoxList.Controls.Add(chbl);

            row.Controls.Add(cellLabel);
            row.Controls.Add(cellCheckBoxList);
            // Category to table row mapping
            categoryToPanelMap.Add(categoryId, row);
            // Fill new checkboxlist with options from category
            FillCheckboxlist(optionsWithNames, chbl, categoryId);
        }
    }


    /// <summary>
    /// Gets the options with names from external data source or database.
    /// </summary>
    /// <param name="skuIdOfProductWithCategories">Id of product with categories.</param>
    /// <param name="categoryId">The category id.</param>    
    private Dictionary<int, string> GetOptionsWithNames(int skuIdOfProductWithCategories, int categoryId)
    {
        Dictionary<int, string> optionsWithName = new Dictionary<int, string>();

        if ((ExternalDataSource != null) && (ExternalDataSource.Count > 0))
        {
            // External data
            OptionCategoryInfo categoryInfo = ExternalDataSource.Keys.First(k => k.CategoryID == categoryId);
            List<SKUInfo> externalOptions = ExternalDataSource[categoryInfo];

            foreach (SKUInfo info in externalOptions)
            {
                optionsWithName.Add(info.SKUID, info.SKUName);
            }

            return optionsWithName;
        }
        else
        {
            InfoDataSet<SKUInfo> dsOptions = VariantHelper.GetEnabledOptionsWithVariantOptions(skuIdOfProductWithCategories, categoryId);

            if (!DataHelper.DataSourceIsEmpty(dsOptions))
            {
                foreach (var option in dsOptions)
                {
                    optionsWithName.Add(option.SKUID, option.SKUName);
                }
            }

            return optionsWithName;
        }
    }


    /// <summary>
    /// Gets the categories with names from external data source or database.
    /// </summary>
    /// <param name="skuIdOfProductWithCategories">Id of product with categories.</param>  
    private Dictionary<int, string> GetCategoriesWithNames(int skuIdOfProductWithCategories)
    {
        Dictionary<int, string> categoriesWithName = new Dictionary<int, string>();

        if ((ExternalDataSource != null) && (ExternalDataSource.Count > 0))
        {
            // External data
            foreach (OptionCategoryInfo info in ExternalDataSource.Keys)
            {
                // Use category live site display name in case it is available, otherwise category display name
                categoriesWithName.Add(info.CategoryID, info.CategoryTitle);
            }

            return categoriesWithName;
        }
        // Load data from DB
        DataSet dsOptionCategories;
        // If ShowOnlyUsedCategories is true, dataset will contain only already used categories in variants
        if (ShowOnlyUsedCategories)
        {
            dsOptionCategories = VariantHelper.GetProductVariantsCategories(skuIdOfProductWithCategories);
        }
        else
        {
            dsOptionCategories = OptionCategoryInfoProvider.GetProductOptionCategories(skuIdOfProductWithCategories, true, OptionCategoryTypeEnum.Attribute);
        }

        if (DataHelper.DataSourceIsEmpty(dsOptionCategories))
        {
            return categoriesWithName;
        }

        foreach (DataRow catDr in dsOptionCategories.Tables[0].Rows)
        {
            int categoryId = ValidationHelper.GetInteger(catDr["CategoryID"], 0);
            string categoryName = ValidationHelper.GetString(catDr["CategoryLiveSiteDisplayName"], String.Empty);

            if (string.IsNullOrEmpty(categoryName))
            {
                categoryName = ValidationHelper.GetString(catDr["CategoryDisplayName"], String.Empty);
            }

            categoriesWithName.Add(categoryId, categoryName);
        }

        return categoriesWithName;
    }


    /// <summary>
    /// Fills the checkboxlist.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="chbl">The CheckBoxList to fill.</param>
    /// <param name="categoryId">Id of the option category which options belong to.</param>
    private void FillCheckboxlist(Dictionary<int, string> options, CMSCheckBoxList chbl, int categoryId)
    {
        // Fill checkboxlist with checkboxes
        foreach (KeyValuePair<int, string> pair in options)
        {
            string optName = pair.Value;
            optName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(optName));
            ListItem chckbox = new ListItem(optName, "true");
            chckbox.Selected = true;
            chbl.Items.Add(chckbox);

            // Map option(SKU) id to created checkbox
            int optId = pair.Key;
            optionToCheckBoxMap.Add(optId, chckbox);
            optionToCategoryMap.Add(optId, categoryId);
        }
    }


    /// <summary>
    /// Returns the selected or unselected option ids in dictionary with category id as a key.
    /// </summary>
    /// <param name="selected">if set to true selected ids are returned.</param>    
    private Dictionary<int, List<int>> GetOptionIDs(bool selected)
    {
        Dictionary<int, List<int>> selectedOptionIds = new Dictionary<int, List<int>>();

        foreach (KeyValuePair<int, ListItem> pair in optionToCheckBoxMap)
        {
            if (pair.Value.Selected == selected)
            {
                int optionId = pair.Key;
                int categoryId = optionToCategoryMap[optionId];

                if (!selectedOptionIds.ContainsKey(categoryId))
                {
                    selectedOptionIds.Add(categoryId, new List<int>());
                }
                // Product option ids for one category
                selectedOptionIds[categoryId].Add(pair.Key);
            }
        }

        return selectedOptionIds;
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtVariantNameNumber.Text = string.Empty;
        SetAllCheckboxesTo(true);
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);

        foreach (KeyValuePair<int, ListItem> pair in optionToCheckBoxMap)
        {
            state.AddValue(pair.Key.ToString(), pair.Value.Selected);
        }
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);

        foreach (KeyValuePair<int, ListItem> pair in optionToCheckBoxMap)
        {
            bool check = state.GetBoolean(pair.Key.ToString(), true);
            pair.Value.Selected = check;
        }
    }

    #endregion


    #region "ISimpleDataContainer"

    /// <summary>
    /// Gets or sets the value of the column.
    /// </summary>
    /// <param name="columnName">Name of the column.</param>
    public object this[string columnName]
    {
        get
        {
            return GetValue(columnName);
        }
        set
        {
            SetValue(columnName, value);
        }
    }


    /// <summary>
    /// Returns value of column.
    /// </summary>
    /// <param name="columnName">Column name</param>
    public object GetValue(string columnName)
    {
        switch (columnName.ToLowerCSafe())
        {
            case "showonlyusedcategories":
                return ShowOnlyUsedCategories;
            case "shownamenumbersearch":
                return ShowNameNumberSearch;
        }

        return null;
    }


    /// <summary>
    /// Sets value of column.
    /// </summary>
    /// <param name="columnName">Column name</param>
    /// <param name="value">Column value</param>
    public bool SetValue(string columnName, object value)
    {
        switch (columnName.ToLowerCSafe())
        {
            case "showonlyusedcategories":
                ShowOnlyUsedCategories = ValidationHelper.GetBoolean(value, ShowOnlyUsedCategories);
                return true;
            case "shownamenumbersearch":
                ShowNameNumberSearch = ValidationHelper.GetBoolean(value, ShowNameNumberSearch);
                return true;
        }

        return false;
    }

    #endregion
}