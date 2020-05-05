<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ContactSelector.ascx.cs" Inherits="CMSModules_ContactManagement_FormControls_ContactSelector" %>
<%@ Register src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" tagname="UniSelector" tagprefix="cms" %>

<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" DisplayNameFormat="{%ContactFirstName%} {%ContactMiddleName%} {%ContactLastName%}"
            ObjectType="om.contact" ResourcePrefix="om.contact" ReturnColumnName="ContactID" SelectionMode="SingleTextBox" DialogWindowHeight="640"/>
    </ContentTemplate>
</cms:CMSUpdatePanel>
