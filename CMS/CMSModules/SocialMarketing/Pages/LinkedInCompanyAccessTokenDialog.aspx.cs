using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.SocialMarketing.LinkedIn;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSModules_SocialMarketing_Pages_LinkedInCompanyAccessTokenDialog : CMSModalPage
{
    #region "Variables"

    private string mDataKey;
    private Hashtable mParameters;

    #endregion


    /// <summary>
    /// Space delimited list of member permissions the application is requesting on behalf of the user to manage companies.
    /// </summary>
    private const string COMPANIES_API_PERMISSION_SCOPE = "rw_organization_admin r_organization_social w_organization_social";


    #region "Properties"

    private string DataKey
    {
        get
        {
            if (String.IsNullOrEmpty(mDataKey))
            {
                mDataKey = QueryHelper.GetString("dataKey", String.Empty);
            }

            return mDataKey;
        }
    }

    /// <summary>
    /// Parameters given from control that opened the dialog.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null && !String.IsNullOrEmpty(DataKey))
            {
                mParameters = WindowHelper.GetItem(DataKey) as Hashtable;
            }

            return mParameters;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckPermissions(ModuleName.SOCIALMARKETING, PermissionsEnum.Read.ToString());
        PageTitle.TitleText = GetString("sm.linkedin.account.companyaccesstoken");
        PageTitle.ShowFullScreenButton = false;
        PageTitle.ShowCloseButton = false;

        if (Parameters == null)
        {
            ShowError(GetString("dialogs.badhashtext"));
            return;
        }

        var data = LinkedInProvider.GetLinkedInData();
        if (data.UserDeniedAccess)
        {
            ShowError(GetString("sm.linkedin.account.msg.accesstokenrefused"));
            return;
        }

        var url = LinkedInAuthenticationHelper.GetPurifiedUrl();

        if (!String.IsNullOrEmpty(data.Code))
        {
            CompleteAuthorization(data, url);
            return;
        }

        BeginAuthorization(url);
    }


    /// <summary>
    /// Completes the authorization process. Access tokens and list of administrated companies are retrived and set to control that opened the dialog.
    /// </summary>
    private void CompleteAuthorization(ILinkedInData data, Uri url)
    {
        if (LinkedInProvider.Authorize(data, url, out var token))
        {
            List<CMS.SocialMarketing.LinkedInCompany> companies;
            try
            {
                companies = CMS.SocialMarketing.LinkedInHelper.GetUserCompanies(token.AccessToken);
            }
            catch (Exception ex)
            {
                LogAndShowError("LinkedInCompanyAccessToken", "GET_COMPANIES", ex);

                return;
            }

            string formattedExpiration = token.Expiration.HasValue ? TimeZoneHelper.ConvertToUserTimeZone(token.Expiration.Value, true, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite) : String.Empty;
            string json = JsonConvert.SerializeObject(
                new
                {
                    accessToken = token.AccessToken,
                    accessTokenSecret = "",
                    tokenExpiration = token.Expiration.HasValue ? token.Expiration.Value.ToString("g", CultureInfo.InvariantCulture) : String.Empty,
                    tokenExpirationString = formattedExpiration,
                    tokenAppId = Parameters["AppInfoId"],
                    companies
                },
                new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
            );

            // Set retrieved access token to the opener window
            string script = String.Format(@"
if(wopener.linkedInCompanyControl && wopener.linkedInCompanyControl['{0}']) {{
    wopener.linkedInCompanyControl['{0}'].setData({1});
}}
CloseDialog();", Parameters["ClientID"], json);

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "TokenScript", script, true);
        }
    }


    /// <summary>
    /// Begins authorization process and redirects client to the LinkedIn authorization page.
    /// </summary>
    private void BeginAuthorization(Uri url)
    {
        try
        {
            var data = new LinkedInData((string)Parameters["ApiKey"], (string)Parameters["ApiSecret"]);
            data.AdditionalQueryParameters["scope"] = COMPANIES_API_PERMISSION_SCOPE;
            LinkedInProvider.OpenAuthorizationPage(data, url);
        }
        catch (CMS.SocialMarketing.LinkedInApiUnauthorizedException)
        {
            // The keys in LinkedIn application are not valid
            ShowError(GetString("sm.linkedin.account.msg.unauthorized"));
        }
        catch (Exception ex)
        {
            LogAndShowError("LinkedInCompanyAccessToken", "AUTH_BEGIN", ex);
        }
    }

    #endregion
}
