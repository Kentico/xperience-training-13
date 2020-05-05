<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_Currencies_Currency_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Currency - Edit"
     Codebehind="Currency_Edit.aspx.cs" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
   <cms:UIForm runat="server" ID="EditForm" ObjectType="ecommerce.currency" CssClass="CurrencyEditForm"
        RedirectUrlAfterCreate="Currency_Edit.aspx?currencyid={%EditedObject.CurrencyId%}&siteId={?siteId?}&saved=1" />
</asp:Content>
