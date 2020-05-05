<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartContent"
     CodeBehind="ShoppingCartContent.ascx.cs" %>
<%@ Import Namespace="CMS.Ecommerce.Web.UI" %>

<%@ Register Src="~/CMSModules/Ecommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>

<asp:Panel ID="pnlCartContent" runat="server" DefaultButton="btnUpdate">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="3" ResourceString="shoppingcart.cartcontent" EnableViewState="false" />
    <div class="content-block-50">
        <%-- Message labels --%>
        <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
        <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
            Visible="false" />
        <%-- Add item action --%>
        <asp:Panel ID="pnlNewItem" runat="server" EnableViewState="false" CssClass="form-group form-group-buttons">
            <cms:CMSButton ID="lnkNewItem" runat="server" CssClass="NewItemLink" EnableViewState="false" ButtonStyle="Primary" />
        </asp:Panel>
        <table width="100%">
            <tr>
                <%-- Currency selector --%>
                <td class="TextRight">
                    <asp:Panel ID="pnlCurrency" runat="server" CssClass="control-group-inline content-block currency-selector">
                        <cms:LocalizedLabel ResourceString="ecommerce.shoppingcartcontent.currency" ID="lblCurrency" runat="server"
                            EnableViewState="false" AssociatedControlID="selectCurrency" CssClass="form-control-text input-label" />
                        <cms:CurrencySelector ID="selectCurrency" runat="server" UseNameForSelection="false" DisplayOnlyWithExchangeRate="true"
                            AddAllItemsRecord="false" EnsureSelectedItem="true" DoFullPostback="true" RenderInline="true" AutoPostBack="true" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <%-- Cart items grid --%>
                <td>
                    <cms:UIGridView ID="gridData" runat="server" AutoGenerateColumns="false" AllowSorting="true"
                        CssClass="CartContentTable" EnableViewState="True">
                        <Columns>
                            <asp:BoundField DataField="CartItemId" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label ID="lblGuid" runat="server" Text='<%#Eval("CartItemGuid")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CartItemParentGuid" />
                            <asp:BoundField DataField="SKUID" />
                            <%-- Remove check box field --%>
                            <asp:TemplateField>
                                <ItemStyle CssClass="text-center" />
                                <ItemTemplate>
                                    <cms:CMSCheckBox ID="chkRemove" runat="server" Checked="false" Visible='<%#!GetBoolean(Eval("IsProductOption"))%>'
                                        EnableViewState="false" />
                                    <cms:LocalizedLabel ID="lblRemove" runat="server" AssociatedControlID="chkRemove"
                                        EnableViewState="false" Display="false" ResourceString="general.remove" Visible='<%#!GetBoolean(Eval("IsProductOption"))%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- Actions field --%>
                            <asp:TemplateField>
                                <ItemStyle CssClass="unigrid-actions" />
                                <ItemTemplate>
                                    <asp:Label ID="lblEditAdminButtons" Text='<%#GetOrderItemEditAction(Eval("CartItemGuid")) + GetSKUEditAction(Eval("SKUID"), Eval("SKUSiteID"), Eval("SKUParentSKUID"), Eval("IsProductOption")) %>' runat="server" EnableViewState="false" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- Product name field --%>
                            <asp:TemplateField>
                                <HeaderStyle CssClass="main-column-100" />
                                <ItemTemplate>
                                    <%#GetSKUName(Eval("SKUName"), Eval("IsProductOption"), Eval("CartItemText"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- Units field --%>
                            <asp:TemplateField>
                                <ItemStyle CssClass="text-center" />
                                <HeaderStyle CssClass="text-center" />
                                <ItemTemplate>
                                    <cms:CMSTextBox ID="txtUnits" runat="server" Text='<%#Eval("Units")%>' CssClass="input-width-20 input-number"
                                        MaxLength="9" Visible='<%#!GetBoolean(Eval("IsProductOption"))%>'
                                        EnableViewState="false" />
                                    <cms:LocalizedLabel ID="lblUnits" runat="server" AssociatedControlID="txtUnits" Display="false"
                                        EnableViewState="false" ResourceString="general.units" Visible='<%#!GetBoolean(Eval("IsProductOption"))%>' />
                                    <%#GetChildCartItemUnits(Eval("Units"), Eval("IsProductOption"))%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- Unit price field --%>
                            <asp:TemplateField>
                                <ItemStyle CssClass="text-right" />
                                <HeaderStyle CssClass="text-right wrap-nowrap" />
                                <ItemTemplate>
                                    <asp:Label ID="lblSKUPrice" runat="server" Text='<%#GetFormattedValue(Eval("UnitPrice"))%>'
                                        EnableViewState="false" Visible='<%#!GetBoolean(Eval("IsProductOption")) || GetBoolean(Eval("IsAccessoryProduct"))%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- Item discount field --%>
                            <asp:TemplateField>
                                <ItemStyle CssClass="text-right" />
                                <HeaderStyle CssClass="text-right wrap-nowrap" />
                                <ItemTemplate>
                                    <asp:Label ID="lblItemDiscount" runat="server" Text='<%#GetFormattedValue(Eval("TotalDiscount"))%>'
                                        EnableViewState="false" Visible='<%#!GetBoolean(Eval("IsProductOption")) || GetBoolean(Eval("IsAccessoryProduct"))%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- Subtotal field --%>
                            <asp:TemplateField>
                                <ItemStyle CssClass="text-right unigrid-actions" />
                                <HeaderStyle CssClass="text-right wrap-nowrap" />
                                <ItemTemplate>
                                    <asp:Label ID="lblSubtotal" CssClass="text-unigrid-action" runat="server" Text='<%#GetFormattedValue(Eval("TotalPrice"))%>'
                                        EnableViewState="false" Visible='<%#!GetBoolean(Eval("IsProductOption")) || GetBoolean(Eval("IsAccessoryProduct"))%>' />
                                    <asp:Label ID="lblPriceDetailAction" runat="server" Text='<%#GetPriceDetailLink(Eval("CartItemGuid"))%>'
                                        CssClass="ProductPriceDetailLink" ToolTip='<%#GetString("com.showpricedetail")%>' EnableViewState="false" Visible='<%#!GetBoolean(Eval("IsProductOption")) || GetBoolean(Eval("IsAccessoryProduct"))%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </cms:UIGridView>
                </td>
            </tr>
            <tr>
                <%-- Discount coupon --%>
                <td class="TextRight control-group-inline">
                    <asp:PlaceHolder ID="plcCoupon" runat="server" EnableViewState="false">
                        <cms:LocalizedLabel ResourceString="ecommerce.shoppingcartcontent.coupon" ID="lblCoupon" CssClass="form-control-text input-label"
                            AssociatedControlID="txtCoupon" runat="server" EnableViewState="false" />&nbsp;
                        <cms:CMSTextBox ID="txtCoupon" runat="server" MaxLength="200" EnableViewState="false" />
                    </asp:PlaceHolder>
                </td>
            </tr>
            <asp:PlaceHolder ID="plcCouponCodes" runat="server" EnableViewState="false">
            <tr>
                <%-- Discount coupons --%>
                <td class="TextRight control-group-inline">
                    <cms:BasicRepeater runat="server" ID="rptrCouponCodes">
                        <ItemTemplate>
                            <div class="cart-coupon-code">
                                <div class="label">
                                    <span><%# Eval<string>("Code") %></span>
                                </div>
                                <div class="button">
                                    <%# GetDiscountCouponCodeRemoveButton(Eval<string>("Code")) %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </cms:BasicRepeater>
                </td>
            </tr>
            </asp:PlaceHolder>
            <tr>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="TextLeft">
                    <asp:Panel ID="pnlPrice" runat="server" EnableViewState="false">
                        <table width="100%">
                            <%-- Order discount --%>
                            <asp:PlaceHolder ID="plcOrderDiscounts" runat="server">
                                <tr class="MultiBuyDiscount">
                                    <td class="col1">&nbsp;
                                    </td>
                                    <td style="white-space: nowrap; width: 100px;">
                                        <asp:PlaceHolder runat="server" ID="plcOrderDiscountNames" />
                                    </td>
                                    <td class="TextRight" style="white-space: nowrap; width: 100px;">
                                        <asp:PlaceHolder runat="server" ID="plcOrderDiscountValues" />
                                    </td>
                                </tr>
                            </asp:PlaceHolder>                            
                            <%-- Shipping price --%>
                            <asp:PlaceHolder ID="plcShippingPrice" runat="server">
                                <tr class="TotalShipping">
                                    <td class="col1">&nbsp;
                                    </td>
                                    <td style="white-space: nowrap; width: 100px;">
                                        <strong>
                                            <asp:Label ID="lblShippingPrice" runat="server" EnableViewState="false" />
                                        </strong>
                                    </td>
                                    <td class="TextRight" style="white-space: nowrap; width: 100px;">
                                        <strong>
                                            <asp:Label ID="lblShippingPriceValue" runat="server" EnableViewState="false" />
                                        </strong>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <%-- Tax price --%>
                            <asp:PlaceHolder ID="plcTax" runat="server">
                                <tr class="TotalTax">
                                    <td class="col1">&nbsp;
                                    </td>
                                    <td style="white-space: nowrap; width: 100px;">
                                        <strong>
                                            <asp:Label ID="tblTotalTax" runat="server" EnableViewState="false" />
                                        </strong>
                                    </td>
                                    <td class="TextRight" style="white-space: nowrap; width: 100px;">
                                        <strong>
                                            <asp:Label ID="lblTotalTaxValue" runat="server" EnableViewState="false" />
                                        </strong>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <%-- Other payments --%>
                            <asp:PlaceHolder ID="plcOtherPayments" runat="server">
                                <tr class="MultiBuyDiscount">
                                    <td class="col1">&nbsp;
                                    </td>
                                    <td style="white-space: nowrap; width: 100px;">
                                        <asp:PlaceHolder runat="server" ID="plcOtherPaymentsNames" />
                                    </td>
                                    <td class="TextRight" style="white-space: nowrap; width: 100px;">
                                        <asp:PlaceHolder runat="server" ID="plcOrderOtherPaymentsValues" />
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <%-- Total price --%>
                            <tr class="TotalPrice">
                                <td class="col1">&nbsp;
                                </td>
                                <td style="white-space: nowrap; width: 100px;">
                                    <strong>
                                        <asp:Label ID="lblTotalPrice" runat="server" EnableViewState="false" />
                                    </strong>
                                </td>
                                <td class="TextRight" style="white-space: nowrap; width: 100px;">
                                    <strong>
                                        <asp:Label ID="lblTotalPriceValue" runat="server" EnableViewState="false" />
                                    </strong>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <%-- Empty action --%>
                                <td class="col1">
                                    <cms:CMSButton ID="btnEmpty" runat="server" OnClick="btnEmpty_Click" ButtonStyle="Default"
                                        EnableViewState="false" />
                                </td>
                                <%-- Update action --%>
                                <td colspan="2" class="TextRight">
                                    <cms:CMSButton ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" ButtonStyle="Default"
                                        EnableViewState="false" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </div>
    <cms:CMSCheckBox ID="chkSendEmail" runat="server" Visible="false" EnableViewState="false" />
</asp:Panel>
