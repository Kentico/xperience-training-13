using System;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_SystemTables_Pages_Development_Views_View_Edit : GlobalAdminPage
{
    #region "Private variables"

    private string objName = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Query param validation
        string hash = QueryHelper.GetString("hash", null);

        string paramName = "new";
        string newView = QueryHelper.GetString(paramName, null);
        if (String.IsNullOrEmpty(newView))
        {
            paramName = "objname";
        }

        if (!QueryHelper.ValidateHashString(QueryHelper.GetString(paramName, null), hash))
        {
            ShowError(GetString("sysdev.views.corruptedparameters"));
            editSQL.Visible = false;
            return;
        }


        objName = HttpUtility.HtmlDecode(QueryHelper.GetString("objname", null));

        // Check if edited view exists and redirect to error page if not
        TableManager tm = new TableManager(null);

        if (!String.IsNullOrEmpty(objName) && !tm.ViewExists(objName))
        {
            EditedObject = null;
        }

        // Init edit area
        editSQL.ObjectName = objName;
        editSQL.HideSaveButton = objName != null;
        editSQL.IsView = true;
        editSQL.OnSaved += editSQL_OnSaved;
        bool loadedCorrectly = true;
        if (!RequestHelper.IsPostBack())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                ShowChangesSaved();
            }

            loadedCorrectly = editSQL.SetupControl();
        }

        // Create breadcrumbs
        CreateBreadcrumbs();
        
        if (objName != null)
        {
            // Save button
            HeaderActions.AddAction(new SaveAction
            {
                Enabled = loadedCorrectly,
                RegisterShortcutScript = loadedCorrectly
            });

            // Restore button
            if (editSQL.RollbackAvailable)
            {
                HeaderActions.AddAction(new HeaderAction
                {
                    Text = GetString("dbobjects.restoredefault"),
                    CommandName = "restoredefault"
                });
            }

            HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        }
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    private void CreateBreadcrumbs()
    {
        var isNew = String.IsNullOrEmpty(objName);
        if (isNew)
        {
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("sysdev.views"),
                RedirectUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Views", false),
            });
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("sysdev.views.createview")
            });
        }
        else
        {
            // Ensure suffix
            UIHelper.SetBreadcrumbsSuffix(GetString("sysdev.view"));
        }
    }


    #region "Event handlers"

    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "restoredefault":
                editSQL.Rollback();
                break;

            case ComponentEvents.SAVE:
                editSQL.SaveObject();
                break;
        }
    }


    private void editSQL_OnSaved(object sender, EventArgs e)
    {
        if (objName == null)
        {
            string url = UIContextHelper.GetElementUrl(ModuleName.CMS, "EditViews", false);
            url = URLHelper.AddParameterToUrl(url, "objname", HTMLHelper.HTMLEncode(editSQL.ObjectName));
            url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(editSQL.ObjectName, true));
            url = URLHelper.AddParameterToUrl(url, "saved", "1");
            URLHelper.Redirect(url);
        }

        ShowChangesSaved();
    }

    #endregion
}