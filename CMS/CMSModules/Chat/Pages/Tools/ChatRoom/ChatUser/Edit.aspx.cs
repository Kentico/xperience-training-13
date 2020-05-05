using System;

using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.MacroEngine;
using CMS.UIControls;


[ParentObject(ChatRoomInfo.OBJECT_TYPE, "roomid")]

[EditedObject(ChatRoomUserInfo.OBJECT_TYPE, "roomUserID")]

[Help("chat_rooms_new_user", NewObject = true)]
[Help("chat_rooms_edit_user", ExistingObject = true)]

[Breadcrumbs()]
[Breadcrumb(0, "chat.roomusers", "~/CMSModules/Chat/Pages/Tools/ChatRoom/ChatUser/List.aspx?roomid={?roomid?}", null)]
[Breadcrumb(1, Text = "{%EditedChatUserNickname%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "chat.roomuser.new", NewObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_ChatUser_Edit : CMSChatRoomPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObjectParent != null)
        {
            editElem.ChatRoomID = ((ChatRoomInfo)EditedObjectParent).ChatRoomID;
        }

        // If editing, add user's nickname to macro resolver, so it can be retrieved when applying breacrumb attribute
        ChatRoomUserInfo editedChatUser = EditedObject as ChatRoomUserInfo;
        if (editedChatUser != null && editedChatUser.ChatRoomUserID > 0)
        {
            int chatUserID = editedChatUser.ChatRoomUserChatUserID;

            ChatUserInfo chatUser = ChatUserInfoProvider.GetChatUserInfo(chatUserID);

            MacroContext.CurrentResolver.SetNamedSourceData("EditedChatUserNickname", chatUser.ChatUserNickname);
        }
    }

    #endregion
}
