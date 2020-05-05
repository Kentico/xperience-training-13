<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Forums_Tools_Posts_ForumPost_LeftBorder"  Codebehind="ForumPost_LeftBorder.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Forum Left border</title>
    <style type="text/css">
        
        body
		{
			margin: 0px;
			padding: 0px;
			height: 100%; 
			width: 100%;
			overflow: hidden;
			background-color: #fff;
		    /*margin-top: -18px;*/
		}  
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlOuter" runat="server">
            <asp:Panel ID="pnlInner" runat="server">
                &nbsp;
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
