using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Syndication_Basic_AtomFeed : CMSAbstractWebPart
{
    #region "Variables"

    private CMSBaseDataSource mDataSourceControl = null;
    private Guid mFeedGUID = Guid.Empty;

    #endregion


    #region "Atom Feed Properties"

    /// <summary>
    /// Querystring key which is used for Atom feed identification on a page with multiple Atom feeds.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("QueryStringKey"), null), atomFeed.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            atomFeed.QueryStringKey = value;
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
            atomFeed.FeedName = valueToSet;
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
            atomFeed.LinkText = value;
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
            atomFeed.LinkIcon = value;
        }
    }


    /// <summary>
    /// Indicates if the Atom feed is automatically discovered by the browser.
    /// </summary>
    public bool EnableAtomAutodiscovery
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableAtomAutodiscovery"), true);
        }
        set
        {
            SetValue("EnableAtomAutodiscovery", value);
            atomFeed.EnableAutodiscovery = value;
        }
    }

    #endregion


    #region "Atom Repeater properties"

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
            atomFeed.FeedTitle = value;
        }
    }


    /// <summary>
    /// Subtitle/Description of the feed.
    /// </summary>
    public string FeedSubtitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FeedSubtitle"), string.Empty);
        }
        set
        {
            SetValue("FeedSubtitle", value);
            atomFeed.FeedSubtitle = value;
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
            atomFeed.FeedLanguage = value;
        }
    }


    /// <summary>
    /// The unique identifier of the feed. If the value is empty, Guid.Empty will be used.
    /// </summary>
    public Guid FeedGUID
    {
        get
        {
            if (mFeedGUID == Guid.Empty)
            {
                return InstanceGUID;
            }
            else
            {
                return mFeedGUID;
            }
        }
        set
        {
            Guid valueToSet = value;
            // If no feed GUID was specified
            if (valueToSet == Guid.Empty)
            {
                // Set default GUID
                valueToSet = InstanceGUID;
            }
            mFeedGUID = valueToSet;
            atomFeed.FeedGUID = valueToSet;
        }
    }


    /// <summary>
    /// Last significant modification date of the feed.
    /// </summary>
    public DateTime FeedUpdated
    {
        get
        {
            return ValidationHelper.GetDateTimeSystem(GetValue("FeedUpdated"), DateTimeHelper.ZERO_TIME);
        }
        set
        {
            SetValue("FeedUpdated", value);
            atomFeed.FeedUpdated = value;
        }
    }


    /// <summary>
    /// Author name of the feed.
    /// </summary>
    public string FeedAuthorName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FeedAuthorName"), string.Empty);
        }
        set
        {
            SetValue("FeedAuthorName", value);
            atomFeed.FeedAuthorName = value;
        }
    }


    /// <summary>
    /// Custom feed header XML which is generated before feed items. If the value is empty default header for Atom feed is generated.
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
            atomFeed.HeaderXML = value;
        }
    }


    /// <summary>
    /// Custom feed footer XML which is generated after feed items. If the value is empty default footer for Atom feed is generated.
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
            atomFeed.FooterXML = value;
        }
    }

    #endregion


    #region "Datasource properties"

    /// <summary>
    /// Gets or sets name of source.
    /// </summary>
    public string DataSourceName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataSourceName"), string.Empty);
        }
        set
        {
            SetValue("DataSourceName", value);
            atomFeed.DataSourceName = value;
        }
    }


    /// <summary>
    /// Control with data source.
    /// </summary>
    public CMSBaseDataSource DataSourceControl
    {
        get
        {
            return mDataSourceControl;
        }
        set
        {
            mDataSourceControl = value;
            atomFeed.DataSourceControl = value;
        }
    }


    /// <summary>
    /// Related data object.
    /// </summary>
    public override object RelatedData
    {
        get
        {
            if (atomFeed.DataSourceControl != null)
            {
                return atomFeed.DataSourceControl.RelatedData;
            }
            return base.RelatedData;
        }
    }

    #endregion


    #region "Cache properties"

    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return ValidationHelper.GetString(base.CacheItemName, atomFeed.CacheItemName);
        }
        set
        {
            base.CacheItemName = value;
            atomFeed.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, atomFeed.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            atomFeed.CacheDependencies = value;
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
            atomFeed.CacheMinutes = value;
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
            atomFeed.TransformationName = value;
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
            atomFeed.StopProcessing = value;
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
            atomFeed.StopProcessing = true;
        }
        else
        {
            // Atom feed properties
            atomFeed.FeedName = FeedName;
            atomFeed.FeedLink = URLHelper.GetAbsoluteUrl(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, QueryStringKey, FeedName));
            atomFeed.LinkText = LinkText;
            atomFeed.LinkIcon = LinkIcon;
            atomFeed.FeedTitle = FeedTitle;
            atomFeed.FeedSubtitle = FeedSubtitle;
            atomFeed.FeedLanguage = FeedLanguage;
            atomFeed.FeedAuthorName = FeedAuthorName;
            atomFeed.FeedGUID = FeedGUID;
            atomFeed.FeedUpdated = FeedUpdated;
            atomFeed.EnableAutodiscovery = EnableAtomAutodiscovery;
            atomFeed.QueryStringKey = QueryStringKey;
            atomFeed.HeaderXML = HeaderXML;
            atomFeed.FooterXML = FooterXML;

            // Cache properties
            atomFeed.CacheItemName = CacheItemName;
            atomFeed.CacheDependencies = CacheDependencies;
            atomFeed.CacheMinutes = CacheMinutes;

            // Transformation properties
            atomFeed.TransformationName = TransformationName;

            // Datasource properties
            atomFeed.DataSourceName = DataSourceName;
        }
    }

    #endregion
}