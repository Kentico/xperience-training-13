<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Automation_Pages_Automation_Step_General" Theme="Default" Title="Workflows - Workflow Steps"
     Codebehind="Automation_Step_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/Edit.ascx" TagName="WorkflowStepEdit"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:WorkflowStepEdit runat="server" ID="editElem" ShowGeneralProperties="true" ShowAdvancedProperties="false" />
</asp:Content>
