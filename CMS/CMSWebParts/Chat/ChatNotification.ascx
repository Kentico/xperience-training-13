<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Chat/ChatNotification.ascx.cs" Inherits="CMSWebParts_Chat_ChatNotification" %>

<asp:Panel runat="server" ID="pnlWPNotifications" CssClass="ChatWebpartContainer ChatWebpartContainerNotification">

<%-- Panel that is displayed when there are no notifications --%>
<asp:Panel runat="server" ID="pnlChatNotificationEmpty">
    <cms:LocalizedLabel runat="server" ID="lblChatNotificationEmptyText" ResourceString="chat.notification.empty" EnableViewState="false" />
</asp:Panel>

<%-- Panel that is displayed when there exist open notifications --%>
<asp:Panel runat="server" ID="pnlChatNotificationFull" style="display: none;">
    <asp:LinkButton runat="server" ID="btnChatNotificationFullLink" EnableViewState="false">
        <cms:LocalizedLabel runat="server" ID="lblChatNotificationFullTextPre" ResourceString="chat.notification.youhave" EnableViewState="false" />
        (<asp:Label runat="server" ID="lblChatNotificationFullTextNumber" />)
    </asp:LinkButton>
</asp:Panel>

<%-- Panel with the list of notifications --%>
<asp:Panel runat="server" ID="pnlChatNotificationNotifications" CssClass="ChatDialogBody ChatNotificationsDialogBody">
        <div class="ChatDialogHeader">
            <table>
                <tr>
                    <td><asp:Image ID="imgChatRoomsPrompt" runat="server" /></td>
                    <td><cms:LocalizedLabel runat="server" ID="lblChatNotificationNotificationsHeader" SetWindowTitle="false" ResourceString="chat.notification.notificationsheader" /></td>
                </tr>
            </table>      
        </div>
        <div class="ChatDialogContent ChatNotificationsDialogContent">
            <asp:Panel runat="server" ID="pnlChatNotificationNotificationsList"></asp:Panel>
        </div>
        <div class="ChatDialogFooter">
            <div class="ChatDialogFooterButtons">
                <cms:LocalizedButton runat="server" ID="btnRemoveAllNotifications" UseSubmitBehavior="true"
                    ButtonStyle="Primary" ResourceString="chat.notification.removeall" />
                <cms:LocalizedButton runat="server" ID="btnChatNotificationPromptClose" UseSubmitBehavior="true"
                    ButtonStyle="Primary" ResourceString="general.close" />
            </div>
        </div>
</asp:Panel>

<%-- Panel with the notification bubble --%>
<asp:Panel runat="server" ID="pnlNotificationInfoBubble" CssClass="ChatNotificationBubble" style="display:none">
    <div class="ChatNotificationBubbleContent">
        <div class="ChatNotificationBubbleHeader"><cms:LocalizedLabel ID="lblHeader" runat="server" ResourceString="chat.notification.bubble.header" /></div>
        <div class="ChatNotificationBubbleMessage"><cms:LocalizedLabel ID="lblInfoMessage" runat="server" ResourceString="chat.notification.empty" /></div>
        <div class="ChatNotificationBubbleButtons">
            <asp:LinkButton runat="server" ID="btnShow" ></asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnClose" ></asp:LinkButton>
        </div>
</div>
</asp:Panel>

</asp:Panel>