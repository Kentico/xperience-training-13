<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Workflows_Workflow_Step_Security" Theme="Default"  Codebehind="Workflow_Step_Security.aspx.cs" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/Security.ascx"
    TagName="StepSecurity" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:StepSecurity ID="ucSecurity" runat="server" />
</asp:Content>
