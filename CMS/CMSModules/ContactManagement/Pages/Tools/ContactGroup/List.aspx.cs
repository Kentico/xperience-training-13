using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.UIControls;


[Title("om.contactgroup.list")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "ContactGroups")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_List : CMSContactManagementPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        hdrActions.AddAction(new HeaderAction
        {
            Text = GetString("om.contactgroup.new"),
            RedirectUrl = ResolveUrl(UIContextHelper.GetElementUrl(ModuleName.CONTACTMANAGEMENT, "NewContactGroup") + "&displaytitle=false")
        });

        // Register script for UniMenu button selection
        AddMenuButtonSelectScript(this, "ContactGroups", null, "menu");
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