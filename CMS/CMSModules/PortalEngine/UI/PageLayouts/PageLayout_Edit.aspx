<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Edit"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master"
    Title="Page Layout Edit"  Codebehind="PageLayout_Edit.aspx.cs" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:PreviewHierarchy ID="ucHierarchy" runat="server" 
        ContentControlPath="~/CMSModules/PortalEngine/Controls/Layout/TemplateLayoutEdit.ascx"
        ShowPanelSeparator="true" />
</asp:Content>