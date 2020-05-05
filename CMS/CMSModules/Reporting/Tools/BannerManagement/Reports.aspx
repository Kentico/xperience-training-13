<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_BannerManagement_Reports"
    Title="Banner reports" Theme="Default"  Codebehind="Reports.aspx.cs" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx" TagName="GraphTypeAndPeriod" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms" TagName="DisabledModule" %>

<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <div class="control-group-inline header-actions-container">
        <cms:ReportHeader runat="server" ID="reportHeader" />
    </div>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel runat="server" ID="pnlDisabled">
        <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSAnalyticsEnabled" />
    </asp:Panel>
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <cms:GraphTypeAndPeriod runat="server" ID="ucGraphTypePeriod" />
    <cms:DisplayReport ID="ucDisplayReport" runat="server"
        BodyCssClass="DisplayReportBody" IsLiveSite="false" RenderCssClasses="true" />
    <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
</asp:Content>
