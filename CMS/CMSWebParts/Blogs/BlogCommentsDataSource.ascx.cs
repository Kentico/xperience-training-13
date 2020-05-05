using System;

using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_Blogs_BlogCommentsDataSource : CMSAbstractWebPart
{
    #region "Blog comments filter properties"

    /// <summary>
    /// Gets or sets if only approved comments should be selected.
    /// </summary>
    public bool SelectOnlyApproved
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyApproved"), true);
        }
        set
        {
            SetValue("SelectOnlyApproved", value);
            srcComments.SelectOnlyApproved = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition for blog comments.
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
            srcComments.WhereCondition = value;
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
            srcComments.OrderBy = value;
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
            srcComments.TopN = value;
        }
    }


    /// <summary>
    /// Gest or sets property to select columns from Data source.
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
            srcComments.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), "");
        }
        set
        {
            SetValue("SiteName", value);
            srcComments.SiteName = value;
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
            srcComments.SourceFilterName = value;
        }
    }

    #endregion


    #region "Document (Blog posts)  filter properties"

    /// <summary>
    /// Indicates if the comments should be retrieved according to document filter settings.
    /// </summary>
    public bool UseDocumentFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseDocumentFilter"), false);
        }
        set
        {
            SetValue("UseDocumentFilter", value);
            srcComments.UseDocumentFilter = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition for blog posts.
    /// </summary>
    public string DocumentsWhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentsWhereCondition"), "");
        }
        set
        {
            SetValue("DocumentsWhereCondition", value);
            srcComments.DocumentsWhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), false);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            srcComments.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CultureCode"), LocalizationContext.PreferredCultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            srcComments.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), -1);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            srcComments.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), srcComments.Path);
        }
        set
        {
            SetValue("Path", value);
            srcComments.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), false);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            srcComments.SelectOnlyPublished = value;
        }
    }

    #endregion


    #region "System settings properties"

    /// <summary>
    /// Gest or sest the cache item name.
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
            srcComments.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcComments.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcComments.CacheDependencies = value;
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
            srcComments.CacheMinutes = value;
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

            // Comments filter properties
            srcComments.SelectOnlyApproved = SelectOnlyApproved;
            srcComments.SiteName = siteName;
            srcComments.WhereCondition = WhereCondition;
            srcComments.OrderBy = OrderBy;
            srcComments.TopN = SelectTopN;
            srcComments.SelectedColumns = Columns;
            srcComments.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcComments.SourceFilterName = FilterName;

            // Caching properties
            srcComments.CacheItemName = CacheItemName;
            srcComments.CacheDependencies = CacheDependencies;
            srcComments.CacheMinutes = CacheMinutes;

            // Document filter properties
            srcComments.UseDocumentFilter = UseDocumentFilter;
            srcComments.DocumentsWhereCondition = DocumentsWhereCondition;
            srcComments.CombineWithDefaultCulture = CombineWithDefaultCulture;
            srcComments.CultureCode = cultureCode;
            srcComments.SelectOnlyPublished = SelectOnlyPublished;
            srcComments.MaxRelativeLevel = MaxRelativeLevel;
            srcComments.Path = aliasPath;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcComments.ClearCache();
    }

    #endregion
}