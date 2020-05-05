<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Integration_FormControls_ConnectorSelector"
     Codebehind="ConnectorSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%ConnectorDisplayName%}"
            IsLiveSite="false" ObjectType="integration.connector" AllowEmpty="false" AllowAll="true"
            ResourcePrefix="integration.connector" ReturnColumnName="ConnectorID" SelectionMode="SingleDropDownList" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
