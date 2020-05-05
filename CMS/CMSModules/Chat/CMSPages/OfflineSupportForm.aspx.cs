using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CMS.Chat;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Chat_CMSPages_OfflineSupportForm : CMSLiveModalPage
{
    public CMSModules_Chat_CMSPages_OfflineSupportForm()
    {
        Init += new EventHandler(CMSModules_Chat_CMSPages_OfflineSupportForm_Init);
    }


    // Init event
    void CMSModules_Chat_CMSPages_OfflineSupportForm_Init(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            int roomID = QueryHelper.GetInteger("roomid", 0);

            try
            {
                if (ChatUserHelper.CheckJoinRoomRights(roomID))
                {
                    ChatRoomUserInfo chatRoomUser = ChatRoomUserInfoProvider.GetChatRoomUser(ChatOnlineUserHelper.GetLoggedInChatUser().ChatUserID, roomID);
                    
                    IEnumerable<MessageData> messages = ChatMessageInfoProvider.GetLatestMessages(roomID, 300, chatRoomUser.ChatRoomUserJoinTime);

                    StringBuilder sb = new StringBuilder();

                    // Iterate backwards over the messages, take only non-system messages
                    foreach (MessageData message in messages.Where(m => !m.SystemMessageType.IsSystemMessage()).Reverse())
                    {
                        sb.AppendFormat("{0} {1}: {2}", message.LastModified, message.Nickname, message.MessageText);
                        sb.NewLine();
                    }

                    messageEditElem.MessageText = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                Service.Resolve<IEventLogService>().LogException("CMSModules_Chat_CMSPages_OfflineSupportForm", "GET ROOM MESSAGES", ex);
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;

        title.TitleText = GetString("chat.support.offlineform.title");

        if (!RequestHelper.IsPostBack())
        {
            if (!ChatSettingsProvider.IsSupportMailEnabledAndValid)
            {
                lblStatusMessage.ResourceString = "chat.support.mailformnotavailiable";
                ShowHideControls(true);
            }
            else
            {
                ShowHideControls(false);
            }
        }
    }


    void SendMessage(DateTime dateTimeCreated, string email, string subject, string message)
    {
        if (!ChatSettingsProvider.IsSupportMailEnabledAndValid)
        {
            lblStatusMessage.ResourceString = "chat.support.mailformnotavailiable";
        }
        else if (SupportOfflineMessageHelper.SendSupportMessageAsMail(dateTimeCreated, email, subject, message))
        {
            lblStatusMessage.ResourceString = "chat.support.messagewassent";
        }
        else
        {
            lblStatusMessage.ResourceString = "chat.support.tryagainlater";
        }
        
        ShowHideControls(true);
   }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (messageEditElem.Validate())
        {
            SendMessage(DateTime.Now, messageEditElem.Sender, messageEditElem.Subject, messageEditElem.MessageText);
        }
    }


    private void ShowHideControls(bool wasSubmitted)
    {
        messageEditElem.Visible = !wasSubmitted;
        lblStatusMessage.Visible = wasSubmitted;

        btnSubmit.Visible = !wasSubmitted;
        btnClose.Visible = wasSubmitted;
    }
}