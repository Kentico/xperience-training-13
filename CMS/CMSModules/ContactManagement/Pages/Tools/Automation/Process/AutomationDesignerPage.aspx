<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AutomationDesignerPage.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Theme="Default" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_AutomationDesignerPage" %>
<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/AutomationDesigner.ascx" TagName="AutomationDesigner" TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:AutomationDesigner ID="designerElem" runat="server" />
</asp:Content>