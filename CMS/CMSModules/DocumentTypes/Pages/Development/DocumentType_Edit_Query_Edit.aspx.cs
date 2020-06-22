using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;

// Actions
[SaveAction(0)]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Query_Edit : CMSModalDesignPage
{
    #region "Variables"

    private bool mDialogMode;
    private bool mIsEditMode;
    private bool mIsGenerated;
    private QueryInfo mQuery;
    private int mQueryClassId;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        mDialogMode = QueryHelper.GetBoolean("editonlycode", false);

        if (mDialogMode)
        {
            // Check permissions - user must have the permission to edit the code
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Design", "EditSQLCode"))
            {
                RedirectToAccessDenied("CMS.Design", "EditSQLCode");
            }

            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }
        else
        {
            CheckGlobalAdministrator();
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        GetParameters();

        if (mIsEditMode && (mQuery != null))
        {
            SetEditedObject(mQuery, mDialogMode ? "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Query_Frameset.aspx" : "");
        }

        // Set QueryEdit params - must be initialized before load
        queryEdit.RefreshPageURL = !mIsEditMode ? GetEditUrl() : "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Query_Edit.aspx?tabmode=1";
        queryEdit.ClassID = mQueryClassId;
        queryEdit.QueryID = (mQuery != null ? mQuery.QueryID : 0);
        queryEdit.DialogMode = mDialogMode;
        queryEdit.EditMode = mIsEditMode;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UserInfo ui = MembershipContext.AuthenticatedUser;
        queryEdit.IsSiteManager = !mDialogMode && ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);

        if (mDialogMode)
        {
            SetDialogMode();
        }
        else
        {
            if (!TabMode || !mIsEditMode)
            {
                InitBreadcrumbs();
            }
        }

        // Update breadcrumbs on save
        if (!RequestHelper.IsPostBack() && (QueryHelper.GetInteger("saved", 0) == 1))
        {
            ScriptHelper.RefreshTabHeader(Page, mQuery.QueryName);
        }
    }


    private void GetParameters()
    {
        // Get query depending on whether this was invoked from a dialog or site manager
        if (mDialogMode)
        {
            string queryName = QueryHelper.GetString("name", string.Empty);
            if (queryName != string.Empty)
            {
                mQuery = QueryInfoProvider.GetQueryInfo(queryName, throwException: false);
                string encodedQueryName = HTMLHelper.HTMLEncode(queryName);

                // If edit was called from dialog but wrong query is specified, return
                if (mQuery == null)
                {
                    // Non-existing query
                    ShowError(GetString("query.querynotexist").Replace("%%code%%", encodedQueryName));
                    queryEdit.Visible = false;
                    mIsEditMode = true;
                    return;
                }
                else if (mQuery.QueryID <= 0)
                {
                    // Generated query
                    ShowInformation(String.Format(GetString("query.isgenerated"), encodedQueryName));
                    queryEdit.Visible = false;
                    mIsEditMode = true;
                    mIsGenerated = true;
                    return;
                }
            }
        }
        else
        {
            int queryId = QueryHelper.GetInteger("objectid", 0);
            if (queryId > 0)
            {
                mQuery = QueryInfo.Provider.Get(queryId);
            }
        }

        // If query not specified, a new query will be created, so get document type's ID
        mIsEditMode = (mQuery != null);

        if (!mIsEditMode)
        {
            mQueryClassId = QueryHelper.GetInteger("parentobjectid", 0);
        }
    }


    private void SetDialogMode()
    {
        if (!TabMode)
        {
            SetTitle(mIsEditMode ? GetString("query.edit") : GetString("query.new"));
        }

        if (!mIsGenerated && (!mIsEditMode || (mQuery != null)))
        {
            // Do not bind Save event for generated queries - those cannot be saved
            Save += (s, ea) => queryEdit.Save(true);
        }

        // Set refresh URL so that we don't lose dialog mode        
        queryEdit.RefreshPageURL = "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Query_Edit.aspx";
    }


    private void InitBreadcrumbs()
    {
        int documentTypeId = (mQuery == null) ? mQueryClassId : mQuery.ClassID;
        SetBreadcrumb(0, GetString("DocumentType_Edit_Query_Edit.Queries"), string.Format("~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Query_List.aspx?parentobjectid={0}", documentTypeId), TabMode ? "_parent" : null, null);
        SetBreadcrumb(1, mIsEditMode ? mQuery.QueryName : GetString("DocumentType_Edit_Query_Edit.NewQueryName"), null, null, null);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.DocumentEngine", "EditQuery");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false");
        }

        return String.Empty;
    }

    #endregion
}