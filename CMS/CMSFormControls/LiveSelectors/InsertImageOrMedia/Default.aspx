<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" Inherits="CMSFormControls_LiveSelectors_InsertImageOrMedia_Default"
     Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Insert</title>
</head>
<frameset border="0" rows="<%=TabsFrameHeight%>, *, <%=FooterFrameHeight%>" id="rowsFrameset">
    <frame name="insertHeader" src="<%=mBlankUrl%>" scrolling="no" frameborder="0" noresize="noresize"
        id="menu" />
    <frame name="insertContent" src="<%=mBlankUrl%>" scrolling="no" frameborder="0" id="content" />
    <frame name="insertFooter" src="Footer.aspx<%=Request.Url.Query%>" scrolling="no"
        frameborder="0" noresize="noresize" id="footer" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
