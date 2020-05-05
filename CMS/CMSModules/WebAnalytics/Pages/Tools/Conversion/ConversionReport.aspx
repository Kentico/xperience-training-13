<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="ConversionReport.aspx.cs" Inherits="CMSModules_WebAnalytics_Pages_Tools_Conversion_ConversionReport"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Conversion report"
    Theme="Default" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx"
    TagName="GraphType" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ConversionReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeaderActions"
    TagPrefix="cms" %>
<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <div class="header-actions-container">
        <cms:ReportHeaderActions runat="server" ID="reportHeaderActions" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlDisabled" CssClass="header-panel-alert">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled" />
    </asp:Panel>
    <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
    <asp:Panel CssClass="header-panel" runat="server" ID="pnlHeader">
        <cms:GraphType runat="server" ID="ucGraphType" />
    </asp:Panel>
    <div class="ReportBody">
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
        <cms:ReportHeader ID="ucReportHeader" runat="server" />
        <asp:PlaceHolder runat="server" ID="pnlDisplayReport"></asp:PlaceHolder>
    </div>
</asp:Content>
