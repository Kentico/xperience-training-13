using System;

using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;

// Help
[Help("activity_new", "helptopic")]
[UIElement(ModuleName.ACTIVITIES, "Activities")]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_Activities_Pages_Tools_Activities_Activity_New : CMSContactManagementPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        int siteId = QueryHelper.GetInteger("siteid", 0);
        int contactId = QueryHelper.GetInteger("contactid", 0);

        // Init breadcrumbs
        var listBreadCrumb = new BreadcrumbItem { 
            Text = GetString("om.activity.list"),
            RedirectUrl = "~/CMSModules/Activities/Pages/Tools/Activities/Activity/List.aspx" 
        };
        var newItemBreadCrumb = new BreadcrumbItem { 
            Text = GetString("om.activity.newcustom") 
        };

        if (contactId > 0)
        {
            // New custom activity page was opened from pages of edited contact
            listBreadCrumb.RedirectUrl = "~/CMSModules/ContactManagement/Pages/Tools/Contact/Tab_Activities.aspx?contactId=" + contactId;
        }

        listBreadCrumb.RedirectUrl = AddSiteQuery(listBreadCrumb.RedirectUrl, siteId);
        PageBreadcrumbs.AddBreadcrumb(listBreadCrumb);
        PageBreadcrumbs.AddBreadcrumb(newItemBreadCrumb);
    }

    #endregion
}