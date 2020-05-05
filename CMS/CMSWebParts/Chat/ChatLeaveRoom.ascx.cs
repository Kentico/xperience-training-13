using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

public partial class CMSWebParts_Chat_ChatLeaveRoom : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Gets or sets GroupID property.
    /// </summary>
    public string GroupID
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("GroupID"), "DefaultGroup");
        }
        set
        {
            this.SetValue("GroupID", value);
        }
    }
    

    /// <summary>
    /// Gets or sets RedirectURL property.
    /// </summary>
    public string RedirectURL
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("RedirectURL"), "");
        }
        set
        {
            this.SetValue("RedirectURL", value);
        }
    }

    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }

    #endregion

    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeLeaveRoom", this, InnerContainerTitle, InnerContainerName);
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
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatLeaveRoom_files/ChatLeaveRoom.js");

        string redirectURL = RedirectURL.Length > 0 ? RedirectURL : ChatSettingsProvider.RedirectURLLeaveSetting;

        // Prepare and run startup script
        string startupScript = String.Format("InitChatLeaveRoom({{groupID:{0}, clientID:'{1}', btnChatLeaveRoom:{2},pnlContent:{3}, redirectURL:{4}, envelopeID: '#envelope_{1}' }});",
            ScriptHelper.GetString(GroupID),
            ClientID,
            ScriptHelper.GetString("#" + btnChatLeaveRoom.ClientID),
            ScriptHelper.GetString('#' + pnlChatLeaveRoom.ClientID),
            redirectURL.Length > 0 ? ScriptHelper.GetString(ChatHelper.GetDocumentAbsoluteUrl(redirectURL)) : "\"\""
            );

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatLeaveRoom_" + ClientID, startupScript, true);
    }
}
