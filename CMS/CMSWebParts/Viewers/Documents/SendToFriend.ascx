<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Viewers_Documents_SendToFriend"
     Codebehind="~/CMSWebParts/Viewers/Documents/SendToFriend.ascx.cs" %>
<cms:CMSRepeater ID="repItems" runat="server" EnableViewState="false" StopProcessing="true" />
<asp:Label ID="lblHeader" runat="server" CssClass="sendToFriendHeader" EnableViewState="false" />
<asp:Panel ID="pnlSendToFriend" runat="server" DefaultButton="btnSend" CssClass="sendToFriendPanel">
    <div style="padding-bottom: 3px;">
        <asp:Label ID="lblInfo" runat="server" EnableViewState="false" />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" EnableViewState="false" />
    </div>
    <asp:PlaceHolder ID="plcForm" runat="server">
        <div>
            <asp:Label ID="lblEmailTo" runat="server" EnableViewState="false" />
            <cms:CMSTextBox ID="txtEmailTo" runat="server" CssClass="send-to-friend-textbox"/>
            <cms:CMSButton ID="btnSend" runat="server" OnClick="btnSend_Click" ValidationGroup="sendToFriend"
                EnableViewState="false" ButtonStyle="Default" />
            <cms:CMSRequiredFieldValidator ID="rfvEmailTo" runat="server" ControlToValidate="txtEmailTo"
                ValidationGroup="sendToFriend" Display="Dynamic" EnableViewState="false" />
        </div>
        <asp:Label ID="lblYourMessage" runat="server" CssClass="sendToFriendYourMessage"
            EnableViewState="false" />
        <asp:Panel ID="pnlMessageText" runat="server">
            <asp:Label ID="lblMessageText" runat="server" EnableViewState="false" />
            <cms:CMSTextArea ID="txtMessageText" runat="server" Rows="7" />
        </asp:Panel>
    </asp:PlaceHolder>
</asp:Panel>
