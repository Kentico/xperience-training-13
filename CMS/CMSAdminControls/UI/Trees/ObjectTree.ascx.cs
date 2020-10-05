using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_Trees_ObjectTree : CMSUserControl
{
    #region "Events"

    /// <summary>
    /// Fires when new node is created, returns true if the object should be created.
    /// </summary>
    public event ObjectTypeTreeNode.OnBeforeCreateNodeHandler OnBeforeCreateNode;

    #endregion


    #region "Variables"

    private string mNodeTextTemplate = "##ICON####NODENAME##";
    private string mSelectedNodeTextTemplate = "##ICON####NODENAME##";

    #endregion


    #region "Properties"

    /// <summary>
    /// Tree definition.
    /// </summary>
    public ObjectTypeTreeNode RootNode
    {
        get;
        set;
    }


    /// <summary>
    /// Template of the node text.
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
    /// Template of the selected node text.
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
    /// Template of the node Value.
    /// </summary>
    public string ValueTextTemplate
    {
        get;
        set;
    }


    /// <summary>
    /// Site ID.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Tree view control.
    /// </summary>
    public TreeView TreeView
    {
        get
        {
            return treeElem;
        }
    }


    /// <summary>
    /// Use postback on node selection.
    /// </summary>
    public bool UsePostback
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control view state is enabled
    /// </summary>
    public override bool EnableViewState
    {
        get
        {
            return base.EnableViewState;
        }
        set
        {
            base.EnableViewState = value;
            treeElem.EnableViewState = value;
        }
    }


    /// <summary>
    /// Defines whether some specific object type should be preselected.
    /// </summary>
    public string PreselectObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Defines whether preselected object type is site-related object type.
    /// </summary>
    public bool IsPreselectedObjectTypeSiteObject
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        treeElem.ExpandImageToolTip = GetString("contenttree.expand");
        treeElem.CollapseImageToolTip = GetString("contenttree.collapse");

        string imagePath = "Design/Controls/Tree";
        if (CultureHelper.IsUICultureRTL())
        {
            imagePath = "RTL/" + imagePath;
        }

        treeElem.LineImagesFolder = GetImageUrl(imagePath, false);

        if (!RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Reloads control data
    /// </summary>
    public void ReloadData()
    {
        treeElem.Nodes.Clear();

        if (RootNode != null)
        {
            TreeNode rootNode = CreateNode(RootNode);
            rootNode.Selected = true;
            rootNode.Expanded = true;

            treeElem.Nodes.Add(rootNode);

            // Expand node structure
            TreeNode nodeToExpand = treeElem.SelectedNode.Parent;
            while (nodeToExpand != null)
            {
                nodeToExpand.Expanded = true;
                nodeToExpand = nodeToExpand.Parent;
            }
        }
    }


    /// <summary>
    /// Defines whether preselected object type is contained in the structure.
    /// </summary>
    public bool ContainsObjectType(string type)
    {
        if (treeElem.Nodes.Count > 0)
        {
            return ContainsObjectType(treeElem.Nodes[0], type);
        }

        return false;
    }


    /// <summary>
    /// Defines whether preselected object type is contained in the structure.
    /// </summary>
    public bool ContainsObjectType(TreeNode nodeToExpand, string type)
    {
        if (nodeToExpand.Value == type)
        {
            return true;
        }

        if (nodeToExpand.ChildNodes.Count > 0)
        {
            foreach (TreeNode node in nodeToExpand.ChildNodes)
            {
                if (ContainsObjectType(node, type))
                {
                    return true;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// Create tree node from supplied data
    /// </summary>
    /// <param name="source">Tree node data</param>
    /// <returns>TreeNode</returns>
    public TreeNode CreateNode(ObjectTypeTreeNode source)
    {
        // Create new node
        TreeNode newNode = new TreeNode();

        // Get the image
        string type = source.ObjectType ?? source.Group;

        if (!UsePostback)
        {
            newNode.NavigateUrl = "#";
        }

        string template = NodeTextTemplate;

        // Site
        int siteId = 0;
        if (source.Site)
        {
            siteId = SiteID;
        }

        // Title
        var name = GetNodeName(type);

        if ((type == String.Empty) || (type == ObjectHelper.GROUP_OBJECTS))
        {
            template = SelectedNodeTextTemplate;
            siteId = -1;
            name = GetString("ObjectTasks.Root");
        }

        if (source.Main)
        {
            name = "<strong>" + name + "</strong>";
        }

        newNode.Text = template.Replace("##NODENAME##", name).Replace("##SITEID##", siteId.ToString()).Replace("##OBJECTTYPE##", HttpUtility.UrlEncode(type));

        if (ValueTextTemplate != null)
        {
            newNode.Value = ValueTextTemplate.Replace("##SITEID##", siteId.ToString()).Replace("##OBJECTTYPE##", type);
        }

        // Disable if not active node
        if (UsePostback && !source.Active)
        {
            newNode.SelectAction = TreeNodeSelectAction.None;
        }

        if (source.Group != null && !source.Active)
        {
            newNode.SelectAction = TreeNodeSelectAction.Expand;
        }

        // Add child nodes
        if (source.ChildNodes.Count > 0)
        {
            IEnumerable<ObjectTypeTreeNode> children = source.ChildNodes;
            if (source.SortChildren)
            {
                children = children.OrderBy(n => GetNodeName(n.ObjectType ?? n.Group));
            }

            foreach (var child in children)
            {
                if ((SiteID > 0) || !child.Site)
                {
                    if ((OnBeforeCreateNode == null) || OnBeforeCreateNode(child))
                    {
                        // Create child node
                        TreeNode childNode = CreateNode(child);
                        if ((child.ObjectType != null) || (childNode.ChildNodes.Count > 0))
                        {
                            newNode.ChildNodes.Add(childNode);
                            // Preselect node
                            if ((child.ObjectType == PreselectObjectType) && (child.Site == IsPreselectedObjectTypeSiteObject))
                            {
                                childNode.Selected = true;
                            }
                        }
                    }
                }
            }
        }

        newNode.Expanded = source.Expand;

        return newNode;
    }


    /// <summary>
    /// Gets node name according to supplied type
    /// </summary>
    /// <param name="type">Object type or group</param>
    private string GetNodeName(string type)
    {
        return GetString(TypeHelper.GetTasksResourceKey(type));
    }

    #endregion
}