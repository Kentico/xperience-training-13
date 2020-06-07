using System;

using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Process_Detail : CMSAutomationPage, IProcessDetailPage
{
    public bool IsContactDetailShown => true;


    public CMSAutomationManager AutomationManager { get;  set; }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        AutomationManager.AutomationInfoLabel.Visible = false;
    }


    public void SetBreadcrumbs()
    {
        SetBreadcrumb(0, GetString("ma.contact.contacts"), GetListingPageUrl(), null, null);
        SetBreadcrumb(1, HTMLHelper.HTMLEncode(ContactInfoProvider.GetContactFullName(AutomationManager.ObjectID)), null, null, null);
    }


    public void AfterAutomationManagerAction(string actionName)
    {
        switch (actionName)
        {
            case ComponentEvents.AUTOMATION_REMOVE:
            case ComponentEvents.AUTOMATION_START:
                URLHelper.Redirect(GetListingPageUrl());
                break;
        }
    }


    public string GetListingPageUrl()
    {
        return ResolveUrl(string.Format("~/CMSModules/ContactManagement/Pages/Tools/Automation/Process/Tab_Contacts.aspx?objectid={0}&processid={0}", AutomationManager.Process?.WorkflowID));
    }
}
