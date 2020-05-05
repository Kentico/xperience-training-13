using System;

using CMS.Core;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[Title("Customers_Edit.ItemListLink")]
[Action(0, "Customers_List.NewItemCaption", "{%UIContext.GetElementUrl(\"cms.ecommerce\",\"NewCustomer\", false)|(encode)false%}")]
[UIElement(ModuleName.ECOMMERCE, "Customers")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_List : CMSEcommercePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        var id = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            URLHelper.Redirect(UIContextHelper.GetElementUrl(ModuleName.ECOMMERCE, "EditCustomersProperties", false, id));
        }
    }
}