using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSMessages_Error : MessagePage
{
    /// <summary>
    /// Gets the title displayed on the error page
    /// </summary>
    private string ErrorPageTitle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the text displayed on the error page
    /// </summary>
    private string ErrorPageText
    {
        get;
        set;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set error state
        Response.StatusCode = 500;

        if (!ConnectionHelper.ConnectionAvailable)
        {
            // Could not connect to the database
            CssLinkHelper.MinifyCurrentRequest = false;

            string errorMessage = GetString("General.Connection.ErrorMessage");

            // Display detailed error message only if it is allowed in web.config 
            if (ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSDisplayApplicationErrorMessages"], false))
            {
                errorMessage += CMSApplication.ApplicationErrorMessage;
            }
            else
            {
                errorMessage += HTMLHelper.HTMLEncode(GetString("General.Connection.DetailedErrorMessage"));
            }

            titleElem.TitleText = GetString("General.Connection.ErrorMessageTitle"); 
            lblInfo.Text = errorMessage;
        }
        else
        {
            // Get the resource strings for error page content
            GetTitleAndTextResourceStrings();

            // Display title
            titleElem.TitleText = GetString(ErrorPageTitle);
            
            // Display custom error message
            lblInfo.Text = GetString(ErrorPageText);

            // Set button
            bool cancel = QueryHelper.GetBoolean("cancel", false);
            if (cancel)
            {
                // Display Cancel button
                btnCancel.Visible = true;
                btnCancel.Text = GetString("General.Cancel");
            }
            else
            {
                // Display link to home page
                lnkBack.Visible = true;
                lnkBack.Text = GetString("Error.Back");
                lnkBack.NavigateUrl = "~/";
            }
        }
    }


    /// <summary>
    /// Checks if hash is valid (there are two parameters excluded from hash validation - backlink and cancel)
    /// </summary>
    /// <returns>True if hash is valid, otherwise false</returns>
    private bool IsHashValid()
    {
        bool isValid = false;

        var hashSettings = new HashSettings("") { Redirect = false };

        if (QueryHelper.ValidateHash("hash", "cancel", hashSettings))
        {
            isValid = true;
        }

        return isValid;
    }


    /// <summary>
    /// Gets the title and text to be displayed on error page
    /// </summary>
    private void GetTitleAndTextResourceStrings()
    {
        // If hash is valid, get the values from the query string
        if (IsHashValid())
        {
            // Get title from URL
            ErrorPageTitle = QueryHelper.GetText("title", "");
            // Get text from URL
            ErrorPageText = QueryHelper.GetText("text", "");
        }
        else
        {
            // Get general title
            ErrorPageTitle = GetString("Error.Header");
            
            // Get general error text or display information about invalid hash if hash was found in URL
            ErrorPageText = QueryHelper.Contains("hash") ? GetString("page.notconsistentparameters") : GetString("error.info");
        }
    }
}