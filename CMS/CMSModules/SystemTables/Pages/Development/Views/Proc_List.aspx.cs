using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_SystemTables_Pages_Development_Views_Proc_List : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Prepare the new object header element
        string url = UIContextHelper.GetElementUrl(ModuleName.CMS, "EditStoredProcedures", false);
        url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash("1", true));
        url = URLHelper.AddParameterToUrl(url, "new", "1");
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("sysdev.procedures.createprocedure"),
            RedirectUrl = url
        });

        lstSQL.Views = false;
        lstSQL.EditPage = UIContextHelper.GetElementUrl(ModuleName.CMS, "EditStoredProcedures", false);
    }
}