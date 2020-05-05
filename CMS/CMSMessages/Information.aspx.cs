using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSMessages_Information : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Title = "CMS - Information";

        // Initialize title
        hdnPermission.Text = PageTitle.TitleText = GetString("CMSDesk.Information");
        PageTitle.HideTitle = false;

        lblMessage.Text = GetMessageText();

        // Handle dialog
        CMSDialogHelper.RegisterDialogHelper(this);
        var script = String.Format("HandleAspxPageDialog('{0}');", CurrentMaster.PanelHeader.ClientID);
        ScriptHelper.RegisterStartupScript(this, typeof(string), "HandleAspxPageDialog", script, true);
    }


    /// <summary>
    /// Gets the text to be displayed as an information message. 
    /// If the hash is valid return the value from the query string, otherwise return empty string.
    /// </summary>
    /// <returns>Information message text</returns>
    private static string GetMessageText()
    {
        // Set the default message text shown when page parameters are not consistent.
        string messageText = GetString("page.notconsistentparameters");

        // Hash settings do not allow redirect when hash is not valid
        var hashSettings = new HashSettings("") { Redirect = false };

        // Validate the hash, exclude the requestguid parameter used in system development mode.
        if (QueryHelper.ValidateHash("hash", "requestguid", hashSettings))
        {
            // If hash is valid get the message from URL
            messageText = QueryHelper.GetText("message", QueryHelper.GetText("resstring", ""));
        }

        return GetString(messageText);
    }
}
