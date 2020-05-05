using System;

using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;

// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "om.activitytype.list", "~/CMSModules/Activities/Pages/Tools/Activities/ActivityType/List.aspx", null)]
[Breadcrumb(1, "om.activitytype.new")]
// Help
[Help("activitytype_new", "helptopic")]
[UIElement(ModuleName.ONLINEMARKETING, "ActivityTypes")]
[Security(GlobalAdministrator = true)]
public partial class CMSModules_Activities_Pages_Tools_Activities_ActivityType_New : CMSContactManagementPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageBreadcrumbs.Items[0].RedirectUrl = AddSiteQuery(PageBreadcrumbs.Items[0].RedirectUrl, QueryHelper.GetInteger("siteid", 0));
    }

    #endregion
}