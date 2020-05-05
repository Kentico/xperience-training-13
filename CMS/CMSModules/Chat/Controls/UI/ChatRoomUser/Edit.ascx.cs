using System;
using System.Collections.Generic;

using CMS.Chat;

using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Chat_Controls_UI_ChatRoomUser_Edit : CMSAdminEditControl
{
    #region "Fields"

    private ChatRoomInfo chatRoom;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current room
    /// </summary>
    private ChatRoomInfo ChatRoom
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
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return this.EditForm;
        }
    }



    /// <summary>
    /// User will be inserted/edited in this room
    /// </summary>
    public int ChatRoomID { get; set; }


    /// <summary>
    /// Gets current RoomUser
    /// </summary>
    public ChatRoomUserInfo TypedEditedObject
    {
        get
        {
            return (ChatRoomUserInfo)UIContext.EditedObject;
        }
    }


    /// <summary>
    /// True if this control is editing existing object. False if new object is being created.
    /// </summary>
    public bool IsEditing
    {
        get
        {
            return (TypedEditedObject != null) && (TypedEditedObject.ChatRoomUserID > 0);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        UIFormControl.OnCheckPermissions += EditForm_OnCheckPermissions;
        UIFormControl.OnBeforeValidate += EditForm_OnBeforeValidate;
        UIFormControl.OnBeforeSave += EditForm_OnBeforeSave;

        ChatRoomInfo room = ChatRoom;

        if ((room == null) || room.IsOneToOneSupport)
        {
            RedirectToInformation(GetString("chat.error.internal"));
        }

        if (!RequestHelper.IsPostBack())
        {
            List<AdminLevelEnum> itemsToAdd = new List<AdminLevelEnum>();

            // Level can be set to None only if editing existing user in room
            if (IsEditing)
            {
                itemsToAdd.Add(AdminLevelEnum.None);
            }

            // Level can be set to Join only in private rooms
            if (room.ChatRoomPrivate)
            {
                itemsToAdd.Add(AdminLevelEnum.Join);
            }

            // Level can be set to Admin always
            itemsToAdd.Add(AdminLevelEnum.Admin);

            foreach (AdminLevelEnum enumValue in itemsToAdd)
            {
                fdrpAdminLevel.DropDownList.Items.Add(new ListItem(enumValue.ToStringValue(), ((int)enumValue).ToString()));
            }

            if (IsEditing)
            {
                fdrpAdminLevel.SelectedValue = ((int)TypedEditedObject.ChatRoomUserAdminLevel).ToString();
            }
        }


        if (IsEditing)
        {
            ChatUserInfo chatUser = ChatUserInfoProvider.GetChatUserInfo(TypedEditedObject.ChatRoomUserChatUserID);

            litChatUserLink.Text = ChatUIHelper.GetCMSDeskChatUserField(this, chatUser);

            fUserSelector.Value = chatUser.ChatUserUserID;

            litChatUserLink.Visible = true;
            fUserSelector.Visible = false;

            // Disable user selector and set ProcessDisabledFields to false, so it won't be validated
            UIFormControl.ProcessDisabledFields = false;
            fUserSelector.Enabled = false;
        }
        else
        {
            litChatUserLink.Visible = false;
            fUserSelector.Visible = true;

            if (room.ChatRoomSiteID.HasValue)
            {
                fUserSelector.SiteID = room.ChatRoomSiteID.Value;
                fUserSelector.ShowSiteFilter = false;
            }
        }
    }


    void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        int chatUserID;

        if (IsEditing)
        {
            chatUserID = TypedEditedObject.ChatRoomUserChatUserID;
        }
        else
        {
            int userID = ValidationHelper.GetInteger(fUserSelector.Value, 0);

            chatUserID = ChatUserHelper.GetChatUserFromCMSUser(UserInfoProvider.GetUserInfo(userID)).ChatUserID;
        }

        AdminLevelEnum adminLevel = ChatHelper.GetEnum(Convert.ToInt32(fdrpAdminLevel.SelectedValue), AdminLevelEnum.None);

        ChatRoomUserHelper.SetChatAdminLevel(ChatRoomID, chatUserID, adminLevel);


        URLHelper.Redirect(UrlResolver.ResolveUrl(string.Format("List.aspx?roomid={0}&saved=1", ChatRoomID)));

        // Stop processing, because save was handled manually
        UIFormControl.StopProcessing = true;
    }


    void EditForm_OnBeforeValidate(object sender, EventArgs e)
    {
        int chatUserID;

        if (IsEditing)
        {
            chatUserID = TypedEditedObject.ChatRoomUserChatUserID;
        }
        else
        {
            int userID = ValidationHelper.GetInteger(fUserSelector.Value, 0);

            UserInfo user = UserInfoProvider.GetUserInfo(userID);
            if (user == null)
            {
                ShowErrorAndStopProcessing("chat.pleaseselectuser");

                return;
            }

            if (user.IsPublic())
            {
                ShowErrorAndStopProcessing("chat.cantaddpermissionstopublicuser");

                return;
            }

            chatUserID = ChatUserHelper.GetChatUserFromCMSUser(user).ChatUserID;

            ChatRoomUserInfo chatRoomUser = ChatRoomUserInfoProvider.GetChatRoomUser(chatUserID, ChatRoomID);

            // If user with already raised privilegies is beign created
            if ((chatRoomUser != null) && (chatRoomUser.ChatRoomUserAdminLevel >= (ChatRoom.ChatRoomPrivate ? AdminLevelEnum.Join : AdminLevelEnum.Admin)))
            {
                ShowErrorAndStopProcessing("chat.errror.userhasalreadyraisedprivilegies");
                return;
            }
        }
    }


    void EditForm_OnCheckPermissions(object sender, EventArgs e)
    {
        ((CMSChatPage)Page).CheckModifyPermission(ChatRoom.ChatRoomSiteID);
    }


    /// <summary>
    /// Shows error and sets StopProcessing flag of UIFormControl to true.
    /// 
    /// Error is passed as resource string, which is resolved before displaying.
    /// </summary>
    /// <param name="resourceString">Error resource string</param>
    private void ShowErrorAndStopProcessing(string resourceString)
    {
        ShowError(GetString(resourceString));

        UIFormControl.StopProcessing = true;
    }

    #endregion
}
