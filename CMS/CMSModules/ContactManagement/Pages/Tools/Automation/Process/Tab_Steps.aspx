<%@ Page Language="C#" AutoEventWireup="false"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Automation process – Steps"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Steps" Theme="Default" CodeBehind="Tab_Steps.aspx.cs" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/AutomationDesigner.ascx" TagName="AutomationDesigner" TagPrefix="cms" %>

<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:AutomationDesigner ID="designerElem" runat="server" />
</asp:Content>