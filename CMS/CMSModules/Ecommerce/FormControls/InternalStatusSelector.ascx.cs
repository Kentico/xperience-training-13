using System;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_InternalStatusSelector : SiteSeparatedObjectSelector
{
    #region "Variables"

    private bool mReflectGlobalProductsUse = false;

    #endregion


    #region "Properties"

    /// <summary>
    ///  If true, selected value is InternalStatusName, if false, selected value is InternalStatusID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseStatusNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseStatusNameForSelection", value);
            base.UseNameForSelection = value;
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


    /// <summary>
    /// Allows to access uniselector object.
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Convert given status name to its ID for specified site.
    /// </summary>
    /// <param name="name">Code name of the internal status to be converted.</param>
    /// <param name="siteName">Name of the site to translate code name for.</param>
    protected override int GetID(string name, string siteName)
    {
        InternalStatusInfo status = InternalStatusInfo.Provider.Get(name, SiteInfoProvider.GetSiteID(siteName));

        return (status != null) ? status.InternalStatusID : 0;
    }

    #endregion


    #region "Lifecycle"

    protected override void OnPreRender(EventArgs e)
    {
        if (RequestHelper.IsPostBack() && DependsOnAnotherField)
        {
            InitSelector();
            uniSelector.Reload(true);
        }

        base.OnPreRender(e);
    }

    #endregion
}