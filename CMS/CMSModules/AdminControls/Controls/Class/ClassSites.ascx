<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_ClassSites"
     Codebehind="ClassSites.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:LocalizedHeading ID="headTitle" Level="4" runat="server" CssClass="listing-title" EnableViewState="false" />
<cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
    SelectionMode="Multiple" ResourcePrefix="sitesselect" />
