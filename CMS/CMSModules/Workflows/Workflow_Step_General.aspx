<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Workflows_Workflow_Step_General" Theme="Default" Title="Workflows - Workflow Steps"
     Codebehind="Workflow_Step_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/Edit.ascx" TagName="WorkflowStepEdit"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:WorkflowStepEdit runat="server" ID="editElem" ShowGeneralProperties="true" ShowAdvancedProperties="true" />
</asp:Content>
