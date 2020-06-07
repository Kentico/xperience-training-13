using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[Title("ma.ui.detail")]
[UIElement(ModuleName.ONLINEMARKETING, "PendingContacts", false, true)]
public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_Process_Detail : CMSContactManagementPage, IProcessDetailPage
{
    public bool IsContactDetailShown => true;


    public CMSAutomationManager AutomationManager { get; set; }


    public void SetBreadcrumbs()
    {
        string bcUrl = null;

        if (!IsDialog)
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
                if (IsDialog)
                {
                    ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "closePCWidgetDialog", ScriptHelper.GetScript("window.refreshPageOnClose = true; CloseDialog();"));
                }
                else
                {
                    URLHelper.Redirect(GetListingPageUrl());
                }
                break;
        }
    }


    public string GetListingPageUrl()
    {
        return ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/PendingContacts/List.aspx?siteid=" + SiteID);
    }
}
