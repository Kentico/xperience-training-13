<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Inherits="CMSModules_Chat_Pages_ChatSupportWindow_ChatRoomWindow" Theme="Default"  Codebehind="ChatRoomWindow.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSWebParts/Chat/ChatRoomMessages.ascx" TagName="ChatRoomMessages" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatMessageSend.ascx" TagName="ChatMessageSend" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatRoomUsers.ascx" TagName="ChatRoomUsers" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Chat/ChatErrors.ascx" TagName="ChatErrors" TagPrefix="cms" %>


<asp:Content ContentPlaceHolderID="plcContent" runat="server" ID="plcContent">
    <div class="SupportChatWindow">
        <div class="dialog-header non-selectable">
            <h2 class="dialog-header-title"><asp:Literal ID="lblTitle" runat="server" EnableViewState="false" /></h2>
        </div>
        <asp:Panel ID="pnlChatRoomWindow" CssClass="ChatRoomWindow ChatPopupWindow IsOneToOne IsSupport" runat="server">
            <asp:Label runat="server" ID="lblError" Visible="false" CssClass="ErrorLabel"></asp:Label>
            <%-- Chat webparts, this window is opened from Desk only --%>
            <asp:Panel runat="server" ID="pnlBody" CssClass="ChatBody">
                <asp:Panel runat="server" ID="pnlTop" CssClass="ChatTop">
                    <cms:ChatRoomUsers ID="ChatRoomUsersElem" runat="server" />
                    <div class="ChatTopControls">
                        <asp:Panel runat="server" ID="pnlSupportSendMail" Visible="false" CssClass="FloatLeft">
                            <cms:LocalizedHyperlink runat="server" ID="hplSupportSendMail" ResourceString="chat.support.opensupportmaildialog"></cms:LocalizedHyperlink>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlFooter" CssClass="FloatRight">
                            <cms:LocalizedButton ID="btnCloseWindow" runat="server" UseSubmitBehavior="true" ButtonStyle="Primary" ResourceString="chat.closewindow" />
                        </asp:Panel>
                    </div>
                </asp:Panel>
                <cms:ChatRoomMessages ID="ChatRoomMessagesElem" runat="server" />
                <asp:Panel runat="server" ID="pnlBottom" CssClass="ChatBottom">
                    <cms:ChatErrors ID="ChatErrorsElem" runat="server" />
                    <cms:ChatMessageSend ID="ChatMessageSendElem" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </div>
</asp:Content>

