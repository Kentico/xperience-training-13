using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.UIControls;

[UIElement(ModuleName.CONTACTMANAGEMENT, "Contacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_List : CMSContactManagementPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set header actions (add button)
        string url = ResolveUrl("New.aspx");

        hdrActions.AddAction(new HeaderAction
        {
            Text = GetString("om.contact.importcsv"),
            RedirectUrl = ResolveUrl("ImportCSV.aspx"),
            ButtonStyle = ButtonStyle.Primary
        });

        hdrActions.AddAction(new HeaderAction
            {
                Text = GetString("om.contact.new"),
                RedirectUrl = url,
                ButtonStyle = ButtonStyle.Default
        });

        var deletingInactiveContactsMethod = SettingsKeyInfoProvider.GetValue("CMSDeleteInactiveContactsMethod");
        if (string.IsNullOrEmpty(deletingInactiveContactsMethod) && LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.FullContactManagement))
        {
            ShowWarning(string.Format(GetString("om.contactlist.inactivecontacts.warning"), DocumentationHelper.GetDocumentationTopicUrl("contacts_automatic_deletion")));               
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        hdrActions.Enabled = AuthorizationHelper.AuthorizedModifyContact(false);
    }

    #endregion
}