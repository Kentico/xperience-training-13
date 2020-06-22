using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;

public partial class CMSModules_Settings_Controls_SettingsTree : CMSUserControl
{
    #region "Variables"

    private int mTabIndex;

    protected string mCategoryName = null;
    protected string mModuleName = null;
    protected string mJavaScriptHandler = null;

    private SettingsCategoryInfo mRoot;

    private int mMaxRelativeLevel = 1;
    private bool mRootIsClickable = true;
    private bool mShowEmptyCategories = true;
    private bool mShowHeaderPanel = true;

    private string mExpandedPaths = String.Empty;

    #endregion


    #region "Dynamic controls"

    private Panel mSitePanel;
    private Panel mActionsPanel;
    private UniTree mTree;
    private CMSAccessibleButton mNewItemButton;
    private CMSAccessibleButton mDeleteItemButton;
    private CMSAccessibleButton mMoveUpItemButton;
    private CMSAccessibleButton mMoveDownItemButton;
    private FormEngineUserControl mSiteSelector;


    /// <summary>
    /// Site selector
    /// </summary>
    private FormEngineUserControl SiteSelector
    {
        get
        {
            return mSiteSelector ?? (mSiteSelector = (FormEngineUserControl)paneMenu.FindControl("siteSelector"));
        }
    }


    /// <summary>
    /// Site panel
    /// </summary>
    private Panel SitePanel
    {
        get
        {
            return mSitePanel ?? (mSitePanel = (Panel)paneMenu.FindControl("pnlSite"));
        }
    }


    /// <summary>
    /// Actions panel
    /// </summary>
    private Panel ActionsPanel
    {
        get
        {
            return mActionsPanel ?? (mActionsPanel = (Panel)paneMenu.FindControl("pnlActions"));
        }
    }


    /// <summary>
    /// Tree view
    /// </summary>
    private UniTree Tree
    {
        get
        {
            return mTree ?? (mTree = (UniTree)paneTree.FindControl("t"));
        }
    }


    /// <summary>
    /// New item button
    /// </summary>
    protected CMSAccessibleButton NewItemButton
    {
        get
        {
            return mNewItemButton ?? (mNewItemButton = (CMSAccessibleButton)paneMenu.FindControl("btnNew"));
        }
    }


    /// <summary>
    /// Delete item button
    /// </summary>
    protected CMSAccessibleButton DeleteItemButton
    {
        get
        {
            return mDeleteItemButton ?? (mDeleteItemButton = (CMSAccessibleButton)paneMenu.FindControl("btnDelete"));
        }
    }


    /// <summary>
    /// Move up item button
    /// </summary>
    protected CMSAccessibleButton MoveUpItemButton
    {
        get
        {
            return mMoveUpItemButton ?? (mMoveUpItemButton = (CMSAccessibleButton)paneMenu.FindControl("btnUp"));
        }
    }


    /// <summary>
    /// Move down item button
    /// </summary>
    protected CMSAccessibleButton MoveDownItemButton
    {
        get
        {
            return mMoveDownItemButton ?? (mMoveDownItemButton = (CMSAccessibleButton)paneMenu.FindControl("btnDown"));
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if site selector should be displayed
    /// </summary>
    public bool ShowSiteSelector
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value of hidden field where are stored ElementID and ParentID separated by |.
    /// </summary>
    public string Value
    {
        get
        {
            return hidSelectedElem.Value;
        }
        set
        {
            hidSelectedElem.Value = value;
        }
    }


    /// <summary>
    /// Element ID.
    /// </summary>
    public int CategoryID
    {
        get;
        set;
    }


    /// <summary>
    /// Parent element ID.
    /// </summary>
    public int ParentID
    {
        get;
        set;
    }


    /// <summary>
    /// Category module ID.
    /// </summary>
    public int CategoryModuleID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates selected module.
    /// </summary>
    private int ModuleID
    {
        get
        {
            return QueryHelper.GetInteger("moduleid", 0);
        }
    }

    /// <summary>
    /// Gets the root node of the tree.
    /// </summary>
    public TreeNode RootNode
    {
        get
        {
            return Tree.CustomRootNode;
        }
    }


    /// <summary>
    /// Gets or sets select path.
    /// </summary>
    public string SelectPath
    {
        get
        {
            return Tree.SelectPath;
        }
        set
        {
            Tree.SelectPath = value;
        }
    }


    /// <summary>
    /// Gets or sets expanded path.
    /// </summary>
    public string ExpandPath
    {
        get
        {
            return Tree.ExpandPath;
        }
        set
        {
            Tree.ExpandPath = value;
        }
    }


    /// <summary>
    /// Gets or sets selected item.
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return Tree.SelectedItem;
        }
        set
        {
            Tree.SelectedItem = value;
        }
    }


