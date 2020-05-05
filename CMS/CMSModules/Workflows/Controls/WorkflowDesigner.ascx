<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="WorkflowDesigner.ascx.cs"
    Inherits="CMSModules_Workflows_Controls_WorkflowDesigner" %>
<%@ Register Src="~/CMSModules/Workflows/Controls/WorkflowDesignerToolbar.ascx" TagName="EditorToolbar" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/WebServiceChecker.ascx" TagName="WebServiceChecker" TagPrefix="cms" %>

<cms:CMSPanel ID="toolbarContainer" ShortID="tc" CssClass="ContentMenu GraphMenu"
    runat="server" FixedPosition="true">
    <cms:EditorToolbar ID="toolbar" runat="server" />
</cms:CMSPanel>
<cms:WebServiceChecker ID="serviceChecker" runat="server" />
<cms:UniGraph runat="server" ID="uniGraph" />