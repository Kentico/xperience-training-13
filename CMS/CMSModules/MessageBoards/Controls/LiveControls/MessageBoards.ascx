<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_LiveControls_MessageBoards"
     Codebehind="MessageBoards.ascx.cs" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/LiveControls/Boards.ascx" TagName="Boards"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Messages/MessageList.ascx"
    TagName="Messages" TagPrefix="cms" %>

<cms:BasicTabControl ID="tabElem" runat="server" TabControlLayout="Horizontal" UseClientScript="true"
    UsePostback="true" />
<asp:PlaceHolder ID="tabMessages" runat="server">
    <asp:Panel ID="plcMessages" CssClass="TabBody" runat="server">
        <cms:Messages ID="messages" runat="server" />
    </asp:Panel>
</asp:PlaceHolder>
<asp:PlaceHolder ID="tabBoards" runat="server" Visible="false">
    <asp:Panel ID="plcBoards" CssClass="TabBody" runat="server">
        <cms:Boards ID="boards" runat="server" />
    </asp:Panel>
</asp:PlaceHolder>
