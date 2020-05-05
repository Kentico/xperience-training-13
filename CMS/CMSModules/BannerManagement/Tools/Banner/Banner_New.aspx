<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true" Inherits="CMSModules_BannerManagement_Tools_Banner_Banner_New"
    Title="New banner" Theme="Default"  Codebehind="Banner_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/BannerManagement/Controls/BannerEdit.ascx" TagName="BannerEdit" TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcContent" runat="Server">
    <cms:BannerEdit ID="bannerEdit" runat="server" IsEdit="false" />
</asp:Content>
