<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_SplitView_Separator"
    Theme="Default"  Codebehind="Separator.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>CMSDesk - Split view separator</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<body class="<%=mBodyClass%>">
    <form id="form1" runat="server">
        <asp:Panel runat="server" ID="PanelSeparator" CssClass="TreeBorder SplitSeparator">
            &nbsp;
        </asp:Panel>
    </form>
</body>
</html>
