<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Tab_Parameters.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Workflow action properties – Parameters"
    Inherits="CMSModules_Workflows_Pages_WorkflowAction_Tab_Parameters" Theme="Default" %>            
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx"
    TagName="FieldEditor" TagPrefix="cms" %>
    
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:FieldEditor ID="fieldEditor" runat="server" Mode="ProcessActions" IsLiveSite="false" />
</asp:Content>
