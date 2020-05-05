using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.UIControls;


[EditedObject(OrderInfo.OBJECT_TYPE, "orderId")]
[UIElement(ModuleName.ECOMMERCE, "Orders.General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_General : CMSEcommercePage
{
    #region "Page events"

    /// <summary>
    /// Page OnInit event.
    /// </summary>
    /// <param name="e">Event args</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        editOrderGeneral.OnCheckPermissions += editOrderGeneral_OnCheckPermissions;
        editOrderGeneral.OnAfterDataLoad += editOrderGeneral_OnAfterDataLoad;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { window.location.replace(window.location.href); }"));

        var order = EditedObject as OrderInfo;
        if (order != null)
        {
            // Check order site ID
            CheckEditedObjectSiteID(order.OrderSiteID);

            // Hide select and clear buttons
            var addressDetail = editOrderGeneral.FieldControls["OrderCompanyAddressID"] as UniSelector;
            if (addressDetail != null)
            {
                addressDetail.Value = order.OrderCompanyAddress?.AddressID;

                addressDetail.ButtonSelect.Visible = false;
                addressDetail.ButtonClear.Visible = false;
            }
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Checks modify permission.
    /// </summary>
    protected void editOrderGeneral_OnCheckPermissions(object sender, EventArgs e)
    {
        // Check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission(EcommercePermissions.ORDERS_MODIFY))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, "EcommerceModify OR ModifyOrders");
        }
    }


    /// <summary>
    /// Disables datetime picker in form.
    /// </summary>
    protected void editOrderGeneral_OnAfterDataLoad(object sender, EventArgs e)
    {
        if (editOrderGeneral.FieldControls["orderdate"] != null)
        {
            editOrderGeneral.FieldControls["orderdate"].Enabled = false;
        }
    }

    #endregion
}
