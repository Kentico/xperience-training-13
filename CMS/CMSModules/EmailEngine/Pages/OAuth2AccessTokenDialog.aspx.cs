using System;
using System.Collections;
using System.Threading;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_EmailEngine_Pages_OAuth2AccessTokenDialog : CMSModalGlobalAdminPage
{
    private const string SESSION_KEY = "EmailOAuthCredentials_TokenRequest";
    private const string AUTHORIZATION_CODE_QUERY_PARAMETER = "code";
    private const string STATE_QUERY_PARAMETER = "state";

    private readonly string mRedirectUrl = URLHelper.GetAbsoluteUrl("~/CMSModules/EmailEngine/Pages/OAuth2AccessTokenDialog.aspx?redirected=1");


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        PageTitle.ShowFullScreenButton = false;
        PageTitle.ShowCloseButton = false;

        bool redirected = QueryHelper.GetBoolean("redirected", false);

        if (!redirected)
        {
            BeginAuthorization();
            return;
        }
        else
        {
            var authorizationCode = QueryHelper.GetString(AUTHORIZATION_CODE_QUERY_PARAMETER, String.Empty);

            if (String.IsNullOrEmpty(authorizationCode))
            {
                ShowError(GetString("emailoauthtokendialog.authcodemissing"));
            }

            var state = QueryHelper.GetString(STATE_QUERY_PARAMETER, String.Empty);
   

            var sessionData = WindowHelper.GetItem(SESSION_KEY) as Hashtable;

            if (sessionData == null)
            {
                ShowError(GetString("dialogs.badhashtext"));
                return;
            }

            var expectedState = (string)sessionData["state"];
            var credentialsGuid = ValidationHelper.GetGuid(sessionData["credentials"], Guid.Empty);

            if (String.IsNullOrEmpty(expectedState) || credentialsGuid == Guid.Empty)
            {
                ShowError(GetString("dialogs.badhashtext"));
                return;
            }

            if (!String.Equals(state, expectedState, StringComparison.Ordinal))
            {
                ShowError(GetString("emailoauthtokendialog.invalidstateparam"));
            }

            var credentials = EmailOAuthCredentialsInfo.Provider.Get(credentialsGuid);

            if (credentials == null)
            {
                ShowError(GetString("emailoauthtokendialog.credentialsnotfound"));
                return;
            }

            CompleteAuthorization(authorizationCode, credentials);
        }
    }


    private void CloseDialog()
    {
        var closeDialogScript = "CloseAndRefresh();";
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseDialogScript" + ClientID, closeDialogScript, true);
    }


    /// <summary>
    /// Begins authorization process and redirects client to the OAuth provider authorization page.
    /// </summary>
    private void BeginAuthorization()
    {
        var credentialsGuid = QueryHelper.GetGuid("credentials", Guid.Empty);
        var credentials = EmailOAuthCredentialsInfo.Provider.Get(credentialsGuid);

        if (credentials == null)
        {
            ShowError(GetString("emailoauthtokendialog.credentialsnotfound"));
            return;
        }

        var provider = ClassHelper.GetClass(credentials.EmailOAuthCredentialsProviderAssemblyName, credentials.EmailOAuthCredentialsProviderClass) as IEmailOAuthProvider;

        if (provider == null)
        {
            throw new InvalidOperationException($"Authentication provider '{credentials.EmailOAuthCredentialsProviderClass}' for credentials {credentials.EmailOAuthCredentialsDisplayName} was not found.");
        }

        var state = Guid.NewGuid().ToString();

        // Store credentials and state parameter in session
        Hashtable parameters = new Hashtable
        {
                { "state", state },
                { "credentials", credentialsGuid.ToString() }
        };

        WindowHelper.Add(SESSION_KEY, parameters);

        try
        {
            var authCodeUrl = provider.GetAuthorizationUrl(mRedirectUrl, state, credentials);

            URLHelper.Redirect(authCodeUrl);
        }
        catch (ThreadAbortException)
        {
            // Reset exception - this exception is expected because client is redirected to external page.
            Thread.ResetAbort();
        }
        catch (Exception exception)
        {
            ShowErrorAndLogException(exception);
            RemoveSessionEntry();
        }
    }


    /// <summary>
    /// Trades the <paramref name="authorizationCode"/> for OAuth access token and stores it in the <paramref name="credentials"/>.
    /// </summary>
    private void CompleteAuthorization(string authorizationCode, EmailOAuthCredentialsInfo credentials)
    {
        try
        {
            var provider = ClassHelper.GetClass(credentials.EmailOAuthCredentialsProviderAssemblyName, credentials.EmailOAuthCredentialsProviderClass) as IEmailOAuthProvider;
            if (provider == null)
            {
                throw new InvalidOperationException($"Authentication provider '{credentials.EmailOAuthCredentialsProviderClass}' for credentials {credentials.EmailOAuthCredentialsDisplayName} was not found.");
            }

            provider.CompleteAuthorization(authorizationCode, mRedirectUrl, credentials);

            CloseDialog();
        }
        catch (Exception exception)
        {
            ShowErrorAndLogException(exception);
        }
        finally
        {
            RemoveSessionEntry();
        }
    }


    private void ShowErrorAndLogException(Exception exception)
    {
        ShowError(ResHelper.GetStringFormat("General.ErrorOccuredLog", "OAuthTokenDialog", "ACCESSTOKEN", exception.Message));
        Service.Resolve<IEventLogService>().LogException("OAuthTokenDialog", "ACCESSTOKEN", exception);
    }


    private static void RemoveSessionEntry()
    {
        WindowHelper.Remove(SESSION_KEY);
    }
}
