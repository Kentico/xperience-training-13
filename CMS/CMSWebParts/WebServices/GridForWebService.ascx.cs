using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_WebServices_GridForWebService : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether to hide control when no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), basicDataGrid.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            basicDataGrid.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed when no data found.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), basicDataGrid.ZeroRowsText), basicDataGrid.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            basicDataGrid.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the sorting is ascending.
    /// </summary>
    public bool SortAscending
    {
        get
        {
            return (ValidationHelper.GetBoolean(GetValue("SortAscending"), basicDataGrid.SortAscending));
        }
        set
        {
            SetValue("SortAscending", value);
            basicDataGrid.SortAscending = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the sorting is allowed.
    /// </summary>
    public bool AllowSorting
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowSorting"), basicDataGrid.AllowSorting);
        }
        set
        {
            SetValue("AllowSorting", value);
            basicDataGrid.AllowSorting = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the sorting process is in the code.
    /// </summary>
    public bool ProcessSorting
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ProcessSorting"), basicDataGrid.ProcessSorting);
        }
        set
        {
            SetValue("ProcessSorting", value);
            basicDataGrid.ProcessSorting = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the column which should be used for sorting.
    /// </summary>
    public string SortField
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SortField"), basicDataGrid.SortField), basicDataGrid.SortField);
        }
        set
        {
            SetValue("SortField", value);
            basicDataGrid.SortField = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the the paging should be allowed.
    /// </summary>
    public bool AllowPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowPaging"), basicDataGrid.AllowPaging);
        }
        set
        {
            SetValue("AllowPaging", value);
            basicDataGrid.AllowPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the custom paging should be allowed.
    /// </summary>
    public bool AllowCustomPaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowCustomPaging"), basicDataGrid.AllowCustomPaging);
        }
        set
        {
            SetValue("AllowCustomPaging", value);
            basicDataGrid.AllowCustomPaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the size of the page if the paging is allowed.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), basicDataGrid.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            basicDataGrid.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the databind is automatic.
    /// </summary>
    public bool DataBindByDefault
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DataBindByDefault"), basicDataGrid.DataBindByDefault);
        }
        set
        {
            SetValue("DataBindByDefault", value);
            basicDataGrid.DataBindByDefault = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager mode.
    /// </summary>
    public PagerMode PagingMode
    {
        get
        {
            return basicDataGrid.GetPagerMode(ValidationHelper.GetString(GetValue("Mode"), ""));
        }
        set
        {
            SetValue("Mode", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the header should be shown.
    /// </summary>
    public bool ShowHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowHeader"), basicDataGrid.ShowHeader);
        }
        set
        {
            SetValue("ShowHeader", value);
            basicDataGrid.ShowHeader = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the footer should be shown.
    /// </summary>
    public bool ShowFooter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFooter"), basicDataGrid.ShowFooter);
        }
        set
        {
            SetValue("ShowFooter", value);
            basicDataGrid.ShowFooter = value;
        }
    }


    /// <summary>
    /// Gets or sets the tool tip value.
    /// </summary>
    public string ToolTip
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ToolTip"), basicDataGrid.ToolTip);
        }
        set
        {
            SetValue("ToolTip", value);
            basicDataGrid.ToolTip = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the columns are generated automatically.
    /// </summary>
    public bool AutoGenerateColumns
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoGenerateColumns"), basicDataGrid.AutoGenerateColumns);
        }
        set
        {
            SetValue("AutoGenerateColumns", value);
            basicDataGrid.AutoGenerateColumns = value;
        }
    }

    #endregion


    #region "Webservices properties"

    /// <summary>
    /// Gets or sets the parameters of the web service.
    /// </summary>
    public string WebServiceParameters
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WebServiceParameters"), wsDataSource.WebServiceParameters);
        }
        set
        {
            SetValue("WebServiceParameters", value);
            wsDataSource.WebServiceParameters = value;
        }
    }


    /// <summary>
    /// Gets or sets the url of the web service.
    /// </summary>
    public string WebServiceURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WebServiceURL"), wsDataSource.WebServiceUrl);
        }
        set
        {
            SetValue("WebServiceURL", value);
            wsDataSource.WebServiceUrl = value;
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
            wsDataSource.StopProcessing = value;
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
            basicDataGrid.Visible = false;
            wsDataSource.StopProcessing = true;
        }
        else
        {
            // Setup the control
            // Setup paging
            basicDataGrid.AllowPaging = AllowPaging;
            basicDataGrid.PageSize = PageSize;
            basicDataGrid.AllowCustomPaging = AllowCustomPaging;
            basicDataGrid.PagerStyle.Mode = PagingMode;
            basicDataGrid.ShowHeader = ShowHeader;
            basicDataGrid.ShowFooter = ShowFooter;

            basicDataGrid.ToolTip = ToolTip;

            // Set SkinID property
            if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (ValidationHelper.GetString(Page.StyleSheetTheme, "") == ""))
            {
                basicDataGrid.SkinID = SkinID;
            }

            basicDataGrid.EnableViewState = true;

            // Setup sorting
            basicDataGrid.AllowSorting = AllowSorting;
            basicDataGrid.ProcessSorting = ProcessSorting;
            basicDataGrid.SortAscending = SortAscending;
            basicDataGrid.SortField = SortField;

            basicDataGrid.AutoGenerateColumns = AutoGenerateColumns;
            basicDataGrid.DataBindByDefault = DataBindByDefault;

            basicDataGrid.HideControlForZeroRows = HideControlForZeroRows;
            basicDataGrid.ZeroRowsText = ZeroRowsText;

            // Get web service URL and parameters
            wsDataSource.WebServiceUrl = WebServiceURL;
            wsDataSource.WebServiceParameters = WebServiceParameters;

            // Connect with data source
            basicDataGrid.DataSource = wsDataSource.DataSource;

            Visible = !(DataHelper.DataSourceIsEmpty(basicDataGrid.DataSource) && HideControlForZeroRows);
        }
    }


    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        basicDataGrid.SkinID = SkinID;

        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Reload control's data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }
}