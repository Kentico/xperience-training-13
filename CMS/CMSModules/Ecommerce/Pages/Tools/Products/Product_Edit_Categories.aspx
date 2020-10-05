<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Product_Edit_Categories.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Categories"
    Theme="Default" %>
<%@ Register Src="~/CMSModules/Categories/Controls/MultipleCategoriesSelector.ascx"
    TagName="MultipleCategoriesSelector" TagPrefix="cms" %>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="categories.documentassignedto" CssClass="listing-title" EnableViewState="false" DisplayColon="false" />
    <cms:MultipleCategoriesSelector ID="categoriesElem" runat="server" IsLiveSite="false" />
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
