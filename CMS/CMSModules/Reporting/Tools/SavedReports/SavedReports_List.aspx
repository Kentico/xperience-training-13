<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Reporting_Tools_SavedReports_SavedReports_List" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="SavedReport list" Theme="Default"  Codebehind="SavedReports_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <cms:UniGrid runat="server" ID="UniGrid" GridName="SavedReports_List.xml" Columns="SavedReportID, SavedReportTitle, SavedReportDate, SavedReportCreatedByUserID"
        IsLiveSite="false" OrderBy="SavedReportDate DESC" />
</asp:Content>
