<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="~/CMSWebParts/Chat/ChatWebpart.ascx.cs" Inherits="CMSWebParts_Chat_ChatWebpart" %>

<%@ Register Src="~/CMSWebParts/Chat/ChatRoomMessages.ascx" TagName="ChatRoomMessages"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatMessageSend.ascx" TagName="ChatMessageSend"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatRoomUsers.ascx" TagName="ChatRoomUsers"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatErrors.ascx" TagName="ChatErrors" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatLeaveRoom.ascx" TagName="ChatLeaveRoom"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatLogin.ascx" TagName="ChatLogin" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatNotification.ascx" TagName="ChatNotification"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatOnlineUsers.ascx" TagName="ChatOnlineUsers"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatRoomName.ascx" TagName="ChatRoomName" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatRooms.ascx" TagName="ChatRooms" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatSearchOnlineUsers.ascx" TagName="ChatSearchOnlineUsers"
    TagPrefix="cms" %>

<cms:LocalizedLabel ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel webpart-error-label" Visible="false" />
<asp:Panel runat="server" ID="pnlChatWebpart" CssClass="ChatWebpart">
    <asp:Panel runat="server" ID="pnlChatWebpartLeftCol" CssClass="ChatWebpartLeftCol">
        <cms:ChatLogin runat="server" ID="Login" InnerContainerName="GrayBoxWithHeader" InnerContainerTitle="{$chat.chatwp.headlogin$}" />
        <cms:ChatOnlineUsers runat="server" ID="OnlineUsers" Enabled="false" Visible="false" InnerContainerName="GrayBoxWithHeader" InnerContainerTitle="{$chat.chatwp.headonlineusers$}" />
        <cms:ChatSearchOnlineUsers runat="server" ID="SearchOnlineUsers" Enabled="false"
            Visible="false" InnerContainerName="GrayBoxWithHeader" InnerContainerTitle="{$chat.chatwp.headsearchonlineusers$}" />
        <cms:ChatRooms runat="server" ID="Rooms" InnerContainerName="GrayBoxWithHeader" InnerContainerTitle="{$chat.chatwp.headrooms$}" />
        <cms:ChatRoomUsers runat="server" ID="RoomUsers" InnerContainerName="GrayBoxWithHeader" InnerContainerTitle="{$chat.chatwp.headroomusers$}" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlChatWebpartRightCol" CssClass="ChatWebpartRightCol">
        <asp:Panel runat="server" ID="pnlChatWebpartHeader" CssClass="ChatWebpartHeader">
            <cms:ChatRoomName runat="server" ID="RoomName" />
            <cms:ChatLeaveRoom runat="server" ID="RoomLeave" />
            <cms:ChatNotification runat="server" ID="Notification" />
        </asp:Panel>
        <cms:ChatMessageSend runat="server" ID="RoomMessageSend" />
        <cms:ChatErrors runat="server" ID="Errors" />
        <cms:ChatRoomMessages runat="server" ID="RoomMessages" />
    </asp:Panel>
    <div style="clear: both"></div>
</asp:Panel>
