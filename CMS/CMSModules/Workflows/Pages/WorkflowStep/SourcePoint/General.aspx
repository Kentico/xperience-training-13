<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default"  Codebehind="General.aspx.cs" Inherits="CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_General"
    Title="Workflows - Step Edit" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/SourcePoint/Edit.ascx"
    TagName="SourcePointEdit" TagPrefix="cms" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:SourcePointEdit runat="server" ID="editElem" />
</asp:Content>
