using System;
using System.Collections;
using System.Collections.Generic;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;

using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.DragAndDrop;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_Controls_ContentTree : ContentActionsControl, ICallbackEventHandler
{
    #region "Variables"

    private string mNodeTextTemplate = "##ICON####NODENAME##";
    private string mSelectedNodeTextTemplate = "##ICON####NODENAME##";

    private bool selectedRendered;
    private readonly Hashtable allNodes = new Hashtable();

    private string mPath = "/";
    private string mBasePath;
    private bool? mCheckPermissions;

    protected TreeSiteMapProvider mMapProvider;
    private TreeProvider mMapTreeProvider;

    private int mMaxTreeNodes;
    private string mMaxTreeNodeText;
    private string mPreferredCulture;
    private string mCMSFileIconSet = string.Empty;
    private bool mDeniedNodePostback = true;
    private string mCallbackResult;

    private TreeNode mSelectedNode;


    /// <summary>
    /// Nodes to expand.
    /// </summary>
    protected List<int> expandNodes = new List<int>();

    #endregion


    #region "Events"

    public event EventHandler RootNodeCreated;

    #endregion


    #region "Properties"

    /// <summary>
    /// Folder name where file type icons are located. "List" icon set is used by default.
    /// </summary>
    public string CMSFileIconSet
    {
        get
        {
            if (mCMSFileIconSet == string.Empty)
            {
                return "List";
            }

            return mCMSFileIconSet;
        }
        set
        {
            mCMSFileIconSet = value;
        }
    }


    /// <summary>
    /// True if the special marks (NOTTRANSLATED, REDIRECTION, ...) should be rendered.
    /// </summary>
    public bool AllowMarks
    {
        get;
        set;
    }


    /// <summary>
    /// Preferred tree culture.
    /// </summary>
    public string PreferredCulture
    {
        get
        {
            return mPreferredCulture ?? (mPreferredCulture = LocalizationContext.PreferredCultureCode);
        }
        set
        {
            mPreferredCulture = value;
        }
    }


    /// <summary>
    /// Culture the tree is working with. Uses preferred culture when not set.
    /// </summary>
    public string Culture
    {
        get
        {
            if (ViewState["Culture"] == null)
            {
                ViewState["Culture"] = PreferredCulture;
            }
            return (string)ViewState["Culture"];
        }
        set
        {
            ViewState["Culture"] = value;
            // Set the culture to MapTreeProvider in case it already has been initialized
            MapTreeProvider.PreferredCultureCode = value;
        }
    }


    /// <summary>
    /// Template of the node text, use {0} to insert the original node text, {1} to insert the Node ID.
    /// </summary>
    public string NodeTextTemplate
    {
        get
        {
            return mNodeTextTemplate;
        }
        set
        {
            mNodeTextTemplate = value;
        }
    }


    /// <summary>
    /// Template of the SelectedNode text, use {0} to insert the original SelectedNode text, {1} to insert the SelectedNode ID.
    /// </summary>
    public string SelectedNodeTextTemplate
    {
        get
        {
            return mSelectedNodeTextTemplate;
        }
        set
        {
            mSelectedNodeTextTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the current node ID.
    /// </summary>
    public int SelectedNodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Selected node document.
    /// </summary>
    public TreeNode SelectedNode
    {
        get
        {
            if (mSelectedNode == null)
            {
                // Set preferred culture
                TreeProvider.PreferredCultureCode = Culture;
                // Get the document
                mSelectedNode = TreeProvider.SelectSingleNode(SelectedNodeID, TreeProvider.ALL_CULTURES, true);

                if (!SelectPublishedData)
                {
                    mSelectedNode = DocumentHelper.GetDocument(mSelectedNode, TreeProvider);
                }

                // Fall back to root if selected node doesn't exist or its alias path is outside of user's starting path
                if (UseFallbackToRoot(mSelectedNode))
                {
                    mSelectedNode = TreeProvider.SelectSingleNode(RootNodeID, TreeProvider.ALL_CULTURES, true);

                    // Register fallback script only if user alias path exists on current site
                    // and the UI is Pages, because in products the fallback will be handled
                    // by a wrapper page (Products_Frameset.aspx.cs)
                    if (!IsProductTree)
                    {
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "fallbackToRoot", GetFallBackToRootScript(false), true);
                    }
                }
            }

            return mSelectedNode;
        }
    }


    /// <summary>
    /// Gets or sets the node ID to expand.
    /// </summary>
    public int ExpandNodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Site name to display.
    /// </summary>
    public string SiteName
    {
        get
        {
            string siteName = ValidationHelper.GetString(ViewState["SiteName"], string.Empty);
            if ((siteName == string.Empty) && (SiteContext.CurrentSite != null))
            {
                siteName = SiteContext.CurrentSiteName;
            }
            return siteName;
        }
        set
        {
            ViewState["SiteName"] = value;
            MapProvider.SiteName = value;
        }
    }


    /// <summary>
    /// Nodes selecting path.
    /// </summary>
    public string Path
    {
        get
        {
            return mPath;
        }
        set
        {
            mPath = value;
            MapProvider.Path = value;
        }
    }


    /// <summary>
    /// Indicates if the permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            if (mCheckPermissions == null)
            {
                mCheckPermissions = TreeProvider.CheckDocumentUIPermissions(SiteName);
            }
            return mCheckPermissions.Value;
        }
        set
        {
            mCheckPermissions = value;
            MapProvider.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Tree provider.
    /// </summary>
    private TreeProvider MapTreeProvider
    {
        get
        {
            if (mMapTreeProvider == null)
            {
                mMapTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser);
                mMapTreeProvider.PreferredCultureCode = Culture;
            }

            return mMapTreeProvider;
        }
    }


    /// <summary>
    /// Sitemap provider that the tree uses.
    /// </summary>
    public TreeSiteMapProvider MapProvider
    {
        get
        {
            if (mMapProvider == null)
            {
                mMapProvider = new TreeSiteMapProvider();

                mMapProvider.TreeProvider = MapTreeProvider;
                mMapProvider.CultureCode = TreeProvider.ALL_CULTURES;
                mMapProvider.SiteName = SiteName;
                mMapProvider.BindNodeData = true;
                mMapProvider.OrderBy = "NodeOrder ASC, NodeName ASC, NodeAlias ASC";
                mMapProvider.SelectOnlyPublished = false;
                mMapProvider.CombineWithDefaultCulture = true;
                mMapProvider.Path = Path;
                mMapProvider.MaxRelativeLevel = 1;
                mMapProvider.CheckPermissions = CheckPermissions;

                // Limit results only if MaxTreeNodes is specified
                if (MaxTreeNodes > 0)
                {
                    mMapProvider.MaxTreeNodes = MaxTreeNodes + 1;
                }
            }

            return mMapProvider;
        }
    }


    /// <summary>
    /// Root node.
    /// </summary>
    public TreeSiteMapNode RootNode
    {
        get
        {
            return (TreeSiteMapNode)MapProvider.RootNode;
        }
    }


    /// <summary>
    /// Root node identifier.
    /// </summary>
    public int RootNodeID
    {
        get
        {
            return (int)RootNode.GetValue("NodeID");
        }
    }


    /// <summary>
    /// Maximum number of tree nodes displayed within the tree. If lower or same as 0 all nodes will be visible.
    /// </summary>
    public int MaxTreeNodes
    {
        get
        {
            if (mMaxTreeNodes <= 0)
            {
                mMaxTreeNodes = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSMaxTreeNodes");
            }
            return mMaxTreeNodes;
        }
        set
        {
            mMaxTreeNodes = value;

            // Limit results only when MaxTreeNodes is specified
            MapProvider.MaxTreeNodes = mMaxTreeNodes > 0 ? value + 1 : mMaxTreeNodes;
        }
    }


    /// <summary>
    /// Text to appear within the latest node when max tree nodes applied.
    /// </summary>
    public string MaxTreeNodeText
    {
        get
        {
            return mMaxTreeNodeText ?? (mMaxTreeNodeText = GetString("general.SeeListing"));
        }
        set
        {
            mMaxTreeNodeText = value;
        }
    }


    /// <summary>
    /// Indicates whether only published document should be displayed.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return MapProvider.SelectOnlyPublished;
        }
        set
        {
            MapProvider.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Indicates whether published document data should be displayed.
    /// </summary>
    public bool SelectPublishedData
    {
        get
        {
            return MapProvider.SelectPublishedData;
        }
        set
        {
            MapProvider.SelectPublishedData = value;
        }
    }


    /// <summary>
    /// Indicates whether access denied node causes postback.
    /// </summary>
    public bool DeniedNodePostback
    {
        get
        {
            return mDeniedNodePostback;
        }
        set
        {
            mDeniedNodePostback = value;
        }
    }


    /// <summary>
    /// True if dragging / dropping of the items is allowed
    /// </summary>
    public bool AllowDragAndDrop
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether the tree is used for browsing products.
    /// </summary>
    public bool IsProductTree
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Enable or disable link action according to user's UI Profile
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "New") &&
            MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "New.LinkExistingDocument"))
        {
            ltlCaptureCueCtrlShift.Text = ScriptHelper.GetScript("var captureCueCtrlShift = true;");
        }
        else
        {
            ltlCaptureCueCtrlShift.Text = ScriptHelper.GetScript("var captureCueCtrlShift = false;");
        }

        bool isRTL = CultureHelper.IsUICultureRTL();
        if (IsLiveSite)
        {
            isRTL = CultureHelper.IsPreferredCultureRTL();
        }

        treeElem.LineImagesFolder = GetImageUrl(isRTL ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree");
        treeElem.ImageSet = TreeViewImageSet.Custom;
        treeElem.ExpandImageToolTip = GetString("general.expand");
        treeElem.CollapseImageToolTip = GetString("general.collapse");
        mBasePath = RequestContext.URL.LocalPath;

        if (!RequestHelper.IsCallback() && !RequestHelper.IsPostBack())
        {
            // Register tree progress icon
            ScriptHelper.RegisterTreeProgress(Page);

            if (AllowDragAndDrop)
            {
                ScriptHelper.RegisterStartupScript(this, typeof(string), "reinitDrag", ScriptHelper.GetScript(
                    @"
if (TreeView_ProcessNodeData) { base_TreeView_ProcessNodeData = TreeView_ProcessNodeData };
TreeView_ProcessNodeData = function(result, context) {
    if (base_TreeView_ProcessNodeData) { base_TreeView_ProcessNodeData(result, context) }
    if (window.lastDragAndDropBehavior) { lastDragAndDropBehavior._initializeDraggableItems(); } 
}
"));
            }
        }
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (!RequestHelper.IsCallback())
        {
            plcDrag.Visible = AllowDragAndDrop;

            if (AllowDragAndDrop)
            {
                // Create drag and drop extender
                DragAndDropExtender extDragDrop = new DragAndDropExtender();

                extDragDrop.ID = "extDragDrop";
                extDragDrop.TargetControlID = "pnlTree";
                extDragDrop.DragItemClass = "DDItem";
                extDragDrop.DragItemHandleClass = "DDHandle";
                extDragDrop.DropCueID = "pnlCue";
                extDragDrop.OnClientDrop = "OnDropNode";

                plcDrag.Controls.Add(extDragDrop);

                pnlCue.Style.Add("display", "none");

                string script =
                    @"
function MoveNodeAsync(nodeId, targetNodeId, position, copy, link) {
    var param = '';
    if (link) {
        if (position) {
            param = 'LinkNodePosition';
        } else {
            param = 'LinkNode';
        }
    } else if (copy) {
        if (position) {
            param = 'CopyNodePosition';
        } else {
            param = 'CopyNode';
        }
    } else {
        if (position) {
            param = 'MoveNodePosition';
        } else {
            param = 'MoveNode';
        }
    }
    param += ';' + nodeId + ';' + targetNodeId;" +
                    GetEventReference("param") + @"
}"
                    ;

                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ContentTree", ScriptHelper.GetScript(script));
            }
            else
            {
                pnlCue.Visible = false;
            }
        }
    }


    /// <summary>
    /// Gets the event reference to the action.
    /// </summary>
    /// <param name="eventArgument">Action to perform</param>
    protected string GetEventReference(string eventArgument)
    {
        return Page.ClientScript.GetCallbackEventReference(this, eventArgument, "DragActionDone", null);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (RequestHelper.IsCallback() || !ReloadData())
        {
            return;
        }

        // If selected node was not rendered (on original load), load it
        if (selectedRendered || (SelectedNode == null))
        {
            return;
        }

        // Ensure all parents of the selected node
        foreach (int nodeId in expandNodes)
        {
            EnsureNode(null, nodeId);
        }

        // Ensure the node itself
        EnsureNode(SelectedNode, 0);
    }


    /// <summary>
    /// Ensures the given node within the tree.
    /// </summary>
    /// <param name="node">Node to ensure</param>
    /// <param name="nodeId">Ensure by NodeID</param>
    protected void EnsureNode(TreeNode node, int nodeId)
    {
        if (node == null)
        {
            // If not already exists, do not add
            if (allNodes[nodeId] != null)
            {
                return;
            }
            else
            {
                // Get the node
                node = TreeProvider.SelectSingleNode(nodeId, TreeProvider.ALL_CULTURES, true);

                if (!SelectPublishedData)
                {
                    node = DocumentHelper.GetDocument(node, TreeProvider);
                }
            }
        }
        else
        {
            nodeId = node.NodeID;
        }

        if (node != null)
        {
            // Get the correct parent node
            System.Web.UI.WebControls.TreeNode parentNode = (System.Web.UI.WebControls.TreeNode)allNodes[node.NodeParentID];
            if (parentNode != null)
            {
                // Expand the parent
                parentNode.Expanded = true;

                // If still not present, add the node
                if (allNodes[nodeId] == null)
                {
                    TreeSiteMapNode sourceNode = new TreeSiteMapNode(MapProvider, nodeId.ToString());
                    sourceNode.TreeNode = node;

                    System.Web.UI.WebControls.TreeNode newNode = CreateNode(sourceNode, 0, true);

                    // If MaxTreeNodes threshold reached, sourceNode must be placed before "Click here..." node
                    if (parentNode.ChildNodes.Count >= MaxTreeNodes)
                    {
                        parentNode.ChildNodes.AddAt(parentNode.ChildNodes.Count - 1, newNode);
                    }
                    else
                    {
                        parentNode.ChildNodes.Add(newNode);
                    }
                }
            }
            else
            {
                // Get the correct node and add it to list of processed nodes
                TreeSiteMapNode targetNode = MapProvider.GetNodeByAliasPath(node.NodeAliasPath);

                if (targetNode != null)
                {
                    List<int> procNodes = new List<int>();
                    procNodes.Add((int)targetNode.NodeData["NodeID"]);

                    if (targetNode.ParentNode != null)
                    {
                        // Repeat until existing parent node in allNodes is found
                        do
                        {
                            int targetParentNodeId = (int)((TreeSiteMapNode)targetNode.ParentNode).NodeData["NodeID"];
                            procNodes.Add(targetParentNodeId);
                            targetNode = (TreeSiteMapNode)targetNode.ParentNode;
                        } while ((targetNode.ParentNode != null) && (allNodes[(int)(((TreeSiteMapNode)(targetNode.ParentNode)).NodeData["NodeID"])] == null));
                    }

                    // Process nodes in reverse order
                    procNodes.Reverse();
                    if (!procNodes.Any(p => (p <= 0)))
                    {
                        foreach (int nodeID in procNodes)
                        {
                            EnsureNode(null, nodeID);
                        }
                    }
                }
            }
        }
    }

    #endregion


    #region "Tree management methods"

    /// <summary>
    /// Reloads the data.
    /// </summary>
    /// <returns>If reloading failed returns false, else returns true</returns>
    public bool ReloadData()
    {
        try
        {
            MapProvider.ReloadData();

            // Expand current node parent
            if ((ExpandNodeID <= 0) && (SelectedNode != null))
            {
                ExpandNodeID = SelectedNode.NodeParentID;
            }

            // If expand node set, set the node to expand
            if (ExpandNodeID > 0)
            {
                // Get node list to expand
                expandNodes.Clear();

                TreeNode node = TreeProvider.SelectSingleNode(ExpandNodeID, TreeProvider.ALL_CULTURES);
                if (node != null)
                {
                    TreeSiteMapNode targetNode = MapProvider.GetNodeByAliasPath(node.NodeAliasPath);
                    if (targetNode != null)
                    {
                        int targetNodeId = (int)targetNode.NodeData["NodeID"];
                        expandNodes.Add(targetNodeId);
                        while (targetNode.ParentNode != null)
                        {
                            int targetParentNodeId = (int)((TreeSiteMapNode)targetNode.ParentNode).NodeData["NodeID"];
                            expandNodes.Add(targetParentNodeId);
                            targetNode = (TreeSiteMapNode)targetNode.ParentNode;
                        }
                    }
                }
            }

            treeElem.Nodes.Clear();

            // Add root node
            treeElem.Nodes.Add(CreateNode(RootNode, 0, false));

            // Raise root node created event
            RaiseRootNodeCreated();

            return true;
        }
        catch (Exception ex)
        {
            lblError.Text = GetString("ContentTree.FailedLoad");

            Service.Resolve<IEventLogService>().LogException("ContentTree", "LOAD", ex, SiteContext.CurrentSiteID);

            return false;
        }
    }


    protected void RaiseRootNodeCreated()
    {
        if (RootNodeCreated == null)
        {
            return;
        }

        RootNodeCreated(treeElem, EventArgs.Empty);
    }


    protected void treeElem_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        e.Node.ChildNodes.Clear();
        e.Node.PopulateOnDemand = false;

        // Use selected node if same as the one being populated
        var nodeId = ValidationHelper.GetInteger(e.Node.Value, 0);
        var selectedNode = SelectedNode;
        var selectedNodeId = selectedNode != null ? selectedNode.NodeID : 0;
        var node = nodeId == selectedNodeId ? selectedNode : TreeProvider.SelectSingleNode(nodeId);

        // Check explore tree permission for current node
        bool userHasExploreTreePermission = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.ExploreTree) == AuthorizationResultEnum.Allowed);
        if (userHasExploreTreePermission)
        {
            SiteMapNodeCollection childNodes = MapProvider.GetChildNodes(nodeId);
            int index = 0;
            foreach (TreeSiteMapNode childNode in childNodes)
            {
                int childNodeId = (int)childNode.NodeData["NodeID"];
                if (childNodeId == nodeId)
                {
                    continue;
                }

                System.Web.UI.WebControls.TreeNode newNode = CreateNode(childNode, index, true);
                e.Node.ChildNodes.Add(newNode);
                index++;
            }
        }
        else
        {
            // Add 'access denied node'
            System.Web.UI.WebControls.TreeNode tempNode = new System.Web.UI.WebControls.TreeNode();
            tempNode.Text = GetString("ContentTree.ExploreChildsDenied");
            tempNode.NavigateUrl = (DeniedNodePostback ? mBasePath + "#" : string.Empty);
            e.Node.ChildNodes.Add(tempNode);
        }
    }


    /// <summary>
    /// Creates the tree node.
    /// </summary>
    /// <param name="sourceNode">Source node</param>
    /// <param name="index">Node index</param>
    /// <param name="childNode">True if the node is child node</param>
    protected System.Web.UI.WebControls.TreeNode CreateNode(TreeSiteMapNode sourceNode, int index, bool childNode)
    {
        System.Web.UI.WebControls.TreeNode newNode = new System.Web.UI.WebControls.TreeNode();
        ISimpleDataContainer container = sourceNode;

        int nodeId = (int)container.GetValue("NodeID");
        int nodeLevel = (int)container.GetValue("NodeLevel");

        if (nodeId < 0)
        {
            newNode.SelectAction = TreeNodeSelectAction.None;
            newNode.Text = GetString("ContentTree.ReadDocumentDenied");
            newNode.NavigateUrl = (DeniedNodePostback ? mBasePath + "#" : string.Empty);
            return newNode;
        }

        // Show complete node if index is lower than MaxTreeNodes or level is lower than RootNodeLevel
        if ((MaxTreeNodes <= 0) || (index < MaxTreeNodes) || (nodeLevel <= MapProvider.RootNodeLevel + 1))
        {
            allNodes[nodeId] = newNode;

            // Set the base data
            newNode.Value = nodeId.ToString();
            newNode.NavigateUrl = "javascript:void(0);";

            int classId = ValidationHelper.GetInteger(container.GetValue("NodeClassID"), 0);
            DataClassInfo ci = DataClassInfoProvider.GetDataClassInfo(classId);
            if (ci == null)
            {
                throw new Exception("[ContentTree.CreateNode]: Node class not found.");
            }

            string className = ci.ClassName;

            var sb = new StringBuilder();
            var iconClass = ValidationHelper.GetString(ci.GetValue("ClassIconClass"), String.Empty);
            var icon = UIHelper.GetDocumentTypeIcon(Page, className, iconClass);
            sb.Append(icon);
            string imageTag = sb.ToString();

            string nodeName = HttpUtility.HtmlEncode(ValidationHelper.GetString(container.GetValue("DocumentName"), string.Empty));
            string nodeNameJava = ScriptHelper.GetString(nodeName);
            string marks = "";

            // Render special marks only if allowed
            if (AllowMarks)
            {
                int workflowStepId = ValidationHelper.GetInteger(container.GetValue("DocumentWorkflowStepID"), 0);
                WorkflowStepTypeEnum stepType = WorkflowStepTypeEnum.Undefined;

                if (workflowStepId > 0)
                {
                    WorkflowStepInfo stepInfo = WorkflowStepInfo.Provider.Get(workflowStepId);
                    if (stepInfo != null)
                    {
                        stepType = stepInfo.StepType;
                    }
                }

                // Add icons
                marks = DocumentUIHelper.GetDocumentMarks(Page, SiteName, Culture, stepType, sourceNode, true);
                if (!string.IsNullOrEmpty(marks))
                {
                    marks = string.Format("<span class=\"tn-group\">{0}</span>", marks);
                }
            }

            string template;

            if ((SelectedNode != null) && (nodeId == SelectedNode.NodeID))
            {
                template = SelectedNodeTextTemplate;
                selectedRendered = true;
            }
            else
            {
                template = NodeTextTemplate;
            }

            // Prepare the node text
            newNode.Text = ResolveNode(template, nodeName, imageTag, nodeNameJava, nodeId, marks);

            // Drag and drop envelope
            if (AllowDragAndDrop)
            {
                sb.Length = 0;

                if (childNode)
                {
                    sb.Append("<span id=\"target_", nodeId, "\"><span class=\"DDItem\" id=\"node_", nodeId, "\"><span class=\"DDHandle\" id=\"handle_", nodeId, "\" onmousedown=\"return false;\" onclick=\"return false;\">", newNode.Text, "</span></span></span>");
                }
                else
                {
                    sb.Append("<span id=\"target_", nodeId, "\" class=\"RootNode\"><span class=\"DDItem\" id=\"node_", nodeId, "\">", newNode.Text, "</span></span>");
                }

                newNode.Text = sb.ToString();
            }

            bool nodeHasChildren = ValidationHelper.GetBoolean(container.GetValue("NodeHasChildren"), false);
            // Check if can expand
            if (!nodeHasChildren)
            {
                newNode.PopulateOnDemand = false;
                newNode.Expanded = true;
            }
            else
            {
                if ((sourceNode.ChildNodes.Count > 0) || !sourceNode.ChildNodesLoaded)
                {
                    newNode.PopulateOnDemand = true;
                }
            }

            // Set expanded status
            string aliasPath = ValidationHelper.GetString(container.GetValue("NodeAliasPath"), string.Empty);
            newNode.Expanded = aliasPath.Equals(MapProvider.Path, StringComparison.InvariantCultureIgnoreCase) || expandNodes.Contains(nodeId);
        }
        else
        {
            string parentNodeId = ValidationHelper.GetString(container.GetValue("NodeParentID"), string.Empty);
            newNode.Value = nodeId.ToString();
            newNode.Text = MaxTreeNodeText.Replace("##PARENTNODEID##", parentNodeId);
            newNode.NavigateUrl = "#";
        }

        return newNode;
    }


    private string ResolveNode(string template, string nodeName, string imageTag, string nodeNameJava, int sourceNodeId, string statusIcons)
    {
        return template.Replace("##NODEID##", sourceNodeId.ToString()).Replace("##NODENAMEJAVA##", nodeNameJava).Replace("##NODENAME##", nodeName).Replace("##ICON##", imageTag).Replace("##STATUSICONS##", statusIcons);
    }


    public string GetFallBackToRootScript(bool refreshTree)
    {
        if (RootNode != null)
        {
            string script = null;
            if (refreshTree)
            {
                script += "RefreshTree(0, " + RootNodeID + ");";
            }
            script += "SelectNode(" + RootNodeID + ");";
            return script;
        }
        return null;
    }

    #endregion


    #region "Action handling"

    /// <summary>
    /// Adds the alert error message to the response.
    /// </summary>
    /// <param name="message">Message</param>
    protected override void AddError(string message)
    {
        mCallbackResult += "alert(" + ScriptHelper.GetString(message) + ");";
    }


    /// <summary>
    /// Sets the expanded node ID.
    /// </summary>
    /// <param name="nodeId">Node ID to set</param>
    protected override void SetExpandedNode(int nodeId)
    {
        ExpandNodeID = nodeId;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Indicates if there is needed fallback to root document.
    /// </summary>
    /// <param name="selectedNode">Selected node</param>
    private bool UseFallbackToRoot(TreeNode selectedNode)
    {
        if (selectedNode != null)
        {
            bool useFallback = false;
            string currentAliasPath = selectedNode.NodeAliasPath;

            // Check user's starting path
            string userStartingPath = CurrentUser.UserStartingAliasPath;
            if (!String.IsNullOrEmpty(userStartingPath))
            {
                useFallback = !currentAliasPath.StartsWith(userStartingPath, StringComparison.InvariantCultureIgnoreCase)
                    && (TreePathUtils.GetNodeIdByAliasPath(selectedNode.NodeSiteName, userStartingPath) > 0);

            }

            if (IsProductTree)
            {
                // Check products starting path if in Products UI
                string productsStartingPath = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSStoreProductsStartingPath");
                if (!String.IsNullOrEmpty(productsStartingPath))
                {
                    useFallback |= !currentAliasPath.StartsWith(productsStartingPath, StringComparison.InvariantCultureIgnoreCase)
                        && (TreePathUtils.GetNodeIdByAliasPath(selectedNode.NodeSiteName, productsStartingPath) > 0);
                }
            }

            return useFallback;
        }

        return true;
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets the callback result.
    /// </summary>
    string ICallbackEventHandler.GetCallbackResult()
    {
        return mCallbackResult;
    }


    /// <summary>
    /// Processes the callback action.
    /// </summary>
    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        eventArgument = eventArgument.ToLowerCSafe();
        string[] parameters = eventArgument.Split(';');
        if (parameters.Length < 3)
        {
            return;
        }

        // Get the arguments
        string action = parameters[0];
        int nodeId = ValidationHelper.GetInteger(parameters[1], 0);
        int targetId = ValidationHelper.GetInteger(parameters[2], 0);

        // Get the target node
        TreeNode targetNode = TreeProvider.SelectSingleNode(targetId, TreeProvider.ALL_CULTURES);
        if (targetNode == null)
        {
            AddError(GetString("ContentRequest.ErrorMissingTarget") + " " + eventArgument);
            mCallbackResult += GetFallBackToRootScript(true);
            return;
        }

        // Get the node
        TreeNode node = TreeProvider.SelectSingleNode(nodeId);
        if (node == null)
        {
            AddError(GetString("ContentRequest.ErrorMissingSource"));
            mCallbackResult += GetFallBackToRootScript(true);
            return;
        }

        // Get new parent ID
        int newParentId = targetNode.NodeID;
        if (action.Contains("position"))
        {
            if (action.Contains("move") && (nodeId == targetId))
            {
                // There is no need to change position
                return;
            }

            if (!targetNode.IsRoot())
            {
                newParentId = targetNode.NodeParentID;
            }
        }
        else if (node.NodeParentID == newParentId)
        {
            // Move/Copy/Link as the first node under the same parent
            if (action.EndsWithCSafe("position"))
            {
                action = action.Substring(0, action.Length - 8);
            }

            action += "first";
        }

        bool copy = (action.Contains("copy"));
        bool link = (action.Contains("link"));

        // Do not allow to move or copy under itself
        if ((node.NodeID == newParentId) && !copy && !link)
        {
            AddError(GetString("ContentRequest.CannotMoveToItself"));
            return;
        }

        // Local action - Only position change
        if ((node.NodeParentID == newParentId) && !copy && !link)
        {
            // Local action - Only position change
            int originalPosition = node.NodeOrder;
            TreeNode newNode = ProcessAction(node, targetNode, action, false, false, true);
            if ((newNode != null) && (originalPosition != newNode.NodeOrder))
            {
                // Log the synchronization tasks for the entire tree level
                DocumentSynchronizationHelper.LogDocumentChangeOrder(SiteContext.CurrentSiteName, newNode.NodeAliasPath, TreeProvider);

                mCallbackResult += "CancelDragOperation(); RefreshTree(" + newNode.NodeParentID + ", currentNodeId);";
            }
        }
        else
        {
            // Different parent
            mCallbackResult += "DragOperation(" + nodeId + ", " + targetId + ", '" + action + "');";
        }
    }

    #endregion
}