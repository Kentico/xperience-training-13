<%@ Page Language="C#" AutoEventWireup="false" Theme="Default"
    Inherits="CMSModules_Reporting_Tools_Report_Print"  Codebehind="Report_Print.aspx.cs" %>

<%@ Register Src="~/CMSModules/Reporting/Controls/DisplayReport.ascx" TagName="DisplayReport" TagPrefix="cms" %>
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
<body onload="window.print();" class="<%=mBodyClass%>">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManager" runat="server" />
        <asp:PlaceHolder runat="server" ID="pnlManager" />
        <asp:Panel runat="server" ID="pnlContent">
            <cms:DisplayReport ID="DisplayReport" runat="server" BodyFormCssClass="ReportFilter" IsLiveSite="false" />
        </asp:Panel>
    </form>
</body>
</html>
