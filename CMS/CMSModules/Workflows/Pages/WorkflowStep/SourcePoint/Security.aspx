<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default"
    AutoEventWireup="true"  Codebehind="Security.aspx.cs" Inherits="CMSModules_Workflows_Pages_WorkflowStep_SourcePoint_Security"
    Title="Workflows - Source Point Security" %>

<%@ Register Src="~/CMSModules/Workflows/Controls/UI/WorkflowStep/Security.ascx"
    TagPrefix="cms" TagName="SecurityEdit" %>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:SecurityEdit runat="server" ID="securityElem" />
</asp:Content>
