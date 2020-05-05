<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_FormControls_Roles_SelectRole"  Codebehind="SelectRole.ascx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ObjectType="cms.role" SelectionMode="MultipleTextBox"
            OrderBy="RoleDisplayName" ResourcePrefix="roleselect" runat="server"
            ID="usRoles" ShortID="s" AllowEditTextBox="true" AddGlobalObjectSuffix="true" AllowEmpty="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
