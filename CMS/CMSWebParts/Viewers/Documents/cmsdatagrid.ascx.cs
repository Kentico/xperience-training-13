using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_cmsdatagrid : CMSAbstractWebPart
{
    #region "Variables"

    protected string[] mColumnList;
    protected string[] mGlobalNameID = null;

    #endregion


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
            gridElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, gridElem.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            gridElem.CacheDependencies = value;
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
            gridElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first page is sets when sort of some column is changed.
    /// </summary>
    public bool SetFirstPageAfterSortChange
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SetFirstPageAfterSortChange"), gridElem.SetFirstPageAfterSortChange);
        }
        set
        {
            SetValue("SetFirstPageAfterSortChange", value);
            gridElem.SetFirstPageAfterSortChange = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), gridElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    /// <summary>
    /// Gets or sets the class names which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ClassNames"), gridElem.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            gridElem.ClassNames = value;
        }
    }


    /// <summary>
    /// Code name of the category to display documents from.
    /// </summary>
    public string CategoryName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoryName"), gridElem.CategoryName);
        }
        set
        {
            SetValue("CategoryName", value);
            gridElem.CategoryName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), gridElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            gridElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CultureCode"), gridElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            gridElem.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), gridElem.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            gridElem.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("OrderBy"), gridElem.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            gridElem.OrderBy = value;
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
            gridElem.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), gridElem.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            gridElem.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether filter out duplicate documents.
    /// </summary>
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), gridElem.FilterOutDuplicates);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            gridElem.FilterOutDuplicates = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), gridElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            gridElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), gridElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            gridElem.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), gridElem.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            gridElem.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string SelectedColumns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), gridElem.SelectedColumns);
        }
        set
        {
            SetValue("Columns", value);
            gridElem.SelectedColumns = value;
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
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), gridElem.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            gridElem.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("RelationshipName"), gridElem.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            gridElem.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), gridElem.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            gridElem.RelationshipWithNodeGuid = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the transformation which is used for displaying the results of selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedItemTransformationName"), gridElem.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            gridElem.SelectedItemTransformationName = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), gridElem.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            gridElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), gridElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            gridElem.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sorting is ascending at default.
    /// </summary>
    public bool SortAscending
    {
        get
        {
            return (ValidationHelper.GetBoolean(GetValue("SortAscending"), gridElem.SortAscending));
        }
        set
        {
            SetValue("SortAscending", value);
            gridElem.SortAscending = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sorting is allowed.
    /// </summary>
    public bool AllowSorting
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowSorting"), gridElem.AllowSorting);
        }
        set
        {
            SetValue("AllowSorting", value);
            gridElem.AllowSorting = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sorting process is proceeded in the code.
    /// </summary>
    public bool ProcessSorting
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ProcessSorting"), gridElem.ProcessSorting);
        }
        set
        {
            SetValue("ProcessSorting", value);
            gridElem.ProcessSorting = value;
        }
    }


    /// <summary>
    /// Gets or sets the default sort field.
    /// </summary>
    public string SortField
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SortField"), gridElem.SortField);
        }
        set
        {
            SetValue("SortField", value);
            gridElem.SortField = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging will be allowed.
    /// </summary>
    public bool AllowPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowPaging"), gridElem.AllowPaging);
        }
        set
        {
            SetValue("AllowPaging", value);
            gridElem.AllowPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether custom paging is enabled.
    /// </summary>
    public bool AllowCustomPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowCustomPaging"), gridElem.AllowCustomPaging);
        }
        set
        {
            SetValue("AllowCustomPaging", value);
            gridElem.AllowCustomPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the size of the page if the paging is allowed.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), gridElem.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            gridElem.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether DataBind will be proceeded automatically.
    /// </summary>
    public bool DataBindByDefault
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DataBindByDefault"), gridElem.DataBindByDefault);
        }
        set
        {
            SetValue("DataBindByDefault", value);
            gridElem.DataBindByDefault = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagerMode PagingMode
    {
        get
        {
            return gridElem.GetPagerMode(ValidationHelper.GetString(GetValue("Mode"), ""));
        }
        set
        {
            SetValue("Mode", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether header will be displayed.
    /// </summary>
    public bool ShowHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowHeader"), gridElem.ShowHeader);
        }
        set
        {
            SetValue("ShowHeader", value);
            gridElem.ShowHeader = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether footer will be displayed.
    /// </summary>
    public bool ShowFooter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFooter"), gridElem.ShowFooter);
        }
        set
        {
            SetValue("ShowFooter", value);
            gridElem.ShowFooter = value;
        }
    }


    /// <summary>
    /// Gets or sets the tool tip text.
    /// </summary>
    public string ToolTip
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ToolTip"), gridElem.ToolTip);
        }
        set
        {
            SetValue("ToolTip", value);
            gridElem.ToolTip = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'New button' should be displayed.
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
    /// Gets or sets the value that represent column selector settings.
    /// </summary>
    public string ColumnsSelector
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ColumnSelector"), "");
        }
        set
        {
            SetValue("ColumnSelector", value);
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), gridElem.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            gridElem.FilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the SkinID which should be used.
    /// </summary>
    public override string SkinID
    {
        get
        {
            return base.SkinID;
        }
        set
        {
            base.SkinID = value;
            if ((gridElem != null) && (PageCycle < PageCycleEnum.Initialized))
            {
                gridElem.SkinID = SkinID;
            }
        }
    }


    /// <summary>
    /// Grid control.
    /// </summary>
    public CMSDataGrid GridControl
    {
        get
        {
            return gridElem;
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
            gridElem.StopProcessing = value;
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
            gridElem.StopProcessing = true;
        }
        else
        {
            gridElem.ControlContext = ControlContext;

            gridElem.EnableViewState = true;
            // Set properties from web part form   
            gridElem.CacheItemName = CacheItemName;
            gridElem.CacheDependencies = CacheDependencies;
            gridElem.CacheMinutes = CacheMinutes;
            gridElem.CheckPermissions = CheckPermissions;

            gridElem.ClassNames = ClassNames;
            gridElem.CategoryName = CategoryName;
            gridElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
            gridElem.FilterOutDuplicates = FilterOutDuplicates;
            gridElem.CultureCode = CultureCode;

            gridElem.MaxRelativeLevel = MaxRelativeLevel;
            gridElem.OrderBy = OrderBy;

            gridElem.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;

            gridElem.RelationshipName = RelationshipName;
            gridElem.RelationshipWithNodeGuid = RelationshipWithNodeGUID;
            gridElem.SelectedItemTransformationName = SelectedItemTransformationName;

            gridElem.Path = Path;

            gridElem.SelectTopN = SelectTopN;
            gridElem.SelectedColumns = SelectedColumns;

            gridElem.SelectOnlyPublished = SelectOnlyPublished;
            gridElem.SiteName = SiteName;
            gridElem.WhereCondition = WhereCondition;

            gridElem.HideControlForZeroRows = HideControlForZeroRows;
            gridElem.ZeroRowsText = ZeroRowsText;

            if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (ValidationHelper.GetString(Page.StyleSheetTheme, "") == ""))
            {
                gridElem.SkinID = SkinID;
            }

            gridElem.SortAscending = SortAscending;
            gridElem.AllowSorting = AllowSorting;
            gridElem.ProcessSorting = ProcessSorting;
            gridElem.SortField = SortField;

            gridElem.AllowPaging = AllowPaging;
            gridElem.AllowCustomPaging = AllowCustomPaging;
            gridElem.PageSize = PageSize;
            gridElem.DataBindByDefault = DataBindByDefault;

            gridElem.SelectTopN = SelectTopN;
            gridElem.PagerStyle.Mode = PagingMode;
            gridElem.FilterName = FilterName;

            gridElem.ShowHeader = ShowHeader;
            gridElem.ShowFooter = ShowFooter;
            gridElem.ToolTip = ToolTip;
            gridElem.SetFirstPageAfterSortChange = SetFirstPageAfterSortChange;

            // CMSEditModeButtonAdd
            btnAdd.Visible = ShowNewButton;
            btnAdd.Text = NewButtonText;

            string[] mClassNames = gridElem.ClassNames.Split(';');
            btnAdd.ClassName = DataHelper.GetNotEmpty(mClassNames[0], "");

            string mPath = "";
            if (gridElem.Path.EndsWithCSafe("/%"))
            {
                mPath = gridElem.Path.Remove(gridElem.Path.Length - 2);
            }
            if (gridElem.Path.EndsWithCSafe("/"))
            {
                mPath = gridElem.Path.Remove(gridElem.Path.Length - 1);
            }

            btnAdd.Path = DataHelper.GetNotEmpty(mPath, "");

            LoadFromDataClass(gridElem.ClassNames);


            InitColumns(ColumnsSelector);

            /*if (RequestHelper.IsPostBack())
            {
                this.gridElem.ReloadData(true);
            }*/

            gridElem.DataBindByDefault = false;
        }
    }


    /// <summary>
    /// Applies given StyleSheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        gridElem.SkinID = SkinID;
        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        gridElem.ReloadData(true);
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.ReloadData(true);
    }


    /// <summary>
    /// Gets information from XML and add it to columns.
    /// </summary>
    /// <param name="mXML">Input XML string</param>
    /// <returns>Return false if params is null or empty or when Xml haven't any node</returns>
    protected bool InitColumns(string mXML)
    {
        // Check if data are valid
        if (gridElem.Columns.Count > 0)
        {
            return false;
        }

        if (string.IsNullOrEmpty(mXML))
        {
            return false;
        }

        if ((mColumnList == null) || (mColumnList.Length == 0))
        {
            return false;
        }

        // Load XML from string
        XmlDocument mXMLDocument = new XmlDocument();
        mXMLDocument.LoadXml(mXML);

        XmlNodeList NodeList = mXMLDocument.DocumentElement.GetElementsByTagName("column");
        if (NodeList.Count == 0)
        {
            return false;
        }

        gridElem.AutoGenerateColumns = false;

        // Go through all nodes
        foreach (XmlNode node in NodeList)
        {
            string mName = XmlHelper.GetXmlAttributeValue(node.Attributes["name"], "");
            string mHeader = XmlHelper.GetXmlAttributeValue(node.Attributes["header"], "");
            string mType = XmlHelper.GetXmlAttributeValue(node.Attributes["type"], "");

            // If name is not empty
            if (DataHelper.GetNotEmpty(mName, "") != "")
            {
                // And it is in the table
                if (IsInTable(mName))
                {
                    // Create new column
                    DataGridColumn column = null;
                    if ((mType != null) && (mType == "link"))
                    {
                        if (mGlobalNameID == null)
                        {
                            mGlobalNameID = new string[0];
                        }

                        string[] mHelpGlobal = new string[mGlobalNameID.Length + 1];

                        mHelpGlobal[mGlobalNameID.Length] = mName;

                        for (int i = 0; i < mGlobalNameID.Length; i++)
                        {
                            mHelpGlobal[i] = mGlobalNameID[i];
                        }

                        mGlobalNameID = mHelpGlobal;

                        TemplateColumn col = new TemplateColumn();
                        col.ItemTemplate = new LinkItemTemplate(mName);
                        column = col;
                    }
                    else
                    {
                        BoundColumn col = new BoundColumn();
                        column = col;

                        col.DataField = mName;
                    }

                    // Load header
                    if (DataHelper.GetNotEmpty(mHeader, "") != "")
                    {
                        column.HeaderText = ResHelper.LocalizeString(mHeader);
                    }
                    else
                    {
                        column.HeaderText = mName;
                    }

                    if (gridElem.AllowSorting)
                    {
                        column.SortExpression = mName;
                    }

                    gridElem.Columns.Add(column);
                }
            }
        }

        return true;
    }


    /// <summary>
    ///  Check if column is valid for classnames.
    /// </summary>
    /// <param name="mColumn">Name of column</param>
    /// <returns>Return true if column is in table</returns>
    protected bool IsInTable(string mColumn)
    {
        if ((DataHelper.GetNotEmpty(mColumn, "") == "") || (mColumnList == null))
        {
            // If column isn't set return false            
            return false;
        }

        // Find column in table
        for (int i = mColumnList.GetLowerBound(0); i <= mColumnList.GetUpperBound(0); i++)
        {
            if (mColumnList[i] == mColumn)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Load Columns names from Tree.
    /// </summary>
    protected void LoadFromDataClass(string classNames)
    {
        List<string> columnList = new List<string>();
        List<string> classesList = new List<string>();

        if (!string.IsNullOrEmpty(classNames))
        {
            classesList = new List<string>(classNames.Split(';'));
        }

        // If more than 1 DataClasses are in classesList, get columns only from CMS.Tree and CMS.Document.
        //if (classesList.Count > 1)
        //{
        //    classesList = new List<string>();
        //}
        classesList.Add("CMS.Tree");
        classesList.Add("CMS.Document");

        // Fill columnList with column names from all classes.
        foreach (string className in classesList)
        {
            try
            {
                if (className != "")
                {
                    IDataClass dc = DataClassFactory.NewDataClass(className);
                    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(className);

                    // Get columns only from couplet classes.
                    if (dci.ClassIsCoupledClass)
                    {
                        foreach (string columnName in dc.StructureInfo.ColumnNames)
                        {
                            columnList.Add(columnName);
                        }
                    }
                }
            }
            catch
            {
            }
        }
        // Set string array.
        mColumnList = columnList.ToArray();
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set visibility of control
        Visible = gridElem.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(gridElem.DataSource) && (HideControlForZeroRows))
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Clear cache.
    /// </summary>
    public override void ClearCache()
    {
        gridElem.ClearCache();
    }
}