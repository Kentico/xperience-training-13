<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_PaymentGateways_AuthorizeNetForm"
     Codebehind="AuthorizeNetForm.ascx.cs" %>

<div class="h4"><%= GetString("authorizenetform.title") %></div>
<asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel" Visible="false" />
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ResourceString="authorizenetform.cardnumber" ID="lblCardNumber" runat="server" ShowRequiredMark="True" EnableViewState="false" AssociatedControlID="txtCardNumber" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtCardNumber" runat="server"  MaxLength="100" EnableViewState="false" autocomplete="off" /><br />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ResourceString="authorizenetform.cardccv" ID="lblCCV" ToolTipResourceString="authorizenetform.cardccvtooltip" runat="server" AssociatedControlID="txtCCV" ShowRequiredMark="True" EnableViewState="false" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox ID="txtCCV" runat="server"  MaxLength="100" EnableViewState="false" autocomplete="off" /><br />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" ResourceString="authorizenetform.cardexpiration" ID="lblExpiration" runat="server" EnableViewState="false" ShowRequiredMark="True"  AssociatedControlID="drpMonths"/>
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList ID="drpMonths" runat="server" />
            <cms:CMSDropDownList ID="drpYears" runat="server" /><br />
        </div>
    </div>
</div>