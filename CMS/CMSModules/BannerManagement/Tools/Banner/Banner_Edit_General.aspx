<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Tools_Banner_Banner_Edit_General"
    Title="Banner category properties" Theme="Default"  Codebehind="Banner_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/BannerManagement/Controls/BannerEdit.ascx" TagName="BannerEdit" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:BannerEdit ID="bannerEdit" runat="server" IsEdit="true" />
</asp:Content>
