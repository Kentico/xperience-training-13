using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UIProfiles_UIMenu : UIMenu
{
    #region "Variables"

    private string mRootTargetURL = String.Empty;

    private bool mModuleAvailabilityForSiteRequired;

    private UIElementInfo root;

    private int totalNodes;
    private int mMaxRelativeLevel = -1;
    private bool mEnableRootSelect = true;
    private bool mUseIFrame;

    private string mSelectedNode = String.Empty;
    private string mSelectedNodeTemplate;
    private string mNodeTemplate;

    public const string NODE_CODENAME_MACRO = "##NODECODENAME##";
    public const string NODE_NAME_MACRO = "##NODENAME##";

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, target frame is not in parent frames but iframe
    /// </summary>
    public bool UseIFrame
    {
        get
        {
            return mUseIFrame;
        }
        set
        {
            mUseIFrame = value;
        }
    }


    /// <summary>
    /// Gets the root node of the tree.
    /// </summary>
    public TreeNode RootNode
    {
        get
        {
            return treeElem.CustomRootNode;
        }
    }


    /// <summary>
    /// Used in scenario when starting page is different from selected node
    /// </summary>
    public String StartingPage
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the root element is clickable.
    /// </summary>
    public bool EnableRootSelect
    {
        get
        {
            return mEnableRootSelect;
        }
        set
        {
            mEnableRootSelect = value;
        }
    }


    /// <summary>
    /// Code name of the UIElement.
    /// </summary>
    public string ElementName
    {
        get;
        set;
    }


    /// <summary>
    /// Code name of the module.
    /// </summary>
    public string ModuleName
    {
        get;
        set;
    }


    /// <summary>
    /// Query parameter name for the selection of the item.
    /// </summary>
    public string QueryParameterName
    {
        get;
        set;
    }


    /// <summary>
    /// Get selected node code name.
    /// </summary>
    private string SelectedNode
    {
        get
        {
            if (String.IsNullOrEmpty(mSelectedNode))
            {
                mSelectedNode = QueryHelper.GetString(QueryParameterName, String.Empty);
                if (mSelectedNode.StartsWithCSafe("cms.", true))
                {
                    mSelectedNode = mSelectedNode.Substring(4);
                }

                treeElem.SelectedItem = mSelectedNode;
            }
            return mSelectedNode.ToLowerCSafe();
        }
        set
        {
            treeElem.SelectedItem = SelectedNode;
            mSelectedNode = value;
        }
    }


    /// <summary>
    /// Template of selected tree node.
    /// </summary>
    public string SelectedNodeTemplate
    {
        get
        {
            return mSelectedNodeTemplate ?? (mSelectedNodeTemplate = "<span id=\"node_" + NODE_CODENAME_MACRO + "\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">" + NODE_NAME_MACRO + "</span></span>");
        }
        set
        {
            mSelectedNodeTemplate = value;
        }
    }


    /// <summary>
    /// Template of tree node.
    /// </summary>
    public string NodeTemplate
    {
        get
        {
            return mNodeTemplate ?? (mNodeTemplate = "<span id=\"node_" + NODE_CODENAME_MACRO + "\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">" + NODE_NAME_MACRO + "</span></span>");
        }
        set
        {
            mNodeTemplate = value;
        }
    }


    /// <summary>
    /// Indicates if all nodes should be expanded.
    /// </summary>
    public bool ExpandAll
    {
        get
        {
            return treeElem.ExpandAll;
        }
        set
        {
            treeElem.ExpandAll = value;
        }
    }


    /// <summary>
    /// Indicates number of expanded levels.
    /// </summary>
    public int ExpandLevel
    {
        get
        {
            return treeElem.ExpandLevel;
        }
        set
        {
            treeElem.ExpandLevel = value;
        }
    }


    /// <summary>
    /// Gets the value which indicates whether there is some tab displayed or not.
    /// </summary>
    public bool MenuEmpty
    {
        get
        {
            if (treeElem.CustomRootNode != null)
            {
                return (treeElem.CustomRootNode.ChildNodes.Count == 0);
            }
            return true;
        }
    }


    /// <summary>
    /// Root node target URL.
    /// </summary>
    public string RootTargetURL
    {
        get
        {
            return mRootTargetURL;
        }
        set
        {
            mRootTargetURL = value;
        }
    }


    /// <summary>
    /// Indicates if site availability of the corresponding module (module with name in format "cms.[ElementName]") is required for each UI element in the menu. Takes effect only when corresponding module exists.
    /// </summary>
    public bool ModuleAvailabilityForSiteRequired
    {
        get
        {
            return mModuleAvailabilityForSiteRequired;
        }
        set
        {
            mModuleAvailabilityForSiteRequired = value;
        }
    }


    /// <summary>
    /// Gets or sets maximal relative level displayed (depth of the tree to load).
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return mMaxRelativeLevel;
        }
        set
        {
            mMaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Indicates if the icon should be displayed in the root of the tree.
    /// </summary>
    public bool DisplayRootIcon
    {
        get;
        set;
    }


    /// <summary>
    /// Add editing icon in development mode
    /// </summary>
    private string DevIcon
    {
        get
        {
            if (SystemContext.DevelopmentMode && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                return PortalUIHelper.GetResourceUIElementLink(ModuleName, ElementName);
            }

            return String.Empty;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetRootNode();

        SetTreeProvider();

        HandleThePreselection();

        treeElem.UsePostBack = false;
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;

        SetNodeTemplate();

        treeElem.ReloadData();


        RegisterJavascriptFroHighlighting();
        RegisterJavascriptForPreselection();

        LogTheSecurity();
    }

    #endregion


    #region "Custom methods"

    /// <summary>
    /// Set path to preselected item
    /// </summary>
    private void HandleThePreselection()
    {
        if (!String.IsNullOrEmpty(SelectedNode))
        {
            UIElementInfo uiElement = UIElementInfoProvider.GetUIElementInfo(ModuleName, SelectedNode);
            if (uiElement != null)
            {
                treeElem.ExpandPath = uiElement.ElementIDPath;
            }
        }
    }


    /// <summary>
    /// Set tree provider
    /// </summary>
    protected void SetTreeProvider()
    {
        string levelWhere = (MaxRelativeLevel <= 0 ? "" : " (ElementLevel <= " + (root.ElementLevel + MaxRelativeLevel) + ")");
        string levelColumn = "CASE ElementLevel WHEN " + (root.ElementLevel + MaxRelativeLevel) + " THEN 0 ELSE ElementChildCount END AS ElementChildCount";

        UniTreeProvider provider = new UniTreeProvider();
        provider.RootLevelOffset = root.ElementLevel;
        provider.ObjectType = "cms.uielement";
        provider.DisplayNameColumn = "ElementDisplayName";
        provider.IDColumn = "ElementID";
        provider.LevelColumn = "ElementLevel";
        provider.OrderColumn = "ElementOrder";
        provider.ParentIDColumn = "ElementParentID";
        provider.PathColumn = "ElementIDPath";
        provider.ValueColumn = "ElementName";
        provider.ChildCountColumn = "ElementChildCount";
        provider.WhereCondition = levelWhere;
        provider.CaptionColumn = "ElementCaption";
        provider.Columns = "ElementID, ElementName, ElementDisplayName, ElementGuid, ElementLevel, ElementOrder, ElementIconClass, ElementParentID, ElementIDPath, ElementCaption, ElementIconPath, ElementTargetURL, ElementType, ElementFeature, ElementVisibilityCondition, ElementResourceID, " + levelColumn;
        provider.ImageColumn = "ElementIconPath";
        provider.IconClassColumn = "ElementIconClass";

        treeElem.ProviderObject = provider;
    }


    /// <summary>
    /// Set default template for nodes
    /// </summary>
    protected void SetNodeTemplate()
    {
        treeElem.SelectedNodeTemplate = SelectedNodeTemplate;
        treeElem.NodeTemplate = NodeTemplate;
    }


    protected void SetRootNode()
    {
        if (String.IsNullOrEmpty(ElementName))
        {
            // Get the root UI element
            root = UIElementInfoProvider.GetRootUIElementInfo(ModuleName);
        }
        else
        {
            // Get the specified element
            root = UIElementInfoProvider.GetUIElementInfo(ModuleName, ElementName);
        }

        string codeName = root.ElementName.Replace(".", String.Empty).ToLowerCSafe();
        string caption = UIElementInfoProvider.GetElementCaption(root);
        string rootUrl = !String.IsNullOrEmpty(RootTargetURL) ? GetUrl(RootTargetURL) : GetUrl(root.ElementTargetURL);

        string rootText = caption + DevIcon;

        if (EnableRootSelect)
        {
            rootText = String.Format("<a href=\"{0}\" target=\"{1}\"><span id=\"node_{2}\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\"><span class=\"Name\">{3}</span></span></a>", rootUrl, TargetFrame, codeName, caption);
            treeElem.SetRoot(rootText, root.ElementID.ToString(), null, rootUrl, TargetFrame);
            treeElem.EnableRootAction = true;
        }
        else
        {
            treeElem.SetRoot(rootText, root.ElementID.ToString(), null);
        }

        treeElem.ExpandPath = root.ElementIDPath;
    }


    /// <summary>
    /// Logs security info
    /// </summary>
    protected void LogTheSecurity()
    {
        DataRow sdr = SecurityDebug.StartSecurityOperation("LoadUIMenu");

        if (sdr != null)
        {
            SecurityDebug.FinishSecurityOperation(sdr, MembershipContext.AuthenticatedUser.UserName, ModuleName, ElementName, totalNodes, SiteContext.CurrentSiteName);
        }
    }


    /// <summary>
    /// Register JS for highlighting nodes after click
    /// </summary>
    protected void RegisterJavascriptFroHighlighting()
    {
        string script = @"
function SelectNode(node){
    $cmsj('.ContentTreeSelectedItem').removeClass('ContentTreeSelectedItem').addClass('ContentTreeItem');
    $cmsj(node).addClass('ContentTreeSelectedItem').removeClass('ContentTreeItem');    
}
$cmsj(document).on('click', 'span[name=treeNode]', function(){
    SelectNode(this);
});";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "UIMenu_SelectItem", script, true);
    }


    /// <summary>
    /// Register JS for preselection node
    /// </summary>
    protected void RegisterJavascriptForPreselection()
    {
        if (!String.IsNullOrEmpty(SelectedNode))
        {
            String target = UseIFrame ? "" : "parent.";
            String targetUrl = String.IsNullOrEmpty(StartingPage) ? "$cmsj(node).parent().attr('href')" : "'" + StartingPage + "'";
            string script = @"
$cmsj(document).ready(SelectPreselectedNode('#node_" + SelectedNode.Replace(".", String.Empty) + @"'));
                    
function SelectPreselectedNode(node){
    var targetFrame = $cmsj(node).parent().attr('target');
    var targetUrl = $cmsj(node).parent().attr('href');

    " + target + @"frames[targetFrame].location.href = " + targetUrl + @";

    SelectNode(node);
}";

            ScriptHelper.RegisterStartupScript(this, typeof(string), "UIMenu_PreSelectItem", script, true);
        }
    }


    /// <summary>
    /// Get completly resolved url (resolve macros, hash, url)
    /// </summary>
    private string GetUrl(string url)
    {
        url = MacroResolver.Resolve(url);
        url = URLHelper.EnsureHashToQueryParameters(url);
        url = UrlResolver.ResolveUrl(url);

        return url;
    }

    #endregion


    #region "On Events"

    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        if (itemData != null)
        {
            UIElementInfo uiElement = new UIElementInfo(itemData);

            if (!UIContextHelper.CheckElementAvailabilityInUI(uiElement))
            {
                return null;
            }

            // Check permissions
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement(uiElement.ElementResourceID, uiElement.ElementName, ModuleAvailabilityForSiteRequired))
            {
                defaultNode = RaiseOnNodeCreated(uiElement, defaultNode);

                String url = UIContextHelper.GetElementUrl(uiElement, UIContext);
                if (uiElement.ElementType != UIElementTypeEnum.PageTemplate)
                {
                    url = URLHelper.RemoveParameterFromUrl(url, "displaytitle");
                }

                // If url is empty -> don't show
                if (defaultNode == null || String.IsNullOrEmpty(url))
                {
                    return null;
                }

                // Prepare node info
                string caption = UIElementInfoProvider.GetElementCaption(uiElement);
                string targetFrame = !String.IsNullOrEmpty(defaultNode.Target) ? defaultNode.Target : TargetFrame;
                string codeName = uiElement.ElementName.Replace(".", String.Empty).ToLowerCSafe();

                UIElementInfo sel = UIContextHelper.CheckSelectedElement(uiElement, UIContext);
                if (sel != null)
                {
                    SelectedNode = uiElement.ElementName;

                    String selectionSuffix = ValidationHelper.GetString(UIContext["selectionSuffix"], String.Empty);
                    String spUrl = UIContextHelper.GetElementUrl(sel, UIContext);
                    spUrl = URLHelper.AppendQuery(spUrl, selectionSuffix);

                    // Append object ID
                    url = URLHelper.UpdateParameterInUrl(url, "objectid", UIContext.ObjectID.ToString());

                    StartingPage = URLHelper.UpdateParameterInUrl(spUrl, "displaytitle", "true");
                }

                // Set node
                defaultNode.Target = targetFrame;
                defaultNode.NavigateUrl = GetUrl(url);
                defaultNode.Text = defaultNode.Text.Replace(NODE_CODENAME_MACRO, codeName);

                totalNodes++;

                // Set first child node to be selected if root cant be selected
                if (!EnableRootSelect && string.IsNullOrEmpty(SelectedNode))
                {
                    // Is a page node (with page url)
                    if (url != ApplicationUrlHelper.COLLAPSIBLE_EMPTY_PARENT_ELEMENT_URL)
                    {
                        SelectedNode = uiElement.ElementName;
                    }
                    else
                    {
                        // Try to display a child element
                        if (uiElement.ElementChildCount > 0)
                        {
                            defaultNode.Expanded = true;
                        }
                    }
                }

                // If url is '@' don't redirect, only collapse children
                if (uiElement.ElementTargetURL == ApplicationUrlHelper.COLLAPSIBLE_EMPTY_PARENT_ELEMENT_URL)
                {
                    // Onclick simulates click on '+' or '-' button
                    const string onClick = "onClick=\"var js = $cmsj(this).parents('tr').find('a').attr('href');eval(js);return false; \"";
                    defaultNode.Text = "<span id=\"node_" + uiElement.ElementName + "\" class=\"ContentTreeItem \"" + onClick + " ><span class=\"Name\">" + ResHelper.LocalizeString(caption) + "</span></span>";
                }

                return defaultNode;
            }
        }

        return null;
    }

    #endregion
}
