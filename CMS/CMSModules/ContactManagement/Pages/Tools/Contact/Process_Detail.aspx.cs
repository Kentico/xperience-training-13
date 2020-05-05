using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "ContactProcesses")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Process_Detail : CMSContactManagementPage, IProcessDetailPage
{
    public void SetBreadcrumbs(CMSAutomationManager automationManager)
    {
        SetBreadcrumb(0, GetString("ma.contact.processes"), GetListingUrl(automationManager), null, null);
        SetBreadcrumb(1, HTMLHelper.HTMLEncode(automationManager.Process?.WorkflowDisplayName), null, null, null);
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
        return ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/Contact/Tab_Processes.aspx?objectid=" + automationManager.ObjectID);
    }
}
