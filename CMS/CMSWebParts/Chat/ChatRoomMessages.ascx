<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Chat_ChatRoomMessages"  Codebehind="~/CMSWebParts/Chat/ChatRoomMessages.ascx.cs" %>

<asp:Panel ID="pnlChatRoomWebpart" runat="server" CssClass="ChatWebpartContainer ChatWebpartContainerRoomMessages">

<%-- Panel with the list messages --%>
<asp:Panel ID="pnlChatRoomMessages" runat="server"></asp:Panel>

<%-- Informatino panel --%>
<asp:Panel runat="server" ID="pnlChatRoomMessagesInfoDialog" CssClass="ChatDialogBody ChatMessageInformDialogBody">
        <div class="ChatDialogHeader">
            <table>
                <tr>
                    <td><asp:Image ID="imgInformationDialog" runat="server" /></td>
                    <td><cms:LocalizedLabel runat="server" ID="lblChatMessageInformDialogHeader" SetWindowTitle="false" ResourceString="chat.messages.informationdialog" /></td>
                </tr>
            </table>
        </div>
        <div class="ChatDialogContent ChatMessageInformDialogContent">
            <cms:LocalizedLabel runat="server" ID="lblChatMessageInformDialog" ResourceString="chat.messages.userisnotonline" /><br />
        </div>
        <div class="ChatDialogFooter">
            <div class="ChatDialogFooterButtons">
                <cms:LocalizedButton runat="server" ID="btnChatMessageInformDialogClose" UseSubmitBehavior="true" 
                    ButtonStyle="Primary" ResourceString="general.close" />
            </div>
        </div>
</asp:Panel>

</asp:Panel>
