<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_Error"
    Theme="Default"  Codebehind="Error.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>


<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <meta name="robots" content="noindex">
    <title>System error</title>
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
                <asp:HyperLink runat="server" ID="lnkBack" EnableViewState="false" Visible="false" />
                <cms:CMSButton runat="server" ID="btnCancel" EnableViewState="false" ButtonStyle="Primary" Visible="false" OnClientClick="return CloseDialog();" />
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
