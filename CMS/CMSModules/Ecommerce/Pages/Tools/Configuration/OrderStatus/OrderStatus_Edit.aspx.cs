using System;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "OrderStatus_Edit.ItemListLink", "OrderStatus_List.aspx?siteId={?siteId?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "OrderStatus_Edit.NewItemCaption", NewObject = true)]
// Edited object
[EditedObject(OrderStatusInfo.OBJECT_TYPE, "orderStatusId")]
// Title
[Title("OrderStatus_Edit.HeaderCaption", ExistingObject = true)]
[Title("OrderStatus_Edit.NewItemCaption", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_OrderStatus_OrderStatus_Edit : CMSOrderStatusesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register check permissions
        EditForm.OnCheckPermissions += (s, args) => CheckConfigurationModification();

        OrderStatusInfo orderStatusObj = EditedObject as OrderStatusInfo;
        if ((orderStatusObj != null) && (orderStatusObj.StatusID > 0))
        {
            // Check if not editing object from another site
            CheckEditedObjectSiteID(orderStatusObj.StatusSiteID);
        }

        // Register new object handling
        EditForm.OnBeforeSave += (s, args) =>
        {
            OrderStatusInfo status = EditForm.Data as OrderStatusInfo;
            if ((status != null) && (status.StatusID == 0))
            {
                // Assign order - place 
                status.StatusOrder = status.Generalized.GetLastObjectOrder(null);
            }
        };
    }


    protected void EditForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        EditForm.ObjectSiteID = ConfiguredSiteID;
    }
}