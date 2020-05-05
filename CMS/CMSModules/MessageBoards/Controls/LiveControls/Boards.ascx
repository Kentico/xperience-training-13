<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Controls_LiveControls_Boards"  Codebehind="Boards.ascx.cs" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardList.ascx" TagName="BoardList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardEdit.ascx" TagName="BoardEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardModerators.ascx"
    TagName="BoardModerators" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardSecurity.ascx"
    TagName="BoardSecurity" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/LiveControls/Subscriptions.ascx"
    TagName="BoardSubscriptions" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Messages/MessageList.ascx"
    TagName="BoardMessages" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:PlaceHolder ID="plcTabsHeader" runat="server" Visible="false">
    <asp:Panel ID="pnlEditActions" runat="server" EnableViewState="false">
        <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" EnableViewState="false" HideBreadcrumbs="false" PropagateToMainNavigation="false" />
        <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
    </asp:Panel>
    <cms:BasicTabControl ID="tabElem" runat="server" UseClientScript="true" UsePostback="true" />
</asp:PlaceHolder>
<asp:Panel runat="server" ID="pnlContent" CssClass="TabBody">
    <asp:Literal ID="ltlUpdateFormScript" runat="server" EnableViewState="false" />
    <asp:PlaceHolder ID="plcList" runat="server">
        <cms:BoardList ID="boardList" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcTabs" runat="server" Visible="false">
        <asp:PlaceHolder ID="tabMessages" runat="server">
            <cms:BoardMessages ID="boardMessages" runat="server" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="tabEdit" runat="server" Visible="false">
            <cms:BoardEdit ID="boardEdit" runat="server" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="tabModerators" runat="server" Visible="false">
            <cms:BoardModerators ID="boardModerators" runat="server" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="tabSecurity" runat="server" Visible="false">
            <cms:BoardSecurity ID="boardSecurity" runat="server" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="tabSubscriptions" runat="server" Visible="false">
            <cms:BoardSubscriptions runat="server" ID="boardSubscriptions" />
        </asp:PlaceHolder>
    </asp:PlaceHolder>
</asp:Panel>
