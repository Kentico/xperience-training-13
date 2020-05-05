<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Ecommerce_Controls_PaymentGateways_CreditPaymentForm"  Codebehind="CreditPaymentForm.ascx.cs" %>

<cms:LocalizedLabel ID="lblTitle" ResourceString="CreditPayment.lblTitle" runat="server" CssClass="BlockTitle" EnableViewState="false" />
<asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
    Visible="false" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ResourceString="CreditPayment.lblCredit" ID="lblCredit" runat="server" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <asp:Label ID="lblCreditValue" CssClass="form-control-text" runat="server" EnableViewState="false" />
        </div>
    </div>
</div>