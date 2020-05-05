using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_SystemTables_Pages_Development_Views_Views_List : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Prepare the new object header element
        string url = UIContextHelper.GetElementUrl(ModuleName.CMS, "NewView", false);
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash("1", true));
        url = URLHelper.AddParameterToUrl(url, "new", "1");
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("sysdev.views.createview"),
            RedirectUrl = url
        });

        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("sysdev.views.refreshviews"),
            CommandName = "refreshallviews"
        });

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        lstSQL.Views = true;
        lstSQL.EditPage = UIContextHelper.GetElementUrl(ModuleName.CMS, "EditViews", false);
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "refreshallviews")
        {
            lstSQL.RefreshAllViews();
            ShowConfirmation(GetString("systbl.views.allviewsrefreshed"));
        }
    }
}