<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Reporting_Tools_SavedReports_SavedReport_View" Theme="Default"  Codebehind="SavedReport_View.aspx.cs" 
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Literal runat="server" ID="ltlHtml" EnableViewState="false" />
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
