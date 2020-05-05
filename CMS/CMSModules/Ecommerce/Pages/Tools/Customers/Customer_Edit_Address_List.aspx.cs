using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject(CustomerInfo.OBJECT_TYPE, "customerId")]
[Action(0, "Customer_Edit_Address_List.NewItemCaption", "Customer_Edit_Address_Edit.aspx?customerId={%EditedObject.ID%}")]
[UIElement(ModuleName.ECOMMERCE, "Customers.Addresses")]
public partial class CMSModules_Ecommerce_Pages_Tools_Customers_Customer_Edit_Address_List : CMSEcommercePage
{
    protected CustomerInfo customer = null;


    protected void Page_Load(object sender, EventArgs e)
    {
        customer = EditedObject as CustomerInfo;
        if (customer != null)
        {
            UniGrid.OnAction += uniGrid_OnAction;
            UniGrid.WhereCondition = "AddressCustomerID = " + customer.CustomerID;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        int addressId = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("Customer_Edit_Address_Edit.aspx?customerId=" + customer.CustomerID + "&addressId=" + addressId));
        }
    }
}