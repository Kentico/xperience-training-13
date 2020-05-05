using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.UIControls;


public partial class CMSModules_ApplicationDashboard_ApplicationDashboard : CMSDeskPage
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check license for the current domain
        LicenseValidationEnum licenseCheck = LicenseHelper.ValidateLicenseForDomain(RequestContext.CurrentDomain);
        if (licenseCheck != LicenseValidationEnum.Valid)
        {
            URLHelper.ResponseRedirect(URLHelper.ResolveUrl("~/CMSMessages/invalidlicensekey.aspx"));
        }

        InitializeLicenseOwner();

        // Provide application name for client to refresh breadcrumbs correctly
        RequestContext.ClientApplication.Add("applicationName", GetString("cms.dashboard"));

        ScriptHelper.RegisterAngularModule("CMS.ApplicationDashboard/main");
    }


    /// <summary>
    /// Initializes the license owner label.
    /// </summary>
    private void InitializeLicenseOwner()
    {
        var lic = LicenseKeyInfoProvider.GetLicenseKeyInfo(RequestContext.CurrentDomain);

        // Display the license owner if present in license
        if ((lic != null) && !string.IsNullOrEmpty(lic.Owner))
        {
            pnlLicenseOwner.Visible = true;
            lblLicenseOwner.Text = HTMLHelper.HTMLEncode(String.Format(GetString("cms.dashboard.licenseowner"), lic.Owner));
        }
    }
}
