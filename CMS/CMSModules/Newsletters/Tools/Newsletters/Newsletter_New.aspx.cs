using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.Scheduler;
using CMS.SiteProvider;
using CMS.UIControls;

[Breadcrumbs]
[Breadcrumb(0, ResourceString = "Newsletter_Edit.ItemListLink", TargetUrl = "~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_List.aspx")]
[Breadcrumb(1, ResourceString = "Newsletter_Edit.NewItemCaption")]
[EditedObject("newsletter.newsletter", "objectid")]
[Title("newsletters.newsletters")]
[UIElement("CMS.Newsletter", "AddANewNewsletter")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_New : CMSNewsletterPage
{
    protected NewsletterInfo TypedEditedObject
    {
        get
        {
            return EditedObject as NewsletterInfo;
        }
        set
        {
            EditedObject = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            SetDefaultValues();
        }

        // Hide detailed report as it could confuse non-technical users
        NewForm.FieldControls["NewsletterDynamicURL"].SetValue("ShowDetailedError", false);
        NewForm.FieldControls["NewsletterDynamicURL"].SetValue("StatusErrorMessage", GetString("general.pagenotfound"));

        ShowCustomRequiredMarks();
        SetFieldsVisibilityAccordingToType();
        HideTypeSelectionForNonEMSLicenses();
    }


    private void HideTypeSelectionForNonEMSLicenses()
    {
        var license = LicenseKeyInfoProvider.GetLicenseKeyInfo(RequestContext.CurrentDomain);
        if (license.Edition != ProductEditionEnum.EnterpriseMarketingSolution)
        {
            NewForm.FieldsToHide.Add("NewsletterType");
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        SetVisibility();
    }


    protected void BeforeSave(object sender, EventArgs e)
    {
        // Set site ID
        NewForm.Data.SetValue("NewsletterSiteID", SiteContext.CurrentSiteID);

        // Clear other possibilities
        if (GetUpperNewsletterSource() != NewsletterSource.Dynamic)
        {
            NewForm.Data.SetValue("NewsletterDynamicURL", null);
            NewForm.Data.SetValue("NewsletterDynamicScheduledTaskID", null);
        }

        if (IsCampaignEmailType())
        {
            ClearSubscriptionTemplateID();
        }
    }


    private void ClearSubscriptionTemplateID()
    {
        NewForm.Data.SetValue("NewsletterSubscriptionTemplateID", null);
    }


    protected void ValidateValues(object sender, EventArgs e)
    {
        // Validate unsubscription template
        if (ValidationHelper.GetInteger(NewForm.GetFieldValue("NewsletterUnsubscriptionTemplateID"), 0) == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoUnsubscriptionTemplateSelected"));
            NewForm.StopProcessing = true;
        }

        // Validate subscription template
        if (IsNewsletterType() && ValidationHelper.GetInteger(NewForm.GetFieldValue("NewsletterSubscriptionTemplateID"), 0) == 0)
        {
            ShowError(GetString("Newsletter_Edit.NoSubscriptionTemplateSelected"));
            NewForm.StopProcessing = true;
        }

        // If Dynamic, validate schedule interval and Source page URL
        if (GetUpperNewsletterSource() == NewsletterSource.Dynamic)
        {
            if (ValidationHelper.GetString(NewForm.GetFieldValue("NewsletterDynamicURL"), string.Empty) == string.Empty)
            {
                // Source page URL can not be empty
                ShowError(GetString("newsletter_edit.sourcepageurlempty"));
                NewForm.StopProcessing = true;
            }

            if (chkSchedule.Checked)
            {
                if (!ScheduleInterval.CheckOneDayMinimum())
                {
                    // Problem occurred while setting schedule interval for dynamic newsletter
                    ShowError(GetString("Newsletter_Edit.NoDaySelected"));
                    NewForm.StopProcessing = true;
                }

                if (SchedulingHelper.DecodeInterval(ScheduleInterval.ScheduleInterval).StartTime == DateTime.MinValue)
                {
                    ShowError(GetString("Newsletter.IncorrectDate"));
                    NewForm.StopProcessing = true;
                }
            }
        }
    }


    protected void AfterSave(object sender, EventArgs e)
    {
        if (GetUpperNewsletterSource() != NewsletterSource.Dynamic)
        {
            var templateIDsString = ValidationHelper.GetString(NewForm.GetFieldValue("NewsletterTemplateIDs"), "");
            AddTemplatesToNewsletter(templateIDsString);
        }

        if ((GetUpperNewsletterSource() == NewsletterSource.Dynamic) && chkSchedule.Checked)
        {
            // If Scheduling is enabled, create task
            CreateTask();
        }
        else
        {
            // Redirect to newly created newsletter
            Redirect();
        }
    }

    private void AddTemplatesToNewsletter(string templateIDsString) {
        if (String.IsNullOrEmpty(templateIDsString))
        {
            return;
        }

        var templateIDs = templateIDsString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var templateID in templateIDs)
        {
            EmailTemplateNewsletterInfo.Provider.Add(ValidationHelper.GetInteger(templateID, 0), TypedEditedObject.NewsletterID);
        }
    }


    protected void chkSchedule_CheckedChanged(object sender, EventArgs e)
    {
        // Set visibility for schedule interval control
        ScheduleInterval.Visible = chkSchedule.Checked;
    }


    /// <summary>
    /// Sets visibility of controls.
    /// </summary>
    private void SetVisibility()
    {
        plcSchedule.Visible = GetSchedulerVisibility();
        ScheduleInterval.Visible = chkSchedule.Checked && plcSchedule.Visible;
    }


    /// <summary>
    /// Sets default values of controls.
    /// </summary>
    private void SetDefaultValues()
    {
        if (ScheduleInterval.StartTime.SelectedDateTime == DateTimeHelper.ZERO_TIME)
        {
            ScheduleInterval.StartTime.SelectedDateTime = DateTime.Now;
        }
    }


    /// <summary>
    /// Shows required field marks not handled automatically.
    /// </summary>
    private void ShowCustomRequiredMarks()
    {
        // Show required marks for fields visible only if the dynamic type is selected
        LocalizedLabel dynamicUrlLabel = NewForm.FieldLabels["NewsletterDynamicURL"];
        if (dynamicUrlLabel != null)
        {
            dynamicUrlLabel.ShowRequiredMark = true;
        }

        ShowRequiredMarkForSubscriptionTemplateIDField();
    }
    
    
    private void ShowRequiredMarkForSubscriptionTemplateIDField()
    {
        var subscriptionTemplateID = NewForm.FieldLabels["NewsletterSubscriptionTemplateID"];
        subscriptionTemplateID.ShowRequiredMark = true;
    }


    private void SetFieldsVisibilityAccordingToType()
    {
        if (IsCampaignEmailType())
        {
            NewForm.FieldsToHide.Add("NewsletterSubscriptionTemplateID");
        }
    }


    /// <summary>
    /// Returns newsletter source (upper).
    /// </summary>
    /// <returns>Newsletter source string</returns>
    private string GetUpperNewsletterSource()
    {
        return ValidationHelper.GetString(NewForm.GetFieldValue("NewsletterSource"), "").ToUpperInvariant();
    }


    private EmailCommunicationTypeEnum GetSelectedCommunicationType()
    {
        return (EmailCommunicationTypeEnum)ValidationHelper.GetInteger(NewForm.GetFieldValue("NewsletterType"), 0);
    }


    private bool IsNewsletterType()
    {
        return GetSelectedCommunicationType() == EmailCommunicationTypeEnum.Newsletter;
    }


    private bool IsCampaignEmailType()
    {
        return GetSelectedCommunicationType() == EmailCommunicationTypeEnum.EmailCampaign;
    }
    
    
    private bool GetSchedulerVisibility()
    {
        return  IsNewsletterType() && IsDynamicNewsletter();
    }


    private bool IsDynamicNewsletter()
    {
        return GetUpperNewsletterSource() == NewsletterSource.Dynamic;
    }


    /// <summary>
    /// Redirects to newly created newsletter.
    /// </summary>
    private void Redirect()
    {
        if (TypedEditedObject != null)
        {
            string url = UIContextHelper.GetElementUrl("cms.newsletter", "EditNewsletterProperties", false);
            url = URLHelper.AddParameterToUrl(url, "objectid", Convert.ToString(TypedEditedObject.NewsletterID));
            url = URLHelper.AddParameterToUrl(url, "tabindex", "1");
            url = URLHelper.AddParameterToUrl(url, "saved", "1");
            URLHelper.Redirect(url);
        }
    }


    /// <summary>
    /// Create schedule task.
    /// </summary>
    private void CreateTask()
    {
        try
        {
            var newsletter = TypedEditedObject;

            var task = NewsletterTasksManager.CreateOrUpdateDynamicNewsletterTask(newsletter, ScheduleInterval.TaskInterval);
            TaskInfo.Provider.Set(task);

            newsletter.NewsletterDynamicScheduledTaskID = task.TaskID;
            NewsletterInfo.Provider.Set(newsletter);

            Redirect();
        }
        catch (Exception ex)
        {
            ShowError(GetString(ex.Message));
        }
    }
}