<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Controls_LiveControls_Subscription"  Codebehind="Subscription.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" tagname="HeaderActions" tagprefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Subscriptions/SubscriptionEdit.ascx" TagName="SubscriptionEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Forums/Controls/Subscriptions/SubscriptionList.ascx" TagName="SubscriptionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:PlaceHolder ID="plcList" runat="server">
    <cms:HeaderActions ID="actionsElem" runat="server"  />
    <cms:SubscriptionList ID="subscriptionList" runat="server" />
</asp:PlaceHolder>
<asp:Placeholder ID="plcBreadcrumbs" runat="server">
    <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
    <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
</asp:Placeholder>
<asp:PlaceHolder ID="plcEdit" runat="server" Visible="false">
    <cms:SubscriptionEdit ID="subscriptionEdit" runat="server" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcNew" runat="server" Visible="false">
    <cms:SubscriptionEdit ID="subscriptionNew" runat="server" />
</asp:PlaceHolder>
