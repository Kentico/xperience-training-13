<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_MyDesk_Recent_Recent"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="MyDesk - Recent pages"
     Codebehind="Recent.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="Recent" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="server">
    <cms:Recent runat="server" ID="ucRecent" ListingType="RecentDocuments" IsLiveSite="false" />
</asp:Content>
