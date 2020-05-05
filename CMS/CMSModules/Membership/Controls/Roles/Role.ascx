<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Controls_Roles_Role"
     Codebehind="Role.ascx.cs" %>

<%@ Register Src="RoleEdit.ascx" TagName="RoleEdit" TagPrefix="cms" %>
<%@ Register Src="RoleUsers.ascx" TagName="RoleUsers" TagPrefix="cms" %>

<asp:Literal runat="server" ID="ltlScript"></asp:Literal>
<asp:Panel runat="server" ID="pnlBody" CssClass="PollEdit">
    <div class="TabsHeader">
        <cms:BasicTabControl ID="tabMenu" runat="server" />
    </div>
    <asp:Panel runat="server" ID="pnlRoleLinks" CssClass="TabsContent">
        <div>
            <cms:RoleEdit ID="RoleEdit" runat="server" />
            <cms:RoleUsers ID="RoleUsers" runat="server" />
        </div>
    </asp:Panel>
    <asp:Button ID="btnUpdate" runat="server" Text="Button" CssClass="HiddenButton"
        OnClick="btnUpdate_Click" />
</asp:Panel>