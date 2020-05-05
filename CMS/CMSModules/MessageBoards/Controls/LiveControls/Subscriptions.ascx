<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_MessageBoards_Controls_LiveControls_Subscriptions"  Codebehind="Subscriptions.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" tagname="HeaderActions" tagprefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardSubscription.ascx" TagName="BoardSubscriptionEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/MessageBoards/Controls/Boards/BoardSubscriptions.ascx" TagName="BoardSubscriptionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:PlaceHolder ID="plcSubscriptionList" runat="server">
    <asp:Panel ID="pnlSubscriptionActions" runat="server" CssClass="PageHeaderLine">
        <cms:HeaderActions ID="newSubscription" runat="server" />
    </asp:Panel>
    <cms:BoardSubscriptionList ID="boardSubscriptionList" runat="server" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcEditSubscriptions" runat="server" Visible="false">
    <asp:Panel ID="pnlEditHeader" runat="server" CssClass="Actions" EnableViewState="false">
        <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" EnableViewState="false" HideBreadcrumbs="false" PropagateToMainNavigation="false" />
        <asp:LinkButton ID="lnkBackHidden" runat="server" EnableViewState="false" CausesValidation="false" />
    </asp:Panel>
    <cms:BoardSubscriptionEdit ID="boardSubscriptionEdit" runat="server" />
</asp:PlaceHolder>
