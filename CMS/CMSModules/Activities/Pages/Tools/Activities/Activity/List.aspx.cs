using System;
using System.Linq;

using CMS.Activities;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "Activities")]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_Activities_Pages_Tools_Activities_Activity_List : CMSContactManagementPage
{
    #region "Variables"

    private int mCurrentSiteID;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mCurrentSiteID = SiteContext.CurrentSiteID;
        
        // Show info if activity logging is disabled (do not show anything if global objects or all sites is selected)
        ucDisabledModule.ParentPanel = pnlDis;
        pnlDis.Visible = !ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(SiteContext.CurrentSiteName);

        // Initialize list and filter controls, display all activities
        listElem.SiteID = UniSelector.US_ALL_RECORDS;
        listElem.OrderBy = "ActivityCreated DESC";
        listElem.ShowSiteNameColumn = true;

        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            ShowChangesSaved();
        }

        // Set header actions (add button)
        var url = ResolveUrl("New.aspx?siteId=" + mCurrentSiteID);

        hdrActions.AddAction(new HeaderAction()
        {
            Text = GetString("om.activity.newcustom"),
            RedirectUrl = url
        });
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable manual creation of activity if no custom activity type is available
        var activityType = ActivityTypeInfo.Provider.Get()
                                                   .WhereEquals("ActivityTypeIsCustom", 1)
                                                   .WhereEquals("ActivityTypeEnabled", 1)
                                                   .WhereEquals("ActivityTypeManualCreationAllowed", 1)
                                                   .TopN(1)
                                                   .Column("ActivityTypeID")
                                                   .FirstOrDefault();

        bool aCustomActivityExists = (activityType != null);

        // Disable actions for unauthorized users
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ACTIVITIES, "ManageActivities"))
        {
            hdrActions.Enabled = false;
        }
        // Allow new button only if custom activity exists
        else if (!aCustomActivityExists)
        {
            lblWarnNew.ResourceString = "om.activities.nocustomactivity";
            hdrActions.Enabled = false;
            lblWarnNew.Visible = true;
        }
    }

    #endregion
}