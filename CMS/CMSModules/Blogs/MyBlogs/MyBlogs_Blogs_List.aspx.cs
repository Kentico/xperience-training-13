using System;
using System.Data;

using CMS.Base;

using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Blogs;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.BLOGS, "MyBlogsList")]
public partial class CMSModules_Blogs_MyBlogs_MyBlogs_Blogs_List : CMSContentManagementPage
{
    #region "Variables"

    protected CurrentUserInfo currentUser;
    private bool contentExploreTreePermission;
    private bool contentReadPermission;
    private bool contentCreatePermission;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        gridBlogs.ZeroRowsText = GetString("mydesk.ui.noblogs");
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = MembershipContext.AuthenticatedUser;
        if (currentUser == null)
        {
            return;
        }

        // No cms.blog doc. type
        if (DataClassInfoProvider.GetDataClassInfo("cms.blog") == null)
        {
            RedirectToInformation(GetString("blog.noblogdoctype"));
        }

        // Prepare permissions for external data bound
        contentExploreTreePermission = currentUser.IsAuthorizedPerResource("cms.content", "exploretree");
        contentReadPermission = currentUser.IsAuthorizedPerResource("cms.content", "read");
        contentCreatePermission = currentUser.IsAuthorizedPerResource("cms.content", "create");

        // Register grid events
        gridBlogs.OnExternalDataBound += gridBlogs_OnExternalDataBound;
        gridBlogs.OnDataReload += gridBlogs_OnDataReload;
        gridBlogs.ShowActionsMenu = true;
        gridBlogs.Columns = "BlogID, ClassName, BlogName, NodeID, DocumentCulture, NodeOwner, BlogModerators";

        // Get all possible columns to retrieve
        gridBlogs.AllColumns = SqlHelper.JoinColumnList(ObjectTypeManager.GetColumnNames(BlogHelper.BLOG_OBJECT_TYPE));
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        var blogPostClass = DataClassInfoProvider.GetDataClassInfo("cms.blogpost");

        gridBlogs.JavaScriptModule = "CMS.Blogs/BlogsGrid";
        gridBlogs.JavaScriptModuleData = new
        {
            PagesApplicationHash = ApplicationUrlHelper.GetApplicationHash("cms.content", "content"),
            GridSelector = "#" + gridBlogs.ClientID,
            BlogPostClassId = blogPostClass != null ? blogPostClass.ClassID.ToString() : String.Empty
        };
    }

    #endregion


    #region "UniGrid Events"

    protected DataSet gridBlogs_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        totalRecords = -1;
        return BlogHelper.GetBlogs(SiteContext.CurrentSiteName, currentUser.UserID, null, columns, completeWhere);
    }


    protected object gridBlogs_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        var lowerSourceName = sourceName.ToLowerCSafe();
        switch (lowerSourceName)
        {
            case "edit":
            case "newpost":
                // Get dataItemRow from which we can receive parameters specified in grid xml definition
                var dataItemRow = ((DataRowView)(((GridViewRow)(parameter)).DataItem)).Row;
                var nodeId = dataItemRow.Field<Int32>("NodeID");
                var documentCulture = dataItemRow.Field<String>("DocumentCulture");
                var button = ((CMSGridActionButton)sender);

                // Register data attributes for use in JavaScript module
                button.Attributes.Add("data-node-id", nodeId.ToString());
                button.Attributes.Add("data-document-culture", documentCulture);

                if ((!contentExploreTreePermission || !contentReadPermission) || ((lowerSourceName == "newpost") && !contentCreatePermission))
                {
                    // User has to have 'exploretree' and 'read' permissions for content to be able to view the blog
                    button.ToolTip = GetString("blogs.permissions.content");
                    button.Enabled = false;
                }

                break;
        }

        return parameter;
    }

    #endregion
}