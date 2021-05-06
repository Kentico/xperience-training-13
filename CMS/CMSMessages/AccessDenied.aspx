<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSMessages_AccessDenied"
    Theme="Default" EnableEventValidation="false"  Codebehind="AccessDenied.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Debug/SecurityLog.ascx" TagName="SecurityLog"
    TagPrefix="cms" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>CMS - Access denied</title>
    <style type="text/css">
        body {
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
            <asp:Label ID="lblMessage" runat="server" Text="Label" /><br />
            <br />
                <cms:LocalizedHyperlink ID="lnkGoBack" runat="server" NavigateUrl="~/default.aspx" ResourceString="CMSDesk.GoBack" /><br />
            <br />
                <cms:LocalizedButton ID="btnSignOut" Visible="false" runat="server" ButtonStyle="Primary"
                    OnClick="btnSignOut_Click" ResourceString="signoutbutton.signout" />
                <cms:LocalizedButton ID="btnLogin" Visible="false" runat="server" ButtonStyle="Primary"
                    OnClick="btnLogin_Click" ResourceString="screenlock.signin" />
            <asp:Literal ID="ltlScript" runat="server" Text="" />
            <br />
            <br />
            <cms:SecurityLog ID="logSec" runat="server" InitFromRequest="true" />
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
