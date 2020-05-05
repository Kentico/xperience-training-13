using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Query_Header : CMSModalDesignPage
{
    #region "Variables"

    private bool mDialogMode;

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        mDialogMode = QueryHelper.GetBoolean("editonlycode", false);

        if (mDialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/TabsHeader.master";
        }
        else
        {
            CheckGlobalAdministrator();
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (PersistentEditedObject == null)
        {
            // Get query depending on whether this was invoked from a dialog or site manager
            if (mDialogMode)
            {
                string queryName = QueryHelper.GetString("name", string.Empty);
                if (queryName != string.Empty)
                {
                    PersistentEditedObject = QueryInfoProvider.GetQueryInfo(queryName, throwException: false);
                }
            }
            else
            {
                int queryId = QueryHelper.GetInteger("queryid", 0);
                if (queryId > 0)
                {
                    PersistentEditedObject = QueryInfoProvider.GetQueryInfo(queryId);
                }
            }
        }

        QueryInfo query = PersistentEditedObject as QueryInfo;

        // Initialize breadcrumbs and tabs
        if (query != null)
        {
            SetEditedObject(query, null);

            if (!mDialogMode)
            {
                SetBreadcrumb(0, GetString("DocumentType_Edit_Query_Edit.Queries"), ResolveUrl("~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Query_List.aspx?documenttypeid=" + query.ClassID), "_parent", null);
                SetBreadcrumb(1, query.QueryName, null, null, null);
            }
            else
            {
                SetTitle(GetString("query.edit"));

                string selector = QueryHelper.GetControlClientId("selectorid", string.Empty);
                if (!string.IsNullOrEmpty(selector) && RequestHelper.IsPostBack())
                {
                    ScriptHelper.RegisterWOpenerScript(this);
                    // Add selector refresh
                    string script = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{ wopener.US_SelectNewValue_{0}('{1}'); }}", selector, query.QueryFullName);

                    ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script, true);
                }
            }

            // Set tabs number and ensure additional tab
            InitTabs("q_edit_content");

            string url = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Query_Edit.aspx" + RequestContext.CurrentQueryString;
            url = URLHelper.RemoveParameterFromUrl(url, "saved");

            if (mDialogMode)
            {
                url = URLHelper.AddParameterToUrl(url, "name", query.QueryFullName);
            }
            else
            {
                url = URLHelper.AddParameterToUrl(url, "queryid", query.QueryID.ToString());
            }
            SetTab(0, GetString("general.general"), ResolveUrl(url), String.Empty);
        }
    }
}
