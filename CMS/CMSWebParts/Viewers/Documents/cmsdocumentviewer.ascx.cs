using System;
using System.Web.UI;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_cmsdocumentviewer : CMSAbstractWebPart
{
    #region "Private properties"

    /// <summary>
    /// Indicates whether selected transformation is ASCX or XSLT.
    /// </summary>
    private bool isAscx = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Repeater control.
    /// </summary>
    public CMSRepeater RepeaterControl
    {
        get
        {
            return repElem;
        }
    }


    /// <summary>
    /// Viewer control.
    /// </summary>
    public CMSViewer ViewerControl
    {
        get
        {
            return viewElem;
        }
    }

    #endregion


    #region "Relationships properties"

    /// <summary>
    /// Gets or sets the value that indicates whether related node is on the left side.
    /// </summary>
    public bool RelatedNodeIsOnTheLeftSide
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), viewElem.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            viewElem.RelatedNodeIsOnTheLeftSide = value;
            repElem.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RelationshipName"), viewElem.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            viewElem.RelationshipName = value;
            repElem.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), viewElem.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            viewElem.RelationshipWithNodeGuid = value;
            repElem.RelationshipWithNodeGuid = value;
        }
    }

    #endregion


    #region "Basic repeater/viewer properties"

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
            viewElem.CacheItemName = value;
            repElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, repElem.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            viewElem.CacheDependencies = value;
            repElem.CacheDependencies = value;
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
            viewElem.CacheMinutes = value;
            repElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), viewElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            viewElem.CheckPermissions = value;
            repElem.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ClassNames"), viewElem.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            viewElem.ClassNames = value;
            repElem.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names which should be displayed.
    /// </summary>
    public string CategoryName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CategoryName"), viewElem.CategoryName);
        }
        set
        {
            SetValue("CategoryName", value);
            viewElem.CategoryName = value;
            repElem.CategoryName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), viewElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            viewElem.CombineWithDefaultCulture = value;
            repElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CultureCode"), viewElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            viewElem.CultureCode = value;
            repElem.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), viewElem.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            viewElem.MaxRelativeLevel = value;
            repElem.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("OrderBy"), viewElem.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            viewElem.OrderBy = value;
            repElem.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), null);
        }
        set
        {
            SetValue("Path", value);
            viewElem.Path = value;
            repElem.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), viewElem.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            viewElem.SelectOnlyPublished = value;
            repElem.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), viewElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            viewElem.SiteName = value;
            repElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), viewElem.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            repElem.SelectTopN = value;
            viewElem.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), viewElem.SelectedColumns);
        }
        set
        {
            SetValue("Columns", value);
            repElem.Columns = value;
            viewElem.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), viewElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            viewElem.WhereCondition = value;
            repElem.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), viewElem.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            viewElem.TransformationName = value;
            repElem.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), viewElem.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            viewElem.SelectedItemTransformationName = value;
            repElem.SelectedItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the alternate results.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternatingItemTransformationName"), repElem.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingItemTransformationName", value);
            repElem.AlternatingTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), viewElem.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            viewElem.HideControlForZeroRows = value;
            repElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), viewElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            viewElem.ZeroRowsText = value;
            repElem.ZeroRowsText = value;
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), repElem.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            repElem.FilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), repElem.EnablePaging);
        }
        set
        {
            SetValue("EnablePaging", value);
            repElem.EnablePaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager position.
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return repElem.PagerControl.GetPagerPosition(DataHelper.GetNotEmpty(GetValue("PagerPosition"), repElem.PagerControl.PagerPosition.ToString()));
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            repElem.PagerControl.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of the documents displayed on each sigle page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), repElem.PagerControl.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            repElem.PagerControl.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the HTML text before pager.
    /// </summary>
    public string PagerHTMLBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLBefore"), repElem.PagerControl.PagerHTMLBefore);
        }
        set
        {
            SetValue("PagerHTMLBefore", value);
            repElem.PagerControl.PagerHTMLBefore = value;
        }
    }


    /// <summary>
    /// Gets or sets the HTML text after pager.
    /// </summary>
    public string PagerHTMLAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLAfter"), repElem.PagerControl.PagerHTMLAfter);
        }
        set
        {
            SetValue("PagerHTMLAfter", value);
            repElem.PagerControl.PagerHTMLAfter = value;
        }
    }


    /// <summary>
    /// Gets or sets the page numbers separator.
    /// </summary>
    public string PageNumbersSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageNumbersSeparator"), repElem.PagerControl.PageNumbersSeparator);
        }
        set
        {
            SetValue("PageNumbersSeparator", value);
            repElem.PagerControl.PageNumbersSeparator = value;
        }
    }


    /// <summary>
    /// Gets or sets the results position.
    /// </summary>
    public ResultsLocationTypeEnum ResultsPosition
    {
        get
        {
            return repElem.PagerControl.GetResultPosition(DataHelper.GetNotEmpty(GetValue("ResultsPosition"), repElem.PagerControl.ResultsLocation.ToString()));
        }
        set
        {
            SetValue("ResultsPosition", value);
            repElem.PagerControl.ResultsLocation = value;
        }
    }


    /// <summary>
    /// Gets or sets the navigation mode.
    /// </summary>
    public BackNextLocationTypeEnum BackNextLocation
    {
        get
        {
            return repElem.PagerControl.GetBackNextLocation(DataHelper.GetNotEmpty(GetValue("BackNextLocation"), repElem.PagerControl.BackNextLocation.ToString()));
        }
        set
        {
            SetValue("BackNextLocation", value);
            repElem.PagerControl.BackNextLocation = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("QueryStringKey"), repElem.PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            repElem.PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagingModeTypeEnum PagingMode
    {
        get
        {
            return repElem.PagerControl.GetPagingMode(DataHelper.GetNotEmpty(GetValue("PagingMode"), repElem.PagerControl.PagingMode.ToString()));
        }
        set
        {
            SetValue("PagingMode", value.ToString());
            repElem.PagerControl.PagingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether  first and last page is shown if paging is allowed.
    /// </summary>
    public bool ShowFirstLast
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFirstLast"), repElem.PagerControl.ShowFirstLast);
        }
        set
        {
            SetValue("ShowFirstLast", value);
            repElem.PagerControl.ShowFirstLast = value;
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
            viewElem.StopProcessing = value;
            repElem.StopProcessing = value;
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
            viewElem.StopProcessing = true;
            repElem.StopProcessing = true;
        }
        else
        {
            // Get transformation name
            string transformationName = String.IsNullOrEmpty(TransformationName) ? SelectedItemTransformationName : TransformationName;
            if (!string.IsNullOrEmpty(transformationName))
            {
                // Get transformation info
                TransformationInfo transformation = TransformationInfoProvider.GetTransformation(transformationName);
                // Get type of transformation
                if (transformation != null)
                {
                    if (transformation.TransformationType == TransformationTypeEnum.Xslt)
                    {
                        // XSLT transformations (transform locally)
                        isAscx = false;
                        UseXSLT();
                    }
                    else
                    {
                        // Other transformations (display using repeater)
                        UseStandard();
                    }
                }
            }
        }

    }


    /// <summary>
    /// Use CMSViewer for XSLT transformation.
    /// </summary>
    protected void UseXSLT()
    {
        repElem.StopProcessing = true;

        viewElem.ControlContext = ControlContext;

        // Basic control properties
        viewElem.HideControlForZeroRows = HideControlForZeroRows;
        viewElem.ZeroRowsText = ZeroRowsText;

        // Set properties from Webpart form   
        viewElem.CacheItemName = CacheItemName;
        viewElem.CacheDependencies = CacheDependencies;
        viewElem.CacheMinutes = CacheMinutes;
        viewElem.CheckPermissions = CheckPermissions;
        viewElem.ClassNames = ClassNames;
        viewElem.CategoryName = CategoryName;
        viewElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
        viewElem.CultureCode = CultureCode;
        viewElem.MaxRelativeLevel = MaxRelativeLevel;
        viewElem.OrderBy = OrderBy;
        viewElem.WhereCondition = WhereCondition;
        viewElem.SelectOnlyPublished = SelectOnlyPublished;
        viewElem.SiteName = SiteName;
        viewElem.Path = Path;
        viewElem.SelectTopN = SelectTopN;
        viewElem.SelectedColumns = Columns;

        // Set relationship properties
        viewElem.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
        viewElem.RelationshipName = RelationshipName;
        viewElem.RelationshipWithNodeGuid = RelationshipWithNodeGUID;

        // Apply transformations
        viewElem.TransformationName = TransformationName;
        viewElem.SelectedItemTransformationName = SelectedItemTransformationName;
    }


    /// <summary>
    /// Use CMSRepeater for standard transformations.
    /// </summary>
    protected void UseStandard()
    {
        viewElem.StopProcessing = true;

        repElem.ControlContext = ControlContext;

        // Basic control properties
        repElem.HideControlForZeroRows = HideControlForZeroRows;
        repElem.ZeroRowsText = ZeroRowsText;

        // Set properties from Webpart form   
        repElem.CacheItemName = CacheItemName;
        repElem.CacheDependencies = CacheDependencies;
        repElem.CacheMinutes = CacheMinutes;
        repElem.CheckPermissions = CheckPermissions;
        repElem.ClassNames = ClassNames;
        repElem.CategoryName = CategoryName;
        repElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
        repElem.CultureCode = CultureCode;
        repElem.MaxRelativeLevel = MaxRelativeLevel;
        repElem.OrderBy = OrderBy;
        repElem.Path = Path;
        repElem.WhereCondition = WhereCondition;
        repElem.SelectOnlyPublished = SelectOnlyPublished;
        repElem.SiteName = SiteName;
        repElem.SelectTopN = SelectTopN;
        repElem.Columns = Columns;
        repElem.FilterName = FilterName;

        // Set relationship properties
        repElem.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
        repElem.RelationshipName = RelationshipName;
        repElem.RelationshipWithNodeGuid = RelationshipWithNodeGUID;

        // Apply transformations
        repElem.TransformationName = TransformationName;
        repElem.SelectedItemTransformationName = SelectedItemTransformationName;
        repElem.AlternatingTransformationName = AlternatingTransformationName;

        // Set pager properties
        repElem.EnablePaging = EnablePaging;
        repElem.PagerControl.PagerPosition = PagerPosition;
        repElem.PageSize = PageSize;
        repElem.PagerControl.PagingMode = PagingMode;
        repElem.PagerControl.BackNextLocation = BackNextLocation;
        repElem.PagerControl.QueryStringKey = QueryStringKey;
        repElem.PagerControl.ShowFirstLast = ShowFirstLast;
        repElem.PagerControl.PagerHTMLBefore = PagerHTMLBefore;
        repElem.PagerControl.PagerHTMLAfter = PagerHTMLAfter;
        repElem.PagerControl.PageNumbersSeparator = PageNumbersSeparator;
        repElem.PagerControl.ResultsLocation = ResultsPosition;
    }

    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);
    }

    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Get type of transformation
        if (isAscx)
        {
            Visible = repElem.Visible && !StopProcessing;

            if (DataHelper.DataSourceIsEmpty(repElem.DataSource) && (HideControlForZeroRows))
            {
                Visible = false;
            }
        }
        else
        {
            Visible = viewElem.Visible && !StopProcessing;

            if (DataHelper.DataSourceIsEmpty(viewElem.DataSource) && (HideControlForZeroRows))
            {
                Visible = false;
            }
        }
    }


    /// <summary>
    /// Reload data override.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        repElem.ReloadData(true);
    }


    /// <summary>
    /// Clear cache.
    /// </summary>
    public override void ClearCache()
    {
        repElem.ClearCache();
    }
}