    /// <summary>
    /// Code name of the Category.
    /// </summary>
    public string CategoryName
    {
        get
        {
            return mCategoryName;
        }
        set
        {
            mCategoryName = value;
            mRoot = null;
        }
    }


    /// <summary>
    /// Name of the javascript function which is called when specified tab (Category) is clicked. 
    /// Category code name is passed as parameter.
    /// </summary>
    public string JavaScriptHandler
    {
        get
        {
            return mJavaScriptHandler;
        }
        set
        {
            mJavaScriptHandler = value;
        }
    }


    /// <summary>
    /// Gets the value which indicates whether there is some tab displayed or not.
    /// </summary>
    public bool MenuEmpty
    {
        get
        {
            if ((Tree.ProviderObject != null) && (Tree.ProviderObject.RootNode != null))
            {
                return Tree.ProviderObject.RootNode.HasChildNodes;
            }
            return false;
        }
    }


    /// <summary>
    /// Gets the value which indicates whether root node is clickable or not.
    /// </summary>
    public bool RootIsClickable
    {
        get
        {
            return (mRootIsClickable);
        }
        set
        {
            mRootIsClickable = value;
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
    /// Id of the site. Used for generating JavaScriptHandler second argument.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Category, which should be used as root category.
    /// </summary>
    public SettingsCategoryInfo RootCategory
    {
        get
        {
            if (mRoot == null)
            {
                // Get the info
                if (String.IsNullOrEmpty(CategoryName))
                {
                    // Get the root category
                    mRoot = SettingsCategoryInfoProvider.GetRootSettingsCategoryInfo();
                }
                else
                {
                    // Get the specified category
                    mRoot = SettingsCategoryInfo.Provider.Get(CategoryName);
                }
            }

            return mRoot;
        }
        set
        {
            mRoot = value;
            if (mRoot != null)
            {
                mCategoryName = mRoot.CategoryName;
            }
        }
    }


    /// <summary>
    /// Indicates whether categories without displayable keys are to be shown. Default value is true;
    /// </summary>
    public bool ShowEmptyCategories
    {
        get
        {
            return mShowEmptyCategories;
        }
        set
        {
            mShowEmptyCategories = value;
        }
    }


    /// <summary>
    /// Indicates whether header panel should be displayed on the top of settings tree.
    /// </summary>
    public bool ShowHeaderPanel
    {
        get
        {
            return mShowHeaderPanel;
        }
        set
        {
            mShowHeaderPanel = value;
        }
    }

    #endregion


    #region "Custom events"

    /// <summary>
    /// Node created delegate.
    /// </summary>
    public delegate TreeNode NodeCreatedEventHandler(SettingsCategoryInfo category, TreeNode defaultNode);


    /// <summary>
    /// Node created event handler.
    /// </summary>
    public event NodeCreatedEventHandler OnNodeCreated;

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads tree data.
    /// </summary>
    public void ReloadData()
    {
        GetExpandedPaths();

        Tree.ReloadData();
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        SitePanel.Visible = ShowHeaderPanel && ShowSiteSelector;
        ActionsPanel.Visible = ShowHeaderPanel && !SitePanel.Visible;
        plcActionSelectionScript.Visible = ShowHeaderPanel && !ShowSiteSelector;
        plcSelectionScript.Visible = !plcActionSelectionScript.Visible;

        if (RootCategory != null)
        {
            string levelWhere = (MaxRelativeLevel <= 0 ? "" : " AND (CategoryLevel <= " + (RootCategory.CategoryLevel + MaxRelativeLevel) + ")");
            // Restrict CategoryChildCount to MaxRelativeLevel. If level < MaxRelativeLevel, use count of non-group children.
            string levelColumn = "CASE CategoryLevel WHEN " + MaxRelativeLevel + " THEN 0 ELSE  (SELECT COUNT(*) AS CountNonGroup FROM CMS_SettingsCategory AS sc WHERE (sc.CategoryParentID = CMS_SettingsCategory.CategoryID) AND (sc.CategoryIsGroup = 0)) END AS CategoryChildCount";

            // Create and set category provider
            UniTreeProvider provider = new UniTreeProvider();
            provider.RootLevelOffset = RootCategory.CategoryLevel;
            provider.ObjectType = "CMS.SettingsCategory";
            provider.DisplayNameColumn = "CategoryDisplayName";
            provider.IDColumn = "CategoryID";
            provider.LevelColumn = "CategoryLevel";
            provider.OrderColumn = "CategoryOrder";
            provider.ParentIDColumn = "CategoryParentID";
            provider.PathColumn = "CategoryIDPath";
            provider.ValueColumn = "CategoryID";
            provider.ChildCountColumn = "CategoryChildCount";
            provider.ImageColumn = "CategoryIconPath";

            provider.WhereCondition = "((CategoryIsGroup IS NULL) OR (CategoryIsGroup = 0)) " + levelWhere;
            if (!ShowEmptyCategories)
            {
                var where = "CategoryID IN (SELECT CategoryParentID FROM CMS_SettingsCategory WHERE (CategoryIsGroup = 0) OR (CategoryIsGroup = 1 AND CategoryID IN (SELECT KeyCategoryID FROM CMS_SettingsKey WHERE ISNULL(SiteID, 0) = 0 AND ISNULL(KeyIsHidden, 0) = 0";
                if (SiteID > 0)
                {
                    where += " AND KeyIsGlobal = 0";
                }
                where += ")))";
                provider.WhereCondition = SqlHelper.AddWhereCondition(provider.WhereCondition, where);
            }
            provider.Columns = "CategoryID, CategoryName, CategoryDisplayName, CategoryLevel, CategoryOrder, CategoryParentID, CategoryIDPath, CategoryIconPath, CategoryResourceID, " + levelColumn;

            if (String.IsNullOrEmpty(JavaScriptHandler))
            {
                Tree.SelectedNodeTemplate = "<span id=\"node_##NODECODENAME##\" name=\"treeNode\" class=\"ContentTreeItem ##NAMECSSCLASS## ContentTreeSelectedItem\" onclick=\"SelectNode('##NODECODENAME##');\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span>";
                Tree.NodeTemplate = "<span id=\"node_##NODECODENAME##\" name=\"treeNode\" class=\"ContentTreeItem ##NAMECSSCLASS##\" onclick=\"SelectNode('##NODECODENAME##');\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span>";
            }
            else
            {
                Tree.SelectedNodeTemplate = "<span id=\"node_##NODECODENAME##\" name=\"treeNode\" class=\"ContentTreeItem ##NAMECSSCLASS## ContentTreeSelectedItem\" onclick=\"SelectNode('##NODECODENAME##'); if (" + JavaScriptHandler + ") { " + JavaScriptHandler + "('##NODECODENAME##',##NODEID##, ##SITEID##, ##PARENTID##, ##RESOURCEID##); }\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span>";
                Tree.NodeTemplate = "<span id=\"node_##NODECODENAME##\" name=\"treeNode\" class=\"ContentTreeItem ##NAMECSSCLASS##\" onclick=\"SelectNode('##NODECODENAME##'); if (" + JavaScriptHandler + ") { " + JavaScriptHandler + "('##NODECODENAME##',##NODEID##, ##SITEID##, ##PARENTID##, ##RESOURCEID##); }\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span>";
            }

            Tree.UsePostBack = false;
            Tree.ProviderObject = provider;
            Tree.ExpandPath = RootCategory.CategoryIDPath;

            Tree.OnNodeCreated += Tree_OnNodeCreated;
        }

        GetExpandedPaths();

        NewItemButton.ToolTip = GetString("settings.newelem");
        DeleteItemButton.ToolTip = GetString("settings.deleteelem");
        MoveUpItemButton.ToolTip = GetString("settings.modeupelem");
        MoveDownItemButton.ToolTip = GetString("settings.modedownelem");

        // Create new element javascript
        NewItemButton.OnClientClick = "return newItem();";

        // Confirm delete
        DeleteItemButton.OnClientClick = "if(!deleteConfirm()) { return false; }";

        var isPostback = RequestHelper.IsPostBack();
        if (!isPostback)
        {
            Tree.ReloadData();

            if (QueryHelper.GetBoolean("reloadtreeselect", false))
            {
                var category = SettingsCategoryInfo.Provider.Get(CategoryID);
                // Select requested category
                RegisterSelectNodeScript(category);
            }
        }

        if (ShowSiteSelector)
        {
            if (!isPostback)
            {
                if (QueryHelper.Contains("selectedSiteId"))
                {
                    // Get from URL
                    SiteID = QueryHelper.GetInteger("selectedSiteId", 0);
                    SiteSelector.Value = SiteID;
                }
            }
            else
            {
                SiteID = ValidationHelper.GetInteger(SiteSelector.Value, 0);
            }

            // Style site selector 
            SiteSelector.SetValue("AllowGlobal", true);
            SiteSelector.SetValue("GlobalRecordValue", 0);

            bool reload = QueryHelper.GetBoolean("reload", true);

            // URL for tree selection
            string script = "var categoryURL = '" + UIContextHelper.GetElementUrl(ModuleName.CMS, "Settings.Keys") + "';\n";
            script += "var doNotReloadContent = false;\n";

            // Select category
            SettingsCategoryInfo sci = SettingsCategoryInfo.Provider.Get(CategoryID);
            if (sci != null)
            {
                // Stop reloading of right frame, if explicitly set
                if (!reload)
                {
                    script += "doNotReloadContent = true;";
                }
                script += SelectAtferLoad(sci.CategoryIDPath, sci.CategoryName, sci.CategoryID, sci.CategoryParentID);
            }

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "SelectCat", ScriptHelper.GetScript(script));
        }
        else
        {
            ResourceInfo resource = ResourceInfo.Provider.Get(ModuleID);

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
var frameURL = '", UIContextHelper.GetElementUrl(ModuleName.CMS, "EditSettingsCategory", false), @"';
var rootId = ", (RootCategory != null ? RootCategory.CategoryID : 0), @";
var selectedModuleId = ", ModuleID, @";
var developmentMode = ", SystemContext.DevelopmentMode ? "true" : "false", @";
var resourceInDevelopment = ", (resource != null) && resource.ResourceIsInDevelopment ? "true" : "false", @";
var postParentId = ", CategoryID, @";

function newItem() {
    var hidElem = document.getElementById('" + hidSelectedElem.ClientID + @"');
    var ids = hidElem.value.split('|');
    if (window.parent != null && window.parent.frames['settingsmain'] != null) {
        window.parent.frames['settingsmain'].location = '" + ResolveUrl("~/CMSModules/Modules/Pages/Settings/Category/Edit.aspx") + @"?moduleid=" + ModuleID + @"&parentId=' + ids[0];
    } 
    return false;
}

function deleteConfirm() {
    return confirm(" + ScriptHelper.GetString(GetString("settings.categorydeleteconfirmation")) + @");
}
");

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "setupTreeScript", ScriptHelper.GetScript(sb.ToString()));
        }
    }


