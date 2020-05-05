<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="DocumentType_Edit_Query_Frameset.aspx.cs"
    Inherits="CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Query_Frameset" Theme="Default" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Development - Document types - Edit query</title>
</head>
<frameset border="0" rows="<%=mHeight%>, *" id="rowsFrameset">
    <frame name="q_edit_menu" src="DocumentType_Edit_Query_Header.aspx?<%=QueryHelper.EncodedQueryString%>"
        scrolling="no" frameborder="0" noresize="noresize" />
    <frame name="q_edit_content" src="DocumentType_Edit_Query_Edit.aspx?<%=QueryHelper.EncodedQueryString%>"
        frameborder="0" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
