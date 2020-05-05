<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Default.aspx.cs" Inherits="CMSModules_Modules_Pages_Settings_Default" %>

<!DOCTYPE html>
<html>
<head id="Head2" runat="server" enableviewstate="false">
    <title>Development - Settings</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
            height: 100%;
        }
    </style>
</head>
<frameset border="0" cols="304,*" framespacing="0" runat="server" id="colsFrameset">
    <frame id="frameTree" runat="server" name="settingstree" scrolling="no" frameborder="0" />
    <frame id="frameMain" runat="server" name="settingsmain" scrolling="auto" frameborder="0" />
</frameset>
<cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />

</html>
