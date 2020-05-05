using System;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_ChatSupportRequest : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Transformation name for messages in ChatRoomMessages child webpart
    /// </summary>
    public string ChatMessageTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatMessageTransformationName"), ChatSettingsProvider.TransformationRoomMessages);
        }
        set
        {
            this.SetValue("ChatMessageTransformationName", value);
        }
    }

    /// <summary>
    /// Transformation name for errors in ChatErrors child webpart
    /// </summary>
    public string ChatErrorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatErrorTransformationName"), ChatSettingsProvider.TransformationErrors);
        }
        set
        {
            this.SetValue("ChatErrorTransformationName", value);
        }
    }


    /// <summary>
    /// Transformation name for clear all errors button in ChatErrors child webpart
    /// </summary>
    public string ChatErrorDeleteAllButtonTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatErrorDeleteAllButtonTransformationName"), ChatSettingsProvider.TransformationErrorsDeleteAll);
        }
        set
        {
            this.SetValue("ChatErrorDeleteAllButtonTransformationName", value);
        }
    }

    /// <summary>
    /// Transformation name for users in ChatRoomUsers child webpart
    /// </summary>
    public string ChatRoomUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatRoomUserTransformationName"), ChatSettingsProvider.TransformationRoomUsers);
        }
        set
        {
            this.SetValue("ChatRoomUserTransformationName", value);
        }
    }


    /// <summary>
    /// Transformation name for support chat request button
    /// </summary>
    public string ChatSupportRequestTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ChatSupportRequestTransformationName"), ChatSettingsProvider.TransformationSupportRequest);
        }
        set
        {
            this.SetValue("ChatSupportRequestTransformationName", value);
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ChatCssHelper.RegisterStylesheet(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion

        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatSupportRequest_files/ChatSupportRequest.js");

        // Create and store settings for popup chat window.
        int id = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        string json = JsonConvert.SerializeObject(
            new
            {
                onlineUrl = ChatUIHelper.GetChatRoomWindowURL(),
                clientID = pnlSupportChatRequest.ClientID,
                guid = id,
                trans = ChatUIHelper.GetWebpartTransformation(string.IsNullOrEmpty(ChatSupportRequestTransformationName) ? ChatSettingsProvider.TransformationSupportRequest : ChatSupportRequestTransformationName, "chat.error.transformation.request"),
                mailEnabled = ChatSettingsProvider.IsSupportMailEnabledAndValid,
                pnlInformDialog = "#" + pnlChatSupportRequestInfoDialog.ClientID,
                btnInformDialogClose = "#" + btnChatSupportRequestInformDialogClose.ClientID
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );

        string startupScript = String.Format("ChatSupportRequest({0});", json);

        // Run startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SupportChatRequest_" + ClientID, startupScript, true);
    }
}
