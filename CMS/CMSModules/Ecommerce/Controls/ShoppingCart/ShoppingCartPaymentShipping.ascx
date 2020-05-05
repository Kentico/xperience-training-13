<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPaymentShipping"
     Codebehind="ShoppingCartPaymentShipping.ascx.cs" %>

<%@ Register Src="~/CMSModules/ECommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/ShippingSelector.ascx" TagName="ShippingSelector"
    TagPrefix="cms" %>

<cms:LocalizedHeading runat="server" ID="headTitle" Level="3" ResourceString="shoppingcart.shippingpaymentoptions" EnableViewState="false" />
<div class="BlockContent">
    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />    
    <div class="form-horizontal">
        <%-- Shipping --%>
        <asp:PlaceHolder ID="plcShipping" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ResourceString="shoppingcartpaymentshipping.shipping" ID="lblShipping" runat="server" EnableViewState="false" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:ShippingSelector ID="selectShipping" runat="server" AddNoneRecord="false" AddAllItemsRecord="false" UseNameForSelection="false"
                        AutoPostBack="true" />
                </div>
            </div>
        </asp:PlaceHolder>
        <%-- Payment --%>
        <asp:PlaceHolder ID="plcPayment" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ResourceString="shoppingcartpaymentshipping.payment" ID="lblPayment" runat="server" EnableViewState="false" CssClass="control-label" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:PaymentSelector ID="selectPayment" runat="server" AutoPostBack="true" AddAllItemsRecord="false" UseNameForSelection="false" OnChanged="selectPayment_Changed" />
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</div>