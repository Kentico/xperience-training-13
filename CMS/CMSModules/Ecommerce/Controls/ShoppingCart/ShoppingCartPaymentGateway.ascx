<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPaymentGateway"  Codebehind="ShoppingCartPaymentGateway.ascx.cs" %>
<asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel" Visible="false" />
<asp:Label ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel" Visible="false" />

<cms:LocalizedHeading runat="server" ID="headTitle" Level="3" ResourceString="PaymentSummary.Title" EnableViewState="false" />
<div class="BlockContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="PaymentSummary.OrderId" ID="lblOrderId" runat="server" EnableViewState="false" /></div>
            <div class="editing-form-value-cell">
                <asp:Label ID="lblOrderIdValue" runat="server" EnableViewState="false" CssClass="form-control-text" /></div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="PaymentSummary.Payment" ID="lblPayment" runat="server" EnableViewState="false" /></div>
            <div class="editing-form-value-cell">
                <asp:Label ID="lblPaymentValue" runat="server" EnableViewState="false" CssClass="form-control-text" /></div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ResourceString="PaymentSummary.TotalPrice" ID="lblTotalPrice" runat="server" EnableViewState="false" /></div>
            <div class="editing-form-value-cell">
                <asp:Label ID="lblTotalPriceValue" runat="server" EnableViewState="false" CssClass="form-control-text" /></div>
        </div>
    </div>
    <asp:Panel ID="PaymentDataContainer" runat="server" CssClass="PaymentGatewayDataContainer" />
</div>