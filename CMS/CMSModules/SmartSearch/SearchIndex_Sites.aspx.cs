using System;
using System.Linq;
using System.Web.UI;

using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_SearchIndex_Sites : GlobalAdminPage, IPostBackEventHandler
{
    private int indexId = 0;
    private string currentValues = string.Empty;


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check read permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.searchindex", CMSAdminControl.PERMISSION_READ))
        {
            RedirectToAccessDenied("cms.searchindex", CMSAdminControl.PERMISSION_READ);
        }

        indexId = QueryHelper.GetInteger("indexid", 0);

        // Get the user sites
        currentValues = GetIndexSites();

        if (!RequestHelper.IsPostBack())
        {
            usSites.Value = currentValues;
        }

        usSites.OnSelectionChanged += usSites_OnSelectionChanged;
    }


    /// <summary>
    /// Returns string with site ids where user is member.
    /// </summary>    
    private string GetIndexSites()
    {
        var siteIDs = SearchIndexSiteInfoProvider.GetIndexSiteBindings(indexId).Column("IndexSiteID").Select(x => x.IndexSiteID).ToList();
        if (siteIDs.Count > 0)
        {
            return TextHelper.Join(";", siteIDs);
        }

        return String.Empty;
    }


    /// <summary>
    /// Handles site selector selection change event.
    /// </summary>
    protected void usSites_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveIndexes();
    }


    /// <summary>
    /// Saves changes in site assignment.
    /// </summary>
    protected void SaveIndexes()
    {
        // Check permissions 
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.searchindex", CMSAdminControl.PERMISSION_MODIFY))
        {
            RedirectToAccessDenied("cms.searchindex", CMSAdminControl.PERMISSION_MODIFY);
        }

        // Remove old items
        string newValues = ValidationHelper.GetString(usSites.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to site
                foreach (string item in newItems)
                {
                    int siteId = ValidationHelper.GetInteger(item, 0);

                    // Unassign site from index
                    SearchIndexSiteInfo.Provider.Remove(indexId, siteId);
                }
            }
        }


        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to site
                foreach (string item in newItems)
                {
                    int siteId = ValidationHelper.GetInteger(item, 0);

                    // Assign site to index
                    SearchIndexSiteInfo.Provider.Add(indexId, siteId);
                }
            }
        }

        // Show saved message with rebuild link
        ShowChangesSaved();
        ShowInformation(String.Format(GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + Page.ClientScript.GetPostBackEventReference(this, "saved") + "\">" + GetString("General.clickhere") + "</a>"));
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Handles click on rebuild link (after sites are saved).
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "saved")
        {
            // Check permissions 
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.searchindex", CMSAdminControl.PERMISSION_MODIFY))
            {
                RedirectToAccessDenied("cms.searchindex", CMSAdminControl.PERMISSION_MODIFY);
            }

            SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(indexId);
            if (sii.IndexType.Equals(CMS.DocumentEngine.TreeNode.OBJECT_TYPE, StringComparison.OrdinalIgnoreCase) || (sii.IndexType == SearchHelper.DOCUMENTS_CRAWLER_INDEX))
            {
                if (!SearchIndexCultureInfoProvider.SearchIndexHasAnyCulture(sii.IndexID))
                {
                    ShowError(GetString("index.noculture"));
                    return;
                }

                if (!SearchIndexSiteInfoProvider.SearchIndexHasAnySite(sii.IndexID))
                {
                    ShowError(GetString("index.nosite"));
                    return;
                }
            }

            if (SearchHelper.CreateRebuildTask(indexId))
            {
                ShowInformation(GetString("srch.index.rebuildstarted"));
            }
            else
            {
                ShowError(GetString("index.nocontent"));
            }
        }
    }

    #endregion
}