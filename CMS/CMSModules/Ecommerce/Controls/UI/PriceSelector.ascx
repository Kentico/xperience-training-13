<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_PriceSelector"
     Codebehind="PriceSelector.ascx.cs" %>
<%-- Price value --%>
<cms:CMSTextBox ID="txtPrice" runat="server" CssClass="form-control" />
<%-- Currency code --%>
<asp:Label ID="lblCurrencyCode" runat="server" EnableViewState="false" CssClass="form-control-text" />
