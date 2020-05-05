using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

using DataPager = CMS.DocumentEngine.Web.UI.DataPager;


public partial class CMSWebParts_Viewers_Documents_ImageGallery : CMSAbstractWebPart
{
    #region "Document properties"

    /// <summary>
    /// Gets or sest the cache item name.
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
            lstImages.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, lstImages.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            lstImages.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sest the cache minutes.
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
            lstImages.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), lstImages.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            lstImages.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), lstImages.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            lstImages.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), lstImages.CultureCode), lstImages.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            lstImages.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), lstImages.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            lstImages.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sest the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), lstImages.OrderBy), lstImages.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            lstImages.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), lstImages.Path);
        }
        set
        {
            SetValue("Path", value);
            lstImages.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), lstImages.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            lstImages.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), lstImages.SiteName), lstImages.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            lstImages.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), lstImages.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            lstImages.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), lstImages.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            lstImages.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string SelectedColumns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedColumns"), lstImages.SelectedColumns);
        }
        set
        {
            SetValue("SelectedColumns", value);
            lstImages.SelectedColumns = value;
        }
    }

    #endregion


    #region "Relationships properties"

    /// <summary>
    /// Gets or sets the value that indicates whether Related node is on the left side.
    /// </summary>
    public bool RelatedNodeIsOnTheLeftSide
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), lstImages.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            lstImages.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("RelationshipName"), lstImages.RelationshipName), lstImages.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            lstImages.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), lstImages.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            lstImages.RelationshipWithNodeGuid = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for image thumbnail.
    /// </summary>
    public string ThumbnailTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ThumbnailTransformation"), lstImages.TransformationName), lstImages.TransformationName);
        }
        set
        {
            SetValue("ThumbnailTransformation", value);
            lstImages.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for image detail.
    /// </summary>
    public string DetailTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("DetailTransformation"), lstImages.SelectedItemTransformationName), lstImages.SelectedItemTransformationName);
        }
        set
        {
            SetValue("DetailTransformation", value);
            lstImages.SelectedItemTransformationName = value;
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Gets the data pager control instance.
    /// </summary>
    public DataPager PagerControl
    {
        get
        {
            return lstImages.PagerControl;
        }
    }


    /// <summary>
    /// Gets or sets the pager position.
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return PagerControl.PagerPosition;
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            PagerControl.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of the documents displayed on each sigle page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), lstImages.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            lstImages.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("QueryStringKey"), PagerControl.QueryStringKey), PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public PagingModeTypeEnum PagingMode
    {
        get
        {
            return PagerControl.GetPagingMode(DataHelper.GetNotEmpty(GetValue("PagingMode"), PagerControl.PagingMode.ToString()));
        }
        set
        {
            SetValue("PagingMode", value.ToString());
            PagerControl.PagingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether  first and last page is shown if paging is allowed.
    /// </summary>
    public bool ShowFirstLast
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFirstLast"), PagerControl.ShowFirstLast);
        }
        set
        {
            SetValue("ShowFirstLast", value);
            PagerControl.ShowFirstLast = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether navigation buttons should be displayed on the top of the control.
    /// </summary>
    public bool ShowButtonsOnTop
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowButtonsOnTop"), true);
        }
        set
        {
            SetValue("ShowButtonsOnTop", value);
            RelocateButtons();
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), lstImages.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            lstImages.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), lstImages.ZeroRowsText), lstImages.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            lstImages.ZeroRowsText = value;
        }
    }

    #endregion


    #region "Repeat properties"

    /// <summary>
    /// Gets or sets the value that indicates number of columns to diplay.
    /// </summary>
    public int Columns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Columns"), lstImages.RepeatColumns);
        }
        set
        {
            SetValue("Columns", value);
            lstImages.RepeatColumns = value;
            PagerControl.PageSize = value * RowsPerPage;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates numbers of rows to display.
    /// </summary>
    public int RowsPerPage
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RowsPerPage"), 1);
        }
        set
        {
            SetValue("RowsPerPage", value);
            PagerControl.PageSize = Columns * value;
        }
    }


    /// <summary>
    /// Gets or sets whether control is displayed in a table or flow layout.
    /// </summary>
    public RepeatLayout RepeatLayout
    {
        get
        {
            return CMSDataList.GetRepeatLayout(DataHelper.GetNotEmpty(GetValue("RepeatLayout"), lstImages.RepeatLayout.ToString()));
        }
        set
        {
            SetValue("RepeatLayout", value);
            lstImages.RepeatLayout = value;
        }
    }


    /// <summary>
    /// Gets or sets whether the datalist control display verically or horizontally.
    /// </summary>
    public RepeatDirection RepeatDirection
    {
        get
        {
            return CMSDataList.GetRepeatDirection(DataHelper.GetNotEmpty(GetValue("RepeatDirection"), lstImages.RepeatDirection.ToString()));
        }
        set
        {
            SetValue("RepeatDirection", value);
            lstImages.RepeatDirection = value;
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
            lstImages.StopProcessing = value;
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
        // WAI validation
        lblImages.ResourceString = "ImageGallery.Images";
        lblImages.Attributes.Add("style", "display: none;");

        lstImages.PagerControl = pagerElem;
        pagerElem.OnDataSourceChanged += pagerElem_OnDataSourceChanged;

        if (StopProcessing)
        {
            lstImages.StopProcessing = true;
        }
        else
        {
            lstImages.ControlContext = ControlContext;

            // Set properties from Webpart form   
            lstImages.CacheItemName = CacheItemName;
            lstImages.CacheDependencies = CacheDependencies;
            lstImages.CacheMinutes = CacheMinutes;
            lstImages.CheckPermissions = CheckPermissions;
            lstImages.CombineWithDefaultCulture = CombineWithDefaultCulture;
            lstImages.CultureCode = CultureCode;
            lstImages.MaxRelativeLevel = MaxRelativeLevel;
            lstImages.OrderBy = OrderBy;
            lstImages.SelectOnlyPublished = SelectOnlyPublished;
            lstImages.SiteName = SiteName;
            lstImages.WhereCondition = WhereCondition;
            lstImages.SelectTopN = SelectTopN;
            lstImages.SelectedColumns = SelectedColumns;

            // Relationships
            lstImages.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
            lstImages.RelationshipName = RelationshipName;
            lstImages.RelationshipWithNodeGuid = RelationshipWithNodeGUID;

            // Transformation properties
            lstImages.TransformationName = ThumbnailTransformation;
            lstImages.Path = Path;

            // Pager properties
            lstImages.PageSize = Columns * RowsPerPage;
            PagerControl.PagerPosition = PagerPosition;
            PagerControl.QueryStringKey = QueryStringKey;
            PagerControl.PagingMode = PagingMode;
            PagerControl.ShowFirstLast = ShowFirstLast;
            lstImages.EnablePaging = true;

            // Repeat properties
            lstImages.RepeatColumns = Columns;
            lstImages.RepeatLayout = RepeatLayout;
            lstImages.RepeatDirection = RepeatDirection;

            // Public properties
            lstImages.HideControlForZeroRows = HideControlForZeroRows;
            lstImages.ZeroRowsText = ZeroRowsText;

            RelocateButtons();

            if (StandAlone)
            {
                lstImages.ReloadData(true);
            }
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = lstImages.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(lstImages.DataSource) && HideControlForZeroRows)
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reload date.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        lstImages.ReloadData(true);
    }


    /// <summary>
    /// OnDataSource chaged handler.
    /// </summary>
    private void pagerElem_OnDataSourceChanged(object sender, EventArgs e)
    {
        if (QueryHelper.Contains("imagepath"))
        {
            string path = QueryHelper.GetString("imagepath", "");

            PagingMode = PagingModeTypeEnum.QueryString;
            PagerControl.PageSize = 1;
            int currentPage = PagerControl.GetItemPage("NodeAliasPath = '" + SqlHelper.EscapeQuotes(path) + "'");
            PagerControl.CurrentPage = currentPage;
            PagerControl.Visible = false;
            lstImages.TransformationName = DetailTransformation;

            plcNavigation.Visible = true;

            lnkNext.Text = GetString("general.Next");
            DataRow nextItem = PagerControl.GetItemDataRow(currentPage);
            if (nextItem != null)
            {
                lnkNext.NavigateUrl = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "imagepath", (string)nextItem["NodeAliasPath"]);
            }

            lnkPrevious.Text = GetString("ImageGallery.Previous");
            DataRow prevItem = PagerControl.GetItemDataRow(currentPage - 2);
            if (prevItem != null)
            {
                lnkPrevious.NavigateUrl = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "imagepath", (string)prevItem["NodeAliasPath"]);
            }

            lnkThumbnails.Text = GetString("ImageGallery.Thumbnails");
            lnkThumbnails.NavigateUrl = URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, "imagepath");

            ltlScript.Text += ScriptHelper.GetScript("var drpElem = document.getElementById('" + drpImages.ClientID + "');");
            // Prepare the images dropdown list
            drpImages.Attributes.Add("onchange", "ChangeImage();");
            drpImages.Items.Clear();
            int index = 1;
            DataView images = (DataView)PagerControl.DataSource;
            foreach (DataRow dr in images.Table.Rows)
            {
                string url = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "imagepath", (string)dr["NodeAliasPath"]);
                drpImages.Items.Add(new ListItem(index.ToString(), url));
                index++;
            }
            drpImages.SelectedIndex = currentPage - 1;
            lblOf.Text = GetString("ImageGallery.Of");
            lblTotal.Text = images.Table.Rows.Count.ToString();
        }
    }


    /// <summary>
    /// Reloacate buttons.
    /// </summary>
    private void RelocateButtons()
    {
        plcButtonsTop.Controls.Remove(plcNavigation);
        plcButtonsBottom.Controls.Remove(plcNavigation);
        plcButtonsTop.Controls.Remove(plcPager);
        plcButtonsBottom.Controls.Remove(plcPager);

        if (!ShowButtonsOnTop)
        {
            plcButtonsBottom.Controls.Add(plcNavigation);
            plcButtonsBottom.Controls.Add(plcPager);
        }
        else
        {
            plcButtonsTop.Controls.Add(plcNavigation);
            plcButtonsTop.Controls.Add(plcPager);
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        lstImages.ClearCache();
    }
}
