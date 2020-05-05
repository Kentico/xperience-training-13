<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Reporting_Tools_SavedReports_SavedReport_Print" Theme="Default"  Codebehind="SavedReport_Print.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Saved Report Print</title>
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
        <asp:Literal ID="ltlHtml" runat="server" EnableViewState="false" />
    </form>
</body>
</html>
