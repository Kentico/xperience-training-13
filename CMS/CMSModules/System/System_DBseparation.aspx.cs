using System;
using System.Data.SqlClient;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.UIControls;


[CheckLicence(FeatureEnum.DBSeparation)]
public partial class CMSModules_System_System_DBseparation : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!SqlInstallationHelper.DatabaseIsSeparated())
        {
            var connectionString = new SqlConnectionStringBuilder(ConnectionHelper.ConnectionString);
            if (connectionString.IntegratedSecurity)
            {
                btnLaunch.Visible = false;
                ShowWarning(GetString("separationDB.errorwinauth"));
            }
            else
            {
                btnLaunch.ResourceString = "separationDB.launchseparation";
                ShowInformation(GetString("separationDB.separatedescription"));
                btnLaunch.OnClientClick = "modalDialog('" + Page.ResolveUrl("~/CMSInstall/SeparateDB.aspx") + "','DBSeparation', 620, 582); return false;";
            }
        }
        else
        {
            btnLaunch.ResourceString = "separationDB.launchjoin";
            ShowInformation(GetString("separationDB.joindescription"));
            btnLaunch.OnClientClick = "modalDialog('" + Page.ResolveUrl("~/CMSInstall/JoinDB.aspx") + "','DBJoin', 620, 582); return false;";
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { window.location.replace(window.location.href); }"));
        ScriptHelper.RegisterDialogScript(Page);
    }
}
