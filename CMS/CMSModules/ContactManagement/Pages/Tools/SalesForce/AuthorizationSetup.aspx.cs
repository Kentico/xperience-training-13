using System;
using System.Collections;
using System.Text;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SalesForce;
using CMS.SalesForce.RestContract;


/// <summary>
/// Guides the user through the process of authorizing access to SalesForce organization.
/// </summary>
public partial class CMSModules_ContactManagement_Pages_Tools_SalesForce_AuthorizationSetup : CMSSalesForceDialogPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help topic page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "salesforce_integration_config";

    #endregion


    #region "Private members"

    private OrganizationCredentials mCredentials;
    private string mSourceCredentialsHiddenFieldClientId;
    private string mSourceMessageLabelClientId;
    private string mSourceUrlScheme;
    private int mSourceUrlPort;
    private string mSourceSiteName;

    #endregion


    #region "Protected properties"

    protected OrganizationCredentials Credentials
    {
        get
        {
            return mCredentials;
        }
    }

    protected string SourceCredentialsHiddenFieldClientId
    {
        get
        {
            return mSourceCredentialsHiddenFieldClientId;
        }
    }

    protected string SourceMessageLabelClientId
    {
        get
        {
            return mSourceMessageLabelClientId;
        }
    }

    protected string SourceUrlScheme
    {
        get
        {
            return mSourceUrlScheme;
        }
    }

    protected int SourceUrlPort
    {
        get
        {
            return mSourceUrlPort;
        }
    }

    protected string SourceSiteName
    {
        get
        {
            return mSourceSiteName;
        }
    }

    protected string RedirectUrl
    {
        get
        {
            Uri currentUrl = RequestContext.URL;
            if (ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSSalesForceHttpCallbackEnabled"], false))
            {
                UriBuilder builder = new UriBuilder
                {
                    Scheme = "http",
                    Host = currentUrl.Host,
                    Path = currentUrl.AbsolutePath
                };
                if (currentUrl.Port != 80)
                {
                    builder.Port = currentUrl.Port;
                }
                return builder.Uri.ToString();
            }
            else
            {
                UriBuilder builder = new UriBuilder
                {
                    Scheme = "https",
                    Host = currentUrl.Host,
                    Path = currentUrl.AbsolutePath
                };
                return builder.Uri.ToString();
            }
        }
    }

    protected string ParametersId
    {
        get
        {
            string value = QueryHelper.GetString("pid", null);
            if (String.IsNullOrEmpty(value))
            {
                value = QueryHelper.GetString("state", null);
            }

            return value;
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        Page.ClientScript.GetPostBackEventReference(ConfirmButton, String.Empty);
        PageTitle.TitleText = GetString("sf.authorization.title");
        PageTitle.IsDialog = false;
        PageTitle.HelpTopicName = HELP_TOPIC_LINK;
        CurrentMaster.HeadElements.Text += @"<base target=""_self"" />";
        ConfirmButton.Click += ConfirmButton_Click;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            RestoreParameters();
            if (RequestHelper.IsPostBack() && Page.Request.Params[postEventSourceID] == "SaveButton")
            {
                // The access to the Settings page was lost, it might happen as the Internet Explorer opens the target URL of SalesForce redirection in a new window.
                if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
                {
                    // The current user is a global administrator, so it is safe to proceed. 
                    // Without this check there is a security vulnerability as any CMS Desk user could choose a site and authorize access to his or her Sales Cloud organization.
                    CloseDialog(GetString("sf.authorization.partialsuccess"));
                    SettingsKeyInfoProvider.SetGlobalValue("CMSSalesForceCredentials", CredentialsHiddenField.Value);
                    CredentialsHiddenField.Value = String.Empty;
                }
            }
            else
            {
                RedirectUrlLiteral.Text = HTMLHelper.HTMLEncode(RedirectUrl);
                string authorizationCode = QueryHelper.GetString("code", null);
                if (!String.IsNullOrEmpty(authorizationCode))
                {
                    SalesForceAuthorizationHelper authorizationHelper = new SalesForceAuthorizationHelper(Credentials.ClientId, Credentials.ClientSecret, RedirectUrl);
                    GetAuthenticationTokensResponse response = authorizationHelper.GetAuthenticationTokens(authorizationCode);
                    Identity identity = authorizationHelper.GetIdentity(response);
                    Credentials.RefreshToken = response.RefreshToken;
                    Credentials.OrganizationBaseUrl = response.InstanceBaseUrl;
                    Credentials.UserName = identity.UserName;
                    Credentials.OrganizationName = GetOrganizationName(Credentials, identity.OrganizationId);
                    StoreParameters();
                    if (RequestContext.CurrentScheme != SourceUrlScheme)
                    {
                        RedirectToScheme(SourceUrlScheme, SourceUrlPort);
                    }
                    else
                    {
                        CloseDialog(GetString("sf.authorization.success"));
                    }
                }
                else
                {
                    string state = QueryHelper.GetString("state", null);
                    if (!String.IsNullOrEmpty(state))
                    {
                        CloseDialog(GetString("sf.authorization.success"));
                    }
                }
            }
        }
        catch (Exception exception)
        {
            HandleError(exception);
        }
    }

    #endregion


    #region "Control event methods"

    protected void ConfirmButton_Click(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(ClientIdentifierTextBox.Text) || String.IsNullOrEmpty(ClientSecretTextBox.Text))
        {
            SalesForceError.Report(GetString("sf.authorization.missingconsumerinput"));
        }
        else
        {
            Credentials.ClientId = ClientIdentifierTextBox.Text;
            Credentials.ClientSecret = ClientSecretTextBox.Text;
            try
            {
                StoreParameters();
                SalesForceAuthorizationHelper authorizationHelper = new SalesForceAuthorizationHelper(Credentials.ClientId, Credentials.ClientSecret, RedirectUrl);
                string authorizationUrl = authorizationHelper.GetAuthorizationUrl(ParametersId);
                URLHelper.ResponseRedirect(authorizationUrl, false);
            }
            catch (Exception exception)
            {
                HandleError(exception);
            }
        }
    }

    #endregion


    #region "Private methods"

    private string GetOrganizationName(OrganizationCredentials credentials, string organizationId)
    {
        RefreshTokenSessionProvider provider = new RefreshTokenSessionProvider
        {
            ClientId = credentials.ClientId,
            ClientSecret = credentials.ClientSecret,
            RefreshToken = credentials.RefreshToken
        };
        Session session = provider.CreateSession();
        SalesForceClient client = new SalesForceClient(session);
        EntityModel organizationModel = client.DescribeEntity("Organization");
        SelectEntitiesResult result = client.SelectEntities(String.Format("select Name, Division from Organization where Id = '{0}'", organizationId), organizationModel);
        if (result.TotalEntityCount == 0)
        {
            return String.Empty;
        }
        Entity organization = result.Entities[0];
        string name = organization.GetAttributeValue<string>("Name");
        string division = organization.GetAttributeValue<string>("Division");
        StringBuilder builder = new StringBuilder();
        if (!String.IsNullOrEmpty(name))
        {
            builder.Append(name);
        }
        if (!String.IsNullOrEmpty(division))
        {
            if (builder.Length > 0)
            {
                builder.AppendFormat("({0})", division);
            }
            else
            {
                builder.Append(division);
            }
        }

        return builder.ToString();
    }

    private void StoreParameters()
    {
        Hashtable parameters = WindowHelper.GetItem(ParametersId) as Hashtable;
#pragma warning disable 618
        parameters["Credentials"] = EncryptionHelper.EncryptData(OrganizationCredentials.Serialize(Credentials));
#pragma warning restore 618
        WindowHelper.Add(ParametersId, parameters);
    }

    private void RestoreParameters()
    {
        string parametersId = QueryHelper.GetString("pid", null);
        if (!String.IsNullOrEmpty(parametersId))
        {
            if (!QueryHelper.ValidateHash("hash"))
            {
                throw new Exception("[SalesForceAuthorizationSetupPage.RestoreParameters]: Invalid query hash.");
            }
        }
        else
        {
            parametersId = QueryHelper.GetString("state", null);
        }
        Hashtable parameters = WindowHelper.GetItem(parametersId) as Hashtable;
        if (parameters == null)
        {
            throw new Exception("[SalesForceAuthorizationSetupPage.RestoreParameters]: The dialog page parameters are missing, the session might have been lost.");
        }
        string credentials = ValidationHelper.GetString(parameters["Credentials"], String.Empty);
        if (!String.IsNullOrEmpty(credentials))
        {
            mCredentials = OrganizationCredentials.Deserialize(EncryptionHelper.DecryptData(credentials).TrimEnd('\0'));
        }
        else
        {
            mCredentials = new OrganizationCredentials();
        }
        mSourceMessageLabelClientId = ValidationHelper.GetString(parameters["MessageLabelClientId"], String.Empty);
        mSourceCredentialsHiddenFieldClientId = ValidationHelper.GetString(parameters["CredentialsHiddenFieldClientId"], String.Empty);
        mSourceUrlScheme = ValidationHelper.GetString(parameters["UrlScheme"], String.Empty);
        mSourceUrlPort = ValidationHelper.GetInteger(parameters["UrlPort"], -1);
        mSourceSiteName = ValidationHelper.GetString(parameters["SiteName"], String.Empty);
    }

    private void RedirectToScheme(string scheme, int port)
    {
        Uri currentUrl = RequestContext.URL;
        UriBuilder builder = new UriBuilder
        {
            Scheme = scheme,
            Port = port,
            Host = currentUrl.Host,
            Path = currentUrl.AbsolutePath,
            Query = String.Format("state={0}", ParametersId)
        };
        URLHelper.ResponseRedirect(builder.ToString(), false);

    }

    private void CloseDialog(string message)
    {
        string text = HTMLHelper.HTMLEncode(message);
#pragma warning disable 618
        CredentialsHiddenField.Value = EncryptionHelper.EncryptData(OrganizationCredentials.Serialize(Credentials));
#pragma warning restore 618
        MessageHiddenField.Value = text;
        MessageLabel.InnerHtml = text;
        MessageLabel.Visible = true;
        MainMessagePanel.Visible = true;
        MainPanel.Visible = false;
        FooterPanel.Visible = false;
    }

    private void HandleError(Exception exception)
    {
        SalesForceError.Report(exception);
        Service.Resolve<IEventLogService>().LogException("Salesforce.com Connector", "AuthorizationSetupPage", exception);
    }

    #endregion
}
