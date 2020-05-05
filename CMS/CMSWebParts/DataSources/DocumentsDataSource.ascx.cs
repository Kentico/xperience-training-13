using System;

using CMS.Helpers;
using CMS.Localization;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_DataSources_DocumentsDataSource : CMSAbstractWebPart
{
    #region "Document properties"

    /// <summary>
    /// Load pages individually.
    /// </summary>
    public bool LoadPagesIndividually
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LoadPagesIndividually"), true);
        }
        set
        {
            SetValue("LoadPagesIndividually", value);
            srcDocuments.LoadPagesIndividually = value;
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
            srcDocuments.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcDocuments.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcDocuments.CacheDependencies = value;
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
            srcDocuments.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
            srcDocuments.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Classnames"), "");
        }
        set
        {
            SetValue("ClassNames", value);
            srcDocuments.ClassNames = value;
        }
    }


    /// <summary>
    /// Category name.
    /// </summary>
    public string CategoryName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoryName"), "");
        }
        set
        {
            SetValue("CategoryName", value);
            srcDocuments.CategoryName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), srcDocuments.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            srcDocuments.CombineWithDefaultCulture = value;
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
            srcDocuments.CultureCode = value;
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
            srcDocuments.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
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
            srcDocuments.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), srcDocuments.Path);
        }
        set
        {
            SetValue("Path", value);
            srcDocuments.Path = value;
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
            srcDocuments.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
            srcDocuments.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
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
            srcDocuments.WhereCondition = value;
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
            srcDocuments.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of a filter to be used for data source.
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
            srcDocuments.SourceFilterName = value;
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
            return ValidationHelper.GetBoolean(GetValue("EnableSelectedItem"), srcDocuments.LoadCurrentPageOnly);
        }
        set
        {
            SetValue("EnableSelectedItem", value);
            srcDocuments.LoadCurrentPageOnly = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), srcDocuments.SelectedColumns);
        }
        set
        {
            SetValue("Columns", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the duplicated (linked) items should be filtered out from the data.
    /// </summary>   
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), false);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            srcDocuments.FilterOutDuplicates = value;
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
            // Document properties
            srcDocuments.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcDocuments.LoadPagesIndividually = LoadPagesIndividually;
            srcDocuments.CacheItemName = CacheItemName;
            srcDocuments.CacheDependencies = CacheDependencies;
            srcDocuments.CacheMinutes = CacheMinutes;
            srcDocuments.CategoryName = CategoryName;
            srcDocuments.CheckPermissions = CheckPermissions;
            srcDocuments.ClassNames = ClassNames;
            srcDocuments.CombineWithDefaultCulture = CombineWithDefaultCulture;
            srcDocuments.CultureCode = CultureCode;
            srcDocuments.MaxRelativeLevel = MaxRelativeLevel;
            srcDocuments.OrderBy = OrderBy;
            srcDocuments.Path = Path;
            srcDocuments.SelectOnlyPublished = SelectOnlyPublished;
            srcDocuments.SiteName = SiteName;
            srcDocuments.WhereCondition = WhereCondition;
            srcDocuments.SelectTopN = SelectTopN;
            srcDocuments.SourceFilterName = FilterName;
            srcDocuments.LoadCurrentPageOnly = LoadCurrentPageOnly;
            srcDocuments.FilterOutDuplicates = FilterOutDuplicates;
            srcDocuments.SelectedColumns = Columns;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcDocuments.ClearCache();
    }
}