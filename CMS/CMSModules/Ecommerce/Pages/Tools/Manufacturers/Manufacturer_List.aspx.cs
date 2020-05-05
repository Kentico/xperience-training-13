using System;

using CMS.Base;
using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("Manufacturer_Edit.ItemListLink")]
[Action(0, "Manufacturer_List.NewItemCaption", "{%UIContextHelper.GetElementUrl(\"CMS.Ecommerce\", \"NewManufacturer\", \"false\")|(encode)false%}")]
[UIElement(ModuleName.ECOMMERCE, "Manufacturers")]
public partial class CMSModules_Ecommerce_Pages_Tools_Manufacturers_Manufacturer_List : CMSManufacturersPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        HandleGridsSiteIDColumn(UniGrid);
        UniGrid.WhereCondition = InitSiteWhereCondition("ManufacturerSiteID").ToString(true);
    }
    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            var url = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "EditManufacturer", false, actionArgument.ToInteger(0));
            url = URLHelper.AddParameterToUrl(url, "action", "edit");
            URLHelper.Redirect(url);
        }
    }

    #endregion
}