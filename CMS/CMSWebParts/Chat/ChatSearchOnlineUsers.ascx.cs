using System;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_ChatSearchOnlineUsers : CMSAbstractWebPart
{

    #region "Variables"

    bool mInviteMode = false;
    string mGroupName = "";
    bool mIsSupport = false;

    #endregion


    #region Properties

    /// <summary>
    /// Gets or sets OnlineUserTransformationName property.
    /// </summary>
    public string OnlineUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("OnlineUserTransformationName"), ChatSettingsProvider.TransformationOnlineUsers);
        }
        set
        {
            SetValue("OnlineUserTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatMessageTransformationName property.
    /// </summary>
    public string ChatMessageTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatMessageTransformationName"), ChatSettingsProvider.TransformationRoomMessages);
        }
        set
        {
            SetValue("ChatMessageTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatErrorTransformationName property.
    /// </summary>
    public string ChatErrorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatErrorTransformationName"), ChatSettingsProvider.TransformationErrors);
        }
        set
        {
            SetValue("ChatErrorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatErrorDeleteAllButtonTransformationName property.
    /// </summary>
    public string ChatErrorDeleteAllButtonTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatErrorDeleteAllButtonTransformationName"), ChatSettingsProvider.TransformationErrorsDeleteAll);
        }
        set
        {
            SetValue("ChatErrorDeleteAllButtonTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatRoomUserTransformationName property.
    /// </summary>
    public string ChatRoomUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatRoomUserTransformationName"), ChatSettingsProvider.TransformationRoomUsers);
        }
        set
        {
            SetValue("ChatRoomUserTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets PagingEnabled property.
    /// </summary>
    public bool PagingEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("PagingEnabled"), false);
        }
        set
        {
            SetValue("PagingEnabled", value);
        }
    }


    /// <summary>
    /// Gets or sets PagingItems property.
    /// </summary>
    public int PagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PagingItems"), -1);
        }
        set
        {
            SetValue("PagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets GroupPagesBy property.
    /// </summary>
    public int GroupPagesBy
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupPagesBy"), -1);
        }
        set
        {
            SetValue("GroupPagesBy", value);
        }
    }


    /// <summary>
    /// Gets or sets ResponseMaxUsers property.
    /// </summary>
    public int ResponseMaxUsers
    {
        get
        {
            int maxUsers = ValidationHelper.GetInteger(GetValue("ResponseMaxUsers"), -1);
            if (maxUsers < 0)
            {
                maxUsers = ChatSettingsProvider.WPSearchModeMaxUsers;
            }
            return maxUsers;
        }
        set
        {
            SetValue("ResponseMaxUsers", value);
        }
    }


    /// <summary>
    /// Indicates if webpart is in invite mode
    /// That means that clicking users in list will invite them to current room.
    /// </summary>
    public bool InviteMode
    {
        get
        {
            return mInviteMode;
        }
        set
        {
            mInviteMode = value;
        }
    }


    /// <summary>
    /// Gets or sets GroupName property (only used when invite mode is set).
    /// </summary>
    public string GroupName
    {
        get
        {
            return mGroupName;
        }
        set
        {
            mGroupName = value;
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


    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }


    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

    }


    protected void Page_Prerender(object sender, EventArgs e)
    {
        if (!IsVisible)
        {
            return;
        }
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeOnlineUsers", this, InnerContainerTitle, InnerContainerName);
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
        if (!IsVisible)
        {
            pnlChatSearchOnlineUsersWP.Visible = false;
            return;
        }

        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion
        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatSearchOnlineUsers_files/ChatSearchOnlineUsers.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ListPaging.js");

        if (!PagingEnabled)
        {
            pnlChatSearchOnlineUsersPaging.Visible = false;
        }
        if (InviteMode)
        {
            pnlChatSearchOnlineUsersInvite.Visible = true;
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "SearchOnlineUsers_" + ClientID, BuildStartupScript(), true);
    }


    #region "Methods"

    /// <summary>
    /// Builds startup script.
    /// </summary>
    private string BuildStartupScript()
    {
        int id = ChatPopupWindowSettingsHelper.Store(ChatMessageTransformationName, ChatRoomUserTransformationName, ChatErrorTransformationName, ChatErrorDeleteAllButtonTransformationName);

        string json = JsonConvert.SerializeObject(
            new
            {
                onlineUserTemplate = ChatUIHelper.GetWebpartTransformation(OnlineUserTransformationName, "chat.error.transformation.users.onlineuser"),
                chatRoomWindowUrl = ChatUIHelper.GetChatRoomWindowURL(Page),
                clientID = ClientID,
                GUID = id,
                contentClientID = pnlChatSearchOnlineUsers.ClientID,
                textbox = txtChatSearchOnlineUsers.ClientID,
                button = btnChatSearchOnlineUsers.ClientID,
                pnlPaging = pnlChatSearchOnlineUsersPaging.ClientID,
                pagingEnabled = PagingEnabled,
                pagingItems = PagingItems > 0 ? PagingItems : ChatSettingsProvider.WPPagingItems,
                groupPagesBy = GroupPagesBy >= 0 ? GroupPagesBy : ChatSettingsProvider.WPGroupPagesBy,
                maxUsers = ResponseMaxUsers,
                inviteMode = InviteMode,
                loadingDiv = ChatUIHelper.GetWebpartLoadingDiv("ChatSearchOnlineUsersWPLoading", "chat.wploading.searchonlineusers"),
                resStrMoreFound = (ResponseMaxUsers > 0) ? String.Format(ResHelper.GetString("chat.searchonlineusers.morefound"), ResponseMaxUsers) : "",
                resStrNotFound = ResHelper.GetString("chat.searchonlineusers.notfound"),
                pnlInfo = pnlChatSearchOnlineUsersInfo.ClientID,
                resStrFound = ResHelper.GetString("chat.searchonlineusers.results"),
                envelopeID = "envelope_" + ClientID,
                groupID = GroupName,
                invitePanel = pnlChatSearchOnlineUsersInvite.ClientID
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        return String.Format("InitChatSearchOnlineUsersWebpart({0});", json);
    }

    #endregion

}
