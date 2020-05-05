using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.WebAnalytics.Web.UI;


public partial class CMSModules_SmartSearch_Controls_SearchResults : CMSUserControl, ISearchFilterable, IUniPageable
{
    #region "Variables"

    // Filter support
    private string mFilterSearchCondition = null;
    private string mFilterSearchSort = null;
    private string mFilterID = null;

    // Search
    private string mSearchSort = null;
    private string mSearchCondition = null;
    private string mIndexes = null;
    private string mPath = null;
    private bool mCheckPermissions = false;
    private SearchOptionsEnum mSearchOptions = SearchOptionsEnum.BasicSearch;
    private string mTransformationName = "CMS.Root.SmartSearchResults";
    private string mDocumentTypes = null;
    private bool? mCombineWithDefaultCulture = null;
    private string mCultureCode = LocalizationContext.PreferredCultureCode;
    private bool mSearchInAttachments = false;
    private string mAttachmentsWhere = "";
    private string mAttachmentsOrderBy = "";
    private string mNoResultsText = ResHelper.GetString("srch.results.noresults");
    private string mSearchTextValidationFailedText = String.Empty;
    private string mSearchTextValidationFailedCssClass = String.Empty;
    private bool mIgnoreTransformations = false;
    private bool mBlockFieldOnlySearch = true;
    private bool mSearchTextRequired = true;
    private bool mResetPager = false;

    // Pager template
    private string mPagesTemplateName = null;
    private string mCurrentPageTemplateName = null;
    private string mSeparatorTemplateName = null;
    private string mFirstPageTemplateName = null;
    private string mLastPageTemplateName = null;
    private string mPreviousPageTemplateName = null;
    private string mNextPageTemplateName = null;
    private string mPreviousGroupTemplateName = null;
    private string mNextGroupTemplateName = null;
    private string mLayoutTemplateName = null;

    // Direct access templates
    private ITemplate mPagesTemplate = null;
    private ITemplate mCurrentPageTemplate = null;
    private ITemplate mFirstPageTemplate = null;
    private ITemplate mLastPageTemplate = null;
    private ITemplate mPreviousPageTemplate = null;
    private ITemplate mNextPageTemplate = null;
    private ITemplate mPreviousGroupTemplate = null;
    private ITemplate mNextGroupTemplate = null;
    private ITemplate mLayoutTemplate = null;
    private ITemplate mPageNumbersSeparatorTemplate = null;
    // Template
    private ITemplate mItemTemplate = null;
    private ITemplate mAlternatingItemTemplate = null;
    private ITemplate mFooterTemplate = null;
    private ITemplate mHeaderTemplate = null;
    private ITemplate mSeparatorTemplate = null;

    // Basic repeater instance
    private BasicRepeater repSearchResults = new BasicRepeater();
    private int mMaxResults = 0;

    // Pager instance
    private UniPager pgr = null;

    #endregion


    #region "Delegates & events"

    /// <summary>
    /// Search completed deleagate.
    /// </summary>
    /// <param name="visible">Determines whether this control is visible</param>
    public delegate void SearchCompletedHandler(bool visible);

    /// <summary>
    /// Raises when search is completed.
    /// </summary>
    public event SearchCompletedHandler OnSearchCompleted;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            if (IsLiveSite)
            {
                return plcMess;
            }
            else
            {
                return base.MessagesPlaceHolder;
            }
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Control CSS class
    /// </summary>
    public string CssClass
    {
        get;
        set;
    }

    #endregion


    #region "Search properties"

