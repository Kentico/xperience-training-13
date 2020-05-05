<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Chat/ChatOnlineUsers.ascx.cs" Inherits="CMSWebParts_Chat_ChatOnlineUsers" %>

<asp:Panel runat="server" CssClass="ChatWebpartContainer ChatWebpartContainerOnlineUsers" ID="pnlOnlineUsersWP">
<div class="ChatWebpartFilterPaging">
    <asp:Panel ID="pnlChatOnlineUsersFiltering" runat="server" Visible="false">
         <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <cms:CMSTextBox ID="txtChatOnlineUsersFilter" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:LocalizedButton ID="btnChatOnlineUsersFilter" runat="server" ResourceString="chat.onlineusers.filterbtn" ButtonStyle="Default" UseSubmitBehavior="true" />
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlChatOnlineUsersInfo" runat="server" CssClass="chat-filtered-users-results"></asp:Panel>
    </asp:Panel>
    <asp:Panel ID="pnlChatOnlineUsersInvite" runat="server" Visible="false">
            <cms:LocalizedLabel runat="server" ID="lblChatRoomUsersInvitePromptSelect" EnableViewState="false" ResourceString="chat.invite.select" />
    </asp:Panel>
    <asp:Panel ID="pnlChatOnlineUsersPaging" runat="server"></asp:Panel>
</div>
<%-- Panel with the list of online users --%>
<asp:Panel ID="pnlChatOnlineUsers" runat="server"></asp:Panel>

</asp:Panel>