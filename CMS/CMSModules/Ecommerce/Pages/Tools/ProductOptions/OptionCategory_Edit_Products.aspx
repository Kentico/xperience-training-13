<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="OptionCategory_Edit_Products.aspx.cs"
    Title="Product list" Inherits="CMSModules_Ecommerce_Pages_Tools_ProductOptions_OptionCategory_Edit_Products"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="com.productOptionCategory.pruducts" CssClass="listing-title" EnableViewState="false" />
    <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="ecommerce.sku"
        SelectionMode="Multiple" ResourcePrefix="com.selectproducts" DisplayNameFormat="{%SKUName%}"
        DynamicColumnName="false" GridName="OptionCategory_Edit_Products.xml"
        AdditionalColumns="SKUName,SKUNumber,SKUDepartmentID,SKUPrice,SKUSiteID" 
        UseDefaultNameFilter="false" FilterControl="~/CMSModules/Ecommerce/Controls/Filters/SimpleProductFilter.ascx" />
</asp:Content>

