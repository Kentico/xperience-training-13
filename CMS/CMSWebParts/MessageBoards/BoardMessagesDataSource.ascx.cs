using System;

using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_MessageBoards_BoardMessagesDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the message board name.
    /// </summary>
    public string BoardName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BoardName"), srcMessages.BoardName);
        }
        set
        {
            SetValue("BoardName", value);
            srcMessages.BoardName = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), srcMessages.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            srcMessages.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets Select only approved property.
    /// </summary>
    public bool SelectOnlyApproved
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyApproved"), srcMessages.SelectOnlyApproved);
        }
        set
        {
            SetValue("SelectOnlyApproved", value);
            srcMessages.SelectOnlyApproved = value;
        }
    }


    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), srcMessages.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            srcMessages.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), srcMessages.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            srcMessages.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets top N selected documents.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), srcMessages.TopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            srcMessages.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), srcMessages.SourceFilterName);
        }
        set
        {
            SetValue("FilterName", value);
            srcMessages.SourceFilterName = value;
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
            srcMessages.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcMessages.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcMessages.CacheDependencies = value;
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
            srcMessages.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets selected columns.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), srcMessages.SelectedColumns);
        }
        set
        {
            SetValue("Columns", value);
            srcMessages.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Indicates if group messages should be included.
    /// </summary>
    public bool ShowGroupMessages
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowGroupMessages"), false);
        }
        set
        {
            SetValue("ShowGroupMessages", value);
            srcMessages.ShowGroupMessages = value;
        }
    }

    #endregion


    #region "Document properties"

    /// <summary>
    /// Indicates if the comments should be retrieved according to document filter settings.
    /// </summary>
    public bool UseDocumentFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseDocumentFilter"), srcMessages.UseDocumentFilter);
        }
        set
        {
            SetValue("UseDocumentFilter", value);
            srcMessages.UseDocumentFilter = value;
        }
    }


    /// <summary>
    /// Gets or sets the alias path of the board document.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), srcMessages.Path);
        }
        set
        {
            SetValue("Path", value);
            srcMessages.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the board document.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CultureCode"), srcMessages.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            srcMessages.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition for board documents.
    /// </summary>
    public string DocumentsWhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentsWhereCondition"), srcMessages.DocumentsWhereCondition);
        }
        set
        {
            SetValue("DocumentsWhereCondition", value);
            srcMessages.DocumentsWhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets combine with default culture for board document.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), srcMessages.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            srcMessages.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets select only published for documents.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), srcMessages.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            srcMessages.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets max relative level for board documents.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), srcMessages.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            srcMessages.MaxRelativeLevel = value;
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
            // Prepare alias path
            string aliasPath = Path;
            if (String.IsNullOrEmpty(aliasPath))
            {
                aliasPath = "/%";
            }
            aliasPath = MacroResolver.ResolveCurrentPath(aliasPath);

            // Prepare site name
            string siteName = SiteName;
            if (String.IsNullOrEmpty(siteName))
            {
                siteName = SiteContext.CurrentSiteName;
            }

            // Prepare culture code
            string cultureCode = CultureCode;
            if (String.IsNullOrEmpty(cultureCode))
            {
                cultureCode = LocalizationContext.PreferredCultureCode;
            }

            srcMessages.BoardName = BoardName;
            srcMessages.SiteName = siteName;
            srcMessages.WhereCondition = WhereCondition;
            srcMessages.OrderBy = OrderBy;
            srcMessages.TopN = SelectTopN;
            srcMessages.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcMessages.SourceFilterName = FilterName;
            srcMessages.CacheItemName = CacheItemName;
            srcMessages.CacheDependencies = CacheDependencies;
            srcMessages.CacheMinutes = CacheMinutes;
            srcMessages.SelectOnlyApproved = SelectOnlyApproved;
            srcMessages.SelectedColumns = Columns;
            srcMessages.ShowGroupMessages = ShowGroupMessages;

            // Documents properties
            srcMessages.Path = aliasPath;
            srcMessages.UseDocumentFilter = UseDocumentFilter;
            srcMessages.CultureCode = cultureCode;
            srcMessages.DocumentsWhereCondition = DocumentsWhereCondition;
            srcMessages.CombineWithDefaultCulture = CombineWithDefaultCulture;
            srcMessages.SelectOnlyPublished = SelectOnlyPublished;
            srcMessages.MaxRelativeLevel = MaxRelativeLevel;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcMessages.ClearCache();
    }

    #endregion
}