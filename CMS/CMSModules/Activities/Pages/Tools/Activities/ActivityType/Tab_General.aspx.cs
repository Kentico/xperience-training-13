using System;

using CMS.Activities;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;

// Edited object
[EditedObject(ActivityTypeInfo.OBJECT_TYPE, "typeId")]

// Help
[Help("activitytype_edit", "helptopic")]
[Breadcrumb(0, "om.activitytype.list", "~/CMSModules/Activities/Pages/Tools/Activities/ActivityType/List.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}")]
[UIElement(ModuleName.ONLINEMARKETING, "ActivityTypes")]
[Security(GlobalAdministrator = true)]
public partial class CMSModules_Activities_Pages_Tools_Activities_ActivityType_Tab_General : CMSContactManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
}