<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_SiteOffline"
    Theme="Default"  Codebehind="SiteOffline.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>CMS - Site offline</title>
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
        </asp:Panel>
    </asp:Panel>
    </form>
</body>
</html>
