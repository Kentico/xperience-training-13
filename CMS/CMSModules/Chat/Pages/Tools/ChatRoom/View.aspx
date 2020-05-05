<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="View.aspx.cs" Inherits="CMSModules_Chat_Pages_Tools_ChatRoom_View" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  %>

<%@ Register src="~/CMSWebParts/Chat/ChatRoomUsers.ascx" tagname="ChatRoomUsers" tagprefix="cms" %>
<%@ Register src="~/CMSWebParts/Chat/ChatRoomMessages.ascx" tagname="ChatRoomMessages" tagprefix="cms" %>
<%@ Register src="~/CMSWebParts/Chat/ChatMessageSend.ascx" tagname="ChatMessageSend" tagprefix="cms" %>
<%@ Register src="~/CMSWebParts/Chat/ChatErrors.ascx" tagname="ChatError" tagprefix="cms" %>
<%@ Register src="~/CMSWebParts/Chat/ChatNotification.ascx" tagname="ChatNotification" tagprefix="cms" %>
<%@ Register src="~/CMSWebParts/Chat/ChatRoomName.ascx" tagname="ChatRoomName" tagprefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
<asp:Panel style="font-family:Arial" class="ChatDeskView" runat="server" ID="pnlChatView">
    <asp:Panel runat="server" ID="pnlChatWebpartLeftCol" style="float:left; max-width:300px;" >
        <cms:ChatRoomUsers runat="server" ID="ChatRoomUsers" InnerContainerName="GrayBoxWithHeader" InnerContainerTitle="{$chat.chatwp.headroomusers$}"  />
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlChatWebpartRightCol" style="float:left; max-width:600px" >
        <asp:Panel runat="server" ID="pnlChatWebpartHeader" CssClass="ChatWebpartHeader">
            <cms:ChatRoomName runat="server" ID="RoomName"  />
            <cms:ChatNotification runat="server" ID="ChatNotification" />
        </asp:Panel>
        <cms:ChatMessageSend runat="server" ID="ChatRoomMessageSend" />
        <cms:ChatError runat="server" ID="ChatErrors" />
        <cms:ChatRoomMessages runat="server" ID="ChatRoomMessages" />
    </asp:Panel>

</asp:Panel>

</asp:Content>