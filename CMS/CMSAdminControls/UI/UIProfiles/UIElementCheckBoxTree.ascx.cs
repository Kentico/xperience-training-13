using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UIProfiles_UIElementCheckBoxTree : CMSUserControl, ICallbackEventHandler
{
    #region "Variables"

    protected string mGroupPrefix;
    protected int mModuleID;
    protected int mRoleID;
    protected int mSiteID;
    private bool mEnabled = true;
    private string mSiteName;
    private string mCallbackRef;
    private UIElementInfo root;

    #endregion


    #region "Constants"

    private const string SELECT_DESELECT_LINKS = @"&nbsp;(<span class='link' onclick=""SelectAllSubelements($cmsj(this), {0}, {1}); {2}; return false;"">{3}</span>,&nbsp;<span class='link' onclick=""DeselectAllSubelements($cmsj(this), {0}, {1}); {2};return false;"" >{4}</span>)";

    #endregion


    #region "Properties"

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
    /// Indicates if all nodes should be collapsed.
    /// </summary>
    public bool CollapseAll
    {
        get
        {
            return treeElem.CollapseAll;
        }
        set
        {
            treeElem.CollapseAll = value;
        }
    }


    /// <summary>
    /// Gets or sets the prefix of the element name which should not have the checkbox.
    /// </summary>
    public string GroupPreffix
    {
        get
        {
            return mGroupPrefix;
        }
        set
        {
            mGroupPrefix = value;
        }
    }


    /// <summary>
    /// ID of the module.
    /// </summary>
    public int ModuleID
    {
        get
        {
            return mModuleID;
        }
        set
        {
            mModuleID = value;
        }
    }


    /// <summary>
    /// Indicates if also elements placed under current module's section which belongs to different module should be visible. Applies only if <see cref="SingleModule"/> is enabled.
    /// </summary>
    public bool ShowAllElementsFromModuleSection
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the role.
    /// </summary>
    public int RoleID
    {
        get
        {
            return mRoleID;
        }
        set
        {
            mRoleID = value;
        }
    }


    /// <summary>
    /// ID of the site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// If true, only current module is shown
    /// </summary>
    public bool SingleModule
    {
        get;
        set;
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
            treeElem.IsLiveSite = false;
        }
    }


    /// <summary>
    /// Indicates if checkboxes and select/deselect all should be enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Name of the site.
    /// </summary>
    private string SiteName
    {
        get
        {
            if (mSiteName == null)
            {
                SiteInfo si = SiteInfo.Provider.Get(SiteID);
                if (si != null)
                {
                    mSiteName = si.SiteName;
                }
            }
            return mSiteName;
        }
    }


    /// <summary>
    /// Callback reference for selecting items.
    /// </summary>
    private string CallbackRef
    {
        get
        {
            if (String.IsNullOrEmpty(mCallbackRef))
            {
                mCallbackRef = Page.ClientScript.GetCallbackEventReference(this, "hdnValue.value", "callbackHandler", "callbackHandler");
            }

            return mCallbackRef;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        treeElem.OnNodeCreated += treeElem_OnNodeCreated;
    }


    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Register scripts
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "UITreeCallbackHandler", ScriptHelper.GetScript(
            "var hdnValue = document.getElementById('" + hdnValue.ClientID + "'); function callbackHandler(content, context) {}"));

        // Use images according to culture
        treeElem.LineImagesFolder = GetImageUrl(CultureHelper.IsUICultureRTL() ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree");

        string noChkSelectString = GetString("uiprofile.selectallconfirmation");
        string noChkDeselectString = GetString("uiprofile.deselectallconfirmation");
        string selectString = GetString("uiprofile.selectallcurrentconfirmation");
        string deselectString = GetString("uiprofile.deselectallcurrentconfirmation");

        // Register scripts only if enabled
        if (Enabled)
        {
            string script = $@"
function SelectAllSubelements(elem, id, hasChkBox) {{
    if ((hasChkBox ? confirm('{selectString}') : confirm('{noChkSelectString}'))) {{
        hdnValue.value = 's;' + id + ';' + (hasChkBox? 1 : 0);
        var tab = elem.parents('table');
        tab.find('input:enabled[type=checkbox]').prop('checked', true);
        var node = tab.next();
        if ((node.length > 0)&&(node[0].nodeName.toLowerCase() == 'div')) {{
            node.find('input:enabled[type=checkbox]').prop('checked', true);
        }}
    }}
}}
function DeselectAllSubelements(elem, id, hasChkBox) {{
    if ((hasChkBox ? confirm('{deselectString}') : confirm('{noChkDeselectString}'))) {{
        hdnValue.value = 'd;' + id + ';' + (hasChkBox? 1 : 0);
        var tab = elem.parents('table');
        tab.find('input:enabled[type=checkbox]').prop('checked', false);
        var node = tab.next();
        if ((node.length > 0)&&(node[0].nodeName.toLowerCase() == 'div')) {{
            node.find('input:enabled[type=checkbox]').prop('checked', false);
        }}
    }}
}}";
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "UITreeSelectScripts", ScriptHelper.GetScript(script));
        }
    }


    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        // Get data
        if (itemData != null)
        {
            int id = ValidationHelper.GetInteger(itemData["ElementID"], 0);
            int childCount = ValidationHelper.GetInteger(itemData["ElementChildCount"], 0);
            bool selected = ValidationHelper.GetBoolean(itemData["ElementSelected"], false);
            string displayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ValidationHelper.GetString(itemData["ElementDisplayName"], string.Empty)));
            string elementName = ValidationHelper.GetString(itemData["ElementName"], string.Empty);
            int parentID = ValidationHelper.GetInteger(itemData["ElementParentID"], 0);
            int resourceID = ValidationHelper.GetInteger(itemData["ElementResourceID"], 0);
            string nodePath = ValidationHelper.GetString(itemData["ElementIDPath"], string.Empty);
            string itemClass = "ContentTreeItem";
            string onClickDeclaration = string.Format(" var chkElem_{0} = document.getElementById('chk_{0}'); ", id);
            string onClickCommon = string.Format("  hdnValue.value = {0} + ';' + chkElem_{0}.checked; {1};", id, CallbackRef);
            string onClickSpan = string.Format(" chkElem_{0}.checked = !chkElem_{0}.checked; ", id);

            // Expand for root
            if (parentID == 0)
            {
                defaultNode.Expanded = true;
            }
           
            if (!nodePath.EndsWith("/", StringComparison.Ordinal))
            {
                nodePath += "/";
            }

            bool chkEnabled = Enabled;
            if (!SingleModule && (ModuleID > 0))
            {
                bool isEndItem = false;
                bool isChild = false;
                bool isParent = false;

                var paths = treeElem.ExpandPath
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    // Add slash - select only children
                    .Select(path => path.EndsWith("/", StringComparison.Ordinal) ? path : path + "/");

                // Check expanded paths
                foreach (var path in paths)
                {
                    if (!isChild)
                    {
                        isChild = nodePath.StartsWith(path, StringComparison.InvariantCultureIgnoreCase);
                    }

                    // Module node is same node as specified in paths collection
                    isEndItem = path.Equals(nodePath, StringComparison.InvariantCultureIgnoreCase);

                    // Test for parent - expand
                    if (path.StartsWith(nodePath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        defaultNode.Expanded = true;
                        isParent = true;
                        break;
                    }
                }

                // Display for non selected module items
                if (resourceID != ModuleID)
                {
                    // Parent special css
                    if (isParent)
                    {
                        itemClass += " highlighted disabled";
                    }
                    else
                    {
                        // Disable non parent
                        chkEnabled = false;
                        itemClass += " disabled";
                    }
                }
                else if (isEndItem)
                {
                    // Special class for end module item
                    itemClass += " highlighted";
                }
            }

            // Get button links
            string links = null;

            string nodeText;
            if (!String.IsNullOrEmpty(GroupPreffix) && elementName.StartsWith(GroupPreffix, StringComparison.InvariantCultureIgnoreCase))
            {
                if (childCount > 0 && chkEnabled)
                {
                    links = string.Format(SELECT_DESELECT_LINKS, id, "false", CallbackRef, GetString("uiprofile.selectall"), GetString("uiprofile.deselectall"));
                }
                nodeText = $"<span class='{itemClass}'>{displayName}</span>{links}";
            }
            else
            {
                string warning = string.Empty;

                if ((SiteName != null) && !ResourceSiteInfoProvider.IsResourceOnSite(ApplicationUrlHelper.GetResourceName(resourceID), SiteName))
                {
                    warning = UIHelper.GetAccessibleIconTag("icon-exclamation-triangle",
                        String.Format(GetString("uiprofile.warningmodule"), "cms." + elementName.ToLowerInvariant()),
                        additionalClass: "color-orange-80");
                }

                if (childCount > 0 && chkEnabled)
                {
                    links = string.Format(SELECT_DESELECT_LINKS, id, "true", CallbackRef, GetString("uiprofile.selectall"), GetString("uiprofile.deselectall"));
                }

                nodeText = string.Format(@"<span class='checkbox tree-checkbox'><input type='checkbox' id='chk_{0}' name='chk_{0}'{1}{2} onclick=""{3}"" /><label for='chk_{0}'>&nbsp;</label><span class='{4}' onclick=""{5} return false;""><span class='Name'>{6}</span></span>{7}</span>{8}",
                                         id,
                                         chkEnabled ? string.Empty : " disabled='disabled'",
                                         selected ? " checked='checked'" : string.Empty,
                                         chkEnabled ? onClickDeclaration + onClickCommon : "return false;",
                                         itemClass,
                                         chkEnabled ? onClickDeclaration + onClickSpan + onClickCommon : "return false;",
                                         displayName,
                                         warning,
                                         links);
            }

            defaultNode.ToolTip = string.Empty;
            defaultNode.Text = nodeText;
        }

        return defaultNode;
    }


    /// <summary>
    /// Reloads the tree data.
    /// </summary>
    public void ReloadData()
    {
        var elementProvider = new UniTreeProvider
        {
            QueryName = "cms.uielement.selecttree",
            DisplayNameColumn = "ElementDisplayName",
            IDColumn = "ElementID",
            LevelColumn = "ElementLevel",
            OrderColumn = "ElementOrder",
            ParentIDColumn = "ElementParentID",
            PathColumn = "ElementIDPath",
            ValueColumn = "ElementID",
            ChildCountColumn = "ElementChildCount",
            ImageColumn = "ElementIconPath",
            Parameters = new QueryDataParameters { { "@RoleID", RoleID } },
            IconClassColumn = "ElementIconClass"
        };

        treeElem.ExpandTooltip = GetString("general.expand");
        treeElem.CollapseTooltip = GetString("general.collapse");
        treeElem.UsePostBack = false;
        treeElem.EnableRootAction = false;
        treeElem.ProviderObject = elementProvider;
        if (SingleModule)
        {
            ResourceInfo ri = ResourceInfo.Provider.Get(ModuleID);
            if (ri != null)
            {
                root = UIElementInfoProvider.GetModuleTopUIElement(ModuleID);
                if (root != null)
                {
                    treeElem.ExpandPath = root.ElementIDPath;
                    string links = null;
                    if (Enabled)
                    {
                        links = string.Format(SELECT_DESELECT_LINKS, root.ElementID, "false", CallbackRef, GetString("uiprofile.selectall"), GetString("uiprofile.deselectall"));
                    }
                    string rootText = HTMLHelper.HTMLEncode(ri.ResourceDisplayName) + links;
                    treeElem.SetRoot(rootText, root.ElementID.ToString(), ResolveUrl(root.ElementIconPath));
                    elementProvider.RootLevelOffset = root.ElementLevel;
                }

                if (!ShowAllElementsFromModuleSection)
                {
                    elementProvider.WhereCondition = "ElementResourceID = " + ModuleID;
                }
            }
        }
        else
        {
            if (ModuleID > 0)
            {
                var where = $@"ElementResourceID = {ModuleID} AND (ElementParentID IS NULL OR ElementParentID NOT IN (SELECT ElementID FROM CMS_UIElement WHERE ElementResourceID={ModuleID})) 
    AND (NOT EXISTS (SELECT  ElementIDPath FROM CMS_UIElement AS u WHERE CMS_UIElement.ElementIDPath LIKE u.ElementIDPath + '%' AND ElementResourceID = {ModuleID}
    AND u.ElementIDPath != CMS_UIElement.ElementIDPath))";

                var idPaths = UIElementInfo.Provider.Get()
                    .Where(where)
                    .WhereNotEmpty("ElementIDPath")
                    .Columns("ElementIDPath")
                    .OrderByAscending("ElementLevel")
                    .GetListResult<string>();

                var expandedPath = String.Empty;

                if (idPaths.Any())
                {
                    expandedPath = String.Join(";", idPaths) + ";";
                }

                treeElem.ExpandPath = expandedPath;
            }

        }

        treeElem.ReloadData();
    }


    /// <summary>
    /// Recursivelly select or deselect all child elements.
    /// </summary>
    /// <param name="select">Determines the type of action</param>
    /// <param name="parentId">ID of the parent UIElement</param>
    /// <param name="excludeRoot">Indicates whether to exclude root element from selection/deselection</param>
    private void SelectDeselectAll(bool select, int parentId, bool excludeRoot)
    {
        // Check manage permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY))
        {
            RedirectToAccessDenied("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY);
        }

        // Get the children and select them (do not use module as filter if all elements should be visible)
        string where = ((ModuleID > 0) && !ShowAllElementsFromModuleSection) ?
            String.Format(@"(ElementResourceID = {0} OR EXISTS (SELECT ElementID FROM CMS_UIElement AS x WHERE x.ElementIDPath LIKE CMS_UIElement.ElementIDPath+ '%' AND x.ElementResourceID = {0})) AND
                            ElementIDPath LIKE (SELECT TOP 1 ElementIDPath FROM CMS_UIElement WHERE ElementID = {1}) + '%' ", ModuleID, parentId) :
                           "ElementIDPath LIKE (SELECT TOP 1 ElementIDPath FROM CMS_UIElement WHERE ElementID = " + parentId + ") + '%' ";
        if (excludeRoot)
        {
            where += " AND NOT ElementID = " + parentId;
        }
        if (!String.IsNullOrEmpty(GroupPreffix))
        {
            where += " AND ElementName NOT LIKE '" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(GroupPreffix)) + "%'";
        }

        using (CMSActionContext context = new CMSActionContext())
        {
            // Many updates caused deadlocks with CMS_Role table, disable touch parent of the role
            context.TouchParent = false;

            var elementIds = UIElementInfo.Provider.Get()
                .Where(where)
                .Columns("ElementID")
                .GetListResult<int>();

            foreach (var id in elementIds)
            {
                if (select)
                {
                    RoleUIElementInfo.Provider.Add(RoleID, id);
                }
                else
                {
                    RoleUIElementInfo.Provider.Remove(RoleID, id);
                }
            }

            // Explicitly touch the role only once
            RoleInfo.Provider.Get(RoleID)
                ?.Update();
        }
    }


    #region "Callback handling"

    public string GetCallbackResult()
    {
        return string.Empty;
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        // Check manage permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.UIPersonalization", CMSAdminControl.PERMISSION_MODIFY))
        {
            return;
        }

        string[] test = eventArgument.Split(';');
        if ((test.Length == 2) || (test.Length == 3))
        {
            if (test.Length == 3)
            {
                bool excludeRoot = !ValidationHelper.GetBoolean(test[2], false);
                if (test[0] == "s")
                {
                    int id = ValidationHelper.GetInteger(test[1], 0);
                    SelectDeselectAll(true, id, excludeRoot);
                }
                else if (test[0] == "d")
                {
                    // Deselect all action
                    int id = ValidationHelper.GetInteger(test[1], 0);
                    SelectDeselectAll(false, id, excludeRoot);
                }
            }
            else if (test.Length == 2)
            {
                // Basic checkbox click
                int id = ValidationHelper.GetInteger(test[0], 0);
                bool chk = ValidationHelper.GetBoolean(test[1], false);

                if (chk)
                {
                    RoleUIElementInfo.Provider.Add(RoleID, id);
                }
                else
                {
                    RoleUIElementInfo.Provider.Remove(RoleID, id);
                }
            }

            // Invalidate all users
            UserInfo.TYPEINFO.InvalidateAllObjects();
        }
    }

    #endregion
}
