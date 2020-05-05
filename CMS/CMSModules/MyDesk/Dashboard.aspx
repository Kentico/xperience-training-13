<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dashboard.master" Codebehind="Dashboard.aspx.cs" Theme="Default" EnableEventValidation="false"
    Inherits="CMSModules_MyDesk_Dashboard" %>

<%@ Register Src="~/CMSModules/Widgets/Controls/Dashboard.ascx" TagName="Dashboard" TagPrefix="cms" %>

<asp:Content runat="server" ID="cplcContent" ContentPlaceHolderID="plcContent">
    <cms:Dashboard ID="cmsDashboard" runat="server" ShortID="d" />
</asp:Content>
