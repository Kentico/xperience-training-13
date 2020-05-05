using System;

using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSModules_Chat_CMSPages_ChatRoomWindow : CMSLiveModalPage
{
    #region "Private fields"

    private int PopupSettingsId
    {
        get
        {
            return QueryHelper.GetInteger("popupSettingsId", 0);
        }
    }


    #endregion


    #region "Private methods"

    private void SetWebpartsUp()
    {
        // Load parent webpart settings stored in DB.
        ChatPopupWindowSettingsInfo settings = ChatPopupWindowSettingsHelper.GetPopupWindowSettings(PopupSettingsId);

        // If no settings are stored use default values.
        if (settings == null)
        {
            settings = new ChatPopupWindowSettingsInfo();
            settings.MessageTransformationName = ChatSettingsProvider.TransformationRoomMessages;
            settings.UserTransformationName = ChatSettingsProvider.TransformationRoomUsers;
            settings.ErrorTransformationName = ChatSettingsProvider.TransformationErrors;
            settings.ErrorClearTransformationName = ChatSettingsProvider.TransformationErrorsDeleteAll;
        }
        else
        {
            if (string.IsNullOrEmpty(settings.MessageTransformationName))
            {
                settings.MessageTransformationName = ChatSettingsProvider.TransformationRoomMessages;
            }
            if (string.IsNullOrEmpty(settings.UserTransformationName))
            {
                settings.UserTransformationName = ChatSettingsProvider.TransformationRoomUsers;
            }
            if (string.IsNullOrEmpty(settings.ErrorTransformationName))
            {
                settings.ErrorTransformationName = ChatSettingsProvider.TransformationErrors;
            }
            if (string.IsNullOrEmpty(settings.ErrorClearTransformationName))
            {
                settings.ErrorClearTransformationName = ChatSettingsProvider.TransformationErrorsDeleteAll;
            }
        }

        // Set errors webpart up.
        ChatErrorsElem.ErrorTransformationName = settings.ErrorTransformationName;
        ChatErrorsElem.ButtonDeleteAllTransformationName = settings.ErrorClearTransformationName;
        ChatErrorsElem.ShowDeleteAllBtn = true;

        // Set messages webpart up.
        ChatRoomMessagesElem.ChatMessageTransformationName = settings.MessageTransformationName;
        ChatRoomMessagesElem.Direction = ChatRoomMessagesDirectionEnum.Down;
        ChatRoomMessagesElem.Count = ChatSettingsProvider.FirstLoadMessagesCountSetting;

        // Set users webpart up.
        ChatRoomUsersElem.ChatUserTransformationName = settings.UserTransformationName;
        ChatRoomUsersElem.EnablePaging = true;
        ChatRoomUsersElem.PagingItems = 5;
        ChatRoomUsersElem.GroupPagesBy = 10;
        ChatRoomUsersElem.EnableFiltering = true;
        ChatRoomUsersElem.ShowFilterItems = 6;
        ChatRoomUsersElem.SortByStatus = true;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;

        passwordPromptElem.TitleText = GetString("chat.password");

        // Script references insertion
        ScriptHelper.RegisterJQuery(Page);
        ChatScriptHelper.RegisterChatNotificationManager(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/jquery/jquery-resize.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Chat/CMSPages/Scripts/ChatRoomWindow.js");

        // Get information about chat room this window has been opened for.
        int roomID = QueryHelper.GetInteger("windowroomid", 0);
        try
        {
            ChatUserHelper.VerifyChatUserHasJoinRoomRights(roomID);
        }
        catch(ChatServiceException)
        {
            DisplayError();

            return;
        }

        ChatRoomInfo room = ChatRoomInfoProvider.GetChatRoomInfo(roomID);
        if (room == null)
        {
            DisplayError();

            return;
        }

        string pnlChatRoomWindowCssClass = " ChatPopupWindow";

        // Disable irrelevant controls
        if (room.ChatRoomIsOneToOne)
        {
            ChatMessageSendElem.HideUserPicker();
            pnlChatRoomWindowCssClass += " IsOneToOne";

            // Set appropriate window title
            string pageTitle = GetString(room.ChatRoomIsSupport ? "chat.title.support" : "chat.title.privateconversation");
            Page.Header.Title = title.TitleText = pageTitle;
        }
        else
        {
            Page.Header.Title = title.TitleText = room.ChatRoomDisplayName;
        }

        SetWebpartsUp();

        if (room.ChatRoomIsSupport)
        {
            pnlChatRoomWindowCssClass += " IsSupport";
            if (ChatSettingsProvider.IsSupportMailEnabledAndValid)
            {
                pnlSupportSendMail.Visible = true;
                hplSupportSendMail.NavigateUrl = ChatSettingsProvider.SupportMailDialogURL + "?roomid=" + roomID;
                hplSupportSendMail.Target = "_blank";
            }
        }
        pnlChatRoomWindow.CssClass += pnlChatRoomWindowCssClass;
        string json = JsonConvert.SerializeObject(
            new
            {
                roomID = roomID,
                password = room.HasPassword,
                pnlChatRoomPasswordPrompt = '#' + pnlChatRoomPasswordPrompt.ClientID,
                txtChatRoomPasswordPromptInput = '#' + txtChatRoomPasswordPromptInput.ClientID,
                btnChatRoomPasswordPromptSubmit = '#' + btnChatRoomPasswordPromptSubmit.ClientID,
                isOneToOne = room.IsWhisperRoom,
                isCustomerSupport = room.ChatRoomIsSupport,
                hplSupportSendMailClientID = room.ChatRoomIsSupport ? '#' + hplSupportSendMail.ClientID : "",
                pnlPasswordPromptError = '#' + pnlChatRoomsPromptPasswordError.ClientID,
                pnlChatRoomWindow = '#' + pnlChatRoomWindow.ClientID,
                ChatRoomMessagesClientID = ChatRoomMessagesElem.ClientID,
                btnClose = "#" + btnCloseWindow.ClientID,
                notificationManagerOptions = new
                    {
                        eventName = "newmessage",
                        soundFile = ChatSettingsProvider.EnableSoundLiveChat ? ResolveUrl("~/CMSModules/Chat/CMSPages/Sound/Chat_message.mp3") : String.Empty,
                        notifyTitle = GetString("chat.general.newmessages")
                    }
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );


        string startupScript = String.Format("ChatRoomWindow({0});", json);

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatRoomWindow_" + ClientID, startupScript, true);
    }


    private void DisplayError(string message = null)
    {
        lblError.Text = GetString(message ?? "chat.error.window.badroomid");
        lblError.Visible = true;
    }

    #endregion
}
