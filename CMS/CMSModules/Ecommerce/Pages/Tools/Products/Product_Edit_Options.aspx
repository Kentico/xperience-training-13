<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Options"
    Theme="Default"  Codebehind="Product_Edit_Options.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/ProductOptions.ascx" TagName="ProductOptions"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:ProductOptions ID="ucOptions" runat="server" DisplayAddSharedCategoryButton="false" />
</asp:Content>
