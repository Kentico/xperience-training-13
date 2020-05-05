using System;

using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;


[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "ContactAccounts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Accounts : CMSContactManagementPage
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
        CurrentMaster.PanelContent.CssClass = String.Empty;
    }
}