<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_CssStylesheets_Pages_CssStylesheet_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Edit CSS Stylesheet"
    EnableEventValidation="false"  Codebehind="CssStylesheet_General.aspx.cs" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PreviewHierarchy ID="ucHierarchy" runat="server" CookiesPreviewStateName="css"
        ContentControlPath="~/CMSModules/CSSStyleSheets/Controls/General.ascx" />
</asp:Content>
