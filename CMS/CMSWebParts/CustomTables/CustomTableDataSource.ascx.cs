using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_CustomTables_CustomTableDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the custom table name.
    /// </summary>
    public string CustomTable
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CustomTable"), "");
        }
        set
        {
            SetValue("CustomTable", value);
            srcTables.CustomTable = value;
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
            srcTables.WhereCondition = value;
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
            srcTables.OrderBy = value;
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
            srcTables.TopN = value;
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
            srcTables.SourceFilterName = value;
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
            srcTables.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcTables.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcTables.CacheDependencies = value;
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
            srcTables.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gest or sets columns which will be displayed.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), "");
        }
        set
        {
            SetValue("Columns", value);
            srcTables.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates that if a page is selected,
    /// the datasource will provide data for the selected page only.
    /// </summary>
    public bool LoadCurrentPageOnly
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableSelectedItem"), srcTables.LoadCurrentPageOnly);
        }
        set
        {
            SetValue("EnableSelectedItem", value);
            srcTables.LoadCurrentPageOnly = value;
        }
    }


    /// <summary>
    /// Gets or sets query string key name. Presence of the key in query string indicates, 
    /// that some item should be selected. The item is determined by query string value.        
    /// </summary>
    public string SelectedQueryStringKeyName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedQueryStringKeyName"), srcTables.SelectedQueryStringKeyName);
        }
        set
        {
            SetValue("SelectedQueryStringKeyName", value);
            srcTables.SelectedQueryStringKeyName = value;
        }
    }


    /// <summary>
    /// Gets or sets columns name by which the item is selected.
    /// </summary>
    public string SelectedDatabaseColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedDatabaseColumnName"), srcTables.SelectedDatabaseColumnName);
        }
        set
        {
            SetValue("SelectedDatabaseColumnName", value);
            srcTables.SelectedDatabaseColumnName = value;
        }
    }


    /// <summary>
    /// Gets or sets validation type for query string value which determines selected item. 
    /// Options are int, guid and string.
    /// </summary>
    public string SelectedValidationType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedValidationType"), srcTables.SelectedValidationType);
        }
        set
        {
            SetValue("SelectedValidationType", value);
            srcTables.SelectedValidationType = value;
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
            srcTables.StopProcessing = value;
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
            srcTables.CustomTable = CustomTable;
            srcTables.WhereCondition = WhereCondition;
            srcTables.OrderBy = OrderBy;
            srcTables.TopN = SelectTopN;
            srcTables.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcTables.SourceFilterName = FilterName;
            srcTables.CacheItemName = CacheItemName;
            srcTables.CacheDependencies = CacheDependencies;
            srcTables.CacheMinutes = CacheMinutes;
            srcTables.SelectedColumns = Columns;
            srcTables.LoadCurrentPageOnly = LoadCurrentPageOnly;
            srcTables.SelectedQueryStringKeyName = SelectedQueryStringKeyName;
            srcTables.SelectedDatabaseColumnName = SelectedDatabaseColumnName;
            srcTables.SelectedValidationType = SelectedValidationType;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcTables.ClearCache();
    }

    #endregion
}