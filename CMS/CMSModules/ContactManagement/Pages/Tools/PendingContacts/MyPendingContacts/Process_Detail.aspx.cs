using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "MyContacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_MyPendingContacts_Process_Details : CMSContentManagementPage, IProcessDetailPage
{
    public bool IsContactDetailShown => true;


    public CMSAutomationManager AutomationManager { get; set; }


    public void SetBreadcrumbs()
    {
        string bcUrl = null;
        if (QueryHelper.GetInteger("dialogmode", 0) != 1)
        {
            bcUrl = GetListingPageUrl();
        }

        SetBreadcrumb(0, GetString("ma.contact.pendingcontacts"), bcUrl, null, null);
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
        return ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/PendingContacts/MyPendingContacts/PendingContacts.aspx");
    }
}
