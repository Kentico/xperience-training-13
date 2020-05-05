<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Widgets_Controls_WidgetMenu"  Codebehind="WidgetMenu.ascx.cs" %>
<cms:ContextMenu runat="server" ID="menuMoveTo" MenuID="moveToMenuWidget" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MenuLevel="1"
    ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlZoneMenu" CssClass="PortalContextMenu WebPartContextMenu">
        <div class="NormalLayoutMenu AnyMenu">
            <cms:ContextMenuItem runat="server" ID="iTop" />
            <cms:ContextMenuItem runat="server" ID="iUp" />
            <cms:ContextMenuSeparator runat="server" ID="sm1" />
            <cms:ContextMenuItem runat="server" ID="iDown" />
            <cms:ContextMenuItem runat="server" ID="iBottom" />
        </div>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuUp" MenuID="upMenuWidget" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MouseButton="Right"
    MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlUpMenu" CssClass="PortalContextMenu WebPartContextMenu">
        <asp:Panel runat="server" ID="pnlTop" CssClass="Item">
            <asp:Panel runat="server" ID="pnlTopPadding" CssClass="ItemPadding">
                <asp:Label runat="server" ID="lblTop" CssClass="Name" EnableViewState="false" Text="Top" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuDown" MenuID="downMenuWidget" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MouseButton="Right"
    MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlDownMenu" CssClass="PortalContextMenu WebPartContextMenu">
        <asp:Panel runat="server" ID="pnlBottom" CssClass="Item">
            <asp:Panel runat="server" ID="pnlBottomPadding" CssClass="ItemPadding">
                <asp:Label runat="server" ID="lblBottom" CssClass="Name" EnableViewState="false"
                    Text="Bottom" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</cms:ContextMenu>
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
    var lm_widgetMenu = 'widgetMenu';

    function ContextGetWidgetDefinition() {
        return GetContextMenuParameter(lm_widgetMenu);
    }

    function ContextConfigureWidget() {
        ConfigureWidget(ContextGetWidgetDefinition());
    }

    function ContextMoveWidgetUp() {
        MoveWebPartUp(ContextGetWidgetDefinition());
    }

    function ContextMoveWidgetDown() {
        MoveWebPartDown(ContextGetWidgetDefinition());
    }

    function ContextRemoveWidget() {
        RemoveWidget(ContextGetWidgetDefinition());
    }

    function ContextCloneWidget() {
        CloneWebPart(ContextGetWidgetDefinition());
    }

    function ContextMoveWidgetTop() {
        var zone = ContextGetWidgetDefinition();
        MoveWebPart(zone, zone.zoneId, 0);
    }

    function ContextMoveWidgetBottom() {
        var zone = ContextGetWidgetDefinition();
        MoveWebPart(zone, zone.zoneId, 1000);
    }


    function ContextCopyWidget(e) {
        CopyWebPart(ContextGetWidgetDefinition(), lm_widgetMenu);
    }

    function ContextPasteWidget(e) {
        PasteWebPart(ContextGetWidgetDefinition());
    }

    PM_EnsurePasteHandler(lm_widgetMenu, '<%= iPaste.ClientID %>');
    //]]>
</script>

