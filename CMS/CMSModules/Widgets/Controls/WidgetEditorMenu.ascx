<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="WidgetEditorMenu.ascx.cs" Inherits="CMSModules_Widgets_Controls_WidgetEditorMenu" %>
<asp:Panel runat="server" ID="pnlWidgetMenu" CssClass="PortalContextMenu WebPartContextMenu">
    <asp:Panel runat="server" ID="pnlProperties" CssClass="Item">
        <asp:Panel runat="server" ID="pnlPropertiesPadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblProperties" CssClass="Name" EnableViewState="false"
                Text="Properties" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlSep1" CssClass="Separator"></asp:Panel>
    <cms:ContextMenuItem runat="server" ID="iMoveTo" SubMenuID="moveToMenuWidget" />
    <cms:ContextMenuItem runat="server" ID="iCopy" />
    <cms:ContextMenuItem runat="server" ID="iPaste" />
    <asp:Panel runat="server" ID="pnlDelete" CssClass="Item">
        <asp:Panel runat="server" ID="pnlDeletePadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblDelete" CssClass="Name" EnableViewState="false"
                Text="Delete" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    //<![CDATA[
    var lm_widgetEditorMenu = 'widgetEditorMenu';

    function ContextGetWidgetEditorDefinition() {
        return GetContextMenuParameter(lm_widgetEditorMenu);
    }

    function ContextConfigureWidgetEditor() {
        ConfigureWidget(ContextGetWidgetEditorDefinition());
    }

    function ContextRemoveWidgetEditor() {
        RemoveWidget(ContextGetWidgetEditorDefinition());
    }

    function ContextCopyWidgetEditor(e) {
        CopyWebPart(ContextGetWidgetEditorDefinition(), lm_widgetEditorMenu);
    }

    function ContextPasteWidgetEditor(e) {
        PasteWebPart(ContextGetWidgetEditorDefinition());
    }

    PM_EnsurePasteHandler(lm_widgetEditorMenu, '<%= iPaste.ClientID %>');
    //]]>
</script>

