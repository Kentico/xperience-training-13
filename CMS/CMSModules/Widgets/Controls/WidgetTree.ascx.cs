using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Widgets_Controls_WidgetTree : CMSAdminControl
{
    #region "Variables"

    private int mMaxTreeNodes = -1;

    /// <summary>
    /// Index used for item count under one node.
    /// </summary>
    private int indexMaxTreeNodes = -1;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether use max node limit stored in settings.
    /// </summary>
    public bool UseMaxNodeLimit { get; set; } = true;


    /// <summary>
    /// If true, only global settings are used.
    /// </summary>
    public bool UseGlobalSettings { get; set; } = false;


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
    /// Gets or sets whether widgets are shown in tree or not.
    /// </summary>
    public bool SelectWidgets { get; set; } = true;


    /// <summary>
    /// Gets or sets whether recently used link is shown or not.
    /// </summary>
    public bool ShowRecentlyUsed { get; set; } = false;


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
    /// Enables or disables use of postback in tree. If disabled JavaScript is used.
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
        categoryProvider.OrderColumn = "ObjectType DESC";
        categoryProvider.ParentIDColumn = "ParentID";
        categoryProvider.PathColumn = "ObjectPath";
        categoryProvider.ValueColumn = "ObjectID";
        categoryProvider.ChildCountColumn = "CompleteChildCount";
        categoryProvider.QueryName = "cms.widgetcategory.selectallview";
        categoryProvider.ObjectTypeColumn = "ObjectType";
        categoryProvider.ImageColumn = "WidgetCategoryImagePath";
        categoryProvider.Columns = "ObjectID, DisplayName, ParentID, WidgetCategoryImagePath, ObjectPath, WidgetCategoryChildCount, ObjectLevel, CompleteChildCount, ObjectType";


        // Build where condition       
        var currentUser = MembershipContext.AuthenticatedUser;
        string securityWhere = String.Empty;


        // Security where condition
        if (!currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            securityWhere = "(WidgetSecurity = 0)"; // Allowed for all        
            if (AuthenticationHelper.IsAuthenticated())
            {
                securityWhere += " OR (WidgetSecurity = 1)"; // Authenticated
                if (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
                {
                    securityWhere += " OR (WidgetSecurity = 7)"; // Global admin
                }
                securityWhere += " OR ((WidgetSecurity = 2) AND (ObjectID IN ( SELECT WidgetID FROM CMS_WidgetRole WHERE RoleID IN (SELECT RoleID FROM View_CMS_UserRole_MembershipRole_ValidOnly_Joined WHERE UserID = " + currentUser.UserID + "))))"; // Authorized roles
            }
        }

        //  Categories AND widgets
        if (SelectWidgets)
        {
            categoryProvider.WhereCondition = securityWhere;
        }
        else // Only Categories
        {
            categoryProvider.WhereCondition = "ObjectType = 'widgetcategory'";
            categoryProvider.ChildCountColumn = "WidgetCategoryChildCount";
            categoryProvider.ObjectTypeColumn = "";

            // Create WHERE condition which filters categories without widgets
            string where = "";
            where = @"0 <>(SELECT TOP 1 ObjectID FROM 
            (SELECT ObjectPath AS WidgetPath, ObjectType, WidgetSecurity, ObjectID
            FROM View_CMS_WidgetCategoryWidget_Joined) AS SUB 
            WHERE SUB.ObjectType = 'widget' AND SUB.WidgetPath LIKE ObjectPath + '%'";

            if (!string.IsNullOrEmpty(securityWhere))
            {
                where += " AND ( " + securityWhere + ")";
            }

            categoryProvider.WhereCondition = SqlHelper.AddWhereCondition(categoryProvider.WhereCondition, where + ")");
        }

        // Set up tree 
        treeElem.ProviderObject = categoryProvider;

        if (SelectWidgets)
        {
            string script = "onclick=\"SelectNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##);return false;\"";
            treeElem.NodeTemplate = String.Format("<span id=\"##OBJECTTYPE##_##NODEID##\" {0} name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>", script);
            treeElem.SelectedNodeTemplate = String.Format("<span id=\"##OBJECTTYPE##_##NODEID##\" {0} name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>", script);
        }
        else
        {
            treeElem.NodeTemplate = "<span onclick=\"SelectNode(##NODEID##, this);return false;\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.DefaultItemTemplate = "<span onclick=\"SelectNode('recentlyused', this);return false;\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.SelectedDefaultItemTemplate = "<span onclick=\"SelectNode('recentlyused', this);return false;\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            treeElem.SelectedNodeTemplate = "<span onclick=\"SelectNode(##NODEID##, this);return false;\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";

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
            treeElem.AddDefaultItem(GetString("widgets.recentlyused"), "recentlyused", null);
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
    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        if (UseMaxNodeLimit && (MaxTreeNodes > 0))
        {
            // Get parentID from data row
            int parentID = ValidationHelper.GetInteger(itemData["ParentID"], 0);
            string objectType = ValidationHelper.GetString(itemData["ObjectType"], String.Empty);

            // Don't use maxnodes limitation for categories
            if (objectType.ToLowerCSafe() == "widgetcategory")
            {
                return defaultNode;
            }

            // Increment index count in collapsing
            indexMaxTreeNodes++;
            if (indexMaxTreeNodes == MaxTreeNodes)
            {
                // Load parentid
                int parentParentID = 0;

                WidgetCategoryInfo parentParent = WidgetCategoryInfoProvider.GetWidgetCategoryInfo(parentID);
                if (parentParent != null)
                {
                    parentParentID = parentParent.WidgetCategoryParentID;
                }

                TreeNode node = new TreeNode();
                node.Text = "<span class=\"ContentTreeItem\" onclick=\"SelectNode(" + parentID + " ,'widgetcategory'," + parentParentID + ",true ); return false;\"><span class=\"Name\">" + GetString("general.seelisting") + "</span></span>";
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
        OnItemSelected?.Invoke(selectedValue);
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
