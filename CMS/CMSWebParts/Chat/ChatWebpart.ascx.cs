using System;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

public partial class CMSWebParts_Chat_ChatWebpart : CMSAbstractWebPart
{
    #region "Properties"
    

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
    /// Gets or sets EnableBBCode property.
    /// </summary>
    public bool EnableBBCode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableBBCode"), false);
        }
        set
        {
            SetValue("EnableBBCode", value);
        }
    }

    
    /// <summary>
    /// Gets or sets DisplayInline property.
    /// </summary>
    public bool DisplayInline
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayInline"), false);
        }
        set
        {
            SetValue("DisplayInline", value);
        }
    }


    /// <summary>
    /// Gets or sets Direction property.
    /// </summary>
    public ChatRoomMessagesDirectionEnum Direction
    {
        get
        {
            return (ChatRoomMessagesDirectionEnum)ValidationHelper.GetInteger(GetValue("Direction"), (int)ChatRoomMessagesDirectionEnum.Up);
        }
        set
        {
            SetValue("Direction", (int)value);
        }
    }


    /// <summary>
    /// Gets or sets Count property.
    /// </summary>
    public int Count
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Count"), -1);
        }
        set
        {
            SetValue("Count", value);
        }
    }


    /// <summary>
    /// Gets or sets EnableNotificationBubble property.
    /// </summary>
    public bool EnableNotificationBubble
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableNotificationBubble"), true);
        }
        set
        {
            SetValue("EnableNotificationBubble", value);
        }
    }


    /// <summary>
    /// Gets or sets DisplayInitialTitle property.
    /// </summary>
    public bool DisplayInitialTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayInitialTitle"), false);
        }
        set
        {
            SetValue("DisplayInitialTitle", value);
        }
    }


    /// <summary>
    /// Initial title text
    /// </summary>
    public string InitialTitle
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("InitialTitle"), ResHelper.GetString("chat.roomname.initialtitle"));
        }
        set
        {
            SetValue("InitialTitle", value);
        }
    }

    
    /// <summary>
    /// Gets or sets ErrorShowDeleteAllBtn property.
    /// </summary>
    public bool ErrorShowDeleteAllBtn
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ErrorShowDeleteAllBtn"), false);
        }
        set
        {
            SetValue("ErrorShowDeleteAllBtn", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomsEnablePaging property.
    /// </summary>
    public bool RoomsEnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RoomsEnablePaging"), false);
        }
        set
        {
            SetValue("RoomsEnablePaging", value);
        }
    }

    
    /// <summary>
    /// Gets or sets RoomsPagingItems property.
    /// </summary>
    public int RoomsPagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RoomsPagingItems"), -1);
        }
        set
        {
            SetValue("RoomsPagingItems", value);
        }
    }
    
    
    /// <summary>
    /// Gets or sets RoomsGroupPagesBy property.
    /// </summary>
    public int RoomsGroupPagesBy
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RoomsGroupPagesBy"), -1);
        }
        set
        {
            SetValue("RoomsGroupPagesBy", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomsEnableFiltering property.
    /// </summary>
    public bool RoomsEnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RoomsEnableFiltering"), false);
        }
        set
        {
            SetValue("RoomsEnableFiltering", value);
        }
    }

    
    /// <summary>
    /// Gets or sets RoomsShowFilterItems property.
    /// </summary>
    public int RoomsShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RoomsShowFilterItems"), -1);
        }
        set
        {
            SetValue("RoomsShowFilterItems", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomUsersEnablePaging property.
    /// </summary>
    public bool RoomUsersEnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RoomUsersEnablePaging"), false);
        }
        set
        {
            SetValue("RoomUsersEnablePaging", value);
        }
    }

    
    /// <summary>
    /// Gets or sets RoomUsersPagingItems property.
    /// </summary>
    public int RoomUsersPagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RoomUsersPagingItems"), -1);
        }
        set
        {
            SetValue("RoomUsersPagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomUsersGroupPagesBy property.
    /// </summary>
    public int RoomUsersGroupPagesBy
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RoomUsersGroupPagesBy"), -1);
        }
        set
        {
            SetValue("RoomUsersGroupPagesBy", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomUsersEnableFiltering property.
    /// </summary>
    public bool RoomUsersEnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RoomUsersEnableFiltering"), false);
        }
        set
        {
            SetValue("RoomUsersEnableFiltering", value);
        }
    }

    
    /// <summary>
    /// Gets or sets RoomUsersShowFilterItems property.
    /// </summary>
    public int RoomUsersShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RoomUsersShowFilterItems"), -1);
        }
        set
        {
            SetValue("RoomUsersShowFilterItems", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomUsersSortByStatus property.
    /// </summary>
    public bool RoomUsersSortByStatus
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RoomUsersSortByStatus"), false);
        }
        set
        {
            SetValue("RoomUsersSortByStatus", value);
        }
    }


    /// <summary>
    /// Gets or sets RoomsEnablePopup property.
    /// </summary>
    public bool RoomsEnablePopup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RoomsEnablePopup"), false);
        }
        set
        {
            SetValue("RoomsEnablePopup", value);
        }
    }


    /// <summary>
    /// Gets or sets SearchMode property.
    /// </summary>
    public bool SearchMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SearchMode"), false);
        }
        set
        {
            SetValue("SearchMode", value);
        }
    }


    /// <summary>
    /// Gets or sets OnlineUsersEnablePaging property.
    /// </summary>
    public bool OnlineUsersEnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("OnlineUsersEnablePaging"), false);
        }
        set
        {
            SetValue("OnlineUsersEnablePaging", value);
        }
    }

    
    /// <summary>
    /// Gets or sets OnlineUsersPagingItems property.
    /// </summary>
    public int OnlineUsersPagingItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("OnlineUsersPagingItems"), -1);
        }
        set
        {
            SetValue("OnlineUsersPagingItems", value);
        }
    }


    /// <summary>
    /// Gets or sets OnlineUsersGroupPagesBy property.
    /// </summary>
    public int OnlineUsersGroupPagesBy
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("OnlineUsersGroupPagesBy"), -1);
        }
        set
        {
            SetValue("OnlineUsersGroupPagesBy", value);
        }
    }


    /// <summary>
    /// Gets or sets OnlineUsersEnableFiltering property.
    /// </summary>
    public bool OnlineUsersEnableFiltering
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("OnlineUsersEnableFiltering"), false);
        }
        set
        {
            SetValue("OnlineUsersEnableFiltering", value);
        }
    }

    
    /// <summary>
    /// Gets or sets OnlineUsersShowFilterItems property.
    /// </summary>
    public int OnlineUsersShowFilterItems
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("OnlineUsersShowFilterItems"), -1);
        }
        set
        {
            SetValue("OnlineUsersShowFilterItems", value);
        }
    }


    /// <summary>
    /// Gets or sets ResponseMaxUsers property.
    /// </summary>
    public int ResponseMaxUsers
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ResponseMaxUsers"), -1);
        }
        set
        {
            SetValue("ResponseMaxUsers", value);
        }
    }


    /// <summary>
    /// Gets or sets ChatRoomsTransformation property.
    /// </summary>
    public string ChatRoomsTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatRoomsTransformation"), ChatSettingsProvider.TransformationRooms);
        }
        set
        {
            SetValue("ChatRoomsTransformation", value);
        }
    }

    
    /// <summary>
    /// Gets or sets ChatRoomNameTransformationName property.
    /// </summary>
    public string ChatRoomNameTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ChatRoomNameTransformationName"), ChatSettingsProvider.TransformationRoomName);
        }
        set
        {
            SetValue("ChatRoomNameTransformationName", value);
        }
    }

    
    /// <summary>
    /// Gets or sets NotificationTransformation property.
    /// </summary>
    public string NotificationTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NotificationTransformation"), ChatSettingsProvider.TransformationNotifications);
        }
        set
        {
            SetValue("NotificationTransformation", value);
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
    /// Gets or sets ChatUserTransformationName property.
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
    /// Gets or sets ItemTemplate property.
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
    /// Gets or sets ButtonTemplate property.
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
    
   
    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        ChatCssHelper.RegisterStylesheet(Page);
    } 


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!ObjectFactory<ILicenseService>.StaticSingleton().IsFeatureAvailable(FeatureEnum.Chat))
        {
            ShowError();
            return;
        }

        // Register to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Insert cript references
        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatWebpart_files/ChatWebpart.js");
        
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatWebpart_" + ClientID, string.Format("InitChatWebpart('{0}');", pnlChatWebpartHeader.ClientID), true);
        
        SetUpWebparts();
    }


    private void ShowError()
    {
        lblError.Text = String.Format(GetString("licenselimitation.featurenotavailable"), FeatureEnum.Chat);
        lblError.Visible = true;
        pnlChatWebpart.Visible = false;
    }


    private void SetUpWebparts()
    {
        RoomLeave.GroupID = GroupID;

        Errors.ErrorTransformationName = ChatErrorTransformationName;
        Errors.ButtonDeleteAllTransformationName = ChatErrorDeleteAllButtonTransformationName;
        Errors.ShowDeleteAllBtn = ErrorShowDeleteAllBtn;

        Notification.NotificationTransformation = NotificationTransformation;
        Notification.ChatMessageTransformationName = ChatMessageTransformationName;
        Notification.ChatRoomUserTransformationName = ChatUserTransformationName;
        Notification.ChatErrorTransformationName = ChatErrorTransformationName;
        Notification.ChatErrorDeleteAllButtonTransformationName = ChatErrorDeleteAllButtonTransformationName;
        Notification.EnableNotificationBubble = EnableNotificationBubble;

        OnlineUsers.OnlineUserTransformationName = OnlineUserTransformationName;
        OnlineUsers.EnablePaging = OnlineUsersEnablePaging;
        OnlineUsers.PagingItems = OnlineUsersPagingItems;
        OnlineUsers.GroupPagesBy = OnlineUsersGroupPagesBy;
        OnlineUsers.EnableFiltering = OnlineUsersEnableFiltering;
        OnlineUsers.ShowFilterItems = OnlineUsersShowFilterItems;
        OnlineUsers.ChatMessageTransformationName = ChatMessageTransformationName;
        OnlineUsers.ChatRoomUserTransformationName = ChatUserTransformationName;
        OnlineUsers.ChatErrorTransformationName = ChatErrorTransformationName;
        OnlineUsers.ChatErrorDeleteAllButtonTransformationName = ChatErrorDeleteAllButtonTransformationName;

        SearchOnlineUsers.OnlineUserTransformationName = OnlineUserTransformationName;
        SearchOnlineUsers.PagingEnabled = OnlineUsersEnablePaging;
        SearchOnlineUsers.PagingItems = OnlineUsersPagingItems;
        SearchOnlineUsers.GroupPagesBy = OnlineUsersGroupPagesBy;
        SearchOnlineUsers.ResponseMaxUsers = ResponseMaxUsers;
        SearchOnlineUsers.ChatMessageTransformationName = ChatMessageTransformationName;
        SearchOnlineUsers.ChatRoomUserTransformationName = ChatUserTransformationName;
        SearchOnlineUsers.ChatErrorTransformationName = ChatErrorTransformationName;
        SearchOnlineUsers.ChatErrorDeleteAllButtonTransformationName = ChatErrorDeleteAllButtonTransformationName;

        Rooms.GroupID = GroupID;
        Rooms.ListItemTransformation = ChatRoomsTransformation;
        Rooms.EnablePaging = RoomsEnablePaging;
        Rooms.PagingItems = RoomsPagingItems;
        Rooms.GroupPagesBy = RoomsGroupPagesBy;
        Rooms.EnableFiltering = RoomsEnableFiltering;
        Rooms.ShowFilterItems = RoomsShowFilterItems;
        Rooms.EnablePopup = RoomsEnablePopup;

        RoomMessages.GroupID = GroupID;
        RoomMessages.Count = Count;
        RoomMessages.EnableBBCode = EnableBBCode;
        RoomMessages.DisplayInline = DisplayInline;
        RoomMessages.Direction = Direction;
        RoomMessages.ChatMessageTransformationName = ChatMessageTransformationName;

        RoomName.GroupID = GroupID;
        RoomName.DisplayInitialTitle = DisplayInitialTitle;
        RoomName.InitialTitle = InitialTitle;
        RoomName.ChatRoomNameTransformationName = ChatRoomNameTransformationName;

        RoomUsers.GroupID = GroupID;
        RoomUsers.EnablePaging = RoomUsersEnablePaging;
        RoomUsers.PagingItems = RoomUsersPagingItems;
        RoomUsers.GroupPagesBy = RoomUsersGroupPagesBy;
        RoomUsers.EnableFiltering = RoomUsersEnableFiltering;
        RoomUsers.ShowFilterItems = RoomUsersShowFilterItems;
        RoomUsers.ChatUserTransformationName = ChatUserTransformationName;
        RoomUsers.ChatRoomUserTransformationName = ChatUserTransformationName;
        RoomUsers.ChatMessageTransformationName = ChatMessageTransformationName;
        RoomUsers.ChatErrorTransformationName = ChatErrorTransformationName;
        RoomUsers.ChatErrorDeleteAllButtonTransformationName = ChatErrorDeleteAllButtonTransformationName;
        RoomUsers.SortByStatus = RoomUsersSortByStatus;
        RoomUsers.InviteSearchMode = SearchMode;
        RoomUsers.InviteSearchModeMaxUsers = ResponseMaxUsers;

        RoomMessageSend.GroupID = GroupID;
        RoomMessageSend.EnableBBCode = EnableBBCode;

        if (SearchMode)
        {
            SearchOnlineUsers.Enabled = true;
            SearchOnlineUsers.Visible = true;
        }
        else
        {
            OnlineUsers.Enabled = true;
            OnlineUsers.Visible = true;
        }
    }
}
