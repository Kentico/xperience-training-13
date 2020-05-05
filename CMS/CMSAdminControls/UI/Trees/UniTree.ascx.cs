using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Trees_UniTree : UniTree, IPostBackEventHandler
{
    #region "Constants"

    public string NODE_CODENAME_MACRO = "##NODECODENAME##";
    public const string NODE_NAME_MACRO = "##NODENAME##";
    public const string NODE_ICON_MACRO = "##ICON##";

    #endregion


    #region "Variables"

    private TreeNode mRootNode;
    private string mSelectedItem;
    private bool mEnableRootAction = true;

    private bool mDisplayPopulatingIndicator = true;

    private string selectedPath = String.Empty;
    private string mCollapseTooltip;
    private string mExpandTooltip;
    private string mLineImagesFolder = String.Empty;

    private readonly ArrayList defaultItems = new ArrayList();

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether tree displays all roots elements (parentID IS NULL)
    /// </summary>
    public bool MultipleRoots
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the root element action should be None or Select.
    /// </summary>
    public bool EnableRootAction
    {
        get
        {
            return mEnableRootAction;
        }
        set
        {
            mEnableRootAction = value;
        }
    }


    /// <summary>
    /// Indicates if ##NODENAME## should be localized.
    /// </summary>
    public bool Localize
    {
        get;
        set;
    }


    /// <summary>
    /// If true, spans IDs are general (f.e. category not reportcategory)
    /// </summary>
    public bool GeneralIDs
    {
        get;
        set;
    }

    /// <summary>
    /// Indicates number of expanded levels.
    /// </summary>
    public int ExpandLevel
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if all nodes should be expanded.
    /// </summary>
    public bool ExpandAll
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if all nodes should be collapsed.
    /// </summary>
    public bool CollapseAll
    {
        get;
        set;
    }


    /// <summary>
    /// Gets root node from provider object.
    /// </summary>
    public TreeNode RootNode
    {
        get
        {
            if (mRootNode == null)
            {
                return CustomRootNode;
            }

            return mRootNode;
        }
    }


    /// <summary>
    /// Tree view control
    /// </summary>
    protected override UITreeView TreeView
    {
        get
        {
            return treeElem;
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether populating indicator should be displayed or not.
    /// </summary>
    public bool DisplayPopulatingIndicator
    {
        get
        {
            return mDisplayPopulatingIndicator;
        }
        set
        {
            mDisplayPopulatingIndicator = value;
        }
    }


    /// <summary>
    /// Gets or sets the ToolTip for the image that is displayed for the expandable node indicator.
    /// </summary>
    public string ExpandTooltip
    {
        get
        {
            return mExpandTooltip;
        }
        set
        {
            mExpandTooltip = value;
            TreeView.ExpandImageToolTip = value;
        }
    }


    /// <summary>
    /// Gets or sets the ToolTip for the image that is displayed for the collapsible node indicator.
    /// </summary>
    public string CollapseTooltip
    {
        get
        {
            return mCollapseTooltip;
        }
        set
        {
            TreeView.CollapseImageToolTip = value;
            mCollapseTooltip = value;
        }
    }


    /// <summary>
    /// Gets or sets the path to a folder that contains the line images that are used to connect child nodes to parent nodes.   
    /// </summary>
    public string LineImagesFolder
    {
        get
        {
            if (String.IsNullOrEmpty(mLineImagesFolder))
            {
                if ((IsLiveSite && CultureHelper.IsPreferredCultureRTL()) || (!IsLiveSite && CultureHelper.IsUICultureRTL()))
                {
                    mLineImagesFolder = "~" + RequestContext.CurrentRelativePath + "?cmsimg=/rt";
                }
                else
                {
                    mLineImagesFolder = "~" + RequestContext.CurrentRelativePath + "?cmsimg=/t";
                }
            }
            return mLineImagesFolder;
        }
        set
        {
            mLineImagesFolder = value;
            TreeView.LineImagesFolder = value;
        }
    }


    /// <summary>
    /// Gets or sets selected item.
    /// </summary>
    public override string SelectedItem
    {
        get
        {
            return mSelectedItem ?? (mSelectedItem = hdnSelectedItem.Value);
        }
        set
        {
            hdnSelectedItem.Value = value;
            mSelectedItem = value;
        }
    }


    /// <summary>
    /// Gets the client ID of hidden field with selected item value.
    /// </summary>
    public string SelectedItemFieldId
    {
        get
        {
            return hdnSelectedItem.ClientID;
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        TreeView.TreeNodePopulate += (s, args) => PopulateNode(args.Node);
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        mCollapseTooltip = GetString("general.collapse");
        mExpandTooltip = GetString("general.expand");

        TreeView.ExpandImageToolTip = ExpandTooltip;
        TreeView.CollapseImageToolTip = CollapseTooltip;

        TreeView.LineImagesFolder = LineImagesFolder;
    }


    /// <summary>
    /// Page PreRender.
    /// </summary>
    /// <param name="e">Arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        int index = 0;

        foreach (object item in defaultItems)
        {
            string[] defaultItem = (string[])item;

            if (defaultItem != null)
            {
                // Generate link HTML tag
                string selectedItem = ValidationHelper.GetString(SelectedItem, "").ToLowerCSafe();
                string template = (selectedItem == defaultItem[2].ToLowerCSafe()) ? SelectedDefaultItemTemplate : DefaultItemTemplate;

                string link = ReplaceMacros(template, 0, 0, defaultItem[0], defaultItem[1], 0, "", "");

                // Add complete HTML code to page
                if (UsePostBack)
                {
                    link = "<span onclick=\"" + ControlsHelper.GetPostBackEventReference(this, HTMLHelper.HTMLEncode(defaultItem[2])) + "\">" + link + "</span>";
                }

                TreeNode tn = new TreeNode();
                tn.Text = link;
                tn.NavigateUrl = RequestContext.CurrentURL + "#";
                TreeView.Nodes.AddAt(index, tn);
                index++;
            }
        }

        if (DisplayPopulatingIndicator && !RequestHelper.IsCallback())
        {
            // Register tree progress icon
            ScriptHelper.RegisterTreeProgress(Page);
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handle node is populated.
    /// </summary>
    protected void PopulateNode(TreeNode node)
    {
        if ((ProviderObject != null) && (node != null))
        {
            string[] splitted = node.Value.Split('_');
            int nodeID = ValidationHelper.GetInteger(splitted[0], 0);

            // Get node object type
            string nodeObjectType = String.Empty;
            if (splitted.Length == 2)
            {
                nodeObjectType = splitted[1];
            }

            // Get child nodes
            SiteMapNodeCollection childNodes = ProviderObject.GetChildNodes(node.Value, node.Depth + 1);

            // Add to treeview
            foreach (UniTreeNode childNode in childNodes)
            {
                // Get ID
                int childNodeId = (int)(((DataRow)childNode.ItemData)[ProviderObject.IDColumn]);

                // Get object type
                string childNodeType = String.Empty;
                if (!String.IsNullOrEmpty(ProviderObject.ObjectTypeColumn))
                {
                    childNodeType = ValidationHelper.GetString((((DataRow)childNode.ItemData)[ProviderObject.ObjectTypeColumn]), "");
                }

                // Don't insert one object more than once
                if ((childNodeId != nodeID) || (nodeObjectType != childNodeType))
                {
                    TreeNode createdNode = CreateNode(childNode);
                    RaiseOnNodeCreated(childNode, ref createdNode);
                    if (createdNode != null)
                    {
                        node.ChildNodes.Add(createdNode);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        if (!StopProcessing && (ProviderObject != null))
        {
            TreeView.Nodes.Clear();
            TreeView.EnableViewState = false;

            // Add custom root node
            if (CustomRootNode != null)
            {
                if (!EnableRootAction)
                {
                    CustomRootNode.SelectAction = TreeNodeSelectAction.None;
                }

                TreeView.Nodes.Add(CustomRootNode);
                RaiseOnPopulateRootNode();
            }
            else
            {
                if (MultipleRoots)
                {
                    // Load all roots
                    DataSet ds = ProviderObject.LoadData(ProviderObject.ParentIDColumn + " IS NULL");
                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            // Add all results as roots
                            UniTreeNode rootNode = new UniTreeNode(ProviderObject, "");
                            rootNode.ItemData = dr;
                            TreeView.Nodes.Add(CreateNode(rootNode));
                        }
                    }
                }
                else
                    // Add root node from provider
                    if (ProviderObject.RootNode != null)
                    {
                        mRootNode = CreateNode((UniTreeNode)ProviderObject.RootNode);

                        RaiseOnNodeCreated((UniTreeNode)ProviderObject.RootNode, ref mRootNode);
                        mRootNode.Expanded = true;
                        TreeView.Nodes.Add(mRootNode);
                    }
            }
        }
    }


    /// <summary>
    /// Populates root node.
    /// </summary>
    public void RaiseOnPopulateRootNode()
    {
        TreeNode node = TreeView.Nodes[0];
        if (node != null)
        {
            if (!String.IsNullOrEmpty(ExpandPath))
            {
                node.Expanded = true;
                PopulateNode(node);
            }
        }
    }


    /// <summary>
    /// Adds root node (allows customization of the root item).
    /// </summary>
    /// <param name="rootText">Root text</param>
    /// <param name="value">Root value</param>
    /// <param name="imagePath">Image path</param>
    public void SetRoot(string rootText, string value, string imagePath)
    {
        CustomRootNode = new TreeNode(rootText, value, imagePath);
    }


    /// <summary>
    /// Adds root node (allows customization of the root item).
    /// </summary>
    /// <param name="rootText">Root text</param>
    /// <param name="value">Root value</param>
    /// <param name="imagePath">Image path</param>
    /// <param name="navigateUrl">Navigate URL</param>
    /// <param name="target">Target</param>
    public void SetRoot(string rootText, string value, string imagePath, string navigateUrl, string target)
    {
        CustomRootNode = new TreeNode(rootText, value, imagePath, navigateUrl, target);
    }


    /// <summary>
    /// Creates node.
    /// </summary>
    /// <param name="uniNode">Node to create</param>
    protected TreeNode CreateNode(UniTreeNode uniNode)
    {
        var data = (DataRow)uniNode.ItemData;
        if (data == null)
        {
            return null;
        }

        TreeNode node = new TreeNode();

        // Get data
        int childNodesCount = 0;
        if (!String.IsNullOrEmpty(ProviderObject.ChildCountColumn))
        {
            childNodesCount = ValidationHelper.GetInteger(data[ProviderObject.ChildCountColumn], 0);
        }

        // Node ID
        int nodeId = 0;
        if (!String.IsNullOrEmpty(ProviderObject.IDColumn))
        {
            nodeId = ValidationHelper.GetInteger(data[ProviderObject.IDColumn], 0);
        }

        // Node value
        string nodeValue = String.Empty;
        if (!String.IsNullOrEmpty(ProviderObject.ValueColumn))
        {
            nodeValue = nodeId.ToString();
        }

        string objectType = String.Empty;
        if (!String.IsNullOrEmpty(ProviderObject.ObjectTypeColumn))
        {
            objectType = ValidationHelper.GetString(data[ProviderObject.ObjectTypeColumn], "");

            // Add object type to value
            nodeValue += "_" + objectType;
        }

        // Caption
        string displayName = String.Empty;
        if (!String.IsNullOrEmpty(ProviderObject.CaptionColumn))
        {
            displayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(data[ProviderObject.CaptionColumn], "")));
        }

        // Display name, if caption is empty (or non existent)
        if (!String.IsNullOrEmpty(ProviderObject.DisplayNameColumn) && String.IsNullOrEmpty(displayName))
        {
            displayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(data[ProviderObject.DisplayNameColumn], "")));
        }

        // Path
        string nodePath = String.Empty;
        if (!String.IsNullOrEmpty(ProviderObject.PathColumn))
        {
            nodePath = HTMLHelper.HTMLEncode(ValidationHelper.GetString(data[ProviderObject.PathColumn], "")).ToLowerCSafe();
        }

        // Parent ID
        int parentId = 0;
        if (!String.IsNullOrEmpty(ProviderObject.ParentIDColumn))
        {
            parentId = ValidationHelper.GetInteger(data[ProviderObject.ParentIDColumn], 0);
        }

        // Parameter
        string parameter = String.Empty;
        if (!String.IsNullOrEmpty(ProviderObject.ParameterColumn))
        {
            parameter = HTMLHelper.HTMLEncode(ValidationHelper.GetString(data[ProviderObject.ParameterColumn], "")).ToLowerCSafe();
        }

        int nodeLevel = 0;
        if (!String.IsNullOrEmpty(ProviderObject.LevelColumn))
        {
            nodeLevel = ValidationHelper.GetInteger(data[ProviderObject.LevelColumn], 0);
        }

        // Set navigation URL to current page
        node.NavigateUrl = RequestContext.CurrentURL + "#";

        // Set value
        node.Value = nodeValue;

        // Get tree icon
        var treeIcon = GetNodeIconMarkup(uniNode, data);

        // Set text
        string text;

        string selectedItem = ValidationHelper.GetString(SelectedItem, "");
        string selectPathLowered = SelectPath.ToLowerCSafe();

        if (nodeValue.EqualsCSafe(selectedItem, true) || ((selectPathLowered == nodePath) && String.IsNullOrEmpty(selectedItem)))
        {
            text = ReplaceMacros(SelectedNodeTemplate, nodeId, childNodesCount, displayName, treeIcon, parentId, objectType, parameter);
        }
        else
        {
            text = ReplaceMacros(NodeTemplate, nodeId, childNodesCount, displayName, treeIcon, parentId, objectType, parameter);
        }

        if (UsePostBack)
        {
            text = "<span onclick=\"" + ControlsHelper.GetPostBackEventReference(this, nodeValue + ";" + nodePath) + "\">" + text + "</span>";
        }

        node.Text = text;

        // Set populate node automatically
        if (childNodesCount != 0)
        {
            node.PopulateOnDemand = true;
        }

        // Expand tree            
        if (ExpandAll)
        {
            node.Expanded = true;
        }
        else if (CollapseAll)
        {
            node.Expanded = false;
        }
        else
        {
            // Handle expand path
            if (!nodePath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                nodePath += "/";
            }

            string expandPathLowered = ExpandPath.ToLowerCSafe();

            if (selectPathLowered.StartsWithCSafe(nodePath) && (selectPathLowered != nodePath))
            {
                node.Expanded = true;
            }
            else if ((expandPathLowered.StartsWithCSafe(nodePath)))
            {
                node.Expanded = true;
            }
            else
                // Path expanded by user
                if (selectedPath.StartsWithCSafe(nodePath) && (selectedPath != nodePath))
                {
                    node.Expanded = true;
                }
                // Expand all roots for multiple roots tree
                else if ((parentId == 0) && MultipleRoots)
                {
                    node.Expanded = true;
                }
                else
                {
                    node.Expanded = false;
                }
        }

        // Expand level
        if ((ExpandLevel != 0) && !CollapseAll)
        {
            node.Expanded = nodeLevel <= ExpandLevel;
        }

        return node;
    }


    /// <summary>
    /// Replaces all macros in template by values.
    /// </summary>
    /// <param name="template">Template with macros</param>
    /// <param name="itemID">Item ID</param>
    /// <param name="childCount">Child count</param>
    /// <param name="nodeName">Node name</param>
    /// <param name="parentNodeID">Parent item ID</param>
    /// <param name="icon">Icon</param>  
    /// <param name="objectType">Object type</param>
    /// <param name="parameter">Additional parameter</param>
    public string ReplaceMacros(string template, int itemID, int childCount, string nodeName, string icon, int parentNodeID, string objectType, string parameter)
    {
        template = template.Replace("##NODEID##", itemID.ToString());
        template = template.Replace("##NODEJAVA##", ScriptHelper.GetString(nodeName));
        template = template.Replace("##NODECHILDNODESCOUNT##", childCount.ToString());
        if (Localize)
        {
            nodeName = ResHelper.LocalizeString(nodeName);
        }
        template = template.Replace("##NODENAME##", nodeName);
        template = template.Replace("##ICON##", icon);
        template = template.Replace("##PARENTNODEID##", parentNodeID.ToString());

        // For general IDs, replace f.e. 'reportcategory' with 'category' and f.e. 'report' with 'item'
        if (GeneralIDs)
        {
            objectType = objectType.Contains("category") ? "category" : "item";
        }

        template = template.Replace("##OBJECTTYPE##", objectType);
        template = template.Replace("##PARAMETER##", parameter);

        return template;
    }


    /// <summary>
    /// Gets tree node icon
    /// </summary>
    /// <param name="node">Tree node</param>
    /// <param name="data">Source data</param>
    private string GetNodeIconMarkup(UniTreeNode node, DataRow data)
    {
        string imagePath = null;
        string iconClass = null;

        // Get image path from data
        if (!string.IsNullOrEmpty(ProviderObject.ImageColumn))
        {
            imagePath = ValidationHelper.GetString(data[ProviderObject.ImageColumn], null);
        }

        // Get icon class from data
        if (!string.IsNullOrEmpty(ProviderObject.IconClassColumn))
        {
            iconClass = ValidationHelper.GetString(data[ProviderObject.IconClassColumn], null);
        }

        // Get image or icon from external event
        if (OnGetImage != null)
        {
            // Set node as event argument
            var args = new UniTreeImageArgs
            {
                TreeNode = node
            };

            OnGetImage.StartEvent(args);

            // Get image or icon class
            if (string.IsNullOrEmpty(imagePath) && !string.IsNullOrEmpty(args.ImagePath))
            {
                imagePath = args.ImagePath;
            }
            else if (string.IsNullOrEmpty(iconClass) && !string.IsNullOrEmpty(args.IconClass))
            {
                iconClass = args.IconClass;
            }
        }

        // If no definition found, use default image path
        if (string.IsNullOrEmpty(imagePath) && string.IsNullOrEmpty(iconClass))
        {
            // Image path has higher priority
            if (!string.IsNullOrEmpty(DefaultImagePath))
            {
                imagePath = DefaultImagePath;
            }
            else if (!string.IsNullOrEmpty(DefaultIconClass))
            {
                iconClass = DefaultIconClass;
            }
        }

        return UIHelper.GetAccessibleImageMarkup(Page, iconClass, imagePath, size: FontIconSizeEnum.Standard);
    }


    /// <summary>
    /// Adds default item to control (link over the tree).
    /// </summary>
    /// <param name="itemName">Item name</param>
    /// <param name="value">Value</param>
    /// <param name="iconClass">Font icon class</param>
    public void AddDefaultItem(string itemName, string value, string iconClass)
    {
        string imgTag = "";
        itemName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(itemName));
        
        // Generate image HTML tag
        if (!String.IsNullOrEmpty(iconClass))
        {
            imgTag = UIHelper.GetAccessibleIconTag(iconClass);
        }

        // Insert default item to arraylist
        string[] defaultItem = new string[3];
        defaultItem[0] = itemName;
        defaultItem[1] = imgTag;
        defaultItem[2] = value;

        defaultItems.Add(defaultItem);
    }


    /// <summary>
    /// Returns javascript code raising postback and OnItemSelected event.
    /// </summary>
    public string GetOnSelectedItemBackEventReference()
    {
        return ControlsHelper.GetPostBackEventReference(btnItemSelected);
    }


    /// <summary>
    /// Handles simulated hidden button click and raises OnItemSelected event with value from hidden field.
    /// </summary>
    protected void btnItemSelected_Click(object sender, EventArgs e)
    {
        RaiseOnItemSelected(hdnSelectedItem.Value);
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Raises event postback event.
    /// </summary>
    public void RaisePostBackEvent(string eventArgument)
    {
        // Get argument
        string arg = HttpUtility.HtmlDecode(eventArgument);

        // Get value
        string[] selectedItem = arg.Split(';');
        string value = selectedItem[0];

        // Get path
        if (selectedItem.Length >= 2)
        {
            selectedPath = selectedItem[1].ToLowerCSafe();
        }

        // Raise event
        RaiseOnItemSelected(value);
    }

    #endregion
}
