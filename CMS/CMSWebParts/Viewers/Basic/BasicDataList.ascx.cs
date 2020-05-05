using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Basic_BasicDataList : CMSAbstractWebPart
{
    #region "Variables"

    private BasicDataList BasicDataList = new BasicDataList();
    private CMSBaseDataSource mDataSourceControl = null;

    // Indicates whether control was binded
    private bool binded = false;

    // Indicates whether current control was added to the filter collection
    private bool mFilterControlAdded = false;

    // Indicates whether filter changed event was registered
    private bool filterRegistered;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether dynamic controls should be resolved
    /// </summary>
    public bool ResolveDynamicControls
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResolveDynamicControls"), BasicDataList.ResolveDynamicControls);
        }
        set
        {
            SetValue("ResolveDynamicControls", value);
            BasicDataList.ResolveDynamicControls = value;
        }
    }


    /// <summary>
    /// Gets or sets name of source.
    /// </summary>
    public string DataSourceName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataSourceName"), "");
        }
        set
        {
            SetValue("DataSourceName", value);
        }
    }


    /// <summary>
    /// Control with data source.
    /// </summary>
    public CMSBaseDataSource DataSourceControl
    {
        get
        {
            // Check if control is empty and load it with the data
            if (mDataSourceControl == null)
            {
                if (!String.IsNullOrEmpty(DataSourceName))
                {
                    mDataSourceControl = CMSControlsHelper.GetFilter(DataSourceName) as CMSBaseDataSource;
                }
            }

            return mDataSourceControl;
        }
        set
        {
            mDataSourceControl = value;
        }
    }


    /// <summary>
    /// Gets or sets AlternatingItemTemplate property.
    /// </summary>
    public string AlternatingItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternatingItemTransformationName"), "");
        }
        set
        {
            SetValue("AlternatingItemTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets FooterTemplate property.
    /// </summary>
    public string FooterTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FooterTransformationName"), "");
        }
        set
        {
            SetValue("FooterTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets HeaderTemplate property.
    /// </summary>
    public string HeaderTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderTransformationName"), "");
        }
        set
        {
            SetValue("HeaderTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets SelectedItemStyle property.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), "");
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), "");
        }
        set
        {
            SetValue("TransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets SeparatorTemplate property.
    /// </summary>
    public string SeparatorTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SeparatorTransformationName"), "");
        }
        set
        {
            SetValue("SeparatorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets FooterTemplate for selected item.
    /// </summary>
    public string SelectedItemFooterTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemFooterTransformationName"), "");
        }
        set
        {
            SetValue("SelectedItemFooterTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets HeaderTemplate for selected item.
    /// </summary>
    public string SelectedItemHeaderTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemHeaderTransformationName"), "");
        }
        set
        {
            SetValue("SelectedItemHeaderTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), true);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            BasicDataList.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), "");
        }
        set
        {
            SetValue("ZeroRowsText", value);
            BasicDataList.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the count of repeat columns.
    /// </summary>
    public int RepeatColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RepeatColumns"), BasicDataList.RepeatColumns);
        }
        set
        {
            SetValue("RepeatColumns", value);
            BasicDataList.RepeatColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets whether control is displayed in a table or flow layout.
    /// </summary>
    public RepeatLayout RepeatLayout
    {
        get
        {
            return CMSDataList.GetRepeatLayout(DataHelper.GetNotEmpty(GetValue("RepeatLayout"), BasicDataList.RepeatLayout.ToString()));
        }
        set
        {
            SetValue("RepeatLayout", value.ToString());
            BasicDataList.RepeatLayout = value;
        }
    }


    /// <summary>
    /// Gets or sets whether DataList control displays vertically or horizontally.
    /// </summary>
    public RepeatDirection RepeatDirection
    {
        get
        {
            return CMSDataList.GetRepeatDirection(DataHelper.GetNotEmpty(GetValue("RepeatDirection"), BasicDataList.RepeatDirection.ToString()));
        }
        set
        {
            SetValue("RepeatDirection", value.ToString());
            BasicDataList.RepeatDirection = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// On content loaded override.
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
            // Set properties
            BasicDataList.ResolveDynamicControls = ResolveDynamicControls;
            BasicDataList.HideControlForZeroRows = HideControlForZeroRows;
            BasicDataList.DataBindByDefault = false;
            BasicDataList.RepeatColumns = RepeatColumns;
            BasicDataList.RepeatDirection = RepeatDirection;
            BasicDataList.RepeatLayout = RepeatLayout;
            BasicDataList.OnPageChanged += new EventHandler<EventArgs>(BasicDataList_OnPageChanged);

            EnsureFilterControl();

            if (!String.IsNullOrEmpty(ZeroRowsText))
            {
                BasicDataList.ZeroRowsText = ZeroRowsText;
            }
        }
    }


    /// <summary>
    /// Ensures current control in the filters collection.
    /// </summary>
    protected void EnsureFilterControl()
    {
        if (!mFilterControlAdded)
        {
            // Add basic repeater to the filter collection
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), BasicDataList);
            mFilterControlAdded = true;
        }
    }


    /// <summary>
    /// Binds datasource control data to the viewer control
    /// </summary>
    private void BindControl()
    {
        if (DataSourceControl != null)
        {
            BasicDataList.DataSource = DataSourceControl.LoadData(true);
            LoadTransformations();
            BasicDataList.DataBind();
            binded = true;
        }
    }


    /// <summary>
    /// OnPageChanged event handler.
    /// </summary>
    private void BasicDataList_OnPageChanged(object sender, EventArgs e)
    {
        EnsureChildControls();

        BindControl();
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        PageContext.InitComplete += PageHelper_InitComplete;
    }


    void PageHelper_InitComplete(object sender, EventArgs e)
    {
        //Register filter change event
        RegisterFilterEvent();
    }


    /// <summary>
    /// Registers filter event
    /// </summary>
    private void RegisterFilterEvent()
    {
        //Registers filter change event
        if ((DataSourceControl != null) && !filterRegistered)
        {
            filterRegistered = true;
            DataSourceControl.OnFilterChanged += DataSourceControl_OnFilterChanged;
        }
    }


    protected override void CreateChildControls()
    {
        // Add control to the control collection
        plcBasicDataList.Controls.Add(BasicDataList);

        base.CreateChildControls();
    }


    /// <summary>
    /// Loads and setups web part.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        EnsureChildControls();
        RegisterFilterEvent();

        // Check whether postback was executed from current transformation item
        if (RequestHelper.IsPostBack())
        {
            // Indicates whether postback was fired from current control
            bool bindControl = false;

            // Check event target value and callback parameter value
            string eventTarget = ValidationHelper.GetString(Request.Form[Page.postEventSourceID], String.Empty);
            string callbackParam = ValidationHelper.GetString(Request.Form["__CALLBACKPARAM"], String.Empty);
            if (eventTarget.StartsWithCSafe(UniqueID) || callbackParam.StartsWithCSafe(UniqueID) || eventTarget.EndsWithCSafe(ContextMenu.CONTEXT_MENU_SUFFIX))
            {
                bindControl = true;
            }
            // Check whether request key contains some control assigned to current control
            else
            {
                foreach (string key in Request.Form.Keys)
                {
                    if ((key != null) && key.StartsWithCSafe(UniqueID))
                    {
                        bindControl = true;
                        break;
                    }
                }
            }

            if (bindControl)
            {
                BindControl();
            }
        }

        base.OnLoad(e);
    }


    /// <summary>
    /// Load transformations with dependence on current datasource type and datasource type.
    /// </summary>
    protected void LoadTransformations()
    {
        CMSBaseDataSource docDataSource = DataSourceControl as CMSBaseDataSource;
        if (!String.IsNullOrEmpty(SelectedItemTransformationName) && (docDataSource != null) && docDataSource.IsSelected)
        {
            BasicDataList.ItemTemplate = TransformationHelper.LoadTransformation(this, SelectedItemTransformationName);

            if (!String.IsNullOrEmpty(SelectedItemFooterTransformationName))
            {
                BasicDataList.FooterTemplate = TransformationHelper.LoadTransformation(this, SelectedItemFooterTransformationName);
            }
            else
            {
                BasicDataList.FooterTemplate = null;
            }

            if (!String.IsNullOrEmpty(SelectedItemHeaderTransformationName))
            {
                BasicDataList.HeaderTemplate = TransformationHelper.LoadTransformation(this, SelectedItemHeaderTransformationName);
            }
            else
            {
                BasicDataList.HeaderTemplate = null;
            }
        }
        else
        {
            // Apply transformations if they exist
            if (!String.IsNullOrEmpty(AlternatingItemTransformationName))
            {
                BasicDataList.AlternatingItemTemplate = TransformationHelper.LoadTransformation(this, AlternatingItemTransformationName);
            }

            if (!String.IsNullOrEmpty(FooterTransformationName))
            {
                BasicDataList.FooterTemplate = TransformationHelper.LoadTransformation(this, FooterTransformationName);
            }

            if (!String.IsNullOrEmpty(HeaderTransformationName))
            {
                BasicDataList.HeaderTemplate = TransformationHelper.LoadTransformation(this, HeaderTransformationName);
            }

            if (!String.IsNullOrEmpty(TransformationName))
            {
                BasicDataList.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
            }

            if (!String.IsNullOrEmpty(SeparatorTransformationName))
            {
                BasicDataList.SeparatorTemplate = TransformationHelper.LoadTransformation(this, SeparatorTransformationName);
            }
        }
    }


    /// <summary>
    /// OnFilter change event handler.
    /// </summary>
    private void DataSourceControl_OnFilterChanged()
    {
        EnsureChildControls();

        // Set forcibly visibility
        Visible = true;

        BindControl();
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Datasource data
        object ds = null;

        // Set transformations if data source is not empty
        if (DataSourceControl != null)
        {
            // Get data from datasource
            ds = DataSourceControl.DataSource;

            // Check whether data exist
            if ((!DataHelper.DataSourceIsEmpty(ds)) && (!binded))
            {
                // Initialize related data if provided
                if (DataSourceControl.RelatedData != null)
                {
                    RelatedData = DataSourceControl.RelatedData;
                }
                BasicDataList.DataSource = DataSourceControl.DataSource;
                LoadTransformations();
                BasicDataList.DataBind();
            }
        }

        base.OnPreRender(e);

        // Hide control for zero rows
        if (((DataSourceControl == null) || (DataHelper.DataSourceIsEmpty(ds))) && (HideControlForZeroRows))
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
        EnsureFilterControl();

        base.ReloadData();
    }

    #endregion;
}
