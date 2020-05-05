using System;

using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;


[EditedObject(AccountInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "account.general")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_General : CMSContactManagementPage
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