<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ContactRoleSelector.ascx.cs" Inherits="CMSModules_ContactManagement_FormControls_ContactRoleSelector" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" DisplayNameFormat="{%ContactRoleDisplayName%}"
            ObjectType="om.contactrole" ResourcePrefix="om.contactrole" ReturnColumnName="ContactRoleID" SelectionMode="SingleDropDownList"/>
    </ContentTemplate>
</cms:CMSUpdatePanel>
