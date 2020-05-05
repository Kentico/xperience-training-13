using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_Syndication_Basic_RSSRepeater : CMSAbstractWebPart
{
    #region "Variables"

    private CMSBaseDataSource mDataSourceControl = null;

    #endregion


    #region "RSS Repeater properties"

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
            rssRepeater.FeedName = valueToSet;
        }
    }


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
            rssRepeater.FeedTitle = value;
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
            rssRepeater.FeedDescription = value;
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
            rssRepeater.FeedLanguage = value;
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
            rssRepeater.HeaderXML = value;
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
            rssRepeater.FooterXML = value;
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
            rssRepeater.DataSourceName = value;
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
            rssRepeater.DataSourceControl = value;
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
            return ValidationHelper.GetString(base.CacheItemName, rssRepeater.CacheItemName);
        }
        set
        {
            base.CacheItemName = value;
            rssRepeater.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, rssRepeater.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            rssRepeater.CacheDependencies = value;
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
            rssRepeater.CacheMinutes = value;
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
            rssRepeater.TransformationName = value;
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
            rssRepeater.StopProcessing = value;
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
        var vm = PortalContext.ViewMode;

        if (StopProcessing || PortalContext.IsDesignMode(vm) || vm.IsOneOf(ViewModeEnum.Edit, ViewModeEnum.EditDisabled))
        {
            rssRepeater.StopProcessing = true;
        }
        else
        {
            // RSS feed properties
            rssRepeater.FeedName = FeedName;
            rssRepeater.FeedLink = URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL);
            rssRepeater.FeedTitle = FeedTitle;
            rssRepeater.FeedDescription = FeedDescription;
            rssRepeater.FeedLanguage = FeedLanguage;
            rssRepeater.HeaderXML = HeaderXML;
            rssRepeater.FooterXML = FooterXML;

            // Cache properties
            rssRepeater.CacheItemName = CacheItemName;
            rssRepeater.CacheDependencies = CacheDependencies;
            rssRepeater.CacheMinutes = CacheMinutes;

            // Transformation properties
            rssRepeater.TransformationName = TransformationName;

            // Datasource properties
            rssRepeater.DataSourceName = DataSourceName;
        }
    }

    #endregion
}