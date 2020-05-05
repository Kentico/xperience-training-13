<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Staging_Tools_Data_Frameset"
    EnableViewState="false"  Codebehind="Frameset.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Content staging</title>
</head>
<frameset border="0" cols="304, *" rows="*" id="colsFrameset" runat="server">
    <frame name="tasksTree" src="Tree.aspx" scrolling="no" frameborder="0" runat="server" />
    <frame name="tasksContent" src="Tasks.aspx?siteid=-1" scrolling="auto" frameborder="0" runat="server" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
