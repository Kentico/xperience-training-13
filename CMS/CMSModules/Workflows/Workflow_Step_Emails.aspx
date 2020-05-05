<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Workflow_Step_Emails"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" EnableEventValidation="false"
    Theme="Default" Title="Workflow Edit - Step E-mails"  Codebehind="Workflow_Step_Emails.aspx.cs" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/Emails.ascx" TagName="WorkflowStepEmails"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:WorkflowStepEmails ID="ucEmails" runat="server" />
</asp:Content>
