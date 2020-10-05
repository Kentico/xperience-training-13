using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSWebParts_Ecommerce_Products_Products : CMSAbstractWebPart
{
    #region "Constants"

    private const string FILTER_ALL = "ALL";

    private const string PRODUCT_TYPE_PRODUCTS = "PRODUCTS";
    private const string PRODUCT_TYPE_PRODUCT_OPTIONS = "PRODUCTOPTIONS";

    private const string NEEDS_SHIPPING_YES = "YES";
    private const string NEEDS_SHIPPING_NO = "NO";

    private const string ALLOW_FOR_SALE_YES = "YES";
    private const string ALLOW_FOR_SALE_NO = "NO";

    private const string ORDER_BY_NAME_ASC = "Name ASC";
    private const string ORDER_BY_NAME_DESC = "Name DESC";
    private const string ORDER_BY_NUMBER_ASC = "Number ASC";
    private const string ORDER_BY_NUMBER_DESC = "Number DESC";
    private const string ORDER_BY_PRICE_ASC = "Price ASC";
    private const string ORDER_BY_PRICE_DESC = "Price DESC";
    private const string ORDER_BY_DATE_DESC = "Date DESC";
    private const string ORDER_BY_DATE_ASC = "Date ASC";
    private const string ORDER_BY_AVAILABLE_ITEMS_ASC = "AvailableItems ASC";
    private const string ORDER_BY_REORDER_DESC = "Reorder DESC";

    private const string COLUMN_NUMBER = "NUMBER";
    private const string COLUMN_PRICE = "PRICE";
    private const string COLUMN_DEPARTMENT = "DEPARTMENT";
    private const string COLUMN_MANUFACTURER = "MANUFACTURER";
    private const string COLUMN_SUPPLIER = "SUPPLIER";
    private const string COLUMN_BRAND = "BRAND";
    private const string COLUMN_COLLECTION = "COLLECTION";
    private const string COLUMN_TAX_CLASS = "TAXCLASS";
    private const string COLUMN_PUBLIC_STATUS = "PUBLICSTATUS";
    private const string COLUMN_INTERNAL_STATUS = "INTERNALSTATUS";
    private const string COLUMN_REORDER_AT = "REORDERAT";
    private const string COLUMN_AVAILABLE_ITEMS = "AVAILABLEITEMS";
    private const string COLUMN_ITEMS_TO_BE_REORDERED = "ITEMSTOBEREORDERED";
    private const string COLUMN_ALLOW_FOR_SALE = "ALLOWFORSALE";

    #endregion


    #region "Properties"

    /// <summary>
    /// Include.
    /// </summary>
    public string ProductType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ProductType"), "products");
        }
        set
        {
            SetValue("ProductType", value);
        }
    }


    /// <summary>
    /// Product type.
    /// </summary>
    public string Representing
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Representing"), "");
        }
        set
        {
            SetValue("Representing", value);
        }
    }


    /// <summary>
    /// Product number contains.
    /// </summary>
    public string ProductNumber
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ProductNumber"), "");
        }
        set
        {
            SetValue("ProductNumber", value);
        }
    }


    /// <summary>
    /// Department.
    /// </summary>
    public string Department
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Department"), "");
        }
        set
        {
            SetValue("Department", value);
        }
    }


    /// <summary>
    /// Manufacturer.
    /// </summary>
    public string Manufacturer
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Manufacturer"), "");
        }
        set
        {
            SetValue("Manufacturer", value);
        }
    }


    /// <summary>
    /// Brand.
    /// </summary>
    public string Brand
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Brand"), "");
        }
        set
        {
            SetValue("Brand", value);
        }
    }


    /// <summary>
    /// Collection.
    /// </summary>
    public string Collection
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Collection"), "");
        }
        set
        {
            SetValue("Collection", value);
        }
    }


    /// <summary>
    /// Collection.
    /// </summary>
    public string TaxClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TaxClass"), "");
        }
        set
        {
            SetValue("TaxClass", value);
        }
    }


    /// <summary>
    /// Supplier.
    /// </summary>
    public string Supplier
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Supplier"), "");
        }
        set
        {
            SetValue("Supplier", value);
        }
    }


    /// <summary>
    /// Needs shipping.
    /// </summary>
    public string NeedsShipping
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NeedsShipping"), "all");
        }
        set
        {
            SetValue("NeedsShipping", value);
        }
    }


    /// <summary>
    /// Price from.
    /// </summary>
    public decimal PriceFrom
    {
        get
        {
            return ValidationHelper.GetDecimalSystem(GetValue("PriceFrom"), 0);
        }
        set
        {
            SetValue("PriceFrom", value);
        }
    }


    /// <summary>
    /// Price to.
    /// </summary>
    public decimal PriceTo
    {
        get
        {
            return ValidationHelper.GetDecimalSystem(GetValue("PriceTo"), 0);
        }
        set
        {
            SetValue("PriceTo", value);
        }
    }


    /// <summary>
    /// Public status.
    /// </summary>
    public string PublicStatus
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PublicStatus"), "");
        }
        set
        {
            SetValue("PublicStatus", value);
        }
    }


    /// <summary>
    /// Internal status.
    /// </summary>
    public string InternalStatus
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InternalStatus"), "");
        }
        set
        {
            SetValue("InternalStatus", value);
        }
    }


    /// <summary>
    /// Allow for sale.
    /// </summary>
    public string AllowForSale
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AllowForSale"), "all");
        }
        set
        {
            SetValue("AllowForSale", value);
        }
    }


    /// <summary>
    /// Available items less than.
    /// </summary>
    public string AvailableItems
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AvailableItems"), "");
        }
        set
        {
            SetValue("AvailableItems", value);
        }
    }


    /// <summary>
    /// Needs to be reordered.
    /// </summary>
    public bool NeedsToBeReordered
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("NeedsToBeReordered"), false);
        }
        set
        {
            SetValue("NeedsToBeReordered", value);
        }
    }


    /// <summary>
    /// Top N.
    /// </summary>
    public int TopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("TopN"), 0);
        }
        set
        {
            SetValue("TopN", value);
        }
    }


    /// <summary>
    /// Sort by.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "");
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), 0);
        }
        set
        {
            SetValue("PageSize", value);
        }
    }


    /// <summary>
    /// Visible columns in listing.
    /// </summary>
    public string VisibleColumns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("VisibleColumns"), "");
        }
        set
        {
            SetValue("VisibleColumns", value);
        }
    }


    /// <summary>
    /// Gets the messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;

    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        DisplayColumns();
    }

    #endregion


    #region "Event handlers"

    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView rowView = parameter as DataRowView;
        if (rowView != null)
        {
            SKUInfo sku = new SKUInfo(rowView.Row);
            switch (sourceName.ToLowerCSafe())
            {
                case "skuname":
                    string fullName = sku.SKUName;

                    // For variant, add name from parent SKU
                    if (sku.SKUParentSKUID != 0)
                    {
                        SKUInfo parentSku = SKUInfo.Provider.Get(sku.SKUParentSKUID);
                        fullName = string.Format("{0}: {1}", parentSku.SKUName, sku.SKUName);
                    }
                    return HTMLHelper.HTMLEncode(fullName);

                case "optioncategory":
                    OptionCategoryInfo optionCategory = OptionCategoryInfo.Provider.Get(sku.SKUOptionCategoryID);

                    // Return option category display name for product option or '-' for product
                    if (optionCategory != null)
                    {
                        return HTMLHelper.HTMLEncode(optionCategory.CategoryDisplayName ?? "");
                    }
                    return "-";

                case "skunumber":
                    return ResHelper.LocalizeString(sku.SKUNumber, null, true);

                case "skuprice":
                    // Format price
                    return CurrencyInfoProvider.GetFormattedPrice(sku.SKUPrice, sku.SKUSiteID);

                case "skudepartmentid":
                    // Transform to display name and localize
                    DepartmentInfo department = DepartmentInfo.Provider.Get(sku.SKUDepartmentID);
                    return (department != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(department.DepartmentDisplayName)) : "";

                case "skumanufacturerid":
                    // Transform to display name and localize
                    ManufacturerInfo manufacturer = ManufacturerInfo.Provider.Get(sku.SKUManufacturerID);
                    return (manufacturer != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(manufacturer.ManufacturerDisplayName)) : "";

                case "skubrandid":
                    // Transform to display name and localize
                    BrandInfo brand = BrandInfo.Provider.Get(sku.SKUBrandID);
                    return (brand != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(brand.BrandDisplayName)) : "";

                case "skucollectionid":
                    // Transform to display name and localize
                    CollectionInfo collection = CollectionInfo.Provider.Get(sku.SKUCollectionID);
                    return (collection != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(collection.CollectionDisplayName)) : "";

                case "skutaxclassid":
                    // Transform to display name and localize
                    TaxClassInfo taxClass = TaxClassInfo.Provider.Get(sku.SKUTaxClassID);
                    return (taxClass != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(taxClass.TaxClassDisplayName)) : "";

                case "skusupplierid":
                    // Transform to display name and localize
                    SupplierInfo supplier = SupplierInfoProvider.ProviderObject.Get(sku.SKUSupplierID);
                    return (supplier != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(supplier.SupplierDisplayName)) : "";

                case "skupublicstatusid":
                    // Transform to display name and localize
                    PublicStatusInfo publicStatus = PublicStatusInfo.Provider.Get(sku.SKUPublicStatusID);
                    return (publicStatus != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(publicStatus.PublicStatusDisplayName)) : "";

                case "skuinternalstatusid":
                    // Transform to display name and localize
                    InternalStatusInfo internalStatus = InternalStatusInfo.Provider.Get(sku.SKUInternalStatusID);
                    return (internalStatus != null) ? HTMLHelper.HTMLEncode(ResHelper.LocalizeString(internalStatus.InternalStatusDisplayName)) : "";

                case "skuavailableitems":
                    int? count = sku.SKUAvailableItems as int?;
                    int? reorderAt = sku.SKUReorderAt as int?;

                    // Emphasize the number when product needs to be reordered
                    if (count.HasValue && ((reorderAt.HasValue && (count <= reorderAt)) || (!reorderAt.HasValue && (count <= 0))))
                    {
                        // Format message informing about insufficient stock level
                        return String.Format("<span class=\"OperationFailed\">{0}</span>", count);
                    }
                    return count;

                case "itemstobereordered":
                    int difference = sku.SKUReorderAt - sku.SKUAvailableItems;

                    // Return difference, or '-'
                    return (difference > 0) ? difference.ToString() : "-";

                case "skusiteid":
                    return UniGridFunctions.ColoredSpanYesNo(sku.SKUSiteID == 0);
            }
        }

        return parameter;
    }

    #endregion


    #region "Methods - webpart"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();

        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.PRODUCTS_READ))
        {
            ShowError(String.Format(GetString("CMSMessages.AccessDeniedResource"), "EcommerceRead OR ReadProducts"));
            gridElem.Visible = false;
            return;
        }

        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        gridElem.WhereCondition = GetWhereCondition().ToString(true);
        gridElem.OrderBy = GetOrderBy();
        SetTopN();
        gridElem.Pager.DefaultPageSize = GetPageSize();
    }

    #endregion


    #region "Methods - private"

    /// <summary>
    /// Displays or hides columns based on VisibleColumns property.
    /// </summary>
    private void DisplayColumns()
    {
        string[] visibleColumns = VisibleColumns.Split('|');

        // Hide all first
        foreach (var item in gridElem.NamedColumns.Values)
        {
            item.Visible = false;
        }

        // Show columns that should be visible
        foreach (var item in visibleColumns)
        {
            string key = null;
            switch (item)
            {
                case COLUMN_NUMBER:
                    key = "Number";
                    break;

                case COLUMN_PRICE:
                    key = "Price";
                    break;

                case COLUMN_DEPARTMENT:
                    key = "Department";
                    break;

                case COLUMN_MANUFACTURER:
                    key = "Manufacturer";
                    break;

                case COLUMN_SUPPLIER:
                    key = "Supplier";
                    break;

                case COLUMN_BRAND:
                    key = "Brand";
                    break;

                case COLUMN_COLLECTION:
                    key = "Collection";
                    break;

                case COLUMN_TAX_CLASS:
                    key = "TaxClass";
                    break;

                case COLUMN_PUBLIC_STATUS:
                    key = "PublicStatus";
                    break;

                case COLUMN_INTERNAL_STATUS:
                    key = "InternalStatus";
                    break;

                case COLUMN_REORDER_AT:
                    key = "ReorderAt";
                    break;

                case COLUMN_AVAILABLE_ITEMS:
                    key = "AvailableItems";
                    break;

                case COLUMN_ITEMS_TO_BE_REORDERED:
                    key = "ItemsToBeReordered";
                    break;

                case COLUMN_ALLOW_FOR_SALE:
                    key = "AllowForSale";
                    break;
            }

            // Show column
            if (key != null)
            {
                gridElem.NamedColumns[key].Visible = true;
            }
        }

        // Show option category column if not only product listed
        if (ProductType != PRODUCT_TYPE_PRODUCTS)
        {
            gridElem.NamedColumns["OptionCategory"].Visible = true;
        }

        // If global products are allowed, display column
        if (ECommerceSettings.AllowGlobalProducts(SiteContext.CurrentSiteName))
        {
            gridElem.NamedColumns["SKUSiteID"].Visible = true;
        }
    }


    /// <summary>
    /// Returns where condition based on webpart fields.
    /// </summary>
    private WhereCondition GetWhereCondition()
    {
        var where = new WhereCondition().WhereEquals("SKUSiteID", SiteContext.CurrentSiteID);

        // Show products only from current site or global too, based on setting
        if (ECommerceSettings.AllowGlobalProducts(SiteContext.CurrentSiteName))
        {
            where.Where(w => w.WhereEquals("SKUSiteID", SiteContext.CurrentSiteID).Or().WhereNull("SKUSiteID"));
        }

        // Show/hide product variants - it is based on type of inventory tracking for parent product
        string trackByVariants = TrackInventoryTypeEnum.ByVariants.ToStringRepresentation();
        where.Where(v => v.Where(w => w.WhereNull("SKUParentSKUID").And().WhereNotEquals("SKUTrackInventory", trackByVariants))
                          .Or()
                          .Where(GetParentProductWhereCondition(new WhereCondition().WhereEquals("SKUTrackInventory", trackByVariants))));

        // Product type filter
        if (!string.IsNullOrEmpty(ProductType) && (ProductType != FILTER_ALL))
        {
            if (ProductType == PRODUCT_TYPE_PRODUCTS)
            {
                where.WhereNull("SKUOptionCategoryID");
            }
            else if (ProductType == PRODUCT_TYPE_PRODUCT_OPTIONS)
            {
                where.WhereNotNull("SKUOptionCategoryID");
            }
        }

        // Representing filter
        if (!string.IsNullOrEmpty(Representing) && (Representing != FILTER_ALL))
        {
            SKUProductTypeEnum productTypeEnum = EnumStringRepresentationExtensions.ToEnum<SKUProductTypeEnum>(Representing);
            string productTypeString = productTypeEnum.ToStringRepresentation();

            where.WhereEquals("SKUProductType", productTypeString);
        }

        // Product number filter
        if (!string.IsNullOrEmpty(ProductNumber))
        {
            where.WhereContains("SKUNumber", ProductNumber);
        }

        // Department filter
        DepartmentInfo di = DepartmentInfo.Provider.Get(Department, SiteInfoProvider.GetSiteID(CurrentSiteName));
        di = di ?? DepartmentInfo.Provider.Get(Department, 0);

        if (di != null)
        {
            where.Where(GetColumnWhereCondition("SKUDepartmentID", new WhereCondition().WhereEquals("SKUDepartmentID", di.DepartmentID)));
        }

        // Manufacturer filter
        ManufacturerInfo mi = ManufacturerInfo.Provider.Get(Manufacturer, SiteInfoProvider.GetSiteID(CurrentSiteName));
        mi = mi ?? ManufacturerInfo.Provider.Get(Manufacturer, 0);
        if (mi != null)
        {
            where.Where(GetColumnWhereCondition("SKUManufacturerID", new WhereCondition().WhereEquals("SKUManufacturerID", mi.ManufacturerID)));
        }

        // Brand filter
        BrandInfo bi = BrandInfoProvider.ProviderObject.Get(Brand, SiteInfoProvider.GetSiteID(CurrentSiteName));
        if (bi != null)
        {
            where.Where(GetColumnWhereCondition("SKUBrandID", new WhereCondition().WhereEquals("SKUBrandID", bi.BrandID)));
        }

        // Collection filter
        CollectionInfo ci = CollectionInfo.Provider.Get(Collection, SiteInfoProvider.GetSiteID(CurrentSiteName));    
        if (ci != null)
        {
            where.Where(GetColumnWhereCondition("SKUCollectionID", new WhereCondition().WhereEquals("SKUCollectionID", ci.CollectionID)));
        }

        // Tax class filter
        TaxClassInfo tci = TaxClassInfo.Provider.Get(TaxClass, SiteInfoProvider.GetSiteID(CurrentSiteName));
        if (tci != null)
        {
            where.Where(GetColumnWhereCondition("SKUTaxClassID", new WhereCondition().WhereEquals("SKUTaxClassID", tci.TaxClassID)));
        }

        // Supplier filter
        SupplierInfo si = SupplierInfoProvider.GetSupplierInfo(Supplier, CurrentSiteName);
        si = si ?? SupplierInfoProvider.GetSupplierInfo(Supplier, null);
        if (si != null)
        {
            where.Where(GetColumnWhereCondition("SKUSupplierID", new WhereCondition().WhereEquals("SKUSupplierID", si.SupplierID)));
        }

        // Needs shipping filter
        if (!string.IsNullOrEmpty(NeedsShipping) && (NeedsShipping != FILTER_ALL))
        {
            if (NeedsShipping == NEEDS_SHIPPING_YES)
            {
                where.Where(GetColumnWhereCondition("SKUNeedsShipping", new WhereCondition().WhereTrue("SKUNeedsShipping")));
            }
            else if (NeedsShipping == NEEDS_SHIPPING_NO)
            {
                where.Where(GetColumnWhereCondition("SKUNeedsShipping", new WhereCondition().WhereFalse("SKUNeedsShipping").Or().WhereNull("SKUNeedsShipping")));
            }
        }

        // Price from filter
        if (PriceFrom > 0)
        {
            where.WhereGreaterOrEquals("SKUPrice", PriceFrom);
        }

        // Price to filter
        if (PriceTo > 0)
        {
            where.WhereLessOrEquals("SKUPrice", PriceTo);
        }

        // Public status filter
        PublicStatusInfo psi = PublicStatusInfo.Provider.Get(PublicStatus, SiteInfoProvider.GetSiteID(CurrentSiteName));
        if (psi != null)
        {
            where.Where(GetColumnWhereCondition("SKUPublicStatusID", new WhereCondition().WhereEquals("SKUPublicStatusID", psi.PublicStatusID)));
        }

        // Internal status filter
        InternalStatusInfo isi = InternalStatusInfo.Provider.Get(InternalStatus, SiteInfoProvider.GetSiteID(CurrentSiteName));
        if (isi != null)
        {
            where.Where(GetColumnWhereCondition("SKUInternalStatusID", new WhereCondition().WhereEquals("SKUInternalStatusID", isi.InternalStatusID)));
        }

        // Allow for sale filter
        if (!string.IsNullOrEmpty(AllowForSale) && (AllowForSale != FILTER_ALL))
        {
            if (AllowForSale == ALLOW_FOR_SALE_YES)
            {
                where.WhereTrue("SKUEnabled");
            }
            else if (AllowForSale == ALLOW_FOR_SALE_NO)
            {
                where.WhereEqualsOrNull("SKUEnabled", false);
            }
        }

        // Available items filter
        if (!string.IsNullOrEmpty(AvailableItems))
        {
            int value = ValidationHelper.GetInteger(AvailableItems, int.MaxValue);
            where.WhereLessOrEquals("SKUAvailableItems", value);
        }

        // Needs to be reordered filter
        if (NeedsToBeReordered)
        {
            where.Where(w => w.Where(v => v.WhereNull("SKUReorderAt").And().WhereLessOrEquals("SKUAvailableItems", 0))
                              .Or()
                              .Where(z => z.WhereNotNull("SKUReorderAt").And().WhereLessOrEquals("SKUAvailableItems".AsColumn(), "SKUReorderAt".AsColumn())));
        }

        return where;
    }


    /// <summary>
    /// Returns where condition for a single column. It consider parent product for variants.
    /// </summary>
    /// <param name="column">Column to filter for.</param>
    /// <param name="condition">Condition to restrict.</param>
    private WhereCondition GetColumnWhereCondition(string column, WhereCondition condition)
    {
        return new WhereCondition().Where(condition.Or().WhereNull(column).And().Where(GetParentProductWhereCondition(condition)));
    }


    /// <summary>
    /// Returns where condition based on parent product for variants.
    /// </summary>
    /// <param name="condition">Condition to restrict parent product.</param>
    private WhereCondition GetParentProductWhereCondition(WhereCondition condition)
    {
        return new WhereCondition().WhereNotNull("SKUParentSKUID").And().WhereIn("SKUParentSKUID", new IDQuery<SKUInfo>("SKUID").Where(condition));
    }


    /// <summary>
    /// Set TopN property to unigrid and disables Pager, if TopN field is specified.
    /// </summary>
    private void SetTopN()
    {
        if (TopN > 0)
        {
            gridElem.TopN = TopN;
            gridElem.Pager.DisplayPager = false;
        }
    }


    /// <summary>
    /// Returns page size for unigrid.
    /// </summary>
    private int GetPageSize()
    {
        switch (PageSize)
        {
            case -1:
            case 10:
            case 25:
            case 50:
            case 100:
                return PageSize;
            default:
                return 10;
        }
    }

    /// <summary>
    /// Returns string for ORDER BY clause.
    /// </summary>
    private string GetOrderBy()
    {
        // OrderBy specified by drop-down list
        string column = null;
        switch (OrderBy)
        {
            case ORDER_BY_NAME_DESC:
                column = "SKUName DESC";
                break;

            case ORDER_BY_NUMBER_ASC:
                column = "SKUNumber ASC";
                break;

            case ORDER_BY_NUMBER_DESC:
                column = "SKUNumber DESC";
                break;

            case ORDER_BY_PRICE_ASC:
                column = "SKUPrice ASC";
                break;

            case ORDER_BY_PRICE_DESC:
                column = "SKUPrice DESC";
                break;

            case ORDER_BY_DATE_DESC:
                column = "SKUCreated DESC";
                break;

            case ORDER_BY_DATE_ASC:
                column = "SKUCreated ASC";
                break;

            case ORDER_BY_AVAILABLE_ITEMS_ASC:
                column = "SKUAvailableItems ASC";
                break;

            case ORDER_BY_REORDER_DESC:
                column = "(ISNULL(SKUReorderAt,0) - SKUAvailableItems) DESC";
                break;

            case ORDER_BY_NAME_ASC:
            default:
                column = "SKUName ASC";
                break;
        }

        // Disables remembering unigrid state
        gridElem.RememberState = false;

        return SqlHelper.AddOrderBy("", column);
    }

    #endregion
}