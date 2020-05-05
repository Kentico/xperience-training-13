using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Basic_BasicRepeater : CMSAbstractWebPart
{
    #region "Variables"

    private BasicRepeater BasicRepeater = new BasicRepeater();

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
            return ValidationHelper.GetBoolean(GetValue("ResolveDynamicControls"), BasicRepeater.ResolveDynamicControls);
        }
        set
        {
            SetValue("ResolveDynamicControls", value);
            BasicRepeater.ResolveDynamicControls = value;
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
            BasicRepeater.DataSourceName = value;
        }
    }


    /// <summary>
    /// Control with data source.
    /// </summary>
    public CMSBaseDataSource DataSourceControl
    {
        get
        {
            return BasicRepeater.DataSourceControl;
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
    /// Gets or sets ItemTemplate for selected item.
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
            if (!String.IsNullOrEmpty(ZeroRowsText))
            {
                BasicRepeater.ZeroRowsText = ZeroRowsText;
            }

            BasicRepeater.ResolveDynamicControls = ResolveDynamicControls;
            BasicRepeater.DataSourceName = DataSourceName;
            BasicRepeater.HideControlForZeroRows = HideControlForZeroRows;
            BasicRepeater.DataBindByDefault = false;
            BasicRepeater.OnPageChanged += new EventHandler<EventArgs>(BasicRepeater_OnPageChanged);

            EnsureFilterControl();
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
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), BasicRepeater);
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
            BasicRepeater.DataSource = DataSourceControl.LoadData(true);
            LoadTransformations();
            BasicRepeater.DataBind();
            binded = true;
        }
    }


    /// <summary>
    /// OnPageChaged event handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArg</param>
    private void BasicRepeater_OnPageChanged(object sender, EventArgs e)
    {
        EnsureChildControls();

        BindControl();
    }


    protected override void CreateChildControls()
    {
        // Add control to the control collection
        plcBasicRepeater.Controls.Add(BasicRepeater);

        base.CreateChildControls();
    }


    void PageHelper_InitComplete(object sender, EventArgs e)
    {
        //Register filter change event
        RegisterFilterEvent();
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        PageContext.InitComplete += PageHelper_InitComplete;
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
    /// Load transformations with dependence on datasource type and datasource state.
    /// </summary>
    protected void LoadTransformations()
    {
        CMSBaseDataSource docDataSource = DataSourceControl as CMSBaseDataSource;
        if (!String.IsNullOrEmpty(SelectedItemTransformationName) && (docDataSource != null) && docDataSource.IsSelected)
        {
            BasicRepeater.ItemTemplate = TransformationHelper.LoadTransformation(this, SelectedItemTransformationName);

            if (!String.IsNullOrEmpty(SelectedItemFooterTransformationName))
            {
                BasicRepeater.FooterTemplate = TransformationHelper.LoadTransformation(this, SelectedItemFooterTransformationName);
            }
            else
            {
                BasicRepeater.FooterTemplate = null;
            }

            if (!String.IsNullOrEmpty(SelectedItemHeaderTransformationName))
            {
                BasicRepeater.HeaderTemplate = TransformationHelper.LoadTransformation(this, SelectedItemHeaderTransformationName);
            }
            else
            {
                BasicRepeater.HeaderTemplate = null;
            }
        }
        else
        {
            // Apply transformations if they exist
            if (!String.IsNullOrEmpty(TransformationName))
            {
                BasicRepeater.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
            }
            if (!String.IsNullOrEmpty(AlternatingItemTransformationName))
            {
                BasicRepeater.AlternatingItemTemplate = TransformationHelper.LoadTransformation(this, AlternatingItemTransformationName);
            }
            if (!String.IsNullOrEmpty(FooterTransformationName))
            {
                BasicRepeater.FooterTemplate = TransformationHelper.LoadTransformation(this, FooterTransformationName);
            }
            if (!String.IsNullOrEmpty(HeaderTransformationName))
            {
                BasicRepeater.HeaderTemplate = TransformationHelper.LoadTransformation(this, HeaderTransformationName);
            }
            if (!String.IsNullOrEmpty(SeparatorTransformationName))
            {
                BasicRepeater.SeparatorTemplate = TransformationHelper.LoadTransformation(this, SeparatorTransformationName);
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

                BasicRepeater.DataSource = DataSourceControl.DataSource;
                LoadTransformations();
                BasicRepeater.DataBind();
            }
        }

        base.OnPreRender(e);

        // Hide control for zero rows
        if (((DataSourceControl == null) || DataHelper.DataSourceIsEmpty(ds)) && HideControlForZeroRows)
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
        EnsureFilterControl();
        base.ReloadData();
    }

    #endregion;
}
