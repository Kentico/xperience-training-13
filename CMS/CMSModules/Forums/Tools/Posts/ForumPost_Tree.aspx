<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Forums_Tools_Posts_ForumPost_Tree"
    Theme="Default"  Codebehind="ForumPost_Tree.aspx.cs" %>

<%@ Register Src="~/CMSModules/Forums/Controls/PostTree.ascx" TagName="PostTree" TagPrefix="cms" %>
<%@ Register src="~/CMSAdminControls/UI/Trees/TreeBorder.ascx" tagname="TreeBorder" tagprefix="cms" %>
<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Forums - Forum Tree</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
            width: 100%;
            overflow: hidden;
        }
    </style>
</head>
<body class="TreeBody <%=mBodyClass%>">
    <form id="form1" runat="server">
    <asp:Panel runat="server" ID="pnlBody" CssClass="ContentTree">
        <cms:TreeBorder ID="borderElem" runat="server" FramesetName="colsFrameset" />
        <div class="TreeArea">
            <div class="TreeAreaTree">
                <cms:PostTree ID="treeElem" runat="server" ItemSelectedItemCssClass="ContentTreeSelectedItem" />
            </div>
        </div>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    </form>
</body>
</html>
