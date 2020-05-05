<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="AutomationProcessSelector.ascx.cs"
    Inherits="CMSModules_Automation_FormControls_AutomationProcessSelector" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%WorkflowDisplayName%}" IsLiveSite="false"
            ObjectType="ma.automationprocess" AllowEmpty="true" ResourcePrefix="ma.automationprocess" ReturnColumnName="WorkflowID"
            SelectionMode="SingleDropDownList" WhereCondition="(WorkflowEnabled = 1) OR (WorkflowEnabled IS NULL)" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
