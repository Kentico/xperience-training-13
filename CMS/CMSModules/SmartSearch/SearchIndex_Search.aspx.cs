using System;
using System.Data;
using System.Web;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_SearchIndex_Search : GlobalAdminPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        pgrSearch.PagedControl = repSearchResults;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Get current index id
        int indexId = QueryHelper.GetInteger("indexId", 0);

        // Get current index info object
        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(indexId);

        if (sii?.IndexProvider.Equals(SearchIndexInfo.AZURE_SEARCH_PROVIDER, StringComparison.OrdinalIgnoreCase) ?? false)
        {
            ShowInformation(GetString("smartsearch.searchpreview.azure.unavailable"));
            searchPnl.Visible = false;

            return;
        }

        // Show information about limited features of smart search preview
        ShowInformation(GetString("smartsearch.searchpreview.limitedfeatures"));

        // Show warning if indes isn't ready yet
        if ((sii != null) && (SearchIndexInfoProvider.GetIndexStatus(sii) == IndexStatusEnum.NEW))
        {
            ShowWarning(GetString("srch.index.needrebuild"));
        }

        if (!RequestHelper.IsPostBack())
        {
            // Get current search text from query string
            string searchText = QueryHelper.GetString("searchtext", "");
            // Check whether search text is defined
            if (!string.IsNullOrEmpty(searchText))
            {
                // Get current search mode from query string
                string searchMode = QueryHelper.GetString("searchmode", "");
                SearchModeEnum searchModeEnum = searchMode.ToEnum<SearchModeEnum>();

                // Check whether index info exists
                if (sii != null)
                {
                    // Keep search text in search textbox
                    //txtSearchFor.Text = searchText;
                    var condition = new SearchCondition(null, searchModeEnum, SearchOptionsEnum.FullSearch);

                    searchText = SearchSyntaxHelper.CombineSearchCondition(searchText, condition);

                    // Get positions and ranges for search method
                    int startPosition = 0;
                    int numberOfProceeded = 100;
                    int displayResults = 100;
                    if (pgrSearch.CurrentPageSize != 0 && pgrSearch.GroupSize != 0)
                    {
                        startPosition = (pgrSearch.CurrentPage - 1) * pgrSearch.CurrentPageSize;
                        numberOfProceeded = (((pgrSearch.CurrentPage / pgrSearch.GroupSize) + 1) * pgrSearch.CurrentPageSize * pgrSearch.GroupSize) + pgrSearch.CurrentPageSize;
                        displayResults = pgrSearch.CurrentPageSize;
                    }

                    // Prepare parameters
                    SearchParameters parameters = new SearchParameters()
                    {
                        SearchFor = searchText,
                        SearchSort = null,
                        Path = null,
                        ClassNames = null,
                        CurrentCulture = "##ALL##",
                        DefaultCulture = null,
                        CombineWithDefaultCulture = false,
                        CheckPermissions = false,
                        SearchInAttachments = false,
                        User = MembershipContext.AuthenticatedUser,
                        SearchIndexes = sii.IndexName,
                        StartingPosition = startPosition,
                        DisplayResults = displayResults,
                        NumberOfProcessedResults = numberOfProceeded,
                        NumberOfResults = 0,
                        AttachmentWhere = null,
                        AttachmentOrderBy = null,
                    };

                    // Search
                    var results = SearchHelper.Search(parameters);

                    // Fill repeater with results
                    repSearchResults.DataSource = results.Items;
                    repSearchResults.PagerForceNumberOfResults = parameters.NumberOfResults;
                    repSearchResults.DataBind();

                    // Show now results found ?
                    if (parameters.NumberOfResults == 0)
                    {
                        lblNoResults.Text = "<br />" + GetString("srch.results.noresults.preview");
                        lblNoResults.Visible = true;

                        Exception searchError = results.LastError;
                        if (searchError != null)
                        {
                            pnlError.Visible = true;
                            lblError.Text = GetString("smartsearch.searcherror") + " " + searchError.Message;
                        }
                    }
                }
            }

            // Fill CMSDropDownList option with values
            ControlsHelper.FillListControlWithEnum<SearchModeEnum>(drpSearchMode, "srch.dialog", useStringRepresentation: true);
            drpSearchMode.SelectedValue = QueryHelper.GetString("searchmode", EnumHelper.GetDefaultValue<SearchModeEnum>().ToStringRepresentation());

            // Set up search text
            txtSearchFor.Text = QueryHelper.GetString("searchtext", "");
        }
    }


    /// <summary>
    /// Search button click handler.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string url = RequestContext.CurrentURL;

        // Remove pager query string
        url = URLHelper.RemoveParameterFromUrl(url, "page");

        // Update search text parameter
        url = URLHelper.UpdateParameterInUrl(url, "searchtext", HttpUtility.UrlEncode(txtSearchFor.Text));

        // Update search mode parameter
        url = URLHelper.UpdateParameterInUrl(url, "searchmode", HttpUtility.UrlEncode(drpSearchMode.SelectedValue));

        // Redirect
        URLHelper.Redirect(url);
    }
}