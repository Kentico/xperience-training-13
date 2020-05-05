<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSMessages_invalidlicensekey" Theme="Default"  Codebehind="invalidlicensekey.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI.HtmlControls" Assembly="System.Web" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Invalid license key</title>
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
                <asp:Label ID="lblRawUrl" runat="server" />
                <asp:Label ID="lblRawUrlValue" runat="server" Font-Bold="true" />
                <br />
                <br />
                <asp:Label ID="lblResult" runat="server" />
                <asp:Label ID="lblResultValue" runat="server" Font-Bold="true" />
                <br />
                <br />
                <cms:LocalizedLabel ID="lblGoTo" runat="server" ResourceString="invalidLicense.goto" />
                <cms:LocalizedHyperlink ID="lnkGoToValue" runat="server" ResourceString="invalidlicense.location" Target="_top" />
                <cms:LocalizedLabel ID="lblAddLicense" runat="server" ResourceString="invalidLicense.addlicense" DisplayColon="true" />
                <cms:LocalizedLabel ID="lblAddLicenseValue" runat="server"  Font-Bold="true"/>
                <br />
                <br />
                <br />
                <cms:LocalizedLabel ID="lblHowTo" runat="server" ResourceString="invalidlicense.howto" Font-Bold="true" />
                <ul>
                    <li>
                        <cms:LocalizedLabel ID="lblOpt1First" runat="server" ResourceString="invalidlicense.howto.option1.firstpart" />
                        <cms:LocalizedHyperLink ID="lnkOpt1" Target="_blank" runat="server" ResourceString="invalidlicense.clientportal" />
                        <cms:LocalizedLabel ID="lblOpt1Second" runat="server" ResourceString="invalidlicense.howto.option1.secondpart" /><br />
                    </li>
                    <li>
                        <cms:LocalizedLabel ID="lblOpt2" runat="server" ResourceString="invalidlicense.howto.option2" />
                        <asp:HyperLink ID="lnkOpt2" runat="server" NavigateUrl="mailto:sales@kentico.com" Text="sales@kentico.com" />.<br />
                    </li>
                    <li>
                        <cms:LocalizedLabel ID="lblOpt3First" runat="server" ResourceString="invalidlicense.howto.option3.firstpart" />
                        <cms:LocalizedHyperLink ID="lnkOpt3" Target="_blank" runat="server" ResourceString="invalidlicense.clientportal" />
                        <cms:LocalizedLabel ID="lblOpt3Second" runat="server" ResourceString="invalidlicense.howto.option3.secondpart" />
                    </li>
                    <asp:HtmlGenericControl runat="server" ID="genTrial" TagName="li" Visible="False" />
                </ul>
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
