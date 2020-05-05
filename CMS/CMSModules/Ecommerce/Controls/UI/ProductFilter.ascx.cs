using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Ecommerce_Controls_UI_ProductFilter : CMSAbstractDataFilterControl
{
    #region "Variables"

    private TreeNode mParentNode;
    private CMSUserControl filteredControl;
    private string mFilterMode;
    private bool allowGlobalProducts;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current filter mode.
    /// </summary>
    public override string FilterMode
    {
        get
        {
            return mFilterMode ?? (mFilterMode = ValidationHelper.GetString(filteredControl.GetValue("FilterMode"), "").ToLowerInvariant());
        }
        set
        {
            mFilterMode = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition().ToString(true);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    private TreeNode ParentNode
    {
        get
        {
            var uiContext = UIContextHelper.GetUIContext(this);
            return mParentNode ?? (mParentNode = uiContext.EditedObject as TreeNode);
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the advanced filter is displayed or not. 
    /// </summary>
    private bool IsAdvancedMode
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false);
        }
        set
        {
            ViewState["IsAdvancedMode"] = value;
        }
    }


  #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        filteredControl = FilteredControl as CMSUserControl;

        // Hide filter button, this filter has its own
        UniGrid grid = filteredControl as UniGrid;
        if (grid != null)
        {
            grid.HideFilterButton = true;
        }

        allowGlobalProducts = ECommerceSettings.AllowGlobalProducts(SiteContext.CurrentSiteName);

        // Display Global and site option if global products are allowed
        siteElem.ShowSiteAndGlobal = allowGlobalProducts;

        // Initialize controls
        if (!RequestHelper.IsPostBack())
        {
            FillThreeStateDDL(ddlNeedsShipping);
            FillThreeStateDDL(ddlAllowForSale);
            FillDocumentTypesDDL();
            ResetFilter();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup filter mode
        SetFieldsVisibility();

        // When global SKUs can be included in listing
        if (allowGlobalProducts)
        {
            // Display global departments, manufacturers, suppliers and global statuses too
            departmentElem.DisplayGlobalItems = true;
            publicStatusElem.DisplayGlobalItems = true;
            internalStatusElem.DisplayGlobalItems = true;
            supplierElem.DisplayGlobalItems = true;
            manufacturerElem.DisplayGlobalItems = true;
        }

        plcSite.Visible = allowGlobalProducts;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Appends given selector filter to given <paramref name="whereCondition"/> accordingly to filter state.
    /// </summary>
    private void AppendSelectorFilter(FormEngineUserControl selector, string selectingColumn, WhereCondition whereCondition)
    {
        if (!selector.HasValue)
        {
            return;
        }

        var value = ValidationHelper.GetInteger(selector.Value, UniSelector.US_NONE_RECORD);
        if (value == UniSelector.US_NONE_RECORD)
        {
            whereCondition.WhereNull(selectingColumn);
        }
        else if (value != UniSelector.US_ALL_RECORDS)
        {
            whereCondition.WhereEquals(selectingColumn, value);
        }
    }


    /// <summary>
    /// Creates where condition according to values selected in filter.
    /// </summary>
    private WhereCondition GenerateWhereCondition()
    {
        var where = new WhereCondition();

        string productNameColumnName = (ParentNode != null) ? "DocumentName" : "SKUName";

        // Append name/number condition
        var nameOrNumber = txtNameOrNumber.Text.Trim().Truncate(txtNameOrNumber.MaxLength);
        if (!string.IsNullOrEmpty(nameOrNumber))
        {
            where.Where(k => k.Where(w => w.WhereContains(productNameColumnName, nameOrNumber)
                                           .Or()
                                           .WhereContains("SKUNumber", nameOrNumber)));
        }

        // Append site condition
        if (allowGlobalProducts && (siteElem.SiteID != UniSelector.US_GLOBAL_AND_SITE_RECORD))
        {
            // Restrict SKUSiteID only for products not for product section (full listing mode)
            int selectedSiteID = (siteElem.SiteID > 0) ? siteElem.SiteID : 0;
            where.Where(w => w.WhereEquals("SKUSiteID".AsColumn().IsNull(0), selectedSiteID).Or().WhereNull("SKUID"));
        }

        // Append product type condition
        if ((selectProductTypeElem.Value != null) && (selectProductTypeElem.Value.ToString() != "ALL"))
        {
            where.WhereEquals("SKUProductType", selectProductTypeElem.Value);
        }

        // Append department condition
        AppendSelectorFilter(departmentElem, "SKUDepartmentID", where);

        // Manufacturer value
        AppendSelectorFilter(manufacturerElem, "SKUManufacturerID", where);

        // Brand value
        AppendSelectorFilter(brandElem, "SKUBrandID", where);

        // Supplier value
        AppendSelectorFilter(supplierElem, "SKUSupplierID", where);

        // Collection value
        AppendSelectorFilter(collectionElem, "SKUCollectionID", where);

        // Store status value
        AppendSelectorFilter(publicStatusElem, "SKUPublicStatusID", where);

        // Internal status value
        AppendSelectorFilter(internalStatusElem, "SKUInternalStatusID", where);

        // Append needs shipping condition
        int needsShipping = ValidationHelper.GetInteger(ddlNeedsShipping.SelectedValue, -1);
        if (needsShipping >= 0)
        {
            where.WhereEquals("SKUNeedsShipping".AsColumn().IsNull(0), needsShipping);
        }

        // Append allow for sale condition
        int allowForSale = ValidationHelper.GetInteger(ddlAllowForSale.SelectedValue, -1);
        if (allowForSale >= 0)
        {
            where.WhereEquals("SKUEnabled", allowForSale);
        }

        // When in document mode
        if (ParentNode != null)
        {
            int docTypeId = ValidationHelper.GetInteger(drpDocTypes.SelectedValue, 0);
            if (docTypeId > 0)
            {
                // Append document type condition
                where.WhereEquals("NodeClassID", docTypeId);
            }
        }

        return where;
    }


    /// <summary>
    /// Applies filter to unigrid
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        if (!IsAdvancedMode)
        {
            ResetAdvancedFilterPart();
        }

        ApplyFilter(sender, e);
    }


    /// <summary>
    /// Applies filter to unigrid.
    /// </summary>
    protected void ApplyFilter(object sender, EventArgs e)
    {
        UniGrid grid = filteredControl as UniGrid;
        grid?.ApplyFilter(sender, e);
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = filteredControl as UniGrid;
        grid?.Reset();
    }


    /// <summary>
    /// Sets the advanced mode.
    /// </summary>
    protected void lnkShowAdvancedFilter_Click(object sender, EventArgs e)
    {
        IsAdvancedMode = true;
        SetFieldsVisibility();
    }


    /// <summary>
    /// Sets the simple mode.
    /// </summary>
    protected void lnkShowSimpleFilter_Click(object sender, EventArgs e)
    {
        IsAdvancedMode = false;
        SetFieldsVisibility();
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    private void ResetAdvancedFilterPart()
    {
        siteElem.SiteID = UniSelector.US_GLOBAL_AND_SITE_RECORD;
        departmentElem.Value = UniSelector.US_ALL_RECORDS;
        selectProductTypeElem.Value = "ALL";
        manufacturerElem.Value = UniSelector.US_ALL_RECORDS;
        supplierElem.Value = UniSelector.US_ALL_RECORDS;
        internalStatusElem.Value = UniSelector.US_ALL_RECORDS;
        publicStatusElem.Value = UniSelector.US_ALL_RECORDS;
        brandElem.Value = UniSelector.US_ALL_RECORDS;
        collectionElem.Value = UniSelector.US_ALL_RECORDS;
        ddlNeedsShipping.SelectedIndex = 0;
        ddlAllowForSale.SelectedIndex = 0;
        drpDocTypes.SelectedIndex = 0;
    }


    /// <summary>
    /// Shows/hides fields of filter according to simple/advanced mode.
    /// </summary>
    private void SetFieldsVisibility()
    {
        plcSimpleFilter.Visible = !IsAdvancedMode;
        plcAdvancedFilter.Visible = IsAdvancedMode;

        plcAdvancedFilterType.Visible = IsAdvancedMode;
        plcAdvancedFilterGeneral.Visible = IsAdvancedMode;

        bool documentMode = (ParentNode != null);
        plcAdvancedDocumentType.Visible = IsAdvancedMode && documentMode;
    }


    /// <summary>
    /// Fills items 'Yes', 'No' and 'All' to given drop down list.
    /// </summary>
    /// <param name="dropDown">Drop down list to be filled.</param>
    private void FillThreeStateDDL(CMSDropDownList dropDown)
    {
        dropDown.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        dropDown.Items.Add(new ListItem(GetString("general.yes"), "1"));
        dropDown.Items.Add(new ListItem(GetString("general.no"), "0"));
    }


    /// <summary>
    /// Fills dropdown list with document types.
    /// </summary>
    private void FillDocumentTypesDDL()
    {
        drpDocTypes.Items.Clear();

        // Add (All) record
        drpDocTypes.Items.Add(new ListItem(GetString("general.selectall"), UniSelector.US_ALL_RECORDS.ToString()));

        // Select only document types from current site marked as product
        DataSet ds = DocumentTypeHelper.GetDocumentTypeClasses()
            .OnSite(SiteContext.CurrentSiteID)
            .WhereTrue("ClassIsProduct")
            .OrderBy("ClassDisplayName")
            .Columns("ClassID", "ClassDisplayName");

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string name = ValidationHelper.GetString(dr["ClassDisplayName"], "");
                int id = ValidationHelper.GetInteger(dr["ClassID"], 0);

                if (!String.IsNullOrEmpty(name) && (id > 0))
                {
                    // Handle document name
                    name = ResHelper.LocalizeString(MacroResolver.Resolve(name));

                    drpDocTypes.Items.Add(new ListItem(name, id.ToString()));
                }
            }
        }
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("AdvancedMode", IsAdvancedMode);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        IsAdvancedMode = state.GetBoolean("AdvancedMode");
        SetFieldsVisibility();
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtNameOrNumber.Text = String.Empty;
        ResetAdvancedFilterPart();
    }

    #endregion
}