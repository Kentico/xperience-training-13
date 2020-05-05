<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MessageBoards_Content_Properties_Default"
     Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>CMSDesk - Message boards</title>
</head>
<frameset border="0" rows="<%=TitleOnlyHeight%>,*, <%=FooterFrameHeight%>" id="rowsFrameset">
    <frame name="header" src="header.aspx<%=Request.Url.Query%>" scrolling="no" noresize="noresize"
        frameborder="0" />
    <frameset border="0" cols="304,*" id="colsFrameset" runat="server">
        <frame name="tree" runat="server" scrolling="no" frameborder="0" id="tree" />
        <frame name="main" runat="server" scrolling="auto" frameborder="0" id="main" />
    </frameset>
    <frame name="contentfooter" src="footer.aspx" scrolling="no" noresize="noresize"
        frameborder="0" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
