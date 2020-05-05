<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/LiveSite/Dialogs/ModalSimplePage.master"
    Inherits="CMSModules_Chat_CMSPages_ChatRoomWindow" Theme="Default"  Codebehind="ChatRoomWindow.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSWebParts/Chat/ChatRoomMessages.ascx" TagName="ChatRoomMessages"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatMessageSend.ascx" TagName="ChatMessageSend"
    TagPrefix="cms" %> 
<%@ Register Src="~/CMSWebParts/Chat/ChatRoomUsers.ascx" TagName="ChatRoomUsers"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatErrors.ascx" TagName="ChatErrors" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
    

<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="plcContent">
<asp:Panel ID="pnlChatRoomWindow" CssClass="ChatRoomWindow" runat="server">
    <asp:Label runat="server" ID="lblError" Visible="false" CssClass="ErrorLabel"></asp:Label>
    <%-- Password prompt --%>
    <asp:Panel runat="server" ID="pnlChatRoomPasswordPrompt" CssClass="ModalPopupDialog"
        Style="display: none;">
        <div class="DialogPageBody">
            <div class="PageHeader">
                <cms:PageTitle runat="server" ID="passwordPromptElem" />
            </div>
            <div class="DialogPageContent">
                <cms:LocalizedLabel runat="server" ID="lblChatRoomPasswordPromptEnterPassword" ResourceString="chat.rooms.enterpassword" /><br />
                <cms:CMSTextBox runat="server" ID="txtChatRoomPasswordPromptInput" TextMode="Password" /><br />
                <asp:Panel runat="server" ID="pnlChatRoomsPromptPasswordError" CssClass="ChatError"></asp:Panel>
            </div>
            <div class="PageFooterLine">
                
                <div class="Buttons">
                    <cms:LocalizedButton runat="server" ID="btnChatRoomPasswordPromptSubmit" UseSubmitBehavior="true"
                        ButtonStyle="Primary" ResourceString="chat.ok" />
                </div>
                <div class="ClearBoth">
                    &nbsp;</div>
            </div>
        </div>
    </asp:Panel>
    <%-- Chat webparts --%>
    <div class="ChatBody">
        <cms:ChatErrors ID="ChatErrorsElem" runat="server" />
        <cms:ChatRoomUsers ID="ChatRoomUsersElem" runat="server" />
        <cms:ChatMessageSend ID="ChatMessageSendElem" runat="server" />
        <cms:ChatRoomMessages ID="ChatRoomMessagesElem" runat="server" />
    </div>
    <asp:Panel runat="server" ID="pnlFooterContainer" Visible="true">
        <div id="divFooter" class="PageFooterLine" >
        
            <asp:Panel runat="server" ID="pnlSupportSendMail" Visible="false" CssClass="FloatLeft">
                <cms:LocalizedHyperlink runat="server" ID="hplSupportSendMail" ResourceString="chat.support.opensupportmaildialog"></cms:LocalizedHyperlink>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlFooter" CssClass="FloatRight">
                <cms:LocalizedButton ID="btnCloseWindow" runat="server" UseSubmitBehavior="true" ButtonStyle="Primary" ResourceString="chat.closewindow"/>
            </asp:Panel>
        </div>
    </asp:Panel>

</asp:Panel>
</asp:Content>

