using System;

using CMS.Activities;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[Action(0, "om.activitytype.new", "New.aspx")]
[UIElement(ModuleName.ONLINEMARKETING, "ActivityTypes")]
[Security(GlobalAdministrator = true)]
public partial class CMSModules_Activities_Pages_Tools_Activities_ActivityType_List : CMSContactManagementPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        int currSiteId = SiteContext.CurrentSiteID;

        bool globalObjectsSelected = (currSiteId == UniSelector.US_GLOBAL_RECORD);
        bool allSitesSelected = (currSiteId == UniSelector.US_ALL_RECORDS);

        // Show info if activity logging is disabled (do not show anything if global objects or all sites is selected)
        ucDisabledModule.ParentPanel = pnlDis;
        pnlDis.Visible = !globalObjectsSelected && !allSitesSelected && !ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(SiteContext.CurrentSiteName);

        if (CurrentMaster.HeaderActions.ActionsList.Count > 0)
        {
            CurrentMaster.HeaderActions.ActionsList[0].RedirectUrl += RequestContext.CurrentQueryString;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        CurrentMaster.HeaderActions.Enabled = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ACTIVITIES, "ManageActivities");
    }

    #endregion
}