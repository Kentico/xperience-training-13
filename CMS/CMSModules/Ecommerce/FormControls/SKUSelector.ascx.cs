using System;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_SKUSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mDisplayOnlyEnabled = true;
    private bool? mAllowGlobalProducts;
    private bool? mAllowGlobalOptions;
    private bool? mDisplaySiteProducts;
    private bool? mDisplayGlobalProducts;
    private bool? mDisplaySiteOptions;
    private bool? mDisplayGlobalOptions;

    private int mSiteId = -1;
    private int mProductOptionCategoryID;
    private string mAdditionalItems = "";

    private bool mAllowEmpty;
    private bool mUseGUIDForSelection;
    private bool mDisplayStandardProducts = true;
    private bool mDisplayProductVariants;
    private bool mDisplayMemberships = true;
    private bool mDisplayEproducts = true;
    private bool mDisplayBundles = true;
    private bool mDisplayOnlyProductsWithoutOptions;
    private bool mAllowMultipleChoice;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether also product options should be displayed.
    /// </summary>
    public bool DisplayProductOptions
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether also product variants should be displayed.
    /// </summary>
    public bool DisplayProductVariants
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayProductVariants"), mDisplayProductVariants);
        }
        set
        {
            SetValue("DisplayProductVariants", value);
            mDisplayProductVariants = value;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            // For multiple choice, text with IDs/GUIDs is returned
            if (AllowMultipleChoice)
            {
                return uniSelector.Value;
            }

            // Use GUID
            if (UseGUIDForSelection)
            {
                if (SKUGUID != Guid.Empty)
                {
                    return SKUGUID;
                }

                return null;
            }

            // If any product in single selection mode is selected 
            if (SKUID == 0)
            {
                return null;
            }

            // Use ID
            return SKUID;
        }
        set
        {
            // For multiple choice, text with IDs/GUIDs is set
            if (AllowMultipleChoice)
            {
                // Ensure uniselector's selection mode
                uniSelector.SelectionMode = SelectionModeEnum.Multiple;

                uniSelector.Value = value;
            }
            else
            {
                // Ensure uniselector's selection mode
                uniSelector.SelectionMode = SelectionModeEnum.SingleTextBox;
                // Use GUID
                if (UseGUIDForSelection)
                {
                    SKUGUID = ValidationHelper.GetGuid(value, Guid.Empty);
                }
                // Use ID
                else
                {
                    SKUID = ValidationHelper.GetInteger(value, 0);
                }
            }
        }
    }


    /// <summary>
    /// Product display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return uniSelector.ValueDisplayName;
        }
    }


    /// <summary>
    /// Gets or sets the SKU ID.
    /// </summary>
    public int SKUID
    {
        get
        {
            // Use GUID
            if (UseGUIDForSelection)
            {
                Guid guid = ValidationHelper.GetGuid(uniSelector.Value, Guid.Empty);

                SKUInfo skui = SKUInfo.Provider.Get(guid);

                return (skui != null) ? skui.SKUID : 0;
            }

            // Use ID
            return ValidationHelper.GetInteger(uniSelector.Value, 0);
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            // Use GUID
            if (UseGUIDForSelection)
            {
                SKUInfo skui = SKUInfo.Provider.Get(value);

                if (skui != null)
                {
                    uniSelector.Value = skui.SKUGUID;
                }
            }
            // Use ID
            else
            {
                uniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Selected SKU GUID.
    /// </summary>
    public Guid SKUGUID
    {
        get
        {
            // Use GUID
            if (UseGUIDForSelection)
            {
                return ValidationHelper.GetGuid(uniSelector.Value, Guid.Empty);
            }
            // Use ID
            int id = ValidationHelper.GetInteger(uniSelector.Value, 0);

            SKUInfo skui = SKUInfo.Provider.Get(id);

            return (skui != null) ? skui.SKUGUID : Guid.Empty;
        }
        set
        {
            // Use GUID
            if (UseGUIDForSelection)
            {
                uniSelector.Value = value;
            }
            // Use ID
            else
            {
                SKUInfo skui = SKUInfo.Provider.Get(value);

                if (skui != null)
                {
                    uniSelector.Value = skui.SKUID;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the CMSDropDownList.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Indicates if site products will be displayed. By default, value is based on SiteID property and global objects setting.
    /// </summary>
    public bool DisplaySiteProducts
    {
        get
        {
            // Unknown yet
            if (!mDisplaySiteProducts.HasValue)
            {
                // Display site item when working with site records
                mDisplaySiteProducts = (SiteID != 0);
            }

            return mDisplaySiteProducts.Value;
        }
        set
        {
            mDisplaySiteProducts = value;
        }
    }


    /// <summary>
    /// Indicates if global products will be displayed. By default, value is based on SiteID property and global objects setting.
    /// </summary>
    public bool DisplayGlobalProducts
    {
        get
        {
            // Unknown yet
            if (!mDisplayGlobalProducts.HasValue)
            {
                mDisplayGlobalProducts = false;
                if ((SiteID == 0) || AllowGlobalProducts)
                {
                    mDisplayGlobalProducts = true;
                }
            }

            return mDisplayGlobalProducts.Value;
        }
        set
        {
            mDisplayGlobalProducts = value;
        }
    }


    /// <summary>
    /// Indicates if site options will be displayed. By default, value is based on SiteID property and global objects setting.
    /// </summary>
    public bool DisplaySiteOptions
    {
        get
        {
            // Unknown yet
            if (!mDisplaySiteOptions.HasValue)
            {
                // Display site item when working with site records
                mDisplaySiteOptions = (SiteID != 0);
            }

            return mDisplaySiteOptions.Value;
        }
        set
        {
            mDisplaySiteOptions = value;
        }
    }


    /// <summary>
    /// Indicates if global options will be displayed. By default, value is based on SiteID property and global objects setting.
    /// </summary>
    public bool DisplayGlobalOptions
    {
        get
        {
            // Unknown yet
            if (!mDisplayGlobalOptions.HasValue)
            {
                mDisplayGlobalOptions = false;
                if ((SiteID == 0) || AllowGlobalOptions)
                {
                    mDisplayGlobalOptions = true;
                }
            }

            return mDisplayGlobalOptions.Value;
        }
        set
        {
            mDisplayGlobalOptions = value;
        }
    }


    /// <summary>
    /// Allows to display SKUs only for specified site id. Use 0 for global SKUs levels. Default value is current site id. 
    /// </summary>
    public int SiteID
    {
        get
        {
            // No site id given
            if (mSiteId == -1)
            {
                mSiteId = SiteContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;

            mDisplayGlobalProducts = null;
            mDisplaySiteProducts = null;
            mAllowGlobalOptions = null;
            mAllowGlobalProducts = null;
        }
    }


    /// <summary>
    /// Allows to display only enabled items. Default value is true.
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
    /// Id of items which have to be displayed. Use ',' or ';' as separator if more ids required.
    /// </summary>
    public string AdditionalItems
    {
        get
        {
            return mAdditionalItems;
        }
        set
        {
            // Prevent from setting null value
            mAdditionalItems = (value != null) ? value.Replace(';', ',') : "";
        }
    }


    /// <summary>
    /// UniSelector object used for selection.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Indicates if it should be possible to clear the selection.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEmpty"), mAllowEmpty);
        }
        set
        {
            SetValue("AllowEmpty", value);
            mAllowEmpty = value;
        }
    }


    /// <summary>
    /// Indicates if multiple choice is allowed.
    /// </summary>   
    public bool AllowMultipleChoice
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowMultipleChoice"), mAllowMultipleChoice);
        }
        set
        {
            SetValue("AllowMultipleChoice", value);
            mAllowMultipleChoice = value;
            if (mAllowMultipleChoice)
            {
                // Ensure uniselector's selection mode
                uniSelector.SelectionMode = SelectionModeEnum.Multiple;
            }
        }
    }


    /// <summary>
    /// Indicates if SKU GUID is used for SKU selection.
    /// </summary>
    public bool UseGUIDForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseGUIDForSelection"), mUseGUIDForSelection);
        }
        set
        {
            SetValue("UseGUIDForSelection", value);
            mUseGUIDForSelection = value;
        }
    }


    /// <summary>
    /// Indicates if standard products will be displayed.
    /// </summary>
    public bool DisplayStandardProducts
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayStandardProducts"), mDisplayStandardProducts);
        }
        set
        {
            SetValue("DisplayStandardProducts", value);
            mDisplayStandardProducts = value;
        }
    }


    /// <summary>
    /// Indicates if memberships will be displayed.
    /// </summary>
    public bool DisplayMemberships
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMemberships"), mDisplayMemberships);
        }
        set
        {
            SetValue("DisplayMemberships", value);
            mDisplayMemberships = value;
        }
    }


    /// <summary>
    /// Indicates if e-products will be displayed.
    /// </summary>
    public bool DisplayEproducts
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayEproducts"), mDisplayEproducts);
        }
        set
        {
            SetValue("DisplayEproducts", value);
            mDisplayEproducts = value;
        }
    }


    /// <summary>
    /// Indicates if bundles will be displayed.
    /// </summary>
    public bool DisplayBundles
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayBundles"), mDisplayBundles);
        }
        set
        {
            SetValue("DisplayBundles", value);
            mDisplayBundles = value;
        }
    }


    /// <summary>
    /// Display only products without product options.
    /// </summary>
    public bool DisplayOnlyProductsWithoutOptions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyProductsWithoutOptions"), mDisplayOnlyProductsWithoutOptions);
        }
        set
        {
            SetValue("DisplayOnlyProductsWithoutOptions", value);
            mDisplayOnlyProductsWithoutOptions = value;
        }
    }


    /// <summary>
    /// Product option category ID. If set, only product option from this category will be listed.
    /// </summary>
    public int ProductOptionCategoryID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ProductOptionCategoryID"), mProductOptionCategoryID);
        }
        set
        {
            SetValue("ProductOptionCategoryID", value);
            mProductOptionCategoryID = value;
        }
    }


    /// <summary>
    /// Text to show when no SKUs selected.
    /// </summary>
    public bool DisplayNoDataMessage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayNoDataMessage"), true);
        }
        set
        {
            SetValue("DisplayNoDataMessage", value);
        }
    }

    #endregion


    #region "Protected properties"

    /// <summary>
    /// Returns true if site given by SiteID uses global products beside site-specific ones.
    /// </summary>
    protected bool AllowGlobalProducts
    {
        get
        {
            // Unknown yet
            if (!mAllowGlobalProducts.HasValue)
            {
                mAllowGlobalProducts = false;
                // Try to figure out from settings
                mAllowGlobalProducts = ECommerceSettings.AllowGlobalProducts(SiteID);
            }

            return mAllowGlobalProducts.Value;
        }
    }


    /// <summary>
    /// Returns true if site given by SiteID uses global options beside site-specific ones.
    /// </summary>
    protected bool AllowGlobalOptions
    {
        get
        {
            // Unknown yet
            if (!mAllowGlobalOptions.HasValue)
            {
                mAllowGlobalOptions = false;
                mAllowGlobalOptions = ECommerceSettings.AllowGlobalProductOptions(SiteID);
            }

            return mAllowGlobalOptions.Value;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// OnInit event
    /// </summary>   
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        uniSelector.ReturnColumnName = (UseGUIDForSelection) ? "SKUGUID" : "SKUID";
        uniSelector.AllowEmpty = AllowEmpty;

        // Ensure selection mode
        uniSelector.SelectionMode = AllowMultipleChoice ? SelectionModeEnum.Multiple : SelectionModeEnum.SingleTextBox;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
            return;
        }

        ReloadData();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (!DisplayNoDataMessage)
        {
            uniSelector.ZeroRowsText = string.Empty;
        }


        var where = new WhereCondition();

        if (ProductOptionCategoryID > 0)
        {
            where.WhereEquals("SKUOptionCategoryID", ProductOptionCategoryID);
        }
        else
        {
            var productSiteWhere = new WhereCondition();

            // Add global products
            if (DisplayGlobalProducts)
            {
                productSiteWhere.Where(w => w.WhereNull("SKUOptionCategoryID")
                                             .WhereNull("SKUSiteID"));
            }

            // Add site specific products
            if (DisplaySiteProducts)
            {
                productSiteWhere.Or().Where(w => w.WhereNull("SKUOptionCategoryID")
                                                  .WhereEquals("SKUSiteID", SiteID));
            }

            where.Where(productSiteWhere);
        }

        // Exclude standard products if needed
        if (!DisplayStandardProducts)
        {
            where.WhereNotEquals("SKUProductType", SKUProductTypeEnum.Product.ToStringRepresentation());
        }

        // Exclude memberships if needed
        if (!DisplayMemberships)
        {
            where.WhereNotEquals("SKUProductType", SKUProductTypeEnum.Membership.ToStringRepresentation());
        }

        // Exclude e-products if needed
        if (!DisplayEproducts)
        {
            where.WhereNotEquals("SKUProductType", SKUProductTypeEnum.EProduct.ToStringRepresentation());
        }

        // Exclude bundles if needed
        if (!DisplayBundles)
        {
            where.WhereNotEquals("SKUProductType", SKUProductTypeEnum.Bundle.ToStringRepresentation());
        }

        // Exclude products with product options if needed
        if (DisplayOnlyProductsWithoutOptions)
        {
            where.WhereNotIn("SKUID", new IDQuery<SKUOptionCategoryInfo>("SKUID"));
        }

        if (DisplayProductOptions && (ProductOptionCategoryID <= 0))
        {
            var optionsSiteWhere = new WhereCondition();

            // Add global options
            if (DisplayGlobalOptions)
            {
                optionsSiteWhere.Where(w => w.WhereNotNull("SKUOptionCategoryID")
                                             .WhereNull("SKUSiteID"));
            }

            // Add site specific options
            if (DisplaySiteOptions)
            {
                optionsSiteWhere.Or().Where(w => w.WhereNotNull("SKUOptionCategoryID")
                                                  .WhereEquals("SKUSiteID", SiteID));
            }

            where.Or().Where(optionsSiteWhere);
            where = new WhereCondition().Where(where);
        }

        // Filter out only enabled items
        if (DisplayOnlyEnabled)
        {
            where.WhereTrue("SKUEnabled");
        }

        if (!DisplayProductOptions && (ProductOptionCategoryID <= 0))
        {
            where.WhereNull("SKUOptionCategoryID");
        }

        if (DisplayProductVariants)
        {
            // Alias for COM_SKU 
            var parents = new QuerySourceTable("COM_SKU", "parentIDs");

            // Select IDs of all products that are not parents of variants
            var ids = new IDQuery<SKUInfo>()
                .Columns(new QueryColumn("COM_SKU.SKUID"))
                .Source(s => s.LeftJoin(parents, "COM_SKU.SKUID", "parentIDs.SKUParentSKUID"))
                .Where(new WhereCondition().WhereNull("parentIDs.SKUID"));

            where.WhereIn("SKUID", ids);
        }
        else
        {
            // Do not display variants
            where.WhereNull("SKUParentSKUID");
        }

        // Add items which have to be on the list
        var additionalList = AdditionalItems.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

        if (additionalList.Length > 0)
        {
            var ids = ValidationHelper.GetIntegers(additionalList, 0);
            where.Or().WhereIn("SKUID", ids);
        }

        // Selected value must be on the list
        if (SKUID > 0)
        {
            where.Or().WhereEquals("SKUID", SKUID);
        }

        uniSelector.WhereCondition = where.ToString(true);
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
            if (id > 0)
            {
                return base.GetWhereCondition();
            }
        }
        return null;
    }

    #endregion
}