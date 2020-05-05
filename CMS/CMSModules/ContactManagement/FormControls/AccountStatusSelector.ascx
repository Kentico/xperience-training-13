<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="AccountStatusSelector.ascx.cs"
    Inherits="CMSModules_ContactManagement_FormControls_AccountStatusSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniselector" runat="server" IsLiveSite="false" ObjectType="om.accountstatus"
            SelectionMode="SingleDropDownList" AllowEmpty="true" AllowAll="true" ResourcePrefix="om.accountstatus"
            ReturnColumnName="AccountStatusID" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
