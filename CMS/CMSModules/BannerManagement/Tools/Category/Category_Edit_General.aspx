<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Tools_Category_Category_Edit_General"
    Title="Banner category properties" Theme="Default"  Codebehind="Category_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/BannerManagement/Controls/CategoryEdit.ascx" TagName="BannerCategoryEdit" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:BannerCategoryEdit ID="categoryEdit" runat="server" IsEdit="true" />
</asp:Content>
