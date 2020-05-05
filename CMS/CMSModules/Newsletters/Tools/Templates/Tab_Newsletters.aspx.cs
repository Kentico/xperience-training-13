using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.DataEngine;
using CMS.DataEngine.Query;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

// Set edited object
[EditedObject("newsletter.emailtemplate", "objectid")]
[Security(Resource = "CMS.Newsletter", UIElements = "Templates.Newsletters")]
[Security(Resource = "CMS.Newsletter", UIElements = "TemplateProperties")]
[UIElement("CMS.Newsletter", "Templates.Newsletters")]
public partial class CMSModules_Newsletters_Tools_Templates_Tab_Newsletters : CMSNewsletterPage
{
    private EmailTemplateInfo emailTemplateInfo;
    private string currentValues;


    protected void Page_Load(object sender, EventArgs e)
    {
        lblErrorMessage.Visible = false;

        emailTemplateInfo = EditedObject as EmailTemplateInfo;

        if (emailTemplateInfo == null)
        {
            pnlAvailability.Visible = false;
            return;
        }

        // Initialize newsletter selector
        var where = new WhereCondition()
            .WhereEquals("NewsletterSource", NewsletterSource.TemplateBased)
            .WhereEquals("NewsletterSiteID", SiteContext.CurrentSiteID);
        usNewsletters.WhereCondition = where.ToString(true);

        if (!RequestHelper.IsPostBack())
        {
            LoadNewsletterTemplateBindings();
        }

        usNewsletters.OnSelectionChanged += usNewsletters_OnSelectionChanged;
    }


    /// <summary>
    /// Uniselector event handler.
    /// </summary>
    protected void usNewsletters_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveNewsletterTemplateBindings();
    }


    /// <summary>
    /// Load control.
    /// </summary>
    private void LoadNewsletterTemplateBindings()
    {
        GetCurrentNewsletters();
        usNewsletters.Value = currentValues;
        usNewsletters.Reload(true);
    }


    /// <summary>
    /// Loads current newsletters from DB.
    /// </summary>
    private void GetCurrentNewsletters()
    {
        var templateNewsletters = EmailTemplateNewsletterInfo.Provider
            .Get()
            .WhereEquals("TemplateID", emailTemplateInfo.TemplateID)
            .Column("NewsletterID")
            .GetListResult<int>();

        currentValues = TextHelper.Join(";", templateNewsletters);
    }


    /// <summary>
    /// Save changes.
    /// </summary>
    private void SaveNewsletterTemplateBindings()
    {
        // Check 'Manage templates' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managetemplates"))
        {
            RedirectToAccessDenied("cms.newsletter", "managetemplates");
        }

        if (RequestHelper.IsPostBack())
        {
            GetCurrentNewsletters();
        }

        var newValues = ValidationHelper.GetString(usNewsletters.Value, null);
        RemoveOldRecords(newValues, currentValues);
        AddNewRecords(newValues, currentValues);

        LoadNewsletterTemplateBindings();
    }


    /// <summary>
    /// Remove newsletters from template.
    /// </summary>
    private void RemoveOldRecords(string newValues, string currentNewsletters)
    {
        var items = DataHelper.GetNewItemsInList(newValues, currentNewsletters);

        if (String.IsNullOrEmpty(items))
        {
            return;
        }

        var newsletterIds = GetNewsletterIds(items).ToList();

        var newsletterIdsToRemove = newsletterIds.Where(IsSafeToRemove).ToList();
        var irremovableNewsletterIds = newsletterIds.Except(newsletterIdsToRemove).ToList();
        
        if (irremovableNewsletterIds.Any())
        {
            AddErrorMessage(irremovableNewsletterIds);
        }
        
        RemoveTemplateBindings(newsletterIdsToRemove);
    }


    private void AddErrorMessage(ICollection<int> irremovableNewsletterIds)
    {
        var newsletters = NewsletterInfo.Provider.Get()
            .WhereIn("NewsletterID", irremovableNewsletterIds)
            .Columns("NewsletterDisplayName")
            .GetListResult<string>();

        lblErrorMessage.Text = string.Format(GetString("newsletter.templatenewsletter.lastbindingerror"), string.Join(", ", newsletters));
        lblErrorMessage.Visible = true;
    }


    private void RemoveTemplateBindings(IEnumerable<int> newsletterIds)
    {
        foreach (var newsletterId in newsletterIds)
        {
            EmailTemplateNewsletterInfo.Provider.Remove(emailTemplateInfo.TemplateID, newsletterId);
        }
    }


    private static bool IsSafeToRemove(int newsletterId)
    {
        var assignedTemplatesCount = EmailTemplateNewsletterInfo.Provider.Get()
                                                                        .WhereEquals("NewsletterID", newsletterId)
                                                                        .GetCount();

        return assignedTemplatesCount > 1;
    }

    
    /// <summary>
    /// Add newsletters to template.
    /// </summary>
    private void AddNewRecords(string newValues, string currentNewsletters)
    {
        var items = DataHelper.GetNewItemsInList(currentNewsletters, newValues);

        if (String.IsNullOrEmpty(items))
        {
            return;
        }

        var newsletterIds = GetNewsletterIds(items);
        AddTemplateBindings(newsletterIds);
    }


    private void AddTemplateBindings(IEnumerable<int> newsletterIds)
    {
        foreach (var newsletterId in newsletterIds)
        {
            EmailTemplateNewsletterInfo.Provider.Add(emailTemplateInfo.TemplateID, newsletterId);
        }
    }


    private static IEnumerable<int> GetNewsletterIds(string items)
    {
        var modifiedItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        return modifiedItems.Select(item => ValidationHelper.GetInteger(item, 0));
    }
}