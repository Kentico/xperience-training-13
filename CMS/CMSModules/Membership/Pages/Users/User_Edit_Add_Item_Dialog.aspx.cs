using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_User_Edit_Add_Item_Dialog : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        String identifier = QueryHelper.GetString("params", null);
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

        if (parameters != null)
        {
            // Take only first column
            parameters["AdditionalColumns"] = null;
        }

        base.OnPreInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Read"))
        {
            RedirectToAccessDenied("CMS.Users", "Read");
        }

        // Try to get parameters...
        string identifier = QueryHelper.GetString("params", null);
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

        // ... and validate hash
        if ((QueryHelper.ValidateHash("hash", "selectedvalue")) && (parameters != null))
        {
            selectionDialog.LocalizeItems = QueryHelper.GetBoolean("localize", true);

            // Load resource prefix
            string resourcePrefix = ValidationHelper.GetString(parameters["ResourcePrefix"], "general");

            // Set the page title
            string titleText = GetString(resourcePrefix + ".selectitem|general.selectitem");

            // Validity group text
            pnlDateTimeHeading.ResourceString = resourcePrefix + ".bindingproperties";

            PageTitle.TitleText = titleText;
            Page.Title = titleText;

            // Remove additional dialog padding
            CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
        }
        else
        {
            // Redirect to error page
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
        }

        ((ICMSModalMasterPage)CurrentMaster).ShowSaveAndCloseButton();
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Show/hide send notification field
        pnlSendNotification.Visible = QueryHelper.GetBoolean("UseSendNotification", false);

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);

        base.OnPreRender(e);
    }


    protected override void OnPreRenderComplete(EventArgs e)
    {
        base.OnPreRenderComplete(e);

        String okScript;

        if (selectionDialog.UniGrid.IsEmpty)
        {
            pnlDateTime.Visible = false;
            okScript = "return US_Cancel()";
        }
        else
        {
            string script = $@"
function okScript() {{
    if ((typeof(isDateTimeValid) != 'undefined') && (!isDateTimeValid('{ucDateTime.DateTimeTextBox.ClientID}'))) {{
        var lblErr = $cmsj('#{lblError.ClientID}');
        lblErr.text ('{GetString("basicform.errorinvaliddatetimerange")}');
        lblErr.show();
    }} else {{
        var date = $cmsj('#{ucDateTime.DateTimeTextBox.ClientID}').val();
        var sendNotification = $cmsj('#{chkSendNotification.ClientID}').prop('checked');
        if (wopener.setNewDateTime != null) {{
            wopener.setNewDateTime(date);
        }}
        if (wopener.setNewSendNotification != null) {{
            wopener.setNewSendNotification(sendNotification);
        }}
        US_Submit();
    }}
    return false;
}}";

            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "okScript", ScriptHelper.GetScript(script));

            okScript = "return okScript();";
        }

        SetSaveJavascript(okScript);
    }
}
