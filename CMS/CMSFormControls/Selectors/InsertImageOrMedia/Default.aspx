<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Selectors_InsertImageOrMedia_Default"
    EnableEventValidation="false"  Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Insert</title>
</head>
<frameset border="0" rows="<%=TabsFrameHeight%>, *, <%=FooterFrameHeight%>" id="rowsFrameset">
    <frame name="insertHeader" scrolling="no" frameborder="0"
        noresize="noresize" id="menu" />
    <frame name="insertContent" scrolling="no" frameborder="0"
        id="content" />
    <frame name="insertFooter" src="Footer.aspx<%=Request.Url.Query%>" scrolling="no"
        frameborder="0" noresize="noresize" id="footer" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
