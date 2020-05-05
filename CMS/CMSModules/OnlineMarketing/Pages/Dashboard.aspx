<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/Dashboard.master" Codebehind="Dashboard.aspx.cs" Inherits="CMSModules_OnlineMarketing_Pages_Dashboard"
    Theme="Default" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/Widgets/Controls/Dashboard.ascx" TagName="Dashboard" TagPrefix="cms" %>

<asp:Content runat="server" ID="cplcContent" ContentPlaceHolderID="plcContent">
    <cms:Dashboard ID="cmsDashboard" runat="server" ShortID="d" />
</asp:Content>