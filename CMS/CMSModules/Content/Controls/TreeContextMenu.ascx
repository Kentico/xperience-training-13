<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_TreeContextMenu"
     Codebehind="TreeContextMenu.ascx.cs" %>
<cms:ContextMenu runat="server" ID="menuNew" MenuID="newMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" CssClass="TreeContextMenu TreeNewContextMenu" OuterCssClass="TreeContextMenu"
    Dynamic="true" MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlNewMenu" CssClass="TreeNewContextMenu">
        <cms:ContextMenuItem runat="server" ID="iNoChild" Visible="false" />
        <asp:Repeater runat="server" ID="repNew">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlItem" CssClass="Item">
                    <asp:Panel runat="server" ID="pnlItemPadding" CssClass="ItemPadding">
                        <asp:Label runat="server" ID="lblItem" CssClass="Name" EnableViewState="false" Text='<%#HttpUtility.HtmlEncode(ResHelper.LocalizeString(DataBinder.Eval(Container.DataItem, "ClassDisplayName") as String))%>' />
                    </asp:Panel>
                </asp:Panel>
            </ItemTemplate>
        </asp:Repeater>
        <cms:UIPlaceHolder runat="server" ID="plcNewLinkVariant" ElementName="New" ModuleName="CMS.Content">
            <cms:ContextMenuSeparator runat="server" ID="pnlSepNewLinkVariant" />
            <cms:UIPlaceHolder runat="server" ID="plcNewLink" ElementName="New.LinkExistingDocument"
                ModuleName="CMS.Content">
                <cms:ContextMenuItem runat="server" ID="iNewLink" Last="true" ResourceString="content.ui.linkexistingdoc" />
            </cms:UIPlaceHolder>
        </cms:UIPlaceHolder>
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuUp" MenuID="upMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MouseButton="Right"
    MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlUpMenu" CssClass="TreeContextMenu">
        <cms:ContextMenuItem runat="server" ID="iTop" Last="true" ResourceString="UpMenu.IconTop" />
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuDown" MenuID="downMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MouseButton="Right"
    MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlDownMenu" CssClass="TreeContextMenu">
        <cms:ContextMenuItem runat="server" ID="iBottom" Last="true" ResourceString="DownMenu.IconBottom" />
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuSort" MenuID="sortMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MouseButton="Both"
    MenuLevel="1" ShowMenuOnMouseOver="true">
    <asp:Panel runat="server" ID="pnlSortMenu" CssClass="TreeContextMenu">
        <cms:ContextMenuItem runat="server" ID="iAlphaAsc" ResourceString="SortMenu.IconAlphaAsc" />
        <cms:ContextMenuItem runat="server" ID="iAlphaDesc" ResourceString="SortMenu.IconAlphaDesc" />
        <cms:ContextMenuItem runat="server" ID="iDateAsc" ResourceString="SortMenu.IconDateAsc" />
        <cms:ContextMenuItem runat="server" ID="iDateDesc" Last="true" ResourceString="SortMenu.IconDateDesc" />
    </asp:Panel>
</cms:ContextMenu>
<cms:ContextMenu runat="server" ID="menuProperties" MenuID="propertiesMenu" VerticalPosition="Bottom"
    HorizontalPosition="Left" OffsetX="25" ActiveItemCssClass="ItemSelected" MenuLevel="1"
    ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlPropertiesMenu" CssClass="TreeContextMenu">
    </asp:Panel>
</cms:ContextMenu>
<asp:Panel runat="server" ID="pnlNodeMenu" CssClass="TreeContextMenu">
    <cms:ContextMenuItem runat="server" ID="iNoNode" ResourceString="content.documentnotexistsinfoshort" Visible="False" />
    <asp:PlaceHolder ID="plcFirstLevelContainer" runat="server">
        <cms:ContextMenuContainer runat="server" ID="cmcNew" MenuID="newMenu" Parameter="GetContextMenuParameter('nodeMenu')" CssClass="TreeContextMenu">
            <cms:ContextMenuItem runat="server" ID="iNew" ResourceString="ContentMenu.ContextIconNew" />
        </cms:ContextMenuContainer>
        <asp:Panel runat="server" ID="pnlDelete" CssClass="item-last">
            <cms:ContextMenuItem runat="server" ID="iDelete" ResourceString="general.delete" />
        </asp:Panel>
        <cms:ContextMenuSeparator runat="server" ID="pnlSep1" />
        <cms:ContextMenuItem runat="server" ID="iCopy" ResourceString="ContentMenu.IconCopy" />
        <cms:ContextMenuItem runat="server" ID="iMove" Last="true" ResourceString="ContentMenu.IconMove" />
        <cms:ContextMenuSeparator runat="server" ID="pnlSep2" />
        <cms:ContextMenuContainer runat="server" ID="cmcUp" MenuID="upMenu">
            <cms:ContextMenuItem runat="server" ID="iUp" ResourceString="ContentMenu.ContextIconMoveUp" />
        </cms:ContextMenuContainer>
        <cms:ContextMenuContainer runat="server" ID="cmcDown" MenuID="downMenu">
            <cms:ContextMenuItem runat="server" ID="iDown" ResourceString="ContentMenu.ContextIconMoveDown" />
        </cms:ContextMenuContainer>
        <cms:ContextMenuContainer runat="server" ID="cmcSort" MenuID="sortMenu">
            <cms:ContextMenuItem runat="server" ID="iSort" ResourceString="ContentMenu.IconSort" />
        </cms:ContextMenuContainer>
        <cms:ContextMenuSeparator runat="server" ID="pnlSep3" />
        <asp:PlaceHolder ID="plcAdditionalMenuItems" runat="server" />
        <asp:Panel runat="server" ID="pnlRefresh" CssClass="item-last">
            <cms:ContextMenuItem runat="server" ID="iRefresh" ResourceString="ContentMenu.IconRefresh" />
        </asp:Panel>
        <asp:PlaceHolder ID="plcProperties" runat="server">
            <cms:ContextMenuSeparator runat="server" ID="pnlSep4" />
            <cms:ContextMenuContainer runat="server" ID="cmcProperties" MenuID="propertiesMenu">
                <cms:ContextMenuItem runat="server" ID="iProperties" ResourceString="ContentMenu.IconProperties" />
            </cms:ContextMenuContainer>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
</asp:Panel>
