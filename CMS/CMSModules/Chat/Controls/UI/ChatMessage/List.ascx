<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Chat_Controls_UI_ChatMessage_List"  Codebehind="List.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:UniGrid runat="server" ID="gridElem" ObjectType="chat.message" OrderBy="ChatMessageCreatedWhen DESC"
    Columns="ChatMessageID,ChatMessageRecipientID,ChatMessageRoomID,ChatMessageIPAddress,ChatMessageCreatedWhen,ChatMessageUserID,ChatMessageText,ChatMessageRejected,ChatMessageSystemMessageType,(SELECT ChatUserNickname FROM Chat_User WHERE ChatUserID = ChatMessageUserID) AS AuthorNickname,(SELECT CASE WHEN ChatUserUserID IS NULL THEN 1 ELSE 0 END FROM Chat_User WHERE ChatUserID = ChatMessageUserID) AS AuthorIsAnonymous"
    IsLiveSite="false" EditActionUrl="Edit.aspx?messageid={0}&roomid={1}"
    ShowObjectMenu="false" ShowActionsMenu="false" RememberStateByParam="RoomID">
    <GridActions Parameters="ChatMessageID;ChatMessageRoomID">
        <ug:Action Name="edit" Caption="$general.edit$" ExternalSourceName="edit" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="delete" Caption="$general.delete$" ExternalSourceName="delete" Confirmation="$General.ConfirmDelete$" FontIconClass="icon-bin" FontIconStyle="Critical" />
        <ug:Action Name="reject" Caption="$general.reject$" ExternalSourceName="reject" FontIconClass="icon-times-circle" FontIconStyle="Critical" />   
    </GridActions>
    <GridOptions DisplayFilter="true" />
    <GridColumns>
        <ug:Column Source="ChatMessageCreatedWhen" Caption="$chat.message.posted$" Wrap="false" />
        <ug:Column Source="##ALL##" Caption="$chat.message.author$" Wrap="false" ExternalSourceName="ChatMessageAuthor" />
        <ug:Column Source="##ALL##" Caption="$chat.message$" Wrap="false" ExternalSourceName="ChatMessageText">
            <Filter Source="ChatMessageText" Type="Text" />
        </ug:Column>
        <ug:Column source="ChatMessageRejected" ExternalSourceName="#yesno" Caption="$general.rejected$" Visible="false">
            <Filter Source="ChatMessageRejected" Type="Bool" />
        </ug:Column>
        <ug:Column Source="ChatMessageIPAddress" Caption="$chat.message.ipaddress$" Wrap="false">
            <Filter Source="ChatMessageIPAddress" Type="Text" />
        </ug:Column>
        <ug:Column source="##ALL##" ExternalSourceName="ChatMessageSystemMessageType" Caption="$chat.messagetype$" Wrap="false">
            <Filter Source="ChatMessageSystemMessageType" Path="~/CMSModules/Chat/Controls/UI/ChatMessage/MessageTypeFilter.ascx" />
        </ug:Column>
        <ug:Column CssClass="filling-column" />
    </GridColumns>
</cms:UniGrid>