    /// <summary>
    /// Gets or sets filter search condition.
    /// </summary>
    public string FilterSearchCondition
    {
        get
        {
            return mFilterSearchCondition;
        }
        set
        {
            mFilterSearchCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets filter sorting of search.
    /// </summary>
    public string FilterSearchSort
    {
        get
        {
            return mFilterSearchSort;
        }
        set
        {
            mFilterSearchSort = value;
        }
    }


    /// <summary>
    /// Gets or sets sorting of search.
    /// </summary>
    public string SearchSort
    {
        get
        {
            return mSearchSort;
        }
        set
        {
            mSearchSort = value;
        }
    }


    /// <summary>
    /// Gets or sets search condition.
    /// </summary>
    public string SearchCondition
    {
        get
        {
            return mSearchCondition;
        }
        set
        {
            mSearchCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets indexes.
    /// </summary>
    public string Indexes
    {
        get
        {
            return mIndexes;
        }
        set
        {
            mIndexes = value;
        }
    }


    /// <summary>
    /// Gets or sets path.
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
        }
    }


    /// <summary>
    /// Gets or sets document types.
    /// </summary>
    public string DocumentTypes
    {
        get
        {
            return mDocumentTypes;
        }
        set
        {
            mDocumentTypes = value;
        }
    }


    /// <summary>
    /// Gets or sets check permissions.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return mCheckPermissions;
        }
        set
        {
            mCheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets search option.
    /// </summary>
    public SearchOptionsEnum SearchOptions
    {
        get
        {
            return mSearchOptions;
        }
        set
        {
            mSearchOptions = value;
        }
    }


    /// <summary>
    /// Gets or sets transformation name.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return mTransformationName;
        }
        set
        {
            mTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return mCultureCode;
        }
        set
        {
            mCultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets combine with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            // Get from setting if not set
            if (!mCombineWithDefaultCulture.HasValue)
            {
                return SiteInfoProvider.CombineWithDefaultCulture(SiteContext.CurrentSiteName);
            }
            else
            {
                return mCombineWithDefaultCulture.Value;
            }
        }
        set
        {
            mCombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets search int attachments.
    /// </summary>
    public bool SearchInAttachments
    {
        get
        {
            return mSearchInAttachments;
        }
        set
        {
            mSearchInAttachments = value;
        }
    }


    /// <summary>
    /// Gets or sets culture code.
    /// </summary>
    public string AttachmentsWhere
    {
        get
        {
            return mAttachmentsWhere;
        }
        set
        {
            mAttachmentsWhere = value;
        }
    }


    /// <summary>
    /// Gets or sets culture code.
    /// </summary>
    public string AttachmentsOrderBy
    {
        get
        {
            return mAttachmentsOrderBy;
        }
        set
        {
            mAttachmentsOrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets culture code.
    /// </summary>
    public string NoResultsText
    {
        get
        {
            return mNoResultsText;
        }
        set
        {
            mNoResultsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the search mode label text.
    /// </summary>
    public string FilterID
    {
        get
        {
            return mFilterID;
        }
        set
        {
            mFilterID = value;
        }
    }


    /// <summary>
    /// Indicates if transformations should be ignored and templates for direct access should be used.
    /// </summary>
    public bool IgnoreTransformations
    {
        get
        {
            return mIgnoreTransformations;
        }
        set
        {
            mIgnoreTransformations = value;
        }
    }


    /// <summary>
    /// Indicates if parsing errors should be showed if occurs.
    /// </summary>
    public bool ShowParsingErrors
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if search text is required.
    /// </summary>
    public bool SearchTextRequired
    {
        get
        {
            return mSearchTextRequired;
        }
        set
        {
            mSearchTextRequired = value;
        }
    }


    /// <summary>
    /// Sets the text that is displayed when search text is empty and required.
    /// </summary>
    public string SearchTextValidationFailedText
    {
        get
        {
            return mSearchTextValidationFailedText;
        }
        set
        {
            mSearchTextValidationFailedText = value;
        }
    }


    /// <summary>
    /// CSS class that will be assigned to the web part when search text validation fails.
    /// </summary>
    public string SearchTextValidationFailedCssClass
    {
        get
        {
            return mSearchTextValidationFailedCssClass;
        }
        set
        {
            mSearchTextValidationFailedCssClass = value;
        }
    }


    /// <summary>
    /// Indicates if search should be work without search text.
    /// </summary>
    public bool BlockFieldOnlySearch
    {
        get
        {
            return mBlockFieldOnlySearch;
        }
        set
        {
            mBlockFieldOnlySearch = value;
        }
    }


    /// <summary>
    /// If true, fuzzy search (typo tolerant search) is performed. The users query is preprocessed to achieve the task.
    /// </summary>
    public bool DoFuzzySearch
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if filter is applied immediately.
    /// </summary>
    public bool SearchOnEachPageLoad
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SearchOnEachPageLoad"), false);
        }
        set
        {
            SetValue("SearchOnEachPageLoad", value);
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Gets or sets the value that indicates whether scroll position should be cleared after post back paging
    /// </summary>
    public bool ResetScrollPositionOnPostBack
    {
        get
        {
            return pgrSearch.ResetScrollPositionOnPostBack;
        }
        set
        {
            pgrSearch.ResetScrollPositionOnPostBack = value;
        }
    }

    /// <summary>
    /// Gets or sets page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return pgrSearch.PageSize;
        }
        set
        {
            pgrSearch.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets search option.
    /// </summary>
    public UniPagerMode PagingMode
    {
        get
        {
            return pgrSearch.PagerMode;
        }
        set
        {
            pgrSearch.PagerMode = value;
        }
    }


    /// <summary>
    /// Gets or sets query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return pgrSearch.QueryStringKey;
        }
        set
        {
            pgrSearch.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets group size.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return pgrSearch.GroupSize;
        }
        set
        {
            pgrSearch.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return pgrSearch.DisplayFirstLastAutomatically;
        }
        set
        {
            pgrSearch.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return pgrSearch.DisplayPreviousNextAutomatically;
        }
        set
        {
            pgrSearch.DisplayPreviousNextAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public bool HidePagerForSinglePage
    {
        get
        {
            return pgrSearch.HidePagerForSinglePage;
        }
        set
        {
            pgrSearch.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager max pages.
    /// </summary>
    public int MaxPages
    {
        get
        {
            return pgrSearch.MaxPages;
        }
        set
        {
            pgrSearch.MaxPages = value;
        }
    }


    /// <summary>
    /// Gets or sets the max. displayed results.
    /// </summary>
    public int MaxResults
    {
        get
        {
            return mMaxResults;
        }
        set
        {
            mMaxResults = value;
        }
    }

    #endregion


    #region "UniPager Template properties"

    /// <summary>
    /// Gets or sets the pages template name.
    /// </summary>
    public string PagesTemplateName
    {
        get
        {
            return mPagesTemplateName;
        }
        set
        {
            mPagesTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the current page template name.
    /// </summary>
    public string CurrentPageTemplateName
    {
        get
        {
            return mCurrentPageTemplateName;
        }
        set
        {
            mCurrentPageTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator template name.
    /// </summary>
    public string SeparatorTemplateName
    {
        get
        {
            return mSeparatorTemplateName;
        }
        set
        {
            mSeparatorTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the first page template name.
    /// </summary>
    public string FirstPageTemplateName
    {
        get
        {
            return mFirstPageTemplateName;
        }
        set
        {
            mFirstPageTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the last page template name.
    /// </summary>
    public string LastPageTemplateName
    {
        get
        {
            return mLastPageTemplateName;
        }
        set
        {
            mLastPageTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous page template name.
    /// </summary>
    public string PreviousPageTemplateName
    {
        get
        {
            return mPreviousPageTemplateName;
        }
        set
        {
            mPreviousPageTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the next page template name.
    /// </summary>
    public string NextPageTemplateName
    {
        get
        {
            return mNextPageTemplateName;
        }
        set
        {
            mNextPageTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous group template name.
    /// </summary>
    public string PreviousGroupTemplateName
    {
        get
        {
            return mPreviousGroupTemplateName;
        }
        set
        {
            mPreviousGroupTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the next group template name.
    /// </summary>
    public string NextGroupTemplateName
    {
        get
        {
            return mNextGroupTemplateName;
        }
        set
        {
            mNextGroupTemplateName = value;
        }
    }


    /// <summary>
    /// Gets or sets the layout template name.
    /// </summary>
    public string LayoutTemplateName
    {
        get
        {
            return mLayoutTemplateName;
        }
        set
        {
            mLayoutTemplateName = value;
        }
    }

    /// <summary>
    /// Gets or sets the pages template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate PageNumbersTemplate
    {
        get
        {
            return mPagesTemplate;
        }
        set
        {
            mPagesTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the current page template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate CurrentPageTemplate
    {
        get
        {
            return mCurrentPageTemplate;
        }
        set
        {
            mCurrentPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the first page template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate FirstPageTemplate
    {
        get
        {
            return mFirstPageTemplate;
        }
        set
        {
            mFirstPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the last page template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate LastPageTemplate
    {
        get
        {
            return mLastPageTemplate;
        }
        set
        {
            mLastPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous page template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate PreviousPageTemplate
    {
        get
        {
            return mPreviousPageTemplate;
        }
        set
        {
            mPreviousPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next page template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate NextPageTemplate
    {
        get
        {
            return mNextPageTemplate;
        }
        set
        {
            mNextPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous group template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate PreviousGroupTemplate
    {
        get
        {
            return mPreviousGroupTemplate;
        }
        set
        {
            mPreviousGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next group template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate NextGroupTemplate
    {
        get
        {
            return mNextGroupTemplate;
        }
        set
        {
            mNextGroupTemplate = value;
        }
    }

    /// <summary>
    /// Gets or sets the next group template for direct access.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate PageNumbersSeparatorTemplate
    {
        get
        {
            return mPageNumbersSeparatorTemplate;
        }
        set
        {
            mPageNumbersSeparatorTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the layout template name.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public ITemplate LayoutTemplate
    {
        get
        {
            return mLayoutTemplate;
        }
        set
        {
            mLayoutTemplate = value;
        }
    }


    /// <summary>
    /// Sets or gets item template.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public virtual ITemplate ItemTemplate
    {
        get
        {
            return mItemTemplate;
        }
        set
        {
            mItemTemplate = value;
        }
    }


    /// <summary>
    /// Sets or gets alternating item template.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public virtual ITemplate AlternatingItemTemplate
    {
        get
        {
            return mAlternatingItemTemplate;
        }
        set
        {
            mAlternatingItemTemplate = value;
        }
    }


    /// <summary>
    /// Sets or gets header template.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public virtual ITemplate HeaderTemplate
    {
        get
        {
            return mHeaderTemplate;
        }
        set
        {
            mHeaderTemplate = value;
        }
    }


    /// <summary>
    /// Sets or gets footer template.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public virtual ITemplate FooterTemplate
    {
        get
        {
            return mFooterTemplate;
        }
        set
        {
            mFooterTemplate = value;
        }
    }


    /// <summary>
    /// Sets or gets separator template.
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Browsable(false)]
    public virtual ITemplate SeparatorTemplate
    {
        get
        {
            return mSeparatorTemplate;
        }
        set
        {
            mSeparatorTemplate = value;
        }
    }

    #endregion


    #region "Advanced methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Try get external pager
            pgr = UniPagerControl;
            bool isExternal = true;

            // Check whether external pager is set
            if (pgr == null)
            {
                isExternal = false;
                pgr = pgrSearch;

                // UniPager properties
                pgrSearch.PageSize = PageSize;
                pgrSearch.GroupSize = GroupSize;
                pgrSearch.QueryStringKey = QueryStringKey;
                pgrSearch.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
                pgrSearch.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
                pgrSearch.HidePagerForSinglePage = HidePagerForSinglePage;
                pgrSearch.PagerMode = PagingMode;
                pgrSearch.MaxPages = MaxPages;
            }

            if (!isExternal)
            {
                #region "UniPager template properties"

                // UniPager direct templates
                if (PageNumbersTemplate != null)
                {
                    pgrSearch.PageNumbersTemplate = PageNumbersTemplate;
                }

                if (CurrentPageTemplate != null)
                {
                    pgrSearch.CurrentPageTemplate = CurrentPageTemplate;
                }

                if (PageNumbersSeparatorTemplate != null)
                {
                    pgrSearch.PageNumbersSeparatorTemplate = PageNumbersSeparatorTemplate;
                }

                if (FirstPageTemplate != null)
                {
                    pgrSearch.FirstPageTemplate = FirstPageTemplate;
                }

                if (LastPageTemplate != null)
                {
                    pgrSearch.LastPageTemplate = LastPageTemplate;
                }

                if (PreviousPageTemplate != null)
                {
                    pgrSearch.PreviousPageTemplate = PreviousPageTemplate;
                }

                if (NextPageTemplate != null)
                {
                    pgrSearch.NextPageTemplate = NextPageTemplate;
                }

                if (PreviousGroupTemplate != null)
                {
                    pgrSearch.PreviousGroupTemplate = PreviousGroupTemplate;
                }

                if (NextGroupTemplate != null)
                {
                    pgrSearch.NextGroupTemplate = NextGroupTemplate;
                }

                if (LayoutTemplate != null)
                {
                    pgrSearch.LayoutTemplate = LayoutTemplate;
                }

                // UniPager template properties
                if (!String.IsNullOrEmpty(PagesTemplateName))
                {
                    pgrSearch.PageNumbersTemplate = TransformationHelper.LoadTransformation(pgrSearch, PagesTemplateName);
                }

                if (!String.IsNullOrEmpty(CurrentPageTemplateName))
                {
                    pgrSearch.CurrentPageTemplate = TransformationHelper.LoadTransformation(pgrSearch, CurrentPageTemplateName);
                }

                if (!String.IsNullOrEmpty(SeparatorTemplateName))
                {
                    pgrSearch.PageNumbersSeparatorTemplate = TransformationHelper.LoadTransformation(pgrSearch, SeparatorTemplateName);
                }

                if (!String.IsNullOrEmpty(FirstPageTemplateName))
                {
                    pgrSearch.FirstPageTemplate = TransformationHelper.LoadTransformation(pgrSearch, FirstPageTemplateName);
                }

                if (!String.IsNullOrEmpty(LastPageTemplateName))
                {
                    pgrSearch.LastPageTemplate = TransformationHelper.LoadTransformation(pgrSearch, LastPageTemplateName);
                }

                if (!String.IsNullOrEmpty(PreviousPageTemplateName))
                {
                    pgrSearch.PreviousPageTemplate = TransformationHelper.LoadTransformation(pgrSearch, PreviousPageTemplateName);
                }

                if (!String.IsNullOrEmpty(NextPageTemplateName))
                {
                    pgrSearch.NextPageTemplate = TransformationHelper.LoadTransformation(pgrSearch, NextPageTemplateName);
                }

                if (!String.IsNullOrEmpty(PreviousGroupTemplateName))
                {
                    pgrSearch.PreviousGroupTemplate = TransformationHelper.LoadTransformation(pgrSearch, PreviousGroupTemplateName);
                }

                if (!String.IsNullOrEmpty(NextGroupTemplateName))
                {
                    pgrSearch.NextGroupTemplate = TransformationHelper.LoadTransformation(pgrSearch, NextGroupTemplateName);
                }

                if (!String.IsNullOrEmpty(LayoutTemplateName))
                {
                    pgrSearch.LayoutTemplate = TransformationHelper.LoadTransformation(pgrSearch, LayoutTemplateName);
                }

                #endregion
            }

            // Load transformation
            if (!string.IsNullOrEmpty(TransformationName) && !IgnoreTransformations)
            {
                repSearchResults.ItemTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
            }
            // Set transformation directly
            else
            {
                repSearchResults.ItemTemplate = ItemTemplate;
                repSearchResults.HeaderTemplate = HeaderTemplate;
                repSearchResults.FooterTemplate = FooterTemplate;
                repSearchResults.AlternatingItemTemplate = AlternatingItemTemplate;
                repSearchResults.SeparatorTemplate = SeparatorTemplate;
            }

            plcBasicRepeater.Controls.Clear();
            repSearchResults.ID = "repSearchResults";
            plcBasicRepeater.Controls.Add(repSearchResults);

            Visible = true;
        }
    }


    /// <summary>
    ///  On page prerender.
    /// </summary>
    /// <param name="e">Event argument</param>
    protected override void OnPreRender(EventArgs e)
    {
        Search();

        base.OnPreRender(e);
    }


    /// <summary>
    /// Renders control
    /// </summary>
    /// <param name="writer">HTML writer</param>
    protected override void Render(HtmlTextWriter writer)
    {
        // Hide messages placeholder when no messages are being displayed
        plcMess.Visible = ShowParsingErrors || plcMess.HasText;

        pnlSearchResults.AddCssClass(CssClass);

        base.Render(writer);
    }

    /// <summary>
    /// Perform search.
    /// </summary>
    protected void Search()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Check if the search was triggered
            bool searchAllowed = SearchOnEachPageLoad || QueryHelper.Contains("searchtext");

            // Get query strings
            string searchText = QueryHelper.GetString("searchtext", "");
            // Check whether string passes text requirements settings
            bool searchTextIsNotEmptyOrNotRequired = (!SearchTextRequired || !String.IsNullOrEmpty(searchText));

            // Proceed when search was triggered and search text is passing requirements settings.
            // Requirements setting could be overridden on this level by obsolete web.config key. The reason is backward compatibility.
            // Search text required web part setting was introduced after this web.config key. Key default value was at the time set to true.
            // This default value had the same effect as this new web part setting. When someone changed the web.config key to false and then upgraded the solution,
            // required web part setting with default value true would override previous behavior. That's the reason why this obsolete key can override this setting.
            if (searchAllowed && (searchTextIsNotEmptyOrNotRequired || !SearchHelper.SearchOnlyWhenContentPresent))
            {
                string searchMode = QueryHelper.GetString("searchMode", "");
                SearchModeEnum searchModeEnum = searchMode.ToEnum<SearchModeEnum>();

                // Get current culture
                string culture = CultureCode;
                if (string.IsNullOrEmpty(culture))
                {
                    culture = ValidationHelper.GetString(ViewState["CultureCode"], LocalizationContext.PreferredCultureCode);
                }

                var siteName = SiteContext.CurrentSiteName;

                // Get default culture
                string defaultCulture = CultureHelper.GetDefaultCultureCode(siteName);

                // Resolve path
                string path = Path;
                if (!string.IsNullOrEmpty(path))
                {
                    path = MacroResolver.ResolveCurrentPath(Path);
                }

                // Check if search action was fired really on the live site
                if (PortalContext.ViewMode.IsLiveSite() && (DocumentContext.CurrentPageInfo != null))
                {
                    if (AnalyticsHelper.AnalyticsEnabled(siteName) && !string.IsNullOrEmpty(searchText))
                    {
                        if (AnalyticsHelper.JavascriptLoggingEnabled(siteName))
                        {
                            WebAnalyticsServiceScriptsRenderer.RegisterLogSearchCall(Page, DocumentContext.CurrentPageInfo, searchText);
                        }
                        else
                        {
                            // Log on site keywords
                            AnalyticsHelper.LogOnSiteSearchKeywords(siteName, DocumentContext.CurrentAliasPath, culture, searchText, 0, 1);
                        }
                    }
                }

                // Prepare search text
                var docCondition = new DocumentSearchCondition(DocumentTypes, culture, defaultCulture, CombineWithDefaultCulture);

                var searchCond = SearchCondition;
                if (!string.IsNullOrEmpty(FilterSearchCondition) && (searchModeEnum == SearchModeEnum.AnyWordOrSynonyms))
                {
                    // Make sure the synonyms are expanded before the filter condition is applied (filter condition is Lucene syntax, cannot be expanded)
                    searchCond = SearchSyntaxHelper.ExpandWithSynonyms(searchCond, docCondition.Culture);
                }

                var condition = new SearchCondition(searchCond + FilterSearchCondition, searchModeEnum, SearchOptions, docCondition, DoFuzzySearch);

                searchText = SearchSyntaxHelper.CombineSearchCondition(searchText, condition);

                // Get positions and ranges for search method
                int startPosition = 0;
                int numberOfProceeded = 100;
                int displayResults = 100;
                if (pgr.PageSize != 0 && pgr.GroupSize != 0)
                {
                    // Reset pager if needed
                    if (mResetPager)
                    {
                        pgr.CurrentPage = 1;
                    }

                    startPosition = (pgr.CurrentPage - 1) * pgr.PageSize;
                    // Only results covered by current page group are proccessed (filtered) for performance reasons. This may cause decrease of the number of results while paging.
                    numberOfProceeded = (((pgr.CurrentPage / pgr.GroupSize) + 1) * pgr.PageSize * pgr.GroupSize) + pgr.PageSize;
                    displayResults = pgr.PageSize;
                }

                if ((MaxResults > 0) && (numberOfProceeded > MaxResults))
                {
                    numberOfProceeded = MaxResults;
                }

                // Combine regular search sort with filter sort
                string srt = ValidationHelper.GetString(SearchSort, String.Empty).Trim();
                string filterSrt = ValidationHelper.GetString(FilterSearchSort, String.Empty).Trim();

                if (!String.IsNullOrEmpty(filterSrt))
                {
                    if (!String.IsNullOrEmpty(srt))
                    {
                        srt += ", ";
                    }

                    srt += filterSrt;
                }

                // Prepare parameters
                SearchParameters parameters = new SearchParameters
                {
                    SearchFor = searchText,
                    SearchSort = srt,
                    Path = path,
                    ClassNames = DocumentTypes,
                    CurrentCulture = culture,
                    DefaultCulture = defaultCulture,
                    CombineWithDefaultCulture = CombineWithDefaultCulture,
                    CheckPermissions = CheckPermissions,
                    SearchInAttachments = SearchInAttachments,
                    User = MembershipContext.AuthenticatedUser,
                    SearchIndexes = Indexes,
                    StartingPosition = startPosition,
                    DisplayResults = displayResults,
                    NumberOfProcessedResults = numberOfProceeded,
                    NumberOfResults = 0,
                    AttachmentWhere = AttachmentsWhere,
                    AttachmentOrderBy = AttachmentsOrderBy,
                    BlockFieldOnlySearch = BlockFieldOnlySearch,
                };

                // Search
                var results = SearchHelper.Search(parameters);

                int numberOfResults = parameters.NumberOfResults;
                if ((MaxResults > 0) && (numberOfResults > MaxResults))
                {
                    numberOfResults = MaxResults;
                }

                // Limit displayed results according to MaxPages property
                var maxDisplayedResultsOnMaxPages = MaxPages * PageSize;
                // Apply only if MaxPages and PageSize properties are set
                if ((maxDisplayedResultsOnMaxPages > 0) && (numberOfResults > maxDisplayedResultsOnMaxPages))
                {
                    numberOfResults = maxDisplayedResultsOnMaxPages;
                }

                // Fill repeater with results
                repSearchResults.DataSource = results.Items;
                repSearchResults.PagerForceNumberOfResults = numberOfResults;
                PagerForceNumberOfResults = numberOfResults;
                repSearchResults.DataBind();

                // Call page binding event
                if (OnPageBinding != null)
                {
                    OnPageBinding(this, null);
                }

                // Show no results found ?
                if (numberOfResults == 0)
                {
                    if (ShowParsingErrors)
                    {
                        Exception searchError = results.LastError;
                        if (searchError != null)
                        {
                            ShowError(GetString("smartsearch.searcherror") + " " + HTMLHelper.HTMLEncode(searchError.Message));
                        }
                    }
                    lblNoResults.Text = NoResultsText;
                    lblNoResults.Visible = true;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(SearchTextValidationFailedText) && searchAllowed)
                {
                    pnlSearchResults.AddCssClass(SearchTextValidationFailedCssClass);
                    lblNoResults.Text = SearchTextValidationFailedText;
                    lblNoResults.Visible = true;
                }
                else
                {
                    Visible = false;
                }
            }

            // Invoke search completed event
            if (OnSearchCompleted != null)
            {
                OnSearchCompleted(Visible);
            }
        }
    }


    /// <summary>
    /// Applies search filter.
    /// </summary>
    /// <param name="searchCondition">Search condition</param>
    /// <param name="searchSort">Search sort</param>
    /// <param name="filterPostback">If true filter caused the postback which means that filter condition has been changed.</param>
    public void ApplyFilter(string searchCondition, string searchSort, bool filterPostback)
    {
        FilterSearchCondition += " " + searchCondition;
        FilterSearchSort += " " + searchSort;
        mResetPager |= filterPostback;
    }


    /// <summary>
    /// Adds filter option to url.
    /// </summary>
    /// <param name="searchWebpartID">Webpart id</param>
    /// <param name="options">Options</param>
    public void AddFilterOptionsToUrl(string searchWebpartID, string options)
    {
        // Do nothing
    }


    /// <summary>
    /// Loads data.
    /// </summary>
    public void LoadData()
    {
        // Register control for filter
        CMSControlsHelper.SetFilter(FilterID, this);
    }

    #endregion


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item object.
    /// </summary>
    public object PagerDataItem
    {
        get;
        set;
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UniPager UniPagerControl
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the number of result. Enables proceed "fake" datasets, where number
    /// of results in the dataset is not correspondent to the real number of results
    /// This property must be equal -1 if should be disabled
    /// </summary>
    public int PagerForceNumberOfResults
    {
        get;
        set;
    }


    /// <summary>
    /// Occurs when the control bind data.
    /// </summary>
    public event EventHandler<EventArgs> OnPageBinding;

    // Do not display warning for not-used event handler required by interface
#pragma warning disable 67

    /// <summary>
    /// Occurs when the pager change the page and current mode is postback => reload data
    /// </summary>
    public event EventHandler<EventArgs> OnPageChanged;

#pragma warning restore 67

    /// <summary>
    /// Evokes control databind.
    /// </summary>
    public void ReBind()
    {
        // Do nothing
    }

    #endregion
}