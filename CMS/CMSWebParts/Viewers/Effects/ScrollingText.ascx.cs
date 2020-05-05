using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Effects_ScrollingText : CMSAbstractWebPart
{
    #region "Variables"

    protected string mStyleOptions;
    protected int totalItems = 0;

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
            return DataHelper.GetNotEmpty(GetValue("Path"), repItems.Path);
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

    #endregion


    #region "Div properties"

    /// <summary>
    /// Gets or sets the DIV element width.
    /// </summary>
    public int DivWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DivWidth"), 400);
        }
        set
        {
            SetValue("DivWidth", value);
        }
    }


    /// <summary>
    /// Gets or sets the DIV element height.
    /// </summary>
    public int DivHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DivHeight"), 300);
        }
        set
        {
            SetValue("DivHeight", value);
        }
    }


    /// <summary>
    /// Gets or sets the DIV element style.
    /// </summary>
    public string DivStyle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DivStyle"), "");
        }
        set
        {
            SetValue("DivStyle", value);
        }
    }


    /// <summary>
    /// Style options.
    /// </summary>
    private string StyleOptions
    {
        get
        {
            return mStyleOptions ?? (mStyleOptions = "width: " + DivWidth + "px; height: " + DivHeight + "px; overflow: hidden; position: absolute; left: 0px;");
        }
    }

    #endregion


    #region "Javascript properties"

    /// <summary>
    /// Gets or sets the text moving interval (higher value = slower scrolling ).
    /// </summary>
    public int JsMoveTime
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("JsMoveTime"), 1000);
        }
        set
        {
            SetValue("JsMoveTime", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates how long should be item stopped.
    /// </summary>
    public int JsStopTime
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("JsStopTime"), 3000);
        }
        set
        {
            SetValue("JsStopTime", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates moving direction.
    /// </summary>
    public int JsDirection
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("JsDirection"), 0);
        }
        set
        {
            SetValue("JsDirection", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates moving direction.
    /// </summary>
    public bool JsOnMouseStop
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("JsOnMouseStop"), true);
        }
        set
        {
            SetValue("JsOnMouseStop", value);
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
        }
    }

    #endregion


    #region "Methods"

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
        }
        else
        {
            // Set repeater
            repItems.ControlContext = ControlContext;

            // Document properties
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


            // Relationships
            repItems.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
            repItems.RelationshipName = RelationshipName;
            repItems.RelationshipWithNodeGuid = RelationshipWithNodeGUID;

            // Transformation properties
            repItems.TransformationName = TransformationName;
            repItems.AlternatingTransformationName = AlternatingTransformationName;

            // Public properties
            repItems.HideControlForZeroRows = HideControlForZeroRows;
            repItems.ZeroRowsText = ZeroRowsText;
            repItems.ItemSeparator = ItemSeparator;

            repItems.NestedControlsID = NestedControlsID;

            // Register MooTools javascript framework
            ScriptHelper.RegisterMooTools(Page);
        }
    }


    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        repItems.ReloadData(true);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            string divScript = "<div style=\"" +
                               "width: " + (DivWidth) + "px; " +
                               "height: " + DivHeight + "px; " +
                               "overflow: hidden; " +
                               "z-index: 0; " + (DivStyle) + "\">" +
                               "<div id=\"" + ClientID + "\" style=\"" +
                               "width: " + (DivWidth) + "px; " +
                               "height: " + DivHeight + "px; " +
                               "overflow:hidden;" +
                               "visibility:hidden;" +
                               "position:relative;\">";

            ltlBefore.Text = divScript;
            ltlAfter.Text = "</div></div>";

            // Register Slider javascript
            ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Viewers/Effects/ScrollingText_files/ScrollingText.js");

            // Build Javascript
            string jScript =
                @"window.addEvent('load', function(){
                    try {
                        var scroller_" + ClientID + " = new Sroller('" + ClientID + "'," + JsDirection + "," + JsMoveTime + "," + JsStopTime + ",'" + JsOnMouseStop + "'," + DivWidth + "," + DivHeight + @");
                        if (scrollernodes['" + ClientID + @"'].length != 0) 
                        { 
                            startScroller(scroller_" + ClientID + "," + (JsMoveTime + JsStopTime) + @", false); 
                        } 
                    } catch (ex) {}
                });";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ("scrollingScript" + ClientID), ScriptHelper.GetScript(jScript));

            // Hide control based on repeater datasource and HideControlForZeroRows property
            if (!repItems.HasData())
            {
                if (!HideControlForZeroRows)
                {
                    lblNoData.Text = ZeroRowsText;
                    lblNoData.Visible = true;
                }
                else
                {
                    Visible = false;
                }
            }
            else
            {
                Visible = repItems.Visible;
            }
        }
    }


    /// <summary>
    /// Handle item data bound.
    /// </summary>
    protected void repItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        // Add the envelope
        if ((e.Item.Controls.Count > 0) && !(e.Item.Controls[0] is LiteralControl))
        {
            e.Item.Controls[0].Controls.AddAt(0, new LiteralControl("<div class=\"scrollerContent\" style=\"" + StyleOptions + "\" >"));
            e.Item.Controls[0].Controls.Add(new LiteralControl("</div>"));
        }
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
