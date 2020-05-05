using System;
using System.Linq;

using CMS.Activities;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[UIElement(ModuleName.ONLINEMARKETING, "ContactActivities")]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Activities : CMSContactManagementPage
{
    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObject != null)
        {
            var contact = (ContactInfo)EditedObject;

            ucDisabledModule.TestSettingKeys = "CMSEnableOnlineMarketing;CMSCMActivitiesEnabled";
            ucDisabledModule.ParentPanel = pnlDis;

            pnlDis.Visible = !ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(SiteContext.CurrentSiteID);

            listElem.ShowSiteNameColumn = true;
            listElem.SiteID = UniSelector.US_ALL_RECORDS;
            listElem.ContactID = contact.ContactID;
            listElem.OrderBy = "ActivityCreated DESC";

            // Init header action for new custom activities only if custom activity type exists and user is authorized to manage activities
            if (ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(SiteContext.CurrentSiteName) && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ACTIVITIES, "ManageActivities"))
            {
                // Disable manual creation of activity if no custom activity type is available
                var activityType = ActivityTypeInfo.Provider.Get()
                                                   .WhereEquals("ActivityTypeIsCustom", 1)
                                                   .WhereEquals("ActivityTypeEnabled", 1)
                                                   .WhereEquals("ActivityTypeManualCreationAllowed", 1)
                                                   .TopN(1)
                                                   .Column("ActivityTypeID")
                                                   .FirstOrDefault();

                if (activityType != null)
                {
                    // Prepare target URL
                    string url = ResolveUrl($"~/CMSModules/Activities/Pages/Tools/Activities/Activity/New.aspx?contactId={contact.ContactID}");

                    // Init header action
                    HeaderAction action = new HeaderAction()
                    {
                        Text = GetString("om.activity.newcustom"),
                        RedirectUrl = url
                    };
                    CurrentMaster.HeaderActions.ActionsList.Add(action);
                }
            }

            if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
            {
                // Display 'Save' message after new custom activity was created
                ShowChangesSaved();
            }
        }
    }


    protected override void OnLoadComplete(EventArgs e)
    {
        if (EditedObject != null)
        {
            InitializeCampaignJourney((ContactInfo)EditedObject);
        }
    }


    private void InitializeCampaignJourney(ContactInfo contact)
    {
        var contactJourneyService = Service.Resolve<IContactJourneyService>();
        var contactJourney = contactJourneyService.GetContactJourneyForContact(contact.ContactID);

        if(contactJourney != null)
        {
            lblJourneyLenght.Text = HTMLHelper.HTMLEncode(contactJourney.JourneyLengthDaysText);
            lblLastActivity.Text = HTMLHelper.HTMLEncode(contactJourney.LastActivityDaysAgoText);
            lblJourneyStarted.Text = HTMLHelper.HTMLEncode(contactJourney.JourneyLengthStartedDate);
            lblLastActivityDate.Text = HTMLHelper.HTMLEncode(contactJourney.LastActivityDate);
        }
        else
        {
            pnlJourney.Visible = false;
            hdrActivities.Visible = false;
        }
    }
}