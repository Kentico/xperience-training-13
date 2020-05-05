<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ProductOptions_ShoppingCartItemSelector"
    CodeBehind="ShoppingCartItemSelector.ascx.cs" %>
<cms:CMSUpdatePanel ID="upnlAjax" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlContainer" runat="server" CssClass="CartItemSelectorContainer">
            <asp:Panel ID="pnlSeparator" runat="server" CssClass="hr" EnableViewState="false" />
            <asp:Panel ID="pnlSelectors" runat="server" CssClass="CartItemSelector form-horizontal">
                <%-- Here come product options --%>
            </asp:Panel>
            <%-- Price --%>
            <asp:Panel ID="pnlPrice" runat="server" CssClass="TotalPriceContainer" EnableViewState="false">
                <asp:Label ID="lblPrice" runat="server" CssClass="TotalPriceLabel" EnableViewState="false" />
                <asp:Label ID="lblPriceValue" runat="server" CssClass="TotalPrice" EnableViewState="false" />
            </asp:Panel>
            <asp:Panel ID="pnlButton" runat="server" CssClass="AddToCartContainer add-to-cart-container control-group-inline" EnableViewState="false">
                <%-- Units --%>
                <cms:LocalizedLabel ID="lblUnits" runat="server" CssClass="UnitsLabel units-label form-control-text" ResourceString="ecommerce.shoppingcartcontent.skuunits" />
                <cms:CMSTextBox ID="txtUnits" runat="server" CssClass="AddToCartTextBox add-to-cart-text-box" MaxLength="9" />
                <%-- Add to cart controls --%>
                <cms:CMSButton ID="btnAdd" runat="server" ButtonStyle="Primary" CssClass="AddToCartButton add-to-cart-button" OnClick="btnAdd_Click" />
            </asp:Panel>
        </asp:Panel>
        <asp:HiddenField runat="server" ID="hdnSKUID" Value="0" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
