using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Navigation_cmstabcontrol : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether text can be wrapped or space is replaced with non breakable space.
    /// </summary>
    public bool WordWrap
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("WordWrap"), tabElem.WordWrap);
        }
        set
        {
            SetValue("WordWrap", value);
            tabElem.WordWrap = value;
        }
    }


    /// <summary>
    /// Gets or sets the TabControl id prefix.
    /// </summary>
    public string TabControlIdPrefix
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TabControlIdPrefix"), tabElem.TabControlIdPrefix);
        }
        set
        {
            SetValue("TabControlIdPrefix", value);
            tabElem.TabControlIdPrefix = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether image alternate text is rendered.
    /// </summary>
    public bool RenderImageAlt
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderImageAlt"), tabElem.RenderImageAlt);
        }
        set
        {
            SetValue("RenderImageAlt", value);
            tabElem.RenderImageAlt = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether document menu item properties are applied.
    /// </summary>
    public bool ApplyMenuDesign
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ApplyMenuDesign"), tabElem.ApplyMenuDesign);
        }
        set
        {
            SetValue("ApplyMenuDesign", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether item image is displayed for highlighted item when highlighted image is not specified.
    /// </summary>
    public bool UseItemImagesForHiglightedItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseItemImagesForHiglightedItem"), tabElem.UseItemImagesForHighlightedItem);
        }
        set
        {
            SetValue("UseItemImagesForHiglightedItem", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), tabElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            tabElem.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), tabElem.ClassNames), tabElem.ClassNames);
        }
        set
        {
            SetValue("Classnames", value);
            tabElem.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected documents are combined with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), tabElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            tabElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the first item is selected by default if isn't other item selected.
    /// </summary>
    public bool SelectFirstItemByDefault
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectFirstItemByDefault"), tabElem.SelectFirstItemByDefault);
        }
        set
        {
            SetValue("SelectFirstItemByDefault", value);
            tabElem.SelectFirstItemByDefault = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents which should be displayed.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), tabElem.CultureCode), tabElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            tabElem.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents which should be displayed.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), tabElem.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            tabElem.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the highlighted node path.
    /// </summary>
    public string HighlightedNodePath
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("HighlightedNodePath"), tabElem.HighlightedNodePath), tabElem.HighlightedNodePath);
        }
        set
        {
            SetValue("HighlightedNodePath", value);
            tabElem.HighlightedNodePath = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by expression.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), tabElem.OrderBy), tabElem.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            tabElem.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the nodes path.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), tabElem.Path);
        }
        set
        {
            SetValue("Path", value);
            tabElem.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether selected documents must be published.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), tabElem.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            tabElem.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), tabElem.SiteName), tabElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            tabElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether alternating styles are used.
    /// </summary>
    public bool UseAlternatingStyles
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseAlternatingStyles"), tabElem.UseAlternatingStyles);
        }
        set
        {
            SetValue("UseAlternatingStyles", value);
            tabElem.UseAlternatingStyles = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("WhereCondition"), tabElem.WhereCondition), tabElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            tabElem.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the client script is used after selection.
    /// </summary>
    public bool UseClientScript
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseClientScript"), tabElem.UseClientScript);
        }
        set
        {
            SetValue("UseClientScript", value);
            tabElem.UseClientScript = value;
        }
    }


    /// <summary>
    /// Gets or sets the layout of the tab control (horizontal or vertical).
    /// </summary>
    public TabControlLayoutEnum TabControlLayout
    {
        get
        {
            return tabElem.GetTabControlLayout(ValidationHelper.GetString(GetValue("TabControlLayout"), tabElem.TabControlLayout.ToString()));
        }
        set
        {
            SetValue("TabControlLayout", value);
            tabElem.TabControlLayout = value;
        }
    }


    /// <summary>
    /// Gets or sets the link target url.
    /// </summary>
    public string UrlTarget
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("UrlTarget"), tabElem.UrlTarget);
        }
        set
        {
            SetValue("UrlTarget", value);
            tabElem.UrlTarget = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether link title will be rendered.
    /// </summary>
    public bool RenderLinkTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderLinkTitle"), tabElem.RenderLinkTitle);
        }
        set
        {
            SetValue("RenderLinkTitle", value);
            tabElem.RenderLinkTitle = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), tabElem.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            tabElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows results.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), tabElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            tabElem.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), tabElem.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            tabElem.FilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets property which indicates if menu caption should be HTML encoded.
    /// </summary>
    public bool EncodeMenuCaption
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EncodeMenuCaption"), tabElem.EncodeMenuCaption);
        }
        set
        {
            SetValue("EncodeMenuCaption", value);
            tabElem.EncodeMenuCaption = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to be retrieved from database.
    /// </summary>  
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), tabElem.Columns);
        }
        set
        {
            SetValue("Columns", value);
            tabElem.Columns = value;
        }
    }


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
            tabElem.CacheMinutes = value;
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
            tabElem.CacheDependencies = value;
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
            tabElem.CacheItemName = value;
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
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            tabElem.StopProcessing = true;
        }
        else
        {
            tabElem.ControlContext = ControlContext;

            // Set properties from Webpart form        
            tabElem.SelectFirstItemByDefault = SelectFirstItemByDefault;
            tabElem.ApplyMenuDesign = ApplyMenuDesign;
            tabElem.UseItemImagesForHighlightedItem = UseItemImagesForHiglightedItem;
            tabElem.CacheItemName = CacheItemName;
            tabElem.CacheDependencies = CacheDependencies;
            tabElem.CacheMinutes = CacheMinutes;
            tabElem.CheckPermissions = CheckPermissions;
            tabElem.ClassNames = ClassNames;
            tabElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            tabElem.CultureCode = CultureCode;
            tabElem.MaxRelativeLevel = MaxRelativeLevel;
            tabElem.HighlightedNodePath = HighlightedNodePath;
            tabElem.OrderBy = OrderBy;
            tabElem.Path = Path;
            tabElem.SelectOnlyPublished = SelectOnlyPublished;
            tabElem.SiteName = SiteName;
            tabElem.UseAlternatingStyles = UseAlternatingStyles;
            tabElem.WhereCondition = WhereCondition;

            tabElem.UseClientScript = UseClientScript;
            tabElem.TabControlLayout = TabControlLayout;
            tabElem.UrlTarget = UrlTarget;
            tabElem.RenderImageAlt = RenderImageAlt;
            tabElem.RenderLinkTitle = RenderLinkTitle;
            tabElem.TabControlIdPrefix = TabControlIdPrefix;
            tabElem.WordWrap = WordWrap;

            tabElem.HideControlForZeroRows = HideControlForZeroRows;
            tabElem.ZeroRowsText = ZeroRowsText;
            tabElem.EncodeMenuCaption = EncodeMenuCaption;
            tabElem.Columns = Columns;

            tabElem.FilterName = FilterName;
            tabElem.ReloadData(true);
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = tabElem.Visible;

        if (DataHelper.DataSourceIsEmpty(tabElem.DataSource) && (tabElem.HideControlForZeroRows))
        {
            Visible = false;
        }
    }
}