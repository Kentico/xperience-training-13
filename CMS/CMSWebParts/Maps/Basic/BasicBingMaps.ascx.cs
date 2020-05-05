using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_Maps_Basic_BasicBingMaps : CMSAbstractWebPart
{
    #region "Private variables"

    // Base datasource instance
    private CMSBaseDataSource mDataSourceControl;

    // Indicates whether control was binded
    private bool binded;

    // BasicBingMaps instance
    private readonly BasicBingMaps BasicBingMaps = new BasicBingMaps();

    // Indicates whether current control was added to the filter collection
    private bool mFilterControlAdded;

    #endregion


    #region "Public properties"

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

                    // If not found, try to get data source control according to ClientID or find the control on page
                    if (mDataSourceControl == null)
                    {
                        Control parent = Parent;
                        // Find control on page
                        while (parent != null && mDataSourceControl == null)
                        {
                            Control dataSource = parent.FindControl(DataSourceName);
                            if (dataSource != null)
                            {
                                try
                                {
                                    mDataSourceControl = CMSControlsHelper.GetFilter(dataSource.ClientID) as CMSBaseDataSource;
                                    if (mDataSourceControl == null)
                                    {
                                        mDataSourceControl = dataSource as CMSBaseDataSource;
                                    }
                                }
                                catch
                                {
                                }
                            }
                            parent = parent.Parent;
                        }
                    }
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
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), BasicBingMaps.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            BasicBingMaps.HideControlForZeroRows = value;
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
            BasicBingMaps.ZeroRowsText = value;
        }
    }

    #endregion


    #region "Map properties"

    /// <summary>
    /// Gets or sets the latitude of of the center of the map.
    /// </summary>
    public double? Latitude
    {
        get
        {
            object objLat = GetValue("Latitude");
            string lat = DataHelper.GetNotEmpty(objLat, "");
            if (string.IsNullOrEmpty(lat))
            {
                return null;
            }

            return ValidationHelper.GetDoubleSystem(objLat, 0);
        }
        set
        {
            SetValue("Latitude", value);
        }
    }


    /// <summary>
    /// Gets or sets the longitude of of the center of the map.
    /// </summary>
    public double? Longitude
    {
        get
        {
            object objLng = GetValue("Longitude");
            string lng = DataHelper.GetNotEmpty(objLng, "");
            if (string.IsNullOrEmpty(lng))
            {
                return null;
            }

            return ValidationHelper.GetDoubleSystem(objLng, 0);
        }
        set
        {
            SetValue("Longitude", value);
        }
    }


    /// <summary>
    /// Gets or sets the scale of the map.
    /// </summary>
    public int Scale
    {
        get
        {
            int value = ValidationHelper.GetInteger(GetValue("Scale"), 3);
            if (value < 0)
            {
                value = 7;
            }
            return value;
        }
        set
        {
            SetValue("Scale", value);
        }
    }


    /// <summary>
    /// Gets or sets the scale of the map when zoomed (after marker click event).
    /// </summary>
    public int ZoomScale
    {
        get
        {
            int value = ValidationHelper.GetInteger(GetValue("ZoomScale"), 10);
            if (value < 0)
            {
                value = 10;
            }
            return value;
        }
        set
        {
            SetValue("ZoomScale", value);
        }
    }


    /// <summary>
    /// Gets or sets the source latitude field.
    /// </summary>
    public string LatitudeField
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LatitudeField"), "");
        }
        set
        {
            SetValue("LatitudeField", value);
        }
    }


    /// <summary>
    /// Gets or sets the source longitude field.
    /// </summary>
    public string LongitudeField
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LongitudeField"), "");
        }
        set
        {
            SetValue("LongitudeField", value);
        }
    }


    /// <summary>
    /// Gets or sets the tool tip text field (filed for markers tool tip text).
    /// </summary>
    public string ToolTipField
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ToolTipField"), "");
        }
        set
        {
            SetValue("ToolTipField", value);
        }
    }


    /// <summary>
    /// Gets or sets the icon field (fieled for icon URL).
    /// </summary>
    public string IconField
    {
        get
        {
            return ValidationHelper.GetString(GetValue("IconField"), "");
        }
        set
        {
            SetValue("IconField", value);
        }
    }


    /// <summary>
    /// Gets or sets the height of the map.
    /// </summary>
    public string Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Height"), "400");
        }
        set
        {
            SetValue("Height", value);
        }
    }


    /// <summary>
    /// Gets or sets the width of the map.
    /// </summary>
    public string Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Width"), "400");
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether NavigationControl is displayed.
    /// </summary>
    public bool ShowNavigationControl
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNavigationControl"), true);
        }
        set
        {
            SetValue("ShowNavigationControl", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether ScaleControl is displayed.
    /// </summary>
    public bool ShowScaleControl
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowScaleControl"), true);
        }
        set
        {
            SetValue("ShowScaleControl", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the user can drag the map with the mouse. 
    /// </summary>
    public bool EnableMapDragging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableMapDragging"), true);
        }
        set
        {
            SetValue("EnableMapDragging", value);
        }
    }


    /// <summary>
    /// Gets or sets the initial map type. 
    /// Road - The road map style. 
    /// Shaded - The shaded map style, which is a road map with shaded contours. 
    /// Aerial - The aerial map style. 
    /// Hybrid - The hybrid map style, which is an aerial map with a label overlay. 
    /// Birdseye - The bird's eye (oblique-angle) imagery map style. 
    /// BirdseyeHybrid - The bird's eye hybrid map style, which is a bird's eye map with a label overlay. 
    /// </summary>
    public string MapType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MapType"), "road");
        }
        set
        {
            SetValue("MapType", value);
        }
    }

    #endregion


    #region "Advanced map properties"

    /// <summary>
    /// Gets or sets the address field.
    /// </summary>
    public string LocationField
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LocationField"), string.Empty);
        }
        set
        {
            SetValue("LocationField", value);
        }
    }


    /// <summary>
    /// Gets or sets the default location of center of the map.
    /// </summary>
    public string DefaultLocation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultLocation"), string.Empty);
        }
        set
        {
            SetValue("DefaultLocation", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether server processing is enabled.
    /// </summary>
    public bool EnableServerProcessing
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableServerProcessing"), false);
        }
        set
        {
            SetValue("EnableServerProcessing", value);
        }
    }


    /// <summary>
    /// Gets or sets the Bing map key.
    /// </summary>
    public string MapKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MapKey"), string.Empty);
        }
        set
        {
            SetValue("MapKey", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// On init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Due to new design mode (with preview) we need to move map down for the user to be able to drag and drop the control
        if (PortalContext.IsDesignMode(PortalContext.ViewMode))
        {
            Label ltlDesign = new Label();
            ltlDesign.ID = "ltlDesig";
            ltlDesign.Text = "<div class=\"WebpartDesignPadding\"></div>";
            plcBasicBingMaps.Controls.Add(ltlDesign);
        }
    }


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
        if (!StopProcessing)
        {
            #region "Caching options"

            // Set caching options if server processing is enabled
            if (DataSourceControl != null && EnableServerProcessing)
            {
                BasicBingMaps.CacheItemName = DataSourceControl.CacheItemName;
                BasicBingMaps.CacheMinutes = DataSourceControl.CacheMinutes;

                // Cache depends only on data source and properties of data source web part
                string cacheDependencies = CacheHelper.GetCacheDependencies(DataSourceControl.CacheDependencies, DataSourceControl.GetDefaultCacheDependencies());
                // All view modes, except LiveSite mode
                if (PortalContext.ViewMode != ViewModeEnum.LiveSite)
                {
                    // Cache depends on data source, properties of data source web part and properties of BasicBingMaps web part
                    cacheDependencies += "webpartinstance|" + InstanceGUID.ToString().ToLowerCSafe();
                }
                BasicBingMaps.CacheDependencies = cacheDependencies;
            }

            #endregion


            #region "Map properties"

            CMSMapProperties mp = new CMSMapProperties();
            mp.Location = DefaultLocation;
            mp.EnableMapDragging = EnableMapDragging;
            mp.Height = Height;
            mp.Width = Width;
            mp.EnableServerProcessing = EnableServerProcessing;
            mp.Longitude = Longitude;
            mp.Latitude = Latitude;
            mp.LatitudeField = LatitudeField;
            mp.LongitudeField = LongitudeField;
            mp.LocationField = LocationField;
            mp.MapKey = MapKey;
            mp.MapType = MapType;
            mp.Scale = Scale;
            mp.ShowNavigationControl = ShowNavigationControl;
            mp.ShowScaleControl = ShowScaleControl;
            mp.ToolTipField = ToolTipField;
            mp.IconField = IconField;
            mp.ZoomScale = ZoomScale;
            mp.MapId = ClientID;

            #endregion


            BasicBingMaps.MapProperties = mp;
            BasicBingMaps.HideControlForZeroRows = HideControlForZeroRows;
            BasicBingMaps.DataBindByDefault = false;
            BasicBingMaps.MainScriptPath = "~/CMSWebParts/Maps/Basic/BasicBingMaps_files/BingMaps.js";

            // Add basic maps control to the filter collection
            EnsureFilterControl();

            if (!String.IsNullOrEmpty(ZeroRowsText))
            {
                BasicBingMaps.ZeroRowsText = ZeroRowsText;
            }
        }
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Datasource data
        object ds = null;

        if (!StopProcessing)
        {
            // Set transformations if data source is not empty
            if (DataSourceControl != null)
            {
                // Get data from datasource
                ds = DataSourceControl.DataSource;

                // Check whether data exist
                if ((!DataHelper.DataSourceIsEmpty(ds)) && (!binded))
                {
                    // Initilaize related data if provided
                    if (DataSourceControl.RelatedData != null)
                    {
                        RelatedData = DataSourceControl.RelatedData;
                    }
                    LoadTransformations();
                    BasicBingMaps.DataSource = DataSourceControl.DataSource;
                }
            }
        }

        base.OnPreRender(e);

        if (!StopProcessing)
        {
            // Hide control for zero rows
            if (((DataSourceControl == null) || (DataHelper.DataSourceIsEmpty(ds))) && (HideControlForZeroRows))
            {
                Visible = false;
            }
            else
            {
                BasicBingMaps.DataBind();
            }
        }
    }


    /// <summary>
    /// Loads and setups web part.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        if (!StopProcessing)
        {
            // Add control to the control collection
            plcBasicBingMaps.Controls.Add(BasicBingMaps);

            // Check whether postback was executed from current transformation item
            if (RequestHelper.IsPostBack())
            {
                // Indicates whether postback was fired from current control
                bool bindControl = false;

                // Check event target value and callback parameter value
                string eventTarget = ValidationHelper.GetString(Request.Form[Page.postEventSourceID], String.Empty);
                string callbackParam = ValidationHelper.GetString(Request.Form["__CALLBACKPARAM"], String.Empty);
                if (eventTarget.StartsWithCSafe(UniqueID) || callbackParam.StartsWithCSafe(UniqueID) || eventTarget.EndsWithCSafe("_contextMenuControl"))
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
                    // Reload data
                    if (DataSourceControl != null)
                    {
                        LoadTransformations();
                        BasicBingMaps.DataSource = DataSourceControl.DataSource;
                        BasicBingMaps.DataBind();
                        binded = true;
                    }
                }
            }

            //Handle filter change event
            if (DataSourceControl != null)
            {
                DataSourceControl.OnFilterChanged += DataSourceControl_OnFilterChanged;
            }

            // Specify map id for inline widget
            BasicBingMaps.MapProperties.MapId = ClientID;
        }

        base.OnLoad(e);
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

    #endregion


    #region "Private methods"

    /// <summary>
    /// Ensures current control in the filters collection.
    /// </summary>
    protected void EnsureFilterControl()
    {
        if (!mFilterControlAdded)
        {
            // Add basic repeater to the filter collection
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), BasicBingMaps);
            mFilterControlAdded = true;
        }
    }


    /// <summary>
    /// Load transformations with dependence on current datasource type and datasource type.
    /// </summary>
    protected void LoadTransformations()
    {
        CMSBaseDataSource dataSource = DataSourceControl;

        if ((dataSource != null) && !String.IsNullOrEmpty(TransformationName))
        {
            BasicBingMaps.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
        }
    }


    /// <summary>
    /// OnFilter change event handler.
    /// </summary>
    protected void DataSourceControl_OnFilterChanged()
    {
        // Set forcibly visibility
        Visible = true;

        // Reload data
        if (DataSourceControl != null)
        {
            LoadTransformations();
            BasicBingMaps.DataSource = DataSourceControl.DataSource;
            BasicBingMaps.DataBind();
            binded = true;
        }
    }

    #endregion;
}