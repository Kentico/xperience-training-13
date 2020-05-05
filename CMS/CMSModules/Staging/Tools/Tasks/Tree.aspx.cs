using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization.Web.UI;
using CMS.UIControls;


[UIElement("CMS.Staging", "Documents")]
public partial class CMSModules_Staging_Tools_Tasks_Tree : CMSStagingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // check 'Manage servers' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.staging", "ManageDocumentsTasks"))
        {
            RedirectToAccessDenied("cms.staging", "ManageDocumentsTasks");
        }

        ltlScript.Text = ScriptHelper.GetScript("treeUrl = '" + ResolveUrl("~/CMSModules/Staging/Tools/Tasks/tree.aspx") + "';");

        treeContent.NodeTextTemplate = "<span id=\"##NODEID##\" class=\"ContentTreeItem\" onclick=\"SelectNode(##NODEID##); return false;\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        treeContent.SelectedNodeTextTemplate = "<span id=\"##NODEID##\" class=\"ContentTreeSelectedItem\" onclick=\"SelectNode(##NODEID##); return false;\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        treeContent.MaxTreeNodeText = "<span id=\"##NODEID##\" class=\"ContentTreeItem\" onclick=\"SelectNode(##PARENTNODEID##, true); return false;\"><span class=\"Name\">" + GetString("general.SeeListing") + "</span></span>";
        // If no node specified, select root node id
        int nodeId = QueryHelper.GetInteger("stagingnodeid", 0);
        if (nodeId <= 0)
        {
            // Get the root node
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            TreeNode rootNode = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", TreeProvider.ALL_CULTURES, false, null, false);
            if (rootNode != null)
            {
                nodeId = rootNode.NodeID;
            }
        }

        // If nodeId set, init the list of the nodes to expand
        int expandNodeId = QueryHelper.GetInteger("expandnodeid", 0);
        treeContent.ExpandNodeID = expandNodeId;

        // Current Node ID
        treeContent.SelectedNodeID = nodeId;

        // Setup the current node script
        if (nodeId > 0)
        {
            ltlScript.Text += ScriptHelper.GetScript("    currentNodeId = " + nodeId + ";");
        }
    }
}
