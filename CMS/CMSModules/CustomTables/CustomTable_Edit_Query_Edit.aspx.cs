using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;

// Actions
[SaveAction(0)]

// Set edited object
[EditedObject("cms.query", "objectid")]
[UIElement(ModuleName.CUSTOMTABLES, "EditQuery.General")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Query_Edit : CMSCustomTablesPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions - user must have the permission to edit the code
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Design", "EditSQLCode"))
        {
            RedirectToAccessDenied("CMS.Design", "EditSQLCode");
        }

        Title = "Custom table edit - Query";

        // Get the current class ID
        int classId = QueryHelper.GetInteger("parentobjectid", 0);

        QueryInfo query = EditedObject as QueryInfo;

        queryEdit.IsSiteManager = true;

        // If the existing query is being edited
        if (query != null)
        {
            queryEdit.QueryID = query.QueryID;
            queryEdit.EditMode = true;
            queryEdit.RefreshPageURL = "~/CMSModules/CustomTables/CustomTable_Edit_Query_Edit.aspx";
        }
        else
        {
            queryEdit.QueryID = 0;
            queryEdit.RefreshPageURL = GetEditUrl();
            InitBreadcrumb("customtable.edit.newquery", classId);
        }

        // If the new query is being created        
        if (classId > 0)
        {
            queryEdit.ClassID = classId;
        }

        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            // Reload header if changes were saved
            ScriptHelper.RefreshTabHeader(Page, query.QueryName);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.CustomTables", "EditQuery");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "displaytitle=false");
        }

        return String.Empty;
    }


    /// <summary>
    /// Sets the page breadcrumb control.
    /// </summary>
    /// <param name="caption">Caption of the breadcrumb item displayed in the page title as resource string key</param>
    /// <param name="classId">ID of a class</param>
    private void InitBreadcrumb(string caption, int classId)
    {
        string backUrl = "~/CMSModules/CustomTables/CustomTable_Edit_Query_List.aspx";

        if (classId > 0)
        {
            backUrl += "?parentobjectid=" + classId;
        }

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = GetString("general.queries"),
            RedirectUrl = backUrl
        });

        // Add the custom item
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Text = GetString(caption)
        });
    }

    #endregion
}