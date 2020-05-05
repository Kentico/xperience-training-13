using System;
using System.Globalization;
using System.Text;
using System.Web;

using CMS.Helpers;
using CMS.UIControls;
using CMS.WebServices;

public partial class CMSModules_REST_FormControls_GenerateHash : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("rest.generateauthhash");
        btnAuthenticate.Text = GetString("rest.authenticate");
        btnAuthenticate.Click += btnAuthenticate_Click;
        dateExpiration.SelectedDateTime = (dateExpiration.SelectedDateTime == DateTime.MinValue)
            ? DateTime.Now.AddMonths(1)
            : dateExpiration.SelectedDateTime;
    }


    protected void btnAuthenticate_Click(object sender, EventArgs e)
    {
        string[] urls = txtUrls.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        StringBuilder newUrls = new StringBuilder();

        foreach (string url in urls)
        {
            string urlWithoutHashAndUpdatedExpiration = RemoveHashAndUpdateExpiration(url, dateExpiration.SelectedDateTime);
            string newUrl = urlWithoutHashAndUpdatedExpiration;
            string query = URLHelper.GetQuery(newUrl).TrimStart('?');

            if (RESTServiceHelper.TryParseRestUrlPath(newUrl, out string absolutePathPrefix, out string relativeRestPath))
            {
                string domain = URLHelper.GetDomain(newUrl);
                newUrl = URLHelper.RemoveQuery(relativeRestPath);

                // Rewrite the URL to physical URL
                string[] rewritten = BaseRESTService.RewriteRESTUrl(newUrl, query, domain, "GET");

                newUrl = absolutePathPrefix + rewritten[0].TrimStart('~') + "?" + rewritten[1];
                newUrl = HttpUtility.UrlDecode(newUrl);

                // Get the hash from real URL
                newUrls.AppendLine(URLHelper.AddParameterToUrl(urlWithoutHashAndUpdatedExpiration, "hash", RESTServiceHelper.GetUrlPathAndQueryHash(newUrl)));
            }
            else
            {
                newUrls.AppendLine(url);
            }
        }

        txtUrls.Text = newUrls.ToString();
    }


    /// <summary>
    /// Removes the hash parameter from <paramref name="url"/> and updates the hash expiration in ISO-8601 format.
    /// </summary>
    private string RemoveHashAndUpdateExpiration(string url, DateTime hashExpiration)
    {
        url = URLHelper.RemoveParameterFromUrl(url, "hash");
        url = URLHelper.UpdateParameterInUrl(
            url,
            "hashExpirationUtc",
            hashExpiration.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));

        return url;
    }
}