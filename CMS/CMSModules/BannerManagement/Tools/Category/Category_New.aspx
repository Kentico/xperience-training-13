<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Tools_Category_Category_New"
    Title="Banner Management - New Category" Theme="Default"  Codebehind="Category_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/BannerManagement/Controls/CategoryEdit.ascx" TagName="CategoryEdit" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CategoryEdit ID="categoryEdit" runat="server" IsEdit="false" />
</asp:Content>
