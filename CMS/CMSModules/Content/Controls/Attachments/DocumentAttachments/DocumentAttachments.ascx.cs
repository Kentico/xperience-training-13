using System;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Attachments_DocumentAttachments_DocumentAttachments : CMSUserControl
{
    #region "Variables"

    private string mPath;

    #endregion


    #region "Basic repeater properties"

    /// <summary>
    /// Gets the repeater control.
    /// </summary>
    public BasicRepeater Repeater
    {
        get
        {
            return ucRepeater;
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
            return ucRepeater.HideControlForZeroRows;
        }
        set
        {
            ucRepeater.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ucRepeater.ZeroRowsText;
        }
        set
        {
            ucRepeater.ZeroRowsText = value;
        }
    }

    #endregion


    #region "UniPager properties"

    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public bool HidePagerForSinglePage
    {
        get
        {
            return ucPager.HidePagerForSinglePage;
        }
        set
        {
            ucPager.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of records to display on a page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ucPager.PageSize;
        }
        set
        {
            ucPager.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return ucPager.GroupSize;
        }
        set
        {
            ucPager.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager mode.
    /// </summary>
    public UniPagerMode PagingMode
    {
        get
        {
            return ucPager.PagerMode;
        }
        set
        {
            ucPager.PagerMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the querysting parameter.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return ucPager.QueryStringKey;
        }
        set
        {
            ucPager.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return ucPager.DisplayFirstLastAutomatically;
        }
        set
        {
            ucPager.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return ucPager.DisplayPreviousNextAutomatically;
        }
        set
        {
            ucPager.DisplayPreviousNextAutomatically = value;
        }
    }

    #endregion


    #region "UniPager Template properties"

    /// <summary>
    /// Gets or sets the pages template.
    /// </summary>
    public string PagesTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Pages"), "");
        }
        set
        {
            SetValue("Pages", value);
        }
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CurrentPage"), "");
        }
        set
        {
            SetValue("CurrentPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageSeparator"), "");
        }
        set
        {
            SetValue("PageSeparator", value);
        }
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstPage"), "");
        }
        set
        {
            SetValue("FirstPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LastPage"), "");
        }
        set
        {
            SetValue("LastPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousPage"), "");
        }
        set
        {
            SetValue("PreviousPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextPage"), "");
        }
        set
        {
            SetValue("NextPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousGroup"), "");
        }
        set
        {
            SetValue("PreviousGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextGroup"), "");
        }
        set
        {
            SetValue("NextGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerLayout"), "");
        }
        set
        {
            SetValue("PagerLayout", value);
        }
    }

    #endregion


    #region "Data source properties"

    /// <summary>
    /// Gets the datasource control.
    /// </summary>
    public AttachmentsDataSource DataSource
    {
        get
        {
            return ucDataSource;
        }
    }


    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ucDataSource.WhereCondition;
        }
        set
        {
            ucDataSource.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets top N.
    /// </summary>
    public int TopN
    {
        get
        {
            return ucDataSource.TopN;
        }
        set
        {
            ucDataSource.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ucDataSource.SiteName;
        }
        set
        {
            ucDataSource.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ucDataSource.OrderBy;
        }
        set
        {
            ucDataSource.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ucDataSource.SourceFilterName;
        }
        set
        {
            ucDataSource.SourceFilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public string CacheItemName
    {
        get
        {
            return ucDataSource.CacheItemName;
        }
        set
        {
            ucDataSource.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public string CacheDependencies
    {
        get
        {
            return ucDataSource.CacheDependencies;
        }
        set
        {
            ucDataSource.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public int CacheMinutes
    {
        get
        {
            return ucDataSource.CacheMinutes;
        }
        set
        {
            ucDataSource.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets selected columns.
    /// </summary>
    public string SelectedColumns
    {
        get
        {
            return ucDataSource.SelectedColumns;
        }
        set
        {
            ucDataSource.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Tree provider instance used to access data. If no TreeProvider is assigned, a new TreeProvider instance is created.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get
        {
            return ucDataSource.TreeProvider;
        }
        set
        {
            ucDataSource.TreeProvider = value;
        }
    }


    /// <summary>
    /// Indicates whether select also binary content of the attachments.
    /// </summary>
    public bool GetBinary
    {
        get
        {
            return ucDataSource.GetBinary;
        }
        set
        {
            ucDataSource.GetBinary = value;
        }
    }


    /// <summary>
    /// Group GUID (document field GUID) of the grouped attachments.
    /// </summary>
    public Guid AttachmentGroupGUID
    {
        get
        {
            return ucDataSource.AttachmentGroupGUID;
        }
        set
        {
            ucDataSource.AttachmentGroupGUID = value;
        }
    }


    /// <summary>
    /// Form GUID of the temporary attachments.
    /// </summary>
    public Guid AttachmentFormGUID
    {
        get
        {
            return ucDataSource.AttachmentFormGUID;
        }
        set
        {
            ucDataSource.AttachmentFormGUID = value;
        }
    }


    /// <summary>
    /// ID of version history.
    /// </summary>
    public int DocumentVersionHistoryID
    {
        get
        {
            return ucDataSource.DocumentVersionHistoryID;
        }
        set
        {
            ucDataSource.DocumentVersionHistoryID = value;
        }
    }


    /// <summary>
    /// Culture code, such as en-us.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ucDataSource.CultureCode;
        }
        set
        {
            ucDataSource.CultureCode = value;
        }
    }


    /// <summary>
    /// Indicates if the document should be selected eventually from the default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ucDataSource.CombineWithDefaultCulture;
        }
        set
        {
            ucDataSource.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the alias path.
    /// </summary>
    public string Path
    {
        get
        {
            return mPath;
        }
        set
        {
            mPath = value;
            ucDataSource.Path = value;
        }
    }


    /// <summary>
    /// Allows you to specify whether to check permissions of the current user. If the value is 'false' (default value) no permissions are checked. Otherwise, only nodes for which the user has read permission are displayed.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ucDataSource.CheckPermissions;
        }
        set
        {
            ucDataSource.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets whethter datasource is empty or not.
    /// </summary>
    public bool HasData
    {
        get
        {
            return !DataHelper.DataSourceIsEmpty(ucRepeater.DataSource);
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
            ucDataSource.StopProcessing = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ucRepeater.DataBindByDefault = false;
        ucPager.PageControl = ucRepeater.ID;

        // Reload data
        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = ucRepeater.Visible;

        if (!HasData && HideControlForZeroRows)
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public void ReloadData()
    {
        ReloadData(false);
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    /// <param name="forceReload">Indicates if the rload should be forced</param>
    public void ReloadData(bool forceReload)
    {
        #region "Repeater template properties"

        // Apply transformations if they exist
        ucRepeater.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);

        if (!string.IsNullOrEmpty(AlternatingItemTransformationName))
        {
            ucRepeater.AlternatingItemTemplate = TransformationHelper.LoadTransformation(this, AlternatingItemTransformationName);
        }
        if (!string.IsNullOrEmpty(FooterTransformationName))
        {
            ucRepeater.FooterTemplate = TransformationHelper.LoadTransformation(this, FooterTransformationName);
        }
        if (!string.IsNullOrEmpty(HeaderTransformationName))
        {
            ucRepeater.HeaderTemplate = TransformationHelper.LoadTransformation(this, HeaderTransformationName);
        }
        if (!string.IsNullOrEmpty(SeparatorTransformationName))
        {
            ucRepeater.SeparatorTemplate = TransformationHelper.LoadTransformation(this, SeparatorTransformationName);
        }

        #endregion


        #region "UniPager template properties"

        // UniPager template properties
        if (!string.IsNullOrEmpty(PagesTemplate))
        {
            ucPager.PageNumbersTemplate = TransformationHelper.LoadTransformation(ucPager, PagesTemplate);
        }

        if (!string.IsNullOrEmpty(CurrentPageTemplate))
        {
            ucPager.CurrentPageTemplate = TransformationHelper.LoadTransformation(ucPager, CurrentPageTemplate);
        }

        if (!string.IsNullOrEmpty(SeparatorTemplate))
        {
            ucPager.PageNumbersSeparatorTemplate = TransformationHelper.LoadTransformation(ucPager, SeparatorTemplate);
        }

        if (!string.IsNullOrEmpty(FirstPageTemplate))
        {
            ucPager.FirstPageTemplate = TransformationHelper.LoadTransformation(ucPager, FirstPageTemplate);
        }

        if (!string.IsNullOrEmpty(LastPageTemplate))
        {
            ucPager.LastPageTemplate = TransformationHelper.LoadTransformation(ucPager, LastPageTemplate);
        }

        if (!string.IsNullOrEmpty(PreviousPageTemplate))
        {
            ucPager.PreviousPageTemplate = TransformationHelper.LoadTransformation(ucPager, PreviousPageTemplate);
        }

        if (!string.IsNullOrEmpty(NextPageTemplate))
        {
            ucPager.NextPageTemplate = TransformationHelper.LoadTransformation(ucPager, NextPageTemplate);
        }

        if (!string.IsNullOrEmpty(PreviousGroupTemplate))
        {
            ucPager.PreviousGroupTemplate = TransformationHelper.LoadTransformation(ucPager, PreviousGroupTemplate);
        }

        if (!string.IsNullOrEmpty(NextGroupTemplate))
        {
            ucPager.NextGroupTemplate = TransformationHelper.LoadTransformation(ucPager, NextGroupTemplate);
        }

        if (!string.IsNullOrEmpty(LayoutTemplate))
        {
            ucPager.LayoutTemplate = TransformationHelper.LoadTransformation(ucPager, LayoutTemplate);
        }

        #endregion


        if (forceReload)
        {
            ucDataSource.DataSource = null;
        }

        // Connects repeater with data source
        ucRepeater.DataSource = ucDataSource.DataSource;
        ucRepeater.RelatedData = ucDataSource.RelatedData;

        if (HasData)
        {
            ucRepeater.DataBind();
        }
    }


    /// <summary>
    /// Clears control cache.
    /// </summary>
    public void ClearCache()
    {
        ucDataSource.ClearCache();
    }
}