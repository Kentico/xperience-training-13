using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.NEWSLETTER, "Newsletter.Configuration")]
[EditedObject(NewsletterInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Configuration : CMSNewsletterPage
{
    private string mCurrentTemplates;

    /// <summary>
    /// It is true if edited newsletter is dynamic.
    /// </summary>
    protected bool isDynamic = false;


    /// <summary>
    /// Determines if newsletter tracking is enabled.
    /// </summary>
    private bool? mTrackingEnabled;

    private bool TrackingEnabled
    {
        get
        {
            if (mTrackingEnabled == null)
            {
                mTrackingEnabled = NewsletterHelper.IsTrackingAvailable();
            }

            return (bool)mTrackingEnabled;
        }
    }


    /// <summary>
    /// Determines if Online Marketing is enabled.
    /// </summary>
    private bool? mOnlineMarketingEnabled;

    private bool OnlineMarketingEnabled
    {
        get
        {
            if (mOnlineMarketingEnabled == null)
            {
                mOnlineMarketingEnabled = NewsletterHelper.OnlineMarketingAvailable(SiteContext.CurrentSiteName);
            }

            return (bool)mOnlineMarketingEnabled;
        }
    }


    private NewsletterInfo EditedNewsletter
    {
        get
        {
            return (NewsletterInfo)EditedObject;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedNewsletter == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
            return;
        }

        // Set validation messages
        rfvNewsletterDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvNewsletterName.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptyName");
        rfvNewsletterSenderName.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptySenderName");
        rfvNewsletterSenderEmail.ErrorMessage = GetString("Newsletter_Edit.ErrorEmptySenderEmail");
        rfvNewsletterDynamicURL.ErrorMessage = GetString("newsletter_edit.sourcepageurlempty");

        // Enable client side validation for emails
        txtNewsletterSenderEmail
            .EnableClientSideEmailFormatValidation(errorMessageResourceString: "Newsletter_Edit.ErrorEmptySenderEmail")
            .RegisterCustomValidator(rfvNewsletterSenderEmail);

        // Hide detailed report as it could confuse non-technical users
        txtNewsletterDynamicURL.ShowDetailedError = false;
        txtNewsletterDynamicURL.StatusErrorMessage = GetString("general.pagenotfound");

        // Register save button
        CurrentMaster.HeaderActions.AddAction(new SaveAction());
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        InitializeTooltips();

        if (EditedFeedIsEmailCampaign())
        {
            hdrDoubleOptIn.Visible = false;
            pnlDoubleOptIn.Visible = false;
            pnlNewsletterSubscriptionTemplate.Visible = false;
        }

        LoadData();
    }


    protected void LoadData()
    {
        if (!EditedNewsletter.CheckPermissions(PermissionsEnum.Modify, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(EditedNewsletter.TypeInfo.ModuleName, "Configure");
        }

        int siteId = EditedNewsletter.NewsletterSiteID;

        // Initialize template selectors
        string whereTemplate = "TemplateType='{0}' AND TemplateSiteID=" + siteId;

        subscriptionTemplate.WhereCondition = String.Format(whereTemplate, EmailTemplateTypeEnum.Subscription.ToStringRepresentation());
        unsubscriptionTemplate.WhereCondition = String.Format(whereTemplate, EmailTemplateTypeEnum.Unsubscription.ToStringRepresentation());
        optInSelector.WhereCondition = String.Format(whereTemplate, EmailTemplateTypeEnum.DoubleOptIn.ToStringRepresentation());
        usTemplates.WhereCondition = String.Format(whereTemplate, EmailTemplateTypeEnum.Issue.ToStringRepresentation());

        // Check if the newsletter is dynamic and adjust config dialog
        isDynamic = String.Equals(EditedNewsletter.NewsletterSource, NewsletterSource.Dynamic, StringComparison.InvariantCultureIgnoreCase);

        // Display template/dynamic based newsletter config and online marketing config
        plcDynamic.Visible = isDynamic;
        plcTemplates.Visible = !isDynamic;
        plcTracking.Visible = TrackingEnabled;
        plcOM.Visible = OnlineMarketingEnabled;

        if (!RequestHelper.IsPostBack())
        {
            if (QueryHelper.GetBoolean("saved", false))
            {
                // If user was redirected from newsletter_new.aspx, display the 'Changes were saved' message
                ShowChangesSaved();
            }

            // Fill config dialog with newsletter data
            GetNewsletterValues(EditedNewsletter);

            if (!isDynamic)
            {
                LoadTemplates();
            }
            else
            {
                // Check if dynamic newsletter subject is empty
                bool subjectEmpty = string.IsNullOrEmpty(EditedNewsletter.NewsletterDynamicSubject);
                radPageTitle.Checked = subjectEmpty;
                radFollowing.Checked = !subjectEmpty;
                txtSubject.Enabled = radFollowing.Checked;

                if (!subjectEmpty)
                {
                    txtSubject.Text = EditedNewsletter.NewsletterDynamicSubject;
                }

                txtNewsletterDynamicURL.Value = EditedNewsletter.NewsletterDynamicURL;

                TaskInfo task = TaskInfo.Provider.Get(EditedNewsletter.NewsletterDynamicScheduledTaskID);
                if (task != null)
                {
                    chkSchedule.Checked = true;
                    schedulerInterval.Visible = true;
                    schedulerInterval.ScheduleInterval = task.TaskInterval;
                }
                else
                {
                    chkSchedule.Checked = false;
                    schedulerInterval.Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Save button action.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                SaveData();

                break;
        }
    }


    /// <summary>
    /// Saves configuration changes.
    /// </summary>
    protected void SaveData()
    {
        // Check "configure" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "configure"))
        {
            RedirectToAccessDenied("cms.newsletter", "configure");
        }

        string scheduledInterval = null;
        if (isDynamic && chkSchedule.Checked)
        {
            // Get scheduled interval for dynamic newsletter
            scheduledInterval = schedulerInterval.ScheduleInterval;
        }

        string errorMessage = ValidateNewsletterValues();
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return;
        }

        NewsletterInfo newsletterObj = NewsletterInfo.Provider.Get(txtNewsletterName.Text.Trim(), SiteContext.CurrentSiteID);

        // Newsletter's code name must be unique
        if (newsletterObj != null && newsletterObj.NewsletterID != EditedNewsletter.NewsletterID)
        {
            ShowError(GetString("Newsletter_Edit.NewsletterNameExists"));
            return;
        }

        if (newsletterObj == null)
        {
            newsletterObj = NewsletterInfo.Provider.Get(EditedNewsletter.NewsletterID);
        }

        SetNewsletterValues(newsletterObj);

        // Check if subscription template was selected
        int subscriptionTemplateValue = ValidationHelper.GetInteger(subscriptionTemplate.Value, 0);
        if (EditedFeedIsNewsletter() && subscriptionTemplateValue == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoSubscriptionTemplateSelected"));
            return;
        }
        newsletterObj.NewsletterSubscriptionTemplateID = subscriptionTemplateValue;

        // Check if double opt-in template was selected
        if (chkEnableOptIn.Checked)
        {
            int optInTemplateValue = ValidationHelper.GetInteger(optInSelector.Value, 0);
            if (optInTemplateValue == 0)
            {
                ShowError(GetString("Newsletter_Edit.NoOptInTemplateSelected"));
                return;
            }
            newsletterObj.NewsletterOptInTemplateID = optInTemplateValue;
        }
        else
        {
            newsletterObj.NewsletterOptInTemplateID = 0;
        }

        // Check if unsubscription template was selected
        int unsubscriptionTemplateValue = ValidationHelper.GetInteger(unsubscriptionTemplate.Value, 0);
        if (unsubscriptionTemplateValue == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoUnsubscriptionTemplateSelected"));
            return;
        }
        newsletterObj.NewsletterUnsubscriptionTemplateID = unsubscriptionTemplateValue;

        // ID of scheduled task which should be deleted
        int deleteScheduledTaskId = 0;

        if (isDynamic)
        {
            newsletterObj.NewsletterSource = NewsletterSource.Dynamic;
            newsletterObj.NewsletterDynamicURL = txtNewsletterDynamicURL.Value.ToString();
            newsletterObj.NewsletterDynamicSubject = radFollowing.Checked ? txtSubject.Text : string.Empty;

            if ((String.IsNullOrEmpty(txtNewsletterDynamicURL.Value.ToString())))
            {
                // Dynamic URL cannot be empty
                ShowError(GetString("newsletter_edit.sourcepageurlempty"));
                return;
            }

            if (chkSchedule.Checked)
            {
                if (!schedulerInterval.CheckOneDayMinimum())
                {
                    // If problem occurred while setting schedule interval
                    ShowError(GetString("Newsletter_Edit.NoDaySelected"));
                    return;
                }

                TaskInterval taskInterval = SchedulingHelper.DecodeInterval(scheduledInterval);
                if (!DataTypeManager.IsValidDate(taskInterval.StartTime))
                {
                    ShowError(GetString("Newsletter.IncorrectDate"));
                    return;
                }

                UpdateDynamicNewsletterTask(newsletterObj, taskInterval);
            }
            else
            {
                if (newsletterObj.NewsletterDynamicScheduledTaskID > 0)
                {
                    // Store task ID for deletion
                    deleteScheduledTaskId = newsletterObj.NewsletterDynamicScheduledTaskID;
                }
                newsletterObj.NewsletterDynamicScheduledTaskID = 0;
                schedulerInterval.Visible = false;
            }
        }
        else
        {
            newsletterObj.NewsletterSource = NewsletterSource.TemplateBased;

            // Check if at least one template is selected
            if (string.IsNullOrEmpty(ValidationHelper.GetString(usTemplates.Value, null)))
            {
                ShowError(GetString("Newsletter_Edit.NoEmailTemplateSelected"));
                usTemplates.Value = mCurrentTemplates;
                return;
            }
            SaveTemplates();          
        }

        // Save changes to DB
        NewsletterInfo.Provider.Set(newsletterObj);
        if (deleteScheduledTaskId > 0)
        {
            // Delete scheduled task if schedule mail-outs were unchecked
            TaskInfo.Provider.Get(deleteScheduledTaskId)?.Delete();
        }

        ShowChangesSaved();

        // Update breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, newsletterObj.NewsletterDisplayName);
    }


    /// <summary>
    /// Load templates.
    /// </summary>
    private void LoadTemplates()
    {
        GetCurrentTemplates();
        usTemplates.Value = mCurrentTemplates;
        usTemplates.Reload(true);
    }


    /// <summary>
    /// Loads current templates from DB.
    /// </summary>
    private void GetCurrentTemplates()
    {
        var templateNewsletters = EmailTemplateNewsletterInfo.Provider
                                    .Get()
                                    .WhereEquals("NewsletterID", EditedNewsletter.NewsletterID)
                                    .Column("TemplateID")
                                    .Select(template => template.TemplateID);

        mCurrentTemplates = TextHelper.Join(";", templateNewsletters);
    }


    /// <summary>
    /// Save templates.
    /// </summary>
    private void SaveTemplates()
    {
        if (RequestHelper.IsPostBack())
        {
            GetCurrentTemplates();
        }

        var newTemplatesString = ValidationHelper.GetString(usTemplates.Value, null);

        RemoveOldTemplates(newTemplatesString, mCurrentTemplates);
        AddNewTemplates(newTemplatesString, mCurrentTemplates);
        mCurrentTemplates = newTemplatesString;
    }


    /// <summary>
    /// Remove templates from newsletter.
    /// </summary>
    private void RemoveOldTemplates(string newValues, string currentRecords)
    {
        var items = DataHelper.GetNewItemsInList(newValues, currentRecords);

        if (String.IsNullOrEmpty(items))
        {
            return;
        }

        var modifiedItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in modifiedItems)
        {
            EmailTemplateNewsletterInfo.Provider.Remove(ValidationHelper.GetInteger(item, 0), EditedNewsletter.NewsletterID);
        }
    }


    /// <summary>
    /// Add templates to newsletter.
    /// </summary>
    private void AddNewTemplates(string newValues, string currentRecords)
    {
        var items = DataHelper.GetNewItemsInList(currentRecords, newValues);

        if (String.IsNullOrEmpty(items))
        {
            return;
        }

        var modifiedItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in modifiedItems)
        {
            EmailTemplateNewsletterInfo.Provider.Add(ValidationHelper.GetInteger(item, 0), EditedNewsletter.NewsletterID);
        }
    }


    private void UpdateDynamicNewsletterTask(NewsletterInfo newsletterObj, TaskInterval taskInterval)
    {
        var currentTask = TaskInfo.Provider.Get(newsletterObj.NewsletterDynamicScheduledTaskID);
        var taskIntervalStr = SchedulingHelper.EncodeInterval(taskInterval);

        if ((currentTask != null) && (currentTask.TaskInterval == taskIntervalStr))
        {
            // No need to update anything, nothing has changed
            return;
        }

        // Update or create new task based on current taskInterval
        var task = NewsletterTasksManager.CreateOrUpdateDynamicNewsletterTask(newsletterObj, taskInterval, currentTask);
        TaskInfo.Provider.Set(task);

        // Update taskID
        newsletterObj.NewsletterDynamicScheduledTaskID = task.TaskID;
    }


    private bool EditedFeedIsEmailCampaign()
    {
        return EditedNewsletter.NewsletterType == EmailCommunicationTypeEnum.EmailCampaign;
    }


    private bool EditedFeedIsNewsletter()
    {
        return EditedNewsletter.NewsletterType == EmailCommunicationTypeEnum.Newsletter;
    }


    protected void radSubject_CheckedChanged(object sender, EventArgs e)
    {
        txtSubject.Enabled = radFollowing.Checked;
    }


    protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
    {
        // Show/hide scheduler
        schedulerInterval.Visible = chkSchedule.Checked;
    }


    protected void chkEnableOptIn_CheckedChanged(object sender, EventArgs e)
    {
        // Show/hide double opt-in options
        plcOptIn.Visible = chkEnableOptIn.Checked;
    }


    /// <summary>
    /// Initializes config form.
    /// </summary>
    /// <param name="newsletter">Newsletter object</param>
    private void GetNewsletterValues(NewsletterInfo newsletter)
    {
        txtNewsletterDisplayName.Text = newsletter.NewsletterDisplayName;
        txtNewsletterName.Text = newsletter.NewsletterName;
        txtNewsletterSenderName.Text = newsletter.NewsletterSenderName;
        txtNewsletterSenderEmail.Text = newsletter.NewsletterSenderEmail;
        txtNewsletterBaseUrl.Text = newsletter.NewsletterBaseUrl;
        txtNewsletterUnsubscribeUrl.Text = newsletter.NewsletterUnsubscribeUrl;
        txtDraftEmails.Text = newsletter.NewsletterDraftEmails;

        subscriptionTemplate.Value = newsletter.NewsletterSubscriptionTemplateID.ToString();
        unsubscriptionTemplate.Value = newsletter.NewsletterUnsubscriptionTemplateID.ToString();

        if (TrackingEnabled)
        {
            chkTrackOpenedEmails.Checked = newsletter.NewsletterTrackOpenEmails;
            chkTrackClickedLinks.Checked = newsletter.NewsletterTrackClickedLinks;
        }

        if (OnlineMarketingEnabled)
        {
            chkLogActivity.Checked = newsletter.NewsletterLogActivity;
        }

        chkEnableOptIn.Checked = plcOptIn.Visible = newsletter.NewsletterEnableOptIn;
        optInSelector.Value = newsletter.NewsletterOptInTemplateID;
        txtOptInURL.Text = newsletter.NewsletterOptInApprovalURL;
        chkSendOptInConfirmation.Checked = newsletter.NewsletterSendOptInConfirmation;
    }


    /// <summary>
    /// Validates newsletter config form.
    /// </summary>
    /// <returns>Returns error message in case of an error</returns>
    private string ValidateNewsletterValues()
    {
        var validator = new Validator()
                        .NotEmpty(txtNewsletterDisplayName.Text, GetString("general.requiresdisplayname"))
                        .NotEmpty(txtNewsletterName.Text, GetString("Newsletter_Edit.ErrorEmptyName"))
                        .NotEmpty(txtNewsletterSenderName.Text, GetString("Newsletter_Edit.ErrorEmptySenderName"))
                        .NotEmpty(txtNewsletterSenderEmail.Text, GetString("Newsletter_Edit.ErrorEmptySenderEmail"))
                        .MatchesCondition(txtNewsletterSenderEmail, input => input.IsValid(), GetString("Newsletter_Edit.ErrorEmailFormat"))
                        .IsCodeName(txtNewsletterName.Text, GetString("general.invalidcodename"))
                        .MatchesCondition(txtDraftEmails, input => input.IsValid(), GetString("EmailInput.ValidationError"));

        if (!isDynamic)
        {
            validator = validator.NotEmpty(usTemplates.Text, GetString("newsletter_edit.noemailtemplateselected"));
        }

        return validator.Result;
    }


    /// <summary>
    /// Sets newsletter object from config form data.
    /// </summary>
    /// <param name="newsletterObj">Newsletter object</param>
    private void SetNewsletterValues(NewsletterInfo newsletterObj)
    {
        newsletterObj.NewsletterDisplayName = txtNewsletterDisplayName.Text.Trim();
        newsletterObj.NewsletterName = txtNewsletterName.Text.Trim();
        newsletterObj.NewsletterSenderName = txtNewsletterSenderName.Text.Trim();
        newsletterObj.NewsletterSenderEmail = txtNewsletterSenderEmail.Text.Trim();
        newsletterObj.NewsletterBaseUrl = txtNewsletterBaseUrl.Text.Trim();
        newsletterObj.NewsletterUnsubscribeUrl = txtNewsletterUnsubscribeUrl.Text.Trim();
        newsletterObj.NewsletterDraftEmails = txtDraftEmails.Text;
        newsletterObj.NewsletterTrackOpenEmails = TrackingEnabled && chkTrackOpenedEmails.Checked;
        newsletterObj.NewsletterTrackClickedLinks = TrackingEnabled && chkTrackClickedLinks.Checked;
        newsletterObj.NewsletterLogActivity = OnlineMarketingEnabled && chkLogActivity.Checked;
        newsletterObj.NewsletterEnableOptIn = chkEnableOptIn.Checked && (ValidationHelper.GetInteger(optInSelector.Value, 0) > 0);
        newsletterObj.NewsletterOptInApprovalURL = txtOptInURL.Text.Trim();
        newsletterObj.NewsletterSendOptInConfirmation = chkSendOptInConfirmation.Checked;
    }


    /// <summary>
    /// Initializes tooltips and help icons text.
    /// </summary>
    private void InitializeTooltips()
    {
        pnlNewsletterDisplayName.ToolTip = lblNewsletterDisplayName.ToolTip = GetString("newsletter_edit.newsletterdisplayname.description");
        pnlNewsletterName.ToolTip = lblNewsletterName.ToolTip = GetString("newsletter_edit.newslettername.description");
        pnlNewsletterSenderName.ToolTip = lblNewsletterSenderName.ToolTip = GetString("newsletter_edit.newslettersendername.description");
        pnlNewsletterSenderEmail.ToolTip = lblNewsletterSenderEmail.ToolTip = GetString("newsletter_edit.newslettersenderemail.description");
        pnlNewsletterDraftEmails.ToolTip = lblDraftEmails.ToolTip = GetString("newsletter_edit.newsletterdraftemails.description");
        pnlUsTemplates.ToolTip = lblUsTemplates.ToolTip = GetString("newsletter_edit.newslettertemplates.description");
        pnlNewsletterDynamicSubject.ToolTip = lblSubject.ToolTip = GetString("newsletter_edit.newsletterdynamicsubject.description");
        pnlNewsletterDynamicUrl.ToolTip = lblNewsletterDynamicURL.ToolTip = GetString("newsletter_edit.newsletterdynamicurl.description");
        pnlNewsletterDynamicScheduler.ToolTip = lblSchedule.ToolTip = GetString("newsletter_edit.newsletterdynamicscheduledtask.description");
        pnlNewsletterTrackOpenedEmails.ToolTip = lblTrackOpenedEmails.ToolTip = GetString("newsletter_edit.newslettertrackopenedemails.description");
        pnlNewsletterTrackClickedLinks.ToolTip = lblTrackClickedLinks.ToolTip = GetString("newsletter_edit.newslettertrackclickedlinks.description");
        pnlNewsletterLogActivities.ToolTip = lblLogActivity.ToolTip = GetString("newsletter_edit.newsletterlogactivities.description");
        pnlNewsletterOptInTemplate.ToolTip = lblOptInTemplate.ToolTip = GetString("newsletter_edit.newsletteroptintemplate.description");
        pnlNewsletterOptInApprovalUrl.ToolTip = lblOptInURL.ToolTip = GetString("newsletter_edit.newsletteroptinurl.description");
        pnlNewsletterOptInSendConfirmation.ToolTip = lblSendOptInConfirmation.ToolTip = GetString("newsletter_edit.newsletteroptinsendconfirmation.description");

        pnlNewsletterSubscriptionTemplate.ToolTip = lblSubscriptionTemplate.ToolTip = iconHelpSubscriptionTemplate.ToolTip = lblScreenReaderSubscriptionTemplate.Text = GetString("newsletter_edit.newslettersubscriptiontemplate.description");
        pnlNewsletterUnsubscriptionTemplate.ToolTip = lblUnsubscriptionTemplate.ToolTip = iconHelpUnsubscriptionTemplate.ToolTip = lblScreenReaderUnsubscriptionTemplate.Text = GetString("newsletter_edit.newsletterunsubscriptiontemplate.description");
        pnlNewsletterBaseUrl.ToolTip = lblNewsletterBaseUrl.ToolTip = iconHelpBaseUrl.ToolTip = lblScreenReaderBaseUrl.Text = GetString("newsletter_edit.newsletterbaseurl.description");
        pnlNewsletterUnsubscriptionUrl.ToolTip = lblNewsletterUnsubscribeUrl.ToolTip = iconHelpUnsubscribeUrl.ToolTip = lblScreenReaderUnsubscribeUrl.Text = GetString("newsletter_edit.newsletterunsubscribeurl.description");
        pnlNewsletterEnableOptIn.ToolTip = lblEnableOptIn.ToolTip = iconHelpEnableOptIn.ToolTip = lblScreenReaderEnableOptIn.Text = GetString("newsletter_edit.newsletterenableoptin.description");
        pnlNewsletterOptInApprovalUrl.ToolTip = lblOptInURL.ToolTip = iconHelpOptInURL.ToolTip = lblScreenReaderOptInURL.Text = GetString("newsletter_edit.newsletteroptinurl.description");

        ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");
    }
}
