<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Newsletter_Report.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Report" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="newsletter-report">
        <cms:LocalizedHeading ID="lblTitle" Level="3" runat="server" ResourceString="emailmarketing.ui.reportheader" CssClass="newsletter-report-header" />
        <cms:LocalizedLabel ID="lblNoRecords" runat="server" CssClass="newsletter-report-nodata hidden" ResourceString="emailmarketing.ui.newsletter.report.nodata" />
        <div class="tabs hidden">
            <div class="tabs-nav btn-group">
                <button ID="btnTabAbsolute" runat="server" type="button" class="tab-link btn btn-default" />
                <button ID="btnTabPercentages" runat="server" type="button" class="tab-link btn btn-default" />
            </div>
            <div class="tab-content">
                <div id="absoluteChart" class="chart"></div>
            </div>
            <div class="tab-content">
                <div id="stackedChart" class="chart"></div>
            </div>
        </div>
    </div>
</asp:Content>
