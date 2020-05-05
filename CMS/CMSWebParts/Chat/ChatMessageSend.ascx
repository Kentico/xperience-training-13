<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Chat/ChatMessageSend.ascx.cs"
  Inherits="CMSWebParts_Chat_ChatMessageSend" %>
<asp:Panel runat="server" ID="pnlWebpartContent" CssClass="ChatWebpartContainer ChatWebpartContainerMessageSend EnabledWebpart">

  <div class="ChatSendAreaButton">

    <div>
      <cms:CMSTextArea runat="server" ID="txtMessage" />
      <cms:BBEditor ID="ucBBEditor" runat="server" />
    </div>
    <div>
      <asp:PlaceHolder runat="server" ID="plcCannedResponses" Visible="False">
        <cms:LocalizedHyperlink runat="server" ID="btnCannedResponses" CssClass="ChatCannedResponses"
          UseSubmitBehavior="false" ResourceString="chat.cannedresponses" href="#" />
        <asp:HyperLink runat="server" Target="_blank" ID="lnkCannedRespHelp" CssClass="ChatCannedResponsesHelpLink">
        <cms:LocalizedLabel runat="server" CssClass="sr-only" ResourceString="chat.cannedresponses.helplabel"></cms:LocalizedLabel>
        <i aria-hidden="true" class="ChatCannedResponsesHelpIcon icon-question-circle"></i>
        </asp:HyperLink>
      </asp:PlaceHolder>
      <asp:Panel runat="server" ID="pnlRecipientContainer" CssClass="ChatWebpartRecipientConatiner">
        <cms:CMSCheckBox runat="server" ID="chbWhisper" />
        <span class="ChatDrpRecipient">
          <cms:CMSDropDownList runat="server" ID="drpRecipient" />
        </span>
      </asp:Panel>
      <cms:LocalizedButton runat="server" ID="btnSendMessage" ButtonStyle="Primary" UseSubmitBehavior="true"
        EnableViewState="false" ResourceString="chat.sendmessage" Visible="true" />
      <div class="ChatClearBoth"></div>
    </div>
  </div>

  <%-- Information panel --%>
  <asp:Panel runat="server" ID="pnlChatMessageSendInfoDialog" CssClass="ChatDialogBody ChatMessageSendInformDialogBody">
    <div class="ChatDialogHeader">
      <asp:Image ID="imgInformationDialog" runat="server" />
      <cms:LocalizedLabel runat="server" ID="lblChatMessageSendInformDialogHeader" SetWindowTitle="false" ResourceString="chat.messages.informationdialog" />
    </div>
    <div class="ChatDialogContent ChatMessageSendInformDialogContent">
      <cms:LocalizedLabel runat="server" ID="lblChatMessageSendInformDialog" ResourceString="chat.messages.cannotsendemptymessage" /><br />
    </div>
    <div class="ChatDialogFooter">
      <div class="ChatDialogFooterButtons">
        <cms:LocalizedButton runat="server" ID="btnChatMessageSendInformDialogClose" UseSubmitBehavior="true"
          ButtonStyle="Primary" ResourceString="general.close" />
      </div>
    </div>
  </asp:Panel>
</asp:Panel>
