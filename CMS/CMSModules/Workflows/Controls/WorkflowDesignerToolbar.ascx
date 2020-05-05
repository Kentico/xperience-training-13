<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="WorkflowDesignerToolbar.ascx.cs"
    Inherits="CMSModules_Workflows_Controls_WorkflowDesignerToolbar" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniMenu.ascx" TagName="UniMenu" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniMenu/UniGraphToolbar/EditGroup.ascx" TagName="EditToolbar" TagPrefix="cms" %>

<cms:CMSUpdatePanel ID="pnlAjax" runat="server" EnableViewState="false" UpdateMode="Always">
    <ContentTemplate>
        <cms:EditToolbar ID="editToolbar" ShortID="e" runat="server" />
        <cms:UniMenu ID="toolbar" ShortID="t" runat="server" AllowEmptyCaptions="true" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
