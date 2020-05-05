using System;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "InternalStatus_Edit.ItemListLink", "InternalStatus_List.aspx?siteId={?siteId?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "InternalStatus_Edit.NewItemCaption", NewObject = true)]
// Edited object
[EditedObject(InternalStatusInfo.OBJECT_TYPE, "statusId")]
// Title
[Title("InternalStatus_Edit.HeaderCaption", ExistingObject = true)]
[Title("InternalStatus_New.HeaderCaption", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_InternalStatus_InternalStatus_Edit : CMSInternalStatusesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if not editing object from another site
        InternalStatusInfo internalStatusObj = EditedObject as InternalStatusInfo;
        if ((internalStatusObj != null) && (internalStatusObj.InternalStatusID > 0))
        {
            CheckEditedObjectSiteID(internalStatusObj.InternalStatusSiteID);
        }

        // Register check permissions
        EditForm.OnCheckPermissions += (s, args) => CheckConfigurationModification();
    }


    protected void EditForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        EditForm.ObjectSiteID = ConfiguredSiteID;
    }
}