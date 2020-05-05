using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.UIControls;

[UIElement(ModuleName.ONLINEMARKETING, "MyContacts")]
[Security(ModuleName.CONTACTMANAGEMENT, "Read", "")]
public partial class CMSModules_ContactManagement_Pages_Tools_PendingContacts_MyPendingContacts_PendingContacts : CMSContentManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Add Refresh button
        PageTitle.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("general.Refresh"),
            RedirectUrl = "PendingContacts.aspx"
        });
    }
}
