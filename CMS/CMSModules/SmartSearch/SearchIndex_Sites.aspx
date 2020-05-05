<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_SmartSearch_SearchIndex_Sites" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Search Index - Sites List"  Codebehind="SearchIndex_Sites.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:DisabledModule runat="server" ID="ucDisabledModule" TestSettingKeys="CMSSearchIndexingEnabled" InfoText="{$srch.searchdisabledinfo$}" />
    <asp:PlaceHolder ID="plcTable" runat="server">
        <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="SearchIndex_Sites.Available" CssClass="listing-title"
            DisplayColon="true" EnableViewState="false" />
        <cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
            SelectionMode="Multiple" ResourcePrefix="sitesselect" />
    </asp:PlaceHolder>
</asp:Content>
