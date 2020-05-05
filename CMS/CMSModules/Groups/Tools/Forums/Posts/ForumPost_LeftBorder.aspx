<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Groups_Tools_Forums_Posts_ForumPost_LeftBorder"  Codebehind="ForumPost_LeftBorder.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Forum Left border</title>
    <style type="text/css">
        
        body
		{
			margin: 0px;
			padding: 0px 0px 0px 5px;
			height: 100%; 
			width: 100%;
			overflow: hidden;
			background-color: #f5f3ec;
		    /*margin-top: -18px;*/
		}
        
        .Border
        {
            border-left: solid 1px #a4b2bc;
            border-right: solid 1px #a4b2bc;
            height: 100%;
            overflow: hidden;
			padding: 0px 0px 0px 5px;
			margin: 0px;
			position: absolute;
			top: 0;
			width: 6px;
        }       
            
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlOuter" runat="server" CssClass="Border">
            <asp:Panel ID="pnlInner" runat="server" CssClass="Border">
                &nbsp;
            </asp:Panel>
        </asp:Panel>
    </form>
</body>
</html>
