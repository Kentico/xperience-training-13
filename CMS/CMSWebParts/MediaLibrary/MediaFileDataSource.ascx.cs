using System;

using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_MediaLibrary_MediaFileDataSource : CMSAbstractWebPart
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
            srcMedia.WhereCondition = value;
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
            srcMedia.OrderBy = value;
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
            srcMedia.TopN = value;
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
            srcMedia.SourceFilterName = value;
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
            srcMedia.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, srcMedia.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            srcMedia.CacheDependencies = value;
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
            srcMedia.CacheMinutes = value;
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
            srcMedia.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets the source library name.
    /// </summary>
    public string LibraryName
    {
        get
        {
            string libraryName = ValidationHelper.GetString(GetValue("LibraryName"), String.Empty);
            if ((string.IsNullOrEmpty(libraryName) || libraryName == MediaLibraryInfoProvider.CURRENT_LIBRARY) && (MediaLibraryContext.CurrentMediaLibrary != null))
            {
                return MediaLibraryContext.CurrentMediaLibrary.LibraryName;
            }
            return libraryName;
        }
        set
        {
            SetValue("LibraryName", value);
            srcMedia.LibraryName = value;
        }
    }


    /// <summary>
    /// Gets or sets the source file path.
    /// </summary>
    public string FilePath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilePath"), String.Empty);
        }
        set
        {
            SetValue("FilePath", value);
            srcMedia.FilePath = value;
        }
    }


    /// <summary>
    /// Gets or sets the allowed file extensions.
    /// </summary>
    public string FileExtensions
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FileExtension"), String.Empty);
        }
        set
        {
            SetValue("FileExtension", value);
            srcMedia.FileExtensions = value;
        }
    }


    /// <summary>
    /// Gets or sets the file id querysting parameter.
    /// </summary>
    public string FileIDQueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FileIDQueryStringKey"), String.Empty);
        }
        set
        {
            SetValue("FileIDQueryStringKey", value);
        }
    }


    /// <summary>
    /// Gets or sets Check permissions property.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), true);
        }
        set
        {
            SetValue("CheckPermissions", value);
            srcMedia.CheckPermissions = value;
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
            srcMedia.SiteName = value;
        }
    }


    /// <summary>
    /// Indicates if group files should be included.
    /// </summary>
    public bool ShowGroupFiles
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowGroupFiles"), false);
        }
        set
        {
            SetValue("ShowGroupFiles", value);
            srcMedia.ShowGroupFiles = value;
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
            // Do nothing
        }
        else
        {
            // Prepare site name
            string siteName = SiteName;
            if (String.IsNullOrEmpty(siteName))
            {
                siteName = SiteContext.CurrentSiteName;
            }

            srcMedia.SiteName = siteName;
            srcMedia.WhereCondition = WhereCondition;
            srcMedia.OrderBy = OrderBy;
            srcMedia.TopN = SelectTopN;
            srcMedia.FilterName = ID;
            srcMedia.SourceFilterName = FilterName;
            srcMedia.CacheItemName = CacheItemName;
            srcMedia.CacheDependencies = CacheDependencies;
            srcMedia.CacheMinutes = CacheMinutes;
            srcMedia.SelectedColumns = Columns;
            srcMedia.LibraryName = LibraryName;
            srcMedia.FilePath = FilePath;
            srcMedia.FileExtensions = FileExtensions;
            srcMedia.CheckPermissions = CheckPermissions;
            srcMedia.ShowGroupFiles = ShowGroupFiles;
            srcMedia.OnFilterChanged += srcMedia_OnFilterChanged;

            int fid = QueryHelper.GetInteger(FileIDQueryStringKey, 0);
            if (fid > 0)
            {
                srcMedia.WhereCondition = "FileID = " + fid;
            }
        }
    }


    private void srcMedia_OnFilterChanged()
    {
        srcMedia.InvalidateLoadedData();
        srcMedia.WhereCondition = WhereCondition;
        srcMedia.OrderBy = OrderBy;
        srcMedia.TopN = SelectTopN;
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcMedia.ClearCache();
    }

    #endregion
}