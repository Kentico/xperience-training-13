using System;

using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.UIControls;


[ParentObject(ChatRoomInfo.OBJECT_TYPE, "roomid")]

[EditedObject(ChatMessageInfo.OBJECT_TYPE, "messageid")]

[Help("chat_rooms_edit_message")]

[Breadcrumbs()]
[Breadcrumb(0, "chat.messages", "~/CMSModules/Chat/Pages/Tools/ChatRoom/ChatMessage/List.aspx?roomid={?roomid?}", null)]
[Breadcrumb(1, ResourceString = "chat.chatmessage.edit", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "chat.chatmessage.new", NewObject = true)]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_ChatMessage_Edit : CMSChatRoomPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (EditedObjectParent != null)
        {
            editElem.ChatRoomID = ((ChatRoomInfo)EditedObjectParent).ChatRoomID;
        }
    }

    #endregion
}
