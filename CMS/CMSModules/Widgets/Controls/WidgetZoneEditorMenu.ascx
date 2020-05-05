<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="WidgetZoneEditorMenu.ascx.cs" Inherits="CMSModules_Widgets_Controls_WidgetZoneEditorMenu" %>
<asp:Panel runat="server" ID="pnlZoneMenu" CssClass="PortalContextMenu ZoneContextMenu">
    <asp:Panel runat="server" ID="pnlNewWebPart" CssClass="Item">
        <asp:Panel runat="server" ID="pnlNewWebPartPadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblNewWebPart" CssClass="Name" EnableViewState="false"
                Text="NewWebPart" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlSep1" CssClass="Separator"></asp:Panel>
    <asp:Panel runat="server" ID="pnlCopyAllItem" CssClass="Item">
        <asp:Panel runat="server" ID="pnlCopyAllItemPadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblCopy" CssClass="Name" EnableViewState="false"
                Text="CopyWebPart" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlPaste" CssClass="Item">
        <asp:Panel runat="server" ID="pnlPastePadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblPaste" CssClass="Name" EnableViewState="false"
                Text="PasteWebPart" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="Panel1" CssClass="Separator"></asp:Panel>
    <asp:Panel runat="server" ID="pnlDelete" CssClass="Item">
        <asp:Panel runat="server" ID="pnlDeletePadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblDelete" CssClass="Name" EnableViewState="false"
                Text="Delete" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    //<![CDATA[
    var lm_widgetZoneEditorMenu = 'widgetZoneEditorMenu';

    function ContextGetWidgetZoneEditorDefinition() {
        return GetContextMenuParameter(lm_widgetZoneEditorMenu);
    }

    function ContextNewWidgetEditor() {
        NewWidget(ContextGetWidgetZoneEditorDefinition());
    }

    function ContextRemoveAllWidgetsEditor() {
        RemoveAllWidgets(ContextGetWidgetZoneEditorDefinition());
    }

    function ContextCopyAllWidgetsEditor() {
        CopyWebPart(ContextGetWidgetZoneEditorDefinition(), lm_widgetZoneEditorMenu);
    }

    function ContextPasteWidgetZoneEditor() {
        PasteWebPart(ContextGetWidgetZoneEditorDefinition());
    }

    PM_EnsurePasteHandler(lm_widgetZoneEditorMenu, '<%= pnlPaste.ClientID %>');
    //]]>
</script>

