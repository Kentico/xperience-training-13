<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" Inherits="CMSFormControls_Selectors_SelectFileOrFolder_Default"
    EnableEventValidation="false"  Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Insert</title>
</head>
<frameset rows="<%=TitleOnlyHeight%>, *, <%=FooterFrameHeight%>" border="0" id="rowsFrameset">
    <frame name="insertHeader" src="Header.aspx<%=Request.Url.Query%>" scrolling="no"
        frameborder="0" noresize="noresize" id="menu" />
    <frame name="insertContent" src="Content.aspx<%=Request.Url.Query%>" scrolling="no"
        frameborder="0" id="content" />
    <frame name="insertFooter" src="Footer.aspx<%=Request.Url.Query%>" scrolling="no"
        frameborder="0" noresize="noresize" id="footer" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
