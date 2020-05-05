<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_InvoiceTemplate_InvoiceTemplate_Edit"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="E-commerce Configuration - Invoice template"  Codebehind="InvoiceTemplate_Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Macros/MacroEditor.ascx" TagName="MacroEditor" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:MacroEditor runat="server" ID="invoiceTemplate" Language="HTMLMixed" IsLiveSite="False" ResolverName="EcommerceResolver" AutoSize="True" Height="600px" />
</asp:Content>
