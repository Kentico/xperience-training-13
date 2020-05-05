using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.PortalEngine;

public partial class CMSWebParts_Attachments_DocumentAttachmentsWithEffect : CMSAbstractWebPart
{
    #region "Basic repeater properties"

    /// <summary>
    /// Gets or sets AlternatingItemTemplate property.
    /// </summary>
    public string AlternatingItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternatingItemTransformationName"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("FooterTransformationName"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("HeaderTransformationName"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("TransformationName"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("SeparatorTransformationName"), string.Empty);
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
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), ucRepeater.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
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
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), ucRepeater.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
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
            return ValidationHelper.GetBoolean(GetValue("HidePagerForSinglePage"), ucPager.HidePagerForSinglePage);
        }
        set
        {
            SetValue("HidePagerForSinglePage", value);
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
            return ValidationHelper.GetInteger(GetValue("PageSize"), ucPager.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
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
            return ValidationHelper.GetInteger(GetValue("GroupSize"), ucPager.GroupSize);
        }
        set
        {
            SetValue("GroupSize", value);
            ucPager.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager mode ('querystring' or 'postback').
    /// </summary>
    public string PagingMode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagingMode"), "querystring");
        }
        set
        {
            if (value != null)
            {
                SetValue("PagingMode", value);
                switch (value.ToLowerCSafe())
                {
                    case "postback":
                        ucPager.PagerMode = UniPagerMode.PostBack;
                        break;
                    default:
                        ucPager.PagerMode = UniPagerMode.Querystring;
                        break;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the querysting parameter.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("QueryStringKey"), ucPager.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
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
            return ValidationHelper.GetBoolean(GetValue("DisplayFirstLastAutomatically"), ucPager.DisplayFirstLastAutomatically);
        }
        set
        {
            SetValue("DisplayFirstLastAutomatically", value);
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
            return ValidationHelper.GetBoolean(GetValue("DisplayPreviousNextAutomatically"), ucPager.DisplayPreviousNextAutomatically);
        }
        set
        {
            SetValue("DisplayPreviousNextAutomatically", value);
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
            return ValidationHelper.GetString(GetValue("Pages"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("CurrentPage"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("PageSeparator"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("FirstPage"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("LastPage"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("PreviousPage"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("NextPage"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("PreviousGroup"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("NextGroup"), string.Empty);
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
            return ValidationHelper.GetString(GetValue("PagerLayout"), string.Empty);
        }
        set
        {
            SetValue("PagerLayout", value);
        }
    }

    #endregion


    #region "Data source properties"

    /// <summary>
    /// Gets or sets WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), ucDataSource.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
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
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), ucDataSource.TopN);
        }
        set
        {
            SetValue("SelectTopN", value);
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
            return ValidationHelper.GetString(GetValue("SiteName"), ucDataSource.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
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
            return ValidationHelper.GetString(GetValue("OrderBy"), ucDataSource.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
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
            return ValidationHelper.GetString(GetValue("FilterName"), ucDataSource.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            ucDataSource.FilterName = value;
        }
    }


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
            ucDataSource.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, ucDataSource.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            ucDataSource.CacheDependencies = value;
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
            ucDataSource.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Group GUID (document field GUID) of the grouped attachments.
    /// </summary>
    public Guid AttachmentGroupGUID
    {
        get
        {
            string guidAndText = ValidationHelper.GetString(GetValue("AttachmentGroupGUID"), string.Empty);
            string[] values = guidAndText.Split('|');
            return (values.Length >= 1) ? ValidationHelper.GetGuid(values[0], Guid.Empty) : Guid.Empty;
        }
        set
        {
            SetValue("AttachmentGroupGUID", value);
            ucDataSource.AttachmentGroupGUID = value;
        }
    }


    /// <summary>
    /// Culture code, such as en-us.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CultureCode"), ucDataSource.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
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
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), ucDataSource.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
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
            return DataHelper.GetNotEmpty(GetValue("Path"), ucDataSource.Path);
        }
        set
        {
            SetValue("Path", value);
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
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), ucDataSource.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            ucDataSource.CheckPermissions = value;
        }
    }

    #endregion


    #region "Effect & Layout properties"

    /// <summary>
    /// Content before each item.
    /// </summary>
    public string ItemHTMLBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemHTMLBefore"), "");
        }
        set
        {
            SetValue("ItemHTMLBefore", value);
            ucRepeater.ItemHTMLBefore = value;
        }
    }


    /// <summary>
    /// Content after each item.
    /// </summary>
    public string ItemHTMLAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemHTMLAfter"), "");
        }
        set
        {
            SetValue("ItemHTMLAfter", value);
            ucRepeater.ItemHTMLAfter = value;
        }
    }


    /// <summary>
    /// Content after each item.
    /// </summary>
    public string ItemHTMLSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemHTMLSeparator"), "");
        }
        set
        {
            SetValue("ItemHTMLSeparator", value);
            ucRepeater.ItemHTMLSeparator = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to hide layout (Content before, Content after) when no data found.
    /// </summary>
    public bool HideLayoutForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideLayoutForZeroRows"), false);
        }
        set
        {
            SetValue("HideLayoutForZeroRows", value);
            ucRepeater.HideLayoutForZeroRows = value;
        }
    }


    /// <summary>
    /// Script files.
    /// </summary>
    public string ScriptFiles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ScriptFiles"), "");
        }
        set
        {
            SetValue("ScriptFiles", value);
            ucRepeater.ScriptFiles = value;
        }
    }


    /// <summary>
    /// Initialization script.
    /// </summary>
    public string InitScript
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InitScript"), "");
        }
        set
        {
            SetValue("InitScript", value);
            ucRepeater.InitScript = value;
        }
    }


    /// <summary>
    /// Additional CSS files.
    /// </summary>
    public string CSSFiles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CSSFiles"), "");
        }
        set
        {
            SetValue("CSSFiles", value);
            ucRepeater.CSSFiles = value;
        }
    }


    /// <summary>
    /// Inline CSS styles.
    /// </summary>
    public string InlineCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InlineCSS"), "");
        }
        set
        {
            SetValue("InlineCSS", value);
            ucRepeater.InlineCSS = value;
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


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();

        BindData();
    }


    /// <summary>
    /// Causes reloading the data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
        ucDataSource.DataSource = null;
        BindData();
    }


    /// <summary>
    /// Binds the data to the repeater.
    /// </summary>
    public void BindData()
    {
        // Connects repeater with data source
        ucRepeater.DataSource = ucDataSource.DataSource;
        ucRepeater.RelatedData = ucDataSource.RelatedData;

        if (!DataHelper.DataSourceIsEmpty(ucDataSource))
        {
            ucRepeater.DataBind();
        }
    }


    /// <summary>
    /// Initializes control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            ucDataSource.StopProcessing = true;
        }
        else
        {
            ucRepeater.DataBindByDefault = false;
            ucPager.PageControl = ucRepeater.ID;

            ucDataSource.GetBinary = false;

            // Basic control properties
            ucRepeater.HideControlForZeroRows = HideControlForZeroRows;
            ucRepeater.ZeroRowsText = ZeroRowsText;

            // Data source properties
            ucDataSource.WhereCondition = WhereCondition;
            ucDataSource.OrderBy = OrderBy;
            ucDataSource.FilterName = FilterName;
            ucDataSource.CacheItemName = CacheItemName;
            ucDataSource.CacheDependencies = CacheDependencies;
            ucDataSource.CacheMinutes = CacheMinutes;
            ucDataSource.AttachmentGroupGUID = AttachmentGroupGUID;
            ucDataSource.CheckPermissions = CheckPermissions;
            ucDataSource.CombineWithDefaultCulture = CombineWithDefaultCulture;

            if (string.IsNullOrEmpty(CultureCode))
            {
                ucDataSource.CultureCode = DocumentContext.CurrentDocumentCulture.CultureCode;
            }
            else
            {
                ucDataSource.CultureCode = CultureCode;
            }

            ucDataSource.Path = TreePathUtils.EnsureSingleNodePath(MacroResolver.ResolveCurrentPath(Path));
            ucDataSource.SiteName = SiteName;
            ucDataSource.TopN = TopN;

            // UniPager properties
            ucPager.PageSize = PageSize;
            ucPager.GroupSize = GroupSize;
            ucPager.QueryStringKey = QueryStringKey;
            ucPager.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
            ucPager.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
            ucPager.HidePagerForSinglePage = HidePagerForSinglePage;

            switch (PagingMode.ToLowerCSafe())
            {
                case "postback":
                    ucPager.PagerMode = UniPagerMode.PostBack;
                    break;

                default:
                    ucPager.PagerMode = UniPagerMode.Querystring;
                    break;
            }

            // Effect properties
            ucRepeater.ItemHTMLBefore = ItemHTMLBefore;
            ucRepeater.ItemHTMLAfter = ItemHTMLAfter;
            ucRepeater.ItemHTMLSeparator = ItemHTMLSeparator;
            ucRepeater.HideLayoutForZeroRows = HideLayoutForZeroRows;
            ucRepeater.ScriptFiles = ScriptFiles;
            ucRepeater.InitScript = InitScript;
            ucRepeater.CSSFiles = CSSFiles;
            ucRepeater.InlineCSS = InlineCSS;

            // Setup repeater and pager templates
            SetupTemplates();
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        ucDataSource.ClearCache();
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = !ucDataSource.StopProcessing;

        if (!ucDataSource.HasData())
        {
            if (HideControlForZeroRows)
            {
                Visible = false;
            }
            else
            {
                if (HideLayoutForZeroRows)
                {
                    ContentBefore = string.Empty;
                    ContentAfter = string.Empty;
                }
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Setups the templates.
    /// </summary>
    private void SetupTemplates()
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
    }

    #endregion
}