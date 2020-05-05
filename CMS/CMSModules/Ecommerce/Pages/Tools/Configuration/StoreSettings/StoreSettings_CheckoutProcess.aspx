<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_CheckoutProcess" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  Codebehind="StoreSettings_CheckoutProcess.aspx.cs" %>

<%@ Register Src="~/CMSModules/ECommerce/FormControls/CheckoutProcess.ascx" TagName="CheckoutProcess"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CheckoutProcess ID="ucCheckoutProcess" runat="server" IsLiveSite="false" />
</asp:Content>
