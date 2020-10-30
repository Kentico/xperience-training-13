using System;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[ParentObject(ShippingOptionInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.ECOMMERCE, "Configuration.ShippingOptions.ShippingCosts")]
[Action(0, "com.ui.shippingcost.edit_new", "ShippingOption_Edit_ShippingCosts_Edit.aspx?objectid={%EditedObjectParent.ID%}&siteId={?siteId?}")]
[Help("shippingcosts_list", "helpTopic")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_ShippingCosts : CMSShippingOptionsPage
{
    #region "Variables"

    private CurrencyInfo currency = null;

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var shippingOption = EditedObjectParent as ShippingOptionInfo;
        if (shippingOption != null)
        {
            CheckEditedObjectSiteID(shippingOption.ShippingOptionSiteID);
            currency = CurrencyInfoProvider.GetMainCurrency(shippingOption.ShippingOptionSiteID);
        }

        // Init unigrid
        gridElem.ZeroRowsText = GetString("com.ui.shippingcost.edit_nodata");
        gridElem.GridView.RowDataBound += gridElem_RowDataBound;
        gridElem.GridView.AllowSorting = false;
    }

    #endregion


    #region "Event handlers"

    private void gridElem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            e.Row.Cells[1].Text += $" ({ECommerceSettings.MassUnit()})";
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (String.Equals(sourceName, "shippingcostvalue", StringComparison.OrdinalIgnoreCase))
        {
            var value = ValidationHelper.GetDecimal(parameter, 0);

            return CurrencyInfoProvider.GetFormattedPrice(value, currency);
        }

        return parameter;
    }

    #endregion
}