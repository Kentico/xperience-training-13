using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.FORUMS, "NewForum")]
public partial class CMSModules_Forums_Tools_Forums_Forum_New : CMSForumsPage
{
    private int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        groupId = QueryHelper.GetInteger("parentobjectid", 0);

        ForumContext.CheckSite(groupId, 0, 0);

        forumNew.GroupID = groupId;
        forumNew.OnSaved += new EventHandler(forumNew_OnSaved);
        forumNew.IsLiveSite = false;

        InitializeMasterPage();
    }


    protected void forumNew_OnSaved(object sender, EventArgs e)
    {
        string url = UIContextHelper.GetElementUrl("cms.forums", "EditForum", false);
        url = URLHelper.AddParameterToUrl(url, "objectid", forumNew.ForumID.ToString());
        url = URLHelper.AddParameterToUrl(url, "parentobjectid", groupId.ToString());
        URLHelper.Redirect(url);
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        // Initialize help 
        Title = "Forums - New forum";

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("forum_list.headercaption"),
            RedirectUrl = URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("cms.forums", "ForumGroupEditTab_Forums", false), "parentobjectid", groupId.ToString()),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Forum_Edit.NewForum"),
        });
    }
}