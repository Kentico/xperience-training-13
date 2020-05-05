using System;

using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;

[EditedObject(ContactInfo.OBJECT_TYPE, "contactId")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "ContactGeneral")]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_General : CMSContactManagementPage
{
    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }
}