using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Search;
using CMS.UIControls;

public partial class CMSModules_SmartSearch_Controls_IndexInfo : CMSUserControl
{
    /// <summary>
    /// Gets or sets the search index.
    /// </summary>
    public SearchIndexInfo SearchIndex
    {
        get;
        set;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        LoadData();

        ScriptHelper.RegisterDialogScript(Page);
    }


    /// <summary>
    /// Loads the index information.
    /// </summary>
    public void LoadData()
    {
        if (SearchIndex == null)
        {
            return;
        }
        var isAzureIndex = IsAzureIndex();
        if (isAzureIndex)
        {
            SearchIndex.InvalidateIndexStatistics();
        }

        var indexStatus = SearchIndexInfoProvider.GetIndexStatus(SearchIndex);
        var isInAction = indexStatus == IndexStatusEnum.REBUILDING || indexStatus == IndexStatusEnum.OPTIMIZING;
        var isNotReady = !isInAction && indexStatus != IndexStatusEnum.READY;

        // Items count
        lblItemCount.Text = SearchIndex.IndexDocumentCount.HasValue ? SearchIndex.IndexDocumentCount.ToString() : GetString("general.notavailable");

        // File size
        lblFileSize.Text = SearchIndex.IndexSize.HasValue ? DataHelper.GetSizeString(SearchIndex.IndexSize.Value) : GetString("general.notavailable");

        // Status
        var statusString = (indexStatus == IndexStatusEnum.READY) && SearchIndex.IndexIsOutdated
                        ? "srch.status.readybutoutdated"
                        : "srch.status." + indexStatus;

        string statusName = GetString(statusString);

        // Show preloader image and link to thread log in status when in action 
        if (isInAction)
        {
            var statusText = "";
            if (SearchTaskInfoProvider.IndexerThreadGuid != Guid.Empty)
            {
                string url = UrlResolver.ResolveUrl("~/CMSModules/System/Debug/System_ViewLog.aspx");
                url = URLHelper.UpdateParameterInUrl(url, "threadGuid", SearchTaskInfoProvider.IndexerThreadGuid.ToString());
                if (WebFarmHelper.WebFarmEnabled)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "serverName", WebFarmHelper.ServerName);
                }
                statusText = "<a href=\"javascript:void(0)\" onclick=\"modalDialog('" + url + "', 'ThreadProgress', '1000', '700');\" >" + statusName + "</a>";
            }

            ltlStatus.Text = ScriptHelper.GetLoaderInlineHtml(statusText, "form-control-text");
            ltlStatus.Visible = true;
            lblStatus.Visible = false;
        }
        else
        {
            // Show colored status name
            lblStatus.Text = statusName;

            string cssClass = String.Empty;

            if (isNotReady)
            {
                cssClass = "StatusDisabled";
            }
            else if (indexStatus == IndexStatusEnum.READY)
            {
                cssClass = SearchIndex.IndexIsOutdated ? "color-orange-80" : "StatusEnabled";
            }

            lblStatus.CssClass += " " + cssClass;
        }

        // Is optimized visible only if the search index is Azure based
        DateTime lastFileUpdate = DateTimeHelper.ZERO_TIME;
        if (isAzureIndex)
        {
            plcOptimized.Visible = false;
        }
        else
        {
            lblIsOptimized.Text = UniGridFunctions.ColoredSpanYesNo(SearchIndex.IsOptimized());
            lastFileUpdate = SearchIndex.IndexFilesLastUpdate;
        }
        
        // Last update
        var lastUpdate = SearchIndex.IndexLastModified;
        if (lastFileUpdate > lastUpdate)
        {
            lastUpdate = lastFileUpdate;
        }
        
        lblLastUpdate.Text = lastUpdate.ToString();

        // Last rebuild
        lblLastRebuild.Text = SearchIndex.IndexLastRebuildTime != DateTimeHelper.ZERO_TIME 
            ? ValidationHelper.GetString(SearchIndex.IndexLastRebuildTime, "") 
            : GetString("general.notavailable");
    }


    private bool IsAzureIndex()
    {
        return SearchIndex.IndexProvider.Equals(SearchIndexInfo.AZURE_SEARCH_PROVIDER, StringComparison.OrdinalIgnoreCase);
    }
}
