using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.DocumentEngine;
using CMS.MacroEngine;
using CMS.PortalEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_Text_pagedtext : CMSAbstractWebPart
{
    #region "Document properties"

    /// <summary>
    /// Gets or sets the value that indicates whether the permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the default language version of the document 
    /// should be displayed if the document is not translated to the current language.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), true);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
        }
    }


    /// <summary>
    /// Gets or sets the culture version of the displayed content.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CultureCode"), LocalizationContext.PreferredCultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
        }
    }


    /// <summary>
    /// Gets or sets the path to the document.
    /// </summary>
    public string Path
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Path"), "");
        }
        set
        {
            SetValue("Path", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to show only published documents.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), true);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
        }
    }


    /// <summary>
    /// Gets or sets the codename of the site from which you want do display the content.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the document field that should be used as a source of the text.
    /// </summary>
    public string ColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ColumnName"), "");
        }
        set
        {
            SetValue("ColumnName", value);
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Gets or sets the pager position (position of page numbers).
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return textPager.PagerControl.GetPagerPosition(DataHelper.GetNotEmpty(GetValue("PagerPosition"), textPager.PagerControl.PagerPosition.ToString()));
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            textPager.PagerControl.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the page size (number of characters displayed per page).
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), textPager.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            textPager.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("QueryStringKey"), textPager.PagerControl.QueryStringKey), textPager.PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            textPager.PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager format string, you can use standard macros (ie. Page {0} of {2}).
    /// </summary>
    public string PagerFormat
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerFormat"), textPager.PagerControl.ResultsFormat);
        }
        set
        {
            SetValue("PagerFormat", value);
            textPager.PagerControl.ResultsFormat = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagingModeTypeEnum PagingMode
    {
        get
        {
            return textPager.PagerControl.GetPagingMode(DataHelper.GetNotEmpty(GetValue("PagingMode"), textPager.PagerControl.PagingMode.ToString()));
        }
        set
        {
            SetValue("PagingMode", value.ToString());
            textPager.PagerControl.PagingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to show links to first / last page
    /// </summary>
    public bool ShowFirstLast
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFirstLast"), textPager.PagerControl.ShowFirstLast);
        }
        set
        {
            SetValue("ShowFirstLast", value);
            textPager.PagerControl.ShowFirstLast = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager separator text (text which is displayed at the end of each page).
    /// </summary>
    public string PagerSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerSeparator"), textPager.PagerSeparator);
        }
        set
        {
            SetValue("PagerSeparator", value);
            textPager.PagerSeparator = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the text CSS class (css class of the div around the text).
    /// </summary>
    public string TextCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TextCSSClass"), textPager.TextCSSClass);
        }
        set
        {
            SetValue("TextCSSClass", value);
            textPager.TextCSSClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the pager CSS class (css class of the div around the page numbers).
    /// </summary>
    public string PagerCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerCSSClass"), textPager.PagerCSSClass);
        }
        set
        {
            SetValue("PagerCSSClass", value);
            textPager.PagerCSSClass = value;
        }
    }


    /// <summary>
    /// Gets or sets pager numbers separator.
    /// </summary>
    public string PageNumbersSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageNumbersSeparator"), textPager.PagerControl.PageNumbersSeparator);
        }
        set
        {
            SetValue("PageNumbersSeparator", value);
            textPager.PagerControl.PageNumbersSeparator = value;
        }
    }


    /// <summary>
    /// Gets or sets html contect before pager.
    /// </summary>
    public string PagerHTMLBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLBefore"), textPager.PagerControl.PagerHTMLBefore);
        }
        set
        {
            SetValue("PagerHTMLBefore", value);
            textPager.PagerControl.PagerHTMLBefore = value;
        }
    }


    /// <summary>
    /// Gets or sets html contect after pager.
    /// </summary>
    public string PagerHTMLAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLAfter"), textPager.PagerControl.PagerHTMLAfter);
        }
        set
        {
            SetValue("PagerHTMLAfter", value);
            textPager.PagerControl.PagerHTMLAfter = value;
        }
    }


    /// <summary>
    /// Gets or sets the navigation mode.
    /// </summary>
    public BackNextLocationTypeEnum BackNextLocation
    {
        get
        {
            return textPager.PagerControl.GetBackNextLocation(DataHelper.GetNotEmpty(GetValue("BackNextLocation"), textPager.PagerControl.BackNextLocation.ToString()));
        }
        set
        {
            SetValue("BackNextLocation", value.ToString());
            textPager.PagerControl.BackNextLocation = value;
        }
    }

    #endregion


    #region "Methods & events"

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
            // Do nothing
        }
        else
        {
            textPager.PagerControl.PagerPosition = PagerPosition;
            textPager.PagerControl.QueryStringKey = QueryStringKey;
            textPager.PagerControl.PagingMode = PagingMode;
            textPager.PagerControl.ShowFirstLast = ShowFirstLast;
            textPager.PagerControl.ResultsFormat = PagerFormat;
            textPager.PagerSeparator = PagerSeparator;
            textPager.PagerCSSClass = PagerCSSClass;
            textPager.TextCSSClass = TextCSSClass;
            textPager.PageSize = PageSize;
            textPager.PagerControl.PageNumbersSeparator = PageNumbersSeparator;
            textPager.PagerControl.PagerHTMLBefore = PagerHTMLBefore;
            textPager.PagerControl.PagerHTMLAfter = PagerHTMLAfter;
            textPager.PagerControl.BackNextLocation = BackNextLocation;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        LoadData();
    }


    /// <summary>
    /// Loads the data to the control.
    /// </summary>
    protected void LoadData()
    {
        SetContext();
        TreeNode node = null;

        if (Path == string.Empty)
        {
            node = DocumentContext.CurrentDocument;
        }
        else
        {
            // Try to get data from cache
            using (var cs = new CachedSection<TreeNode>(ref node, CacheMinutes, true, CacheItemName, "pagedtext", CacheHelper.GetBaseCacheKey(CheckPermissions, false), SiteName, Path, CultureCode, CombineWithDefaultCulture, SelectOnlyPublished))
            {
                if (cs.LoadData)
                {
                    // Ensure that the path is only for one single document
                    Path = MacroContext.CurrentResolver.ResolvePath(Path.TrimEnd('%').TrimEnd('/'));
                    
                    node = GetDocument();

                    // Prepare the cache dependency
                    if (cs.Cached)
                    {
                        cs.CacheDependency = GetCacheDependency();
                    }

                    cs.Data = node;
                }
            }
        }

        if ((node != null) && (ColumnName != string.Empty))
        {
            var text = ValidationHelper.GetString(node.GetValue(ColumnName), string.Empty);

            if (text != string.Empty)
            {
                textPager.TextSource = text;
                textPager.ReloadData();
            }
            else
            {
                Visible = false;
            }
        }
        else
        {
            Visible = false;
        }

        ReleaseContext();
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();

        textPager.ReloadData();
    }


    private TreeNode GetDocument()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        TreeNode node = tree.SelectSingleNode(SiteName, Path, CultureCode, CombineWithDefaultCulture, null, SelectOnlyPublished, CheckPermissions);

        if ((node != null) && (PortalContext.ViewMode != ViewModeEnum.LiveSite))
        {
            node = DocumentHelper.GetDocument(node, tree);
        }

        return node;
    }

    #endregion
}