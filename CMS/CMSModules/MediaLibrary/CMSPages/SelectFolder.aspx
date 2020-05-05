<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MediaLibrary_CMSPages_SelectFolder"
    EnableEventValidation="false"  Codebehind="SelectFolder.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Copy / Move</title>
</head>
<frameset border="0" rows="*, <%=FooterFrameHeight%>" id="rowsFrameset">
    <frame name="selectFolderContent" src="SelectFolder_Content.aspx<%=Request.Url.Query%>"
        scrolling="no" frameborder="0" id="content" />
    <frame name="selectFolderFooter" src="SelectFolder_Footer.aspx<%=Request.Url.Query%>"
        scrolling="no" frameborder="0" noresize="noresize" id="footer" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
