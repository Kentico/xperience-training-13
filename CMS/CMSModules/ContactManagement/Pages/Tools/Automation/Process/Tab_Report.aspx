<%@ Page Language="C#" AutoEventWireup="True"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Automation process – Reports" EnableEventValidation="false"
    Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Report" Theme="Default"  Codebehind="Tab_Report.aspx.cs" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>

<asp:Content ID="cntHeader" runat="server" ContentPlaceHolderID="plcActions">
    <div class="control-group-inline header-actions-container">
        <cms:ReportHeader ID="reportHeader" runat="server" />
        <cms:HeaderActions ID="additionalActions" runat="server" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisplayReport ID="ucReport" runat="server" BodyCssClass="DisplayReportBody" RenderCssClasses="true" />
</asp:Content>
