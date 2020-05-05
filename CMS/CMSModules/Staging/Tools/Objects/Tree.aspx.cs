using System;

using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.Synchronization.Web.UI;
using CMS.UIControls;


[UIElement("CMS.Staging", "Objects")]
public partial class CMSModules_Staging_Tools_Objects_Tree : CMSStagingPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        objectTree.RootNode = StagingTaskInfoProvider.ObjectTree;
        objectTree.NodeTextTemplate = "<span class=\"ContentTreeItem\" onclick=\"SelectNode('##OBJECTTYPE##', ##SITEID##, this); return false;\"><span class=\"Name\">##NODENAME##</span></span>";
        objectTree.SelectedNodeTextTemplate = "<span id=\"treeSelectedNode\" class=\"ContentTreeSelectedItem\" onclick=\"SelectNode('##OBJECTTYPE##', ##SITEID##, this); return false;\"><span class=\"Name\">##NODENAME##</span></span>";
        objectTree.SiteID = SiteContext.CurrentSiteID;

        // Check 'Manage object tasks' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.staging", "ManageObjectsTasks"))
        {
            RedirectToAccessDenied("cms.staging", "ManageObjectsTasks");
        }
    }
}