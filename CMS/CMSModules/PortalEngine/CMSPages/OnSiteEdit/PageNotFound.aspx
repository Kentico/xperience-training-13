<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="PageNotFound.aspx.cs"
    Inherits="CMSModules_PortalEngine_CMSPages_OnSiteEdit_PageNotFound" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
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
<body class="<%=mBodyClass%> cms-bootstrap">
    <form id="form1" runat="server">
    <asp:PlaceHolder runat="server" ID="plcMain" />
    <asp:Panel ID="PanelBody" runat="server" CssClass="PageBody">
        <asp:Panel ID="PanelTitle" runat="server" CssClass="PageHeader">
            <cms:PageTitle ID="titleElem" runat="server" />
        </asp:Panel>
        <asp:Panel ID="PanelContent" runat="server" CssClass="PageContent">
            <asp:Label runat="server" ID="lblInfo" EnableViewState="false" /><br />
            <br />
            <asp:Label runat="server" ID="lblRootDoc" EnableViewState="false" />
        </asp:Panel>
    </asp:Panel>
    </form>
</body>
</html>
