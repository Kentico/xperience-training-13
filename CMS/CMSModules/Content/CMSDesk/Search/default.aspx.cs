using System;
using System.Data;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;


public partial class CMSModules_Content_CMSDesk_Search_default : CMSContentPage, ITimeZoneManager
{
    #region "Variables"

    private string searchindex = null;
    private bool timeZoneLoaded = false;
    private TimeZoneInfo usedTimeZone = null;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        DocumentManager.RegisterSaveChangesScript = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        pagerElem.PagerMode = UniPagerMode.Querystring;
        pagerElem.HidePagerForSinglePage = true;
        pagerElem.QueryStringKey = "pagesearchresults";

        // Setup page title text and image
        PageTitle.TitleText = GetString("Content.SearchTitle");
        if (!RequestHelper.IsPostBack())
        {
            searchindex = QueryHelper.GetString("searchindex", null);
            SetRepeaters();
        }
        else
        {
            repSearchSQL.Visible = false;
            repSearchSQL.StopProcessing = true;
            repSmartSearch.Visible = false;
            repSmartSearch.StopProcessing = true;
        }

        CreateBreadcrumbs();
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    private void CreateBreadcrumbs()
    {
        EnsureDocumentBreadcrumbs(CurrentMaster.Title.Breadcrumbs, null, GetString("contentmenu.search"));
    }


    /// <summary>
    /// Sets repeaters.
    /// </summary>
    private void SetRepeaters()
    {
        // Display SQL results
        if (searchindex.EqualsCSafe("##SQL##", false))
        {
            repSearchSQL.Visible = true;
            repSearchSQL.StopProcessing = false;
            repSmartSearch.Visible = false;
            repSmartSearch.StopProcessing = true;

            // Hide original pager and set the UI Pager
            pagerElem.ShowPageSize = false;
            repSearchSQL.PagerControl.Visible = false;
            repSearchSQL.PagerControl.OnDataSourceChanged += PagerControl_OnDataSourceChanged;

            repSearchSQL.SelectOnlyPublished = QueryHelper.GetBoolean("searchpublished", true);

            string culture = QueryHelper.GetString("searchculture", "##ANY##");
            string mode = QueryHelper.GetString("searchlanguage", null);
            if ((culture == "##ANY##") || (mode == "<>"))
            {
                culture = TreeProvider.ALL_CULTURES;
            }
            else
            {
                repSearchSQL.CombineWithDefaultCulture = false;
            }
            repSearchSQL.WhereCondition = searchDialog.WhereCondition;
            repSearchSQL.CultureCode = culture;
            repSearchSQL.TransformationName = "~/CMSModules/Content/CMSDesk/Search/CMSDeskSQLSearchResults.ascx";
        }
        // Display Smart search results
        else
        {
            repSearchSQL.Visible = false;
            repSearchSQL.StopProcessing = true;
            repSmartSearch.Visible = true;
            repSmartSearch.StopProcessing = false;
            repSmartSearch.Indexes = searchindex;
            repSmartSearch.IgnoreTransformations = true;

            repSmartSearch.PageSize = 10;
            repSmartSearch.PagingMode = UniPagerMode.Querystring;
            repSmartSearch.HidePagerForSinglePage = true;
            repSmartSearch.UniPagerControl = pagerElem.UniPager;

            pagerElem.PagedControl = repSmartSearch;
            pagerElem.ShowPageSize = true;

            string culture = QueryHelper.GetString("searchculture", "##ANY##");
            if (culture == "##ANY##")
            {
                culture = "##all##";
            }
            repSmartSearch.CultureCode = culture;
        }
    }


    void PagerControl_OnDataSourceChanged(object sender, EventArgs e)
    {
        UniPagerConnector connectorElem = new UniPagerConnector();
        connectorElem.PagerForceNumberOfResults = repSearchSQL.PagerControl.TotalRecords;
        pagerElem.PagedControl = connectorElem;
    }

    #endregion


    #region "ITimeZoneManager Members"

    /// <summary>
    /// Gets time zone type for this page.
    /// </summary>
    public TimeZoneTypeEnum TimeZoneType
    {
        get
        {
            return TimeZoneTypeEnum.Custom;
        }
    }


    /// <summary>
    /// Gets current time zone for UI.
    /// </summary>
    public TimeZoneInfo CustomTimeZone
    {
        get
        {
            // Get time zone for first request only
            if (!timeZoneLoaded)
            {
                usedTimeZone = TimeZoneHelper.GetTimeZoneInfo(MembershipContext.AuthenticatedUser, SiteContext.CurrentSite);
                timeZoneLoaded = true;
            }
            return usedTimeZone;
        }
    }

    #endregion
}