using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_OrderStatusSelector : SiteSeparatedObjectSelector
{
    #region "Properties"

    /// <summary>
    ///  If true, selected value is OrderStatusName, if false, selected value is OrderStatusID.
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
    /// Allows to access uniselector object
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Convert given status name to its ID for specified site.
    /// </summary>
    /// <param name="name">Code name of the order status.</param>
    /// <param name="siteName">Name of the site to translate code name for.</param>
    protected override int GetID(string name, string siteName)
    {
        var status = OrderStatusInfo.Provider.Get(name, SiteInfoProvider.GetSiteID(siteName));

        return (status != null) ? status.StatusID : 0;
    }

    #endregion
}