<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSWebParts/Chat/InitiatedChat.ascx.cs" Inherits="CMSWebParts_Chat_InitiatedChat" %>

<asp:Panel runat="server" ID="pnlInitiatedChat" CssClass="InitiatedChatWebpart" style="display:none">
    <asp:Panel runat="server" ID="pnlContent" CssClass="InitiatedChatContent"></asp:Panel>
    <asp:Panel runat="server" ID="pnlError" CssClass="InitiatedChatErrorPanel">
        <asp:Label runat="server" ID="lblError" CssClass="InitiatedChatError"></asp:Label>
    </asp:Panel>
</asp:Panel>

