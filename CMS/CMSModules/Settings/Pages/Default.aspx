<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Settings_Pages_Default"
     Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server" enableviewstate="false">
    <title>Settings</title>

    <script type="text/javascript">
        //<![CDATA[
        document.titlePart = 'Settings';
        //]]>
    </script>

</head>
<frameset border="0" cols="300, *" runat="server" id="colsFrameset" enableviewstate="false"
    framespacing="0">
    <frame name="categories" src="Categories.aspx" scrolling="no" frameborder="0" runat="server" />
    <frame ID="keysFrame" name="keys" frameborder="0" runat="server" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
