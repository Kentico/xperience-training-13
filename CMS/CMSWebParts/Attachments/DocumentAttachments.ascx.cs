using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.PortalEngine;

public partial class CMSWebParts_Attachments_DocumentAttachments : CMSAbstractWebPart
{
    #region "Basic repeater properties"

    /// <summary>
    /// Gets or sets AlternatingItemTemplate property.
    /// </summary>
    public string AlternatingItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternatingItemTransformationName"), ucAttachments.AlternatingItemTransformationName);
        }
        set
        {
            SetValue("AlternatingItemTransformationName", value);
            ucAttachments.AlternatingItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets FooterTemplate property.
    /// </summary>
    public string FooterTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FooterTransformationName"), ucAttachments.FooterTransformationName);
        }
        set
        {
            SetValue("FooterTransformationName", value);
            ucAttachments.FooterTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets HeaderTemplate property.
    /// </summary>
    public string HeaderTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderTransformationName"), ucAttachments.HeaderTransformationName);
        }
        set
        {
            SetValue("HeaderTransformationName", value);
            ucAttachments.HeaderTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), ucAttachments.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            ucAttachments.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets SeparatorTemplate property.
    /// </summary>
    public string SeparatorTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SeparatorTransformationName"), ucAttachments.SeparatorTransformationName);
        }
        set
        {
            SetValue("SeparatorTransformationName", value);
            ucAttachments.SeparatorTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), ucAttachments.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            ucAttachments.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), ucAttachments.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            ucAttachments.ZeroRowsText = value;
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
            return ValidationHelper.GetBoolean(GetValue("HidePagerForSinglePage"), ucAttachments.HidePagerForSinglePage);
        }
        set
        {
            SetValue("HidePagerForSinglePage", value);
            ucAttachments.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of records to display on a page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), ucAttachments.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            ucAttachments.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupSize"), ucAttachments.GroupSize);
        }
        set
        {
            SetValue("GroupSize", value);
            ucAttachments.GroupSize = value;
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
                        ucAttachments.PagingMode = UniPagerMode.PostBack;
                        break;
                    default:
                        ucAttachments.PagingMode = UniPagerMode.Querystring;
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
            return ValidationHelper.GetString(GetValue("QueryStringKey"), ucAttachments.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            ucAttachments.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFirstLastAutomatically"), ucAttachments.DisplayFirstLastAutomatically);
        }
        set
        {
            SetValue("DisplayFirstLastAutomatically", value);
            ucAttachments.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPreviousNextAutomatically"), ucAttachments.DisplayPreviousNextAutomatically);
        }
        set
        {
            SetValue("DisplayPreviousNextAutomatically", value);
            ucAttachments.DisplayPreviousNextAutomatically = value;
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
            return ValidationHelper.GetString(GetValue("Pages"), ucAttachments.PagesTemplate);
        }
        set
        {
            SetValue("Pages", value);
            ucAttachments.PagesTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CurrentPage"), ucAttachments.CurrentPageTemplate);
        }
        set
        {
            SetValue("CurrentPage", value);
            ucAttachments.CurrentPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageSeparator"), ucAttachments.SeparatorTemplate);
        }
        set
        {
            SetValue("PageSeparator", value);
            ucAttachments.SeparatorTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstPage"), ucAttachments.FirstPageTemplate);
        }
        set
        {
            SetValue("FirstPage", value);
            ucAttachments.FirstPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LastPage"), ucAttachments.LastPageTemplate);
        }
        set
        {
            SetValue("LastPage", value);
            ucAttachments.LastPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousPage"), ucAttachments.PreviousPageTemplate);
        }
        set
        {
            SetValue("PreviousPage", value);
            ucAttachments.PreviousPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextPage"), ucAttachments.NextPageTemplate);
        }
        set
        {
            SetValue("NextPage", value);
            ucAttachments.NextPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousGroup"), ucAttachments.PreviousGroupTemplate);
        }
        set
        {
            SetValue("PreviousGroup", value);
            ucAttachments.PreviousGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextGroup"), ucAttachments.NextGroupTemplate);
        }
        set
        {
            SetValue("NextGroup", value);
            ucAttachments.NextGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerLayout"), ucAttachments.LayoutTemplate);
        }
        set
        {
            SetValue("PagerLayout", value);
            ucAttachments.LayoutTemplate = value;
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
            return ValidationHelper.GetString(GetValue("WhereCondition"), ucAttachments.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            ucAttachments.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets top N.
    /// </summary>
    public int TopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), ucAttachments.TopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            ucAttachments.TopN = value;
        }
    }


    /// <summary>
    /// Gets or sets site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), ucAttachments.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            ucAttachments.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets ORDER BY condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), ucAttachments.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            ucAttachments.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), ucAttachments.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            ucAttachments.FilterName = value;
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
            ucAttachments.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, ucAttachments.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            ucAttachments.CacheDependencies = value;
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
            ucAttachments.CacheMinutes = value;
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
            ucAttachments.AttachmentGroupGUID = value;
        }
    }


    /// <summary>
    /// Culture code, such as en-us.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CultureCode"), ucAttachments.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            ucAttachments.CultureCode = value;
        }
    }


    /// <summary>
    /// Indicates if the document should be selected eventually from the default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), ucAttachments.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            ucAttachments.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the alias path.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), ucAttachments.Path);
        }
        set
        {
            SetValue("Path", value);
            ucAttachments.Path = value;
        }
    }


    /// <summary>
    /// Allows you to specify whether to check permissions of the current user. If the value is 'false' (default value) no permissions are checked. Otherwise, only nodes for which the user has read permission are displayed.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), ucAttachments.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            ucAttachments.CheckPermissions = value;
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
            ucAttachments.StopProcessing = value;
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


    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
            ucAttachments.StopProcessing = true;
        }
        else
        {
            ucAttachments.GetBinary = false;

            // Basic control properties
            ucAttachments.HideControlForZeroRows = HideControlForZeroRows;
            ucAttachments.ZeroRowsText = ZeroRowsText;

            // Data source properties
            ucAttachments.WhereCondition = WhereCondition;
            ucAttachments.OrderBy = OrderBy;
            ucAttachments.FilterName = FilterName;
            ucAttachments.CacheItemName = CacheItemName;
            ucAttachments.CacheDependencies = CacheDependencies;
            ucAttachments.CacheMinutes = CacheMinutes;
            ucAttachments.AttachmentGroupGUID = AttachmentGroupGUID;
            ucAttachments.CheckPermissions = CheckPermissions;
            ucAttachments.CombineWithDefaultCulture = CombineWithDefaultCulture;
            if (string.IsNullOrEmpty(CultureCode))
            {
                ucAttachments.CultureCode = DocumentContext.CurrentDocumentCulture.CultureCode;
            }
            else
            {
                ucAttachments.CultureCode = CultureCode;
            }
          
            ucAttachments.Path = TreePathUtils.EnsureSingleNodePath(MacroResolver.ResolveCurrentPath(Path));
            ucAttachments.SiteName = SiteName;
            ucAttachments.TopN = TopN;


            #region "Repeater template properties"

            // Apply transformations if they exist
            ucAttachments.TransformationName = TransformationName;
            ucAttachments.AlternatingItemTransformationName = AlternatingItemTransformationName;
            ucAttachments.FooterTransformationName = FooterTransformationName;
            ucAttachments.HeaderTransformationName = HeaderTransformationName;
            ucAttachments.SeparatorTransformationName = SeparatorTransformationName;

            #endregion


            // UniPager properties
            ucAttachments.PageSize = PageSize;
            ucAttachments.GroupSize = GroupSize;
            ucAttachments.QueryStringKey = QueryStringKey;
            ucAttachments.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
            ucAttachments.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
            ucAttachments.HidePagerForSinglePage = HidePagerForSinglePage;
            switch (PagingMode.ToLowerCSafe())
            {
                case "postback":
                    ucAttachments.PagingMode = UniPagerMode.PostBack;
                    break;
                default:
                    ucAttachments.PagingMode = UniPagerMode.Querystring;
                    break;
            }


            #region "UniPager template properties"

            // UniPager template properties
            ucAttachments.PagesTemplate = PagesTemplate;
            ucAttachments.CurrentPageTemplate = CurrentPageTemplate;
            ucAttachments.SeparatorTemplate = SeparatorTemplate;
            ucAttachments.FirstPageTemplate = FirstPageTemplate;
            ucAttachments.LastPageTemplate = LastPageTemplate;
            ucAttachments.PreviousPageTemplate = PreviousPageTemplate;
            ucAttachments.NextPageTemplate = NextPageTemplate;
            ucAttachments.PreviousGroupTemplate = PreviousGroupTemplate;
            ucAttachments.NextGroupTemplate = NextGroupTemplate;
            ucAttachments.LayoutTemplate = LayoutTemplate;

            #endregion
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        ucAttachments.ClearCache();
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = !ucAttachments.StopProcessing;

        if (HideControlForZeroRows && !ucAttachments.HasData)
        {
            Visible = false;
        }
    }

    #endregion
}