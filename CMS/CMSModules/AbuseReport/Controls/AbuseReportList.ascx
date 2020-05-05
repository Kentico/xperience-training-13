<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/AbuseReport/Controls/AbuseReportList.ascx.cs"
    Inherits="CMSModules_AbuseReport_Controls_AbuseReportList" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:UniGrid runat="server" ID="ucAbuseReportGrid" GridName="~/CMSModules/AbuseReport/Controls/AbuseReport_List.xml"
    IsLiveSite="false" Columns="ReportID,ReportWhen,ReportStatus,ReportComment,ReportURL,ReportTitle,ReportCulture"
    OrderBy="ReportWhen ASC" HideFilterButton="true" ShowFilter="True" FilterLimit="0"/>
