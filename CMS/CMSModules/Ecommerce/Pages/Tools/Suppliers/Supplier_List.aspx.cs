using System;

using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("supplier_Edit.ItemListLink")]
[Action(0, "supplier_list.newitemcaption", "{%UIContextHelper.GetElementUrl(\"CMS.Ecommerce\", \"new.supplier\", false)|(encode)false%}")]
[UIElement(ModuleName.ECOMMERCE, "Suppliers")]
public partial class CMSModules_Ecommerce_Pages_Tools_Suppliers_Supplier_List : CMSSuppliersPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        gridElem.OnAction += gridElem_OnAction;
        HandleGridsSiteIDColumn(gridElem);
        gridElem.WhereCondition = InitSiteWhereCondition("SupplierSiteID").ToString(true);
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int supplierId = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            var url = UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "edit.supplier", false, supplierId);
            url = URLHelper.AddParameterToUrl(url, "action", "edit");
            URLHelper.Redirect(url);
        }
    }

    #endregion
}