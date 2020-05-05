<%@ Page Language="C#" AutoEventWireup="true" Theme="Default"
    Inherits="CMSModules_Reporting_Tools_Analytics_Print"  Codebehind="Analytics_Print.aspx.cs" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Report Print</title>
    <base target="_self" />
    <style type="text/css">
        @media print {
            body {
                padding: 0;
            }
        }

        @media not print {
            body {
                padding: 10px;
            }
        }
    </style>
</head>
<body onload="window.print();" class="<%=mBodyClass%> cms-bootstrap">
    <form id="form1" runat="server">
        <asp:PlaceHolder runat="server" ID="plcMenu" />
        <asp:ScriptManager ID="scriptManager" runat="server" />
        <asp:Panel runat="server" ID="pnlContent">
            <cms:DisplayReport ID="displayReport" runat="server" BodyCssClass="ReportBodyAnalytics"
                FormCssClass="ReportFilter" IsLiveSite="false" />
        </asp:Panel>
    </form>
</body>
</html>
