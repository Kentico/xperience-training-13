<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_Controls_WebParts_WebPartMenu"
     Codebehind="WebPartMenu.ascx.cs" %>
<cms:ContextMenu runat="server" ID="menuMoveTo" MenuID="moveToMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MenuLevel="1"
    ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlZoneMenu" CssClass="PortalContextMenu WebPartContextMenu">
        <div class="NormalLayoutMenu AnyMenu">
            <cms:ContextMenuItem runat="server" ID="iTop" />
            <cms:ContextMenuItem runat="server" ID="iUp" />
            <cms:ContextMenuSeparator runat="server" ID="sm1" />
            <cms:ContextMenuItem runat="server" ID="iDown" />
            <cms:ContextMenuItem runat="server" ID="iBottom" />
            <cms:ContextMenuSeparator runat="server" ID="sm3" />
        </div>
        <cms:UIRepeater runat="server" ID="repZones" ShortID="z">
            <ItemTemplate>
                <cms:ContextMenuContainer runat="server" ID="cmcZoneVariants" MenuID="zoneVariants"
                    Parameter="GetContextMenuParameter('selectedZoneId')">
                    <cms:ContextMenuItem runat="server" ID="zoneElem" onmouseover='<%#(((CMSWebPartZone)Container.DataItem).HasVariants) ? "SetContextMenuParameter(\"selectedZoneId\", \"" + ((CMSWebPartZone)Container.DataItem).ID + "\");" : "SetContextMenuParameter(\"selectedZoneId\", \"\");"%>'
                        OnClick='<%#"CM_Close(\"zoneVariants\"); ContextMoveWebPartToZone(\"" + ((CMSWebPartZone)Container.DataItem).ID + "\");"%>'
                        Text='<%#(((CMSWebPartZone)Container.DataItem).ZoneTitle != "" ? HTMLHelper.HTMLEncode(((CMSWebPartZone)Container.DataItem).ZoneTitle) : ((CMSWebPartZone)Container.DataItem).ID) + (((CMSWebPartZone)Container.DataItem).HasVariants ? "..." : "")%>' />
                </cms:ContextMenuContainer>
            </ItemTemplate>
        </cms:UIRepeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuWebPartMVTVariants" MenuID="webpartAllMVTVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected"
    MenuLevel="1" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlAllWebPartMVTVariants" CssClass="PortalContextMenu WebPartContextMenu">
        <asp:PlaceHolder ID="plcAddMVTVariant" runat="server">
            <asp:Panel runat="server" ID="pnlAddMVTVariant" CssClass="Item">
                <asp:Panel runat="server" ID="pnlAddMVTVariantPadding" CssClass="ItemPadding">
                    <asp:Label runat="server" ID="lblAddMVTVariant" CssClass="Name" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel1" CssClass="Separator"></asp:Panel>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlNoWebPartMVTVariants" CssClass="Item ItemDisabled" Visible="false">
            <asp:Panel runat="server" ID="Panel2" CssClass="ItemPadding">
                <asp:Label runat="server" ID="lblNoWebPartMVTVariants" CssClass="Name" EnableViewState="false" />
            </asp:Panel>
        </asp:Panel>
        <cms:UIRepeater runat="server" ID="repWebPartMVTVariants" ShortID="wpmvtv">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlVariantItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                        <asp:Label runat="server" ID="lblVariantItem" CssClass="Name" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </cms:UIRepeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuWebPartCPVariants" MenuID="webpartAllCPVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected"
    MenuLevel="1" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlAllWebPartVariants" CssClass="PortalContextMenu WebPartContextMenu">
        <asp:PlaceHolder ID="plcAddCPVariant" runat="server">
            <asp:Panel runat="server" ID="pnlAddCPVariant" CssClass="Item">
                <asp:Panel runat="server" ID="pnlAddCPVariantPadding" CssClass="ItemPadding">
                    <asp:Label runat="server" ID="lblAddCPVariant" CssClass="Name" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel4" CssClass="Separator"></asp:Panel>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlNoWebPartCPVariants" CssClass="Item ItemDisabled" Visible="false">
            <asp:Panel runat="server" ID="Panel3" CssClass="ItemPadding">
                <asp:Label runat="server" ID="lblNoWebPartCPVariants" CssClass="Name" EnableViewState="false" />
            </asp:Panel>
        </asp:Panel>
        <cms:UIRepeater runat="server" ID="repWebPartCPVariants" ShortID="wpv">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlVariantItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                        <asp:Label runat="server" ID="lblVariantItem" CssClass="Name" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </cms:UIRepeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuMoveToZoneVariants" MenuID="zoneVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="50" ActiveItemCssClass="ItemSelected"
    MenuLevel="2" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlZoneVariants" CssClass="PortalContextMenu WebPartContextMenu">
        <asp:Panel runat="server" ID="pnlNoZoneVariants" CssClass="ItemPadding" Visible="false">
            <asp:Literal runat="server" ID="ltlNoZoneVariants" EnableViewState="false" />
        </asp:Panel>
        <cms:UIRepeater runat="server" ID="repMoveToZoneVariants" ShortID="zv">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlZoneVariantItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlZoneItemPadding" CssClass="ItemPadding">
                        <asp:Label runat="server" ID="lblZoneVariantItem" CssClass="Name" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </cms:UIRepeater>
    </asp:Panel>
