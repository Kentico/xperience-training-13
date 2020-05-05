<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Groups_Tools_Forums_Posts_ForumPost_Frameset"
     Codebehind="ForumPost_Frameset.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Forums - Post frameset</title>
</head>
<frameset border="0" cols="304, *" runat="server" id="colsFrameset">
    <frame frameborder="0" scrolling="auto" runat="server" ID="frameTree" />
    <frame frameborder="0" runat="server" ID="frameEdit" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
