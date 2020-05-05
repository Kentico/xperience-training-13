using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_MediaLibrary_MediaLibraryDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), String.Empty);
        }
        set
        {
            SetValue("WhereCondition", value);
            srcMediaLib.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), String.Empty);
        }
        set
        {
            SetValue("OrderBy", value);
            srcMediaLib.OrderBy = value;
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
            srcMediaLib.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), String.Empty);
        }
        set
        {
            SetValue("FilterName", value);
            srcMediaLib.SourceFilterName = value;
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
            srcMediaLib.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcMediaLib.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcMediaLib.CacheDependencies = value;
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
            srcMediaLib.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gest or sets selected columns.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), String.Empty);
        }
        set
        {
            SetValue("Columns", value);
            srcMediaLib.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gest or sets site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), String.Empty);
        }
        set
        {
            SetValue("SiteName", value);
            srcMediaLib.SiteName = value;
        }
    }


    /// <summary>
    /// Indicates if group libraries should be included.
    /// </summary>
    public bool ShowGroupLibraries
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowGroupLibraries"), false);
        }
        set
        {
            SetValue("ShowGroupLibraries", value);
            srcMediaLib.ShowGroupLibraries = value;
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
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            return;
        }

        srcMediaLib.WhereCondition = WhereCondition;
        srcMediaLib.OrderBy = OrderBy;
        srcMediaLib.TopN = SelectTopN;
        srcMediaLib.FilterName = ID;
        srcMediaLib.SourceFilterName = FilterName;
        srcMediaLib.CacheItemName = CacheItemName;
        srcMediaLib.CacheDependencies = CacheDependencies;
        srcMediaLib.CacheMinutes = CacheMinutes;
        srcMediaLib.SelectedColumns = Columns;
        srcMediaLib.ShowGroupLibraries = ShowGroupLibraries;
        srcMediaLib.SiteName = SiteName;
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcMediaLib.ClearCache();
    }

    #endregion
}