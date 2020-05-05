<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_TaxClasses_TaxClass_Products"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"  CodeBehind="TaxClass_Products.aspx.cs"  %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="com.taxclass.assignedtoproducts" CssClass="listing-title" EnableViewState="false" />
    <cms:UniGrid ID="uniGrid" runat="server" GridName="~/App_Data/CMSModules/Ecommerce/UI/Grids/Ecommerce_TaxClass_Products/TaxClass_Products.xml"/>
</asp:Content> 