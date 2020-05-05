<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Chat/ChatSearchOnlineUsers.ascx.cs" Inherits="CMSWebParts_Chat_ChatSearchOnlineUsers" %>

<asp:Panel runat="server" ID="pnlChatSearchOnlineUsersWP" CssClass="ChatWebpartContainer ChatWebpartContainerOnlineUsers">
     <div class="ChatWebpartFilterPaging">
         <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <cms:CMSTextBox ID="txtChatSearchOnlineUsers" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:LocalizedButton ID="btnChatSearchOnlineUsers" runat="server" ResourceString="chat.searchonlineusers.searchbtn" ButtonStyle="Default" ></cms:LocalizedButton>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlChatSearchOnlineUsersInfo" runat="server"></asp:Panel>
        <asp:Panel ID="pnlChatSearchOnlineUsersInvite" runat="server" Visible="false">
            <cms:LocalizedLabel runat="server" ID="lblChatRoomUsersInvitePromptSelect" EnableViewState="false" ResourceString="chat.invite.select" />
        </asp:Panel>
        <asp:Panel ID="pnlChatSearchOnlineUsersPaging" runat="server"></asp:Panel>
    </div>
    <asp:Panel ID="pnlChatSearchOnlineUsers" runat="server"></asp:Panel>
</asp:Panel>