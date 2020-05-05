using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


[Title("om.activitydetails.viewinvoicedetail")]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_Activities_Controls_UI_ActivityDetails_PurchaseDetail : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            return;
        }

        if (!ModuleManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            return;
        }

        int orderId = QueryHelper.GetInteger("orderid", 0);

        // Get order object
        var order = ProviderHelper.GetInfoById(PredefinedObjectType.ORDER, orderId);
        if (order != null)
        {
            ltl.Text = order.GetStringValue("OrderInvoice", "");
        }
    }
}