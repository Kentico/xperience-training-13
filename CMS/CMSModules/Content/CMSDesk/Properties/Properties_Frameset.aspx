<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Content_CMSDesk_Properties_Properties_Frameset"
    Theme="Default"  Codebehind="Properties_Frameset.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Development - Document types - Edit transformation</title>
</head>
<frameset border="0" rows="<%=TitleOnlyHeight%>, *, <%=PropertiesFooterHeight%>" id="rowsFrameset">
    <frame name="header" src="Properties_Header.aspx?<%=QueryHelper.EncodedQueryString%>"
        scrolling="no" frameborder="0" noresize="noresize" />
    <frame name="content" src="<%= contentPage %>"
        frameborder="0" />
    <frame name="footer" src="Properties_Footer.aspx?<%=QueryHelper.EncodedQueryString%>"
        frameborder="0" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
