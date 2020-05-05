<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="List.ascx.cs" Inherits="CMSModules_Chat_Controls_UI_ChatRoomUser_List" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<div style="margin-bottom: 20px;">
    <cms:LocalizedLabel ID="lblDisplayedUsers" runat="server" ResourceString="chat.chatroomusersnote"></cms:LocalizedLabel>
</div>

<cms:UniGrid runat="server" ID="gridElem" ObjectType="chat.roomuser"
    Columns="ChatRoomUserID, ChatRoomUserRoomID, ChatRoomUserChatUserID, ChatRoomUserJoinTime, ChatRoomUserAdminLevel" IsLiveSite="false" ShowObjectMenu="false"
    OrderBy="ChatRoomUserAdminLevel DESC, ChatRoomUserJoinTime DESC" RememberStateByParam="">
    <GridActions Parameters="ChatRoomUserID">
        <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="Allow" CommandArgument="ChatRoomUserID" ExternalSourceName="action_edit" />
        <ug:Action Name="kick" Caption="$chat.kick$" FontIconClass="icon-arrow-right-rect" CommandArgument="ChatRoomUserID" ExternalSourceName="action_kick" />
        <ug:Action Name="revoke" Caption="$chat.kickuserperm$" FontIconClass="icon-times-circle" FontIconStyle="Critical" CommandArgument="ChatRoomUserID" ExternalSourceName="action_revoke" />
    </GridActions>
    <GridOptions DisplayFilter="false" />
    
    <GridColumns>
        <ug:Column Source="ChatRoomUserChatUserID" ExternalSourceName="ChatRoomUserChatUserID" Caption="$chat.user.nickname$" Wrap="false" /> 
        <ug:Column Source="ChatRoomUserAdminLevel" ExternalSourceName="AdminLevel" Caption="chat.adminlevel"></ug:Column>
        <ug:Column Source="ChatRoomUserJoinTime" ExternalSourceName="OnlineStatus" Caption="chat.online"></ug:Column>
        <ug:Column CssClass="filling-column" />
    </GridColumns>
</cms:UniGrid>
