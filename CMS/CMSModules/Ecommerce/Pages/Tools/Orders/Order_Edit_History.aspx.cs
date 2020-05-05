using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.ECOMMERCE, "Orders.History")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_History : CMSEcommercePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var orderId = QueryHelper.GetInteger("orderid", 0);

        var oi = OrderInfo.Provider.Get(orderId);
        EditedObject = oi;

        // Check order site ID
        CheckEditedObjectSiteID(oi.OrderSiteID);

        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.WhereCondition = "OrderID = " + orderId;
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // Show tag for order status column
        if (sourceName.EqualsCSafe("statusName"))
        {
            var statusId = ValidationHelper.GetInteger(parameter, 0);
            var status = OrderStatusInfo.Provider.Get(statusId);

            if (status != null)
            {
                return new Tag
                       {
                           Text = status.StatusDisplayName,
                           Color = status.StatusColor
                       };
            }
        }

        return parameter;
    }
}