</cms:ContextMenu>
<asp:Panel runat="server" ID="pnlWebPartMenu" CssClass="PortalContextMenu WebPartContextMenu">
    <cms:ContextMenuItem runat="server" ID="iProperties" />
    <cms:ContextMenuSeparator runat="server" ID="sep1" />
    <div class="MultipleMenu AnyMenu">
        <div class="FreeLayoutMenu AnyMenu">
            <cms:ContextMenuItem runat="server" ID="iForwardAll" />
            <cms:ContextMenuItem runat="server" ID="iBackwardAll" />
            <cms:ContextMenuSeparator runat="server" ID="sepPos" />
        </div>
        <cms:ContextMenuItem runat="server" ID="iMoveTo" SubMenuID="moveToMenu" />
        <cms:ContextMenuItem runat="server" ID="iCopy" />
        <cms:ContextMenuItem runat="server" ID="iPaste" CssClass="Item" />
        <asp:Panel ID="pnlContextMenuMVTVariants" runat="server" Visible="false">
            <asp:Panel runat="server" ID="pnlSep5" CssClass="Separator"></asp:Panel>
            <cms:ContextMenuContainer runat="server" ID="cmcAllMVTVariants" MenuID="webpartAllMVTVariants"
                Parameter="ContextCreateMenuParameter()">
                <asp:Panel runat="server" ID="pnlMVTVariants" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlMVTVariantsPadding" CssClass="ItemPadding">
                        <asp:Label runat="server" ID="lblMVTVariants" CssClass="NameInactive" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </cms:ContextMenuContainer>
        </asp:Panel>
        <asp:Panel ID="pnlContextMenuCPVariants" runat="server" Visible="false">
            <asp:Panel runat="server" ID="Panel5" CssClass="Separator"></asp:Panel>
            <cms:ContextMenuContainer runat="server" ID="cmcAllCPVariants" MenuID="webpartAllCPVariants"
                Parameter="ContextCreateMenuParameter()">
                <asp:Panel runat="server" ID="pnlCPVariants" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlCPVariantsPadding" CssClass="ItemPadding">
                        <asp:Label runat="server" ID="lblCPVariants" CssClass="NameInactive" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </cms:ContextMenuContainer>
        </asp:Panel>
        <cms:ContextMenuSeparator runat="server" ID="sep2" />
        <cms:ContextMenuItem runat="server" ID="iDelete" />
    </div>
</asp:Panel>
<script type="text/javascript">
    //<![CDATA[
    var lm_webPartMenu = 'webPartMenu';

    function ContextCreateMenuParameter() {
        var objPar = GetContextMenuParameter(lm_webPartMenu);
        return objPar.zoneId + ',' + objPar.webPartId + ',' + objPar.nodeAliasPath + ',' + objPar.instanceGuid;
    }

    function ContextGetWebPartDefinition() {
        return GetContextMenuParameter(lm_webPartMenu);
    }

    function ContextConfigureWebPart() {
        ConfigureWebPart(ContextGetWebPartDefinition());
    }

    function ContextMoveWebPartUp() {
        MoveWebPartUp(ContextGetWebPartDefinition());
    }

    function ContextMoveWebPartDown() {
        MoveWebPartDown(ContextGetWebPartDefinition());
    }

    function ContextMoveWebPartTop() {
        MoveWebPartUp(ContextGetWebPartDefinition(), true);
    }

    function ContextMoveWebPartBottom() {
        MoveWebPartDown(ContextGetWebPartDefinition(), true);
    }

    function ContextRemoveWebPart() {
        RemoveWebPart(ContextGetWebPartDefinition());
    }

    function ContextMoveWebPartToZone(targetZoneId) {
        MoveWebPart(ContextGetWebPartDefinition(), targetZoneId, 1000);
    }

    function ContextCopyWebPart(e) {
        CopyWebPart(ContextGetWebPartDefinition(), lm_webPartMenu);
    }

    function ContextPasteWebPart(e) {
        PasteWebPart(ContextGetWebPartDefinition());
    }

    function ContextCloneWebPart() {
        CloneWebPart(ContextGetWebPartDefinition());
    }
    function ContextAddWebPartMVTVariant() {
        var wp = ContextGetWebPartDefinition();
        AddMVTVariant(wp.zoneId, wp.webPartId, wp.nodeAliasPath, wp.instanceGuid, wp.templateId, 'webpart', '');
    }

    function ContextAddWebPartCPVariant() {
        var wp = ContextGetWebPartDefinition();
        AddPersonalizationVariant(wp.zoneId, wp.webPartId, wp.nodeAliasPath, wp.instanceGuid, wp.templateId, 'webpart', '');
    }

    PM_EnsurePasteHandler(lm_webPartMenu, '<%= iPaste.ClientID %>');
    //]]>
</script>
