using System;

using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;


[EditedObject("om.account", "objectid")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "account.subsidiaries")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_Subsidiaries : CMSContactManagementPage
{
    /// <summary>
    /// Hides close button
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