using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_DataSources_WebServiceDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the web service URL.
    /// </summary>
    public string WebServiceUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WebServiceUrl"), "");
        }
        set
        {
            SetValue("WebServiceUrl", value);
            srcWebService.WebServiceUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the parameters of the web service.
    /// </summary>
    public string WebServiceParameters
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WebServiceParameters"), "");
        }
        set
        {
            SetValue("WebServiceParameters", value);
            srcWebService.WebServiceParameters = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), "");
        }
        set
        {
            SetValue("FilterName", value);
            srcWebService.SourceFilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
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
            srcWebService.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcWebService.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcWebService.CacheDependencies = value;
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
            srcWebService.CacheMinutes = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            srcWebService.WebServiceUrl = WebServiceUrl;
            srcWebService.WebServiceParameters = WebServiceParameters;
            srcWebService.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcWebService.SourceFilterName = FilterName;
            srcWebService.CacheItemName = CacheItemName;
            srcWebService.CacheDependencies = CacheDependencies;
            srcWebService.CacheMinutes = CacheMinutes;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcWebService.ClearCache();
    }

    #endregion
}