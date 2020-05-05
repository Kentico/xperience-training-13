using System;

using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;

[EditedObject("om.account", "objectid")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "account.contacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_Contacts : CMSContactManagementPage
{
    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";
    }
}