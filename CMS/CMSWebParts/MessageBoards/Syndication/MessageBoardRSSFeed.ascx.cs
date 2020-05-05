using System;

using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_MessageBoards_Syndication_MessageBoardRSSFeed : CMSAbstractWebPart
{
    #region "RSS Feed Properties"

    /// <summary>
    /// Querystring key which is used for RSS feed identification on a page with multiple RSS feeds.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("QueryStringKey"), null), rssFeed.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            rssFeed.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Feed name to identify this feed on a page with multiple feeds. If the value is empty the GUID of the web part instance will be used by default.
    /// </summary>
    public string FeedName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("FeedName"), null), GetIdentifier());
        }
        set
        {
            string valueToSet = value;
            // If no feed name was specified
            if (string.IsNullOrEmpty(valueToSet))
            {
                // Set default name
                valueToSet = GetIdentifier();
            }
            SetValue("FeedName", valueToSet);
            rssFeed.FeedName = valueToSet;
        }
    }


    /// <summary>
    /// Text for the feed link.
    /// </summary>
    public string LinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkText"), string.Empty);
        }
        set
        {
            SetValue("LinkText", value);
            rssFeed.LinkText = value;
        }
    }


    /// <summary>
    /// Icon which will be displayed in the feed link.
    /// </summary>
    public string LinkIcon
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkIcon"), string.Empty);
        }
        set
        {
            SetValue("LinkIcon", value);
            rssFeed.LinkIcon = value;
        }
    }


    /// <summary>
    /// Indicates if the RSS feed is automatically discovered by the browser.
    /// </summary>
    public bool EnableRSSAutodiscovery
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableRSSAutodiscovery"), true);
        }
        set
        {
            SetValue("EnableRSSAutodiscovery", value);
            rssFeed.EnableAutodiscovery = value;
        }
    }

    #endregion


    #region "RSS Repeater properties"

    /// <summary>
    /// URL title of the feed.
    /// </summary>
    public string FeedTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FeedTitle"), string.Empty);
        }
        set
        {
            SetValue("FeedTitle", value);
            rssFeed.FeedTitle = value;
        }
    }


    /// <summary>
    /// Description of the feed.
    /// </summary>
    public string FeedDescription
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FeedDescription"), string.Empty);
        }
        set
        {
            SetValue("FeedDescription", value);
            rssFeed.FeedDescription = value;
        }
    }


    /// <summary>
    /// Language of the feed. If the value is empty the content culture will be used.
    /// </summary>
    public string FeedLanguage
    {
        get
        {
            string cultureCode = ValidationHelper.GetString(GetValue("FeedLanguage"), null);
            if (string.IsNullOrEmpty(cultureCode))
            {
                cultureCode = LocalizationContext.PreferredCultureCode;
            }
            return cultureCode;
        }
        set
        {
            SetValue("FeedLanguage", value);
            rssFeed.FeedLanguage = value;
        }
    }


    /// <summary>
    /// Custom feed header XML which is generated before feed items. If the value is empty default header for RSS feed is generated.
    /// </summary>
    public string HeaderXML
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderXML"), null);
        }
        set
        {
            SetValue("HeaderXML", value);
            rssFeed.HeaderXML = value;
        }
    }


    /// <summary>
    /// Custom feed footer XML which is generated after feed items. If the value is empty default footer for RSS feed is generated.
    /// </summary>
    public string FooterXML
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FooterXML"), null);
        }
        set
        {
            SetValue("FooterXML", value);
            rssFeed.FooterXML = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), string.Empty);
        }
        set
        {
            SetValue("TransformationName", value);
            rssFeed.TransformationName = value;
        }
    }

    #endregion


    #region "Datasource properties"

    /// <summary>
    /// Gets or sets the message board name.
    /// </summary>
    public string BoardName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardName"), srcMessages.BoardName);
        }
        set
        {
            SetValue("BoardName", value);
            srcMessages.BoardName = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), srcMessages.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            srcMessages.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets Select only approved property.
    /// </summary>
    public bool SelectOnlyApproved
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyApproved"), srcMessages.SelectOnlyApproved);
        }
        set
        {
            SetValue("SelectOnlyApproved", value);
            srcMessages.SelectOnlyApproved = value;
        }
    }


    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), srcMessages.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            srcMessages.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), srcMessages.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            srcMessages.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets top N selected documents.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), srcMessages.TopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            srcMessages.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), srcMessages.SourceFilterName);
        }
        set
        {
            SetValue("FilterName", value);
            srcMessages.SourceFilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets selected columns.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), srcMessages.SelectedColumns);
        }
        set
        {
            SetValue("Columns", value);
            srcMessages.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Indicates if group messages should be included.
    /// </summary>
    public bool ShowGroupMessages
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowGroupMessages"), false);
        }
        set
        {
            SetValue("ShowGroupMessages", value);
            srcMessages.ShowGroupMessages = value;
        }
    }

    #endregion


    #region "Document properties"

    /// <summary>
    /// Indicates if the comments should be retrieved according to document filter settings.
    /// </summary>
    public bool UseDocumentFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseDocumentFilter"), srcMessages.UseDocumentFilter);
        }
        set
        {
            SetValue("UseDocumentFilter", value);
            srcMessages.UseDocumentFilter = value;
        }
    }


    /// <summary>
    /// Gets or sets the alias path of the board document.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), srcMessages.Path);
        }
        set
        {
            SetValue("Path", value);
            srcMessages.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the board document.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CultureCode"), srcMessages.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            srcMessages.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition for board documents.
    /// </summary>
    public string DocumentsWhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentsWhereCondition"), srcMessages.DocumentsWhereCondition);
        }
        set
        {
            SetValue("DocumentsWhereCondition", value);
            srcMessages.DocumentsWhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets combine with default culture for board document.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), srcMessages.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            srcMessages.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets select only published for documents.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), srcMessages.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            srcMessages.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets max relative level for board documents.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), srcMessages.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            srcMessages.MaxRelativeLevel = value;
        }
    }

    #endregion


    #region "Cache properties"

    /// <summary>
    /// Gest or sets the cache item name.
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
            srcMessages.CacheItemName = value;
            rssFeed.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, rssFeed.CacheDependencies + "\n" + srcMessages.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcMessages.CacheDependencies = value;
            rssFeed.CacheDependencies = value;
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
            srcMessages.CacheMinutes = value;
            rssFeed.CacheMinutes = value;
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
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
            rssFeed.StopProcessing = value;
            srcMessages.StopProcessing = value;
        }
    }

    #endregion


    #region "Overidden methods"

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

    #endregion


    #region "Setup control"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            rssFeed.StopProcessing = true;
            srcMessages.StopProcessing = true;
        }
        else
        {
            string feedCodeName = URLHelper.GetSafeUrlPart(FeedName, SiteName);
            // RSS feed properties
            rssFeed.FeedName = feedCodeName;
            rssFeed.FeedLink = URLHelper.GetAbsoluteUrl(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, QueryStringKey, feedCodeName));
            rssFeed.LinkText = LinkText;
            rssFeed.LinkIcon = LinkIcon;
            rssFeed.FeedTitle = FeedTitle;
            rssFeed.FeedDescription = FeedDescription;
            rssFeed.FeedLanguage = FeedLanguage;
            rssFeed.EnableAutodiscovery = EnableRSSAutodiscovery;
            rssFeed.QueryStringKey = QueryStringKey;
            rssFeed.HeaderXML = HeaderXML;
            rssFeed.FooterXML = FooterXML;


            // Prepare alias path
            string aliasPath = Path;
            if (String.IsNullOrEmpty(aliasPath))
            {
                aliasPath = "/%";
            }
            aliasPath = MacroResolver.ResolveCurrentPath(aliasPath);

            // Prepare site name
            string siteName = SiteName;
            if (String.IsNullOrEmpty(siteName))
            {
                siteName = SiteContext.CurrentSiteName;
            }

            // Prepare culture code
            string cultureCode = CultureCode;
            if (String.IsNullOrEmpty(cultureCode))
            {
                cultureCode = LocalizationContext.PreferredCultureCode;
            }

            // Messages datasource properties
            srcMessages.BoardName = BoardName;
            srcMessages.SiteName = siteName;
            srcMessages.WhereCondition = WhereCondition;
            srcMessages.OrderBy = OrderBy;
            srcMessages.TopN = SelectTopN;
            srcMessages.SourceFilterName = FilterName;
            srcMessages.SelectOnlyApproved = SelectOnlyApproved;
            srcMessages.SelectedColumns = Columns;
            srcMessages.ShowGroupMessages = ShowGroupMessages;

            // Documents properties
            srcMessages.Path = aliasPath;
            srcMessages.UseDocumentFilter = UseDocumentFilter;
            srcMessages.CultureCode = cultureCode;
            srcMessages.DocumentsWhereCondition = DocumentsWhereCondition;
            srcMessages.CombineWithDefaultCulture = CombineWithDefaultCulture;
            srcMessages.SelectOnlyPublished = SelectOnlyPublished;
            srcMessages.MaxRelativeLevel = MaxRelativeLevel;

            // Cache properties
            rssFeed.CacheItemName = CacheItemName;
            rssFeed.CacheDependencies = CacheDependencies;
            rssFeed.CacheMinutes = CacheMinutes;
            srcMessages.CacheItemName = CacheItemName;
            srcMessages.CacheDependencies = CacheDependencies;
            srcMessages.CacheMinutes = CacheMinutes;

            // Transformation properties
            rssFeed.TransformationName = TransformationName;

            // Set datasource
            rssFeed.DataSourceControl = srcMessages;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcMessages.ClearCache();
    }

    #endregion
}