using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_ManufacturerSelector : SiteSeparatedObjectSelector
{
    #region "Variables"

    private bool mReflectGlobalProductsUse;

    #endregion


    #region "Properties"

    /// <summary>
    ///  If true, selected value is ManufacturerName, if false, selected value is ManufacturerID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseManufacturerNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseManufacturerNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Allows to access uniselector object
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Indicates whether global items are to be offered.
    /// </summary>
    public override bool DisplayGlobalItems
    {
        get
        {
            return base.DisplayGlobalItems || (ReflectGlobalProductsUse && ECommerceSettings.AllowGlobalProducts(SiteID));
        }
        set
        {
            base.DisplayGlobalItems = value;
        }
    }


    /// <summary>
    /// Gets or sets a value that indicates if the global items should be displayed when the global products are used on the site.
    /// </summary>
    public bool ReflectGlobalProductsUse
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ReflectGlobalProductsUse"), mReflectGlobalProductsUse);
        }
        set
        {
            SetValue("ReflectGlobalProductsUse", value);
            mReflectGlobalProductsUse = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Convert given manufacturer name to its ID for specified site.
    /// </summary>
    /// <param name="name">Code name of the manufacturer.</param>
    /// <param name="siteName">Name of the site to translate code name for.</param>
    protected override int GetID(string name, string siteName)
    {
        ManufacturerInfo manufacturerInfoObj = ManufacturerInfo.Provider.Get(name, SiteInfoProvider.GetSiteID(siteName));

        return manufacturerInfoObj?.ManufacturerID ?? 0;
    }

    #endregion
}