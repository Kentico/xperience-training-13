using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Forums_Controls_PostTree : ForumViewer
{
    #region "Variables"

    /// <summary>
    /// Forum Post tree provider.
    /// </summary>
    private ForumPostTreeProvider mForumPostTreeProvider;

    private bool mAllowReply = true;
    private string mPostPath = "/%";
    private bool mAdministrationMode;
    private int mSelected;
    private bool mSelectOnlyApproved;
    private bool mRegularLoad;
    private int mMaxPostNodes;
    private bool mUseMaxPostNodes;
    private ForumPostInfo mSelectedPost;
    private string mMaxTreeNodeText;
    private bool? mCanBeModerated;
    private bool? mUserIsModerator;


    /// <summary>
    /// If is true unapproved post is highlighted.
    /// </summary>
    protected bool mHighlightUnApproved = false;

    /// <summary>
    /// Item CSS class.
    /// </summary>
    protected string mItemCssClass = "ThreadPost";

    /// <summary>
    /// Selcted item CSS class.
    /// </summary>
    protected string mSelectedItemCssClass = "ThreadPostSelected";

    #endregion


    #region "Public events and delegates"

    /// <summary>
    /// Image delegate.
    /// </summary>
    public delegate string GetPostStatusIconsEventHandler(ForumPostTreeNode node);

    /// <summary>
    /// Gets status icons.
    /// </summary>
    public event GetPostStatusIconsEventHandler OnGetStatusIcons;

    #endregion


    #region "Public properties"

    /// <summary>
    /// If is true unapproved post is highlighted.
    /// </summary>
    public bool HighlightUnApprove
    {
        get
        {
            return mHighlightUnApproved;
        }
        set
        {
            mHighlightUnApproved = value;
        }
    }


    /// <summary>
    /// Enable subscription.
    /// </summary>
    public bool AllowReply
    {
        get
        {
            return mAllowReply;
        }
        set
        {
            mAllowReply = value;
        }
    }


    /// <summary>
    /// Path.
    /// </summary>
    public string PostPath
    {
        get
        {
            return mPostPath;
        }
        set
        {
            mPostPath = value;
        }
    }


    /// <summary>
    /// Select only approved.
    /// </summary>
    public bool SelectOnlyApproved
    {
        get
        {
            return mSelectOnlyApproved;
        }
        set
        {
            mSelectOnlyApproved = value;
        }
    }


    /// <summary>
    /// Selected post id.
    /// </summary>
    public int Selected
    {
        get
        {
            return mSelected;
        }
        set
        {
            mSelected = value;
        }
    }


    /// <summary>
    /// Selected post info.
    /// </summary>
    public ForumPostInfo SelectedPost
    {
        get
        {
            if ((mSelectedPost == null) && (Selected > 0))
            {
                mSelectedPost = ForumPostInfoProvider.GetForumPostInfo(Selected);
            }

            return mSelectedPost;
        }
        set
        {
            mSelectedPost = value;
        }
    }


    /// <summary>
    /// Maximum number of forum post nodes displayed within one level of the tree.
    /// Setting CMSForumMaxPostNode is used by default.
    /// Note: UseMaxPostNodes property can suppress use of MaxPostNodes
    /// </summary>
    public int MaxPostNodes
    {
        get
        {
            if (mMaxPostNodes <= 0)
            {
                mMaxPostNodes = SettingsKeyInfoProvider.GetIntValue(SiteName + ".CMSForumMaxPostNode");
            }
            return mMaxPostNodes;
        }
        set
        {
            mMaxPostNodes = value;

            // Limit results only when MaxTreeNodes is specified
            MapProvider.MaxPostNodes = mMaxPostNodes > 0 ? value + 1 : mMaxPostNodes;
        }
    }


    /// <summary>
    /// Enables or disables use of MaxPostNodes property, which limits maximum number of posts 
    /// displayed in tree.
    /// </summary>
    public bool UseMaxPostNodes
    {
        get
        {
            return mUseMaxPostNodes;
        }
        set
        {
            mUseMaxPostNodes = value;
        }
    }


    /// <summary> 
    /// Enables or disables administration mode.
    /// </summary>
    public bool AdministrationMode
    {
        get
        {
            return mAdministrationMode;
        }
        set
        {
            mAdministrationMode = value;
        }
    }


    /// <summary>
    /// Post tree provider that the tree uses.
    /// </summary>
    public ForumPostTreeProvider MapProvider
    {
        get
        {
            if (mForumPostTreeProvider == null)
            {
                mForumPostTreeProvider = new ForumPostTreeProvider();

                // Set Map provider values
                mForumPostTreeProvider.BindNodeData = true;
                mForumPostTreeProvider.Path = PostPath;

                if (ExpandTree || DetailModeIE)
                {
                    // Load all posts
                    mForumPostTreeProvider.MaxRelativeLevel = ForumPostTreeProvider.ALL_LEVELS;
                }
                else
                {
                    // Load only first level
                    mForumPostTreeProvider.MaxRelativeLevel = 1;
                }

                mForumPostTreeProvider.SelectOnlyApproved = SelectOnlyApproved;
                mForumPostTreeProvider.ForumID = ForumID;
                mForumPostTreeProvider.WhereCondition = WhereCondition;
                mForumPostTreeProvider.Columns = GetColumns();

                // Limit number of displayed posts, usually in administration
                if (UseMaxPostNodes && (MaxPostNodes > 0))
                {
                    mForumPostTreeProvider.MaxPostNodes = MaxPostNodes + 1;
                }
            }

            return mForumPostTreeProvider;
        }
    }


    /// <summary>
    /// Provides acces to treeElem property.
    /// </summary>
    public TreeView TreeView
    {
        get
        {
            return treeElem;
        }
    }


    /// <summary>
    /// Item CSS class.
    /// </summary>
    public string ItemCssClass
    {
        get
        {
            return mItemCssClass;
        }
        set
        {
            mItemCssClass = value;
        }
    }


    /// <summary>
    /// Selected item CSS class.
    /// </summary>
    public string SelectedItemCssClass
    {
        get
        {
            return mSelectedItemCssClass;
        }
        set
        {
            mSelectedItemCssClass = value;
        }
    }


    /// <summary>
    /// Text to appear within the latest node when max tree nodes applied.
    /// </summary>
    public string MaxTreeNodeText
    {
        get
        {
            if (mMaxTreeNodeText == null)
            {
                mMaxTreeNodeText = GetString("general.seelisting");
            }
            return mMaxTreeNodeText;
        }
        set
        {
            mMaxTreeNodeText = value;
        }
    }


    /// <summary>
    /// Indicates whether posts can be moderated in current context, 
    /// therefore unapproved post could be displayed.
    /// </summary>
    public bool CanBeModerated
    {
        get
        {
            if (!mCanBeModerated.HasValue)
            {
                ForumInfo fi = ForumContext.CurrentForum;
                mCanBeModerated = ((fi != null) && EnableOnSiteManagement && fi.ForumModerated && UserIsModerator);
            }

            return mCanBeModerated.Value;
        }
    }


    /// <summary>
    /// Indicates whether the current user is a moderator. That means: forum moderator or group admin or global admin.
    /// </summary>
    private bool UserIsModerator
    {
        get
        {
            if (!mUserIsModerator.HasValue)
            {
                ForumInfo fi = ForumContext.CurrentForum;
                bool result = (fi != null) && ForumContext.UserIsModerator(fi.ForumID, CommunityGroupID);

                if (!result && (CommunityGroupID > 0))
                {
                    result = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.groups", CMSAdminControl.PERMISSION_MANAGE);
                }

                mUserIsModerator = result;
            }

            return mUserIsModerator.Value;
        }
    }


    /// <summary>
    /// Indicates if special mode for treating (dynamic) detail mode in IE should be used. 
    /// The cause is that, IE can't handle too long parameters of TreeView_PopulateNode javascript function
    /// </summary>
    private bool DetailModeIE
    {
        get
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return (((ShowMode == ShowModeEnum.DetailMode) || (ShowMode == ShowModeEnum.DynamicDetailMode)) && BrowserHelper.IsIE());
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    #endregion


    #region "Helper methods and properties"

    /// <summary>
    /// Gets or sets the value that indicates whether data has been loaded or current load is 
    /// called on demand
    /// </summary>
    protected bool RegularLoad
    {
        get
        {
            return mRegularLoad;
        }
        set
        {
            mRegularLoad = value;
        }
    }


    /// <summary>
    /// Returns list of required columns if current mode is tree, otherwise returns null => load all columns
    /// </summary>
    protected string GetColumns()
    {
        if (ShowMode == ShowModeEnum.TreeMode)
        {
            return "PostSubject, PostIDPath, PostParentID, PostApproved, PostID, PostThreadPosts, PostThreadPostsAbsolute, PostLevel";
        }

        return null;
    }


    /// <summary>
    /// Returns node text for dynamic mode.
    /// </summary>
    /// <param name="postRow">Forum post data row</param>
    private string CreateDetailModeNode(DataRow postRow)
    {
        StringBuilder sbRendered;

        // Render forum post control
        string forumPostControlId = DynamicForumPostRender(postRow, out sbRendered);

        string nodeText = String.Format("<div ID=\"Selected{0}\" style=\"display: block;\" class=\"TreeSelectedPost\">{1}</div>",
                                        forumPostControlId, sbRendered);

        return nodeText;
    }


    /// <summary>
    /// Returns node text for dynamic details mode.
    /// </summary>
    /// <param name="postRow">Forum post data row</param>
    /// <param name="cssClass">CSS class</param>
    /// <param name="statusIcons">Markup of status icons</param>
    /// <param name="postSubject">Post subject</param>    
    private string CreateDynamicDetailModeNode(DataRow postRow, string cssClass, string statusIcons, string postSubject)
    {
        StringBuilder sbRendered;

        // Render forum post control
        string forumPostControlId = DynamicForumPostRender(postRow, out sbRendered);

        string nodeText = String.Format(
            "<span style=\"display:block;\" ID=\"{0}\" class=\"{1}\" onclick=\" SelectForumNode('Selected{0}','{0}'); return false;\">"
            + "{2}" + //imagetag + postsubject
            "</span><div ID=\"Selected{0}\" style=\"display: none;\" class=\"TreeSelectedPost\">{3}{4}</div>",
            forumPostControlId, cssClass, HTMLHelper.HTMLEncode(postSubject), sbRendered, statusIcons);
        return nodeText;
    }


    /// <summary>
    /// Renders ForumPost control for specified node.
    /// </summary>
    /// <param name="postRow">Forum post data row</param>
    /// <param name="sbRendered">String builder instance containing rendered text of control</param>
    private string DynamicForumPostRender(DataRow postRow, out StringBuilder sbRendered)
    {
        // Create detail of post to string
        sbRendered = new StringBuilder();
        string mId = "";
        ForumPostInfo fpi = new ForumPostInfo(postRow);

        if (ShowMode != ShowModeEnum.TreeMode)
        {
            StringWriter sw = new StringWriter(sbRendered);
            Html32TextWriter writer = new Html32TextWriter(sw);
            ForumViewer post = (ForumViewer)Page.LoadUserControl("~/CMSModules/Forums/Controls/Posts/ForumPost.ascx");
            post.ID = "forumPost" + fpi.PostId;

            CopyValues(post);

            post.SetValue("PostInfo", fpi);
            post.ReloadData();
            post.RenderControl(writer);
            mId = ClientID + fpi.PostId;
        }

        return mId;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ForumContext.ForumID = ForumID;

        treeElem.ImageSet = TreeViewImageSet.Custom;
        if ((IsLiveSite && CultureHelper.IsPreferredCultureRTL()) || (!IsLiveSite && CultureHelper.IsUICultureRTL()))
        {
            treeElem.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree", IsLiveSite, false);
        }
        else
        {
            treeElem.LineImagesFolder = GetImageUrl("Design/Controls/Tree", IsLiveSite, false);
        }

        // Loading image script
        string loadingScript = @"
            if (TreeView_PopulateNode) { base_TreeView_PopulateNode = TreeView_PopulateNode };
            TreeView_PopulateNode = function(data, index, node, selectNode, selectImageNode, lineType, text, path, databound, datapath, parentIsLast) {
            if (!data) { return; }
            if (!node.blur) { node = node[0]; }
            node.blur();
            node.firstChild.src = '" + GetImageUrl("Design/Preloaders/preload.gif") + "';" + @"
            if (base_TreeView_PopulateNode) {
                base_TreeView_PopulateNode(data, index, node, selectNode, selectImageNode, lineType, text, path, databound, datapath, parentIsLast); } }";

        ltlScript.Text = ScriptHelper.GetScript(loadingScript);

        treeElem.ExpandImageToolTip = GetString("ContentTree.Expand");
        treeElem.CollapseImageToolTip = GetString("ContentTree.Collapse");
    }


    /// <summary>
    /// OnPreRender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        if (StopProcessing)
        {
            return;
        }

        mForumPostTreeProvider = null;

        RegularLoad = true;

        // Some post should be preselected => load all posts leading to it in one query
        if (SelectedPost != null)
        {
            MapProvider.SelectPostPath = SelectedPost.PostIDPath;
        }

        if (MapProvider.RootNode != null)
        {
            TreeNode rootNode = CreateNode((ForumPostTreeNode)MapProvider.RootNode, 0);
            // Localize root node
            if (rootNode != null)
            {
                rootNode.Text = ResHelper.LocalizeString(rootNode.Text);
                // Add root node
                treeElem.Nodes.Add(rootNode);
            }
            treeElem.EnableViewState = false;
        }
    }


    /// <summary>
    /// Creates tree node.
    /// </summary>
    /// <param name="sourceNode">Node with source data</param>
    /// <param name="index">Node index</param>
    protected TreeNode CreateNode(ForumPostTreeNode sourceNode, int index)
    {
        if (sourceNode == null)
        {
            return null;
        }

        // Create tree node
        TreeNode newNode = new TreeNode();

        DataRow dr = (DataRow)sourceNode.ItemData;

        // Check whether item data are defined, if not it is root node
        if (dr != null)
        {
            int sourceNodeId = (int)dr["PostID"];
            int nodeLevel = (int)dr["PostLevel"];

            // Check on maximum post in tree
            if (!UseMaxPostNodes || (index < MaxPostNodes) || (MaxPostNodes <= 0))
            {
                #region "Set node values and appearance"

                newNode.Value = sourceNodeId.ToString();
                newNode.SelectAction = TreeNodeSelectAction.None;

                bool isApproved = ValidationHelper.GetBoolean(dr["PostApproved"], false);
                string postSubject = (string)dr["PostSubject"];

                string cssClass = ItemCssClass;

                // Add CSS class for unapproved posts
                if (HighlightUnApprove && !isApproved)
                {
                    cssClass += " PostUnApproved";
                }

                string statusIcons = "";
                if (OnGetStatusIcons != null)
                {
                    statusIcons = OnGetStatusIcons(sourceNode);

                }


                // Set by display mode
                switch (ShowMode)
                {
                    // Dynamic detail mode
                    case ShowModeEnum.DynamicDetailMode:
                        newNode.Text = CreateDynamicDetailModeNode(dr, cssClass, statusIcons, postSubject);
                        break;

                    // Detail mode
                    case ShowModeEnum.DetailMode:
                        newNode.Text = CreateDetailModeNode(dr);
                        break;

                    // Tree mode
                    default:

                        if (Selected == sourceNodeId)
                        {
                            cssClass = SelectedItemCssClass;

                            string spanId = String.Empty;
                            if (AdministrationMode)
                            {
                                spanId = "id=\"treeSelectedNode\"";
                            }

                            newNode.Text = String.Format("<span {0} class=\"{1}\" onclick=\"ShowPost({2}); SelectForumNode(this);\"><span class=\"Name\">{4}{3}</span></span>",
                                                         spanId, cssClass, newNode.Value, statusIcons, HTMLHelper.HTMLEncode(postSubject));
                        }
                        else
                        {
                            newNode.Text = String.Format("<span class=\"{0}\" onclick=\"ShowPost({1}); SelectForumNode(this);\"><span class=\"Name\">{3}{2}</span></span>",
                                                         cssClass, newNode.Value, statusIcons, HTMLHelper.HTMLEncode(postSubject));
                        }
                        break;
                }

                #endregion


                if (!ExpandTree)
                {
                    #region "Populate deeper levels on demand"

                    // Check if can expand
                    string childCountColumn = "PostThreadPosts";

                    // Check if unapproved posts can be included
                    if (AdministrationMode || UserIsModerator)
                    {
                        childCountColumn = "PostThreadPostsAbsolute";
                    }

                    int childNodesCount = ValidationHelper.GetInteger(dr[childCountColumn], 0);

                    // If the post is thread(level = 0) then childnodes count 1 means no real child-post
                    if ((childNodesCount == 0) || ((childNodesCount == 1) && (nodeLevel == 0)))
                    {
                        newNode.PopulateOnDemand = false;

                        // No children -> expand
                        newNode.Expanded = true;
                    }
                    else
                    {
                        if (!sourceNode.ChildNodesLoaded)
                        {
                            newNode.PopulateOnDemand = true;
                            newNode.Expanded = false;
                        }
                    }

                    #endregion


                    #region "Expand nodes on the current path"

                    // If preselect is set = first load
                    if (RegularLoad)
                    {
                        string currentNodePath = (string)dr["PostIDPath"];
                        string currentSelectedPath = String.Empty;

                        if (SelectedPost != null)
                        {
                            currentSelectedPath = SelectedPost.PostIDPath;
                        }

                        // Expand if node is on the path
                        if (currentSelectedPath.StartsWithCSafe(currentNodePath))
                        {
                            // Raise OnTreeNodePopulate
                            newNode.PopulateOnDemand = true;
                            newNode.Expanded = true;
                        }
                        else
                        {
                            newNode.Expanded = false;
                        }
                    }

                    #endregion
                }
                else
                {
                    // Populate will be called on each node
                    newNode.PopulateOnDemand = true;
                    newNode.Expanded = true;
                }
            }
            else
            {
                string parentNodeId = ValidationHelper.GetString(dr["PostParentID"], "");
                newNode.Value = sourceNodeId.ToString();
                newNode.Text = MaxTreeNodeText.Replace("##PARENTNODEID##", parentNodeId);
                newNode.SelectAction = TreeNodeSelectAction.None;
            }
        }
        // Root node populate by default
        else
        {
            // Root node as forum display name
            ForumInfo fi = ForumInfoProvider.GetForumInfo(ForumID);

            if (fi != null)
            {
                newNode.Text = "<span class=\"" + ItemCssClass + "\" onclick=\"ShowPost('-1'); SelectForumNode(this); \"><span class=\"Name\">" + HTMLHelper.HTMLEncode(fi.ForumDisplayName) + "</span></span>";
                newNode.Value = "0";
                newNode.SelectAction = TreeNodeSelectAction.None;
            }

            newNode.PopulateOnDemand = true;
            newNode.Expanded = true;
        }

        return newNode;
    }


    /// <summary>
    /// On populate create child nodes.
    /// </summary>
    protected void treeElem_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        e.Node.ChildNodes.Clear();
        e.Node.PopulateOnDemand = false;

        int postId = ValidationHelper.GetInteger(e.Node.Value, 0);

        // Set the ForumID if not set already
        if (ForumContext.ForumID == 0)
        {
            ForumPostInfo postInfo = ForumPostInfoProvider.GetForumPostInfo(postId);
            if (postInfo != null)
            {
                ForumContext.ForumID = postInfo.PostForumID;
                ForumID = postInfo.PostForumID;
            }
        }

        // Get child nodes
        SiteMapNodeCollection childNodes = MapProvider.GetChildNodes(postId, RegularLoad);

        int index = 0;
        foreach (ForumPostTreeNode childNode in childNodes)
        {
            int childNodeId = (int)((DataRow)childNode.ItemData)["PostID"];
            if (childNodeId != postId)
            {
                TreeNode newNode = CreateNode(childNode, index);
                bool? originalExpanded = newNode.Expanded;

                // Force node to expand and load child posts
                if (DetailModeIE)
                {
                    newNode.PopulateOnDemand = true;
                    newNode.Expanded = true;
                }

                e.Node.ChildNodes.Add(newNode);

                // Restore original expanded state
                if (DetailModeIE)
                {
                    newNode.Expanded = originalExpanded;
                }

                index++;
            }

            // Ensure there is only one 'click here for more' item
            if (UseMaxPostNodes && (MaxPostNodes > 0) && (index > MaxPostNodes))
            {
                break;
            }
        }
    }


    /// <summary> 
    /// Render 
    /// </summary>
    protected override void Render(HtmlTextWriter writer)
    {
        if (!StopProcessing)
        {
            base.Render(writer);
        }
    }

    #endregion
}
