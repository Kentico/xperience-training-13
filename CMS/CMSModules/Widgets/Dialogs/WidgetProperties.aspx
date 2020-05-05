<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Widgets_Dialogs_WidgetProperties"
     Codebehind="WidgetProperties.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server">
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

    <script type="text/javascript">
        //<![CDATA[
        function ChangeWidget(zoneId, widgetId, aliasPath) {
            CloseDialog();
            wopener.ConfigureWidget(zoneId, widgetId, aliasPath);
        }

        var refreshPageOnClose = false;

        // Update the variant slider position (used for selecting the last (new) variant)
        function UpdateVariantPosition(itemCode, variantId) {
            if (wopener.UpdateVariantPosition) {
                wopener.UpdateVariantPosition(itemCode, variantId);
            }
        }

        // Inform the parent page that the page content has been changed
        function SetContentChanged() {
            wopener.SetContentChanged();
        }

        //]]>
    </script>

</head>
<frameset border="0" rows="40,*" runat="server" id="rowsFrameset">
    <frame name="widgetpropertiesheader" scrolling="no" noresize="noresize" frameborder="0"
        runat="server" id="frameHeader" />
    <frame name="widgetpropertiescontent" frameborder="0" noresize="noresize" scrolling="no"
        runat="server" id="frameContent" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
