using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Action(0, "Forum_List.NewItemCaption", "Forum_New.aspx?groupId={?parentobjectid?}")]
public partial class CMSModules_Forums_Tools_Forums_Forum_List : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupId = QueryHelper.GetInteger("parentobjectid", 0);

        forumList.GroupID = groupId;
        forumList.OnAction += new CommandEventHandler(forumList_OnAction);
        forumList.IsLiveSite = false;

        InitializeMasterPage(groupId);
    }


    protected void forumList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToString())
        {
            case "edit":
                URLHelper.Redirect(URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("cms.forums","EditForum"), "forumid" , e.CommandArgument.ToString()));
                break;
            default:
                forumList.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage(int groupId)
    {
        // Set title
        Title = "Forum List";
    }
}