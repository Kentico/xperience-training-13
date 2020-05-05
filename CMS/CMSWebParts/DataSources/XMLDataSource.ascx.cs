using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_DataSources_XMLDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the XML URL.
    /// </summary>
    public string XmlUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("XmlUrl"), "");
        }
        set
        {
            SetValue("XmlUrl", value);
            srcXml.XmlUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the XML schema URL.
    /// </summary>
    public string XmlSchemaUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("XmlSchemaUrl"), "");
        }
        set
        {
            SetValue("XmlSchemaUrl", value);
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
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), 0);
        }
        set
        {
            SetValue("SelectTopN", value);
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


    /// <summary>
    /// Gets or sets the name of dataset table which will be used as datasource.
    /// </summary>
    public string TableName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TableName"), "");
        }
        set
        {
            SetValue("TableName", value);
            srcXml.TableName = value;
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
            srcXml.XmlUrl = XmlUrl;
            srcXml.XmlSchemaUrl = XmlSchemaUrl;
            srcXml.WhereCondition = WhereCondition;
            srcXml.OrderBy = OrderBy;
            srcXml.TopN = SelectTopN;
            srcXml.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcXml.SourceFilterName = FilterName;
            srcXml.CacheItemName = CacheItemName;
            srcXml.CacheDependencies = CacheDependencies;
            srcXml.CacheMinutes = CacheMinutes;
            srcXml.TableName = TableName;
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