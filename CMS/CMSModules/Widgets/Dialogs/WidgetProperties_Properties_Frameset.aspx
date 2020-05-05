<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Widgets_Dialogs_WidgetProperties_Properties_Frameset"
    EnableEventValidation="false" ValidateRequest="false"  Codebehind="WidgetProperties_Properties_Frameset.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Widget Properties</title>
    <style type="text/css">
        body
        {
            margin: 0px;
            height: 100%;
            width: 100%;
            overflow: hidden;
        }
    </style>
</head>
<frameset border="0" runat="server" id="rowsFrameset">
    <frame name="widgetpropertiescontent" frameborder="0" noresize="noresize" scrolling="auto"
        runat="server" id="frameContent" />
    <frame name="widgetpropertiesbuttons" frameborder="0" noresize="noresize" scrolling="no"
        runat="server" id="frameButtons" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
