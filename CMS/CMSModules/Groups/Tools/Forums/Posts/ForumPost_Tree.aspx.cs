using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Community.Web.UI;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Groups_Tools_Forums_Posts_ForumPost_Tree : CMSGroupForumPage
{
    protected int forumId = 0;


    protected void Page_PreInit(object sender, EventArgs e)
    {
        treeElem.TreeView.CssClass = "ContentTree";
        treeElem.ItemCssClass = "ContentTreeItem";
        treeElem.SelectedItemCssClass = "ContentTreeSelectedItem";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int postId = QueryHelper.GetInteger("postid", 0);
        forumId = QueryHelper.GetInteger("forumid", 0);

        ltlScript.Text = "var selectedPostId = " + postId + ";";
        ltlScript.Text += "function ShowPost(showId){ selectedPostId = showId; \n if(showId==-1) {parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?forumid=' + " + forumId + ";} else {parent.frames['posts_edit'].location.href = 'ForumPost_View.aspx?postid=' + showId;}} \n";
        ltlScript.Text += "var currentNode = document.getElementById('treeSelectedNode');\n function SelectForumNode(nodeElem){ \n if(currentNode != null)  { currentNode.className = 'ContentTreeItem'; } \n if ( nodeElem != null )\n { currentNode = nodeElem;  currentNode.className = 'ContentTreeSelectedItem'; } \n}";

        ltlScript.Text += @"
        // Display listing
        function Listing(postId) {            
            if (postId == null) {
                parent.frames['posts_edit'].location.href = 'ForumPost_Listing.aspx?postid=0;" + forumId + @"';
            } else {
                parent.frames['posts_edit'].location.href = 'ForumPost_Listing.aspx?postid=' + postId;
            }
        }";


        ltlScript.Text += @"
        // Refresh tree and select post
        function RefreshTree(postId) {
            location.replace('ForumPost_Tree.aspx?postid=' + postId + '&forumid=" + forumId + @"');
        }";

        // Wrap with script tag
        ltlScript.Text = ScriptHelper.GetScript(ltlScript.Text);

        // "Click here for more" template
        treeElem.MaxTreeNodeText = "<span class=\"ContentTreeItem\" onclick=\"Listing(##PARENTNODEID##); return false;\"><span class=\"Name\">" + GetString("general.seelisting") + "</span></span>";

        treeElem.ForumID = forumId;

        // Setup the treeview
        treeElem.AdministrationMode = true;
        treeElem.SelectOnlyApproved = false;
        treeElem.UseMaxPostNodes = true;
        treeElem.IsLiveSite = false;

        treeElem.OnGetStatusIcons += treeElem_OnGetStatusIcons;


        if (postId > 0)
        {
            treeElem.Selected = postId;
        }
    }


    /// <summary>
    /// Sets icon handler.
    /// </summary>
    private string treeElem_OnGetStatusIcons(ForumPostTreeNode node)
    {
        if (node == null)
        {
            return null;
        }

        if (!ValidationHelper.GetBoolean(((DataRow)node.ItemData)["PostApproved"], false))
        {
            return UIHelper.GetAccessibleIconTag("NodeLink icon-circle tn color-red-70", GetString("general.notapproved"));
        }

        return null; 
    }
}
