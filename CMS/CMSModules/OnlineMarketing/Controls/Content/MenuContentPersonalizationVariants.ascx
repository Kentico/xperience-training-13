<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_Content_MenuContentPersonalizationVariants"
     Codebehind="MenuContentPersonalizationVariants.ascx.cs" %>

<asp:Panel runat="server" ID="pnlWebPartMenu" CssClass="PortalContextMenu WebPartContextMenu CPMenu">
<cms:ContextMenu runat="server" ID="menuWebPartCPVariants" MenuID="cpVariantList"
    VerticalPosition="Bottom" HorizontalPosition="Left" ActiveItemCssClass = "CPMenuBorderActive"
    MenuLevel="1" Dynamic="true" ShowMenuOnMouseOver="true" MouseButton="Both">
    <asp:Panel runat="server" ID="pnlAllWebPartVariants" CssClass="PortalContextMenu WebPartContextMenu">
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
</asp:Panel>