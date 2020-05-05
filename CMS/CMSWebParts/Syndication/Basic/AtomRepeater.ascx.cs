using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_Syndication_Basic_AtomRepeater : CMSAbstractWebPart
{
    #region "Variables"

    private CMSBaseDataSource mDataSourceControl = null;
    private Guid mFeedGUID = Guid.Empty;

    #endregion


    #region "Atom Repeater properties"

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
            atomRepeater.FeedName = valueToSet;
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
            atomRepeater.FeedTitle = value;
        }
    }


    /// <summary>
    /// Description/Subtitle of the feed.
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
            atomRepeater.FeedSubtitle = value;
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
            atomRepeater.FeedLanguage = value;
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
            atomRepeater.FeedGUID = valueToSet;
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
            atomRepeater.FeedUpdated = value;
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
            atomRepeater.FeedAuthorName = value;
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
            atomRepeater.HeaderXML = value;
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
            atomRepeater.FooterXML = value;
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
            atomRepeater.DataSourceName = value;
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
            atomRepeater.DataSourceControl = value;
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
            return ValidationHelper.GetString(base.CacheItemName, atomRepeater.CacheItemName);
        }
        set
        {
            base.CacheItemName = value;
            atomRepeater.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, atomRepeater.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            atomRepeater.CacheDependencies = value;
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
            atomRepeater.CacheMinutes = value;
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
            atomRepeater.TransformationName = value;
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
            atomRepeater.StopProcessing = value;
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
            atomRepeater.StopProcessing = true;
        }
        else
        {
            // Atom feed properties
            atomRepeater.FeedName = FeedName;
            atomRepeater.FeedLink = URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL);
            atomRepeater.FeedTitle = FeedTitle;
            atomRepeater.FeedSubtitle = FeedSubtitle;
            atomRepeater.FeedLanguage = FeedLanguage;
            atomRepeater.FeedAuthorName = FeedAuthorName;
            atomRepeater.FeedGUID = FeedGUID;
            atomRepeater.FeedUpdated = FeedUpdated;
            atomRepeater.HeaderXML = HeaderXML;
            atomRepeater.FooterXML = FooterXML;

            // Cache properties
            atomRepeater.CacheItemName = CacheItemName;
            atomRepeater.CacheDependencies = CacheDependencies;
            atomRepeater.CacheMinutes = CacheMinutes;

            // Transformation properties
            atomRepeater.TransformationName = TransformationName;

            // Datasource properties
            atomRepeater.DataSourceName = DataSourceName;
        }
    }

    #endregion
}