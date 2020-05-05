using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

using DataPager = CMS.DocumentEngine.Web.UI.DataPager;


public partial class CMSWebParts_Viewers_Documents_cmsdatalist : CMSAbstractWebPart
{
    #region "Document properties"

    protected CMSDocumentsDataSource mDataSourceControl = null;
    
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
            lstElem.LoadPagesIndividually = value;
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
            lstElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, lstElem.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            lstElem.CacheDependencies = value;
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
            lstElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), lstElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            lstElem.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Classnames"), lstElem.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            lstElem.ClassNames = value;
        }
    }


    /// <summary>
    /// Code name of the category to display documents from.
    /// </summary>
    public string CategoryName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoryName"), lstElem.CategoryName);
        }
        set
        {
            SetValue("CategoryName", value);
            lstElem.CategoryName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), lstElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            lstElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CultureCode"), lstElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            lstElem.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), lstElem.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            lstElem.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("OrderBy"), lstElem.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            lstElem.OrderBy = value;
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
            lstElem.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), lstElem.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            lstElem.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether filter out duplicate documents.
    /// </summary>
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), lstElem.FilterOutDuplicates);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            lstElem.FilterOutDuplicates = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), lstElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            lstElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), lstElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            lstElem.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), lstElem.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            lstElem.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), lstElem.Columns);
        }
        set
        {
            SetValue("Columns", value);
            lstElem.Columns = value;
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Gets DataPager instance.
    /// </summary>
    public DataPager PagerControl
    {
        get
        {
            return lstElem.PagerControl;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), lstElem.EnablePaging);
        }
        set
        {
            SetValue("EnablePaging", value);
            lstElem.EnablePaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager position.
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return PagerControl.GetPagerPosition(DataHelper.GetNotEmpty(GetValue("PagerPosition"), PagerControl.PagerPosition.ToString()));
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            PagerControl.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of the documents displayed on each sigle page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), lstElem.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            lstElem.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("QueryStringKey"), PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagingModeTypeEnum PagingMode
    {
        get
        {
            return PagerControl.GetPagingMode(DataHelper.GetNotEmpty(GetValue("PagingMode"), PagerControl.PagingMode.ToString()));
        }
        set
        {
            SetValue("PagingMode", value.ToString());
            PagerControl.PagingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the navigation mode.
    /// </summary>
    public BackNextLocationTypeEnum BackNextLocation
    {
        get
        {
            return PagerControl.GetBackNextLocation(DataHelper.GetNotEmpty(GetValue("BackNextLocation"), PagerControl.BackNextLocation.ToString()));
        }
        set
        {
            SetValue("BackNextLocation", value.ToString());
            PagerControl.BackNextLocation = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether  first and last page is shown if paging is allowed.
    /// </summary>
    public bool ShowFirstLast
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFirstLast"), PagerControl.ShowFirstLast);
        }
        set
        {
            SetValue("ShowFirstLast", value);
            PagerControl.ShowFirstLast = value;
        }
    }


    /// <summary>
    /// Gets or sets the html before pager.
    /// </summary>
    public string PagerHTMLBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLBefore"), PagerControl.PagerHTMLBefore);
        }
        set
        {
            SetValue("PagerHTMLBefore", value);
            PagerControl.PagerHTMLBefore = value;
        }
    }


    /// <summary>
    /// Gets or sets the html after pager.
    /// </summary>
    public string PagerHTMLAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLAfter"), PagerControl.PagerHTMLAfter);
        }
        set
        {
            SetValue("PagerHTMLAfter", value);
            PagerControl.PagerHTMLAfter = value;
        }
    }


    /// <summary>
    /// Gets or sets the results position.
    /// </summary>
    public ResultsLocationTypeEnum ResultsPosition
    {
        get
        {
            return PagerControl.GetResultPosition(ValidationHelper.GetString(GetValue("ResultsPosition"), ""));
        }
        set
        {
            SetValue("ResultsPosition", value);
            PagerControl.ResultsLocation = value;
        }
    }


    /// <summary>
    /// Gets or sets page numbers separator.
    /// </summary>
    public string PageNumbersSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageNumbersSeparator"), PagerControl.PageNumbersSeparator);
        }
        set
        {
            SetValue("PageNumbersSeparator", value);
            PagerControl.PageNumbersSeparator = value;
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
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), lstElem.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            lstElem.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("RelationshipName"), lstElem.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            lstElem.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), lstElem.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            lstElem.RelationshipWithNodeGuid = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("TransformationName"), lstElem.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            lstElem.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedItemTransformationName"), lstElem.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            lstElem.SelectedItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the alternate results.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("AlternatingTransformationName"), lstElem.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
            lstElem.AlternatingTransformationName = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the nested controls IDs. Use ';' like separator.
    /// </summary>
    public string NestedControlsID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NestedControlsID"), lstElem.NestedControlsID);
        }
        set
        {
            SetValue("NestedControlsID", value);
            lstElem.NestedControlsID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), lstElem.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            lstElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ZeroRowsText"), lstElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            lstElem.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the count of repeat columns.
    /// </summary>
    public int RepeatColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RepeatColumns"), lstElem.RepeatColumns);
        }
        set
        {
            SetValue("RepeatColumns", value);
            lstElem.RepeatColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets whether control is displayed in a table or flow layout.
    /// </summary>
    public RepeatLayout RepeatLayout
    {
        get
        {
            return CMSDataList.GetRepeatLayout(DataHelper.GetNotEmpty(GetValue("RepeatLayout"), lstElem.RepeatLayout.ToString()));
        }
        set
        {
            SetValue("RepeatLayout", value.ToString());
            lstElem.RepeatLayout = value;
        }
    }


    /// <summary>
    /// Gets or sets whether DataList control displays vertically or horizontally.
    /// </summary>
    public RepeatDirection RepeatDirection
    {
        get
        {
            return CMSDataList.GetRepeatDirection(DataHelper.GetNotEmpty(GetValue("RepeatDirection"), lstElem.RepeatDirection.ToString()));
        }
        set
        {
            SetValue("RepeatDirection", value.ToString());
            lstElem.RepeatDirection = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), lstElem.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            lstElem.FilterName = value;
        }
    }


    /// <summary>
    /// Data source name.
    /// </summary>
    public string DataSourceName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataSourceName"), lstElem.DataSourceName);
        }
        set
        {
            SetValue("DataSourceName", value);
            lstElem.DataSourceName = value;
        }
    }


    /// <summary>
    /// Control with data source.
    /// </summary>
    public CMSDocumentsDataSource DataSourceControl
    {
        get
        {
            return mDataSourceControl;
        }
        set
        {
            mDataSourceControl = value;
            lstElem.DataSourceControl = value;
        }
    }


    /// <summary>
    /// DataList control.
    /// </summary>
    public CMSDataList DataListControl
    {
        get
        {
            return lstElem;
        }
    }

    #endregion


    #region "Editing mode buttons properties"

    /// <summary>
    /// Gets or sets the value that inidcates whether 'New button' should be displayed.
    /// </summary>
    public bool ShowNewButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNewButton"), btnAdd.Visible);
        }
        set
        {
            SetValue("ShowNewButton", value);
            btnAdd.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the new button text.
    /// </summary>
    public string NewButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NewButtonText"), btnAdd.Text);
        }
        set
        {
            SetValue("NewButtonText", value);
            btnAdd.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether edit and delete buttons should be displayed.
    /// </summary>
    public bool ShowEditDeleteButtons
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEditDeleteButtons"), lstElem.ShowEditDeleteButtons);
        }
        set
        {
            SetValue("ShowEditDeleteButtons", value);
            lstElem.ShowEditDeleteButtons = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether delete buttons should be displayed.
    /// </summary>
    public bool ShowDeleteButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowDeleteButton"), ShowEditDeleteButtons);
        }
        set
        {
            SetValue("ShowDeleteButton", value);
            lstElem.ShowDeleteButton = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether edit buttons should be displayed.
    /// </summary>
    public bool ShowEditButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEditButton"), ShowEditDeleteButtons);
        }
        set
        {
            SetValue("ShowEditButton", value);
            lstElem.ShowEditButton = value;
        }
    }

    #endregion


    #region "Data binding properties"

    /// <summary>
    /// Gets or sets the value that indicates whether databind will be proceeded automatically.
    /// </summary>
    public bool DataBindByDefault
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DataBindByDefault"), lstElem.DataBindByDefault);
        }
        set
        {
            SetValue("DataBindByDefault", value);
            lstElem.DataBindByDefault = value;
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
            lstElem.StopProcessing = value;
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
        // In design mode is pocessing of control stoped
        if (StopProcessing)
        {
            lstElem.StopProcessing = true;
        }
        else
        {
            lstElem.ControlContext = ControlContext;

            // Document properties
            lstElem.LoadPagesIndividually = LoadPagesIndividually;
            lstElem.NestedControlsID = NestedControlsID;
            lstElem.CacheItemName = CacheItemName;
            lstElem.CacheDependencies = CacheDependencies;
            lstElem.CacheMinutes = CacheMinutes;
            lstElem.CheckPermissions = CheckPermissions;
            lstElem.ClassNames = ClassNames;
            lstElem.CategoryName = CategoryName;
            lstElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            lstElem.FilterOutDuplicates = FilterOutDuplicates;
            lstElem.CultureCode = CultureCode;
            lstElem.Path = Path;
            lstElem.MaxRelativeLevel = MaxRelativeLevel;
            lstElem.OrderBy = OrderBy;
            lstElem.SelectTopN = SelectTopN;
            lstElem.Columns = Columns;
            lstElem.SelectOnlyPublished = SelectOnlyPublished;
            lstElem.SiteName = SiteName;
            lstElem.WhereCondition = WhereCondition;

            // Relationships
            lstElem.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
            lstElem.RelationshipName = RelationshipName;
            lstElem.RelationshipWithNodeGuid = RelationshipWithNodeGUID;

            // Transformations
            lstElem.SelectedItemTransformationName = SelectedItemTransformationName;
            lstElem.AlternatingTransformationName = AlternatingTransformationName;
            lstElem.TransformationName = TransformationName;

            // Data source settings - must be before pager settings
            lstElem.DataSourceName = DataSourceName;
            lstElem.DataSourceControl = DataSourceControl;

            // Pager
            lstElem.EnablePaging = EnablePaging;
            lstElem.PageSize = PageSize;
            PagerControl.PagerPosition = PagerPosition;
            PagerControl.QueryStringKey = QueryStringKey;
            PagerControl.PagingMode = PagingMode;
            PagerControl.BackNextLocation = BackNextLocation;
            PagerControl.ShowFirstLast = ShowFirstLast;
            PagerControl.PagerHTMLBefore = PagerHTMLBefore;
            PagerControl.PagerHTMLAfter = PagerHTMLAfter;
            PagerControl.ResultsLocation = ResultsPosition;
            PagerControl.PageNumbersSeparator = PageNumbersSeparator;

            // Public
            lstElem.RepeatColumns = RepeatColumns;
            lstElem.RepeatLayout = RepeatLayout;
            lstElem.RepeatDirection = RepeatDirection;
            lstElem.HideControlForZeroRows = HideControlForZeroRows;
            lstElem.ZeroRowsText = ZeroRowsText;
            lstElem.FilterName = FilterName;

            // Binding
            lstElem.DataBindByDefault = DataBindByDefault;

            // Editing buttons
            lstElem.ShowEditDeleteButtons = ShowEditDeleteButtons;
            lstElem.ShowEditButton = ShowEditButton;
            lstElem.ShowDeleteButton = ShowDeleteButton;

            // CMSEditModeButtonAdd
            btnAdd.Visible = ShowNewButton;
            btnAdd.Text = NewButtonText;

            string[] mClassNames = lstElem.ClassNames.Split(';');
            btnAdd.ClassName = DataHelper.GetNotEmpty(mClassNames[0], "");

            string mPath = "";
            if (lstElem.Path.EndsWithCSafe("/%"))
            {
                mPath = lstElem.Path.Remove(lstElem.Path.Length - 2);
            }
            if (lstElem.Path.EndsWithCSafe("/"))
            {
                mPath = lstElem.Path.Remove(lstElem.Path.Length - 1);
            }

            btnAdd.Path = DataHelper.GetNotEmpty(mPath, "");

            // Add repeater to the filter collection
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), lstElem);
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = lstElem.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(lstElem.DataSource) && HideControlForZeroRows)
        {
            Visible = false;
        }

        // Hide the Add button for selected items which have the SelectedItem transformation specified
        if (ShowNewButton && !String.IsNullOrEmpty(lstElem.SelectedItemTransformationName) && lstElem.IsSelected)
        {
            btnAdd.Visible = false;
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        lstElem.ReloadData(true);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        lstElem.ClearCache();
    }
}