using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_Maps_Basic_BasicGoogleMaps : CMSAbstractWebPart
{
    #region "Private variables"

    // Base datasource instance
    private CMSBaseDataSource mDataSourceControl;

    // Indicates whether control was binded
    private bool binded;

    // BasicGoogleMaps instance
    private BasicGoogleMaps BasicGoogleMaps = new BasicGoogleMaps();

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
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), BasicGoogleMaps.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            BasicGoogleMaps.HideControlForZeroRows = value;
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
            BasicGoogleMaps.ZeroRowsText = value;
        }
    }

    #endregion


    #region "Map properties"

    /// <summary>
    /// Api key for communicating with Google services.
    /// </summary>
    public string ApiKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ApiKey"), "");
        }
        set
        {
            SetValue("ApiKey", value);
        }
    }


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
    /// Gets or sets the value that indicates whether MapTypeControl is displayed.
    /// </summary>
    public bool ShowMapTypeControl
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowMapTypeControl"), true);
        }
        set
        {
            SetValue("ShowMapTypeControl", value);
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
    /// Gets or sets the value that indicates whether NavigationControl is displayed. This property is used only for backward compatibility.
    /// </summary>
    private bool ShowNavigationControl
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNavigationControl"), true);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether Zoom control is displayed.
    /// </summary>
    public bool ShowZoomControl
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowZoomControl"), ShowNavigationControl);
        }
        set
        {
            SetValue("ShowZoomControl", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether Street view control is displayed.
    /// </summary>
    public bool ShowStreetViewControl
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowStreetViewControl"), ShowNavigationControl);
        }
        set
        {
            SetValue("ShowStreetViewControl", value);
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
    /// Gets or sets the value that indicates whether the keyboard shortcuts are enabled.
    /// </summary>
    public bool EnableKeyboardShortcuts
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableKeyboardShortcuts"), true);
        }
        set
        {
            SetValue("EnableKeyboardShortcuts", value);
        }
    }


    /// <summary>
    /// Gets or sets the initial map type.
    /// ROADMAP - This map type displays a normal street map.
    /// SATELLITE - This map type displays a transparent layer of major streets on satellite images.
    /// HYBRID - This map type displays a transparent layer of major streets on satellite images.
    /// TERRAIN - This map type displays maps with physical features such as terrain and vegetation.
    /// </summary>
    public string MapType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MapType"), "ROADMAP");
        }
        set
        {
            SetValue("MapType", value);
        }
    }


    /// <summary>
    /// The Zoom control may appear in one of the following style options:
    /// Default picks an appropriate navigation control based on the map's size and the device on which the map is running.
    /// Small displays a mini-zoom control, consisting of only + and - buttons. This style is appropriate for small maps.
    /// Large displays the standard zoom slider control.
    /// </summary>
    public int ZoomControlType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ZoomControlType"), 0);
        }
        set
        {
            SetValue("ZoomControlType", value);
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
    /// Gets or sets the default location of the center of the map.
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
            plcBasicGoogleMaps.Controls.Add(ltlDesign);
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
                BasicGoogleMaps.CacheItemName = DataSourceControl.CacheItemName;
                BasicGoogleMaps.CacheMinutes = DataSourceControl.CacheMinutes;

                // Cache depends only on data source and properties of data source web part
                string cacheDependencies = CacheHelper.GetCacheDependencies(DataSourceControl.CacheDependencies, DataSourceControl.GetDefaultCacheDependencies());
                // All view modes, except LiveSite mode
                if (PortalContext.ViewMode != ViewModeEnum.LiveSite)
                {
                    // Cache depends on data source, properties of data source web part and properties of BasicGoogleMaps web part
                    cacheDependencies += "webpartinstance|" + InstanceGUID.ToString().ToLowerCSafe();
                }
                BasicGoogleMaps.CacheDependencies = cacheDependencies;
            }

            #endregion


            #region "Map properties"

            CMSMapProperties mp = new CMSMapProperties();
            mp.Location = DefaultLocation;
            mp.EnableKeyboardShortcuts = EnableKeyboardShortcuts;
            mp.EnableMapDragging = EnableMapDragging;
            mp.Height = Height;
            mp.Width = Width;
            mp.EnableServerProcessing = EnableServerProcessing;
            mp.Longitude = Longitude;
            mp.Latitude = Latitude;
            mp.LatitudeField = LatitudeField;
            mp.LongitudeField = LongitudeField;
            mp.LocationField = LocationField;
            mp.MapType = MapType;
            mp.ZoomControlType = ZoomControlType;
            mp.Scale = Scale;
            mp.ShowZoomControl = ShowZoomControl;
            mp.ShowStreetViewControl = ShowStreetViewControl;
            mp.ShowScaleControl = ShowScaleControl;
            mp.ShowMapTypeControl = ShowMapTypeControl;
            mp.ToolTipField = ToolTipField;
            mp.IconField = IconField;
            mp.ZoomScale = ZoomScale;
            mp.MapId = ClientID;

            #endregion

            BasicGoogleMaps.ApiKey = ApiKey;
            BasicGoogleMaps.MapProperties = mp;
            BasicGoogleMaps.HideControlForZeroRows = HideControlForZeroRows;
            BasicGoogleMaps.DataBindByDefault = false;
            BasicGoogleMaps.MainScriptPath = "~/CMSWebParts/Maps/Basic/BasicGoogleMaps_files/GoogleMaps.js";

            // Add basic maps control to the filter collection
            EnsureFilterControl();

            if (!String.IsNullOrEmpty(ZeroRowsText))
            {
                BasicGoogleMaps.ZeroRowsText = ZeroRowsText;
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
                    BasicGoogleMaps.DataSource = DataSourceControl.DataSource;
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
                BasicGoogleMaps.DataBind();
            }
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


    /// <summary>
    /// Loads and setups web part.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        if (!StopProcessing)
        {
            // Add control to the control collection
            plcBasicGoogleMaps.Controls.Add(BasicGoogleMaps);

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
                        BasicGoogleMaps.DataSource = DataSourceControl.DataSource;
                        BasicGoogleMaps.DataBind();
                        binded = true;
                    }
                }
            }

            //Handle filter change event
            if (DataSourceControl != null)
            {
                DataSourceControl.OnFilterChanged += DataSourceControl_OnFilterChanged;
            }

            // Specify mapId for inline widget
            BasicGoogleMaps.MapProperties.MapId = ClientID;
        }

        base.OnLoad(e);
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
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), BasicGoogleMaps);
            mFilterControlAdded = true;
        }
    }


    /// <summary>
    /// Load transformations with dependence on current datasource type and datasource type.
    /// </summary>
    protected void LoadTransformations()
    {
        CMSBaseDataSource dataSource = DataSourceControl as CMSBaseDataSource;

        if ((dataSource != null) && !String.IsNullOrEmpty(TransformationName))
        {
            BasicGoogleMaps.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
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
            BasicGoogleMaps.DataSource = DataSourceControl.DataSource;
            BasicGoogleMaps.DataBind();
            binded = true;
        }
    }

    #endregion;
}