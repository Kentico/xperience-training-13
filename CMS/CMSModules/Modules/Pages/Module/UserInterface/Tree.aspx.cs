using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Modules_Pages_Module_UserInterface_Tree : GlobalAdminPage
{
    #region "Properties"

    /// <summary>
    /// Id of the current module.
    /// </summary>
    protected int ModuleId
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the current module resource info
    /// </summary>
    protected ResourceInfo CurrentModule
    {
        get
        {
            return ResourceInfo.Provider.Get(ModuleId);
        }
    }


    /// <summary>
    /// Id of passed element. 
    /// </summary>
    protected int ElementId
    {
        get;
        set;
    }


    /// <summary>
    /// Passed UI element.
    /// </summary>
    protected UIElementInfo ElementInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Url to edit UIElement page.
    /// </summary>
    protected string ContentUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Script that selects node.
    /// </summary>
    protected string SelectNodeScript
    {
        get;
        set;
    }

    #endregion


    #region "Dynamic controls"

    private UniTree mTree;
    private CMSAccessibleButton mDeleteButton;
    private CMSAccessibleButton mNewButton;
    private CMSAccessibleButton mUpButton;
    private CMSAccessibleButton mDownButton;

    /// <summary>
    /// Tree control.
    /// </summary>
    private UniTree ContentTree
    {
        get
        {
            return mTree ?? (mTree = (UniTree)paneTree.FindControl("t"));
        }
    }


    /// <summary>
    /// Delete item button.
    /// </summary>
    protected CMSAccessibleButton NewButton
    {
        get
        {
            return mNewButton ?? (mNewButton = (CMSAccessibleButton)paneMenu.FindControl("btnNew"));
        }
    }

    /// <summary>
    /// Delete item button.
    /// </summary>
    protected CMSAccessibleButton DeleteButton
    {
        get
        {
            return mDeleteButton ?? (mDeleteButton = (CMSAccessibleButton)paneMenu.FindControl("btnDelete"));
        }
    }

    /// <summary>
    /// Delete item button.
    /// </summary>
    protected CMSAccessibleButton UpButton
    {
        get
        {
            return mUpButton ?? (mUpButton = (CMSAccessibleButton)paneMenu.FindControl("btnUp"));
        }
    }

    /// <summary>
    /// Delete item button.
    /// </summary>
    protected CMSAccessibleButton DownButton
    {
        get
        {
            return mDownButton ?? (mDownButton = (CMSAccessibleButton)paneMenu.FindControl("btnDown"));
        }
    }

    #endregion


    #region "Methods & Events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        ModuleId = QueryHelper.GetInteger("moduleid", 0);

        if (!RequestHelper.IsPostBack())
        {
            ElementId = QueryHelper.GetInteger("elementId", 0);

            if (ElementId > 0)
            {
                ElementInfo = UIElementInfo.Provider.Get(ElementId);
            }

            // If null get root element
            if (ElementInfo == null)
            {
                ElementInfo = UIElementInfoProvider.GetRootUIElementInfo("cms");
            }
        }

        ContentUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.UserInterface.Edit", false);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SetupActionButtons();

        // Select element and expand modules first level elements
        if (!RequestHelper.IsPostBack())
        {
            // Select given element
            if (ElementInfo != null)
            {
                SelectNode(ElementInfo);
                ContentTree.SelectPath = ElementInfo.ElementIDPath;
            }

            // Expand all module elements
            var allModuleElements = UIElementInfo.Provider.Get().Where("ElementResourceID", QueryOperator.Equals, ModuleId).Columns("ElementID");

            var elementsToExpand = UIElementInfo.Provider.Get()
                .Where("ElementResourceID", QueryOperator.Equals, ModuleId)
                .Where(new WhereCondition().WhereNull("ElementParentID").Or().WhereNotIn("ElementParentID", allModuleElements));

            foreach (var uiElementInfo in elementsToExpand)
            {
                if (!String.IsNullOrEmpty(uiElementInfo.ElementIDPath))
                {
                    ContentTree.ExpandPath += uiElementInfo.ElementIDPath + ";";

                    // No element is selected, select first
                    if (ElementInfo == null)
                    {
                        SelectNode(uiElementInfo);
                        ContentTree.SelectPath = uiElementInfo.ElementIDPath;
                    }
                }
            }
        }

        // Create and set UIElements provider
        var tree = new UniTreeProvider();
        tree.ObjectType = "CMS.UIElement";
        tree.DisplayNameColumn = "ElementDisplayName";
        tree.IDColumn = "ElementID";
        tree.LevelColumn = "ElementLevel";
        tree.OrderColumn = "ElementOrder";
        tree.ParentIDColumn = "ElementParentID";
        tree.PathColumn = "ElementIDPath";
        tree.ValueColumn = "ElementID";
        tree.ChildCountColumn = "ElementChildCount";
        tree.Columns = "ElementID,ElementLevel,ElementOrder,ElementParentID,ElementIDPath,ElementChildCount,ElementDisplayName,ElementResourceID,ElementName,ElementIconPath,ElementIconClass";

        ContentTree.UsePostBack = false;
        ContentTree.ProviderObject = tree;
        ContentTree.OnNodeCreated += uniTree_OnNodeCreated;

        ContentTree.NodeTemplate = "<span id=\"node_##NODEID##\" onclick=\"SelectNode(##NODEID##,##PARENTNODEID##," + ModuleId + ", true); return false;\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        ContentTree.SelectedNodeTemplate = "<span id=\"node_##NODEID##\" onclick=\"SelectNode(##NODEID##,##PARENTNODEID##," + ModuleId + ", true); return false;\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

        // Icons 
        tree.ImageColumn = "ElementIconPath";
    }


    /// <summary>
    /// OnNodeCreated event handler.
    /// </summary>
    TreeNode uniTree_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        bool isChild = false;
        bool disable = false;

        UIElementInfo ui = new UIElementInfo(itemData);

        string[] paths = ContentTree.ExpandPath.ToLowerCSafe().Split(';');
        string nodePath = ui.ElementIDPath;
        if (!nodePath.EndsWith("/", StringComparison.Ordinal))
        {
            nodePath += "/";
        }

        string cssClass = null;

        // Check expanded paths
        foreach (string t in paths)
        {
            var path = t;
            if (path != String.Empty)
            {
                // Add slash - select only children
                if (!path.EndsWith("/", StringComparison.Ordinal))
                {
                    path += "/";
                }

                if (!isChild)
                {
                    isChild = nodePath.StartsWith(path, StringComparison.InvariantCulture);
                }

                cssClass = " highlighted";

                if ((path.StartsWithCSafe(nodePath)))
                {
                    defaultNode.Expanded = true;
                    break;
                }
            }
        }

        // ResourceID
        if ((ModuleId != 0) && (ModuleId != ui.ElementResourceID))
        {
            disable = true;
        }

        string displayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ui.ElementDisplayName));

        // Get tree icon
        string icon = UIHelper.GetAccessibleImageMarkup(this, ui.ElementIconClass, ui.ElementIconPath, size: FontIconSizeEnum.Standard);

        if (disable)
        {
            cssClass = " disabled";
            
            // Expanded node = different module, but parent from module nodes
            if (!isChild && ((defaultNode.Expanded != null) && defaultNode.Expanded.Value))
            {
                cssClass += " highlighted";
            }
        }

        // Ensure default template
        defaultNode.Text = String.Format("<span class=\"ContentTreeItem{5}\" id=\"node_{0}\" onclick=\"SelectNode({0},{1},{2}, true); return false;\" name=\"treeNode\">{4}<span class=\"Name\">{3}</span></span>",
            ui.ElementID, ui.ElementParentID, ui.ElementResourceID, displayName, icon, cssClass);

        return defaultNode;
    }


    /// <summary>
    /// OnPreRender event handler
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        // Select node
        if (!String.IsNullOrEmpty(SelectNodeScript))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "SelectTreeNode", SelectNodeScript, true);
        }

        // Load data
        ContentTree.ReloadData();

        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    protected void btnMoveUp_Click(object sender, EventArgs e)
    {
        GetHiddenValue();
        if (ElementId > 0)
        {
            UIElementInfoProvider.MoveUIElementUp(ElementId);
            AfterAction("moveup", ElementId);
        }
    }


    protected void btnMoveDown_Click(object sender, EventArgs e)
    {
        GetHiddenValue();
        if (ElementId > 0)
        {
            UIElementInfoProvider.MoveUIElementDown(ElementId);
            AfterAction("movedown", ElementId);
        }
    }


    protected void btnDeleteElem_Click(object sender, EventArgs e)
    {
        GetHiddenValue();
        if ((ElementId > 0) && (ElementInfo.ElementParentID > 0))
        {
            ResourceInfo ri = ResourceInfo.Provider.Get(ElementInfo.ElementResourceID);
            if ((ri != null) && !ri.ResourceIsInDevelopment)
            {
                ShowError(GetString("module.action.resourcenotindevelopment"));
                return;
            }

            UIElementInfo.Provider.Delete(UIElementInfo.Provider.Get(ElementId));
            AfterAction("delete", ElementInfo.ElementParentID);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets element ID from hidden value.
    /// </summary>
    private void GetHiddenValue()
    {
        string hidValue = hidSelectedElem.Value;
        int hiddenElemId = ValidationHelper.GetInteger(hidValue, 0);
        if ((hiddenElemId > 0) && (ElementId <= 0))
        {
            ElementId = hiddenElemId;
            ElementInfo = UIElementInfo.Provider.Get(ElementId);
        }
    }


    /// <summary>
    /// Sets selection after action.
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="elementId">Element ID</param>
    private void AfterAction(string actionName, int elementId)
    {
        UIElementInfo elemInfo = UIElementInfo.Provider.Get(elementId);
        if (elemInfo != null)
        {
            ContentTree.SelectPath = elemInfo.ElementIDPath;
            ContentTree.ExpandPath = elemInfo.ElementIDPath + "/";

            switch (actionName.ToLowerCSafe())
            {
                case "delete":
                    // Update menu actions parameters
                    SelectNode(elemInfo);
                    break;
                case "moveup":
                case "movedown":
                    SelectNode(elemInfo, false);
                    break;
                default:
                    SelectNode(elemInfo);
                    break;
            }
        }
    }


    /// <summary>
    /// Register javascript for node selection
    /// </summary>
    /// <param name="element">UIElement to select</param>
    /// <param name="refreshContent">When true content frame is refreshed</param>
    private void SelectNode(UIElementInfo element, bool refreshContent = true)
    {
        SelectNodeScript = String.Format("SelectNode({0}, {1}, {2}, {3});", element.ElementID, element.ElementParentID, element.ElementResourceID, refreshContent ? "true" : "false");
    }


    /// <summary>
    /// Setups action buttons.
    /// </summary>
    private void SetupActionButtons()
    {
        // Prepare actions texts
        NewButton.ToolTip = GetString("resource.ui.newelem");
        DeleteButton.ToolTip = GetString("resource.ui.deleteelem");
        UpButton.ToolTip = GetString("resource.ui.modeupelem");
        DownButton.ToolTip = GetString("resource.ui.modedownelem");

        // Create new element javascript
        string newScript = "var hidElem = document.getElementById('" + hidSelectedElem.ClientID + "'); var ids = hidElem.value.split('|');";
        newScript += "if ((window.parent != null) && (window.parent.frames['uicontent'] != null)) {";
        newScript += "window.parent.frames['uicontent'].location = '" + UIContextHelper.GetElementUrl(ModuleName.CMS, "Modules.UserInterface.New", false) + "&moduleid=" + ModuleId + "&parentId=' + ids[0];";
        newScript += "} return false;";
        NewButton.OnClientClick = newScript;

        string script = "var menuHiddenId = '" + hidSelectedElem.ClientID + "';";
        script += "function deleteConfirm() {";
        script += "return confirm(" + ScriptHelper.GetString(GetString("resource.ui.confirmdelete")) + ");";
        script += "}";
        ltlScript.Text = ScriptHelper.GetScript(script);
    }

    #endregion
}