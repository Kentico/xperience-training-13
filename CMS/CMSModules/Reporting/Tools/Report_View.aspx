<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_Report_View"
    Theme="Default" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"
     Codebehind="Report_View.aspx.cs" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/ReportHeader.ascx" TagName="ReportHeader"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Report View</title>
</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
        <asp:PlaceHolder runat="server" ID="scriptManager" />
        <cms:CMSPanel ID="pnlHeaderActions" runat="server" ShortID="pH" FixedPosition="true" CssClass="header-container">
            <div class="control-group-inline header-actions-container">
                <cms:ReportHeader runat="server" ID="reportHeader" />
            </div>
        </cms:CMSPanel>
        <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
            <cms:DisplayReport ID="displayReport" runat="server" FormCssClass="ReportFilter"
                BodyCssClass="DisplayReportBody" IsLiveSite="false" RenderCssClasses="true" />
            <asp:Literal runat="server" ID="ltlModal" EnableViewState="false" />
        </asp:Panel>
    </form>
</body>
</html>
