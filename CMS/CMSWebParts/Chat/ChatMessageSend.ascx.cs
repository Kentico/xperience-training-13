using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_ChatMessageSend : CMSAbstractWebPart
{
    #region "Constants"

    /// <summary>
    /// Identifier for help topic link.
    /// </summary>
    private const string CANNED_RESPONSES_HELP_TOPIC = "canned_responses";

    #endregion


    #region "Variables"

    bool mIsSupport = false;
    int mTooltipLength = 60;
    int mRoomID = -1;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets RoomName property.
    /// </summary>
    public string RoomName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("RoomName"), "");
        }
        set
        {
            this.SetValue("RoomName", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomID property.
    /// </summary>
    public int RoomID
    {
        get
        {
            if (mRoomID < 0)
            {
                ChatRoomInfo room = ChatRoomInfoProvider.GetChatRoomInfo(RoomName);
                if (room != null)
                {
                    mRoomID = room.ChatRoomID;
                }
                else
                {
                    mRoomID = 0;
                }
            }
            return mRoomID;
        }
        set
        {
            mRoomID = value;
        }
    }


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
    /// Indicates if this webpart is in support chat window.
    /// </summary>
    public bool IsSupport
    {
        get
        {
            return mIsSupport;
        }
        set
        {
            mIsSupport = value;
        }
    }


    /// <summary>
    /// Gets or sets EnableBBCode property.
    /// </summary>
    public bool EnableBBCode
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("EnableBBCode"), true);
        }
        set
        {
            this.SetValue("EnableBBCode", value);
        }
    }

    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Hides elements that are irelevant for one to one window.
    /// </summary>
    public void HideUserPicker()
    {
        pnlRecipientContainer.Visible = false;
    }

    #endregion


    #region "Page Events"

    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeMessageSend", this, InnerContainerTitle, InnerContainerName);
        if (IsSupport)
        {
            ChatCssHelper.RegisterStylesheet(Page, true);
        }
        else
        {
            ChatCssHelper.RegisterStylesheet(Page);
        }
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
        ScriptHelper.RegisterJQueryUI(Page);
        ScriptHelper.RegisterScriptFile(Page, "jquery/jquery-a-tools.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/BBCodeParser.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatMessageSend_files/ChatMessageSend.js");

        imgInformationDialog.ImageUrl = GetImageUrl("General/Labels/Information.png");

        RoomID = ChatUIHelper.GetRoomIdFromQuery(RoomID, GroupID);

        // Register startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatMessageSend_" + ClientID, BuildStartupScript(), true);

        // Set link to documentation and tooltip for canned responses
        lnkCannedRespHelp.NavigateUrl = DocumentationHelper.GetDocumentationTopicUrl(CANNED_RESPONSES_HELP_TOPIC);
        lnkCannedRespHelp.ToolTip = ResHelper.GetString("chat.cannedresponses.helplabel");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Converts control's clientID to javascript jQuery selector string
    /// </summary>
    /// <param name="control">Webcontrol</param>
    private string GetString(WebControl control)
    {
        return "#" + control.ClientID;
    }


    private string BuildStartupScript()
    {

        bool enBBCode = IsSupport || (ChatSettingsProvider.EnableBBCodeSetting && EnableBBCode);
        WebControl input = enBBCode ? ucBBEditor.TextArea : txtMessage;
        if (enBBCode)
        {
            txtMessage.Visible = false;
        }
        else
        {
            ucBBEditor.Visible = false;
        }

        string json = JsonConvert.SerializeObject(
            new
            {
                roomID = RoomID,
                inputClientID = GetString(input),
                buttonClientID = GetString(btnSendMessage),
                groupID = GroupID,
                chbWhisperClientID = GetString(chbWhisper),
                drpRecipientClientID = GetString(drpRecipient),
                pnlRecipientContainerClientID = GetString(pnlRecipientContainer),
                noneLabel = ResHelper.GetString("chat.everyone"),
                enableBBCode = enBBCode,
                bbCodeClientID = GetString(ucBBEditor),
                btnCannedResponses = GetString(btnCannedResponses),
                pnlContent = GetString(pnlWebpartContent),
                envelopeID = "#envelope_" + ClientID,
                informDialogID = GetString(pnlChatMessageSendInfoDialog),
                btnInformDialogClose = GetString(btnChatMessageSendInformDialogClose)
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        string startupScript = String.Format("InitChatSenderWebpart({0});", json);

        // If this webpart is for support person -> generate "Canned responses"
        if ((ChatOnlineUserHelper.GetLoggedInChatUser() != null) && (IsSupport == true))
        {
            // Get canned responses from database
            IEnumerable<ChatSupportCannedResponseInfo> cannedResponses = ChatSupportCannedResponseInfoProvider.GetCannedResponses(ChatOnlineUserHelper.GetLoggedInChatUser().ChatUserID, SiteContext.CurrentSiteID);

            if (cannedResponses.Any())
            {
                plcCannedResponses.Visible = true;

                // Register necessary files
                ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatMessageSend_files/CannedResponses.js");
                CssRegistration.RegisterCssLink(Page, "~/App_Themes/Design/Chat/ChatIntelliSense.css");

                // Creates canned responses in format expected in javascript
                var cannedResponseToSerialize = from cr in cannedResponses
                                                let resolvedText = MacroResolver.Resolve(cr.ChatSupportCannedResponseText)
                                                select new
                                                {
                                                    label = "#" + HTMLHelper.HTMLEncode(cr.ChatSupportCannedResponseTagName),
                                                    tooltip = HTMLHelper.HTMLEncode(TextHelper.LimitLength(resolvedText, mTooltipLength)),
                                                    value = resolvedText
                                                };

                // Serialize canned responses to JS Array expected by javascript
                string cannedResponsesJSArray = "";
                try
                {
                    cannedResponsesJSArray = JsonConvert.SerializeObject(cannedResponseToSerialize, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
                }
                catch (Exception ex)
                {
                    Service.Resolve<IEventLogService>().LogException("Chat", "JSON serialization of canned responses", ex);
                }
                startupScript += string.Format("var CannedResponses = {0};", cannedResponsesJSArray);

                startupScript += string.Format("InitCannedResponses({0}, {1});", ScriptHelper.GetString("#" + input.ClientID), ScriptHelper.GetString("#" + btnCannedResponses.ClientID));
            }
        }

        return startupScript;
    }

    #endregion
}
