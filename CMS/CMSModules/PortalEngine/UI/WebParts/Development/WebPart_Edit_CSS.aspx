<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="~/CMSModules/PortalEngine/UI/WebParts/Development/WebPart_Edit_CSS.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_CSS"
    EnableEventValidation="false" Theme="Default" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/AdminControls/Controls/Preview/PreviewHierarchy.ascx"
    TagName="PreviewHierarchy" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:PreviewHierarchy ID="ucHierarchy" runat="server" CookiesPreviewStateName="wpcss"
        ContentControlPath="~/CMSModules/PortalEngine/Controls/WebParts/WebPartEditCSS.ascx"
        ShowPanelSeparator="true" />
</asp:Content>
