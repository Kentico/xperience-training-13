using System;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Syndication_AtomDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the Atom feed URL.
    /// </summary>
    public string AtomFeedUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AtomFeedUrl"), "");
        }
        set
        {
            SetValue("AtomFeedUrl", value);
            srcXml.XmlUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the custom Atom XSD schema URL.
    /// </summary>
    public string AtomSchemaUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AtomSchemaUrl"), "");
        }
        set
        {
            SetValue("AtomSchemaUrl", value);
            srcXml.XmlSchemaUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), "");
        }
        set
        {
            SetValue("WhereCondition", value);
            srcXml.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "");
        }
        set
        {
            SetValue("OrderBy", value);
            srcXml.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets top N selected documents.
    /// </summary>
    public int TopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("TopN"), 0);
        }
        set
        {
            SetValue("TopN", value);
            srcXml.TopN = value;
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
            srcXml.SourceFilterName = value;
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
            srcXml.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcXml.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcXml.CacheDependencies = value;
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
            srcXml.CacheMinutes = value;
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
            srcXml.StopProcessing = true;
        }
        else
        {
            srcXml.XmlUrl = AtomFeedUrl;
            srcXml.XmlSchemaUrl = AtomSchemaUrl;
            srcXml.WhereCondition = WhereCondition;
            srcXml.OrderBy = OrderBy;
            srcXml.TopN = TopN;
            srcXml.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcXml.SourceFilterName = FilterName;
            srcXml.CacheItemName = CacheItemName;
            srcXml.CacheDependencies = CacheDependencies;
            srcXml.CacheMinutes = CacheMinutes;
            srcXml.TableName = "entry";
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcXml.ClearCache();
    }

    #endregion
}