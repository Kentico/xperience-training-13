using System;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.MacroEngine;
using CMS.DocumentEngine;
using CMS.PortalEngine;

public partial class CMSWebParts_Navigation_cmstreeview : CMSAbstractWebPart
{
    #region "Variables"

    private MacroResolver mLocalResolver;

    #endregion

    #region "Document properties"

    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            treeView.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item dependencies.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return base.CacheDependencies;
        }
        set
        {
            base.CacheDependencies = value;
            treeView.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the cache item. If not explicitly specified, the name is automatically 
    /// created based on the control unique ID
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            treeView.CacheItemName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), treeView.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            treeView.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ClassNames"), treeView.ClassNames), treeView.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            treeView.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected documents are combined with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), treeView.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            treeView.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), treeView.CultureCode), treeView.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            treeView.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), treeView.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            treeView.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), treeView.OrderBy), treeView.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            treeView.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the nodes path.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), treeView.Path);
        }
        set
        {
            SetValue("Path", value);
            treeView.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected documents must be published.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), treeView.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            treeView.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), treeView.SiteName), treeView.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            treeView.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), treeView.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            treeView.WhereCondition = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether text can be wrapped or space is replaced with non breakable space.
    /// </summary>
    public bool WordWrap
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("WordWrap"), treeView.WordWrap);
        }
        set
        {
            SetValue("WordWrap", value);
            treeView.WordWrap = value;
        }
    }


    /// <summary>
    /// Gets or sets the CSS prefix.
    /// </summary>
    public string CSSPrefix
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CSSPrefix"), treeView.CSSPrefix), treeView.CSSPrefix);
        }
        set
        {
            SetValue("CSSPrefix", value);
            treeView.CSSPrefix = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), treeView.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            treeView.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows results.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), treeView.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            treeView.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), treeView.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            treeView.FilterName = value;
        }
    }

    #endregion


    #region "CMSTreeView properties"

    /// <summary>
    /// Gets or sets the value that indicates whether TreeView try fix broken lines.
    /// </summary>
    public bool FixBrokenLines
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FixBrokenLines"), treeView.FixBrokenLines);
        }
        set
        {
            SetValue("FixBrokenLines", value);
            treeView.FixBrokenLines = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether all nodes should be expanded on startup.
    /// </summary>
    public bool ExpandAllOnStartup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExpandAllOnStartup"), treeView.ExpandAllOnStartup);
        }
        set
        {
            SetValue("ExpandAllOnStartup", value);
            treeView.ExpandAllOnStartup = value;
        }
    }


    /// <summary>
    /// Gets or sets the root node text (by default is root text selected from document caption or document name).
    /// </summary>
    public string RootText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RootText"), treeView.RootText);
        }
        set
        {
            SetValue("RootText", value);
            treeView.RootText = value;
        }
    }


    /// <summary>
    ///  Gets or sets the URL to the image which will be displayed for root node.
    /// </summary>
    public string RootImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RootImageUrl"), treeView.RootImageUrl);
        }
        set
        {
            SetValue("RootImageUrl", value);
            treeView.RootImageUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the URL to the image which will be displayed fro all nodes.
    /// </summary>
    public string NodeImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NodeImageUrl"), treeView.NodeImageUrl);
        }
        set
        {
            SetValue("NodeImageUrl", value);
            treeView.NodeImageUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the JavaScript action which is proceeded on OnClick action.
    /// </summary>
    public string OnClickAction
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OnClickAction"), treeView.OnClickAction);
        }
        set
        {
            SetValue("OnClickAction", value);
            treeView.OnClickAction = ResolveOnClickMacros(value);
        }
    }


    /// <summary>
    /// Gets or sets the CSS style which is applied to the selected item.
    /// </summary>
    public string SelectedItemStyle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemStyle"), treeView.SelectedItemStyle);
        }
        set
        {
            SetValue("SelectedItemStyle", value);
            treeView.SelectedItemStyle = value;
        }
    }


    /// <summary>
    /// Gets or sets the CSS class which is applied to the selected item.
    /// </summary>
    public string SelectedItemClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemClass"), treeView.SelectedItemClass);
        }
        set
        {
            SetValue("SelectedItemClass", value);
            treeView.SelectedItemClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the CSS style for all items.
    /// </summary>
    public string ItemStyle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemStyle"), treeView.ItemStyle);
        }
        set
        {
            SetValue("ItemStyle", value);
            treeView.ItemStyle = value;
        }
    }


    /// <summary>
    /// Gets or sets the CSS class for all items.
    /// </summary>
    public string ItemClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemClass"), treeView.ItemClass);
        }
        set
        {
            SetValue("ItemClass", value);
            treeView.ItemClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether root node is not active (do not provide any action).
    /// </summary>
    public bool InactiveRoot
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("InactiveRoot"), treeView.InactiveRoot);
        }
        set
        {
            SetValue("InactiveRoot", value);
            treeView.InactiveRoot = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sub items are loaded dynamically (populate on demand).
    /// </summary>
    public bool DynamicBehavior
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DynamicBehavior"), treeView.DynamicBehavior);
        }
        set
        {
            SetValue("DynamicBehavior", value);
            treeView.DynamicBehavior = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether document type images will be displayed.
    /// </summary>
    public bool DisplayDocumentTypeImages
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayDocumentTypeImages"), treeView.DisplayDocumentTypeImages);
        }
        set
        {
            SetValue("DisplayDocumentTypeImages", value);
            treeView.DisplayDocumentTypeImages = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if tooltips with node name are shown.
    /// </summary>
    public bool ShowToolTips
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowToolTips"), treeView.ShowToolTips);
        }
        set
        {
            SetValue("ShowToolTips", value);
            treeView.ShowToolTips = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether node image isn't active (do not provide any action).
    /// </summary>
    public bool InactiveNodeImage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("InactiveNodeImage"), treeView.InactiveNodeImage);
        }
        set
        {
            SetValue("InactiveNodeImage", value);
            treeView.InactiveNodeImage = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether path to the current item will be expanded.
    /// </summary>
    public bool ExpandCurrentPath
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExpandCurrentPath"), treeView.ExpandCurrentPath);
        }
        set
        {
            SetValue("ExpandCurrentPath", value);
            treeView.ExpandCurrentPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected item will be inactivated (do not provide any action).
    /// </summary>
    public bool InactivateSelectedItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("InactivateSelectedItem"), treeView.InactivateSelectedItem);
        }
        set
        {
            SetValue("InactivateSelectedItem", value);
            treeView.InactivateSelectedItem = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether all items in path will be inactivated.
    /// </summary>
    public bool InactivateAllItemsInPath
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("InactivateAllItemsInPath"), treeView.InactivateAllItemsInPath);
        }
        set
        {
            SetValue("InactivateAllItemsInPath", value);
            treeView.InactivateAllItemsInPath = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected item is highlighted.
    /// </summary>
    public bool HiglightSelectedItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HiglightSelectedItem"), treeView.HiglightSelectedItem);
        }
        set
        {
            SetValue("HiglightSelectedItem", value);
            treeView.HiglightSelectedItem = value;
        }
    }


    /// <summary>
    /// Gets or sets property which indicates if menu caption should be HTML encoded.
    /// </summary>
    public bool EncodeMenuCaption
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EncodeMenuCaption"), treeView.EncodeMenuCaption);
        }
        set
        {
            SetValue("EncodeMenuCaption", value);
            treeView.EncodeMenuCaption = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to be retrieved from database.
    /// </summary>  
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), treeView.Columns);
        }
        set
        {
            SetValue("Columns", value);
            treeView.Columns = value;
        }
    }


    /// <summary>
    /// Local macro resolver instance inherited from CMSTreeView control.
    /// </summary>
    private MacroResolver LocalResolver
    {
        get
        {
            if (mLocalResolver == null)
            {
                mLocalResolver = ContextResolver.CreateChild();
                mLocalResolver.Settings.KeepUnresolvedMacros = true;
                mLocalResolver.Settings.AvoidInjection = false;
            }

            return mLocalResolver;
        }
    }

    #endregion


    #region "CMSTreeView base properties"

    /// <summary>
    /// Gets or sets the URL to a custom image for the collapsible node indicator.
    /// </summary>
    public string CollapseImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CollapseImageUrl"), treeView.CollapseImageUrl);
        }
        set
        {
            SetValue("CollapseImageUrl", value);
            treeView.CollapseImageUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the URL to a custom image for the expandable node indicator.
    /// </summary>
    public string ExpandImageUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExpandImageUrl"), treeView.ExpandImageUrl);
        }
        set
        {
            SetValue("ExpandImageUrl", value);
            treeView.ExpandImageUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the target window or frame in which to display the Web page content that is associated with a node.
    /// </summary>
    public string Target
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Target"), treeView.Target);
        }
        set
        {
            SetValue("Target", value);
            treeView.Target = value;
        }
    }


    /// <summary>
    /// Gets or sets the CSS style which is applied to the inactive item.
    /// </summary>
    public string InactiveItemStyle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InactiveItemStyle"), treeView.InactiveItemStyle);
        }
        set
        {
            SetValue("InactiveItemStyle", value);
            treeView.InactiveItemStyle = value;
        }
    }


    /// <summary>
    /// Gets or sets the CSS class which is applied to the inactive item.
    /// </summary>
    public string InactiveItemClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InactiveItemClass"), treeView.InactiveItemClass);
        }
        set
        {
            SetValue("InactiveItemClass", value);
            treeView.InactiveItemClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether document menu action will be ignored.
    /// </summary>
    public bool IgnoreDocumentMenuAction
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IgnoreDocumentMenuAction"), treeView.IgnoreDocumentMenuAction);
        }
        set
        {
            SetValue("IgnoreDocumentMenuAction", value);
            treeView.IgnoreDocumentMenuAction = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether root node is hidden.
    /// </summary>
    public bool HideRootNode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideRootNode"), treeView.HideRootNode);
        }
        set
        {
            SetValue("HideRootNode", value);
            treeView.HideRootNode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sub tree under current item is expanded.
    /// </summary>
    public bool ExpandSubTree
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ExpandSubTree"), treeView.ExpandSubTree);
        }
        set
        {
            SetValue("ExpandSubTree", value);
            treeView.ExpandSubTree = value;
        }
    }


    /// <summary>
    /// Gets or sets the SkinID which should be used.
    /// </summary>
    public override string SkinID
    {
        get
        {
            return base.SkinID;
        }
        set
        {
            base.SkinID = value;

            // Set SkinID
            if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (ValidationHelper.GetString(Page.StyleSheetTheme, "") == ""))
            {
                treeView.SkinID = SkinID;
            }
        }
    }


    /// <summary>
    /// Gets or sets the value that indicating whether lines connecting  child nodes to parent nodes are displayed.
    /// </summary>
    public bool ShowLines
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowLines"), true);
        }
        set
        {
            SetValue("ShowLines", value);
            treeView.ShowLines = value;
        }
    }


    /// <summary>
    /// Gets or sets ToolTip for the image that is displayed for the expandable node indicator.
    /// </summary>
    public string ExpandImageToolTip
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExpandImageToolTip"), treeView.ExpandImageToolTip);
        }
        set
        {
            SetValue("ExpandImageToolTip", value);
            treeView.ExpandImageToolTip = value;
        }
    }


    /// <summary>
    /// Gets or sets ToolTip for the image that is displayed for the collapsible node indicator.
    /// </summary>
    public string CollapseImageToolTip
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CollapseImageToolTip"), treeView.CollapseImageToolTip);
        }
        set
        {
            SetValue("CollapseImageToolTip", value);
            treeView.CollapseImageToolTip = value;
        }
    }


    /// <summary>
    /// Gets or sets the path to a folder that contains the line images that are used to connect child nodes to parent nodes.
    /// </summary>
    public string LineImagesFolder
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LineImagesFolder"), treeView.LineImagesFolder);
        }
        set
        {
            if (!CultureHelper.IsCultureRTL(DocumentContext.CurrentDocumentCulture.CultureCode))
            {
                SetValue("LineImagesFolder", value);
                treeView.LineImagesFolder = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the path to a folder that contains the line images that are used to connect child nodes to parent nodes when the current culture is a RTL culture.
    /// </summary>
    public string RTLLineImagesFolder
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RTLLineImagesFolder"), treeView.LineImagesFolder);
        }
        set
        {
            if (CultureHelper.IsCultureRTL(DocumentContext.CurrentDocumentCulture.CultureCode))
            {
                SetValue("RTLLineImagesFolder", value);
                treeView.LineImagesFolder = value;
            }
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            treeView.StopProcessing = true;
        }
        else
        {
            // Do not resolve macros in OnClickAction field
            NotResolveProperties = "onclickaction";

            treeView.ControlContext = ControlContext;

            // Set properties from web part form        
            treeView.FixBrokenLines = FixBrokenLines;
            treeView.CacheItemName = CacheItemName;
            treeView.CacheDependencies = CacheDependencies;
            treeView.CacheMinutes = CacheMinutes;
            treeView.CheckPermissions = CheckPermissions;
            treeView.ClassNames = ClassNames;
            treeView.CombineWithDefaultCulture = CombineWithDefaultCulture;
            treeView.CSSPrefix = CSSPrefix;
            treeView.CultureCode = CultureCode;
            treeView.MaxRelativeLevel = MaxRelativeLevel;
            treeView.OrderBy = OrderBy;
            treeView.Path = Path;
            treeView.SelectOnlyPublished = SelectOnlyPublished;
            treeView.SiteName = SiteName;
            treeView.WhereCondition = WhereCondition;
            treeView.EncodeMenuCaption = EncodeMenuCaption;
            treeView.Columns = Columns;

            // Set images for collapse and expand
            treeView.CollapseImageUrl = CollapseImageUrl;
            treeView.ExpandImageUrl = ExpandImageUrl;

            // Set tooltip for collapse and expand
            treeView.ExpandImageToolTip = ExpandImageToolTip;
            treeView.CollapseImageToolTip = CollapseImageToolTip;

            // Set correct line direction according to culture
            if (CultureHelper.IsCultureRTL(DocumentContext.CurrentDocumentCulture.CultureCode))
            {
                treeView.LineImagesFolder = RTLLineImagesFolder;
            }
            else
            {
                treeView.LineImagesFolder = LineImagesFolder;
            }

            treeView.ShowLines = ShowLines;

            // Hide lines if collapse and expand image aren't specified
            if (!String.IsNullOrEmpty(treeView.CollapseImageUrl) || !String.IsNullOrEmpty(treeView.ExpandImageUrl))
            {
                treeView.ShowLines = false;
            }

            treeView.WordWrap = WordWrap;
            treeView.ExpandAllOnStartup = ExpandAllOnStartup;

            // Set other properties
            treeView.RootText = RootText;
            treeView.RootImageUrl = RootImageUrl;
            treeView.NodeImageUrl = NodeImageUrl;

            treeView.OnClickAction = ResolveOnClickMacros(OnClickAction);
            treeView.SelectedItemStyle = SelectedItemStyle;
            treeView.SelectedItemClass = SelectedItemClass;
            treeView.ItemStyle = ItemStyle;
            treeView.ItemClass = ItemClass;

            treeView.InactiveRoot = InactiveRoot;
            treeView.DynamicBehavior = DynamicBehavior;
            treeView.DisplayDocumentTypeImages = DisplayDocumentTypeImages;
            treeView.ShowToolTips = ShowToolTips;
            treeView.InactiveNodeImage = InactiveNodeImage;
            treeView.ExpandCurrentPath = ExpandCurrentPath;
            treeView.InactivateSelectedItem = InactivateSelectedItem;
            treeView.InactivateAllItemsInPath = InactivateAllItemsInPath;
            treeView.HiglightSelectedItem = HiglightSelectedItem;
            treeView.IgnoreDocumentMenuAction = IgnoreDocumentMenuAction;

            treeView.Target = Target;
            treeView.InactiveItemStyle = InactiveItemStyle;
            treeView.InactiveItemClass = InactiveItemClass;

            treeView.ExpandSubTree = ExpandSubTree;
            treeView.HideRootNode = HideRootNode;

            // Set visibility for no records state
            treeView.HideControlForZeroRows = HideControlForZeroRows;
            treeView.ZeroRowsText = ZeroRowsText;

            // Set filter
            treeView.FilterName = FilterName;
        }
    }


    /// <summary>
    /// Resolves the macros within current WebPart context, with special handling for onclickaction field.
    /// </summary>
    /// <param name="inputText">Input text to resolve</param>
    public string ResolveOnClickMacros(string inputText)
    {
        // Special "macro" with two '%' will be resolved later
        if (!String.IsNullOrEmpty(inputText) && !inputText.Contains("%%"))
        {
            // Resolve macros
            return LocalResolver.ResolveMacros(inputText);
        }
        return inputText;
    }


    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        treeView.SkinID = SkinID;
        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        // Reload data
        SetupControl();
        treeView.ReloadData(true);
        base.ReloadData();
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set visibility
        Visible = treeView.Visible;

        if (DataHelper.DataSourceIsEmpty(treeView.DataSource) && (treeView.HideControlForZeroRows))
        {
            Visible = false;
        }
    }
}