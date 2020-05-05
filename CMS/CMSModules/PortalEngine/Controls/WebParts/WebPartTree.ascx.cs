using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_WebParts_WebPartTree : CMSAdminControl
{
    #region "Variables"

    private int mMaxTreeNodes = -1;
    private bool mUseMaxNodeLimit = true;
    private bool mSelectWebParts;
    private bool mShowRecentlyUsed;
    private bool mUseGlobalSettings;

    /// <summary>
    /// Index used for item count under one node.
    /// </summary>
    private int indexMaxTreeNodes = -1;

    #endregion


    #region "Public properties"

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
    /// Indicates whether show multiple roots
    /// </summary>
    public bool MultipleRoots
    {
        get
        {
            return treeElem.MultipleRoots;
        }
        set
        {
            treeElem.MultipleRoots = value;
        }
    }


    /// <summary>
    /// Gets or sets expand path.
    /// </summary>
    public string ExpandPath
    {
        get
        {
            return treeElem.ExpandPath;
        }
        set
        {
            treeElem.ExpandPath = value;
        }
    }


    /// <summary>
    /// Indicates whether use max node limit stored in settings.
    /// </summary>
    public bool UseMaxNodeLimit
    {
        get
        {
            return mUseMaxNodeLimit;
        }
        set
        {
            mUseMaxNodeLimit = value;
        }
    }


    /// <summary>
    /// If true, only settings are used.
    /// </summary>
    public bool UseGlobalSettings
    {
        get
        {
            return mUseGlobalSettings;
        }
        set
        {
            mUseGlobalSettings = value;
        }
    }


    /// <summary>
    /// Maximum tree nodes shown under parent node - this value can be ignored if UseMaxNodeLimit set to false.
    /// </summary>
    public int MaxTreeNodes
    {
        get
        {
            if (mMaxTreeNodes < 0)
            {
                mMaxTreeNodes = SettingsKeyInfoProvider.GetIntValue((UseGlobalSettings ? "" : SiteContext.CurrentSiteName + ".") + "CMSMaxUITreeNodes");
            }
            return mMaxTreeNodes;
        }
        set
        {
            mMaxTreeNodes = value;
        }
    }

    /// <summary>
    /// Gets or sets whether webparts are shown in tree or not.
    /// </summary>
    public bool SelectWebParts
    {
        get
        {
            return mSelectWebParts;
        }
        set
        {
            mSelectWebParts = value;
        }
    }


    /// <summary>
    /// Gets or sets whether recently used link is shown or not.
    /// </summary>
    public bool ShowRecentlyUsed
    {
        get
        {
            return mShowRecentlyUsed;
        }
        set
        {
            mShowRecentlyUsed = value;
        }
    }


    /// <summary>
    /// Gets or sets selected item.
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return treeElem.SelectedItem;
        }
        set
        {
            treeElem.SelectedItem = value;
        }
    }


    /// <summary>
    /// Gets or sets if use postback.
    /// </summary>
    public bool UsePostBack
    {
        get
        {
            return treeElem.UsePostBack;
        }
        set
        {
            treeElem.UsePostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets select path.
    /// </summary>
    public string SelectPath
    {
        get
        {
            return treeElem.SelectPath;
        }
        set
        {
            treeElem.SelectPath = value;
            treeElem.ExpandPath = value;
        }
    }

    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            treeElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            treeElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Root path for the items in the tree.
    /// </summary>
    public virtual string RootPath
    {
        get;
        set;
    }

    #endregion


    #region "Custom events"

    /// <summary>
    /// On selected item event handler.
    /// </summary>    
    public delegate void ItemSelectedEventHandler(string selectedValue);

    /// <summary>
    /// On selected item event handler.
    /// </summary>
    public event ItemSelectedEventHandler OnItemSelected;

    #endregion


    #region "Page and other events"

    /// <summary>
    /// Page_Load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Create and set category provider
        UniTreeProvider categoryProvider = new UniTreeProvider();
        categoryProvider.DisplayNameColumn = "DisplayName";
        categoryProvider.IDColumn = "ObjectID";
        categoryProvider.LevelColumn = "ObjectLevel";
        categoryProvider.ParentIDColumn = "ParentID";
        categoryProvider.PathColumn = "ObjectPath";
        categoryProvider.ValueColumn = "ObjectID";
        categoryProvider.ChildCountColumn = "CompleteChildCount";
        categoryProvider.QueryName = "cms.webpartcategory.selectallview";
        categoryProvider.ObjectTypeColumn = "ObjectType";
        categoryProvider.Columns = "DisplayName, ObjectID, ObjectLevel,ParentID, ObjectPath, CompleteChildCount,ObjectType,CategoryChildCount, CategoryImagePath";
        categoryProvider.ImageColumn = "CategoryImagePath";
        categoryProvider.RootLevelOffset = 0;

        string where = null;

        if (!SelectWebParts)
        {
            // Select only categories
            where = "ObjectType = N'webpartcategory'";

            categoryProvider.ChildCountColumn = "CategoryChildCount";
            categoryProvider.ObjectTypeColumn = "";
        }
        else
        {
            categoryProvider.OrderBy = "ObjectType DESC, DisplayName ASC";
        }

        // Add custom where condition
        if (!string.IsNullOrEmpty(RootPath))
        {
            where = SqlHelper.AddWhereCondition(where, "ObjectPath = '" + SqlHelper.EscapeQuotes(RootPath) + "' OR ObjectPath LIKE '" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(RootPath)) + "/%'");
            categoryProvider.RootLevelOffset = RootPath.Split('/').Length - 1;

            treeElem.ExpandPath = RootPath.TrimEnd() + "/";
        }

        categoryProvider.WhereCondition = where;

        // Set up tree 
        treeElem.ProviderObject = categoryProvider;

        if (SelectWebParts)
        {
            treeElem.NodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##);\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.SelectedNodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##);\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        }
        else
        {
            treeElem.NodeTemplate = "<span onclick=\"SelectNode(##NODEID##, this);\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.DefaultItemTemplate = "<span onclick=\"SelectNode('recentlyused', this);\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.SelectedDefaultItemTemplate = "<span onclick=\"SelectNode('recentlyused', this);\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.SelectedNodeTemplate = "<span onclick=\"SelectNode(##NODEID##, this);\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

            ScriptHelper.RegisterJQuery(Page);

            string js = "var selectedItem = $cmsj('.ContentTreeSelectedItem');" +
                        "function SelectNode(nodeid, sender){" +
                        "selectedItem.removeClass('ContentTreeSelectedItem'); " +
                        "selectedItem.addClass('ContentTreeItem');" +
                        "selectedItem = $cmsj(sender);" +
                        "selectedItem.removeClass('ContentTreeItem'); " +
                        "selectedItem.addClass('ContentTreeSelectedItem'); " +
                        "document.getElementById('" + treeElem.SelectedItemFieldId + "').value = nodeid;" +
                        treeElem.GetOnSelectedItemBackEventReference() +
                        "}";

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "SelectTreeNode", ScriptHelper.GetScript(js));
        }

        // Add last recently used
        if (ShowRecentlyUsed)
        {
            treeElem.AddDefaultItem(GetString("webparts.recentlyused"), "recentlyused", null);
        }

        // Setup event handler
        treeElem.OnItemSelected += treeElem_OnItemSelected;
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    /// <summary>
    /// Page PreRender.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Load data
        if (!RequestHelper.IsPostBack())
        {
            treeElem.ReloadData();
        }
    }


    /// <summary>
    /// Used for maxnodes in collapsed node.
    /// </summary>
    /// <param name="itemData">The data row to check</param>
    /// <param name="defaultNode">The defaul node</param>
    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        if (UseMaxNodeLimit && (MaxTreeNodes > 0))
        {
            // Get parentID from data row
            int parentID = ValidationHelper.GetInteger(itemData["ParentID"], 0);
            string objectType = ValidationHelper.GetString(itemData["ObjectType"], String.Empty);

            // Don't use maxnodes limitation for categories
            if (objectType.ToLowerCSafe() == "webpartcategory")
            {
                return defaultNode;
            }

            // Increment index count in collapsing
            indexMaxTreeNodes++;
            if (indexMaxTreeNodes == MaxTreeNodes)
            {
                // Load parentid
                int parentParentID = 0;

                WebPartCategoryInfo category = WebPartCategoryInfoProvider.GetWebPartCategoryInfoById(parentID);
                if (category != null)
                {
                    parentParentID = category.CategoryParentID;
                }

                TreeNode node = new TreeNode();
                node.Text = "<span class=\"ContentTreeItem\" onclick=\"SelectNode(" + parentID + " ,'webpartcategory'," + parentParentID + ",true ); return false;\"><span class=\"Name\">" + GetString("general.seelisting") + "</span></span>";
                return node;
            }
            if (indexMaxTreeNodes > MaxTreeNodes)
            {
                return null;
            }
        }
        return defaultNode;
    }


    /// <summary>
    /// On selected item event.
    /// </summary>
    /// <param name="selectedValue">Selected value</param>
    protected void treeElem_OnItemSelected(string selectedValue)
    {
        if (OnItemSelected != null)
        {
            OnItemSelected(selectedValue);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the tree data.
    /// </summary>
    public override void ReloadData()
    {
        treeElem.ReloadData();
        base.ReloadData();
    }

    #endregion
}
