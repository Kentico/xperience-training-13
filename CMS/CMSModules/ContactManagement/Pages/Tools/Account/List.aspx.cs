using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;


[UIElement(ModuleName.CONTACTMANAGEMENT, "Accounts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_List : CMSContactManagementPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set header actions (add button)
        string url = ResolveUrl("New.aspx");
        hdrActions.AddAction(new HeaderAction
            {
                Text = GetString("om.account.new"),
                RedirectUrl = url
            });
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disable actions for unauthorized users
        if (!AuthorizationHelper.AuthorizedModifyContact(false))
        {
            hdrActions.Enabled = false;
        }
    }

    #endregion
}