    protected TreeNode Tree_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        if (itemData != null)
        {
            if (MembershipContext.AuthenticatedUser != null)
            {
                SettingsCategoryInfo category = new SettingsCategoryInfo(itemData);

                string currentIdPath = category.CategoryIDPath;

                defaultNode.Expanded = category.CategoryParentID == 0 ? true : defaultNode.Expanded;

                if (!String.IsNullOrEmpty(mExpandedPaths) || (ModuleID > 0))
                {
                    bool selected = false;

                    foreach (string idPath in mExpandedPaths.Split(';'))
                    {
                        if (idPath == currentIdPath)
                        {
                            // End node
                            defaultNode.Expanded = true;

                            defaultNode.Text = defaultNode.Text.Replace("##NAMECSSCLASS##", "highlighted ");
                            selected = true;
                            break;
                        }

                        if (currentIdPath.StartsWith(idPath, StringComparison.Ordinal) && (ModuleID == category.CategoryResourceID))
                        {
                            // Child node
                            defaultNode.Expanded = true;
                            defaultNode.Text = defaultNode.Text.Replace("##NAMECSSCLASS##", "");
                            selected = true;
                            break;
                        }

                        if (idPath.StartsWith(currentIdPath, StringComparison.Ordinal))
                        {
                            // Parent node
                            defaultNode.Expanded = true;
                            defaultNode.Text = defaultNode.Text.Replace("##NAMECSSCLASS##", "highlighted" + (SystemContext.DevelopmentMode ? "" : " disabled"));
                            selected = true;
                            break;
                        }
                    }

                    if (!defaultNode.Expanded.Value || String.IsNullOrEmpty(mExpandedPaths) || !selected)
                    {
                        // Nodes which doesn't belong to selected module have no current module's group
                        defaultNode.Text = defaultNode.Text.Replace("##NAMECSSCLASS##", SystemContext.DevelopmentMode ? "" : "disabled");
                    }
                }
                else
                {
                    // Keep everything same as before and remove css class macro
                    defaultNode.Text = defaultNode.Text.Replace("##NAMECSSCLASS##", "");

                    if (RootCategory.CategoryIDPath == currentIdPath)
                    {
                        // Expand root node
                        defaultNode.Expanded = true;
                    }
                }

                defaultNode.Text = defaultNode.Text.Replace("##NODECUSTOMNAME##", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(category.CategoryDisplayName)));
                defaultNode.Text = defaultNode.Text.Replace("##NODECODENAME##", HTMLHelper.HTMLEncode(category.CategoryName));
                defaultNode.Text = defaultNode.Text.Replace("##SITEID##", SiteID.ToString());
                defaultNode.Text = defaultNode.Text.Replace("##PARENTID##", category.CategoryParentID.ToString());
                defaultNode.Text = defaultNode.Text.Replace("##RESOURCEID##", category.CategoryResourceID.ToString());

                if (OnNodeCreated != null)
                {
                    return OnNodeCreated(category, defaultNode);
                }
                
                return defaultNode;
            }
        }

        return null;
    }


    /// <summary>
    /// Registers calling the SelectNode script for selected category.
    /// </summary>
    /// <param name="category">Category to select</param>
    private void RegisterSelectNodeScript(SettingsCategoryInfo category)
    {
        if (category == null)
        {
            return;
        }

        string script = "SelectNode('" + category.CategoryName + "'); ";
        if (!String.IsNullOrEmpty(JavaScriptHandler))
        {
            script += "if (" + JavaScriptHandler + ") { " + JavaScriptHandler + "('" + category.CategoryName + "'," + category.CategoryID + "," + SiteID + ", " + category.CategoryParentID + ", " + category.CategoryResourceID + "); }";
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "refreshAfterAction", ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Expands tree at specified path and selects tree item by javascript.
    /// </summary>
    /// <param name="path">Selected path</param>
    /// <param name="itemName">Codename of selected tree item</param>
    /// <param name="itemId">Item identifier</param>
    /// <param name="parentId">ID of parent</param>    
    private string SelectAtferLoad(string path, string itemName, int itemId, int parentId)
    {
        SelectPath = path;
        SelectedItem = itemName;
        return String.Format("SelectNode('{0}');NodeSelected('', {2}, {3}, {1});", itemName, parentId, itemId, SiteID);
    }


    /// <summary>
    /// Gets all expanded paths for current module
    /// </summary>
    private void GetExpandedPaths()
    {
        if (ModuleID > 0)
        {
            mExpandedPaths = String.Empty;
            DataSet dsExpanded = SettingsCategoryInfoProvider.GetSettingsCategories("CategoryResourceID = " + ModuleID + " AND (NOT EXISTS (SELECT CategoryIDPath FROM CMS_SettingsCategory AS a WHERE CMS_SettingsCategory.CategoryIDPath LIKE a.CategoryIDPath + '%' AND CategoryResourceID = " + ModuleID + " AND CMS_SettingsCategory.CategoryIDPath != a.CategoryIDPath)) ", "CategoryLevel, CategoryOrder", 0, "CategoryIDPath, CategoryName");
            if (!DataHelper.DataSourceIsEmpty(dsExpanded))
            {
                foreach (DataRow row in dsExpanded.Tables[0].Rows)
                {
                    if (mExpandedPaths != String.Empty)
                    {
                        mExpandedPaths += ";" + row[0];
                    }
                    else
                    {
                        mExpandedPaths = row[0].ToString();
                    }
                }
            }
        }
    }


    private void GetHiddenValues()
    {
        string hidValue = hidSelectedElem.Value;
        string[] split = hidValue.Split('|');
        if (split.Length >= 2)
        {
            CategoryID = ValidationHelper.GetInteger(split[0], 0);
            ParentID = ValidationHelper.GetInteger(split[1], 0);
            if (split.Length == 3)
            {
                mTabIndex = ValidationHelper.GetInteger(split[2], 0);
            }
        }
    }


    #region "Button handling"



    /// <summary>
    /// Raised after menu action (new, delete, up or down) has been taken.
    /// </summary>
    protected void AfterAction(string actionName, int categoryId, int tabIndex = 0)
    {
        SettingsCategoryInfo category = SettingsCategoryInfo.Provider.Get(categoryId);
        if ((category.CategoryResourceID != ModuleID) && !SystemContext.DevelopmentMode)
        {
            // If parent doesn't belong to current module, try find first module category
            DataSet ds = SettingsCategoryInfoProvider.GetSettingsCategories("CategoryResourceID = " + ModuleID, "CategoryLevel, CategoryOrder", 1);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                category = new SettingsCategoryInfo(ds.Tables[0].Rows[0]);
            }
        }
        SelectPath = category.CategoryIDPath;
        SelectedItem = category.CategoryName;

        string scriptAction = String.Empty;
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                ExpandPath = category.CategoryIDPath + "/";
                // Update menu actions parameters
                Value = category.CategoryID + "|" + category.CategoryParentID;
                break;

            case "moveup":
                scriptAction = "window.tabIndex = " + tabIndex + ";";
                break;

            case "movedown":
                scriptAction = "window.tabIndex = " + tabIndex + ";";
                break;
        }

        RegisterSelectNodeScript(category);

        scriptAction += "var postParentId = " + category.CategoryParentID + ";";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "afterActionScript", ScriptHelper.GetScript(scriptAction));

        // Load data
        ReloadData();

    }


    protected void btnMoveUp_Click(object sender, EventArgs e)
    {
        GetHiddenValues();
        if (CategoryID > 0)
        {
            // Move category up
            SettingsCategoryInfoProvider.MoveCategoryUp(CategoryID);
            AfterAction("moveup", CategoryID, mTabIndex);
        }
    }


    protected void btnMoveDown_Click(object sender, EventArgs e)
    {
        GetHiddenValues();
        if (CategoryID > 0)
        {
            // Move category down
            SettingsCategoryInfoProvider.MoveCategoryDown(CategoryID);
            AfterAction("movedown", CategoryID, mTabIndex);
        }
    }


    protected void btnDeleteElem_Click(object sender, EventArgs e)
    {
        GetHiddenValues();
        if ((CategoryID > 0) && (ParentID > 0))
        {
            SettingsCategoryInfo.Provider.Get(CategoryID)?.Delete();
            AfterAction("delete", ParentID);
        }
    }

    #endregion
}