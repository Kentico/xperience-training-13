using System;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSWebParts_Chat_ChatRoomUsers : CMSAbstractWebPart
{
    bool mIsSupport = false;
    int mRoomID = -1;

    #region "Properties"

    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string ChatUserTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatUserTransformationName"), ChatSettingsProvider.TransformationRoomUsers);
        }
        set
        {
            SetValue("ChatUserTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets SortByStatus property.
    /// </summary>
    public bool SortByStatus
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SortByStatus"), true);
        }
        set
        {
            SetValue("SortByStatus", value);
        }
    }


    /// <summary>
    /// Gets or sets InviteEnabled property.
    /// </summary>
    public bool InviteEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("InviteUserEnabled"), true);
        }
        set
        {
            SetValue("InviteUserEnabled", value);
        }
    }


    /// <summary>
    /// Gets or sets InviteSearchMode property.
    /// </summary>
    public bool InviteSearchMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("InviteSearchMode"), ChatSettingsProvider.WPInviteModeSearchMode);
        }
        set
        {
            SetValue("InviteSearchMode", value);
        }
    }


    /// <summary>
    /// Gets or sets InviteSearchModeMaxUsers property.
    /// </summary>
    public int InviteSearchModeMaxUsers
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("InviteSearchModeMaxUsers"), -1);
        }
        set
        {
            SetValue("InviteSearchModeMaxUsers", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatMessageTemplate property.
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
    /// Gets or sets ItemTemplateModified property.
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
    /// Gets or sets ItemTemplateRejected property.
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
    /// Gets or sets ItemTemplateSystem property.
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
    /// Gets or sets RoomName property.
    /// </summary>
    public string RoomName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RoomName"), "");
        }
        set
        {
            SetValue("RoomName", value);
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
            return ValidationHelper.GetString(GetValue("GroupID"), "DefaultGroup");
        }
        set
        {
            SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Gets or sets EnablePaging property.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), false);
        }
        set
        {
            SetValue("EnablePaging", value);
        }
    }


    /// <summary>
    /// Gets or sets EnableFiltering property.
    /// </summary>
    public bool EnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableFiltering"), false);
        }
        set
        {
            SetValue("EnableFiltering", value);
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
    /// Gets or sets ShowFilterItems property.
    /// </summary>
    public int ShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ShowFilterItems"), -1);
        }
        set
        {
            SetValue("ShowFilterItems", value);
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
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeRoomUsers", this, InnerContainerTitle, InnerContainerName);
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

        // Insert cript references
        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ListPaging.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ChatDialogs.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatRoomUsers_files/ChatRoomUsers.js");

        RoomID = ChatUIHelper.GetRoomIdFromQuery(RoomID, GroupID);

        if (EnableFiltering)
        {
            pnlChatRoomUsersFiltering.Visible = true;
        }

        // Set properties to invite webpart
        ChatSearchOnlineUsers.InviteMode = ChatOnlineUsersElem.InviteMode = true;
        ChatSearchOnlineUsers.IsSupport = ChatOnlineUsersElem.IsSupport = IsSupport;
        ChatSearchOnlineUsers.GroupName = ChatOnlineUsersElem.GroupName = GroupID;
        ChatSearchOnlineUsers.OnlineUserTransformationName = ChatOnlineUsersElem.OnlineUserTransformationName = "Chat.Transformations.ChatOnlineUser";
        ChatSearchOnlineUsers.PagingEnabled = ChatOnlineUsersElem.EnablePaging = true;
        int invitePagingItems = (ChatSettingsProvider.WPInviteModePagingItems > 0) ? ChatSettingsProvider.WPInviteModePagingItems : ChatSettingsProvider.WPPagingItems;
        if (!(invitePagingItems > 0))
        {
            invitePagingItems = PagingItems;
        }
        ChatOnlineUsersElem.ShowFilterItems = invitePagingItems + 1;
        ChatSearchOnlineUsers.PagingItems = ChatOnlineUsersElem.PagingItems = invitePagingItems;
        ChatSearchOnlineUsers.ResponseMaxUsers = (InviteSearchModeMaxUsers >= 0) ? InviteSearchModeMaxUsers : ChatSettingsProvider.WPSearchModeMaxUsers;
        ChatOnlineUsersElem.EnableFiltering = true;

        if (InviteSearchMode == true)
        {
            ChatOnlineUsersElem.Visible = false;
        }
        else
        {
            ChatSearchOnlineUsers.Visible = false;
        }

        // Run startup script
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatUsers_" + ClientID, BuildStartupScript() , true);

        imgChatRoomUsersInvitePrompt.ImageUrl = GetImageUrl("CMSModules/CMS_Chat/add24.png");
    }

    #endregion


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
                roomID = RoomID,
                chatUserTemplate = ChatUIHelper.GetWebpartTransformation(ChatUserTransformationName,"chat.error.transformation.users.user"),
                oneToOneURL = ChatUIHelper.GetChatRoomWindowURL(Page),
                contentClientID = pnlChatRoomUsers.ClientID,
                clientID = ClientID,
                groupID = GroupID,
                sortByStatus = SortByStatus,
                GUID = id,
                pnlChatRoomUsersInvitePrompt = pnlChatRoomUsersInvitePrompt.ClientID,
                pnlChatRoomUsersInvite = pnlChatRoomUsersInvite.ClientID,
                chatOnlineUsersElem = ChatOnlineUsersElem.ClientID,
                chatSearchOnlineUsersElem = ChatSearchOnlineUsers.ClientID,
                loadingDiv = ChatUIHelper.GetWebpartLoadingDiv("ChatRoomUsersWPLoading", "chat.wploading.roomusers"),
                btnChatRoomUsersInvite = btnChatRoomUsersInvite.ClientID,
                btnChatRoomsDeletePromptClose = btnChatRoomsDeletePromptClose.ClientID,
                pnlFilterClientID = pnlChatRoomUsersFiltering.ClientID,
                pnlPagingClientID = pnlChatRoomUsersPaging.ClientID,
                pagingItems = PagingItems > 0 ? PagingItems : ChatSettingsProvider.WPPagingItems,
                groupPagesBy = GroupPagesBy >= 0 ? GroupPagesBy : ChatSettingsProvider.WPGroupPagesBy,
                pagingEnabled = EnablePaging,
                btnFilter = btnChatRoomUsersFilter.ClientID,
                txtFilter = txtChatRoomUsersFilter.ClientID,
                filterEnabled = EnableFiltering,
                pnlInfo = pnlChaRoomUsersInfo.ClientID,
                resStrNoFound = ResHelper.GetString("chat.searchonlineusers.notfound"),
                resStrResults = ResHelper.GetString("chat.searchonlineusers.results"),
                filterCount = ShowFilterItems >= 0 ? ShowFilterItems : ChatSettingsProvider.WPShowFilterLimit,
                envelopeID = "envelope_" + ClientID,
                inviteSearchMode = InviteSearchMode,
                inviteEnabled = InviteEnabled
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        return String.Format("InitChatUsersWebpart({0});", json);
    }

    #endregion
}
