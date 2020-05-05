<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_PageNotFound"
    Theme="Default"  Codebehind="PageNotFound.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>


<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Page not found</title>
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
        <asp:Panel ID="PanelBody" runat="server" CssClass="PageBody">
            <asp:Panel ID="PanelTitle" runat="server" CssClass="PageHeader">
                <cms:PageTitle ID="titleElem" runat="server" />
            </asp:Panel>
            <asp:Panel ID="PanelContent" runat="server" CssClass="PageContent">
                <asp:Label runat="server" ID="lblInfo" EnableViewState="false" />
                <br />
                <br />
                <asp:HyperLink runat="server" ID="lnkBack" EnableViewState="false" />
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
