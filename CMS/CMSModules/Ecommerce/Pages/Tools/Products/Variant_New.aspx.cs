using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DataEngine.CollectionExtensions;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.EventLog;
using CMS.Helpers;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;

// Set title
[Title("com.products.newvariant")]

// Set breadcrumbs
[Breadcrumb(0, "com.products.variants", "Product_Edit_Variants.aspx?productId={%EditedObject.ID%}&dialog={?dialog?}", null)]
[Breadcrumb(1, "com.products.newvariant")]

// Set parent and edited object
[EditedObject(SKUInfo.OBJECT_TYPE_SKU, "productId")]

// Set security
[UIElement(ModuleName.ECOMMERCE, "Products.Variants")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Variant_New : CMSProductsPage
{
    #region "Variables"

    private List<Column> mColumnDefinitions;
    private List<string> mExistingItems;

    private DataSet mGeneratedVariants;
    private List<Tuple<OptionCategoryInfo, List<SKUInfo>>> mAllCategoriesOptions;
    private List<Tuple<OptionCategoryInfo, List<SKUInfo>>> mVariantCategoriesOptions;

    private bool mRegenerateVariants;

    private List<DataRow> mVariantsToGenerate;

    #endregion


    #region "Properties"

    /// <summary>
    /// List of categories ID.
    /// </summary>
    private List<int> ExistingCategories
    {
        get
        {
            if (!(ViewState["ExistingCategoriesList"] is List<int>))
            {
                ViewState["ExistingCategoriesList"] = new List<int>();
            }
            return (List<int>)ViewState["ExistingCategoriesList"];
        }
    }


    /// <summary>
    /// Local page messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    /// <summary>
    /// List of existing items.
    /// </summary>
    private List<string> ExistingItems
    {
        get
        {
            return mExistingItems ?? (mExistingItems = GetExistingItems());
        }
        set
        {
            mExistingItems = value;
        }
    }


    /// <summary>
    /// List of categories ID.
    /// </summary>
    private List<int> NewCategories
    {
        get
        {
            if (!(ViewState["NewCategoriesList"] is List<int>))
            {
                ViewState["NewCategoriesList"] = new List<int>();
            }
            return (List<int>)ViewState["NewCategoriesList"];
        }
    }


    /// <summary>
    /// Indicates if filter is shown.
    /// </summary>
    private bool ShowFilter
    {
        get
        {
            if (ViewState["DisplayFilter"] is bool)
            {
                return (bool)ViewState["DisplayFilter"];
            }

            return false;
        }
        set
        {
            ViewState["DisplayFilter"] = value;
        }
    }


    /// <summary>
    /// Current Warning.
    /// </summary>
    private string CurrentWarning
    {
        get
        {
            return ctlAsyncLog.ProcessData.Warning;
        }
        set
        {
            ctlAsyncLog.ProcessData.Warning = value;
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }

    #endregion


    #region "Page Events"

    /// <summary>
    /// Overrides OnPreInit method.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Initialize creation of a new variants
        GlobalObjectsKeyName = ECommerceSettings.ALLOW_GLOBAL_PRODUCTS;

        // Check if product belongs to current site
        var product = EditedObject as SKUInfo;
        if (product != null)
        {
            CheckEditedObjectSiteID(product.SKUSiteID);
        }

        mAllCategoriesOptions = new List<Tuple<OptionCategoryInfo, List<SKUInfo>>>();
        mVariantCategoriesOptions = new List<Tuple<OptionCategoryInfo, List<SKUInfo>>>();

        // Get all enabled product option attribute categories plus option categories used in variants
        DataSet allCategoriesDS = VariantHelper.GetUsedProductOptionCategories(ProductID, OptionCategoryTypeEnum.Attribute);

        // Get all product options categories which are already in variants
        DataSet variantCategoriesDS = VariantHelper.GetProductVariantsCategories(ProductID);

        // Fill categories lists
        FillCategoriesOptions(mAllCategoriesOptions, allCategoriesDS);
        FillCategoriesOptions(mVariantCategoriesOptions, variantCategoriesDS);

        // Pass data to controls
        VariantFilter.ExternalDataSource = CategorySelector.AllCategoriesOptions = mAllCategoriesOptions.ToDictionary(c => c.Item1, c => c.Item2); ;
        CategorySelector.VariantCategoriesOptions = mVariantCategoriesOptions.ToDictionary(c => c.Item1, c => c.Item2); ;
    }


    /// <summary>
    /// Overrides OnInit method.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Assign save action to the save button in header actions
        HeaderActions.AddAction(new SaveAction());
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        CategorySelector.ProductID = ProductID;
        CategorySelector.OnSelectionChanged += CategorySelector_OnSelectionChanged;

        // Get list of existing categories
        GetListOfAlreadyCreatedVariants(ExistingCategories);

        VariantGrid.DelayedReload = true;
        VariantGrid.OnLoadColumns += VariantGrid_OnLoadColumns;

        // Disable pager and display all records
        VariantGrid.PageSize = "##ALL##";
        VariantGrid.PagerConfig = new UniGridPagerConfig { DisplayPager = false };
        VariantGrid.GridView.AllowPaging = false;
        VariantGrid.ApplyPageSize = false;
        VariantGrid.RememberStateByParam = "";
        VariantGrid.Pager.DisplayPager = false;
    }


    /// <summary>
    /// Overrides OnLoad method.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Setup and configure asynchronous control
        SetupAsyncControl();

        // Force no suffix for breadcrumbs
        UIHelper.SetBreadcrumbsSuffix("");
    }


    /// <summary>
    /// Overrides OnLoadCompleted method.
    /// </summary>
    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);

        if (!RequestHelper.IsPostBack())
        {
            // Show error message
            if (QueryHelper.Contains("error"))
            {
                ShowError(HTMLHelper.HTMLEncode(QueryHelper.GetString("error", string.Empty)));
            }
            // Show warning message
            else if (QueryHelper.Contains("warning"))
            {
                ShowWarning(HTMLHelper.HTMLEncode(QueryHelper.GetString("warning", string.Empty)));
            }
        }

        // Setup variants if at least one category is assigned for generation
        if ((NewCategories.Count > 0) || (ExistingCategories.Count > 0))
        {
            SetupVariants();
        }

        // Exclude not selected categories
        VariantFilter.ExcludedCategories = CategorySelector.UnSelectedCategories.ToList();
        VariantFilter.ReloadData(true);
    }


    /// <summary>
    /// Overrides OnPreRender method.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Apply filter
        if (!DataHelper.DataSourceIsEmpty(mGeneratedVariants))
        {
            string query = string.Empty;
            foreach (int categoryId in VariantFilter.SelectedOptionIDs.Keys.ToList())
            {
                // Build query from selected options Ids
                if (mGeneratedVariants.Tables[0].Columns[categoryId.ToString()] != null)
                {
                    // Build query where column named categoryId satisfies conditions
                    query = SqlHelper.AddWhereCondition(query, ("[" + categoryId + "] IN (" + TextHelper.Join(",", VariantFilter.SelectedOptionIDs[categoryId]) + ")"));
                    query = SqlHelper.AddWhereCondition(query, ("[" + categoryId + "] IS NULL"), "OR");
                }
            }

            if (query != string.Empty)
            {
                // Apply query, save result, clear dataTable and put result to daTable
                DataTable dataTable = mGeneratedVariants.Tables[0].Clone();
                mGeneratedVariants.Tables[0].Select(query, "RowNumber").CopyToDataTable(dataTable, LoadOption.OverwriteChanges);
                mGeneratedVariants.Tables.Remove("VariantsDT");
                mGeneratedVariants.Tables.Add(dataTable);
            }
            else
            {
                // No ids selected, clear grid
                mGeneratedVariants.Tables.Remove("VariantsDT");
            }

            SetupVariants(false);
        }

        ManipulateFilter();

        VariantGrid.Visible = (VariantGrid.RowsCount <= 500);
        lblSpecify.Visible = !VariantGrid.Visible;
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Fires when on header action buttons was clicked.
    /// </summary>
    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            // Save variant
            case "save":

                // Check permissions - Modify SKU, Authorized for department
                CheckPermissions(EditedObject as SKUInfo);

                // Check if there are some categories for generation
                if ((NewCategories.Count > 0) || (ExistingCategories.Count > 0))
                {
                    mGeneratedVariants = GenerateVariants();

                    // Get selected rows from UniGrid in DataSet
                    var selectedItems = VariantGrid.SelectedItems.ToHashSetCollection();

                    var selectedRows = from row in mGeneratedVariants.Tables[0].AsEnumerable()
                                       where selectedItems.Contains(
                                       ValidationHelper.GetString(row.Field<int>("RowNumber"), string.Empty))
                                       select row;

                    bool editingExisting = selectedRows.Any(x => ValidationHelper.GetBoolean(x["Exist"], true));

                    // Generator is editing existing variant only in case new category (not used until now) was used to generate variant
                    editingExisting &= (NewCategories.Count > 0);

                    // If manipulating existing variants, regenerate them with new option categories assigned
                    if (editingExisting)
                    {
                        // Check if all new assigned categories have default option selected
                        if (CategorySelector.SelectedCategories.Values.Any(x => x == VariantOptionInfo.ExistingUnselectedOption))
                        {
                            ShowError(GetString("com.variants.nodefaultoption"));
                            SetupVariants(false);
                            return;
                        }
                        mRegenerateVariants = true;
                    }

                    mVariantsToGenerate = (selectedRows.Where(x => ValidationHelper.GetBoolean(x["Exist"], true) == false)).ToList();

                    // Show warning to select some variants in case any new category was selected
                    if (((!mVariantsToGenerate.Any()) && (ExistingCategories.Count > 0) && (NewCategories.Count == 0)) || (selectedItems.Count == 0))
                    {
                        ShowWarning(GetString("com.variants.selectsomevariants"));
                        SetupVariants(false);
                        return;
                    }
                    // Show warning to select some new variants in case user selected new category checkbox.
                    // It is possible that user cannot select any new variant after new category option is being added to the variant, in this case error message is not required.
                    if ((NewCategories.Count == 0)
                              && (!mVariantsToGenerate.Any())
                              && (CategorySelector.SelectedCategories.Values.All(x => x == VariantOptionInfo.ExistingUnselectedOption)))
                    {
                        ShowWarning(GetString("com.variants.selectsomeothervariants"));
                        SetupVariants(false);
                        return;
                    }

                    // Save variants
                    EnsureAsyncLog();
                    RunAsync(GenerateAndSave);
                    return;
                }

                ShowWarning(GetString("com.variants.selectsomecategories"));
                break;
        }
    }


    /// <summary>
    /// Dynamically adds columns to UniGrid.
    /// </summary>
    protected void VariantGrid_OnLoadColumns()
    {
        // If UniGrid has some data, load columns
        if (!DataHelper.DataSourceIsEmpty(mGeneratedVariants))
        {
            VariantGrid.GridColumns.Columns = mColumnDefinitions;

            // Add last column with 100% width
            VariantGrid.GridColumns.Columns.Add(new Column { Width = "100%" });
        }
    }


    /// <summary>
    /// Resets List of new categories and assign to it new values.
    /// </summary>
    protected void CategorySelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Clear list of new categories
        NewCategories.Clear();

        // Get selected categories from all available categories list to preserve right order
        var orderedCategories = mAllCategoriesOptions.Where(c => CategorySelector.SelectedCategories.Keys.Any(cID => c.Item1.CategoryID == cID));

        foreach (int categoryId in orderedCategories.Select(c => c.Item1.CategoryID))
        {
            if (CategorySelector.SelectedCategories[categoryId] != VariantOptionInfo.ExistingUnselectedOption)
            {
                NewCategories.Add(categoryId);
            }
        }

        // Reset selected items in UniGrid
        VariantGrid.SelectedItems = new List<string>();
    }


    /// <summary>
    /// Fires, when show / hide filter button was clicked on.
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        ShowFilter = !ShowFilter;
        ManipulateFilter();
    }


    /// <summary>
    /// External unigrid databound.
    /// </summary>
    protected object VariantGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView row = (DataRowView)parameter;

        switch (sourceName)
        {
            case UniGrid.SELECTION_EXTERNAL_DATABOUND:

                // Disable checkbox if variant already exists
                if (ExistingItems.Contains(ValidationHelper.GetString(row["RowNumber"], string.Empty)))
                {
                    CMSCheckBox chkBox = (CMSCheckBox)sender;
                    chkBox.Enabled = false;
                }

                break;
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Generates variants into DataSet and DataTable.
    /// </summary>
    /// <returns>DataSet of generated variants</returns>
    private DataSet GenerateVariants()
    {
        List<ProductVariant> productVariantList;

        // Creating new variants and some has been already generated
        if ((NewCategories.Count > 0) && (ExistingCategories.Count > 0))
        {
            ProductAttributeSet productAttributeSet = new ProductAttributeSet(CategorySelector.SelectedCategories.Values.Where(x => x > VariantOptionInfo.NewOption));
            List<ProductVariant> oldVariants = VariantHelper.AddNewCategoriesToVariantsOfProduct(ProductID, productAttributeSet);
            productVariantList = VariantHelper.GetAllPossibleVariants(oldVariants);
        }
        else
        {
            productVariantList = VariantHelper.GetAllPossibleVariants(ProductID, NewCategories.Concat(ExistingCategories));
        }

        DataSet ds = new DataSet("VariantsDS");
        DataTable dt = new DataTable("VariantsDT");

        // Combine new and existing categories in right order
        var combinedCategories = ExistingCategories.Union(NewCategories).OrderBy(cID => mAllCategoriesOptions.FindIndex(ca => ca.Item1.CategoryID == cID));

        // Build DataTable and UniGrid structure 
        foreach (int categoryId in combinedCategories)
        {
            var optionCategory = mAllCategoriesOptions.FirstOrDefault(k => k.Item1.CategoryID == categoryId);

            // Add columns to DataTable
            dt.Columns.Add(new DataColumn(categoryId.ToString(), typeof(String)));

            // Add column with option category live site display name to the grid with variant preview. In case live site display name is not available option category display name is used
            if (optionCategory != null)
            {
                AddGridColumn(ResHelper.LocalizeString(optionCategory.Item1.CategoryTitle), categoryId.ToString(), "#transform: ecommerce.skuoption.SKUName");
            }
        }

        // Add RowNumber column to DataTable
        dt.Columns.Add(new DataColumn("RowNumber", typeof(int)));
        AddGridColumn("RowNumber", "RowNumber", string.Empty, true);

        // Add Exist column to DataTable
        dt.Columns.Add(new DataColumn("Exist", typeof(bool)));

        // Add Variant number column to DataTable and UniGrid
        dt.Columns.Add(new DataColumn("VariantNumber", typeof(string)));
        AddGridColumn(GetString("com.sku.skunumber"), "VariantNumber");

        // Fill DataTable
        int index = 0;
        foreach (ProductVariant productVariant in productVariantList)
        {
            DataRow dr = dt.NewRow();

            int i = 0;
            foreach (int categoryId in productVariant.ProductAttributes.CategoryIDs)
            {
                // Fill values to dynamically added column
                var optionCategory = mAllCategoriesOptions.FirstOrDefault(k => k.Item1.CategoryID == categoryId);

                if (optionCategory != null)
                {
                    dr[optionCategory.Item1.CategoryID.ToString()] = productVariant.ProductAttributes[i].SKUID;
                    i++;
                }
            }

            dr["RowNumber"] = index;
            dr["Exist"] = productVariant.Existing;
            dr["VariantNumber"] = productVariant.Variant.SKUNumber;

            dt.Rows.Add(dr);
            index++;
        }

        ds.Tables.Add(dt);
        return ds;
    }


    /// <summary>
    /// Generates variants, builds UniGrid and binds data to it.
    /// </summary>
    private void SetupVariants(bool regenerate = true)
    {
        // Do not setup variants, when callback is requesting page
        if (RequestHelper.IsCallback())
        {
            return;
        }

        // Limit maximum variants count 
        const int maxVariantsCount = 1000;
        if (VariantHelper.GetAllPossibleVariantsCount(ProductID, CategorySelector.SelectedCategories.Keys) > maxVariantsCount)
        {
            ShowWarning(GetString("com.variants.variantlimit"));
            return;
        }

        if (regenerate)
        {
            // Generate Variants
            mGeneratedVariants = GenerateVariants();
            ExistingItems = null;
        }

        // Set UniGrid columns and other properties
        VariantGrid.GridColumns.Columns = mColumnDefinitions;
        VariantGrid.GridOptions.DisplayFilter = false;
        VariantGrid.GridOptions.ShowSelection = true;
        VariantGrid.GridOptions.SelectionColumn = "RowNumber";

        var existingItems = ExistingItems;

        VariantGrid.SelectedItems = ((existingItems != null) && (existingItems.Count > 0)) ? existingItems : VariantGrid.SelectedItems;

        // Bind data to UniGrid
        VariantGrid.DataSource = mGeneratedVariants;
        VariantGrid.ReloadData();
    }


    /// <summary>
    /// Gets list of existing categories.
    /// </summary>
    /// <param name="existingCategories">List of CategoryIds of existing variants</param>
    private void GetListOfAlreadyCreatedVariants(List<int> existingCategories)
    {
        foreach (int categoryID in mVariantCategoriesOptions.Select(c => c.Item1.CategoryID))
        {
            if (!existingCategories.Contains(categoryID))
            {
                existingCategories.Add(categoryID);
            }
        }
    }


    /// <summary>
    /// Returns list of RowIds of variants from DataSet, which already exists in DB.
    /// </summary>
    /// <returns>List of strings - RowIds</returns>
    private List<string> GetExistingItems()
    {
        if (!DataHelper.DataSourceIsEmpty(mGeneratedVariants))
        {
            var result = (from row in mGeneratedVariants.Tables[0].AsEnumerable()
                          where row.Field<bool>("Exist")
                          select ValidationHelper.GetString(row["RowNumber"], string.Empty)).ToList();
            return result;
        }

        return new List<string>();
    }


    /// <summary>
    /// Adds column to list of UniGrid column definition.
    /// </summary>
    /// <param name="columnName">Column name (string)</param>
    /// <param name="source">Name of source (string)</param>
    /// <param name="externalSourceName">Optional - if it is not set no external ExternalSourceName parameter will be set</param>
    /// <param name="hidden">Optional - if it is set to true, column will be not visible in UniGrid</param>
    private void AddGridColumn(string columnName, string source, string externalSourceName = "", bool hidden = false)
    {
        // If definition of columns is empty, create new list and put filter column in it
        if (mColumnDefinitions == null)
        {
            mColumnDefinitions = new List<Column>();
        }

        Column columnDefinition = new Column
        {
            Caption = columnName,
            Source = source,
            AllowSorting = false,
            Wrap = false
        };

        if (externalSourceName != "")
        {
            columnDefinition.ExternalSourceName = externalSourceName;
        }

        if (hidden)
        {
            columnDefinition.CssClass = "Hidden";
        }

        // Add column to column definition
        mColumnDefinitions.Add(columnDefinition);
    }


    /// <summary>
    /// Searches DataRow and gets all optionIds (column values)
    /// where categoryIds (column names) are specified.
    /// </summary>
    /// <param name="dataRow">DataRow to be searched</param>
    /// <param name="categoryIds">List of categoryIds</param>
    /// <returns>List of optionsIds</returns>
    private IEnumerable<int> GetAllOptions(DataRow dataRow, IEnumerable<int> categoryIds)
    {
        return categoryIds.Select(categoryId => ValidationHelper.GetInteger(dataRow[categoryId.ToString()], 0)).ToList();
    }


    /// <summary>
    /// Checks if user has permissions to modify variants. 
    /// </summary>
    /// <param name="skuInfo">SKUInfo object</param>
    private void CheckPermissions(SKUInfo skuInfo)
    {
        if (skuInfo != null)
        {
            CheckProductModifyAndRedirect(skuInfo);
        }
    }


    /// <summary>
    /// Hides / Shows filter, assigns images and labels.
    /// </summary>
    private void ManipulateFilter()
    {
        if (ShowFilter)
        {
            btnFilter.ResourceString = "general.hidefilter";
            plcFilter.Visible = true;
        }
        else
        {
            btnFilter.ResourceString = "general.showfilter";
            plcFilter.Visible = false;
        }

        if ((ExistingCategories.Count > 0) || (CategorySelector.SelectedCategories.Count > 0))
        {
            btnFilter.Visible = true;
        }
        else
        {
            // No variants in grid and filter is inactive
            btnFilter.Visible = false;
        }
    }


    /// <summary>
    /// Fills list with categories and their options.
    /// </summary>
    /// <param name="CategoriesOptions">List to fill</param>
    /// <param name="CategoriesDS">Option categories dataset</param>
    private void FillCategoriesOptions(List<Tuple<OptionCategoryInfo, List<SKUInfo>>> CategoriesOptions, DataSet CategoriesDS)
    {
        if (!DataHelper.DataSourceIsEmpty(CategoriesDS))
        {
            foreach (DataRow categoryRow in CategoriesDS.Tables[0].Rows)
            {
                var categoryId = ValidationHelper.GetInteger(categoryRow["CategoryID"], 0);
                var options = SKUInfoProvider.GetSKUOptionsForProduct(ProductID, categoryId, true).OrderBy("SKUOrder");
                var category = new OptionCategoryInfo(categoryRow);
                CategoriesOptions.Add(new Tuple<OptionCategoryInfo, List<SKUInfo>>(category, options.ToList()));
            }
        }
    }


    /// <summary>
    /// Adds parameter to current URL and Redirects to it
    /// </summary>
    /// <param name="parameter">Parameter to be added.</param>
    /// <param name="value">Value of parameter to be added.</param>
    private void RedirectTo(string parameter, string value)
    {
        string urlToRedirect = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, parameter, value);
        URLHelper.Redirect(urlToRedirect);
    }


    /// <summary>
    /// Generates variants asynchronous.
    /// </summary>
    /// <param name="parameter">AsyncControl parameters</param>
    private void GenerateAndSave(object parameter)
    {
        try
        {
            // Regenerate already existing variants
            if (mRegenerateVariants)
            {
                ProductAttributeSet productAttributeSet = new ProductAttributeSet(CategorySelector.SelectedCategories.Values.Where(x => x > VariantOptionInfo.NewOption));
                List<ProductVariant> existingVariants = VariantHelper.AddNewCategoriesToVariantsOfProduct(ProductID, productAttributeSet);

                // Use special action contexts to turn off unnecessary actions 
                using (ECommerceActionContext eCommerceContext = new ECommerceActionContext())
                {
                    eCommerceContext.TouchParent = false;
                    eCommerceContext.SetLowestPriceToParent = false;

                    existingVariants.ForEach(pVariant =>
                        {
                            AddLog(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(pVariant.Variant.SKUName)) + GetString("com.variants.isredefined"));
                            VariantHelper.SetProductVariant(pVariant);
                        });
                }

                // Save variant to update parent SKULastModified a SKUPrice properties
                var lastVariant = existingVariants.LastOrDefault();
                if (lastVariant != null)
                {
                    lastVariant.Variant.Generalized.SetObject();
                }
            }

            // Generate non-existing variants
            ProductVariant productVariant = null;

            // Use special action contexts to turn off unnecessary actions 
            using (ECommerceActionContext eCommerceContext = new ECommerceActionContext())
            {
                eCommerceContext.TouchParent = false;
                eCommerceContext.SetLowestPriceToParent = false;

                foreach (DataRow row in mVariantsToGenerate)
                {
                    IEnumerable<int> options = GetAllOptions(row, NewCategories.Union(ExistingCategories));
                    productVariant = new ProductVariant(ProductID, new ProductAttributeSet(options));
                    AddLog(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(productVariant.Variant.SKUName)) + GetString("com.variants.isdefined"));
                    productVariant.Set();
                }
            }

            // Save variant to update parent SKULastModified a SKUPrice properties
            if (productVariant != null)
            {
                productVariant.Variant.Generalized.SetObject();
            }

        }
        catch (Exception ex)
        {
            CurrentError = GetString("com.variant.definerror");
            Service.Resolve<IEventLogService>().LogException("Variant definition", "DEFINEVARIANT", ex);
        }
    }

    #endregion


    #region "Handling asynchronous thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        RedirectTo("error", GetString("com.variant.terminated"));
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(CurrentWarning))
        {
            RedirectTo("warning", CurrentWarning);
        }

        if (!String.IsNullOrEmpty(CurrentError))
        {
            RedirectTo("error", CurrentError);
        }

        var url = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_Variants.aspx";
        url = URLHelper.AddParameterToUrl(url, "saved", "1");
        url = URLHelper.AddParameterToUrl(url, "productID", ProductID.ToString());
        url = URLHelper.AddParameterToUrl(url, "dialog", QueryHelper.GetString("dialog", "0"));
        // If generation of variant was successful redirect user to the variant listing of parent product
        URLHelper.Redirect(url);
    }


    /// <summary>
    /// Adds the log information
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog, "Variants");
    }


    /// <summary>
    /// Ensures the logging context
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext currentLog = LogContext.EnsureLog(ctlAsyncLog.ProcessGUID);
        return currentLog;
    }


    /// <summary>
    /// Ensures log for asynchronous control
    /// </summary>
    private void EnsureAsyncLog()
    {
        pnlLog.Visible = true;
        pnlContent.Visible = false;

        CurrentWarning = string.Empty;
        CurrentError = string.Empty;
    }


    /// <summary>
    /// Runs asynchronous thread
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Prepare asynchronous control
    /// </summary>
    private void SetupAsyncControl()
    {
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        ctlAsyncLog.MaxLogLines = 1000;

        // Asynchronous content configuration
        ctlAsyncLog.TitleText = GetString("com.variants.defining");
        if (!RequestHelper.IsCallback())
        {
            // Set visibility of panels
            pnlContent.Visible = true;
            pnlLog.Visible = false;
        }
    }

    #endregion
}
