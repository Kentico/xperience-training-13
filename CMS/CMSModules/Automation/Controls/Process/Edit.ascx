<%@ Control Language="C#" AutoEventWireup="false"  Codebehind="Edit.ascx.cs" Inherits="CMSModules_Automation_Controls_Process_Edit" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/AutomationDesigner.ascx" TagName="AutomationDesigner" TagPrefix="cms" %>

<asp:Panel ID="pnlWorkflow" runat="server" CssClass="automation-readonly-designer">
    <cms:AutomationDesigner ID="ucDesigner" runat="server" />
</asp:Panel>
