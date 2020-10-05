using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_CMSPages_GetInvoice : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var orderId = QueryHelper.GetInteger("orderid", 0);
        var order = OrderInfo.Provider.Get(orderId);
        
        if (order != null)
        {
            var currentUser = MembershipContext.AuthenticatedUser;
            var customer = CustomerInfo.Provider.Get(order.OrderCustomerID);
            var siteName = SiteInfoProvider.GetSiteName(order.OrderSiteID);

            // To see invoice user needs to be global admin or have read order permission or access her own order
            if (((customer != null) && (customer.CustomerUserID == currentUser.UserID)) ||
                currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin) ||
                order.CheckPermissions(PermissionsEnum.Read, siteName, currentUser))
            {
                ltlInvoice.Text = order.OrderInvoice;
            }
        }
    }
}