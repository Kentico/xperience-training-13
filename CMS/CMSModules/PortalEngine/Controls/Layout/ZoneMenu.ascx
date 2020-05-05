<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_Controls_Layout_ZoneMenu"
     Codebehind="ZoneMenu.ascx.cs" %>
<cms:ContextMenu runat="server" ID="menuMoveTo" MenuID="zoneMoveToMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MenuLevel="1"
    ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="Panel1" CssClass="PortalContextMenu ZoneContextMenu">
        <asp:Repeater runat="server" ID="repZones">
            <ItemTemplate>
                <cms:ContextMenuContainer runat="server" ID="cmcZoneVariants" MenuID="moveToZoneVariants"
                    Parameter="GetContextMenuParameter('selectedMoveToZoneId')">
                    <cms:ContextMenuItem runat="server" ID="zoneElem" onmouseover='<%#(((CMSWebPartZone)Container.DataItem).HasVariants) ? "SetContextMenuParameter(\"selectedMoveToZoneId\", \"" + ((CMSWebPartZone)Container.DataItem).ID + "\");" : "SetContextMenuParameter(\"selectedMoveToZoneId\", \"\");"%>'
                        OnClick='<%#"CM_Close(\"webPartZoneMenu\"); ContextMoveWebPartsToZone(\"" + ((CMSWebPartZone)Container.DataItem).ID + "\");"%>' Text='<%#(((CMSWebPartZone)Container.DataItem).ZoneTitle != "" ? HTMLHelper.HTMLEncode(((CMSWebPartZone)Container.DataItem).ZoneTitle) : ((CMSWebPartZone)Container.DataItem).ID) + (((CMSWebPartZone)Container.DataItem).HasVariants ? "..." : "")%>' />
                </cms:ContextMenuContainer>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuZoneMVTVariants" MenuID="zoneAllMVTVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected"
    MenuLevel="1" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlAllMVTVariants" CssClass="PortalContextMenu ZoneContextMenu">
        <asp:PlaceHolder ID="plcAddMVTVariant" runat="server">
            <asp:Panel runat="server" ID="pnlAddMVTVariant" CssClass="Item">
                <asp:Panel runat="server" ID="pnlAddMVTVariantPadding" CssClass="ItemPadding">
                    <asp:Label runat="server" ID="lblAddMVTVariant" CssClass="Name" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel2" CssClass="Separator"></asp:Panel>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlNoZoneMVTVariants" CssClass="Item ItemDisabled" Visible="false">
            <asp:Panel runat="server" ID="Panel5" CssClass="ItemPadding">
                <asp:Label runat="server" ID="lblNoZoneMVTVariants" EnableViewState="false" CssClass="Name" />
            </asp:Panel>
        </asp:Panel>
        <asp:Repeater runat="server" ID="repZoneMVTVariants">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlVariantItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                        <asp:Label runat="server" ID="lblVariantItem" CssClass="Name" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuZoneCPVariants" MenuID="zoneAllCPVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected"
    MenuLevel="1" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlAllCPVariants" CssClass="PortalContextMenu ZoneContextMenu">
        <asp:PlaceHolder ID="plcAddCPVariant" runat="server">
            <asp:Panel runat="server" ID="pnlAddCPVariant" CssClass="Item">
                <asp:Panel runat="server" ID="pnlAddCPVariantPadding" CssClass="ItemPadding">
                    <asp:Label runat="server" ID="lblAddCPVariant" CssClass="Name" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel4" CssClass="Separator"></asp:Panel>
        </asp:PlaceHolder>
        <asp:Panel runat="server" ID="pnlNoZoneCPVariants" CssClass="Item ItemDisabled" Visible="false">
            <asp:Panel runat="server" ID="Panel3" CssClass="ItemPadding">
                <asp:Label runat="server" ID="lblNoZoneCPVariants" EnableViewState="false" CssClass="Name" />
            </asp:Panel>
        </asp:Panel>
        <asp:Repeater runat="server" ID="repZoneCPVariants">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlVariantItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                        <asp:Label runat="server" ID="lblVariantItem" CssClass="Name" EnableViewState="false" />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuMoveToZoneVariants" MenuID="moveToZoneVariants"
    VerticalPosition="Bottom" HorizontalPosition="Left" OffsetX="50"
    ActiveItemCssClass="ItemSelected" MenuLevel="2" Dynamic="true" ShowMenuOnMouseOver="true"
    MouseButton="Both">
    <asp:Panel runat="server" ID="pnlZoneVariants" CssClass="PortalContextMenu ZoneContextMenu">
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
<asp:Panel runat="server" ID="pnlZoneMenu" CssClass="PortalContextMenu ZoneContextMenu">
    <asp:Panel runat="server" ID="pnlConfigureZone" CssClass="Item">
        <asp:Panel runat="server" ID="pnlConfigureZonePadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblConfigureZone" CssClass="Name" EnableViewState="false"
                Text="ConfigureZone" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="Panel6" CssClass="Separator"></asp:Panel>
    <asp:Panel runat="server" ID="pnlNewWebPart" CssClass="Item">
        <asp:Panel runat="server" ID="pnlNewWebPartPadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblNewWebPart" CssClass="Name" EnableViewState="false"
                Text="NewWebPart" />
        </asp:Panel>
    </asp:Panel>
    <cms:ContextMenuContainer runat="server" ID="cmcMoveTo" MenuID="zoneMoveToMenu">
        <asp:Panel runat="server" ID="pnlMoveTo" CssClass="Item">
            <asp:Panel runat="server" ID="pnlMoveToPadding" CssClass="ItemPadding">
                <asp:Label runat="server" ID="lblMoveTo" CssClass="NameInactive" EnableViewState="false"
                    Text="MoveTo" />
            </asp:Panel>
        </asp:Panel>
    </cms:ContextMenuContainer>
    <asp:Panel runat="server" ID="Panel7" CssClass="Separator"></asp:Panel> 
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
    <asp:Panel ID="pnlContextMenuMVTVariants" runat="server" Visible="false">
        <asp:Panel runat="server" ID="Panel8" CssClass="Separator"></asp:Panel>
        <cms:ContextMenuContainer runat="server" ID="cmcAllMVTVariants" MenuID="zoneAllMVTVariants"
            Parameter="ContextCreateZoneMenuParameter()">
            <asp:Panel runat="server" ID="pnlMVTVariants" CssClass="Item">
                <asp:Panel runat="server" ID="pnlMVTVariantsPadding" CssClass="ItemPadding">
                    <asp:Label runat="server" ID="lblMVTVariants" CssClass="NameInactive" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
        </cms:ContextMenuContainer>
    </asp:Panel>
    <asp:Panel ID="pnlContextMenuCPVariants" runat="server" Visible="false">
        <asp:Panel runat="server" ID="Panel9" CssClass="Separator"></asp:Panel>
        <cms:ContextMenuContainer runat="server" ID="cmcAllCPVariants" MenuID="zoneAllCPVariants"
            Parameter="ContextCreateZoneMenuParameter()">
            <asp:Panel runat="server" ID="pnlCPVariants" CssClass="Item">
                <asp:Panel runat="server" ID="pnlCPVariantsPadding" CssClass="ItemPadding">
                    <asp:Label runat="server" ID="lblCPVariants" CssClass="NameInactive" EnableViewState="false" />
                </asp:Panel>
            </asp:Panel>
        </cms:ContextMenuContainer>
    </asp:Panel>
    <asp:Panel runat="server" ID="Panel10" CssClass="Separator"></asp:Panel>
    <asp:Panel runat="server" ID="pnlDelete" CssClass="Item">
        <asp:Panel runat="server" ID="pnlDeletePadding" CssClass="ItemPadding">
            <asp:Label runat="server" ID="lblDelete" CssClass="Name" EnableViewState="false"
                Text="Delete" />
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    //<![CDATA[
    var lm_webPartZoneMenu = 'webPartZoneMenu';

    function ContextCreateZoneMenuParameter() {
        var objPar = GetContextMenuParameter(lm_webPartZoneMenu);
        return objPar.zoneId;
    }

    function ContextGetWebPartZoneDefinition() {
        return GetContextMenuParameter(lm_webPartZoneMenu);
    }

    function ContextNewWebPart() {
        NewWebPart(ContextGetWebPartZoneDefinition());
    }

    function ContextConfigureWebPartZone() {
        ConfigureWebPartZone(ContextGetWebPartZoneDefinition());
    }

    function ContextRemoveAllWebParts() {
        RemoveAllWebParts(ContextGetWebPartZoneDefinition());
    }

    function ContextCopyAllWebParts() {
        var zone = ContextGetWebPartZoneDefinition();
        zone.webPartId = '';
        CopyWebPart(zone, lm_webPartZoneMenu);
    }

    function ContextPasteWebPartZone() {
        PasteWebPart(ContextGetWebPartZoneDefinition());
    }

    function ContextMoveWebPartsToZone(targetZoneId) {
        MoveAllWebParts(ContextGetWebPartZoneDefinition(), targetZoneId);
    }

    function ContextAddWebPartZoneMVTVariant() {
        var zone = ContextGetWebPartZoneDefinition();
        AddMVTVariant(zone.zoneId, '', zone.nodeAliasPath, '', zone.templateId, 'zone', '');
    }

    function ContextAddWebPartZoneCPVariant() {
        var zone = ContextGetWebPartZoneDefinition();
        AddPersonalizationVariant(zone.zoneId, '', zone.nodeAliasPath, '', zone.templateId, 'zone', '');
    }

    PM_EnsurePasteHandler(lm_webPartZoneMenu, '<%= pnlPaste.ClientID %>');
    //]]>
</script>

