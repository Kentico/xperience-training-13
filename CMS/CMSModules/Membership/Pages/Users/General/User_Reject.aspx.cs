using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_General_User_Reject : CMSUsersPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the master page header
        PageTitle.TitleText = GetString("administration.users.rejectusers");
        // Initialize other properties        
        txtReason.Text = GetString("administration.user.reasondefault");
        btnCancel.Attributes.Add("onclick", "return CloseDialog();");

        // Register scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CloseAndRefresh",
                                               ScriptHelper.GetScript(
                                                   "function CloseAndRefresh()\n" +
                                                   "{\n" +
                                                   "var txtReason = document.getElementById('" + txtReason.ClientID + "').value.substring(0, " + txtReason.MaxLength + ");\n" +
                                                   "var chkSendEmail = document.getElementById('" + chkSendEmail.ClientID + "').checked;\n" +
                                                   "wopener.SetRejectParam(txtReason, chkSendEmail, 'true');\n" +
                                                   "CloseDialog();\n" +
                                                   "}\n"));

        txtReason.Focus();

        btnReject.OnClientClick = "CloseAndRefresh(); return false;";

        // Register modal page scripts
        RegisterEscScript();
        RegisterModalPageScripts();
    }
}
