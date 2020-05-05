<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Workflows_Workflow_General" Theme="Default"  Codebehind="Workflow_General.aspx.cs" %>
<%@ Register Src="~/CMSModules/Workflows/Controls/UI/Workflow/Edit.ascx"
    TagName="WorkflowEdit" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
    <cms:WorkflowEdit ID="editElem" runat="server" IsLiveSite="false" />
</asp:Content>
