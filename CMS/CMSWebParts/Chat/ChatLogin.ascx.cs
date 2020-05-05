using System;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_ChatLogin : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Gets or sets RedirectURLEnter property.
    /// </summary>
    public string RedirectURLLogout
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("RedirectURLLogout"), "");
        }
        set
        {
            this.SetValue("RedirectURLLogout", value);
        }
    }


    /// <summary>
    /// Gets or sets RedirectURLLogout property.
    /// </summary>
    public string RedirectURLEnter
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("RedirectURLEnter"), "");
        }
        set
        {
            this.SetValue("RedirectURLEnter", value);
        }
    }

    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }

    #endregion


    #region "Page Events"

    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeLogin", this, InnerContainerTitle, InnerContainerName);
        ChatCssHelper.RegisterStylesheet(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Insert script references
        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatLogin_files/ChatLogin.js");

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatLogin_" + ClientID, BuildStartupScript(), true);

    }

    #endregion


    #region "Methods"

    private string BuildStartupScript()
    {
        txtChatUserLoginRelogNickname.WatermarkText = ChatSettingsProvider.GuestPrefixSetting;

        string redirectURLLogout = RedirectURLLogout.Length > 0 ? RedirectURLLogout : ChatSettingsProvider.RedirectURLLogoutSetting;
        if (redirectURLLogout.Length > 0){
            redirectURLLogout =  ChatHelper.GetDocumentAbsoluteUrl(redirectURLLogout);
        }
        string redirectURLLogin = RedirectURLEnter.Length > 0 ? RedirectURLEnter : ChatSettingsProvider.RedirectURLLoginSetting;
        if (redirectURLLogin.Length > 0)
        {
            redirectURLLogin = ChatHelper.GetDocumentAbsoluteUrl(redirectURLLogin);
        }

        // Set onclick events
        btnChatUserLoggedInChangeNickname.OnClientClick = "ChatManager.Login.DisplayChangeNicknameForm('" + ClientID + "'); return false;";
        btnChatUserChangeNicknameButton.OnClientClick = "ChatManager.Login.ChangeNickname($cmsj('#" + txtChatUserChangeNicknameInput.ClientID + "').val(), '" + ClientID + "'); return false;";
        btnChangeNicknameCancel.OnClientClick = "ChatManager.Login.ChangeNickname(null, null, true); return false;";

        string json = JsonConvert.SerializeObject(
            new
            {
                pnlChatUserLoggedIn = pnlChatUserLoggedIn.ClientID,
                pnlChatUserChangeNicknameForm = pnlChatUserChangeNicknameForm.ClientID,
                lblChatUserLoggedInInfoValue = lblChatUserLoggedInInfoValue.ClientID,
                btnChatUserChangeNicknameButton = btnChatUserChangeNicknameButton.ClientID,
                pnlChatUserLoginError = pnlChatUserLoginError.ClientID,
                lblChatUserLoginErrorText = lblChatUserLoginErrorText.ClientID,
                txtChatUserChangeNicknameInput = txtChatUserChangeNicknameInput.ClientID,
                clientID = ClientID,
                pnlChatUserLoginRelog = pnlChatUserLoginRelog.ClientID,
                btnLogout = btnChatUserLoggedInLogout.ClientID,
                resStrLogout =ResHelper.GetString("chat.login.resStrLogoutAnonym"),
                txtChatUserLoginRelogNickname = txtChatUserLoginRelogNickname.ClientID,
                lblChatUserLoginRelogNickname = lblChatUserLoginRelogNickname.ClientID,
                lblChatUserLoginRelogText = lblChatUserLoginRelogText.ClientID,
                redirectURLLogout = redirectURLLogout,
                redirectURLEnter = redirectURLLogin,
                resStrLogAsAnonym = ResHelper.GetString("chat.login.logasanonym"),
                resStrLogAsCMS = ResHelper.GetString("chat.login.logascms"),
                btnChatUserLoginRelog = btnChatUserLoginRelog.ClientID,
                resStrNoService = ResHelper.GetString("chat.servicenotavailable")
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        return String.Format("InitChatLogin({0});", json);
    }

    #endregion

}
