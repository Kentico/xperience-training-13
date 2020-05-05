using System;

using CMS.Base;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_CMSRepeaterWithEffect : CMSAbstractWebPart
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
            repItems.LoadPagesIndividually = value;
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


    /// <summary>
    /// Gets or sets the class names which should be displayed.
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
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), repItems.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            repItems.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether filter out duplicate documents.
    /// </summary>
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), repItems.FilterOutDuplicates);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            repItems.FilterOutDuplicates = value;
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
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), repItems.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            repItems.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), repItems.OrderBy), repItems.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            repItems.OrderBy = value;
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
            repItems.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), repItems.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            repItems.SelectOnlyPublished = value;
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
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), repItems.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            repItems.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), repItems.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            repItems.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), repItems.Columns);
        }
        set
        {
            SetValue("Columns", value);
            repItems.Columns = value;
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
    /// Gets or sets the navigation mode.
    /// </summary>
    public BackNextLocationTypeEnum BackNextLocation
    {
        get
        {
            return repItems.PagerControl.GetBackNextLocation(DataHelper.GetNotEmpty(GetValue("BackNextLocation"), repItems.PagerControl.BackNextLocation.ToString()));
        }
        set
        {
            SetValue("BackNextLocation", value.ToString());
            repItems.PagerControl.BackNextLocation = value;
        }
    }


    /// <summary>
    /// Gets or sets the html before pager.
    /// </summary>
    public string PagerHTMLBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLBefore"), repItems.PagerControl.PagerHTMLBefore);
        }
        set
        {
            SetValue("PagerHTMLBefore", value);
            repItems.PagerControl.PagerHTMLBefore = value;
        }
    }


    /// <summary>
    /// Gets or sets the html after pager.
    /// </summary>
    public string PagerHTMLAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLAfter"), repItems.PagerControl.PagerHTMLAfter);
        }
        set
        {
            SetValue("PagerHTMLAfter", value);
            repItems.PagerControl.PagerHTMLAfter = value;
        }
    }


    /// <summary>
    /// Gets or sets the results position.
    /// </summary>
    public ResultsLocationTypeEnum ResultsPosition
    {
        get
        {
            return repItems.PagerControl.GetResultPosition(ValidationHelper.GetString(GetValue("ResultsPosition"), ""));
        }
        set
        {
            SetValue("ResultsPosition", value);
            repItems.PagerControl.ResultsLocation = value;
        }
    }


    /// <summary>
    /// Gets or sets the page numbers separator.
    /// </summary>
    public string PageNumbersSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageNumbersSeparator"), repItems.PagerControl.PageNumbersSeparator);
        }
        set
        {
            SetValue("PageNumbersSeparator", value);
            repItems.PagerControl.PageNumbersSeparator = value;
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


    #region "Relationships properties"

    /// <summary>
    /// Gets or sets the value that indicates whether related node is on the left side.
    /// </summary>
    public bool RelatedNodeIsOnTheLeftSide
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), repItems.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            repItems.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("RelationshipName"), repItems.RelationshipName), repItems.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            repItems.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), repItems.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            repItems.RelationshipWithNodeGuid = value;
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
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), repItems.TransformationName), repItems.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            repItems.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), repItems.SelectedItemTransformationName), repItems.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            repItems.SelectedItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the alternate results.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("AlternatingTransformationName"), repItems.AlternatingTransformationName), repItems.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
            repItems.AlternatingTransformationName = value;
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
            return ValidationHelper.GetString(GetValue("NestedControlsID"), repItems.NestedControlsID);
        }
        set
        {
            SetValue("NestedControlsID", value);
            repItems.NestedControlsID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), repItems.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            repItems.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows results.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), repItems.ZeroRowsText), repItems.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            repItems.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator (tetx, html code) which is displayed between displayed items.
    /// </summary>
    public string ItemSeparator
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ItemSeparator"), repItems.ItemSeparator);
        }
        set
        {
            SetValue("ItemSeparator", value);
            repItems.ItemSeparator = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), repItems.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            repItems.FilterName = value;
        }
    }


    /// <summary>
    /// Data source name.
    /// </summary>
    public string DataSourceName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataSourceName"), repItems.DataSourceName);
        }
        set
        {
            SetValue("DataSourceName", value);
            repItems.DataSourceName = value;
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
            repItems.DataSourceControl = value;
        }
    }


    /// <summary>
    /// Repeater control.
    /// </summary>
    public CMSRepeaterWithEffect RepeaterControl
    {
        get
        {
            return repItems;
        }
    }

    #endregion


    #region "Effect & Layout properties"

    /// <summary>
    /// Content before the generated items.
    /// </summary>
    public string RepeaterHTMLBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RepeaterHTMLBefore"), "");
        }
        set
        {
            SetValue("RepeaterHTMLBefore", value);
            repItems.RepeaterHTMLBefore = value;
        }
    }


    /// <summary>
    /// Content after the generated items.
    /// </summary>
    public string RepeaterHTMLAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RepeaterHTMLAfter"), "");
        }
        set
        {
            SetValue("RepeaterHTMLAfter", value);
            repItems.RepeaterHTMLAfter = value;
        }
    }


    /// <summary>
    /// Content before each item.
    /// </summary>
    public string ItemHTMLBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemHTMLBefore"), "");
        }
        set
        {
            SetValue("ItemHTMLBefore", value);
            repItems.ItemHTMLBefore = value;
        }
    }


    /// <summary>
    /// Content after each item.
    /// </summary>
    public string ItemHTMLAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemHTMLAfter"), "");
        }
        set
        {
            SetValue("ItemHTMLAfter", value);
            repItems.ItemHTMLAfter = value;
        }
    }


    /// <summary>
    /// Content after each item.
    /// </summary>
    public string ItemHTMLSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemHTMLSeparator"), "");
        }
        set
        {
            SetValue("ItemHTMLSeparator", value);
            repItems.ItemHTMLSeparator = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to hide layout (Content before, Content after) when no data found.
    /// </summary>
    public bool HideLayoutForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideLayoutForZeroRows"), false);
        }
        set
        {
            SetValue("HideLayoutForZeroRows", value);
            repItems.HideLayoutForZeroRows = value;
        }
    }


    /// <summary>
    /// Script files.
    /// </summary>
    public string ScriptFiles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ScriptFiles"), "");
        }
        set
        {
            SetValue("ScriptFiles", value);
            repItems.ScriptFiles = value;
        }
    }


    /// <summary>
    /// Initialization script.
    /// </summary>
    public string InitScript
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InitScript"), "");
        }
        set
        {
            SetValue("InitScript", value);
            repItems.InitScript = value;
        }
    }


    /// <summary>
    /// Additional CSS files.
    /// </summary>
    public string CSSFiles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CSSFiles"), "");
        }
        set
        {
            SetValue("CSSFiles", value);
            repItems.CSSFiles = value;
        }
    }


    /// <summary>
    /// Inline CSS styles.
    /// </summary>
    public string InlineCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InlineCSS"), "");
        }
        set
        {
            SetValue("InlineCSS", value);
            repItems.InlineCSS = value;
        }
    }

    #endregion


    #region "Editing mode buttons properties"

    /// <summary>
    /// Gets or sets the value that indicates whether 'New button' should be displayed (New button cannot be displayed in the live site).
    /// </summary>
    public bool ShowNewButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNewButton"), btnAdd.Visible);
        }
        set
        {
            bool val = value;

            if (PageManager.ViewMode.IsLiveSite())
            {
                val = false;
            }

            SetValue("ShowNewButton", val);
            btnAdd.Visible = val;
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
            return ValidationHelper.GetBoolean(GetValue("ShowEditDeleteButtons"), repItems.ShowEditDeleteButtons);
        }
        set
        {
            SetValue("ShowEditDeleteButtons", value);
            repItems.ShowEditDeleteButtons = value;
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
            repItems.ShowDeleteButton = value;
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
            repItems.ShowEditButton = value;
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
            btnAdd.StopProcessing = value;
        }
    }

    #endregion


    #region "Page methods"

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
            btnAdd.StopProcessing = true;
        }
        else
        {
            repItems.ControlContext = ControlContext;

            // Document properties
            repItems.LoadPagesIndividually = LoadPagesIndividually;
            repItems.NestedControlsID = NestedControlsID;
            repItems.CacheItemName = CacheItemName;
            repItems.CacheDependencies = CacheDependencies;
            repItems.CacheMinutes = CacheMinutes;
            repItems.CheckPermissions = CheckPermissions;
            repItems.ClassNames = ClassNames;
            repItems.CombineWithDefaultCulture = CombineWithDefaultCulture;
            repItems.CultureCode = CultureCode;
            repItems.MaxRelativeLevel = MaxRelativeLevel;
            repItems.OrderBy = OrderBy;
            repItems.SelectTopN = SelectTopN;
            repItems.Columns = Columns;
            repItems.SelectOnlyPublished = SelectOnlyPublished;
            repItems.FilterOutDuplicates = FilterOutDuplicates;
            repItems.Path = Path;

            repItems.SiteName = SiteName;
            repItems.WhereCondition = WhereCondition;

            // Pager
            repItems.EnablePaging = EnablePaging;
            repItems.PageSize = PageSize;
            repItems.PagerControl.PagerPosition = PagerPosition;
            repItems.PagerControl.QueryStringKey = QueryStringKey;
            repItems.PagerControl.PagingMode = PagingMode;
            repItems.PagerControl.BackNextLocation = BackNextLocation;
            repItems.PagerControl.ShowFirstLast = ShowFirstLast;
            repItems.PagerControl.PagerHTMLBefore = PagerHTMLBefore;
            repItems.PagerControl.PagerHTMLAfter = PagerHTMLAfter;
            repItems.PagerControl.ResultsLocation = ResultsPosition;
            repItems.PagerControl.PageNumbersSeparator = PageNumbersSeparator;

            // Relationships
            repItems.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
            repItems.RelationshipName = RelationshipName;
            repItems.RelationshipWithNodeGuid = RelationshipWithNodeGUID;

            // Transformation properties
            repItems.TransformationName = TransformationName;
            repItems.SelectedItemTransformationName = SelectedItemTransformationName;
            repItems.AlternatingTransformationName = AlternatingTransformationName;

            // Public properties
            repItems.HideControlForZeroRows = HideControlForZeroRows;
            repItems.ZeroRowsText = ZeroRowsText;
            repItems.ItemSeparator = ItemSeparator;
            repItems.FilterName = FilterName;

            // Effect properties
            repItems.RepeaterHTMLBefore = RepeaterHTMLBefore;
            repItems.RepeaterHTMLAfter = RepeaterHTMLAfter;
            repItems.ItemHTMLBefore = ItemHTMLBefore;
            repItems.ItemHTMLAfter = ItemHTMLAfter;
            repItems.ItemHTMLSeparator = ItemHTMLSeparator;
            repItems.HideLayoutForZeroRows = HideLayoutForZeroRows;
            repItems.ScriptFiles = ScriptFiles;
            repItems.InitScript = InitScript;
            repItems.CSSFiles = CSSFiles;
            repItems.InlineCSS = InlineCSS;

            repItems.DataSourceName = DataSourceName;
            repItems.DataSourceControl = DataSourceControl;

            // Edit mode buttons
            if (PageManager.ViewMode.IsLiveSite())
            {
                btnAdd.Visible = false;
                repItems.ShowEditDeleteButtons = false;
                repItems.ShowDeleteButton = false;
                repItems.ShowEditButton = false;
            }
            else
            {
                btnAdd.Visible = ShowNewButton;
                btnAdd.Text = NewButtonText;
                repItems.ShowEditDeleteButtons = ShowEditDeleteButtons;
                repItems.ShowDeleteButton = ShowDeleteButton;
                repItems.ShowEditButton = ShowEditButton;
            }

            if (!string.IsNullOrEmpty(repItems.ClassNames))
            {
                string[] classNames = repItems.ClassNames.Split(';');
                btnAdd.ClassName = DataHelper.GetNotEmpty(classNames[0], "");
            }

            if (!string.IsNullOrEmpty(repItems.Path))
            {
                string path = string.Empty;

                if (repItems.Path.EndsWithCSafe("/%"))
                {
                    path = repItems.Path.Remove(repItems.Path.Length - 2);
                }

                if (repItems.Path.EndsWithCSafe("/"))
                {
                    path = repItems.Path.Remove(repItems.Path.Length - 1);
                }

                btnAdd.Path = DataHelper.GetNotEmpty(path, string.Empty);
            }

            // Add repeater to the filter collection
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), repItems);

            if ((repItems.DataSourceControl != null)
                && (repItems.DataSourceControl.SourceFilterControl != null))
            {
                ((CMSAbstractBaseFilterControl)repItems.DataSourceControl.SourceFilterControl).OnFilterChanged += FilterControl_OnFilterChanged;
            }
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = !repItems.StopProcessing;

        if (DataHelper.DataSourceIsEmpty(repItems.DataSource) && (repItems.HideControlForZeroRows))
        {
            Visible = false;
        }

        // Hide the Add button for selected items which have the SelectedItem transformation specified
        if (ShowNewButton && !String.IsNullOrEmpty(repItems.SelectedItemTransformationName) && repItems.IsSelected)
        {
            btnAdd.Visible = false;
        }
    }


    /// <summary>
    /// Event risen when the source filter has changed
    /// </summary>
    protected void FilterControl_OnFilterChanged()
    {
        // Override previously set visibility. Control's visibility is managed in the PreRender event.
        Visible = true;
    }


    /// <summary>
    /// Reloads data.
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

    #endregion
}