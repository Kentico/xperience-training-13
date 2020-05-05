using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Effects_ContentSlider : CMSAbstractWebPart
{
    #region "Variables"

    private string mStyleOptions;
    private int index = 0;

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
    /// Gets or sets the value that inidcates whether related node is on the left side.
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
    /// Gets or sets the text which is displayed for zero rows result.
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


    /// <summary>
    /// Indicates if page numbers should be visible.
    /// </summary>
    public bool DisplayPageNumbers
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPageNumbers"), true);
        }
        set
        {
            SetValue("DisplayPageNumbers", value);
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
            return mStyleOptions ?? (mStyleOptions = "width: " + DivWidth + "px; height: " + DivHeight + "px; overflow: hidden; position: relative; left: 0px;");
        }
    }

    #endregion


    #region "Javascript properties"

    /// <summary>
    /// Gets or sets the script FadeIn time.
    /// </summary>
    public int JsFadeIn
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("JsFadeIn"), 250);
        }
        set
        {
            SetValue("JsFadeIn", value);
        }
    }


    /// <summary>
    /// Gets or sets the script FadeOut time.
    /// </summary>
    public int JsFadeOut
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("JsFadeOut"), 250);
        }
        set
        {
            SetValue("JsFadeOut", value);
        }
    }


    /// <summary>
    /// Gets or sets the script Break time.
    /// </summary>
    public int JsBreak
    {
        get
        {
            int mBreak = ValidationHelper.GetInteger(GetValue("JsBreak"), 3000);
            if (mBreak < 100)
            {
                mBreak = 100;
            }
            return mBreak;
        }
        set
        {
            SetValue("JsBreak", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether sliding should be started automatically.
    /// </summary>
    public bool JsAutoStart
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("JsAutoStart"), false);
        }
        set
        {
            SetValue("JsAutoStart", value);
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


    #region "Overidden methods"

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
        repItems.ReloadData(true);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        repItems.ClearCache();
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
            repItems.StopProcessing = true;
        }
        else
        {
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

            repItems.ItemDataBound += repItems_ItemDataBound;

            // Register MooTools javascript framework
            ScriptHelper.RegisterMooTools(Page);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnPreRender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            ltlBefore.Text = "<div class=\"Slider\"><div class=\"Content\" id=\"" + ClientID + "\" style=\"" + StyleOptions + "\">";

            // Register Slider javascript
            ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Viewers/Effects/ContentSlider_files/ContentSlider.js");

            // Build Javascript
            string jScript =
                "var CurrentPage_" + ClientID + " = null; var Slider_" + ClientID + " = null; window.addEvent('domready',function(){ \n" +
                "try { \n" +
                "Slider_" + ClientID + " = new ContentSlider(\"" + ClientID + "\"," + JsFadeIn + "," + JsFadeOut + "," + JsBreak + "); \n";

            if ((index != 0) && (JsAutoStart))
            {
                jScript += "autoTurnPage(Slider_" + ClientID + ",0," + (JsFadeIn + JsFadeOut + JsBreak) + ", false); \n";
            }

            // Set back and width of bottom div
            jScript +=
                "var tmp = $('" + ClientID + "'); \n" +
                "tmp.style.backgroundColor = $('" + ClientID + "_page_0').style.borderTopColor; \n" +
                // Get element width
                "var elWidth = 0; if(!isNaN(parseInt(tmp.style.width.substring(0,tmp.style.width.length - 2), 10))){elWidth=parseInt(tmp.style.width.substring(0,tmp.style.width.length - 2), 10); }" +
                // Get border width
                "var borderWidth = 0; if(!isNaN(parseInt($('" + ClientID + "_page_0').style.borderLeftWidth.substring(0, $('" + ClientID + "_page_0').style.borderLeftWidth.length - 2), 10))){borderWidth=parseInt($('" + ClientID + "_page_0').style.borderLeftWidth.substring(0, $('" + ClientID + "_page_0').style.borderLeftWidth.length - 2), 10);}" +
                // Set total width
                "tmp.style.width = (elWidth+(2*borderWidth))+\"px\"; \n";

            for (int i = 0; i < index; i++)
            {
                jScript += "$('" + ClientID + "_page_" + i + "').addEvent('click',function(){Slider_" + ClientID + ".turnPage(" + i + ",false);});\n";
            }

            jScript += "} catch (ex) {}});\n";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "sliderScript" + ClientID, ScriptHelper.GetScript(jScript));

            string bottomDiv = "</div>";
            if (DisplayPageNumbers && (index > 0))
            {
                // DIV with links to pages
                bottomDiv += "<div id=\"" + ClientID + "_pager\" class=\"Pager\" style=\"width:" + DivWidth + "px;\">";
                // Page numbers
                for (int p = 0; p < index; p++)
                {
                    bottomDiv += "<div class=\"PagerPage\" style=\"width:10px;\"><a id=\"" + ClientID + "_page_" + p + "\" href=\"#\" onclick=\"CurrentPage_" + ClientID + "=" + p + ";document.getElementById('" + ClientID + "_runSlider').style.display='inline';return false;\">" + (p + 1) + "</a></div>";
                }
                // Add start link
                bottomDiv += "<div style=\"display:none;\" id=\"" + ClientID + "_runSlider\" class=\"Control\"><a href=\"#\" onclick=\"document.getElementById('" + ClientID + "_runSlider').style.display='none';autoTurnPage(Slider_" + ClientID + ",CurrentPage_" + ClientID + "," + (JsFadeIn + JsFadeOut + JsBreak) + ", false); return false;\" >" + GetString("ContentSlider.Start") + "</a></div>";
                bottomDiv += "<div style=\"clear:both;height:0;line-height:0;\"></div></div>";
            }

            ltlAfter.Text = bottomDiv + "</div>";

            Visible = repItems.Visible;
            if (!repItems.HasData() && HideControlForZeroRows)
            {
                Visible = false;
            }
        }
        else
        {
            Visible = false;
        }
    }

    #endregion


    #region "Other events"

    /// <summary>
    /// Items databound.
    /// </summary>
    protected void repItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        // Add the envelope
        e.Item.Controls.AddAt(0, new LiteralControl("<div id=\"" + ClientID + "_" + index + "_content\" class=\"ContentPage\" style=\"" + StyleOptions + "position:absolute;\" >"));
        e.Item.Controls.Add(new LiteralControl("</div>"));
        index++;
    }

    #endregion
}
