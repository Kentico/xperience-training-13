<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Module_UserInterface_Frameset"
     Codebehind="Frameset.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Module Edit - User interface</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<frameset border="0" cols="320,*" runat="server" id="uiFrameset" enableviewstate="false">
    <frame name="tree" runat="server" id="treeFrame" scrolling="no" frameborder="0" noresize="noresize"
        enableviewstate="false" />
    <frame name="uicontent" runat="server" id="contentFrame" frameborder="0" noresize="noresize"
        enableviewstate="false" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
