<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Workflows_Workflow_Scope_Edit"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" EnableEventValidation="false"
    Theme="Default" Title="Workflows - Workflow Scopes"  Codebehind="Workflow_Scope_Edit.aspx.cs" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel ID="pnlForm" runat="server">
        <cms:UIForm runat="server" ID="editForm" ObjectType="cms.workflowscope" DefaultFieldLayout="TwoColumns" OnOnBeforeDataLoad="editForm_OnBeforeDataLoad" />
    </asp:Panel>
</asp:Content>
