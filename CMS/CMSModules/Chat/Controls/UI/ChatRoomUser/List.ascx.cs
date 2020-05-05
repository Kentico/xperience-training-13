using System;
using System.Data;

using CMS.Base;
using CMS.Chat;

using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Chat_Controls_UI_ChatRoomUser_List : CMSAdminListControl
{
    #region "Private fields"

    private ChatRoomInfo chatRoom;

    #endregion


    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return this.gridElem;
        }
    }


    public ChatRoomInfo ChatRoom
    {
        get
        {
            if (chatRoom == null)
            {
                chatRoom = ChatRoomInfoProvider.GetChatRoomInfo(ChatRoomID);
            }

            return chatRoom;
        }
    }


    /// <summary>
    /// Filters displayed chat users only to users of this chat room.
    /// </summary>
    public int ChatRoomID { get; set; }


    private bool HasUserModifyPermission { get; set; }


    private int? SiteID
    {
        get
        {
            return ((ChatRoomInfo)UIContext.EditedObjectParent).ChatRoomSiteID;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        AdminLevelEnum minimumAdminLevel;
        if (ChatRoom.ChatRoomPrivate)
        {
            // In private rooms, chat user must have join rights or higher to be displayed
            minimumAdminLevel = AdminLevelEnum.Join;
        }
        else
        {
            // In public rooms everybody has Join rights, so only users with AdminRights and higher are displayed
            minimumAdminLevel = AdminLevelEnum.Admin;
        }

        Grid.WhereCondition = string.Format("ChatRoomUserRoomID = {0} AND (ChatRoomUserAdminLevel >= {1} OR ChatRoomUserJoinTime IS NOT NULL)", ChatRoomID, (int)minimumAdminLevel);
        Grid.EditActionUrl = "Edit.aspx?roomUserID={0}&roomid=" + ChatRoomID;

        Grid.OnExternalDataBound += new OnExternalDataBoundEventHandler(Grid_OnExternalDataBound);
        Grid.OnAction += new OnActionEventHandler(Grid_OnAction);

        HasUserModifyPermission = ((CMSChatPage)Page).HasUserModifyPermission(SiteID);
    }


    void Grid_OnAction(string actionName, object actionArgument)
    {
        // If room is global also modifyglobal permission has to be checked.. add it to every action which has modify permisison
        ((CMSChatPage)Page).CheckModifyPermission(SiteID);

        int chatRoomUserID = ValidationHelper.GetInteger(actionArgument, 0);

        if (chatRoomUserID == 0)
        {
            return;
        }

        ChatRoomUserInfo chatRoomUser = ChatRoomUserInfoProvider.GetChatRoomUser(chatRoomUserID);

        if (chatRoomUser == null)
        {
            return;
        }

        // Can't perform any action in one to one support room
        if (ChatRoom.IsOneToOneSupport)
        {
            ShowError(GetString("chat.cantrevokerightsofsupportroomcreator"));
            return;
        }

        switch (actionName)
        {
            case "kick":
                if (!chatRoomUser.IsOnline)
                {
                    ShowError(GetString("chat.cantkickofflineuser"));
                    return;
                }
                ChatRoomUserHelper.KickUserFromRoom(ChatRoomID, ChatUserInfoProvider.GetChatUserInfo(chatRoomUser.ChatRoomUserChatUserID), ChatUserHelper.GetChatUserFromCMSUser(MembershipContext.AuthenticatedUser));
                break;
            case "revoke":
                if (chatRoomUser.ChatRoomUserAdminLevel == AdminLevelEnum.Creator)
                {
                    ShowError(GetString("chat.cantrevokecreatorrights"));
                    return;
                }
                ChatRoomUserHelper.KickUserPermanentlyFromRoom(ChatRoomID, ChatUserInfoProvider.GetChatUserInfo(chatRoomUser.ChatRoomUserChatUserID), ChatUserHelper.GetChatUserFromCMSUser(MembershipContext.AuthenticatedUser));
                break;
        }
    }


    object Grid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        int chatUserID = ValidationHelper.GetInteger(parameter, 0);
        string name = sourceName.ToLowerCSafe();
        switch (name)
        {
            case "chatroomuserchatuserid":
                ChatUserInfo user = ChatUserInfoProvider.GetChatUserInfo(chatUserID);

                return ChatUIHelper.GetCMSDeskChatUserField(this, user);
            case "adminlevel":
                AdminLevelEnum adminLevel = (AdminLevelEnum)parameter;

                return adminLevel.ToStringValue();
            case "onlinestatus":
                if (parameter == DBNull.Value)
                {
                    parameter = null;
                }

                DateTime? joinTime = (DateTime?)parameter;

                string input = "<span class=\"{0}\">{1}</span>";

                return String.Format(input, (joinTime.HasValue) ? "StatusEnabled" : "StatusDisabled", GetString(joinTime.HasValue ? "general.yes" : "general.no"));


            case "action_kick":
            case "action_revoke":
            case "action_edit":
                //Gets the value of the UserName column from the current data row
                DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                bool visible = true;
                CMSGridActionButton actionButton = (CMSGridActionButton)sender;

                // Can't perform any action in one to one support room
                if (ChatRoom.IsOneToOneSupport)
                {
                    visible = false;
                }
                else
                {
                    if (name == "action_kick")
                    {
                        if (row["ChatRoomUserJoinTime"] == DBNull.Value)
                        {
                            visible = false;
                        }
                        actionButton.IconCssClass = "icon-arrow-right-rect";
                    }
                    else if (name == "action_revoke")
                    {
                        // Can't revoke access to the creator of the room
                        // Can't revoke access to the public room
                        if (!ChatRoom.ChatRoomPrivate || ((int)row["ChatRoomUserAdminLevel"] == (int)AdminLevelEnum.Creator))
                        {
                            visible = false;
                        }
                        actionButton.IconCssClass = "icon-times-circle color-red-70";
                    }
                    else if (name == "action_edit")
                    {
                        actionButton.IconCssClass = "icon-edit";
                        actionButton.IconStyle = GridIconStyle.Allow;
                    }
                }

                if (!visible)
                {
                    actionButton.Visible = false;
                }
                else if (!HasUserModifyPermission)
                {
                    actionButton.Enabled = false;
                }

                break;
        }

        return parameter;
    }

    #endregion
}
