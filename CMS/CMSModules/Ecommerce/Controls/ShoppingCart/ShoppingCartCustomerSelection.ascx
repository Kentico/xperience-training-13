<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCustomerSelection"
     Codebehind="ShoppingCartCustomerSelection.ascx.cs" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/CustomerSelector.ascx" TagName="CustomerSelector"
    TagPrefix="cms" %>
<cms:LocalizedHeading runat="server" ID="headCustomer" Level="3" ResourceString="shoppingcart.selectcustomer" EnableViewState="false" />
<div class="content-block-50">
    <cms:CMSButtonGroup runat="server" ID="btnGroup" />
</div>
<%-- Select existing customer --%>
<asp:Label ID="lblSelectError" runat="server" CssClass="ErrorLabel" EnableViewState="False" Visible="False" />
<cms:CustomerSelector runat="server" ID="customerSelector" IsLiveSite="false" />
<%-- Create new customer --%>
<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<cms:UIForm runat="server" ID="customerForm" ObjectType="ecommerce.customer" AlternativeFormName="NewFromOrder" CssClass="ui-form label-width-125" RedirectUrlAfterCreate="" IsLiveSite="false" />
