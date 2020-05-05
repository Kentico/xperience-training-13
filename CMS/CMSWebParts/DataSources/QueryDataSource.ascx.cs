using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_DataSources_QueryDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Load pages individually.
    /// </summary>
    public bool LoadPagesIndividually
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LoadPagesIndividually"), false);
        }
        set
        {
            SetValue("LoadPagesIndividually", value);
            srcElem.LoadPagesIndividually = value;
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
            srcElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcElem.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcElem.CacheDependencies = value;
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
            srcElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the ORDER BY clause.
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
            srcElem.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the WHERE condition.
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
            srcElem.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the SELECT part of the query.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), null);
        }
        set
        {
            SetValue("Columns", value);
            srcElem.SelectedColumns = value;
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
            srcElem.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of a query to be used.
    /// </summary>
    public String QueryName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("QueryName"), "");
        }
        set
        {
            SetValue("QueryName", value);
            srcElem.QueryName = value;
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
            srcElem.SourceFilterName = value;
        }
    }

    #endregion


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
            // Setup query data source
            srcElem.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcElem.LoadPagesIndividually = LoadPagesIndividually;
            srcElem.CacheItemName = CacheItemName;
            srcElem.CacheDependencies = CacheDependencies;
            srcElem.CacheMinutes = CacheMinutes;
            srcElem.OrderBy = OrderBy;
            srcElem.WhereCondition = WhereCondition;
            srcElem.SelectedColumns = Columns;
            srcElem.SelectTopN = SelectTopN;
            srcElem.QueryName = QueryName;
            srcElem.SourceFilterName = FilterName;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcElem.ClearCache();
    }
}