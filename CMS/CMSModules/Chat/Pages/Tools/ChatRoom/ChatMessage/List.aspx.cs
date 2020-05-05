using System;

using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[ParentObject(ChatRoomInfo.OBJECT_TYPE, "roomid")]
[EditedObject(ChatRoomInfo.OBJECT_TYPE, "roomid")]

[Action(0, "chat.chatmessage.new", "Edit.aspx?roomid={%EditedObjectParent.ChatRoomID%}")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_ChatMessage_List : CMSChatRoomPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        listElem.ChatRoomID = QueryHelper.GetInteger("roomid", 0);

        HeaderActions.Enabled = HasUserModifyPermission();
    }

    #endregion
}
