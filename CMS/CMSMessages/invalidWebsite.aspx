<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_invalidWebsite"
    Theme="Default"  Codebehind="invalidWebsite.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>


<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Invalid Website</title>
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
                <div style="padding-bottom: 25px">
                    <asp:Label ID="lblMessage" runat="server" Text="Label" />
                    <asp:Label ID="lblMessageUrl" runat="server" Text="Label" Font-Bold="true" />
                </div>
                <asp:Label ID="lblInfo1" runat="server" Text="Label" />
                <asp:HyperLink runat="server" ID="lnkAdministration" Target="_top" />
                <asp:Label ID="lblInfo2" runat="server" Text="Label" />
                <asp:Label ID="lblInfoDomain" runat="server" Text="Label" Font-Bold="true" />
                <asp:Panel runat="server" ID="pnlLicense" Visible="false">
                    <div  style="padding-top: 25px">
                        <cms:LocalizedLabel runat="server" ID="lblLicenseInfo" ResourceString="Message.LicenseTip" />
                        <asp:HyperLink runat="server" ID="lnkFreeEdition" NavigateUrl="http://www.kentico.com/download/free-edition.aspx">http://www.kentico.com/download/free-edition.aspx</asp:HyperLink>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
