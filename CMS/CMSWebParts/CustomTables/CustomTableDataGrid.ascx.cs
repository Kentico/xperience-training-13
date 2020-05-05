using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Base.Web.UI;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_CustomTables_CustomTableDataGrid : CMSAbstractWebPart
{
    #region "Protected variables"

    protected List<string> mColumnList;
    protected List<string> mGlobalNameID = null;
    protected List<string> mDataType = null;
    protected DataClassInfo mDataClassInfo = null;

    #endregion


    #region "General properties"

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
            gridItems.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, gridItems.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            gridItems.CacheDependencies = value;
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
            gridItems.CacheMinutes = value;
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
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), gridItems.OrderBy), gridItems.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            gridItems.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), gridItems.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            gridItems.WhereCondition = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether first page is sets when sort of some column is changed.
    /// </summary>
    public bool SetFirstPageAfterSortChange
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SetFirstPageAfterSortChange"), gridItems.SetFirstPageAfterSortChange);
        }
        set
        {
            SetValue("SetFirstPageAfterSortChange", value);
            gridItems.SetFirstPageAfterSortChange = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), gridItems.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            gridItems.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), gridItems.ZeroRowsText), gridItems.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            gridItems.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), gridItems.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            gridItems.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sorting is ascending at default.
    /// </summary>
    public bool SortAscending
    {
        get
        {
            return (ValidationHelper.GetBoolean(GetValue("SortAscending"), gridItems.SortAscending));
        }
        set
        {
            SetValue("SortAscending", value);
            if (!RequestHelper.IsPostBack())
            {
                gridItems.SortAscending = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sorting is allowed.
    /// </summary>
    public bool AllowSorting
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowSorting"), gridItems.AllowSorting);
        }
        set
        {
            SetValue("AllowSorting", value);
            gridItems.AllowSorting = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that inidcates whether sorting process is proceeded in the code.
    /// </summary>
    public bool ProcessSorting
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ProcessSorting"), gridItems.ProcessSorting);
        }
        set
        {
            SetValue("ProcessSorting", value);
            gridItems.ProcessSorting = value;
        }
    }


    /// <summary>
    /// Gets or sets the default sort field.
    /// </summary>
    public string SortField
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SortField"), gridItems.SortField), gridItems.SortField);
        }
        set
        {
            SetValue("SortField", value);
            if (!RequestHelper.IsPostBack())
            {
                gridItems.SortField = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging will be allowed.
    /// </summary>
    public bool AllowPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowPaging"), gridItems.AllowPaging);
        }
        set
        {
            SetValue("AllowPaging", value);
            gridItems.AllowPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether custom paging is enabled.
    /// </summary>
    public bool AllowCustomPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowCustomPaging"), gridItems.AllowCustomPaging);
        }
        set
        {
            SetValue("AllowCustomPaging", value);
            gridItems.AllowCustomPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the size of the page if the paging is allowed.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), gridItems.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            gridItems.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether databind will be proceeded automatically.
    /// </summary>
    public bool DataBindByDefault
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DataBindByDefault"), gridItems.DataBindByDefault);
        }
        set
        {
            SetValue("DataBindByDefault", value);
            gridItems.DataBindByDefault = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagerMode PagingMode
    {
        get
        {
            return gridItems.GetPagerMode(ValidationHelper.GetString(GetValue("Mode"), ""));
        }
        set
        {
            SetValue("Mode", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that inidcates whether header will be displayed.
    /// </summary>
    public bool ShowHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowHeader"), gridItems.ShowHeader);
        }
        set
        {
            SetValue("ShowHeader", value);
            gridItems.ShowHeader = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether footer will be displayed.
    /// </summary>
    public bool ShowFooter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFooter"), gridItems.ShowFooter);
        }
        set
        {
            SetValue("ShowFooter", value);
            gridItems.ShowFooter = value;
        }
    }


    /// <summary>
    /// Gets or sets the tool tip text.
    /// </summary>
    public string ToolTip
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ToolTip"), gridItems.ToolTip);
        }
        set
        {
            SetValue("ToolTip", value);
            gridItems.ToolTip = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether columns should be generated automatically.
    /// </summary>
    public bool AutoGenerateColumns
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoGenerateColumns"), gridItems.AutoGenerateColumns);
        }
        set
        {
            SetValue("AutoGenerateColumns", value);
            gridItems.AutoGenerateColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets the custom table.
    /// </summary>
    public string CustomTable
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CustomTable"), "");
        }
        set
        {
            SetValue("CustomTable", value);
            gridItems.QueryName = QueryName;
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
            return ValidationHelper.GetString(GetValue("FilterName"), gridItems.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            gridItems.FilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the SkinID of the grid.
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
            if ((gridItems != null) && (PageCycle < PageCycleEnum.Initialized))
            {
                gridItems.SkinID = value;
            }
        }
    }


    /// <summary>
    /// Detail page path.
    /// </summary>
    public string DetailPagePath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DetailPagePath"), null);
        }
        set
        {
            SetValue("DetailPagePath", value);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Data class information.
    /// </summary>
    private DataClassInfo DataClassInfo
    {
        get
        {
            if (mDataClassInfo == null)
            {
                mDataClassInfo = DataClassInfoProvider.GetDataClassInfo(CustomTable);
            }
            return mDataClassInfo;
        }
    }


    /// <summary>
    /// Gets or sets the query name.
    /// </summary>
    private string QueryName
    {
        get
        {
            if (DataClassInfo != null)
            {
                return DataClassInfo.ClassName + ".selectall";
            }
            else
            {
                return gridItems.QueryName;
            }
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
            gridItems.StopProcessing = value;
        }
    }

    #endregion


    #region "Overidden methods"

    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        gridItems.ClearCache();
    }


    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        gridItems.SkinID = SkinID;
        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        gridItems.ReloadData(true);
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        gridItems.ReloadData(true);
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = gridItems.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(gridItems.DataSource) && HideControlForZeroRows)
        {
            Visible = false;
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            gridItems.StopProcessing = true;
        }
        else
        {
            gridItems.ControlContext = ControlContext;
            gridItems.HideControlForZeroRows = HideControlForZeroRows;
            gridItems.ZeroRowsText = ZeroRowsText;

            if (DataClassInfo == null)
            {
                return;
            }

            // Check permissions if necessary
            if (CheckPermissions && !DataClassInfo.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                return;
            }

            gridItems.CacheItemName = CacheItemName;
            gridItems.CacheDependencies = CacheDependencies;
            gridItems.CacheMinutes = CacheMinutes;
            gridItems.OrderBy = OrderBy;
            gridItems.WhereCondition = WhereCondition;
            gridItems.FilterName = FilterName;

            gridItems.SelectTopN = SelectTopN;
            gridItems.EnableViewState = true;

            gridItems.AllowSorting = AllowSorting;
            gridItems.ProcessSorting = ProcessSorting;

            if (!RequestHelper.IsPostBack())
            {
                gridItems.SortAscending = SortAscending;
                gridItems.SortField = SortField;
            }

            gridItems.AllowPaging = AllowPaging;
            gridItems.PageSize = PageSize;
            gridItems.AllowCustomPaging = AllowCustomPaging;
            gridItems.PagerStyle.Mode = PagingMode;
            gridItems.ShowHeader = ShowHeader;
            gridItems.ShowFooter = ShowFooter;

            gridItems.ToolTip = ToolTip;

            gridItems.DataBindByDefault = DataBindByDefault;

            gridItems.AutoGenerateColumns = AutoGenerateColumns;

            gridItems.QueryName = QueryName;

            gridItems.SetFirstPageAfterSortChange = SetFirstPageAfterSortChange;

            string mXML = ColumnsSelector;

            // Set SkinID properties
            if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (ValidationHelper.GetString(Page.StyleSheetTheme, "") == ""))
            {
                gridItems.SkinID = SkinID;
            }

            LoadFromQuery(gridItems.QueryName);

            if (ColumnSelector(mXML))
            {
                gridItems.AutoGenerateColumns = false;
            }

            if (!DataHelper.DataSourceIsEmpty(gridItems.DataSource))
            {
                gridItems.DataBind();
            }
        }
    }


    /// <summary>
    /// Check if column is valid for classnames.
    /// </summary>
    /// <param name="mColumn">Name of column</param>
    /// <returns>Return true if column is on table</returns>
    protected bool IsInTable(string mColumn)
    {
        if (string.IsNullOrEmpty(mColumn))
        {
            return false;
        }

        return mColumnList.Contains(mColumn);
    }


    /// <summary>
    /// Ensures the column information.
    /// </summary>
    protected void EnsureColumnInfo()
    {
        DataView dv = DataHelper.GetDataView(gridItems.DataSource);
        if (dv != null)
        {
            var mCl = new List<string>(dv.Table.Columns.Count);
            var mDt = new List<string>(dv.Table.Columns.Count);

            foreach (DataColumn dc in dv.Table.Columns)
            {
                mCl.Add(dc.ColumnName);
                mDt.Add(dc.DataType.ToString());
            }

            mDataType = mDt;
            mColumnList = mCl;
        }
    }


    /// <summary>
    /// Load Columns names with Query.
    /// </summary>
    protected void LoadFromQuery(string queryName)
    {
        if (!string.IsNullOrEmpty(queryName))
        {
            // Load the data
            //gridItems.DataBindByDefault = true;
            gridItems.ReloadData(false);

            // Ensure the column information
            EnsureColumnInfo();
        }
    }


    /// <summary>
    /// Gets information from XML and add it to columns.
    /// </summary>
    /// <param name="mXML">Input XML string</param>
    /// <returns>Return false if params is null or empty or when Xml haven't any node</returns>
    protected bool ColumnSelector(string mXML)
    {
        // Check valid state
        if (String.IsNullOrEmpty(mXML))
        {
            return false;
        }

        if ((mColumnList == null) || (mColumnList.Count == 0))
        {
            return false;
        }

        //Create XML document from string
        XmlDocument mXMLDocument = new XmlDocument();

        mXMLDocument.LoadXml(mXML);

        // Get column list
        XmlNodeList NodeList = mXMLDocument.DocumentElement.GetElementsByTagName("column");

        //If empty, nothing to do
        if (NodeList.Count == 0)
        {
            return false;
        }

        //gridItems.ItemDataBound += gridItems_ItemDataBound;
        gridItems.Columns.Clear();

        foreach (XmlNode node in NodeList)
        {
            // Get attributes values
            string mName = XmlHelper.GetXmlAttributeValue(node.Attributes["name"], "");
            string mHeader = XmlHelper.GetXmlAttributeValue(node.Attributes["header"], "");
            string mType = XmlHelper.GetXmlAttributeValue(node.Attributes["type"], "");

            if (DataHelper.GetNotEmpty(mName, "") != "")
            {
                // Check if column is in the table
                if (IsInTable(mName))
                {
                    // Grid value as link
                    if (mType == "link")
                    {
                        if (mGlobalNameID == null)
                        {
                            mGlobalNameID = new List<string>();
                        }

                        mGlobalNameID.Add(mName);

                        TemplateColumn col = new TemplateColumn();
                        col.ItemTemplate = new LinkItemTemplate(mName);

                        // First try header then name
                        col.HeaderText = DataHelper.GetNotEmpty(mHeader, mName);

                        if (gridItems.AllowSorting)
                        {
                            col.SortExpression = mName;
                        }

                        // Add column to the grid
                        gridItems.Columns.Add(col);
                    }
                    // Not link
                    else
                    {
                        BoundColumn col = new BoundColumn();

                        col.DataField = mName;

                        // First try header then name
                        col.HeaderText = DataHelper.GetNotEmpty(mHeader, mName);

                        if (gridItems.AllowSorting)
                        {
                            col.SortExpression = mName;
                        }

                        gridItems.Columns.Add(col);
                    }
                }
            }
        }

        return true;
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// Item data bound handler.
    /// </summary>
    protected void gridItems_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        if (mGlobalNameID != null)
        {
            foreach (var globalName in mGlobalNameID)
            {
                if (!string.IsNullOrEmpty(globalName))
                {
                    Control ctrl = e.Item.FindControl(globalName);
                    if (ctrl != null)
                    {
                        ((HyperLink)ctrl).Text = ((DataRowView)e.Item.DataItem)[globalName].ToString();
                        int itemId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue((DataRowView)e.Item.DataItem, "ItemID"), 0);
                        string detailUrl = RequestContext.CurrentURL;

                        // Add querystring parametr
                        detailUrl = URLHelper.AddParameterToUrl(detailUrl, "id", itemId.ToString());

                        ((HyperLink)ctrl).NavigateUrl = detailUrl;
                    }
                }
            }
        }
        // Timezones
        if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
        {
            object dataItem = e.Item.DataItem;
            if (dataItem.GetType() == typeof(DataRowView))
            {
                // Get data row
                DataRow dr = ((DataRowView)dataItem).Row;

                // Get count of columns (depends whether columns are automatically generated)
                int columnCount = gridItems.AutoGenerateColumns ? dr.Table.Columns.Count : gridItems.Columns.Count;

                // Go through all grid columns
                int j = 0;
                for (int i = 0; i < columnCount; i++)
                {
                    // Get column of current index
                    object column = gridItems.AutoGenerateColumns ? dr[i] : gridItems.Columns[i];

                    if (((column.GetType() == typeof(BoundColumn)) && !gridItems.AutoGenerateColumns) || ((column is DateTime) && gridItems.AutoGenerateColumns))
                    {
                        // Get cell or actual value
                        object cell = gridItems.AutoGenerateColumns ? column : dr[((BoundColumn)column).DataField];

                        // Apply timezone settings
                        if (cell is DateTime)
                        {
                            e.Item.Cells[j].Text = TimeZoneUIMethods.ConvertDateTime((DateTime)cell, this).ToString();
                        }
                    }
                    // DataGrid doesn't show the GUID columns if the columns are autogenerated
                    else if (gridItems.AutoGenerateColumns && (column is Guid))
                    {
                        --j;
                    }

                    ++j;
                }
            }
        }
    }

    #endregion
}