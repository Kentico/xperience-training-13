using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_InitiatedChat : CMSAbstractWebPart
{
    #region Properties

    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("TransformationName"), ChatSettingsProvider.TransformationInitiatedChat);
        }
        set
        {
            this.SetValue("TransformationName", value);
        }
    }


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

    #endregion


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatCssHelper.RegisterStylesheet(Page, false);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
        {
            return;
        }

        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/ChatSettings.ashx");

        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/InitiatedChat_files/InitiatedChat.js");

        int optID = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        // Run script
        string json = JsonConvert.SerializeObject(
            new
            {
                clientID = pnlInitiatedChat.ClientID,
                contentID = pnlContent.ClientID,
                pnlErrorID = pnlError.ClientID,
                lblErrorID = lblError.ClientID,
                windowURL = ChatUIHelper.GetChatRoomWindowURL(),
                trans = ChatUIHelper.GetWebpartTransformation(TransformationName, "chat.error.transformation.initiatedchat.error"),
                guid = optID,
                pingTick = ChatSettingsProvider.GlobalPingIntervalSetting * 1000
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        string startupScript = string.Format("InitInitiatedChatManager({0});", json);
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatInitiatedChat_" + ClientID, startupScript, true);
    }

}
