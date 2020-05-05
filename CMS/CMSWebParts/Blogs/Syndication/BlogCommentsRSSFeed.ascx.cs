using System;

using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Blogs_Syndication_BlogCommentsRSSFeed : CMSAbstractWebPart
{
    #region "RSS Feed Properties"

    /// <summary>
    /// Querystring key which is used for RSS feed identification on a page with multiple RSS feeds.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("QueryStringKey"), rssFeed.QueryStringKey);
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
            return DataHelper.GetNotEmpty(GetValue("FeedName"), GetIdentifier());
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


    #region "System settings properties"

    /// <summary>
    /// Gest or sest the cache item name.
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
            srcComments.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcComments.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcComments.CacheDependencies = value;
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
            srcComments.CacheMinutes = value;
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
            return ValidationHelper.GetBoolean(GetValue("UseDocumentFilter"), false);
        }
        set
        {
            SetValue("UseDocumentFilter", value);
            srcComments.UseDocumentFilter = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), false);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            srcComments.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
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
            srcComments.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), -1);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            srcComments.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), srcComments.Path);
        }
        set
        {
            SetValue("Path", value);
            srcComments.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), false);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            srcComments.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition for blog posts.
    /// </summary>
    public string DocumentsWhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentsWhereCondition"), string.Empty);
        }
        set
        {
            SetValue("DocumentsWhereCondition", value);
            srcComments.DocumentsWhereCondition = value;
        }
    }

    #endregion


    #region "Comments filter properties"

    /// <summary>
    /// Gets or sets if only approved comments should be selected.
    /// </summary>
    public bool SelectOnlyApproved
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyApproved"), true);
        }
        set
        {
            SetValue("SelectOnlyApproved", value);
            srcComments.SelectOnlyApproved = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), "");
        }
        set
        {
            SetValue("SiteName", value);
            srcComments.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), string.Empty);
        }
        set
        {
            SetValue("OrderBy", value);
            srcComments.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition for blog comments.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), string.Empty);
        }
        set
        {
            SetValue("WhereCondition", value);
            srcComments.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), 0);
        }
        set
        {
            SetValue("SelectTopN", value);
            srcComments.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets selected columns.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedColumns"), srcComments.SelectedColumns);
        }
        set
        {
            SetValue("SelectedColumns", value);
            srcComments.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of a filter to be used for data source.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), string.Empty);
        }
        set
        {
            SetValue("FilterName", value);
            srcComments.FilterName = value;
        }
    }

    #endregion


    #region "Data binding properties"

    /// <summary>
    /// Gets or sets the value that indicates whether databind will be proceeded automatically.
    /// </summary>
    public bool DataBindByDefault
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DataBindByDefault"), rssFeed.DataBindByDefault);
        }
        set
        {
            SetValue("DataBindByDefault", value);
            rssFeed.DataBindByDefault = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("TransformationName"), rssFeed.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            rssFeed.TransformationName = value;
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
            srcComments.StopProcessing = value;
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
            rssFeed.StopProcessing = true;
            srcComments.StopProcessing = true;
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
            rssFeed.CacheDependencies = CacheDependencies;
            rssFeed.CacheItemName = CacheItemName;
            rssFeed.CacheMinutes = CacheMinutes;

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

            // Document properties
            srcComments.UseDocumentFilter = UseDocumentFilter;
            srcComments.CacheItemName = CacheItemName;
            srcComments.CacheDependencies = CacheDependencies;
            srcComments.CacheMinutes = CacheMinutes;
            srcComments.CombineWithDefaultCulture = CombineWithDefaultCulture;
            srcComments.CultureCode = cultureCode;
            srcComments.MaxRelativeLevel = MaxRelativeLevel;
            srcComments.OrderBy = OrderBy;
            srcComments.Path = aliasPath;
            srcComments.SelectOnlyPublished = SelectOnlyPublished;
            srcComments.SiteName = siteName;
            srcComments.WhereCondition = WhereCondition;
            srcComments.TopN = SelectTopN;
            srcComments.SelectedColumns = Columns;
            srcComments.SourceFilterName = FilterName;
            srcComments.SelectOnlyApproved = SelectOnlyApproved;

            // Binding
            rssFeed.DataBindByDefault = DataBindByDefault;

            // Transformations
            rssFeed.TransformationName = TransformationName;

            // Connects RSS feed with data source
            rssFeed.DataSourceControl = srcComments;
        }
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcComments.ClearCache();
    }

    #endregion
}