<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Module_Sites"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Module Edit - Sites"
     Codebehind="Sites.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading ID="headTitle" Level="4" runat="server" CssClass="listing-title" EnableViewState="false" ResourceString="Administration-Module_Edit_Sites.SiteTitle"/>
    <cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
        SelectionMode="Multiple" ResourcePrefix="sitesselect" />
</asp:Content>
