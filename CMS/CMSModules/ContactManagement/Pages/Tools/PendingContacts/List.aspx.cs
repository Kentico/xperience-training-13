using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;
using CMS.SiteProvider;

[UIElement(ModuleName.ONLINEMARKETING, "PendingContacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_List : CMSAutomationPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AuthorizedForContacts)
        {
            // User has no permissions
            RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Read");
        }

        // Add Refresh action button
        AddHeaderAction(new HeaderAction()
        {
            Text = GetString("general.Refresh"),
            RedirectUrl = "List.aspx"
        });
    }
}
