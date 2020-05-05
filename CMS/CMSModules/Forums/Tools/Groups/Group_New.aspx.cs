using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("Group_General.NewGroup")]
[UIElement(ModuleName.FORUMS, "NewForumGroup")]
public partial class CMSModules_Forums_Tools_Groups_Group_New : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        groupNewElem.OnSaved += new EventHandler(groupNewElem_OnSaved);
        groupNewElem.IsLiveSite = false;
        InitializeMasterPage();
    }


    protected void groupNewElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect(URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("cms.forums","EditForumGroup", false),"objectid" , groupNewElem.GroupID.ToString()));
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("forumgrouplist.headerforumgrouplist"),
            RedirectUrl = UIContextHelper.GetElementUrl("cms.forums", "Forums", false),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Group_General.NewGroup"),
        });
    }
}