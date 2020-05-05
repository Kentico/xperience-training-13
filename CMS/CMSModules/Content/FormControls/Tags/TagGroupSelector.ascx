<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_FormControls_Tags_TagGroupSelector"  Codebehind="TagGroupSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" SelectionMode="SingleTextBox" AllowEditTextBox="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
