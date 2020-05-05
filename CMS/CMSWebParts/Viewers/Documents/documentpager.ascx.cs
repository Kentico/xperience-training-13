using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_documentpager : CMSAbstractWebPart
{
    #region "Document properties"

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
            repItems.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, repItems.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            repItems.CacheDependencies = value;
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
            repItems.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), repItems.ClassNames), repItems.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            repItems.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), repItems.TransformationName), repItems.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            repItems.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), repItems.CultureCode), repItems.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            repItems.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), repItems.Path);
        }
        set
        {
            SetValue("Path", value);
            repItems.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), repItems.SiteName), repItems.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            repItems.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), repItems.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            repItems.CheckPermissions = value;
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), repItems.EnablePaging);
        }
        set
        {
            SetValue("EnablePaging", value);
            repItems.EnablePaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager position.
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return repItems.PagerControl.GetPagerPosition(DataHelper.GetNotEmpty(GetValue("PagerPosition"), repItems.PagerControl.PagerPosition.ToString()));
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            repItems.PagerControl.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of the documents displayed on each sigle page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), repItems.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            repItems.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("QueryStringKey"), repItems.PagerControl.QueryStringKey), repItems.PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            repItems.PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagingModeTypeEnum PagingMode
    {
        get
        {
            return repItems.PagerControl.GetPagingMode(DataHelper.GetNotEmpty(GetValue("PagingMode"), repItems.PagerControl.PagingMode.ToString()));
        }
        set
        {
            SetValue("PagingMode", value.ToString());
            repItems.PagerControl.PagingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether  first and last page is shown if paging is allowed.
    /// </summary>
    public bool ShowFirstLast
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFirstLast"), repItems.PagerControl.ShowFirstLast);
        }
        set
        {
            SetValue("ShowFirstLast", value);
            repItems.PagerControl.ShowFirstLast = value;
        }
    }

    #endregion


    #region "Other pager properties"

    /// <summary>
    /// Gets or sets the back text.
    /// </summary>
    public string BackText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("BackText"), repItems.PagerControl.BackText);
        }
        set
        {
            SetValue("BackText", value);
            repItems.PagerControl.BackText = value;
        }
    }


    /// <summary>
    /// Gets or sets the next text.
    /// </summary>
    public string NextText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NextText"), repItems.PagerControl.NextText);
        }
        set
        {
            SetValue("NextText", value);
            repItems.PagerControl.NextText = value;
        }
    }


    /// <summary>
    /// Gets or sets the back next display type.
    /// </summary>
    public BackNextDisplayTypeEnum BackNextDisplay
    {
        get
        {
            return repItems.PagerControl.BackNextDisplay;
        }
        set
        {
            repItems.PagerControl.BackNextDisplay = value;
        }
    }


    /// <summary>
    /// Gets or sets the page numbers display mode.
    /// </summary>
    public PageNumbersDisplayTypeEnum PageNumbersDisplay
    {
        get
        {
            return repItems.PagerControl.PageNumbersDisplay;
        }
        set
        {
            repItems.PagerControl.PageNumbersDisplay = value;
        }
    }


    /// <summary>
    /// Gets or sets the result locations.
    /// </summary>
    public ResultsLocationTypeEnum ResultsLocation
    {
        get
        {
            return repItems.PagerControl.ResultsLocation;
        }
        set
        {
            repItems.PagerControl.ResultsLocation = value;
        }
    }

    #endregion


    #region "Class properties"

    /// <summary>
    /// Gets or sets the selected page CSS class.
    /// </summary>
    public string SelectedClass
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedPageClass"), repItems.PagerControl.SelectedClass);
        }
        set
        {
            SetValue("SelectedPageClass", value);
            repItems.PagerControl.SelectedClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the unselected page CSS class.
    /// </summary>
    public string UnselectedClass
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("UnselectedPageClass"), repItems.PagerControl.UnselectedClass);
        }
        set
        {
            SetValue("UnselectedPageClass", value);
            repItems.PagerControl.UnselectedClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected next CSS class.
    /// </summary>
    public string SelectedNextClass
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedNextClass"), repItems.PagerControl.SelectedNextClass);
        }
        set
        {
            SetValue("SelectedNextClass", value);
            repItems.PagerControl.SelectedNextClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected previous CSS class.
    /// </summary>
    public string SelectedPrevClass
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedPrevClass"), repItems.PagerControl.SelectedPrevClass);
        }
        set
        {
            SetValue("SelectedPrevClass", value);
            repItems.PagerControl.SelectedPrevClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the unselected next CSS class.
    /// </summary>
    public string UnselectedNextClass
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("UnselectedNextClass"), repItems.PagerControl.UnselectedNextClass);
        }
        set
        {
            SetValue("UnselectedNextClass", value);
            repItems.PagerControl.UnselectedNextClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the unselected previous CSS class.
    /// </summary>
    public string UnselectedPrevClass
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("UnselectedPrevClass"), repItems.PagerControl.UnselectedPrevClass);
        }
        set
        {
            SetValue("UnselectedPrevClass", value);
            repItems.PagerControl.UnselectedPrevClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the page number separator.
    /// </summary>
    public string PageNumbersSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageNumberSeparator"), repItems.PagerControl.PageNumbersSeparator);
        }
        set
        {
            SetValue("PageNumberSeparator", value);
            repItems.PagerControl.PageNumbersSeparator = value;
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
            repItems.StopProcessing = value;
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
            repItems.StopProcessing = true;
        }
        else
        {
            repItems.ControlContext = ControlContext;

            // Set properties from Webpart form
            repItems.Path = Path;
            repItems.SiteName = SiteName;
            repItems.CultureCode = CultureCode;
            repItems.ClassNames = ClassNames;

            repItems.CacheItemName = CacheItemName;
            repItems.CacheDependencies = CacheDependencies;
            repItems.CacheMinutes = CacheMinutes;
            repItems.TransformationName = TransformationName;

            repItems.CheckPermissions = CheckPermissions;

            repItems.PagerControl.BackText = BackText;
            repItems.PagerControl.NextText = NextText;
            repItems.PagerControl.PagerPosition = PagerPosition;

            EnablePaging = true;
            PageSize = 1;
            repItems.OrderBy = "NodeOrder";

            BackNextDisplay = BackNextDisplayTypeEnum.HyperLinks;
            PageNumbersDisplay = PageNumbersDisplayTypeEnum.Numbers;

            ShowFirstLast = false;
            ResultsLocation = ResultsLocationTypeEnum.None;

            repItems.PagerControl.SelectedClass = SelectedClass;
            repItems.PagerControl.UnselectedClass = UnselectedClass;

            repItems.PagerControl.SelectedNextClass = SelectedNextClass;
            repItems.PagerControl.SelectedPrevClass = SelectedPrevClass;

            repItems.PagerControl.UnselectedNextClass = UnselectedNextClass;
            repItems.PagerControl.UnselectedPrevClass = UnselectedPrevClass;

            repItems.PagerControl.PageNumbersSeparator = PageNumbersSeparator;
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = repItems.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(repItems.DataSource) && repItems.HideControlForZeroRows)
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        repItems.ReloadData(true);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        repItems.ClearCache();
    }
}