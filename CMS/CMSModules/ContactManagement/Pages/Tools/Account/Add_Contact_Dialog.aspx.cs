using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_Pages_Tools_Account_Add_Contact_Dialog : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure, that it is going to be rendered
        pnlRole.Visible = true;

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);

        // Try to get parameters
        string identifier = QueryHelper.GetString("params", null);
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

        // Validate hash
        if ((QueryHelper.ValidateHash("hash", "selectedvalue")) && (parameters != null))
        {
            // Check permissions
            AuthorizationHelper.AuthorizedReadContact(true);
            if (AuthorizationHelper.AuthorizedModifyContact(false))
            {
                contactRoleSelector.IsLiveSite = false;
                contactRoleSelector.UniSelector.DialogWindowName = "SelectContactRole";

                selectionDialog.LocalizeItems = QueryHelper.GetBoolean("localize", true);

                // Load resource prefix
                string resourcePrefix = ValidationHelper.GetString(parameters["ResourcePrefix"], "general");

                // Set the page title
                string titleText = GetString(resourcePrefix + ".selectitem|general.selectitem");

                // Validity group text
                lblAddAccounts.ResourceString = resourcePrefix + ".contactsrole";
                pnlRoleHeading.Visible = true;

                PageTitle.TitleText = titleText;
                Page.Title = titleText;
            }
            // No permission modify
            else
            {
                RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "ModifyAccount");
            }
        }
        else
        {
            // Redirect to error page
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
        }

        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
    }


    protected override void OnPreRender(EventArgs e)
    {
        SetSaveJavascript(@"var role = $cmsj('#" + contactRoleSelector.DropDownList.ClientID + @"').val();
    if (wopener.setRole) wopener.setRole(role);
    return US_Submit();
");
        base.OnPreRender(e);
    }


    protected override void OnPreRenderComplete(EventArgs e)
    {
        pnlRole.Visible = !selectionDialog.UniGrid.IsEmpty;

        base.OnPreRenderComplete(e);
    }
}
