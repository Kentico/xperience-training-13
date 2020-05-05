using System;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "PublicStatus_Edit.ItemListLink", "PublicStatus_List.aspx?siteId={?siteId?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, "PublicStatus_Edit.NewItemCaption", NewObject = true)]
// Edited object
[EditedObject(PublicStatusInfo.OBJECT_TYPE, "publicStatusId")]
// Title
[Title("PublicStatus_Edit.HeaderCaption", ExistingObject = true)]
[Title("PublicStatus_New.HeaderCaption", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_PublicStatus_PublicStatus_Edit : CMSPublicStatusesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register check permissions
        EditForm.OnCheckPermissions += (s, args) => CheckConfigurationModification();

        PublicStatusInfo publicStatusObj = EditedObject as PublicStatusInfo;
        if ((publicStatusObj != null) && (publicStatusObj.PublicStatusID > 0))
        {
            // Check if not editing object from another site
            CheckEditedObjectSiteID(publicStatusObj.PublicStatusSiteID);
        }
    }


    protected void EditForm_OnBeforeDataLoad(object sender, EventArgs e)
    {
        EditForm.ObjectSiteID = ConfiguredSiteID;
    }
}