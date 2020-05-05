using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.DataEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_Maps_Documents_BingMaps : CMSAbstractWebPart
{
    #region "Variables"

    private bool reloadData;

    #endregion


    #region "Location properties"

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

    #endregion


    #region "Map properties"

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


    #region "Document properties"

    /// <summary>
    /// Cache item name.
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
            ucDocumentSource.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, ucDocumentSource.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            ucDocumentSource.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Cache minutes.
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
            ucDocumentSource.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Check permissions.
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
            ucDocumentSource.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), ucDocumentSource.ClassNames), ucDocumentSource.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            ucDocumentSource.ClassNames = value;
        }
    }


    /// <summary>
    /// Combine with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), false);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            ucDocumentSource.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Filter out duplicate documents.
    /// </summary>
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), ucDocumentSource.FilterOutDuplicates);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            ucDocumentSource.FilterOutDuplicates = value;
        }
    }


    /// <summary>
    /// Culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), ucDocumentSource.CultureCode), ucDocumentSource.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            ucDocumentSource.CultureCode = value;
        }
    }


    /// <summary>
    /// Maximal relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), -1);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            ucDocumentSource.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), ucDocumentSource.OrderBy), ucDocumentSource.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            ucDocumentSource.OrderBy = value;
        }
    }


    /// <summary>
    /// Nodes path.
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
            ucDocumentSource.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), ""), "");
        }
        set
        {
            SetValue("TransformationName", value);
        }
    }


    /// <summary>
    /// Select only published nodes.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), true);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            ucDocumentSource.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), ""), SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
            ucDocumentSource.SiteName = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), ucDocumentSource.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            ucDocumentSource.WhereCondition = value;
        }
    }


    /// <summary>
    /// Select top N items.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), ucDocumentSource.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            ucDocumentSource.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates that if a page is selected,
    /// the datasource will provide data for the selected page only.
    /// </summary>
    public bool LoadCurrentPageOnly
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableSelectedItem"), true);
        }
        set
        {
            SetValue("EnableSelectedItem", value);
        }
    }

    #endregion


    #region "Relationships properties"

    /// <summary>
    /// Related node is on the left side.
    /// </summary>
    public bool RelatedNodeIsOnTheLeftSide
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), false);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            ucDocumentSource.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("RelationshipName"), ucDocumentSource.RelationshipName), ucDocumentSource.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            ucDocumentSource.RelationshipName = value;
        }
    }


    /// <summary>
    /// Relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), Guid.Empty);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            ucDocumentSource.RelationshipWithNodeGuid = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Hide control for zero rows.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), ucBingMap.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            ucBingMap.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Zero rows text.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), ucBingMap.ZeroRowsText), ucBingMap.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            ucBingMap.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets whethter datasource is empty or not.
    /// </summary>
    public bool HasData
    {
        get
        {
            return !StopProcessing && !DataHelper.DataSourceIsEmpty(ucDocumentSource.DataSource);
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
            ucDocumentSource.StopProcessing = value;
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
    ///  On load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!StopProcessing)
        {
            // Special case for inline widget
            ucBingMap.MapProperties.MapId = ClientID;
        }
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            ucBingMap.Visible = false;
        }
        else
        {
            #region "Data source properties"

            // Set Documents data source control
            ucDocumentSource.Path = Path;
            ucDocumentSource.ClassNames = ClassNames;
            ucDocumentSource.CombineWithDefaultCulture = CombineWithDefaultCulture;
            ucDocumentSource.CultureCode = CultureCode;
            ucDocumentSource.MaxRelativeLevel = MaxRelativeLevel;
            ucDocumentSource.OrderBy = OrderBy;
            ucDocumentSource.SelectOnlyPublished = SelectOnlyPublished;
            ucDocumentSource.SelectTopN = SelectTopN;
            ucDocumentSource.SiteName = SiteName;
            ucDocumentSource.WhereCondition = WhereCondition;
            ucDocumentSource.FilterOutDuplicates = FilterOutDuplicates;
            ucDocumentSource.CheckPermissions = CheckPermissions;
            ucDocumentSource.RelationshipName = RelationshipName;
            ucDocumentSource.RelationshipWithNodeGuid = RelationshipWithNodeGUID;
            ucDocumentSource.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
            ucDocumentSource.CacheDependencies = CacheDependencies;
            ucDocumentSource.CacheMinutes = CacheMinutes;
            ucDocumentSource.CacheItemName = CacheItemName;
            ucDocumentSource.LoadCurrentPageOnly = LoadCurrentPageOnly;

            #endregion


            #region "Bing map caching options"

            // Set caching options if server processing is enabled
            if (ucDocumentSource != null && EnableServerProcessing)
            {
                ucBingMap.CacheItemName = ucDocumentSource.CacheItemName;
                ucBingMap.CacheMinutes = ucDocumentSource.CacheMinutes;

                // Cache depends on data source and properties of web part
                ucBingMap.CacheDependencies = CacheHelper.GetCacheDependencies(CacheDependencies, ucDocumentSource.GetDefaultCacheDependencies());
            }

            #endregion


            #region "Map properties"

            // Set BasicBingMaps control
            CMSMapProperties mp = new CMSMapProperties();
            mp.EnableMapDragging = EnableMapDragging;
            mp.EnableServerProcessing = EnableServerProcessing;
            mp.Height = Height;
            mp.Latitude = Latitude;
            mp.LatitudeField = LatitudeField;
            mp.Location = DefaultLocation;
            mp.LocationField = LocationField;
            mp.Longitude = Longitude;
            mp.LongitudeField = LongitudeField;
            mp.MapId = ClientID;
            mp.MapKey = MapKey;
            mp.MapType = MapType;
            mp.Scale = Scale;
            mp.ShowNavigationControl = ShowNavigationControl;
            mp.ShowScaleControl = ShowScaleControl;
            mp.ToolTipField = ToolTipField;
            mp.IconField = IconField;
            mp.Width = Width;
            mp.ZoomScale = ZoomScale;

            ucBingMap.DataBindByDefault = false;
            ucBingMap.MapProperties = mp;
            ucBingMap.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
            ucBingMap.MainScriptPath = "~/CMSWebParts/Maps/Documents/BingMaps_files/BingMaps.js";
            ucBingMap.HideControlForZeroRows = HideControlForZeroRows;

            if (!String.IsNullOrEmpty(ZeroRowsText))
            {
                ucBingMap.ZeroRowsText = ZeroRowsText;
            }

            #endregion


            if (reloadData)
            {
                ucDocumentSource.DataSource = null;
            }

            // Connects Bing map with data source
            ucBingMap.DataSource = ucDocumentSource.DataSource;
            ucBingMap.RelatedData = ucDocumentSource.RelatedData;

            if (HasData)
            {
                ucBingMap.DataBind();
            }
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = ucBingMap.Visible;

        if (!HasData && HideControlForZeroRows)
        {
            Visible = false;
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
        SetupControl();
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public override void ReloadData()
    {
        ReloadData(false);
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    /// <param name="forceReload">Indicates if the reload should be forced</param>
    public void ReloadData(bool forceReload)
    {
        reloadData = forceReload;
        SetupControl();
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        ucDocumentSource.ClearCache();
    }

    #endregion
}