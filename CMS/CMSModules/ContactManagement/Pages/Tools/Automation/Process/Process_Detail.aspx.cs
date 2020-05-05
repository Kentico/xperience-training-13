using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Process_Detail : CMSAutomationPage, IProcessDetailPage
{
    public void SetBreadcrumbs(CMSAutomationManager automationManager)
    {
        SetBreadcrumb(0, GetString("ma.contact.contacts"), GetListingUrl(automationManager), null, null);
        SetBreadcrumb(1, HTMLHelper.HTMLEncode(ContactInfoProvider.GetContactFullName(automationManager.ObjectID)), null, null, null);
    }


    public void AfterAutomationManagerAction(string actionName, CMSAutomationManager automationManager)
    {
        switch (actionName)
        {
            case ComponentEvents.AUTOMATION_REMOVE:
            case ComponentEvents.AUTOMATION_START:
                URLHelper.Redirect(GetListingUrl(automationManager));
                break;
        }
    }


    private string GetListingUrl(CMSAutomationManager automationManager)
    {
        return ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/Automation/Process/Tab_Contacts.aspx?processid=" + automationManager.Process?.WorkflowID);
    }
}
