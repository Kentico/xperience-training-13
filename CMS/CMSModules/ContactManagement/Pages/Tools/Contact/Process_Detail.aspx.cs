using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "ContactProcesses")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Process_Detail : CMSContactManagementPage, IProcessDetailPage
{
    public bool IsContactDetailShown => false;


    public CMSAutomationManager AutomationManager { get; set; }


    public void SetBreadcrumbs()
    {
        SetBreadcrumb(0, GetString("ma.contact.processes"), GetListingPageUrl(), null, null);
        SetBreadcrumb(1, HTMLHelper.HTMLEncode(AutomationManager.Process?.WorkflowDisplayName), null, null, null);
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
        return ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/Contact/Tab_Processes.aspx?objectid=" + AutomationManager.ObjectID);
    }
}
