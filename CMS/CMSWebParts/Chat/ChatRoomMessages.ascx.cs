using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_ChatRoomMessages: CMSAbstractWebPart
{
    bool mIsSupport = false;
    int mRoomID = -1;

    #region Properties

    /// <summary>
    /// Gets or sets ItemTemplate property.
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
    /// Gets or sets Count property.
    /// </summary>
    public int Count
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("Count"), -1);
        }
        set
        {
            this.SetValue("Count", value);
        }
    }


    /// <summary>
    /// Gets or sets DisplayInline property.
    /// </summary>
    public bool DisplayInline
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("DisplayInline"), false);
        }
        set
        {
            this.SetValue("DisplayInline", value);
        }
    }


    /// <summary>
    /// Gets or sets Direction property.
    /// </summary>
    public ChatRoomMessagesDirectionEnum Direction
    {
        get
        {
            return (ChatRoomMessagesDirectionEnum)ValidationHelper.GetInteger(this.GetValue("Direction"), (int)ChatRoomMessagesDirectionEnum.Up);
        }
        set
        {
            this.SetValue("Direction", (int)value);
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

    #endregion


    #region "Page events"


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeRoomMessages", this, InnerContainerTitle, InnerContainerName);
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
        // Register to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Insert script references
        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/BBCodeParser.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/SmileysResolver.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatRoomMessages_files/ChatRoomMessages.js");

        imgInformationDialog.ImageUrl = GetImageUrl("General/Labels/Information.png");

        RoomID = ChatUIHelper.GetRoomIdFromQuery(RoomID, GroupID);

        // Prepare and run startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatMessages_" + ClientID, BuildStartupScript(), true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Builds startup script.
    /// </summary>
    private string BuildStartupScript()
    {
        string json = JsonConvert.SerializeObject(
            new
            {
                roomID = RoomID,
                chatMessageTemplate = ChatUIHelper.GetWebpartTransformation(ChatMessageTransformationName,"chat.error.transformation.messages.message"),
                count = (Count >= 0) ? Count : ChatSettingsProvider.FirstLoadMessagesCountSetting,
                contentClientID =  "#" + pnlChatRoomMessages.ClientID,
                displayInline = DisplayInline,
                groupID = GroupID,
                clientID = ClientID,
                direction = (int)Direction,
                enableBBCode = (ChatSettingsProvider.EnableBBCodeSetting && EnableBBCode),
                loadingDiv = ChatUIHelper.GetWebpartLoadingDiv("ChatMessagesWPLoading", "chat.wploading.messages"),
                envelopeID = "#envelope_" + ClientID,
                pnlInformDialog = "#" + pnlChatRoomMessagesInfoDialog.ClientID,
                btnInformDialogClose = "#" + btnChatMessageInformDialogClose.ClientID,
                container = "#" + pnlChatRoomWebpart.ClientID
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        return String.Format("InitChatMessagesWebpart({0});", json);
    }

    #endregion
}
