using System;
using System.Web;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSMessages_invalidWebsite : MessagePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Try skip IIS http errors
        Response.TrySkipIisCustomErrors = true;
        // Set error state
        Response.StatusCode = 503;

        string mPrefix = "http://";

        titleElem.TitleText = GetString("Message.InvalidWebSite");
        string mDomain = RequestContext.URL.Host;
        if (RequestContext.URL.Port != 80)
        {
            mDomain = mDomain + ":" + RequestContext.URL.Port.ToString();
        }

        if (RequestContext.IsSSL)
        {
            mPrefix = "https://";
        }

        lblMessage.Text = GetString("Message.TextInvalidWebSite") + " ";
        lblMessageUrl.Text = mPrefix + mDomain + HttpUtility.HtmlEncode(RequestContext.CurrentURL);

        lblInfo1.Text = GetString("Message.InfoInvalidWebSite1") + " ";
        lblInfo2.Text = " " + GetString("Message.InfoInvalidWebSite2") + " ";
        lblInfoDomain.Text = mDomain;

        // Get the Sites page url
        lnkAdministration.Text = GetString("Message.LinkInvalidWebSite");
        lnkAdministration.NavigateUrl = ApplicationUrlHelper.GetApplicationUrl("cms", "sites");

        if (LicenseContext.CurrentLicenseInfo == null)
        {
            pnlLicense.Visible = true;
        }
    }
}