<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" AutoEventWireup="true"
     Codebehind="Workflow_Designer.aspx.cs" Inherits="CMSModules_Workflows_Workflow_Designer"
    Title="Workflow Edit - Designer" Theme="Default" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/WorkflowDesigner.ascx" TagName="WorkflowDesigner"
    TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:WorkflowDesigner ID="designerElem" ReadOnly="false" runat="server" />
</asp:Content>
