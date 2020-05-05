using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.Newsletters.Web.UI.Internal;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.NEWSLETTER, "Newsletter.Subscribers")]
[EditedObject(NewsletterInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Subscribers : CMSNewsletterPage
{
    private const string SELECT = "SELECT";
    private const string APPROVE = "APPROVE";
    private const string REMOVE = "REMOVE";

    private NewsletterInfo mNewsletter;
    private readonly ISubscriptionService mSubscriptionService = Service.Resolve<ISubscriptionService>();
    private ObjectTransformationDataProvider mStatusDataProvider;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        CurrentMaster.ActionsViewstateEnabled = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        mNewsletter = EditedObject as NewsletterInfo;
        if (mNewsletter == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
            return;
        }

        if (!mNewsletter.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(mNewsletter.TypeInfo.ModuleName, "ManageSubscribers");
        }

        var recipientStatusCalculator = new RecipientStatusCalculator(mNewsletter.NewsletterID);
        mStatusDataProvider = new ObjectTransformationDataProvider();
        mStatusDataProvider.SetDefaultDataHandler((_, subscriberIds) => recipientStatusCalculator.GetStatuses(subscriberIds));

        ScriptHelper.RegisterDialogScript(this);

        CurrentMaster.DisplayActionsPanel = true;

        // Initialize unigrid
        UniGridSubscribers.WhereCondition = "NewsletterID = " + mNewsletter.NewsletterID;
        UniGridSubscribers.OnAction += UniGridSubscribers_OnAction;
        UniGridSubscribers.OnExternalDataBound += UniGridSubscribers_OnExternalDataBound;
        UniGridSubscribers.ZeroRowsText = GetString("newsletter.subscribers.nodata");
        UniGridSubscribers.FilteredZeroRowsText = GetString("newsletter.subscribers.noitemsfound");

        // Initialize selectors and mass actions
        SetupSelectors();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlActions.Visible = !DataHelper.DataSourceIsEmpty(UniGridSubscribers.GridView.DataSource);

        LoadMarketableRecipientsCount();
    }


    /// <summary>
    /// Configures selectors.
    /// </summary>
    private void SetupSelectors()
    {
        SetupContactGroupSelector();
        SetupContactSelector();

        // Initialize mass actions
        if (drpActions.Items.Count == 0)
        {
            drpActions.Items.Add(new ListItem(GetString("general.selectaction"), SELECT));
            drpActions.Items.Add(new ListItem(GetString("newsletter.approvesubscription"), APPROVE));
            drpActions.Items.Add(new ListItem(GetString("newsletter.deletesubscription"), REMOVE));
        }
    }


    private void SetupContactGroupSelector()
    {
        contactGroupsSelector.OnItemsSelected += CGSelector_OnItemsSelected;
        contactGroupsSelector.DialogButton.ButtonStyle = ButtonStyle.Primary;
        contactGroupsSelector.ZeroRowsText = string.Format(GetString("newsletter.subscribers.addcontactgroups.nodata"), UrlResolver.ResolveUrl(ApplicationUrlHelper.GetApplicationUrl(ModuleName.CONTACTMANAGEMENT, "ContactGroups")));
    }


    private void SetupContactSelector()
    {
        contactsSelector.WhereCondition = "NOT (ContactEmail IS NULL OR ContactEmail LIKE '')";
        contactsSelector.OnItemsSelected += ContactSelector_OnItemsSelected;
        contactsSelector.AdditionalSearchColumns = "ContactFirstName,ContactMiddleName,ContactEmail";
        contactsSelector.ZeroRowsText = string.Format(GetString("newsletter.subscribers.addcontacts.nodata"), UrlResolver.ResolveUrl(ApplicationUrlHelper.GetApplicationUrl(ModuleName.CONTACTMANAGEMENT, "ContactsFrameset", "?tabname=contacts")));
        contactsSelector.FilterControl = "~/CMSModules/ContactManagement/FormControls/SearchContactFullName.ascx";
        contactsSelector.UseDefaultNameFilter = false;
    }


    /// <summary>
    /// Unigrid external databound event handler.
    /// </summary>
    protected object UniGridSubscribers_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string sourceNameUpper = sourceName.ToUpperInvariant();
        bool isContactSubscriber = false;
        var param = parameter as DataRowView;

        if (param != null)
        {
            isContactSubscriber = string.Equals(ValidationHelper.GetString(param["SubscriberType"], string.Empty), PredefinedObjectType.CONTACT, StringComparison.OrdinalIgnoreCase);
        }

        switch (sourceNameUpper)
        {
            case APPROVE:
                bool approved = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionApproved"], true);
                CMSGridActionButton button = ((CMSGridActionButton)sender);
                button.Visible = !approved;
                break;

            case "STATUS":
                if (isContactSubscriber)
                {
                    var statusTransformation = new ObjectTransformation
                    {
                        ObjectType = "newsletter.subscribernewsletterlist",
                        ObjectID = ValidationHelper.GetInteger(((DataRowView)parameter)["SubscriberID"], 0),
                        DataProvider = mStatusDataProvider,
                        Transformation = "{% GetResourceString(\"emailmarketing.ui.\" + " + RecipientStatusCalculator.SUBSCRIPTION_STATUS + ") %}",
                        EncodeOutput = false
                    };

                    return statusTransformation;
                }

                return null;

            case "ISMARKETABLE":
                if (isContactSubscriber)
                {
                    var isMarketableTransformation = new ObjectTransformation
                    {
                        ObjectType = "newsletter.subscribernewsletterlist",
                        ObjectID = ValidationHelper.GetInteger(((DataRowView)parameter)["SubscriberID"], 0),
                        DataProvider = mStatusDataProvider,
                        Transformation = "<span class=\" {% (" + RecipientStatusCalculator.EMAIL_RECIPIENT_STATUS + " == \"Marketable\") ? \"tag tag-active\" : \"tag tag-incomplete\" %} \"> {% GetResourceString(\"emailmarketing.status.\" + " + RecipientStatusCalculator.EMAIL_RECIPIENT_STATUS + ") %} </span>",
                        EncodeOutput = false
                    };

                    return isMarketableTransformation;
                }

                return null;
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridSubscribers_OnAction(string actionName, object actionArgument)
    {
        // Check 'manage subscribers' permission
        CheckAuthorization();

        int subscriberId = ValidationHelper.GetInteger(actionArgument, 0);

        DoSubscriberAction(subscriberId, actionName);
    }


    /// <summary>
    /// Checks if the user has permission to manage subscribers.
    /// </summary>
    private static void CheckAuthorization()
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToAccessDenied("cms.newsletter", "managesubscribers");
        }
    }


    /// <summary>
    /// Contact group items selected event handler.
    /// </summary>
    protected void CGSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        // Get new items from selector
        string newValues = ValidationHelper.GetString(contactGroupsSelector.Value, null);

        // Get added items
        string[] newItems = newValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in newItems)
        {
            int contactGroupId = ValidationHelper.GetInteger(item, 0);
            ContactGroupInfo contactGroup = ContactGroupInfo.Provider.Get(contactGroupId);
            mSubscriptionService.Subscribe(contactGroup, mNewsletter);
        }

        contactGroupsSelector.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Contact items selected event handler.
    /// </summary>
    protected void ContactSelector_OnItemsSelected(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        // Get new items from selector
        string newValues = ValidationHelper.GetString(contactsSelector.Value, null);

        // Get added items
        string[] newItems = newValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string item in newItems)
        {
            int contactID = ValidationHelper.GetInteger(item, 0);
            ContactInfo contact = ContactInfo.Provider.Get(contactID);
            mSubscriptionService.Subscribe(contact, mNewsletter, new SubscribeSettings
            {
                AllowOptIn = false,
                SendConfirmationEmail = false,
                RemoveUnsubscriptionFromNewsletter = false,
                RemoveAlsoUnsubscriptionFromAllNewsletters = false,
            });
        }

        contactsSelector.Value = null;
        UniGridSubscribers.ReloadData();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Handles multiple selector actions.
    /// </summary>
    protected void btnOk_Clicked(object sender, EventArgs e)
    {
        // Check permissions
        CheckAuthorization();

        if (drpActions.SelectedValue != SELECT)
        {
            // Go through all selected items
            if (UniGridSubscribers.SelectedItems.Count != 0)
            {
                foreach (string subscriberId in UniGridSubscribers.SelectedItems)
                {
                    int subscriberIdInt = ValidationHelper.GetInteger(subscriberId, 0);

                    DoSubscriberAction(subscriberIdInt, drpActions.SelectedValue);
                }
            }
        }
        UniGridSubscribers.ResetSelection();
        UniGridSubscribers.ReloadData();
    }


    /// <summary>
    /// Performs action on given subscriber.
    /// </summary>
    /// <param name="subscriberId">Id of subscriber</param>
    /// <param name="actionName">Name of action</param>
    private void DoSubscriberAction(int subscriberId, string actionName)
    {
        try
        {
            // Check manage subscribers permission
            var subscriber = SubscriberInfo.Provider.Get(subscriberId);
            if (!subscriber.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                RedirectToAccessDenied(subscriber.TypeInfo.ModuleName, "ManageSubscribers");
            }

            switch (actionName.ToUpperInvariant())
            {
                // Remove subscription
                case REMOVE:
                    mSubscriptionService.RemoveSubscription(subscriberId, mNewsletter.NewsletterID, false);
                    break;

                // Approve subscription
                case APPROVE:
                    SubscriberNewsletterInfoProvider.ApproveSubscription(subscriberId, mNewsletter.NewsletterID);
                    break;
            }
        }
        catch (Exception exception)
        {
            LogAndShowError("Newsletter subscriber", "NEWSLETTERS", exception);
        }
    }


    private void LoadMarketableRecipientsCount()
    {
        var newsletterMarketableRecipientsCount = NewsletterHelper.GetNewsletterMarketableRecipientsCount(mNewsletter);
        ltlTotalRecipientsCount.Text = ResHelper.GetStringFormat("emailmarketing.ui.recipientsheader", newsletterMarketableRecipientsCount);
    }
}