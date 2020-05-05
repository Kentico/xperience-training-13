<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_ShippingSelector"
     Codebehind="ShippingSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" ReturnColumnName="ShippingOptionID" ObjectType="ecommerce.shippingoption" ResourcePrefix="shippingselector" ObjectSiteName="#currentsite" SelectionMode="SingleDropDownList" AllowEmpty="false" UseUniSelectorAutocomplete="false" />
