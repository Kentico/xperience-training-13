<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_Redirect"
    Theme="Default"  Codebehind="Redirect.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>


<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Redirection</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>

    <script type="text/javascript">
    //<![CDATA[
        function IsCMSDesk()
        {
            var result = ((parent != null) && (parent != window) && (parent.IsCMSDesk));
            return result;
        }
    //]]>
    </script>

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
                <asp:HyperLink runat="server" ID="lnkTarget" EnableViewState="false" />
            </asp:Panel>
            <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
        </asp:Panel>
    </form>
</body>
</html>
