using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.DocumentEngine.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSWebParts_Maps_Static_StaticBingMaps : CMSAbstractWebPart
{
    #region "Map properties"

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
    /// Gets or sets the tool tip text for single marker in detail mode.
    /// </summary>
    public string ToolTip
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ToolTip"), "");
        }
        set
        {
            SetValue("ToolTip", value);
        }
    }


    /// <summary>
    /// Gets or sets the content text for single marker in detail mode.
    /// </summary>
    public string Content
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Content"), "");
        }
        set
        {
            SetValue("Content", value);
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
    /// Gets or sets the Bing map key.
    /// </summary>
    public string MapKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MapKey"), "");
        }
        set
        {
            SetValue("MapKey", value);
        }
    }

    #endregion


    #region "Location properties"

    /// <summary>
    /// Gets or sets the default location of the center of the map and/or location of single marker in detail mode.
    /// </summary>
    public string Location
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Location"), "");
        }
        set
        {
            SetValue("Location", value);
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
    /// Gets or sets the latitude of the center of the map and/or latitude of single marker in detail mode.
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
    /// Gets or sets the longitude of the center of the map and/or longitude of single marker in detail mode.
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
    /// Gets or sets the URL for icon.
    /// </summary>
    public string IconURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("IconURL"), "");
        }
        set
        {
            SetValue("IconURL", value);
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            int result = ValidationHelper.GetInteger(GetValue("CacheMinutes"), -1);
            if (result < 0)
            {
                // If not set, get from the site settings
                result = SettingsKeyInfoProvider.GetIntValue(CurrentSiteName + ".CMSCacheMinutes");
            }
            return result;
        }
        set
        {
            SetValue("CacheMinutes", value);
        }
    }

    /// <summary>
    /// Cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CacheItemName"), "");
        }
        set
        {
            SetValue("CacheItemName", value);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// On init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Due to new design mode (with preview) we need to move map down for the user to be able to drag and drop the control
        if (PortalContext.IsDesignMode(PortalContext.ViewMode))
        {
            ltlDesign.Text = "<div class=\"WebpartDesignPadding\"></div>";
        }
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            // Load map
            LoadMap();

            // Register Bing javascript files
            BasicBingMaps.RegisterMapScripts(Page, ClientID, "~/CMSWebParts/Maps/Static/StaticBingMaps_files/BingMaps.js");
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        // Re-load map
        LoadMap();
    }


    /// <summary>
    /// Generates map code and registers Bing javascript files.
    /// </summary>
    public void LoadMap()
    {
        #region "Map properties"

        // Set map properties
        CMSMapProperties mp = new CMSMapProperties();
        mp.EnableMapDragging = EnableMapDragging;
        mp.ShowScaleControl = ShowScaleControl;
        mp.EnableServerProcessing = EnableServerProcessing;
        mp.ShowNavigationControl = ShowNavigationControl;
        mp.IconURL = IconURL;
        mp.MapKey = MapKey;
        mp.Latitude = Latitude;
        mp.Longitude = Longitude;
        mp.ZoomScale = ZoomScale;
        mp.Location = Location;
        mp.ToolTip = ToolTip;
        mp.MapType = MapType;
        mp.Content = Content;
        mp.Height = Height;
        mp.Scale = Scale;
        mp.Width = Width;
        mp.MapId = ClientID;
        mp.Page = Page;

        #endregion


        // Load map
        ltlBingMap.Text = BasicBingMaps.GenerateMap(mp, CacheMinutes, CacheItemName, InstanceGUID);
    }

    #endregion
}