<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Workflows_Workflow_Step_New" Theme="Default" Title="Workflows - New workflow step"
     Codebehind="Workflow_Step_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/Edit.ascx" TagName="StepEdit"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:StepEdit runat="server" ID="editElem" />
</asp:Content>
