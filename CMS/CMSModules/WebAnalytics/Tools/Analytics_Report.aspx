<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Analytics_Report.aspx.cs"
    EnableEventValidation="false" Theme="Default" Inherits="CMSModules_WebAnalytics_Tools_Analytics_Report"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/AnalyticsReportViewer.ascx"
    TagPrefix="cms" TagName="ReportViewer" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <div class="control-group-inline header-actions-container">
        <cms:ReportHeader runat="server" ID="reportHeader" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel-alert">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled" />
    </asp:Panel>
    <cms:GraphPreLoader runat="server" ID="preloader" />
    <cms:ReportViewer runat="server" ID="ucReportViewer" IsLiveSite="false" />
</asp:Content>
