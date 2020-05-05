<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Controls_Members_Members"
     Codebehind="Members.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Groups/Controls/Members/MemberEdit.ascx" TagName="MemberEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Groups/Controls/Members/MemberList.ascx" TagName="MemberList"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
    Visible="false" />
<asp:PlaceHolder ID="plcList" runat="server">
    <cms:HeaderActions ID="actionsElem" runat="server" />
    <cms:MemberList ID="memberListElem" runat="server" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plcEdit" runat="server" Visible="false">
    <asp:Panel ID="pnlEditActions" runat="server">
        <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" HideBreadcrumbs="false" EnableViewState="false" PropagateToMainNavigation="false" />
        <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
    </asp:Panel>
    <cms:MemberEdit ID="memberEditElem" runat="server" />
</asp:PlaceHolder>
<asp:Literal ID="ltlScript" runat="server" />