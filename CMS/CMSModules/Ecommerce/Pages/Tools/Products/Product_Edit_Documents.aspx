<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Documents"
    Theme="Default" Title="Product Edit - Pages" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
     Codebehind="Product_Edit_Documents.aspx.cs" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/ProductDocuments.ascx" TagName="ProductDocuments"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:ProductDocuments ID="productDocuments" runat="server" />
</asp:Content>
