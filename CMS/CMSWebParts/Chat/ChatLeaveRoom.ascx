<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Chat/ChatLeaveRoom.ascx.cs"
    Inherits="CMSWebParts_Chat_ChatLeaveRoom" %>
<asp:Panel runat="server" ID="pnlChatLeaveRoom" CssClass="ChatWebpartContainer ChatWebpartContainerLeaveRoom EnabledWebpart">
    <cms:LocalizedButton runat="server" ID="btnChatLeaveRoom" ButtonStyle="Primary"
        EnableViewState="false" ResourceString="chat.leaveroom" />
</asp:Panel>
