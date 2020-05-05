<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_LicenseLimit"
    Theme="Default"  Codebehind="LicenseLimit.aspx.cs" %>
<%@ Register src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" tagname="PageTitle" tagprefix="cms" %>


<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <meta http-equiv="pragma" content="no-cache" />
    <title>The page cannot be displayed due to license limitations</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<body  class="<%=mBodyClass%>">
    <form id="form1" runat="server">
        <asp:Panel ID="PanelBody" runat="server" CssClass="PageBody">
            <asp:Panel ID="PanelTitle" runat="server" CssClass="PageHeader">
                <cms:PageTitle ID="titleElem" runat="server" />
            </asp:Panel>
            <asp:Panel ID="PanelContent" runat="server" CssClass="PageContent">
                <asp:Label ID="lblMessage" runat="server" />
            </asp:Panel>
        </asp:Panel>
    </form>
    <script type="text/javascript">
        // <![CDATA[
        <%-- 
            All the modal window pages are not closable by clicking on the gray area,
            but this page is different, because there is no close button in the header
            This might get fixed when all the modal pages use dialog=1 GET parameter.
        --%>
        setTimeout(function () {
        	if (top && typeof top.addBackgroundClickHandler === 'function') {
        		top.addBackgroundClickHandler(this);
            }
        }, 500);
        // ]]>
    </script>
</body>
</html>
