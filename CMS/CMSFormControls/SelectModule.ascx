<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_SelectModule"
     Codebehind="SelectModule.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%ResourceDisplayName%}"
            SelectionMode="SingleDropDownList" ObjectType="cms.resource" AllowEmpty="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
