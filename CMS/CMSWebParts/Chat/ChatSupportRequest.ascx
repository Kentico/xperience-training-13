<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Chat/ChatSupportRequest.ascx.cs" Inherits="CMSWebParts_Chat_ChatSupportRequest" %>

<asp:Panel ID="pnlSupportChatRequest" runat="server" CssClass="ChatWebpartContainer ChatWebpartContainerSupportRequest" />
<%-- Informatino panel --%>
<asp:Panel runat="server" ID="pnlChatSupportRequestInfoDialog" CssClass="ChatDialogBody ChatSupportRequestInformDialogBody">
        <div class="ChatDialogHeader">
            <cms:LocalizedLabel runat="server" ID="lblChatSupportRequestInformDialogHeader" ResourceString="chat.title.support" />
        </div>
        <div class="ChatDialogContent ChatSupportRequestInformDialogContent">
            <cms:LocalizedLabel runat="server" ID="lblChatSupportRequestInformDialog" ResourceString="chat.support.supportnotavailable" /><br />
        </div>
        <div class="ChatDialogFooter">
            <div class="ChatDialogFooterButtons">
                <cms:LocalizedButton runat="server" ID="btnChatSupportRequestInformDialogClose" UseSubmitBehavior="true" 
                    ButtonStyle="Primary" ResourceString="general.close" />
            </div>
        </div>
</asp:Panel>
