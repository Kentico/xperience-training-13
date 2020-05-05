<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_PaymentSelector"
     Codebehind="PaymentSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server"
    ObjectType="ecommerce.paymentoption" ResourcePrefix="paymentselector" ReturnColumnName="PaymentOptionID" 
    SelectionMode="SingleDropDownList" AllowEmpty="false" UseUniSelectorAutocomplete="false" />
