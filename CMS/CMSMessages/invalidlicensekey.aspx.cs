using System;
using System.Web;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSMessages_invalidlicensekey : MessagePage
{
    private const string CLIENT_PORTAL = "http://client.kentico.com/";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set error state
        Response.StatusCode = 503;

        titleElem.TitleText = GetString("InvalidLicense.Header");
        lblRawUrl.Text = GetString("InvalidLicense.RawUrl");
        lblResult.Text = GetString("InvalidLicense.Result");

        // URL
        string rawUrl = QueryHelper.GetString("rawUrl", String.Empty).Trim();
        if (rawUrl != String.Empty)
        {
            lblRawUrlValue.Text = HttpUtility.HtmlEncode(rawUrl);
        }
        else
        {
            lblRawUrl.Visible = false;
            lblRawUrlValue.Visible = false;
        }

        // Validation result
        LicenseValidationEnum mResult = (LicenseValidationEnum)QueryHelper.GetInteger("Result", Convert.ToInt32(LicenseValidationEnum.NotAvailable));
        lblResultValue.Text = LicenseHelper.GetValidationResultString(mResult);

        // URL 'Go to:'
        lnkGoToValue.NavigateUrl = ApplicationUrlHelper.GetApplicationUrl("Licenses", "Licenses");
        lblAddLicenseValue.Text = RequestContext.CurrentDomain;

        // How to get license options
        lnkOpt1.NavigateUrl = CLIENT_PORTAL;
        lnkOpt1.ToolTip = CLIENT_PORTAL;
        lnkOpt3.NavigateUrl = CLIENT_PORTAL;
        lnkOpt3.ToolTip = CLIENT_PORTAL;

        if (mResult == LicenseValidationEnum.Expired)
        {
            genTrial.InnerHtml = GetString("invalidlicense.trialexpired");
            genTrial.Visible = true;
        }
    }
}