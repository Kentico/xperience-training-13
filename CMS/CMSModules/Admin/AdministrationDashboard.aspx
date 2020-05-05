<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/Dashboard.master" CodeBehind="AdministrationDashboard.aspx.cs" Inherits="CMSModules_Admin_AdministrationDashboard" %>

<%@ Register Src="~/CMSModules/Widgets/Controls/Dashboard.ascx" TagName="Dashboard"
    TagPrefix="cms" %>
<asp:Content runat="server" ID="cplcContent" ContentPlaceHolderID="plcContent">
    <cms:Dashboard ID="cmsDashboard" runat="server" ShortID="d" />
</asp:Content>
