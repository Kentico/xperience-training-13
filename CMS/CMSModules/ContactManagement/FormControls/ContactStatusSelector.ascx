<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="ContactStatusSelector.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_ContactStatusSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniselector" runat="server" IsLiveSite="false" ObjectType="om.contactstatus"
            SelectionMode="SingleDropDownList" AllowEmpty="true" AllowAll="true" ResourcePrefix="om.contactstatus"
            ReturnColumnName="ContactStatusID" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
