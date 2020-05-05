<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_WebAnalytics_Tools_Default"
     Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Analytics</title>

    <script type="text/javascript">
        function selectTreeNode(nodeName) {
            frames['analyticsTree'].SelectNode(nodeName);
        }
    </script>

</head>
<frameset border="0" cols="295,*" id="colsFrameset" runat="server">
    <frame name="analyticsTree" id="analyticsTree" runat="server" scrolling="no" frameborder="0" />
    <frame name="analyticsDefault" id="analyticsDefault" runat="server" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
