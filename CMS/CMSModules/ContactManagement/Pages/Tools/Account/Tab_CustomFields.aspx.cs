using System;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;

// Edited object
[EditedObject(AccountInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.CONTACTMANAGEMENT, "account.customfields")]
public partial class CMSModules_ContactManagement_Pages_Tools_Account_Tab_CustomFields : CMSContactManagementPage
{
    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
        if (EditedObject != null)
        {
            // Get edited account
            AccountInfo ai = (AccountInfo)EditedObject;
            // Initialize dataform
            formCustomFields.Info = ai;
            formCustomFields.HideSystemFields = true;
            formCustomFields.AlternativeFormFullName = ai.TypeInfo.ObjectClassName;
            formCustomFields.OnCheckPermissions += formCustomFields_OnCheckPermissions;
            formCustomFields.OnAfterSave += formCustomFields_OnAfterSave;
        }
        else
        {
            // Disable dataform
            formCustomFields.Enabled = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (formCustomFields != null)
        {
            // Set submit button's css class
            formCustomFields.SubmitButton.ButtonStyle = ButtonStyle.Default;
        }
    }


    protected void formCustomFields_OnCheckPermissions(object sender, EventArgs e)
    {
        AuthorizationHelper.AuthorizedModifyContact(true);
    }


    protected void formCustomFields_OnAfterSave(object sender, EventArgs e)
    {
        ShowChangesSaved();
    }
}