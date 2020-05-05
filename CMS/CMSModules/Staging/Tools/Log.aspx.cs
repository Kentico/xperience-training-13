using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Synchronization.Web.UI;


public partial class CMSModules_Staging_Tools_Log : CMSStagingPage
{
    private int serverId = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check hash
        if (!QueryHelper.ValidateHash("hash"))
        {
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
        }
        else
        {
            string element = QueryHelper.GetString("tasktype", null);

            // Check UI permissions for Staging
            var user = MembershipContext.AuthenticatedUser;
            if (!user.IsAuthorizedPerUIElement("cms.staging", element))
            {
                RedirectToUIElementAccessDenied("cms.staging", element);
            }

            // Check 'Manage XXX tasks' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.staging", "Manage" + element + "Tasks"))
            {
                RedirectToAccessDenied("cms.staging", "Manage" + element + "Tasks");
            }

            // Register modal dialog scripts
            RegisterModalPageScripts();
            PageTitle.TitleText = GetString("Task.LogHeader");

            serverId = QueryHelper.GetInteger("serverid", 0);
            int taskId = QueryHelper.GetInteger("taskid", 0);

            var condition = new WhereCondition().WhereEquals("SynchronizationTaskID", taskId);

            if (serverId > 0)
            {
                condition.WhereEquals("SynchronizationServerID", serverId);
            }

            gridLog.WhereCondition = condition.ToString(true);
            gridLog.ZeroRowsText = GetString("Task.LogNoEvents");
            gridLog.OnBeforeDataReload += gridLog_OnBeforeDataReload;
        }
    }

    protected void gridLog_OnBeforeDataReload()
    {
        if (serverId > 0)
        {
            gridLog.GridView.Columns[2].Visible = false;
        }
    }
}