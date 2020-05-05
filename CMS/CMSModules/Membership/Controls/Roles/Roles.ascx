<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_Roles_Roles"
     Codebehind="Roles.ascx.cs" %>

<%@ Register Src="Role.ascx" TagName="Role" TagPrefix="cms" %>
<%@ Register Src="RoleList.ascx" TagName="RoleList" TagPrefix="cms" %>
<%@ Register Src="RoleEdit.ascx" TagName="RoleEdit" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/BreadCrumbs.ascx" TagName="Breadcrumbs" TagPrefix="cms" %>

<asp:Panel runat="server" ID="pnlBody">
    <asp:Panel runat="server" ID="headerLinks" CssClass="PageHeaderLinks">
        <div class="Actions">
            <asp:Image ID="imgNewRole" runat="server" CssClass="NewItemImage" />
            <cms:LocalizedLinkButton ID="btnNewRole" runat="server" CssClass="NewItemLink" ResourceString="Administration-Role_List.NewRole" />
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlRolesBreadcrumbs">
        <div class="Actions">
            <cms:Breadcrumbs ID="ucBreadcrumbs" runat="server" EnableViewState="false" HideBreadcrumbs="false" PropagateToMainNavigation="false" />
            <asp:LinkButton ID="lnkBackHidden" runat="server" CausesValidation="false" EnableViewState="false" />
        </div>
    </asp:Panel>
    <div>
        <cms:Role ID="Role" runat="server" />
        <cms:RoleList ID="RoleList" runat="server" />
        <cms:RoleEdit ID="RoleEdit" runat="server" />
    </div>
    <asp:Literal ID="ltlScript" runat="server" />
</asp:Panel>