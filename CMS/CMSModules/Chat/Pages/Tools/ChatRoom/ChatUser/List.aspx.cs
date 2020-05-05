using System;

using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[ParentObject(ChatRoomInfo.OBJECT_TYPE, "roomid")]
[EditedObject(ChatRoomInfo.OBJECT_TYPE, "roomid")]

[Action(0, "chat.chatroomuser.new", "Edit.aspx?roomid={%EditedObjectParent.ChatRoomID%}")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_ChatUser_List : CMSChatRoomPage
{
    #region "Private properties"

    private int ChatRoomID
    {
        get
        {
            return QueryHelper.GetInteger("roomid", 0);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        listElem.ChatRoomID = ChatRoomID;

        HeaderActions.Enabled = HasUserModifyPermission();

        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            ShowChangesSaved();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Disable "Add new user" action if room is support room (one to one)
        if (((ChatRoomInfo)UIContext.EditedObjectParent).IsOneToOneSupport)
        {
            var actions = CurrentMaster.HeaderActions.ActionsList;
            if ((actions != null) && (actions.Count > 0))
            {
                actions[0].Enabled = false;
            }
        }
    }

    #endregion
}
