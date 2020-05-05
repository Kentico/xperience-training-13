<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_Ecommerce_Report_View"
    Theme="Default"  Codebehind="Report_View.aspx.cs" Title="E-commerce - Reports" EnableEventValidation="false" %>

<%@ Register Src="~/CMSModules/WebAnalytics/Controls/GraphPreLoader.ascx" TagName="GraphPreLoader"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/AnalyticsReportViewer.ascx"
    TagPrefix="cms" TagName="ReportViewer" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="head" runat="server" enableviewstate="false">
    <title>E-commerce - Report view</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
            width: 100%;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form runat="server" id="frm">
        <asp:ScriptManager runat="server" ID="scriptManager" />
        <cms:ReportHeader runat="server" ID="reportHeader" PrintPageURL="~/CMSModules/Reporting/Tools/Ecommerce/Report_Print.aspx" />
        <asp:Panel runat="server" ID="pnlBody">
            <asp:PlaceHolder runat="server" ID="plcContainerManager" />
            <cms:ReportViewer runat="server" ID="ucReportViewer" />
            <cms:GraphPreLoader runat="server" ID="ucGraphPreLoader" />
            <asp:Literal runat="server" ID="ltlModal" EnableViewState="false" />
        </asp:Panel>
    </form>
</body>
</html>
