<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Chat_FormControls_RoomSelector"
     Codebehind="RoomSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:UniSelector ID="usRooms" ShortID="s" runat="server" ObjectType="chat.room" SelectionMode="SingleTextBox"
            AllowEditTextBox="false" ReturnColumnName="ChatRoomName"/>
    </ContentTemplate>
</cms:CMSUpdatePanel>
