using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = System.Web.UI.WebControls.TreeNode;


public partial class CMSModules_PortalEngine_Controls_Layout_PageTemplateTree : CMSAdminControl
{
    #region "Variables"

    private int mMaxTreeNodes = -1;
    private bool mUseMaxNodeLimit = true;
    private bool mSelectPageTemplates;
    private bool mShowEmptyCategories = true;
    private int mDocumentID;
    private bool mIsNewPage;
    private bool mUseGlobalSettings;
    private string mSelectFunctionName = "SelectNode";

    /// <summary>
    /// Index used for item count under one node.
    /// </summary>
    private int indexMaxTreeNodes = -1;

    #endregion


    #region "Public properties"

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
    /// If true, only global settings are used.
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
    /// Gets or sets whether page templates are shown in tree or not.
    /// </summary>
    public bool SelectPageTemplates
    {
        get
        {
            return mSelectPageTemplates;
        }
        set
        {
            mSelectPageTemplates = value;
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
    /// Root path
    /// </summary>
    public string RootPath
    {
        get;
        set;
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
    /// Shows or hides empty categories in tree.
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
    /// Gets or sets document id.
    /// </summary>
    public int DocumentID
    {
        get
        {
            return mDocumentID;
        }
        set
        {
            mDocumentID = value;
        }
    }


    /// <summary>
    /// Whether selecting new page.
    /// </summary>
    public bool IsNewPage
    {
        get
        {
            return mIsNewPage;
        }
        set
        {
            mIsNewPage = value;
        }
    }


    /// <summary>
    /// Name of the JS function for the selection
    /// </summary>
    public string SelectFunctionName
    {
        get
        {
            return mSelectFunctionName;
        }
        set
        {
            mSelectFunctionName = value;
        }
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

        InitCategoryProvider();

        treeElem.MultipleRoots = false;

        if (SelectPageTemplates)
        {
            treeElem.NodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"" + SelectFunctionName + "(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##, '##PARAMETER##');\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.SelectedNodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"" + SelectFunctionName + "(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##, '##PARAMETER##');\" name=\"treeNode\"  class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        }
        else
        {
            treeElem.NodeTemplate = "<span onclick=\"" + SelectFunctionName + "(##NODEID##, this);\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.DefaultItemTemplate = "<span onclick=\"" + SelectFunctionName + "('recentlyused', this);\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span><div style=\"clear:both\"></div>";
            treeElem.SelectedDefaultItemTemplate = "<span onclick=\"" + SelectFunctionName + "('recentlyused', this);\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span><div style=\"clear:both\"></div>";
            treeElem.SelectedNodeTemplate = "<span onclick=\"" + SelectFunctionName + "(##NODEID##, this);\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

            ScriptHelper.RegisterJQuery(Page);

            string js =
@"
function " + SelectFunctionName + @"(nodeid, sender) { 
    var si = $cmsj('.ContentTreeSelectedItem');
    si.removeClass('ContentTreeSelectedItem');
    si.addClass('ContentTreeItem');
    si = $cmsj(sender);
    si.removeClass('ContentTreeItem');
    si.addClass('ContentTreeSelectedItem');
    document.getElementById('" + treeElem.SelectedItemFieldId + "').value = nodeid;" +
    treeElem.GetOnSelectedItemBackEventReference() +
"}";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SelectTreeNode", ScriptHelper.GetScript(js));
        }

        // Setup event handler
        treeElem.OnItemSelected += treeElem_OnItemSelected;
        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    /// <summary>
    /// Initializes the category provider
    /// </summary>
    private void InitCategoryProvider()
    {
        // Create and set category provider
        UniTreeProvider categoryProvider = new UniTreeProvider();

        categoryProvider.DisplayNameColumn = "DisplayName";
        categoryProvider.IDColumn = "ObjectID";
        categoryProvider.LevelColumn = "ObjectLevel";
        categoryProvider.ParentIDColumn = "ParentID";
        categoryProvider.PathColumn = "ObjectPath";
        categoryProvider.ValueColumn = "ObjectID";
        categoryProvider.ChildCountColumn = "CompleteChildCount";
        categoryProvider.QueryName = "cms.pagetemplatecategory.selectallview";
        categoryProvider.ObjectTypeColumn = "ObjectType";
        categoryProvider.Columns = "DisplayName, CodeName, ObjectID, ObjectLevel, ParentID, ObjectPath, CompleteChildCount, ObjectType, CategoryChildCount, CategoryImagePath, null as Parameter";
        categoryProvider.ImageColumn = "CategoryImagePath";
        categoryProvider.ParameterColumn = "Parameter";

        if (!SelectPageTemplates)
        {
            categoryProvider.WhereCondition = "ObjectType = 'pagetemplatecategory'";
            categoryProvider.ChildCountColumn = "CategoryChildCount";
            categoryProvider.ObjectTypeColumn = "";
        }
        else
        {
            categoryProvider.OrderBy = "ObjectType DESC, DisplayName ASC";
        }

        // Do not show empty categories
        if (!ShowEmptyCategories)
        {
            categoryProvider.WhereCondition = SqlHelper.AddWhereCondition(categoryProvider.WhereCondition, "CategoryTemplateChildCount > 0 OR CategoryChildCount > 0");

            TreeProvider tp = new TreeProvider(MembershipContext.AuthenticatedUser);
            CMS.DocumentEngine.TreeNode node = DocumentHelper.GetDocument(DocumentID, tp);
            string culture = LocalizationContext.PreferredCultureCode;
            int level = 0;
            string path = string.Empty;

            string className = String.Empty;

            if (node != null)
            {
                level = node.NodeLevel;
                path = node.NodeAliasPath;
                if (IsNewPage)
                {
                    level++;
                    path = path + "/%";
                }
                else
                {
                    culture = node.DocumentCulture;
                }

                className = node.NodeClassName;

                // Check if class id is in query string - then use it's value instead of document class name 
                int classID = QueryHelper.GetInteger("classid", 0);
                if (classID != 0)
                {
                    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(classID);
                    if (dci != null)
                    {
                        className = dci.ClassName;
                    }
                }
            }

            // Add where condition for scopes
            categoryProvider.WhereCondition += " AND (ObjectLevel = 0 OR (SELECT TOP 1 ObjectID FROM View_CMS_PageTemplateCategoryPageTemplate_Joined AS X WHERE X.ObjectType = 'pagetemplate' ";

            categoryProvider.WhereCondition += " AND (X.PageTemplateType IS NULL OR X.PageTemplateType <> N'" + PageTemplateInfoProvider.GetPageTemplateTypeCode(PageTemplateTypeEnum.Dashboard) + "')";

            categoryProvider.WhereCondition += " AND (X.ObjectPath LIKE View_CMS_PageTemplateCategoryPageTemplate_Joined.ObjectPath + '/%')) IS NOT NULL)";

            // Add column count column - minimal number of children
            categoryProvider.Columns += @", (SELECT TOP 1 Count(*) FROM View_CMS_PageTemplateCategoryPageTemplate_Joined AS Y WHERE 
            (Y.ObjectID = View_CMS_PageTemplateCategoryPageTemplate_Joined.ObjectID AND Y.ObjectLevel = 0)
            OR ( View_CMS_PageTemplateCategoryPageTemplate_Joined.ObjectType = 'PageTemplateCategory' 
            AND View_CMS_PageTemplateCategoryPageTemplate_Joined.CategoryChildCount > 0 
            AND Y.ObjectType = 'PageTemplate' AND Y.ObjectLevel > View_CMS_PageTemplateCategoryPageTemplate_Joined.ObjectLevel + 1 ";

            categoryProvider.Columns += " AND Y.ObjectPath LIKE  View_CMS_PageTemplateCategoryPageTemplate_Joined.ObjectPath + '/%')) AS MinNumberOfChilds";
            categoryProvider.ChildCountColumn = "MinNumberOfChilds";
        }

        // Handle the root path
        if (!String.IsNullOrEmpty(RootPath))
        {
            categoryProvider.RootLevelOffset = RootPath.TrimEnd('/').Split('/').Length - 1;

            categoryProvider.WhereCondition = SqlHelper.AddWhereCondition(categoryProvider.WhereCondition, String.Format("((ObjectPath = '{0}' OR ObjectPath LIKE '{0}/%'))", RootPath.TrimEnd('/')));
        }

        treeElem.ProviderObject = categoryProvider;
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
    /// <param name="itemData">The item data</param>
    /// <param name="defaultNode">The default node</param>
    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        if (UseMaxNodeLimit && (MaxTreeNodes > 0))
        {
            // Get parentID from data row
            int parentID = ValidationHelper.GetInteger(itemData["ParentID"], 0);
            string objectType = ValidationHelper.GetString(itemData["ObjectType"], String.Empty);

            // Don't use maxnodes limitation for categories
            if (objectType.ToLowerCSafe() == "pagetemplatecategory")
            {
                return defaultNode;
            }

            // Increment index count in collapsing
            indexMaxTreeNodes++;
            if (indexMaxTreeNodes == MaxTreeNodes)
            {
                // Load parentid
                int parentParentID = 0;
                PageTemplateCategoryInfo parentParent = PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo(parentID);
                if (parentParent != null)
                {
                    parentParentID = parentParent.ParentId;
                }

                TreeNode node = new TreeNode();
                node.Text = "<span class=\"ContentTreeItem\" onclick=\"" + SelectFunctionName + "(" + parentID + " ,'pagetemplatecategory'," + parentParentID + ",true ); return false;\"><span class=\"Name\">" + GetString("general.seelisting") + "</span></span>";
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
    ///  On selected item event.
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
        InitCategoryProvider();

        treeElem.ReloadData();

        base.ReloadData();
    }

    #endregion
}
