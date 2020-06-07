using System;
using System.Collections;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SalesForce;
using CMS.SiteProvider;


/// <summary>
/// Displays the organization credentials setting, and allows the user to authorize access to SalesForce organization.
/// </summary>
public partial class CMSModules_ContactManagement_FormControls_SalesForce_Authorization : FormEngineUserControl, ICallbackEventHandler
{

    #region "Private members"

    private bool mEnabled = true;

    #endregion

    #region "Public properties"

    /// <summary>
    /// Gets or sets the organization credentials in encrypted form.
    /// </summary>
    public override object Value
    {
        get
        {
            return CredentialsHiddenField.Value;
        }
        set
        {
            CredentialsHiddenField.Value = value as string;
        }
    }

    /// <summary>
    /// Gets or sets the value indicating whether this control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }

    /// <summary>
    /// Gets the client identifier of the control holding the setting value.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return CredentialsHiddenField.ClientID;
        }
    }

    #endregion

    #region "Protected properties"

    protected string ParametersId
    {
        get
        {
            string parametersId = ViewState["PID"] as string;
            if (String.IsNullOrEmpty(parametersId))
            {
                parametersId = Guid.NewGuid().ToString("N");
                ViewState["PID"] = parametersId;
            }
            return parametersId;
        }
    }

    #endregion

    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Scripts
        ScriptHelper.RegisterDialogScript(Page);

        // Localization
        SetupButton.Text = GetString("sf.authorize");
        ClearButton.Text = GetString("sf.removeauthorization");

        // Events
        ClearButton.Click += new EventHandler(ClearButton_Click);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        string baseUrl = UrlResolver.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/SalesForce/AuthorizationSetupPrologue.aspx");
        if (RequestContext.CurrentScheme == "https" || ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CMSSalesForceHttpCallbackEnabled"], false))
        {
            baseUrl = UrlResolver.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/SalesForce/AuthorizationSetup.aspx");
        }
        string url = URLHelper.AddParameterToUrl(baseUrl, "pid", ParametersId);
        string script = String.Format("function SalesForce_AuthorizeAccess (arg, context) {{ modalDialog('{0}', 'SalesForceAccessAuthorization', '950', '600', null, false, false, true); return false; }}", URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url)));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "SalesForce_AuthorizeAccess", script, true);
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        InitializeControls();
    }

    #endregion

    #region "Control event methods"

    protected void ClearButton_Click(object sender, EventArgs e)
    {
        Value = String.Empty;
        MessageLabel.Text = GetString("sf.organizationcredentialscleared");
    }

    #endregion

    #region "Private methods"

    private void InitializeControls()
    {
        InitializeSetupButton();
        if (String.IsNullOrEmpty(MessageLabel.Text))
        {
            // No HTML encoding
            MessageLabel.Text = CreateMessage();
        }
        SetupButton.Visible = Enabled;
        ClearButton.Visible = Enabled && !String.IsNullOrEmpty(Value as string);
    }

    private void InitializeSetupButton()
    {
        SetupButton.OnClientClick = String.Format("{0}; return false;", Page.ClientScript.GetCallbackEventReference(this, null, "SalesForce_AuthorizeAccess", null));
    }

    private Hashtable CreateParameters()
    {
        Hashtable parameters = new Hashtable();
        parameters.Add("Credentials", Value);
        parameters.Add("CredentialsHiddenFieldClientId", CredentialsHiddenField.ClientID);
        parameters.Add("MessageLabelClientId", MessageLabel.ClientID);
        parameters.Add("UrlScheme", RequestContext.CurrentScheme);
        parameters.Add("UrlPort", RequestContext.URL.Port);
        parameters.Add("SiteName", GetCurrentSiteNameForSettings());
        
        return parameters;
    }

    private string CreateMessage()
    {
        StringBuilder message = new StringBuilder();
        string content = Value as string;
        if (String.IsNullOrEmpty(content))
        {
            message.Append(GetString("sf.nocredentials"));
        }
        else
        {
            OrganizationCredentials credentials = OrganizationCredentials.Deserialize(EncryptionHelper.DecryptData(content).TrimEnd('\0'));
            message.AppendFormat(GetString("sf.organizationcredentialspresent"), credentials.UserName, credentials.OrganizationName);
        }

        return message.ToString();
    }

    private string GetCurrentSiteNameForSettings()
    {
        int siteId = QueryHelper.GetInteger("SiteID", 0);
        SiteInfo site = SiteInfo.Provider.Get(siteId);
        if (site == null)
        {
            return String.Empty;
        }

        return site.SiteName;
    }

    #endregion

    #region ICallbackEventHandler Members

    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }

    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        Hashtable parameters = CreateParameters();
        WindowHelper.Add(ParametersId, parameters);
    }

    #endregion

}