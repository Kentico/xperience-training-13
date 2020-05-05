using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "MyContacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_MyPendingContacts_Process_Details : CMSContentManagementPage, IProcessDetailPage
{
    public void SetBreadcrumbs(CMSAutomationManager automationManager)
    {
        string bcUrl = null;
        if (QueryHelper.GetInteger("dialogmode", 0) != 1)
        {
            bcUrl = GetListingUrl();
        }

        SetBreadcrumb(0, GetString("ma.contact.pendingcontacts"), bcUrl, null, null);
        SetBreadcrumb(1, HTMLHelper.HTMLEncode(ContactInfoProvider.GetContactFullName(automationManager.ObjectID)), null, null, null);
    }


    public void AfterAutomationManagerAction(string actionName, CMSAutomationManager automationManager)
    {
        switch (actionName)
        {
            case ComponentEvents.AUTOMATION_REMOVE:
            case ComponentEvents.AUTOMATION_START:
                URLHelper.Redirect(GetListingUrl());
                break;
        }
    }


    private string GetListingUrl()
    {
        return ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/PendingContacts/MyPendingContacts/PendingContacts.aspx");
    }
}
