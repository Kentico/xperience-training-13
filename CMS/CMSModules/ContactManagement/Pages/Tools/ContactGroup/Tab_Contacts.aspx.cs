using System;

using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;


[EditedObject(ContactGroupInfo.OBJECT_TYPE, "groupId")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "ContactGroupContacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_Tab_Contacts : CMSContactManagementPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;
    }
}