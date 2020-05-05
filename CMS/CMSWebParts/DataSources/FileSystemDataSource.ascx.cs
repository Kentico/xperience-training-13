using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_DataSources_FileSystemDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the custom table name.
    /// </summary>
    public string Path
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Path"), "");
        }
        set
        {
            SetValue("XmlUrl", value);
            srcFiles.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the include sub dirs property.
    /// </summary>
    public bool IncludeSubDirs
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IncludeSubDirs"), false);
        }
        set
        {
            SetValue("IncludeSubDirs", value);
            srcFiles.IncludeSubDirs = value;
        }
    }


    /// <summary>
    /// Gets or sets the custom table name.
    /// </summary>
    public string FilesFilter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilesFilter"), "");
        }
        set
        {
            SetValue("FilesFilter", value);
            srcFiles.FilesFilter = value;
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
            srcFiles.WhereCondition = value;
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
            srcFiles.OrderBy = value;
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
            srcFiles.TopN = value;
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
            srcFiles.SourceFilterName = value;
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
            srcFiles.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcFiles.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcFiles.CacheDependencies = value;
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
            srcFiles.CacheMinutes = value;
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
            srcFiles.Path = Path;
            srcFiles.IncludeSubDirs = IncludeSubDirs;
            srcFiles.FilesFilter = FilesFilter;
            srcFiles.WhereCondition = WhereCondition;
            srcFiles.OrderBy = OrderBy;
            srcFiles.TopN = SelectTopN;
            srcFiles.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcFiles.SourceFilterName = FilterName;
            srcFiles.CacheItemName = CacheItemName;
            srcFiles.CacheDependencies = CacheDependencies;
            srcFiles.CacheMinutes = CacheMinutes;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcFiles.ClearCache();
    }

    #endregion
}