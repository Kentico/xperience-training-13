<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_WebAnalytics_Tools_Disabled" Theme="Default"  Codebehind="Disabled.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Web analytics</title>
    <style type="text/css">
		body
		{
			margin: 0px;
			padding: 0px;
			height:100%; 
		}
	</style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlBody" runat="server" CssClass="PageBody">
            <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" IsLiveSite="false" />
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
