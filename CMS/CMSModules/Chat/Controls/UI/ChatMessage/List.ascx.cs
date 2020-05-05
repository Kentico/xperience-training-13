using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;


public partial class CMSModules_Chat_Controls_UI_ChatMessage_List : CMSAdminListControl
{
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


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            this.gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Filters displayed chat messages only to messages of this chat room.
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
        Grid.OnExternalDataBound += new OnExternalDataBoundEventHandler(Grid_OnExternalDataBound);
        Grid.OnAction += new OnActionEventHandler(Grid_OnAction);

        Grid.WhereCondition = string.Format("ChatMessageRoomID = {0}", ChatRoomID);

        HasUserModifyPermission = ((CMSChatPage)Page).HasUserModifyPermission(SiteID);
    }


    void Grid_OnAction(string actionName, object actionArgument)
    {
        ((CMSChatPage)Page).CheckModifyPermission(SiteID);


        int chatMessageID = ValidationHelper.GetInteger(actionArgument, 0);
        switch (actionName)
        {
            case "delete":
                ChatMessageInfoProvider.DeleteChatMessageInfo(chatMessageID);
                break;
            case "reject":
                ChatMessageInfo cmi = ChatMessageInfoProvider.GetChatMessageInfo(chatMessageID);
                if (cmi != null)
                {
                    if (cmi.ChatMessageRejected)
                    {
                        // Set message as 'not rejected'
                        cmi.ChatMessageRejected = false;
                    }
                    else
                    {
                        // Set message as 'rejected'
                        cmi.ChatMessageRejected = true;
                    }
                    ChatMessageInfoProvider.SetChatMessageInfo(cmi);
                }
                break;
        }
    }


    object Grid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string name = sourceName.ToLowerCSafe();
        switch (name)
        {
            case "chatmessageauthor":
                {
                    DataRowView row = (DataRowView)parameter;

                    if (row["AuthorNickname"] == DBNull.Value)
                    {
                        return "<span style=\"color: #777777; font-style: italic;\">" + GetString("chat.system") + "</span>";
                    }

                    int chatUserID = ValidationHelper.GetInteger(row["ChatMessageUserID"], 0);
                    string nickname = ValidationHelper.GetString(row["AuthorNickname"], "AuthorNickname");
                    bool isAnonymous = ValidationHelper.GetBoolean(row["AuthorIsAnonymous"], true);

                    return ChatUIHelper.GetCMSDeskChatUserField(this, chatUserID, nickname, isAnonymous);
                }
            case "edit":
            case "reject":
            case "delete":
                {
                    DataRowView row = (DataRowView)((GridViewRow)parameter).DataItem;

                    // Whisper message is consider as system here - it can't be rejected or edited
                    ChatMessageTypeEnum msgType = (ChatMessageTypeEnum)ValidationHelper.GetInteger(row["ChatMessageSystemMessageType"], 0);
                    bool isSystem = ((msgType != ChatMessageTypeEnum.ClassicMessage) && (msgType != ChatMessageTypeEnum.Announcement));

                    bool enabled = true;
                    var actionButton = (CMSGridActionButton)sender;

                    if (isSystem)
                    {
                        if (name == "edit" || name == "reject")
                        {
                            // Disable edit and reject buttons for system messages
                            enabled = false;
                        }
                    }
                    else
                    {
                        if (name == "reject")
                        {
                            bool isRejected = ValidationHelper.GetBoolean(row["ChatMessageRejected"], false);
                            if (isRejected)
                            {
                                actionButton.IconCssClass = "icon-check-circle";
                                actionButton.IconStyle = GridIconStyle.Allow;
                                actionButton.ToolTip = GetString("general.approve");
                            }
                        }
                    }

                    if (!HasUserModifyPermission && name != "edit")
                    {
                        enabled = false;
                    }

                    actionButton.Enabled = enabled;

                    break;
                }
            case "chatmessagesystemmessagetype":
                {
                    DataRowView row = (DataRowView)parameter;

                    ChatMessageTypeEnum messageType = (ChatMessageTypeEnum)ValidationHelper.GetInteger(row["ChatMessageSystemMessageType"], 0);

                    if (messageType == ChatMessageTypeEnum.Whisper)
                    {
                        ChatUserInfo recipient = ChatUserInfoProvider.GetChatUserInfo(ValidationHelper.GetInteger(row["ChatMessageRecipientID"], 0));

                        if (recipient != null)
                        {
                            // Set text to the format "Whisper to somebody", where somebody may be link to the user if he is not anonymous

                            return String.Format(ResHelper.GetString("chat.system.cmsdesk.whisperto"), ChatUIHelper.GetCMSDeskChatUserField(this, recipient));
                        }
                    }

                    return messageType.ToStringValue((int)ChatMessageTypeStringValueUsageEnum.CMSDeskDescription);
                }
            case "chatmessagetext":
                {
                    DataRowView row = (DataRowView)parameter;

                    ChatMessageTypeEnum messageType = (ChatMessageTypeEnum)ValidationHelper.GetInteger(row["ChatMessageSystemMessageType"], 0);

                    string messageText = ValidationHelper.GetString(row["ChatMessageText"], "");

                    if (messageType.IsSystemMessage())
                    {
                        messageText = MacroResolver.Resolve(messageText);
                    }

                    return HTMLHelper.HTMLEncode(messageText);
                }
        }

        return parameter;
    }

    #endregion
}