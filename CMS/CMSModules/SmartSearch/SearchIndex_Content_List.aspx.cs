using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_SmartSearch_SearchIndex_Content_List : GlobalAdminPage
{
    // Index id
    private int indexId = QueryHelper.GetInteger("indexid", 0);
    private bool displayHeaderActions = true;
    private bool displaySpecialActions;
    private bool isOnLineFormEdit;


    protected void Page_Load(object sender, EventArgs e)
    {
        string addAllowed = GetString("srch.index.addcontent");
        string addExcluded = GetString("srch.index.addexcluded");

        contentList.StopProcessing = true;

        // Get search index info
        SearchIndexInfo sii = SearchIndexInfoProvider.GetSearchIndexInfo(indexId);

        if (sii != null)
        {
            // Switch by index type
            switch (sii.IndexType)
            {
                // Documents
                case TreeNode.OBJECT_TYPE:
                case SearchHelper.DOCUMENTS_CRAWLER_INDEX:
                    contentList.StopProcessing = false;
                    contentList.Visible = true;
                    contentList.OnAction += contentList_OnAction;
                    break;

                // Users
                case UserInfo.OBJECT_TYPE:
                    userList.StopProcessing = false;
                    userList.Visible = true;
                    displayHeaderActions = false;
                    break;

                // Custom tables
                case CustomTableInfo.OBJECT_TYPE_CUSTOMTABLE:
                    customTableList.Visible = true;
                    customTableList.StopProcessing = false;
                    customTableList.OnAction += contentList_OnAction;
                    displayHeaderActions = false;
                    displaySpecialActions = true;
                    break;

                // Custom tables
                case PredefinedObjectType.BIZFORM:
                    onLineFormList.Visible = true;
                    onLineFormList.StopProcessing = false;
                    onLineFormList.OnAction += contentList_OnAction;
                    displayHeaderActions = false;
                    displaySpecialActions = true;
                    isOnLineFormEdit = true;
                    break;

                // General index
                case SearchHelper.GENERALINDEX:
                    generalList.Visible = true;
                    generalList.StopProcessing = false;
                    displayHeaderActions = false;
                    break;

                // Custom search
                case SearchHelper.CUSTOM_SEARCH_INDEX:
                    customList.Visible = true;
                    customList.StopProcessing = false;
                    displayHeaderActions = false;
                    break;
            }
        }

        // Display header action if it is required
        if (displayHeaderActions)
        {
            HeaderAction allowed = new HeaderAction();
            allowed.Text = addAllowed;
            allowed.RedirectUrl = ResolveUrl("SearchIndex_Content_Edit.aspx?indexid=" + indexId + "&itemtype=" + SearchIndexSettingsInfo.TYPE_ALLOWED);
            CurrentMaster.HeaderActions.AddAction(allowed);

            HeaderAction excluded = new HeaderAction();
            excluded.Text = addExcluded;
            excluded.RedirectUrl = ResolveUrl("SearchIndex_Content_Edit.aspx?indexid=" + indexId + "&itemtype=" + SearchIndexSettingsInfo.TYPE_EXLUDED);
            CurrentMaster.HeaderActions.AddAction(excluded);
        }

        if (displaySpecialActions)
        {
            HeaderAction actions = new HeaderAction();
            actions.Text = isOnLineFormEdit ? GetString("srch.index.addonlineform") : GetString("srch.index.addcustomtable");
            actions.RedirectUrl = ResolveUrl("SearchIndex_Content_Edit.aspx?indexid=" + indexId + "&itemtype=" + SearchIndexSettingsInfo.TYPE_ALLOWED);
            CurrentMaster.HeaderActions.AddAction(actions);
        }
    }


    /// <summary>
    /// Action event handler.
    /// </summary>
    private void contentList_OnAction(object sender, CommandEventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl("SearchIndex_Content_Edit.aspx?indexid=" + indexId + "&guid=" + e.CommandArgument));
    }
}